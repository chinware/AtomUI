using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

internal enum ImagePreviewLoadPriority
{
    Current = 0,
    Cover = 1,
    Preload = 2
}

internal sealed class ImagePreviewLoadScheduler : IDisposable
{
    private readonly IImageSourceLoader _imageSourceLoader;
    private readonly Func<int> _maxConcurrentLoadsProvider;
    private readonly Action<ImagePreviewItem> _loadSettled;
    private readonly List<LoadRequest> _pendingRequests = [];
    private readonly List<RunningLoad> _runningLoads = [];
    private readonly object _syncRoot = new();
    private CancellationTokenSource _cancellation = new();
    private long _generation;
    private bool _disposed;

    public ImagePreviewLoadScheduler(IImageSourceLoader imageSourceLoader,
                                     Func<int> maxConcurrentLoadsProvider,
                                     Action<ImagePreviewItem> loadSettled)
    {
        _imageSourceLoader          = imageSourceLoader;
        _maxConcurrentLoadsProvider = maxConcurrentLoadsProvider;
        _loadSettled                = loadSettled;
    }

    public void Enqueue(ImagePreviewItem item, ImagePreviewLoadPriority priority)
    {
        if (_disposed ||
            item.IsLoaded ||
            item.IsLoading ||
            item.IsFailed)
        {
            return;
        }

        lock (_syncRoot)
        {
            if (_pendingRequests.Any(request => ReferenceEquals(request.Item, item)) ||
                _runningLoads.Any(load => load.Generation == _generation && ReferenceEquals(load.Item, item)))
            {
                return;
            }

            _pendingRequests.Add(new LoadRequest(item, priority, _generation));
        }

        StartPendingLoads();
    }

    public void Reset()
    {
        CancellationTokenSource oldCancellation;
        List<RunningLoad> runningLoads;
        lock (_syncRoot)
        {
            _generation++;
            _pendingRequests.Clear();
            runningLoads = _runningLoads.ToList();
            oldCancellation = _cancellation;
            _cancellation   = new CancellationTokenSource();
        }

        oldCancellation.Cancel();
        CancelRunningLoads(runningLoads);
        oldCancellation.Dispose();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        CancellationTokenSource cancellation;
        List<RunningLoad> runningLoads;
        lock (_syncRoot)
        {
            _disposed = true;
            _generation++;
            _pendingRequests.Clear();
            runningLoads = _runningLoads.ToList();
            _runningLoads.Clear();
            cancellation = _cancellation;
        }

        cancellation.Cancel();
        CancelRunningLoads(runningLoads);
        cancellation.Dispose();
    }

    private void StartPendingLoads()
    {
        if (_disposed)
        {
            return;
        }

        while (TryTakeNextRequest(out var request, out var cancellationToken))
        {
            _ = LoadItemAsync(request, cancellationToken);
        }
    }

    private bool TryTakeNextRequest(out LoadRequest request, out CancellationToken cancellationToken)
    {
        lock (_syncRoot)
        {
            var maxConcurrentLoads = Math.Max(1, _maxConcurrentLoadsProvider());
            if (_runningLoads.Count >= maxConcurrentLoads || _pendingRequests.Count == 0)
            {
                request           = default;
                cancellationToken = default;
                return false;
            }

            var requestIndex = 0;
            for (var i = 1; i < _pendingRequests.Count; i++)
            {
                if (_pendingRequests[i].Priority < _pendingRequests[requestIndex].Priority)
                {
                    requestIndex = i;
                }
            }

            request = _pendingRequests[requestIndex];
            _pendingRequests.RemoveAt(requestIndex);
            cancellationToken = _cancellation.Token;
            return true;
        }
    }

    private async Task LoadItemAsync(LoadRequest request, CancellationToken cancellationToken)
    {
        var item    = request.Item;
        var version = item.BeginLoading();
        var runningLoad = new RunningLoad(item, version, request.Generation);
        RegisterRunningLoad(runningLoad);
        try
        {
            var loadedSource = await _imageSourceLoader.LoadAsync(item.SourceUri, cancellationToken).ConfigureAwait(false);
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (IsCurrent(request.Generation) && !cancellationToken.IsCancellationRequested)
                {
                    item.CompleteLoading(version, loadedSource);
                    _loadSettled(item);
                }
                else
                {
                    loadedSource.Dispose();
                }
            });
        }
        catch (OperationCanceledException)
        {
            await Dispatcher.UIThread.InvokeAsync(() => item.CancelLoading(version));
        }
        catch (Exception ex)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (IsCurrent(request.Generation) && !cancellationToken.IsCancellationRequested)
                {
                    item.FailLoading(version, ex);
                    _loadSettled(item);
                }
            });
        }
        finally
        {
            var shouldStartNext = false;
            CompleteRunningLoad(runningLoad, ref shouldStartNext);

            if (shouldStartNext)
            {
                if (Dispatcher.UIThread.CheckAccess())
                {
                    StartPendingLoads();
                }
                else
                {
                    Dispatcher.UIThread.Post(StartPendingLoads);
                }
            }
        }
    }

    private void RegisterRunningLoad(RunningLoad runningLoad)
    {
        lock (_syncRoot)
        {
            if (!_disposed && runningLoad.Generation == _generation)
            {
                _runningLoads.Add(runningLoad);
            }
        }
    }

    private void CompleteRunningLoad(RunningLoad runningLoad, ref bool shouldStartNext)
    {
        lock (_syncRoot)
        {
            _runningLoads.Remove(runningLoad);
            shouldStartNext = !_disposed && _pendingRequests.Count > 0;
        }
    }

    private static void CancelRunningLoads(IEnumerable<RunningLoad> runningLoads)
    {
        void Cancel()
        {
            foreach (var runningLoad in runningLoads)
            {
                runningLoad.Item.CancelLoading(runningLoad.Version);
            }
        }

        if (Dispatcher.UIThread.CheckAccess())
        {
            Cancel();
        }
        else
        {
            Dispatcher.UIThread.Post(Cancel);
        }
    }

    private bool IsCurrent(long generation)
    {
        lock (_syncRoot)
        {
            return !_disposed && generation == _generation;
        }
    }

    private readonly record struct LoadRequest(ImagePreviewItem Item, ImagePreviewLoadPriority Priority, long Generation);

    private readonly record struct RunningLoad(ImagePreviewItem Item, long Version, long Generation);
}
