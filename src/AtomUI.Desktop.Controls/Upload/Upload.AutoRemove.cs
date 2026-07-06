using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

public partial class Upload
{
    private void ScheduleSuccessAutoRemove(UploadFileItem item)
    {
        if (SuccessAutoRemoveDelay is null || item.Status != FileUploadStatus.Success)
        {
            return;
        }

        CancelSuccessAutoRemove(item.Id);
        var delay = SuccessAutoRemoveDelay.Value;
        var cancellationTokenSource = new CancellationTokenSource();
        _successAutoRemoveDelays[item.Id] = cancellationTokenSource;
        _ = RunSuccessAutoRemoveAsync(item.Id, delay, cancellationTokenSource.Token);
    }

    private async Task RunSuccessAutoRemoveAsync(Guid id, TimeSpan delay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            if (!cancellationToken.IsCancellationRequested)
            {
                Dispatcher.Post(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var removeTask = RemoveFileAsync(id);
                    if (!removeTask.IsCompletedSuccessfully)
                    {
                        _ = ObserveAutoRemoveTaskAsync(removeTask);
                    }
                });
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static async Task ObserveAutoRemoveTaskAsync(Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void CancelSuccessAutoRemove(Guid id)
    {
        if (!_successAutoRemoveDelays.Remove(id, out var cancellationTokenSource))
        {
            return;
        }

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }

    private void CancelAllSuccessAutoRemove()
    {
        foreach (var id in _successAutoRemoveDelays.Keys.ToArray())
        {
            CancelSuccessAutoRemove(id);
        }
    }
}
