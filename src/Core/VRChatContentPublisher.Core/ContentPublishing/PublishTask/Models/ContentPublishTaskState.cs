using VRChatContentPublisher.Core.ContentPublishing.ContentPublisher;
using VRChatContentPublisher.Core.UserSession;

namespace VRChatContentPublisher.Core.ContentPublishing.PublishTask.Models;

/// <summary>
/// A complete snapshot of a publish task's state, containing only serializable data.
/// Contains no runtime service references (e.g. IFileService, ILogger),
/// making it suitable for serialization/deserialization to support task state persistence and restoration.
/// Also serves as a parameter model for creating new tasks, so adding/removing fields
/// only requires changes to this class rather than updating multiple constructor signatures.
/// </summary>
public sealed class ContentPublishTaskState
{
    #region Task Identity

    /// <summary>
    /// The unique identifier of the task.
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the latest publish attempt.
    /// Null for legacy task records created before this field existed.
    /// </summary>
    public string? AttemptId { get; set; }

    #endregion

    #region Content Information

    /// <summary>
    /// The unique identifier of the content (Avatar ID or World ID).
    /// </summary>
    public string ContentId { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the content.
    /// </summary>
    public string ContentName { get; set; } = string.Empty;

    /// <summary>
    /// The content type (e.g. "avatar" or "world").
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// The target platform (e.g. "standalonewindows", "android").
    /// </summary>
    public string ContentPlatform { get; set; } = string.Empty;

    #endregion

    #region Progress

    /// <summary>
    /// The time when the task was created.
    /// </summary>
    public DateTimeOffset CreatedTime { get; set; }

    /// <summary>
    /// The last error message (serializable).
    /// </summary>
    public string? ErrorMessage { get; set; }

    #endregion

    #region Task Inner State

    /// <summary>
    /// The current publish stage.
    /// </summary>
    public PublishTaskStage CurrentStage { get; set; } = PublishTaskStage.BundleProcessing;

    /// <summary>
    /// The raw (unprocessed) bundle file ID.
    /// </summary>
    public string RawBundleFileId { get; set; } = string.Empty;

    /// <summary>
    /// The processed bundle file ID.
    /// </summary>
    public string BundleFileId { get; set; } = string.Empty;

    /// <summary>
    /// The thumbnail file ID, or null if none.
    /// </summary>
    public string? ThumbnailFileId { get; set; }

    /// <summary>
    /// The content description, or null if none.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The list of tags, or null if none.
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    /// The release status (e.g. "public", "private"), or null if none.
    /// </summary>
    public string? ReleaseStatus { get; set; }

    /// <summary>
    /// JSON-serialized publisher creation options used to reconstruct the
    /// <see cref="IContentPublisher"/> when restoring a task from persistence.
    /// </summary>
    public string? PublisherOptionsJson { get; set; }

    /// <summary>
    /// The VRChat user ID that owns this publish task.
    /// Used to locate the correct <see cref="UserSessionService"/> when restoring.
    /// </summary>
    public string? UserId { get; set; }

    #endregion

    /// <summary>
    /// Creates a default empty state instance with <see cref="CreatedTime"/> set to now.
    /// </summary>
    public ContentPublishTaskState()
    {
        CreatedTime = DateTimeOffset.Now;
    }
}
