namespace AtomUI.Controls;

internal sealed class ImageCancellationState
{
    private readonly CancellationTokenSource _source = new();
    private readonly object _gate = new();
    private bool _controllerReleased;
    private bool _operationReleased;
    private bool _canceling;
    private bool _disposed;

    internal CancellationToken Token => _source.Token;

    internal bool IsCancellationRequested => _source.IsCancellationRequested;

    internal void Cancel()
    {
        lock (_gate)
        {
            if (_disposed || _canceling)
            {
                return;
            }
            _canceling = true;
        }

        try
        {
            _source.Cancel();
        }
        catch (Exception exception) when (ImageLoadEventDispatcher.IsNonFatal(exception))
        {
            // Cancellation callbacks are outside the image state machine.
        }
        finally
        {
            lock (_gate)
            {
                _canceling = false;
                DisposeCore();
            }
        }
    }

    internal void ReleaseController()
    {
        lock (_gate)
        {
            _controllerReleased = true;
            DisposeCore();
        }
    }

    internal void ReleaseOperation()
    {
        lock (_gate)
        {
            _operationReleased = true;
            DisposeCore();
        }
    }

    private void DisposeCore()
    {
        if (!_disposed && !_canceling && _controllerReleased && _operationReleased)
        {
            _disposed = true;
            _source.Dispose();
        }
    }
}
