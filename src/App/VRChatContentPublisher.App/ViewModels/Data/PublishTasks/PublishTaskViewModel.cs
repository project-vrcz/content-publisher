using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MessagePipe;
using VRChatContentPublisher.App.Views;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Models;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Services;
using VRChatContentPublisher.Core.Events.UserSession;

namespace VRChatContentPublisher.App.ViewModels.Data.PublishTasks;

public sealed partial class PublishTaskViewModel(
    ContentPublishTaskService publishTaskService,
    TaskManagerService taskManagerService,
    ISubscriber<SessionStateChangedEvent> sessionStateChangedSubscriber
) : ViewModelBase
{
    public string TaskId => publishTaskService.TaskId;

    public string ContentId => publishTaskService.ContentId;
    public string ContentName => publishTaskService.ContentName;
    public string ContentType => publishTaskService.ContentType;
    public string ContentPlatform => publishTaskService.ContentPlatform;

    public bool CanRetry => publishTaskService.CanPublish;

    public string ProgressText => publishTaskService.ProgressText;
    public double? ProgressValue => publishTaskService.ProgressValue * 100;
    public bool IsIndeterminate => !ProgressValue.HasValue;

    public DateTimeOffset CreatedTime => publishTaskService.CreatedTime;

    public ContentPublishTaskStatus Status => publishTaskService.Status;

    private IDisposable? eventSubscription;

    [RelayCommand]
    private void Load()
    {
        publishTaskService.ProgressChanged += OnTaskProgressChanged;

        eventSubscription =
            sessionStateChangedSubscriber.Subscribe(args => OnPropertyChanged(nameof(CanRetry)));

        // to fix some kind of initial state not updated
        NotifyTaskChanged();
    }

    [RelayCommand]
    private void Unload()
    {
        publishTaskService.ProgressChanged -= OnTaskProgressChanged;

        eventSubscription?.Dispose();
        eventSubscription = null;
    }

    [RelayCommand]
    private void OpenErrorReport()
    {
        var errorReportWindow = new TaskErrorReportWindowViewModel(publishTaskService);
        var window = new TaskErrorReportWindow
        {
            DataContext = errorReportWindow
        };

        window.Show();
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await publishTaskService.CancelAsync();
    }

    [RelayCommand]
    private async Task Remove()
    {
        await taskManagerService.RemoveTaskAsync([publishTaskService.TaskId]);
    }

    [RelayCommand]
    public void Start()
    {
        publishTaskService.Start();
    }

    private void OnTaskProgressChanged(object? o, PublishTaskProgressEventArg publishTaskProgressEventArg)
    {
        Dispatcher.UIThread.Invoke(NotifyTaskChanged);
    }

    private void NotifyTaskChanged()
    {
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(ProgressValue));
        OnPropertyChanged(nameof(IsIndeterminate));
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(CanRetry));
    }
}

public sealed class PublishTaskViewModelFactory(ISubscriber<SessionStateChangedEvent> sessionStateChangedSubscriber)
{
    public PublishTaskViewModel Create(
        ContentPublishTaskService publishTaskService,
        TaskManagerService taskManagerService
    ) => new(publishTaskService, taskManagerService, sessionStateChangedSubscriber);
}