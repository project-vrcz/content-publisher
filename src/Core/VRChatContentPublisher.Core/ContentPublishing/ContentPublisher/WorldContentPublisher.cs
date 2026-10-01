using MessagePipe;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;
using VRChatContentPublisher.ConnectCore.Models;
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
using VRChatContentPublisher.VRChatApi.Models;
using VRChatContentPublisher.VRChatApi.Models.Rest.UnityPackages;
using VRChatContentPublisher.VRChatApi.Models.Rest.Worlds;
using VRChatContentPublisher.VRChatApi.Utils;

namespace VRChatContentPublisher.Core.ContentPublishing.ContentPublisher;

#pragma warning disable CS9124 // options captured into closure — expected for Options pattern
public sealed class WorldContentPublisher(
    WorldContentPublisherOptions options,
    UserSessionService userSessionService,
    ILogger<WorldContentPublisher> logger,
    IFileService fileService,
    ISubscriber<SessionStateChangedEvent> sessionStateChangedSubscriber,
    AppResiliencePipelineBuilderFactory resiliencePipelineBuilderFactory
) : IContentPublisher
{
    internal WorldContentPublisherOptions Options { get; } = options;

    private readonly string[] _udonProducts = options.UdonProducts ?? [];

    private readonly VRChatApiClient _apiClient = userSessionService.GetApiClient();

    public string GetContentType() => "world";

    public string GetContentName() => options.WorldName;
    public string GetContentPlatform() => options.Platform;

    public bool CanPublish()
    {
        return userSessionService.State == UserSessionState.LoggedIn;
    }

    public async ValueTask BeforePublishTaskAsync(
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        CancellationToken cancellationToken = default
    )
    {
        using var activity = CoreActivitySources.ContentPublishing
            .StartActivity("WorldContentPublisher.BeforePublishTaskAsync")?
            .SetContentMetadata(
                options.WorldId,
                options.WorldName,
                GetContentType(),
                options.Platform,
                options.UnityVersion);

        // try fetch world detail, if not found means we need to create a new world.
        try
        {
            await _apiClient.GetWorldAsync(options.WorldId);
            return;
        }
        catch (ApiErrorException ex) when (ex.StatusCode == 404)
        {
            logger.LogInformation("The world {WorldId} was not found. Creating new world.", options.WorldId);
        }

        logger.LogInformation("Uploading thumbnail file for creating new world {WorldId}", options.WorldId);
        if (thumbnailFileId is null)
            throw new InvalidOperationException("Thumbnail must be provided when creating a new world.");

        var thumbnailFile = await fileService.GetFileWithNameAsync(thumbnailFileId);
        await using var thumbnailFileStream = thumbnailFile?.FileStream;

        if (thumbnailFile is null || thumbnailFileStream is null)
            throw new ArgumentException("Could not find the provided thumbnail file.", nameof(thumbnailFileId));

        var thumbnailFileName =
            $"World - {options.WorldName} - Image - {options.UnityVersion}-{options.Platform}{Path.GetExtension(thumbnailFile.FileName)}";
        var imageUrl = await _apiClient.UploadThumbnailAsync(
            thumbnailFileStream,
            VRChatApiFileUtils.GetMimeTypeFromExtension(Path.GetExtension(thumbnailFile.FileName)),
            thumbnailFileName,
            null,
            arg => logger.LogInformation("Uploading thumbnail for world {WorldId}: {ProgressText} ({ProgressValue:P2})",
                options.WorldId, arg.ProgressText, arg.ProgressValue),
            cancellationToken
        );

        logger.LogInformation("Send create world request for {WorldId}", options.WorldId);
        await _apiClient.CreateWorldAsync(new CreateWorldRequest(
            options.WorldId,
            options.WorldName,
            null,
            null,
            null,
            null,
            null,
            imageUrl,
            description,
            tags,
            releaseStatus,
            options.Capacity,
            options.RecommendedCapacity,
            options.PreviewYoutubeId,
            null
        ), cancellationToken);
    }

    private const long MaxBundleFileSizeForMobileBytes = 104857600; // 100 MB

    public async ValueTask PublishAsync(
        string bundleFileId,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        PublishStageProgressReporter progressReporter,
        CancellationToken cancellationToken = default)
    {
        using (CoreActivitySources.ContentPublishing
                   .StartActivity("WorldContentPublisher.PublishAsync")?
                   .SetContentMetadata(
                       options.WorldId,
                       options.WorldName,
                       GetContentType(),
                       options.Platform,
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
                    sessionValidScope.CancellationToken
                );
            }
            catch (OperationCanceledException cancelledException) when (
                cancelledException.CancellationToken == sessionValidScope.CancellationToken)
            {
                if (!cancellationToken.IsCancellationRequested)
                    throw new PublishingCanceledDueToSessionInvalidException(cancelledException);

                throw new PublishingCanceledException(cancelledException);
            }
        }
    }

    private async ValueTask PublishAsyncCore(
        string bundleFileId,
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

        if (!UnityBuildTargetUtils.IsStandalonePlatform(options.Platform) &&
            bundleFileStream.Length > MaxBundleFileSizeForMobileBytes)
            throw new ArgumentException(
                "The provided bundle file exceeds the maximum allowed size of 100 MB for this platform.",
                nameof(bundleFileId));

        cancellationToken.ThrowIfCancellationRequested();

        #endregion

        #region Step.1 Fetch world details

        logger.LogInformation("Publish World {WorldId}", options.WorldId);
        progressReporter.Report("Fetching world detail...");

        var world = await _apiClient.GetWorldAsync(options.WorldId);

        #endregion

        #region Step.2 Try to get the asset file id for target platform, if not create a new one.

        logger.LogInformation(
            "Getting or creating asset bundle file for world {WorldId} with platform {Platform}",
            options.WorldId, options.Platform);
        progressReporter.Report("Getting or creating asset bundle file for target platform...");

        var fileId = await _apiClient.GetOrCreateBundleFileIdAsync(world.UnityPackages,
            $"World - {options.WorldName} - Asset bundle - {options.UnityVersion}-{options.Platform}.vrcw",
            options.Platform
        );

        #endregion

        #region Step.3 Upload AssetBundle - Cleanups any incomplete file versions -> Create and upload a new file version

        logger.LogInformation(
            "Creating and uploading new file version for world {WorldId} with file id {FileId}",
            options.WorldId, fileId);
        progressReporter.Report("Preparing for upload bundle file...");

        var fileVersion = await _apiClient.CreateAndUploadFileVersionAsync(
            bundleFileStream,
            fileId,
            VRChatApiFileUtils.GetMimeTypeFromExtension(".vrcw"),
            "World Bundle",
            arg => progressReporter.Report(arg.ProgressText, arg.ProgressValue), cancellationToken
        );

        if (fileVersion.File is null)
            throw new UnexpectedApiBehaviourException("Api did not return file info for created file version.");

        #endregion

        #region Step.4 [Optional] Upload thumbnail if needed

        string? imageUri = null;
        if (thumbnailFile is not null && thumbnailFileStream is not null)
        {
            logger.LogInformation("Uploading thumbnail for world {WorldId}", options.WorldId);

            var thumbnailFileName =
                $"World - {options.WorldName} - Image - {options.UnityVersion}-{options.Platform}{Path.GetExtension(thumbnailFile.FileName)}";

            imageUri = await _apiClient.UploadThumbnailAsync(
                thumbnailFileStream,
                VRChatApiFileUtils.GetMimeTypeFromExtension(Path.GetExtension(thumbnailFile.FileName)),
                thumbnailFileName,
                world.ImageUrl,
                arg => progressReporter.Report(arg.ProgressText, arg.ProgressValue),
                cancellationToken
            );
        }

        #endregion

        #region Step.5 Update world metadata

        logger.LogInformation("Updating world {WorldId} to use new file version {Version}", options.WorldId,
            fileVersion.Version);
        progressReporter?.Report("Updating world to latest asset version...");

        var updateWorldPipeline = resiliencePipelineBuilderFactory
            .CreateBuilder<bool>("CreateWorldVersion", options.WorldId)
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
                    var latestWorld = await _apiClient.GetWorldAsync(options.WorldId);
                    var isUpdateSucceeded = latestWorld.UnityPackages.Any(pkg =>
                        pkg.Platform == options.Platform &&
                        pkg.AssetVersion == fileVersion.Version &&
                        pkg.UnityVersion == options.UnityVersion &&
                        pkg.AssetUrl == fileVersion.File.Url);

                    if (!isUpdateSucceeded) throw new ResilienceRequestRetryException();
                    return Outcome.FromResult(true);
                }
            })
            .Build();

        await updateWorldPipeline.ExecuteAsync(async ct =>
        {
            await _apiClient.CreateWorldVersionAsync(options.WorldId, new CreateWorldVersionRequest(
                options.WorldName,
                fileVersion.File.Url,
                fileVersion.Version,
                options.Platform,
                options.UnityVersion,
                options.WorldSignature,
                imageUri,
                description,
                tags,
                releaseStatus,
                options.Capacity,
                options.RecommendedCapacity,
                options.PreviewYoutubeId,
                _udonProducts
            ), ct);
            return true;
        }, cancellationToken);

        #endregion

        logger.LogInformation("Successfully published world {WorldId}", options.WorldId);
    }
}

public sealed class WorldContentPublisherFactory(
    ILogger<WorldContentPublisher> logger,
    IFileService fileService,
    ISubscriber<SessionStateChangedEvent> sessionStateChangedSubscriber,
    AppResiliencePipelineBuilderFactory resiliencePipelineBuilderFactory
)
{
    public WorldContentPublisher Create(
        UserSessionService userSessionService,
        WorldContentPublisherOptions options)
    {
        return new WorldContentPublisher(
            options,
            userSessionService,
            logger,
            fileService,
            sessionStateChangedSubscriber,
            resiliencePipelineBuilderFactory
        );
    }
}