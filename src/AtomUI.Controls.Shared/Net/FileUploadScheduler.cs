using System.Collections.Concurrent;
using System.Diagnostics;

namespace AtomUI.Controls;

internal class FileUploadScheduler : IFileUploadScheduler
{
    private SemaphoreSlim _concurrentSemaphore;
    private IFileUploadTransport? _transport;
    private readonly ConcurrentQueue<FileUploadTask> _pendingQueue = new();
    private readonly ConcurrentDictionary<Guid, FileUploadTask> _runningTasks = new();
    private int _isScheduleEnabledFlag = 1;

    public IFileUploadTransport? Transport => _transport;
    
    public FileUploadScheduler(IFileUploadTransport? transport = null, int maxConcurrentTasks = 3)
    {
        _transport           = transport;
        _concurrentSemaphore = new SemaphoreSlim(maxConcurrentTasks);
    }
    
    public void EnqueueTask(FileUploadTask task)
    {
        Debug.Assert(_transport != null);
        _pendingQueue.Enqueue(task);
        _ = TryStartNextUploadAsync();
    }
    
    private async Task TryStartNextUploadAsync()
    {
        if (_transport == null || !IsScheduleEnabled())
        {
            return;
        }

        while (_concurrentSemaphore.CurrentCount > 0 && _pendingQueue.TryDequeue(out var task))
        {
            if (task.Status != FileUploadStatus.Pending)
            {
                continue;
            }
            
            await _concurrentSemaphore.WaitAsync();
            
            if (task.Status != FileUploadStatus.Pending)
            {
                _concurrentSemaphore.Release();
                continue; 
            }
            
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken       = cancellationTokenSource.Token;
            task.CancellationTokenSource = cancellationTokenSource;
            task.Status                  = FileUploadStatus.Uploading;
            _runningTasks.TryAdd(task.Id, task);
   
            Debug.Assert(task.UploadFileInfo != null);
            
            var progress = new Progress<FileUploadProgress>(report =>
            {
                task.Progress = report.Percentage;
                task.UploadProgressHandler?.Invoke(task.Id, task.UploadFileInfo, task.Progress);
            });
            
            task.ExecutionTask = Task.Run(async () =>
            {
                FileUploadResult? result = null;
                try
                {
                    result = await _transport.UploadAsync(
                        task.UploadFileInfo,
                        task.Context,
                        progress,
                        cancellationToken
                    );

                    task.Result = result;
                    task.Status = result.IsSuccess ? FileUploadStatus.Success : FileUploadStatus.Failed;
                    if (!result.IsSuccess)
                    {
                        Debug.WriteLine(
                            $"Upload failed: {task.UploadFileInfo.FilePath}, Reason: {result.UserFriendlyMessage}");
                        task.UploadFailedHandler?.Invoke(task.Id, task.UploadFileInfo, result);
                    }
                    else
                    {
                        task.UploadCompletedHandler?.Invoke(task.Id, task.UploadFileInfo, result);
                    }
                }
                catch (OperationCanceledException)
                {
                    task.Status = FileUploadStatus.Cancelled;
                    task.UploadCancelledHandler?.Invoke(task.Id, task.UploadFileInfo, FileUploadResult.CancelledResult("upload cancelled"));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Upload error: {task.UploadFileInfo.FilePath}, Error: {ex.Message}");
                    task.Status = FileUploadStatus.Failed;
                    task.UploadFailedHandler?.Invoke(task.Id, task.UploadFileInfo, FileUploadResult.FailureResult(FileUploadErrorCode.Unknown, ex.Message));
                }
                finally
                {
                    _runningTasks.TryRemove(task.Id, out _);
                    _concurrentSemaphore.Release();
                    task.CancellationTokenSource?.Dispose();
                    task.CancellationTokenSource = null;
                    task.ExecutionTask = null;
                    _ = TryStartNextUploadAsync();
                }
            }, cancellationToken);
        }
    }
    
    public async Task CancelUploadAsync(FileUploadTask task)
    {
        if (task.Status == FileUploadStatus.Uploading)
        {
            if (task.CancellationTokenSource != null)
            {
                await task.CancellationTokenSource.CancelAsync();
            }
        }
        else if (task.Status == FileUploadStatus.Pending)
        {
            task.Status = FileUploadStatus.Cancelled;
            task.UploadCancelledHandler?.Invoke(task.Id,
                task.UploadFileInfo!,
                FileUploadResult.CancelledResult("upload cancelled"));
        }
    }
    
    public async Task CancelAllAsync(CancellationToken cancellationToken = default)
    {
        var wasScheduleEnabled = IsScheduleEnabled();
        DisableSchedule();

        while (_pendingQueue.TryDequeue(out var pendingTask))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (pendingTask.Status == FileUploadStatus.Pending)
            {
                pendingTask.Status = FileUploadStatus.Cancelled;
                pendingTask.UploadCancelledHandler?.Invoke(pendingTask.Id,
                    pendingTask.UploadFileInfo!,
                    FileUploadResult.CancelledResult("upload cancelled"));
            }
        }

        var runningTasks = _runningTasks.Values.ToArray();
        foreach (var task in runningTasks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await CancelUploadAsync(task);
        }

        var executionTasks = runningTasks
                             .Select(task => task.ExecutionTask)
                             .Where(task => task != null)
                             .Cast<Task>()
                             .ToArray();
        if (executionTasks.Length > 0)
        {
            await Task.WhenAll(executionTasks).WaitAsync(cancellationToken);
        }

        if (wasScheduleEnabled)
        {
            EnableSchedule();
        }
    }

    public async Task SetMaxConcurrentTasksAsync(int taskCount, CancellationToken cancellationToken = default)
    {
        await CancelAllAsync(cancellationToken);
        _concurrentSemaphore.Dispose();
        _concurrentSemaphore = new SemaphoreSlim(taskCount);
        EnableSchedule();
    }

    public async Task SetTransportAsync(IFileUploadTransport transport, CancellationToken cancellationToken = default)
    {
        await CancelAllAsync(cancellationToken);
        _transport = transport;
        EnableSchedule();
    }

    public void DisableSchedule()
    {
        Interlocked.Exchange(ref _isScheduleEnabledFlag, 0);
    }

    public void EnableSchedule()
    {
        Interlocked.Exchange(ref _isScheduleEnabledFlag, 1);
    }

    public bool IsScheduleEnabled()
    {
        return Interlocked.CompareExchange(ref _isScheduleEnabledFlag, 1, 1) == 1;
    }
}
