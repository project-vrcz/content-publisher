using System.Globalization;
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
    public string CreatedTimeRelative => ToRelativeTimeText(DateTimeOffset.Now - CreatedTime);

    public ContentPublishTaskStatus Status => publishTaskService.Status;

    private IDisposable? eventSubscription;
    private readonly DispatcherTimer createdTimeUpdateTimer =
        new(TimeSpan.FromMinutes(1), DispatcherPriority.Background, Dispatcher.UIThread);

    [RelayCommand]
    private void Load()
    {
        publishTaskService.ProgressChanged += OnTaskProgressChanged;

        eventSubscription =
            sessionStateChangedSubscriber.Subscribe(args => OnPropertyChanged(nameof(CanRetry)));
        createdTimeUpdateTimer.Tick += OnCreatedTimeUpdateTimerTick;
        createdTimeUpdateTimer.Start();

        // to fix some kind of initial state not updated
        NotifyTaskChanged();
    }

    [RelayCommand]
    private void Unload()
    {
        publishTaskService.ProgressChanged -= OnTaskProgressChanged;

        eventSubscription?.Dispose();
        eventSubscription = null;
        createdTimeUpdateTimer.Tick -= OnCreatedTimeUpdateTimerTick;
        createdTimeUpdateTimer.Stop();
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
        await taskManagerService.RemoveTaskAsync(publishTaskService.TaskId);
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

    private void OnCreatedTimeUpdateTimerTick(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(CreatedTimeRelative));
    }

    private void NotifyTaskChanged()
    {
        OnPropertyChanged(nameof(ProgressText));
        OnPropertyChanged(nameof(ProgressValue));
        OnPropertyChanged(nameof(IsIndeterminate));
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(CanRetry));
        OnPropertyChanged(nameof(CreatedTimeRelative));
    }

    private static string ToRelativeTimeText(TimeSpan elapsed)
    {
        if (elapsed < TimeSpan.Zero)
            elapsed = TimeSpan.Zero;

        var isChinese =
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals(
                "zh",
                StringComparison.OrdinalIgnoreCase
            );

        if (elapsed < TimeSpan.FromMinutes(1))
            return isChinese ? "刚刚" : "just now";

        if (elapsed < TimeSpan.FromHours(1))
        {
            var minutes = (int)Math.Floor(elapsed.TotalMinutes);
            return isChinese
                ? $"{minutes} 分钟前"
                : $"{minutes} minute{(minutes == 1 ? string.Empty : "s")} ago";
        }

        if (elapsed < TimeSpan.FromDays(1))
        {
            var hours = (int)Math.Floor(elapsed.TotalHours);
            return isChinese
                ? $"{hours} 小时前"
                : $"{hours} hour{(hours == 1 ? string.Empty : "s")} ago";
        }

        var days = (int)Math.Floor(elapsed.TotalDays);
        return isChinese
            ? $"{days} 天前"
            : $"{days} day{(days == 1 ? string.Empty : "s")} ago";
    }
}

public sealed class PublishTaskViewModelFactory(ISubscriber<SessionStateChangedEvent> sessionStateChangedSubscriber)
{
    public PublishTaskViewModel Create(
        ContentPublishTaskService publishTaskService,
        TaskManagerService taskManagerService
    ) => new(publishTaskService, taskManagerService, sessionStateChangedSubscriber);
}