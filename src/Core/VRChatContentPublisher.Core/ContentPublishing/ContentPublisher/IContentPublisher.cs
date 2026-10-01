using VRChatContentPublisher.Core.ContentPublishing.PublishTask;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Exceptions;

namespace VRChatContentPublisher.Core.ContentPublishing.ContentPublisher;

public interface IContentPublisher
{
    string GetContentType();
    string GetContentName();
    string GetContentPlatform();

    bool CanPublish();

    ValueTask BeforePublishTaskAsync(
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Publish content
    /// </summary>
    /// <exception cref="PublishingCanceledDueToSessionInvalidException">Publishing was canceled due to session invalid</exception>
    /// <exception cref="PublishingCanceledException">Publishing was canceled</exception>
    ValueTask PublishAsync(
        string bundleFileId,
        string? thumbnailFileId,
        string? description,
        string[]? tags,
        string? releaseStatus,
        PublishStageProgressReporter progressReporter,
        CancellationToken cancellationToken = default
    );
}