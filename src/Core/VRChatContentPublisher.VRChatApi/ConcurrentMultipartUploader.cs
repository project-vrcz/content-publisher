using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Microsoft.Extensions.Logging;
using Polly;
using VRChatContentPublisher.Core.Resilience;
using VRChatContentPublisher.VRChatApi.ApiClient;
using VRChatContentPublisher.VRChatApi.Exceptions;
using VRChatContentPublisher.VRChatApi.Models.Rest.Files;
using VRChatContentPublisher.VRChatApi.Telemetry;

namespace VRChatContentPublisher.VRChatApi;

public sealed class ConcurrentMultipartUploader(
    VRChatApiClient apiClient,
    HttpClient awsClient,
    ILogger<ConcurrentMultipartUploader> logger,
    Stream fileStream,
    string fileId,
    int fileVersion,
    VRChatApiFileType fileType,
    CancellationToken cancellationToken) : IDisposable
{
    public event EventHandler<(double ProgressPrcentage, long CurrentSpeedBytesPerSecond)>? ProgressChanged;

    private const long ChunkSize = 50 * 1024 * 1024;
    private const int MaxConcurrentUploads = 3;
    private readonly ConcurrentDictionary<int, string> _eTags = new();

    // Progress tracking
    private static readonly TimeSpan ProgressReportInterval = TimeSpan.FromMilliseconds(100);
    private long _completedChunkBytes;
    private readonly ConcurrentDictionary<int, long> _chunkProgress = new();

    // Speed tracking
    private readonly UploadSpeedTracker _speedTracker = new();

    public long CurrentSpeedBytesPerSecond => _speedTracker.GetCurrentSpeed();

    private ResiliencePipeline<HttpResponseMessage> CreateRetryPipeline(int partNumber)
    {
        const int maxRetryAttempts = 5;
        return new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new AppHttpRetryStrategyOptions
            {
                UseJitter = true,
                MaxRetryAttempts = maxRetryAttempts,
                Delay = TimeSpan.FromSeconds(3),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "S3 upload retry {Attempt}/{MaxAttempts} for chunk {PartNumber}, reason: {Reason}",
                        args.AttemptNumber, maxRetryAttempts, partNumber,
                        args.Outcome.Exception?.Message ??
                        args.Outcome.Result?.StatusCode.ToString() ??
                        "unknown");
                    return default;
                }
            })
            .Build();
    }

    #region Upload Logic

    public async Task<string[]> UploadAsync()
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (VRChatApiCoreActivitySources.VRChatApi.StartActivity("ConcurrentMultipartUpload")
                   ?.AddTag("fileId", fileId)
                   .AddTag("fileVersion", fileVersion)
                   .AddTag("fileSizeBytes", fileStream.Length)
                   .AddTag("chunkSizeBytes", ChunkSize)
                   .AddTag("maxConcurrency", MaxConcurrentUploads))
        using (logger.BeginScope("FileId: {FileId}, FileVersion: {FileVersion}", fileId, fileVersion))
        {
            logger.LogInformation(
                "Starting concurrent upload for file {FileId} version {FileVersion} with chunk size {ChunkSize}MB and concurrency {MaxConcurrency}",
                fileId, fileVersion, ChunkSize / 1024 / 1024, MaxConcurrentUploads);

            var progressTimer = new PeriodicTimer(ProgressReportInterval);
            var progressReportTask = Task.Run(async () =>
            {
                // ReSharper disable once AccessToDisposedClosure
                while (await progressTimer.WaitForNextTickAsync(CancellationToken.None))
                    FireProgressChanged();
            }, CancellationToken.None);

            using var concurrencySemaphore = new SemaphoreSlim(MaxConcurrentUploads);
            var uploadTasks = new List<Task>();
            var partNumber = 0;

            // Linked token source used to abort every in-flight chunk upload as soon as one chunk failed for good,
            // so a failed upload does not keep pushing the rest of the file to S3.
            using var uploadCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var uploadCancellationToken = uploadCancellationTokenSource.Token;

            // The first chunk failure. Once it has been set no further chunk upload is started.
            Exception? chunkFailure = null;

            try
            {
                while (fileStream.Position < fileStream.Length && chunkFailure is null)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    // Wait for a free slot to begin the next upload.
                    await concurrencySemaphore.WaitAsync(cancellationToken);

                    if (chunkFailure is not null)
                    {
                        // A chunk failed while waiting for a free slot, abort without starting another chunk.
                        concurrencySemaphore.Release();
                        break;
                    }

                    partNumber++;
                    var currentPartNumber = partNumber;

                    var bufferSize = (int)Math.Min(ChunkSize, fileStream.Length - fileStream.Position);
                    var buffer = new byte[bufferSize];
                    var bytesRead = await fileStream.ReadAsync(buffer, cancellationToken);

                    // Start the upload task for the current chunk.
                    // The upload cancellation token (instead of the caller token) is passed to the chunk upload, so
                    // that a failure of any chunk aborts the remaining in-flight chunks too.
                    // The token is deliberately not passed to Task.Run: the semaphore slot is released inside the
                    // task, so the task body has to run even when the upload has already been aborted.
                    var uploadTask = Task.Run(async () =>
                    {
                        try
                        {
                            await UploadChunkAsync(currentPartNumber, buffer, bytesRead, uploadCancellationToken);
                        }
                        catch (OperationCanceledException ex) when (ex.CancellationToken == uploadCancellationToken)
                        {
                            // Aborted because another chunk failed (or because the caller canceled the upload).
                            // Not a failure of this chunk, so it is not remembered as the abort reason.
                            throw;
                        }
                        catch (Exception ex)
                        {
                            // Remember the first failure and abort all remaining chunk uploads.
                            Interlocked.CompareExchange(ref chunkFailure, ex, null);
                            // ReSharper disable once AccessToDisposedClosure
                            uploadCancellationTokenSource.Cancel();
                            throw;
                        }
                        finally
                        {
                            // Release the semaphore slot once the upload is complete or has failed.
                            // ReSharper disable once AccessToDisposedClosure
                            concurrencySemaphore.Release();
                        }
                    });

                    uploadTasks.Add(uploadTask);
                }

                // Wait for all initiated upload tasks to complete. When a chunk failed, the remaining in-flight
                // chunks have been aborted and their tasks complete as canceled.
                await Task.WhenAll(uploadTasks);
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == uploadCancellationToken)
            {
                logger.LogInformation(ex, "Upload for file {FileId} version {FileVersion} was canceled", fileId,
                    fileVersion);
                throw new OperationCanceledException(
                    "Concurrent multipart upload was canceled.",
                    ex,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                // When a chunk failed, the remaining chunks were aborted and the cancellation caused by that abort
                // is not the reason of the failed upload, so surface the original chunk failure instead.
                if (chunkFailure is { } failure && !ReferenceEquals(failure, ex))
                {
                    logger.LogError(failure,
                        "Aborting upload of file {FileId} version {FileVersion} because a chunk upload failed",
                        fileId, fileVersion);
                    ExceptionDispatchInfo.Capture(failure).Throw();
                }

                logger.LogError(ex, "An error occurred during the upload of file {FileId} version {FileVersion}",
                    fileId,
                    fileVersion);
                // Rethrow to allow the caller to handle the failure.
                throw;
            }
            finally
            {
                progressTimer.Dispose();
            }

            logger.LogInformation("Successfully uploaded all parts for file {FileId} version {FileVersion}", fileId,
                fileVersion);

            // Return the ETag for each part, ordered by part number.
            var result = _eTags.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToArray();

            await progressReportTask;
            return result;
        }
    }

    private async Task UploadChunkAsync(int partNumber, byte[] buffer, int bytesRead, CancellationToken ct)
    {
        using var activity = VRChatApiCoreActivitySources.VRChatApi.StartActivity("UploadChunk")
            ?.AddTag("fileId", fileId)
            .AddTag("fileVersion", fileVersion)
            .AddTag("partNumber", partNumber)
            .AddTag("chunkSizeBytes", bytesRead);

        using (logger.BeginScope(
                   "FileId: {FileId}, FileVersion: {FileVersion}, PartNumber: {PartNumber}",
                   fileId, fileVersion, partNumber))
        {
            try
            {
                logger.LogInformation(
                    "Creating upload chunk {PartNumber} (Size: {Size:F2} MiB) for file {FileId}",
                    partNumber, (double)bytesRead / 1024 / 1024, fileId);

                // 1. Get the pre-signed URL for this part from the VRChat API (only once — URLs are reusable).
                var uploadUrl =
                    await apiClient.GetFilePartUploadUrlAsync(fileId, fileVersion, partNumber, fileType, ct);

                // 2. Upload the data to S3 with retry. On each retry attempt we reset in-flight progress
                //    for this chunk so the progress bar never double-counts bytes from a failed attempt.
                var retryPipeline = CreateRetryPipeline(partNumber);
                var finalResponse = await retryPipeline.ExecuteAsync(async innerCt =>
                {
                    using var attemptActivity =
                        VRChatApiCoreActivitySources.VRChatApi.StartActivity("UploadChunkAttempt");

                    try
                    {
                        // Reset in-flight progress for the current retry attempt.
                        _chunkProgress[partNumber] = 0;

                        using var stream = new MemoryStream(buffer, 0, bytesRead);
                        using var progressStream = new ProgressStreamContent(stream,
                            bytes => OnChunkProgress(partNumber, bytes));

                        var response = await awsClient.PutAsync(uploadUrl, progressStream, innerCt);

                        // Do not throw on a failed status code here: returning the response lets the retry pipeline
                        // decide whether the failure is transient. The final response is turned into a
                        // S3ErrorException with the parsed error response below.
                        if (!response.IsSuccessStatusCode)
                        {
                            attemptActivity?.SetStatus(ActivityStatusCode.Error,
                                $"S3 returned status code {(int)response.StatusCode} ({response.ReasonPhrase}).");
                            return response;
                        }

                        // 3. Extract the ETag from the response headers. Required by S3 to complete the multipart upload.
                        var eTag = response.Headers.ETag?.Tag.Trim('\"', '\'');
                        if (string.IsNullOrEmpty(eTag))
                        {
                            throw new InvalidOperationException(
                                $"S3 did not return an ETag for part {partNumber} of file {fileId}.");
                        }

                        _eTags[partNumber] = eTag;
                        return response;
                    }
                    catch (Exception ex)
                    {
                        attemptActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        throw;
                    }
                }, ct);

                // S3 rejected the chunk with an HTTP error response after all retry attempts, surface the error
                // details it returned.
                if (!finalResponse.IsSuccessStatusCode)
                    throw await S3ErrorException.FromResponseAsync(finalResponse, ct);

                // Mark chunk as fully completed — move bytes from in-flight to completed.
                Interlocked.Add(ref _completedChunkBytes, bytesRead);
                _chunkProgress.TryRemove(partNumber, out _);

                logger.LogInformation("Completed upload for chunk {PartNumber} for file {FileId}", partNumber, fileId);
            }
            catch (Exception e)
            {
                activity?.SetStatus(ActivityStatusCode.Error, e.Message);
                throw;
            }
        }
    }

    #endregion

    #region Progress Tracking

    private void OnChunkProgress(int partNumber, long bytes)
    {
        _chunkProgress.AddOrUpdate(partNumber, bytes, (_, prev) => prev + bytes);
        _speedTracker.RecordBytes(bytes);
    }

    private void FireProgressChanged()
    {
        if (fileStream.Length == 0) return;

        var inFlight = _chunkProgress.Values.Sum();
        var total = _completedChunkBytes + inFlight;
        var progress = Math.Min((double)total / fileStream.Length, 1.0);
        ProgressChanged?.Invoke(this, (progress, _speedTracker.GetCurrentSpeed()));
    }

    #endregion

    public void Dispose()
    {
        awsClient.Dispose();
    }
}

public sealed class ConcurrentMultipartUploaderFactory(
    ILogger<ConcurrentMultipartUploader> logger
)
{
    public ConcurrentMultipartUploader Create(
        Stream fileStream,
        string fileId,
        int fileVersion,
        VRChatApiFileType fileType,
        VRChatApiClient apiClient,
        HttpClient uploadClient,
        CancellationToken cancellationToken = default)
    {
        return new ConcurrentMultipartUploader(
            apiClient,
            uploadClient,
            logger,
            fileStream,
            fileId,
            fileVersion,
            fileType,
            cancellationToken);
    }
}