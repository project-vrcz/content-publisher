using VRChatContentPublisher.Core.AppServices;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask;
using VRChatContentPublisher.Core.Utils;

namespace VRChatContentPublisher.App.ViewModels;

public sealed class TaskErrorReportWindowViewModel(ContentPublishTaskService publishTaskService) : ViewModelBase
{
    private const string UnknownAttemptIdDisplayText = "Unknown (legacy task data)";

    public string TaskId => publishTaskService.TaskId;
    public string AttemptId => publishTaskService.AttemptId ?? UnknownAttemptIdDisplayText;

    public string ContentName => publishTaskService.ContentName;
    public string ContentType => publishTaskService.ContentType;
    public string ContentPlatform => publishTaskService.ContentPlatform;

    public string ExceptionText => publishTaskService.LastError?.ToString() ?? "No error information available.";
    public string PublishStage => publishTaskService.CurrentStage.ToString();

    public string LogFolderPath => AppStorageService.GetLogsPath();

    public string AppVersion => AppVersionUtils.GetAppVersion();
    public string AppCommitHash => AppVersionUtils.GetAppCommitHash();
    public DateTimeOffset? AppBuildDate => AppVersionUtils.GetAppBuildDate()?.ToLocalTime();
}