using MessagePipe;
using VRChatContentPublisher.Core.ContentPublishing.ContentPublisher;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Models;
using VRChatContentPublisher.Core.Database;
using VRChatContentPublisher.Core.Settings;
using VRChatContentPublisher.Core.Settings.Models;

namespace VRChatContentPublisher.Core.ContentPublishing.PublishTask.Services;

public sealed class TaskManagerService(
    ContentPublishTaskFactory contentPublishTaskFactory,
    ContentPublishTaskDatabaseService contentPublishTaskDatabaseService,
    IPublisher<ContentPublishTaskUpdateEventArg> taskUpdatedPublisher,
    IPublisher<ContentPublishTaskCreatedEventArg> taskCreatedPublisher,
    IPublisher<ContentPublishTaskRemovedEventArg> taskRemovedPublisher,
    IWritableOptions<AppSettings> appSettings
)
{
    public IReadOnlyDictionary<string, ContentPublishTaskService> Tasks => _tasks.AsReadOnly();
    private readonly Dictionary<string, ContentPublishTaskService> _tasks = [];

    public event EventHandler<ContentPublishTaskCreatedEventArg>? TaskCreated;
    public event EventHandler<ContentPublishTaskRemovedEventArg>? TaskRemoved;

    public event EventHandler<ContentPublishTaskUpdateEventArg>? TaskUpdated;

    /// <summary>
    /// Creates a new publish task from the given state model.
    /// A unique <see cref="ContentPublishTaskState.TaskId"/> will be generated automatically.
    /// The task state is persisted to the database immediately after creation.
    /// </summary>
    public async ValueTask<ContentPublishTaskService> CreateTask(
        ContentPublishTaskState state,
        IContentPublisher contentPublisher
    )
    {
        state.TaskId = Guid.NewGuid().ToString("D");

        var task = await contentPublishTaskFactory.CreateAsync(state, contentPublisher);

        RegisterTask(task);
        await contentPublishTaskDatabaseService.SaveStateAsync(task.State);
        return task;
    }

    /// <summary>
    /// Restores a publish task from a previously persisted state snapshot.
    /// Does not re-validate files or call <see cref="IContentPublisher.BeforePublishTaskAsync"/>,
    /// which is appropriate when resuming tasks after an application restart.
    /// The restored state is persisted to the database to update the task record.
    /// </summary>
    public async ValueTask<ContentPublishTaskService> RestoreTaskFromStateAsync(
        ContentPublishTaskState restoredState,
        IContentPublisher contentPublisher
    )
    {
        var task = await contentPublishTaskFactory.CreateFromStateAsync(restoredState, contentPublisher);

        RegisterTask(task);
        await contentPublishTaskDatabaseService.SaveStateAsync(task.State);
        return task;
    }

    private void RegisterTask(ContentPublishTaskService task)
    {
        _tasks.Add(task.TaskId, task);

        // Wire up explicit persistence: the task service actively requests
        // persistence on terminal state transitions, and we await it.
        // When AutoRemoveCompletedTasks is enabled and the task reaches Completed status,
        // the state is deleted from DB instead of saved. Intermediate states (InProgress)
        // are always saved to preserve crash recovery.
        task.PersistStateAsync = () =>
        {
            if (appSettings.Value.AutoRemoveCompletedTasks &&
                task.Status == ContentPublishTaskStatus.Completed)
                return new ValueTask(contentPublishTaskDatabaseService.DeleteStateAsync(task.State.TaskId));

            return new ValueTask(contentPublishTaskDatabaseService.SaveStateAsync(task.State));
        };

        var args = new ContentPublishTaskCreatedEventArg(task);
        TaskCreated?.Invoke(this, args);
        taskCreatedPublisher.Publish(args);

        task.ProgressChanged += TaskOnProgressChanged;
    }

    public async ValueTask RemoveTaskAsync(IEnumerable<string> taskIds)
    {
        var tasks = taskIds.Select(taskId => _tasks.GetValueOrDefault(taskId))
            .OfType<ContentPublishTaskService>()
            .Where(task => task.Status switch
            {
                ContentPublishTaskStatus.Failed => true,
                ContentPublishTaskStatus.Completed => true,
                ContentPublishTaskStatus.Canceled => true,
                ContentPublishTaskStatus.Pending => true,
                _ => false
            })
            .ToArray();

        if (tasks.Length <= 0) return;

        foreach (var task in tasks)
        {
            await task.CleanupAsync();
            _tasks.Remove(task.TaskId);
            await contentPublishTaskDatabaseService.DeleteStateAsync(task.TaskId);
        }

        var args = new ContentPublishTaskRemovedEventArg(tasks);
        TaskRemoved?.Invoke(this, args);
        taskRemovedPublisher.Publish(args);
    }

    private void TaskOnProgressChanged(object? sender, PublishTaskProgressEventArg e)
    {
        if (sender is not ContentPublishTaskService task)
            return;

        var args = new ContentPublishTaskUpdateEventArg(task, e);
        TaskUpdated?.Invoke(this, args);
        taskUpdatedPublisher.Publish(args);
    }
}

public record ContentPublishTaskUpdateEventArg(
    ContentPublishTaskService Task,
    PublishTaskProgressEventArg ProgressEventArg);

public record ContentPublishTaskCreatedEventArg(ContentPublishTaskService Task);

public record ContentPublishTaskRemovedEventArg(ContentPublishTaskService[] Tasks);