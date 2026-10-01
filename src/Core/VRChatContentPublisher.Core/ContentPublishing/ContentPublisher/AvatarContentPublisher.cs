using MessagePipe;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using VRChatContentPublisher.ConnectCore.Services;
using VRChatContentPublisher.Core.ContentPublishing.ContentPublisher.Options;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Exceptions;
using VRChatContentPublisher.Core.Events.UserSession;
using VRChatContentPublisher.Core.Extensions;
using VRChatContentPublisher.Core.Resilience;
using VRChatContentPublisher.Core.Shared.Resilience;
using VRChatContentPublisher.Core.Telemetry;
using VRChatContentPublisher.Core.UserSession;
using VRChatContentPublisher.Core.Utils;
using VRChatContentPublisher.VRChatApi;
using VRChatContentPublisher.VRChatApi.ApiClient;
using VRChatContentPublisher.VRChatApi.Exceptions;
using VRChatContentPublisher.VRChatApi.Models.Rest.Avatars;
using VRChatContentPublisher.VRChatApi.Utils;

namespace VRChatContentPublisher.Core.ContentPublishing.ContentPublisher;

public sealed class AvatarContentPublisher(
    AvatarContentPublisherOptions options,
    UserSessionService userSessionService,
    ILogger<AvatarContentPublisher> logger,
    IFileService fileService,
    ISubscriber<SessionStateChangedEvent> sessionStateChangedSubscriber,
    AppResiliencePipelineBuilderFactory resiliencePipelineBuilderFactory
) : IContentPublisher
{
    internal AvatarContentPublisherOptions Options => options;

    private readonly VRChatApiClient _apiClient = userSessionService.GetApiClient();

    public string GetContentType() => "avatar";
    public string GetContentName() => options.Name;
    public string GetContentPlatform() => options.Platform;

    public bool CanPublish()
    {
        return userSessionService.State == UserSessionState.LoggedIn;
    }

    public ValueTask BeforePublishTaskAsync(string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        CancellationToken cancellationToken = default)
    {
        // Do nothing
        return ValueTask.CompletedTask;
    }

    private const long MaxBundleFileSizeForDesktopBytes = 209715200; // 200 MB
    private const long MaxBundleFileSizeForMobileBytes = 10485760; // 10 MB

    public async ValueTask PublishAsync(string bundleFileId,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        PublishStageProgressReporter progressReporter,
        CancellationToken cancellationToken = default)
    {
        using (CoreActivitySources.ContentPublishing
                   .StartActivity("AvatarContentPublisher.PublishAsync")?
                   .SetContentMetadata(
                       options.AvatarId,
                       GetContentName(),
                       GetContentType(),
                       GetContentPlatform(),
                       options.UnityVersion))
        {
            using var sessionValidScope = new EnsureSessionValidScope(
                userSessionService.UserNameOrEmail,
                sessionStateChangedSubscriber,
                cancellationToken
            );

            try
            {
                await PublishAsyncCore(
                    bundleFileId,
                    thumbnailFileId,
                    description,
                    tags,
                    releaseStatus,
                    progressReporter,
                    sessionValidScope.CancellationToken);
            }
            catch (OperationCanceledException canceledException) when (
                canceledException.CancellationToken == sessionValidScope.CancellationToken)
            {
                if (!cancellationToken.IsCancellationRequested)
                    throw new PublishingCanceledDueToSessionInvalidException(canceledException);

                throw new PublishingCanceledException(canceledException);
            }
        }
    }

    private async ValueTask PublishAsyncCore(string bundleFileId,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        PublishStageProgressReporter progressReporter,
        CancellationToken cancellationToken = default)
    {
        #region Initialzation (Get rpc file stream, check CancellationToken)

        cancellationToken.ThrowIfCancellationRequested();

        var (bundleFileStream, thumbnailFile) = await fileService.GetRpcFileStream(bundleFileId, thumbnailFileId);
        await using var stream = bundleFileStream;
        await using var thumbnailFileStream = thumbnailFile?.FileStream;

        EnsureBundleSizeRequirement(bundleFileStream);

        cancellationToken.ThrowIfCancellationRequested();

        #endregion

        #region Step.1 Fetch avatar details

        logger.LogInformation("Publish Avatar {AvatarId}", options.AvatarId);
        progressReporter.Report("Fetching avatar detail...");
        var avatar = await _apiClient.GetAvatarAsync(options.AvatarId, cancellationToken);

        #endregion

        #region Step.2 Try to get the asset file id for target platform, if not create a new one.

        logger.LogInformation(
            "Getting or creating asset bundle file for avatar {AvatarId} with platform {Platform}",
            options.AvatarId, options.Platform);
        progressReporter.Report("Getting or creating asset bundle file for target platform...");

        var fileId = await _apiClient.GetOrCreateBundleFileIdAsync(avatar.UnityPackages,
            $"Avatar - {options.Name} - Asset bundle - {options.UnityVersion}-{options.Platform}.vrca",
            options.Platform
        );

        #endregion

        #region Step.3 Upload AssetBundle - Cleanups any incomplete file versions -> Create and upload a new file version

        logger.LogInformation(
            "Creating and uploading new file version for avatar {AvatarId} with file id {FileId}",
            options.AvatarId, fileId);
        progressReporter.Report("Preparing for upload bundle file...");

        var fileVersion = await _apiClient.CreateAndUploadFileVersionAsync(
            bundleFileStream,
            fileId,
            VRChatApiFileUtils.GetMimeTypeFromExtension(".vrca"),
            "Avatar Bundle",
            arg => progressReporter.Report(arg.ProgressText, arg.ProgressValue), cancellationToken
        );

        if (fileVersion.File is null)
            throw new UnexpectedApiBehaviourException("Api did not return file info for created file version.");

        #endregion

        #region Step.4 [Optional] Upload thumbnail if needed

        string? imageUri = null;
        if (thumbnailFile is not null && thumbnailFileStream is not null)
        {
            logger.LogInformation("Uploading thumbnail for avatar {AvatarId}", options.AvatarId);

            var thumbnailFileName =
                $"Avatar - {options.Name} - Image - {options.UnityVersion}-{options.Platform}{Path.GetExtension(thumbnailFile.FileName)}";

            imageUri = await _apiClient.UploadThumbnailAsync(
                thumbnailFileStream,
                VRChatApiFileUtils.GetMimeTypeFromExtension(Path.GetExtension(thumbnailFile.FileName)),
                thumbnailFileName,
                avatar.ImageUrl,
                arg => progressReporter.Report(arg.ProgressText, arg.ProgressValue),
                cancellationToken
            );
        }

        #endregion

        #region Step.5 Update avatar metadata

        logger.LogInformation("Updating avatar {AvatarId} to use new file version {Version}", options.AvatarId,
            fileVersion.Version);
        progressReporter.Report("Updating avatar to latest asset version...");

        var updateWorldPipeline = resiliencePipelineBuilderFactory
            .CreateBuilder<bool>("CreateAvatarVersion", options.AvatarId)
            .AddRetry(new RetryStrategyOptions<bool>
            {
                ShouldHandle = new PredicateBuilder<bool>()
                    .Handle<ResilienceRequestRetryException>(),
                MaxRetryAttempts = 5
            })
            .AddFallback(new FallbackStrategyOptions<bool>
            {
                ShouldHandle = args =>
                    ValueTask.FromResult(
                        args.Outcome.Exception is not null &&
                        AppHttpClientResiliencePredicates.IsTransientHttpException(args.Outcome.Exception, null)),
                FallbackAction = async args =>
                {
                    var latestAvatar =
                        await _apiClient.GetAvatarAsync(options.AvatarId, args.Context.CancellationToken);
                    var isUpdateSucceeded = latestAvatar.UnityPackages.Any(pkg =>
                        pkg.Platform == options.Platform &&
                        pkg.UnityVersion == options.UnityVersion &&
                        pkg.AssetUrl == fileVersion.File.Url);

                    if (!isUpdateSucceeded) throw new ResilienceRequestRetryException();
                    return Outcome.FromResult(true);
                }
            })
            .Build();

        await updateWorldPipeline.ExecuteAsync(async ct =>
        {
            await _apiClient.CreateAvatarVersionAsync(options.AvatarId, new CreateAvatarVersionRequest(
                options.Name,
                fileVersion.File.Url,
                1,
                options.Platform,
                options.UnityVersion,
                imageUri,
                description,
                tags,
                releaseStatus
            ), ct);
            return true;
        }, cancellationToken);

        #endregion

        logger.LogInformation("Successfully published avatar {AvatarId}", options.AvatarId);
    }

    private void EnsureBundleSizeRequirement(Stream bundleFileStream)
    {
        if (UnityBuildTargetUtils.IsStandalonePlatform(options.Platform))
        {
            if (bundleFileStream.Length > MaxBundleFileSizeForDesktopBytes)
                throw new ArgumentException(
                    "The provided bundle file exceeds the maximum allowed size of 200 MB for this platform.");
        }
        else
        {
            if (bundleFileStream.Length > MaxBundleFileSizeForMobileBytes)
                throw new ArgumentException(
                    "The provided bundle file exceeds the maximum allowed size of 10 MB for this platform.");
        }
    }
}

public sealed class AvatarContentPublisherFactory(
    ILogger<AvatarContentPublisher> logger,
    IFileService fileService,
    ISubscriber<SessionStateChangedEvent> sessionStateChangedSubscriber,
    AppResiliencePipelineBuilderFactory resiliencePipelineBuilderFactory
)
{
    public AvatarContentPublisher Create(
        UserSessionService userSession,
        AvatarContentPublisherOptions options)
    {
        return new AvatarContentPublisher(
            options,
            userSession,
            logger,
            fileService,
            sessionStateChangedSubscriber,
            resiliencePipelineBuilderFactory
        );
    }
}