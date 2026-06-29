using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentPublisher.App.Localization;
using VRChatContentPublisher.App.Services;
using VRChatContentPublisher.App.ViewModels.Pages;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Models;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Services;
using VRChatContentPublisher.Core.Settings;
using VRChatContentPublisher.Core.Settings.Models;
using VRChatContentPublisher.Core.UserSession;

namespace VRChatContentPublisher.App.ViewModels.Data.PublishTasks;

public sealed partial class PublishTaskManagerViewModel(
    IWritableOptions<AppSettings> appSettings,
    UserSessionService userSessionService,
    TaskManagerService taskManagerService,
    PublishTaskViewModelFactory taskFactory,
    LoginPageViewModelFactory loginPageViewModelFactory,
    NavigationService navigationService,
    string userDisplayName)
    : ViewModelBase, IPublishTaskManagerViewModel
{
    public string UserDisplayName { get; } = userDisplayName;
    public AvaloniaList<PublishTaskViewModel> Tasks { get; } = [];

    public bool IsAnyTaskExisting => Tasks.Count > 0;
    public int TotalTaskCount => Tasks.Count;
    public int CompletedTaskCount => Tasks.Count(t => t.Status is ContentPublishTaskStatus.Completed);
    public int FailedTaskCount => Tasks.Count(t => t.Status is ContentPublishTaskStatus.Failed);
    public int CanceledTaskCount => Tasks.Count(t => t.Status is ContentPublishTaskStatus.Canceled);

    public int InProgressTaskCount => Tasks.Count(t =>
        t.Status is ContentPublishTaskStatus.InProgress or ContentPublishTaskStatus.Pending);

    public bool IsRetryAllowed => UserSessionService.State == UserSessionState.LoggedIn;

    public bool IsContentPublishAllowed =>
        UserSessionService.CurrentUser?.CanPublishAvatar() == true &&
        UserSessionService.CurrentUser?.CanPublishWorld() == true;

    [NotifyPropertyChangedFor(nameof(CurrentPageSortModeText))]
    [ObservableProperty]
    private partial AppTasksPageSortMode TasksSortMode { get; set; }

    public string CurrentPageSortModeText =>
        TasksSortMode == AppTasksPageSortMode.LatestFirst
            ? LangKeys.Pages_Settings_Appearance_How_Tasks_Order_In_Tasks_Page_Selector_Latest_First
            : LangKeys.Pages_Settings_Appearance_How_Tasks_Order_In_Tasks_Page_Selector_Oldest_First;

    public UserSessionService UserSessionService => userSessionService;

    [RelayCommand]
    private void Load()
    {
        TasksSortMode = appSettings.Value.TasksPageSortMode;

        Tasks.Clear();

        var viewModels = taskManagerService.Tasks
            .Select(task => taskFactory.Create(task.Value, taskManagerService))
            .ToArray();
        ResortTasks(viewModels);

        NotifyUserSessionChanged();
        NotifyTaskCountsChanged();

        taskManagerService.TaskCreated += OnTaskCreated;
        taskManagerService.TaskRemoved += OnTaskRemoved;
        taskManagerService.TaskUpdated += OnTaskUpdated;

        userSessionService.StateChanged += OnUserSessionStateChanged;
    }

    [RelayCommand]
    private void Unload()
    {
        taskManagerService.TaskCreated -= OnTaskCreated;
        taskManagerService.TaskRemoved -= OnTaskRemoved;
        taskManagerService.TaskUpdated -= OnTaskUpdated;

        userSessionService.StateChanged -= OnUserSessionStateChanged;

        Tasks.Clear();
    }

    [RelayCommand]
    private async Task ToggleSortMode()
    {
        TasksSortMode = TasksSortMode == AppTasksPageSortMode.LatestFirst
            ? AppTasksPageSortMode.OldestFirst
            : AppTasksPageSortMode.LatestFirst;

        await appSettings.UpdateAsync(s => s.TasksPageSortMode = TasksSortMode);
        ResortTasks();
    }

    private void ResortTasks(IEnumerable<PublishTaskViewModel>? source = null)
    {
        var tasks = source ?? Tasks;
        if (TasksSortMode == AppTasksPageSortMode.LatestFirst)
        {
            var sorted = tasks.OrderByDescending(t => t.CreatedTime).ToArray();
            Tasks.Clear();
            Tasks.AddRange(sorted);
        }
        else
        {
            var sorted = tasks.OrderBy(t => t.CreatedTime).ToArray();
            Tasks.Clear();
            Tasks.AddRange(sorted);
        }
    }

    [RelayCommand]
    private async Task RemoveCompletedTasks()
    {
        var completedTaskIds = Tasks
            .Where(t => t.Status is ContentPublishTaskStatus.Completed)
            .Select(task => task.TaskId);
        await taskManagerService.RemoveTaskAsync(completedTaskIds);
    }

    [RelayCommand]
    private async Task RemoveFailedTasks()
    {
        var failedTaskIds = Tasks
            .Where(t => t.Status is ContentPublishTaskStatus.Failed)
            .Select(task => task.TaskId);
        await taskManagerService.RemoveTaskAsync(failedTaskIds);
    }

    [RelayCommand]
    private async Task RemoveCancelledTasks()
    {
        var cancelledTaskIds = Tasks
            .Where(t => t.Status is ContentPublishTaskStatus.Canceled)
            .Select(task => task.TaskId);
        await taskManagerService.RemoveTaskAsync(cancelledTaskIds);
    }

    [RelayCommand]
    private async Task RemovePendingTasks()
    {
        var pendingTaskIds = Tasks
            .Where(t => t.Status is ContentPublishTaskStatus.Pending)
            .Select(task => task.TaskId);
        await taskManagerService.RemoveTaskAsync(pendingTaskIds);
    }

    [RelayCommand]
    private async Task RemoveAllRemovableTasks()
    {
        var removableTaskIds = Tasks
            .Where(t =>
                t.Status is ContentPublishTaskStatus.Completed or
                    ContentPublishTaskStatus.Failed or
                    ContentPublishTaskStatus.Canceled or
                    ContentPublishTaskStatus.Pending)
            .Select(task => task.TaskId);
        await taskManagerService.RemoveTaskAsync(removableTaskIds);
    }

    [RelayCommand]
    private void RetryAllTasks()
    {
        var tasks = Tasks
            .Where(task => task.Status is
                ContentPublishTaskStatus.Failed or
                ContentPublishTaskStatus.Canceled or
                ContentPublishTaskStatus.Pending)
            .ToArray();

        foreach (var task in tasks)
        {
            task.Start();
        }
    }

    [RelayCommand]
    private async Task RepairSessionAsync()
    {
        if (await userSessionService.TryRepairAsync())
            return;

        var page = loginPageViewModelFactory.Create(
            navigationService.Navigate<HomePageViewModel>,
            navigationService.Navigate<HomePageViewModel>,
            userSessionService
        );

        navigationService.Navigate(page);
    }

    private void OnTaskCreated(object? _, ContentPublishTaskCreatedEventArg args)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var task = args.Task;
            if (Tasks.Any(t => t.TaskId == task.TaskId))
                return;

            var viewModel = taskFactory.Create(task, taskManagerService);
            Tasks.Insert(0, viewModel);
            NotifyTaskCountsChanged();
        });
    }

    private void OnTaskRemoved(object? sender, ContentPublishTaskRemovedEventArg e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var taskIds = e.Tasks.Select(task => task.TaskId).Distinct().ToHashSet();
            var viewModels = Tasks.Where(viewModel => taskIds.Contains(viewModel.TaskId));
            Tasks.RemoveAll(viewModels);

            NotifyTaskCountsChanged();
        });
    }

    private void OnTaskUpdated(object? sender, ContentPublishTaskUpdateEventArg e)
    {
        Dispatcher.UIThread.Invoke(NotifyTaskCountsChanged);
    }

    private void OnUserSessionStateChanged(object? sender, UserSessionState e)
    {
        Dispatcher.UIThread.Invoke(NotifyUserSessionChanged);
    }

    private void NotifyUserSessionChanged()
    {
        OnPropertyChanged(nameof(IsRetryAllowed));
        OnPropertyChanged(nameof(IsContentPublishAllowed));
    }

    private void NotifyTaskCountsChanged()
    {
        OnPropertyChanged(nameof(TotalTaskCount));
        OnPropertyChanged(nameof(CompletedTaskCount));
        OnPropertyChanged(nameof(FailedTaskCount));
        OnPropertyChanged(nameof(CanceledTaskCount));
        OnPropertyChanged(nameof(InProgressTaskCount));
        OnPropertyChanged(nameof(IsAnyTaskExisting));
    }
}

public sealed class PublishTaskManagerViewModelFactory(
    IWritableOptions<AppSettings> appSettings,
    PublishTaskViewModelFactory taskFactory,
    LoginPageViewModelFactory loginPageViewModelFactory,
    NavigationService navigationService)
{
    public PublishTaskManagerViewModel Create(
        UserSessionService userSessionService,
        TaskManagerService taskManagerService,
        string userDisplayName
    ) => new(
        appSettings,
        userSessionService,
        taskManagerService,
        taskFactory,
        loginPageViewModelFactory,
        navigationService,
        userDisplayName
    );
}