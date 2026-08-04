using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class UploadQueue
{
    private readonly Upload _owner;
    private readonly object _syncRoot = new();
    private readonly Dictionary<Guid, FileUploadTask> _tasks = new();
    private readonly FileUploadScheduler _scheduler;

    public UploadQueue(Upload owner, IFileUploadTransport? transport = null, int maxConcurrentTasks = 3)
    {
        _owner     = owner;
        _scheduler = new FileUploadScheduler(transport, maxConcurrentTasks);
    }

    internal Task SetTransportAsync(IFileUploadTransport? transport, CancellationToken cancellationToken = default)
    {
        return _scheduler.SetTransportAsync(transport, cancellationToken);
    }

    internal Task SetMaxConcurrentTasksAsync(int maxConcurrentTasks, CancellationToken cancellationToken = default)
    {
        return _scheduler.SetMaxConcurrentTasksAsync(maxConcurrentTasks, cancellationToken);
    }

    internal void Enqueue(UploadFileItem item, UploadFileInfo fileInfo)
    {
        if (_scheduler.Transport is null)
        {
            return;
        }

        var task = new FileUploadTask
        {
            Id             = item.Id,
            UploadFileInfo = fileInfo,
            Context        = item.UserData
        };

        task.UploadProgressHandler  = (_, info, progress) => _owner.NotifyUploadProgress(item, info, progress);
        task.UploadCompletedHandler = (_, info, result) => HandleCompleted(item, info, result);
        task.UploadFailedHandler    = (_, info, result) => HandleFailed(item, info, result);
        task.UploadCancelledHandler = (_, info, result) => HandleCancelled(item, info, result);

        lock (_syncRoot)
        {
            _tasks[item.Id] = task;
        }

        _scheduler.EnqueueTask(task);
    }

    internal async Task CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        FileUploadTask? task;
        lock (_syncRoot)
        {
            _tasks.TryGetValue(id, out task);
        }

        if (task is null)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        await _scheduler.CancelUploadAsync(task);
        RemoveTask(id);
    }

    internal async Task CancelAllAsync(CancellationToken cancellationToken = default)
    {
        await _scheduler.CancelAllAsync(cancellationToken);
        lock (_syncRoot)
        {
            _tasks.Clear();
        }
    }

    private void HandleCompleted(UploadFileItem item, UploadFileInfo fileInfo, FileUploadResult result)
    {
        RemoveTask(item.Id);
        _owner.NotifyUploadCompleted(item, fileInfo, result);
    }

    private void HandleFailed(UploadFileItem item, UploadFileInfo fileInfo, FileUploadResult result)
    {
        RemoveTask(item.Id);
        _owner.NotifyUploadFailed(item, fileInfo, result);
    }

    private void HandleCancelled(UploadFileItem item, UploadFileInfo fileInfo, FileUploadResult result)
    {
        RemoveTask(item.Id);
        _owner.NotifyUploadCancelled(item, fileInfo, result);
    }

    private void RemoveTask(Guid id)
    {
        lock (_syncRoot)
        {
            _tasks.Remove(id);
        }
    }
}
