using System.Diagnostics;
using System.Security.Cryptography;
using Avalonia.Threading;
using Downloader;
using Microsoft.Extensions.Logging;
using VRChatContentPublisher.App.Models.Update;
using VRChatContentPublisher.App.Services.AppLifetime;
using VRChatContentPublisher.Core.Shared;
using VRChatContentPublisher.Platform.Abstraction.Services;

namespace VRChatContentPublisher.App.Services.Update;

public sealed class AppUpdateService(
    IUpdateInstallationService updateInstallationService,
    IHttpClientFactory httpClientFactory,
    ILogger<AppUpdateService> logger,
    AppLifetimeService lifetimeService
)
{
    public bool IsAppUpdateSupported() => updateInstallationService.IsUpdateInstallationSupported();

    private readonly Lock _updateStateLock = new();
    public AppUpdateServiceState UpdateState { get; private set; } = new AppUpdateServiceState.Idle();
    public event EventHandler<AppUpdateServiceState>? OnUpdateStateChanged;

    public AppUpdateInformation? UpdateInformation => UpdateState switch
    {
        AppUpdateServiceState.Downloading downloading => downloading.UpdateInformation,
        AppUpdateServiceState.DownloadError downloadError => downloadError.UpdateInformation,
        AppUpdateServiceState.IntegrityCheckFailed integrityCheckFailed => integrityCheckFailed.UpdateInformation,
        AppUpdateServiceState.WaitingForInstall waitingForInstall => waitingForInstall.UpdateInformation,
        AppUpdateServiceState.InstallError installError => installError.UpdateInformation,
        AppUpdateServiceState.Idle => null
    };

    public Exception? LastException => UpdateState switch
    {
        AppUpdateServiceState.DownloadError downloadError => downloadError.DownloadErrorException,
        AppUpdateServiceState.IntegrityCheckFailed integrityCheckFailed => new UpdateFileIntegrityCheckFailedException(
            integrityCheckFailed.DownloadedFileSha256, integrityCheckFailed.ExceptedFileSha256),
        AppUpdateServiceState.InstallError installError => installError.InstallErrorException,
        _ => null
    };

    #region Download

    public void StartDownloadUpdate(AppUpdateInformation updateInformation)
    {
        if (!IsAppUpdateSupported())
            throw new NotSupportedException("Update are not supported for this platform");

        CleanupAndEnterIdle();
        if (!updateInformation.Platforms
                .TryGetValue(updateInstallationService.GetPlatformIdentifier(), out var platformInformation))
        {
            throw new ArgumentException(
                $"Platform {updateInstallationService.GetPlatformIdentifier()} is not supported for this update");
        }

        logger.LogInformation("Starting download update {Version} Sha256: {Sha256} Url: {DownloadUrl}",
            updateInformation.Version, platformInformation.Sha256, platformInformation.Url);

        var downloadCts = new CancellationTokenSource();
        var pathToDownloadFile = Path.Combine(AppStorageService.GetTempPath(), "update", "package");
        var downloadStateInfo = new AppUpdateServiceState.Downloading
        {
            UpdateInformation = updateInformation,
            DownloadCancellationTokenSource = downloadCts,
            PathToDownloadedFile = pathToDownloadFile
        };

        OnOnUpdateStateChanged(downloadStateInfo);

        var cancellationToken = downloadCts.Token;

        var downloadTask = DownloadBuilder.New()
            .WithUrl(platformInformation.Url)
            .WithFileLocation(pathToDownloadFile)
            .WithConfiguration(new DownloadConfiguration
            {
                ParallelCount = 8,
                ParallelDownload = true,
                EnableAutoResumeDownload = false
            })
            .WithHttpClient(() => httpClientFactory.CreateClient(nameof(AppUpdateService)))
            .Build();

        downloadTask.DownloadProgressChanged += (_, args) =>
        {
            downloadStateInfo.BytesPerSecondSpeed = args.BytesPerSecondSpeed;
            downloadStateInfo.TotalFileSize = downloadTask.TotalFileSize;
            downloadStateInfo.DownloadedFileSize = downloadTask.DownloadedFileSize;
        };

        var downloadResultTcs = new TaskCompletionSource();
        downloadTask.DownloadFileCompleted += (_, args) =>
        {
            if (args.Error is { } ex)
            {
                downloadResultTcs.TrySetException(ex);
                return;
            }

            if (args.Cancelled)
            {
                downloadResultTcs.TrySetCanceled();
                return;
            }

            downloadResultTcs.TrySetResult();
        };

        _ = Task.Run(async () =>
        {
            var downloadResultTask = downloadResultTcs.Task;
            try
            {
                await downloadTask.StartAsync(cancellationToken);
                await downloadResultTask;

                if (cancellationToken.IsCancellationRequested)
                    return;

                await using var fileStream = File.OpenRead(pathToDownloadFile);
                var fileSha256 = await ComputeSha256Async(fileStream, cancellationToken);
                var remoteSha256 = platformInformation.Sha256;

                if (!string.Equals(fileSha256, remoteSha256, StringComparison.OrdinalIgnoreCase))
                    throw new UpdateFileIntegrityCheckFailedException(fileSha256, remoteSha256);
            }
            catch (UpdateFileIntegrityCheckFailedException ex)
            {
                logger.LogError(
                    "Downloaded file sha256 didn't match remote sha256, Remote: {RemoteSha256} Local: {FileSha256}",
                    ex.RemoteSha256, ex.LocalSha256
                );

                TryDeleteFile(pathToDownloadFile);
                OnOnUpdateStateChanged(new AppUpdateServiceState.IntegrityCheckFailed
                {
                    UpdateInformation = updateInformation,
                    DownloadedFileSha256 = ex.LocalSha256,
                    ExceptedFileSha256 = ex.RemoteSha256
                });

                return;
            }
            catch (OperationCanceledException)
                // Downloader won't ensure the exception throw with our CancellationToken,
                // so ignore check exception CancellationToken property
                when (cancellationToken.IsCancellationRequested)
            {
                logger.LogInformation("Download was canceled");
                TryDeleteFile(pathToDownloadFile);
                CleanupAndEnterIdle();

                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to download update");
                TryDeleteFile(pathToDownloadFile);

                OnOnUpdateStateChanged(new AppUpdateServiceState.DownloadError
                {
                    UpdateInformation = updateInformation,
                    DownloadErrorException = ex
                });

                return;
            }
            finally
            {
                await downloadTask.DisposeAsync();
            }

            logger.LogInformation("Update downloaded and waiting for install");
            OnOnUpdateStateChanged(new AppUpdateServiceState.WaitingForInstall
            {
                UpdateInformation = updateInformation,
                PathToDownloadedFile = pathToDownloadFile
            });
        }, cancellationToken).ConfigureAwait(false);
    }

    private static async ValueTask<string> ComputeSha256Async(
        Stream stream, CancellationToken cancellationToken = default
    )
    {
        using var sha256 = SHA256.Create();
        var fileHashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);
        var fileHash = Convert.ToHexStringLower(fileHashBytes);

        return fileHash;
    }

    #endregion

    public async ValueTask InstallUpdateAsync()
    {
        string pathToDownloadedFile;
        AppUpdateInformation updateInformation;

        switch (UpdateState)
        {
            case AppUpdateServiceState.WaitingForInstall waitingForInstallState:
                pathToDownloadedFile = waitingForInstallState.PathToDownloadedFile;
                updateInformation = waitingForInstallState.UpdateInformation;
                break;
            case AppUpdateServiceState.InstallError installErrorState:
                pathToDownloadedFile = installErrorState.PathToDownloadedFile;
                updateInformation = installErrorState.UpdateInformation;
                break;
            default:
                throw new InvalidOperationException("Update Service not in WaitingForInstall or Install state");
        }

        try
        {
            await updateInstallationService.InstallUpdateAsync(pathToDownloadedFile);
            Dispatcher.UIThread.Invoke(lifetimeService.Shutdown);
        }
        catch (Exception ex)
        {
            OnOnUpdateStateChanged(new AppUpdateServiceState.InstallError
            {
                UpdateInformation = updateInformation,
                InstallErrorException = ex,
                PathToDownloadedFile = pathToDownloadedFile
            });

            throw;
        }
    }

    public async ValueTask RetryUpdateAsync()
    {
        switch (UpdateState)
        {
            case AppUpdateServiceState.DownloadError downloadErrorState:
                StartDownloadUpdate(downloadErrorState.UpdateInformation);
                break;
            case AppUpdateServiceState.IntegrityCheckFailed integrityCheckFailedState:
                StartDownloadUpdate(integrityCheckFailedState.UpdateInformation);
                break;
            case AppUpdateServiceState.InstallError:
                await InstallUpdateAsync();
                break;
            default:
                throw new InvalidOperationException("Update Service not in any error state");
        }
    }

    public async ValueTask CancelUpdateAsync()
    {
        logger.LogInformation("Canceling update");

        if (UpdateState is AppUpdateServiceState.Downloading downloadingState)
        {
            await downloadingState.DownloadCancellationTokenSource.CancelAsync();
        }

        CleanupAndEnterIdle();
    }

    private void CleanupAndEnterIdle()
    {
        string? pathToDownloadFile = null;
        switch (UpdateState)
        {
            // Download method will handle file cleanup
            case AppUpdateServiceState.InstallError installErrorState:
                pathToDownloadFile = installErrorState.PathToDownloadedFile;
                break;
            case AppUpdateServiceState.WaitingForInstall waitingForInstallState:
                pathToDownloadFile = waitingForInstallState.PathToDownloadedFile;
                break;
        }

        if (pathToDownloadFile is not null) TryDeleteFile(pathToDownloadFile);
        OnOnUpdateStateChanged(new AppUpdateServiceState.Idle());
    }

    private void TryDeleteFile(string filePath)
    {
        if (!File.Exists(filePath)) return;

        try
        {
            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cleanup downloaded file");
        }
    }

    private void OnOnUpdateStateChanged(AppUpdateServiceState e)
    {
        lock (_updateStateLock)
        {
            UpdateState = e;
        }

        OnUpdateStateChanged?.Invoke(this, e);
    }
}

public sealed class UpdateFileIntegrityCheckFailedException(string localSha256, string remoteSha256)
    : Exception(
        $"Download file has different sha256 compare to remote metadata, local: {localSha256}, remote: {remoteSha256}"
    )
{
    public string LocalSha256 => localSha256;
    public string RemoteSha256 => remoteSha256;
}