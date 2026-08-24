using System.ComponentModel;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

internal sealed class ImagePreviewEntry : INotifyPropertyChanged, IDisposable
{
    private CancellationTokenSource? _fullCancellation;
    private CancellationTokenSource? _thumbnailCancellation;
    private ImageLoadResult? _fullResult;
    private ImageLoadResult? _thumbnailResult;
    private PixelSize? _fullRequestSize;
    private PixelSize? _thumbnailRequestSize;
    private PixelSize? _fullResultRequestSize;
    private PixelSize? _thumbnailResultRequestSize;
    private ImageRequestPriority _fullRequestPriority;
    private ImageRequestPriority _thumbnailRequestPriority;
    private long _fullGeneration;
    private long _thumbnailGeneration;
    private ImageLoadState _fullState;
    private ImageLoadState _thumbnailState;
    private ImageLoadError? _fullError;
    private ImageLoadError? _thumbnailError;
    private ImageLoadProgress? _fullProgress;
    private ImageLoadProgress? _thumbnailProgress;
    private ImageCacheSource _fullCacheSource;
    private bool _disposed;

    internal ImagePreviewEntry(ImagePreviewItem item)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal ImagePreviewItem Item { get; }

    internal IImage? FullImage => _fullResult?.Image;

    internal IImage? ThumbnailImage => _thumbnailResult?.Image ?? FullImage;

    internal ImageLoadState FullState
    {
        get => _fullState;
        private set
        {
            if (_fullState == value)
            {
                return;
            }
            _fullState = value;
            RaisePropertyChanged(nameof(FullState));
            RaisePropertyChanged(nameof(IsFullLoading));
            RaisePropertyChanged(nameof(IsFullLoaded));
            RaisePropertyChanged(nameof(IsFullFailed));
        }
    }

    internal ImageLoadState ThumbnailState
    {
        get => _thumbnailState;
        private set
        {
            if (_thumbnailState == value)
            {
                return;
            }
            _thumbnailState = value;
            RaisePropertyChanged(nameof(ThumbnailState));
            RaisePropertyChanged(nameof(IsThumbnailLoading));
            RaisePropertyChanged(nameof(IsThumbnailLoaded));
            RaisePropertyChanged(nameof(IsThumbnailFailed));
        }
    }

    internal ImageLoadError? FullError
    {
        get => _fullError;
        private set
        {
            if (_fullError == value)
            {
                return;
            }
            _fullError = value;
            RaisePropertyChanged(nameof(FullError));
        }
    }

    internal ImageLoadError? ThumbnailError
    {
        get => _thumbnailError;
        private set
        {
            if (_thumbnailError == value)
            {
                return;
            }
            _thumbnailError = value;
            RaisePropertyChanged(nameof(ThumbnailError));
        }
    }

    internal ImageLoadProgress? FullProgress
    {
        get => _fullProgress;
        private set
        {
            if (_fullProgress == value)
            {
                return;
            }
            _fullProgress = value;
            RaisePropertyChanged(nameof(FullProgress));
        }
    }

    internal ImageLoadProgress? ThumbnailProgress
    {
        get => _thumbnailProgress;
        private set
        {
            if (_thumbnailProgress == value)
            {
                return;
            }
            _thumbnailProgress = value;
            RaisePropertyChanged(nameof(ThumbnailProgress));
        }
    }

    internal bool IsFullLoading => FullState == ImageLoadState.Loading;

    internal bool IsFullLoaded => FullState == ImageLoadState.Loaded;

    internal bool IsFullFailed => FullState == ImageLoadState.Failed;

    internal bool IsThumbnailLoading => ThumbnailState == ImageLoadState.Loading;

    internal bool IsThumbnailLoaded => ThumbnailState == ImageLoadState.Loaded;

    internal bool IsThumbnailFailed => ThumbnailState == ImageLoadState.Failed;

    internal ImageCacheSource FullCacheSource => _fullCacheSource;

    internal void LoadFull(
        int decodePixelWidth,
        int decodePixelHeight,
        ImageRequestPriority priority,
        bool reload = false)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var requestSize = new PixelSize(decodePixelWidth, decodePixelHeight);
        if (!RequiresLoad(
                FullState,
                requestSize,
                _fullRequestSize,
                _fullResultRequestSize,
                priority,
                _fullRequestPriority,
                reload))
        {
            return;
        }
        var generation = ++_fullGeneration;
        Cancel(ref _fullCancellation);
        var cancellation = new CancellationTokenSource();
        _fullCancellation = cancellation;
        _fullRequestSize = requestSize;
        _fullRequestPriority = priority;
        FullError = null;
        FullProgress = null;
        FullState = ImageLoadState.Loading;
        _ = LoadFullAsync(
            generation,
            decodePixelWidth,
            decodePixelHeight,
            priority,
            reload,
            cancellation);
    }

    internal void LoadThumbnail(
        int decodePixelWidth,
        int decodePixelHeight,
        ImageRequestPriority priority,
        bool reload = false)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var requestSize = new PixelSize(decodePixelWidth, decodePixelHeight);
        if (!RequiresLoad(
                ThumbnailState,
                requestSize,
                _thumbnailRequestSize,
                _thumbnailResultRequestSize,
                priority,
                _thumbnailRequestPriority,
                reload))
        {
            return;
        }
        var generation = ++_thumbnailGeneration;
        Cancel(ref _thumbnailCancellation);
        var cancellation = new CancellationTokenSource();
        _thumbnailCancellation = cancellation;
        _thumbnailRequestSize = requestSize;
        _thumbnailRequestPriority = priority;
        ThumbnailError = null;
        ThumbnailProgress = null;
        ThumbnailState = ImageLoadState.Loading;
        _ = LoadThumbnailAsync(
            generation,
            decodePixelWidth,
            decodePixelHeight,
            priority,
            reload,
            cancellation);
    }

    internal void CancelFullLoad()
    {
        _fullGeneration++;
        Cancel(ref _fullCancellation);
        _fullRequestSize = _fullResultRequestSize;
        FullError = null;
        FullProgress = null;
        if (_fullResult is null)
        {
            FullState = ImageLoadState.Idle;
        }
        else
        {
            FullState = ImageLoadState.Loaded;
        }
    }

    internal void CancelThumbnailLoad()
    {
        _thumbnailGeneration++;
        Cancel(ref _thumbnailCancellation);
        _thumbnailRequestSize = _thumbnailResultRequestSize;
        ThumbnailError = null;
        ThumbnailProgress = null;
        if (_thumbnailResult is null)
        {
            ThumbnailState = ImageLoadState.Idle;
        }
        else
        {
            ThumbnailState = ImageLoadState.Loaded;
        }
    }

    internal void Unload()
    {
        _fullGeneration++;
        _thumbnailGeneration++;
        Cancel(ref _fullCancellation);
        Cancel(ref _thumbnailCancellation);
        Interlocked.Exchange(ref _fullResult, null)?.Dispose();
        Interlocked.Exchange(ref _thumbnailResult, null)?.Dispose();
        _fullRequestSize = null;
        _thumbnailRequestSize = null;
        _fullResultRequestSize = null;
        _thumbnailResultRequestSize = null;
        FullError = null;
        ThumbnailError = null;
        FullProgress = null;
        ThumbnailProgress = null;
        FullState = ImageLoadState.Idle;
        ThumbnailState = ImageLoadState.Idle;
        RaisePropertyChanged(nameof(FullImage));
        RaisePropertyChanged(nameof(ThumbnailImage));
    }

    internal void UnloadFull()
    {
        _fullGeneration++;
        Cancel(ref _fullCancellation);
        Interlocked.Exchange(ref _fullResult, null)?.Dispose();
        _fullRequestSize = null;
        _fullResultRequestSize = null;
        FullError = null;
        FullProgress = null;
        FullState = ImageLoadState.Idle;
        RaisePropertyChanged(nameof(FullImage));
        RaisePropertyChanged(nameof(ThumbnailImage));
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        Cancel(ref _fullCancellation);
        Cancel(ref _thumbnailCancellation);
        Interlocked.Exchange(ref _fullResult, null)?.Dispose();
        Interlocked.Exchange(ref _thumbnailResult, null)?.Dispose();
        PropertyChanged = null;
    }

    private async Task LoadFullAsync(
        long generation,
        int width,
        int height,
        ImageRequestPriority priority,
        bool reload,
        CancellationTokenSource cancellation)
    {
        try
        {
            var result = await LoadWithFallbackAsync(
                [Item.Source, Item.FallbackSource],
                width,
                height,
                priority,
                reload,
                progress => PublishFullProgress(generation, progress),
                cancellation.Token).ConfigureAwait(false);
            await Dispatcher.UIThread.InvokeAsync(() => CommitFull(
                generation,
                new PixelSize(width, height),
                result));
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }
        finally
        {
            CompleteCancellation(ref _fullCancellation, cancellation);
        }
    }

    private async Task LoadThumbnailAsync(
        long generation,
        int width,
        int height,
        ImageRequestPriority priority,
        bool reload,
        CancellationTokenSource cancellation)
    {
        try
        {
            var result = await LoadWithFallbackAsync(
                [Item.ThumbnailSource, Item.Source, Item.FallbackSource],
                width,
                height,
                priority,
                reload,
                progress => PublishThumbnailProgress(generation, progress),
                cancellation.Token).ConfigureAwait(false);
            await Dispatcher.UIThread.InvokeAsync(() => CommitThumbnail(
                generation,
                new PixelSize(width, height),
                result));
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
        }
        finally
        {
            CompleteCancellation(ref _thumbnailCancellation, cancellation);
        }
    }

    private async Task<ImageLoadResult> LoadWithFallbackAsync(
        IEnumerable<ImageLoadSource?> sources,
        int width,
        int height,
        ImageRequestPriority priority,
        bool reload,
        Action<ImageLoadProgress> reportProgress,
        CancellationToken cancellationToken)
    {
        try
        {
            var loader = (Application.Current ?? throw new InvalidOperationException(
                "ImagePreviewer requires an active Avalonia Application.")).GetImageLoader();
            var options = reload
                ? (Item.RequestOptions ?? new ImageRequestOptions()) with { CacheMode = ImageCacheMode.Reload }
                : Item.RequestOptions;
            var seen = new HashSet<string>(StringComparer.Ordinal);
            ImageLoadResult? lastFailure = null;
            foreach (var source in sources)
            {
                if (source is null || !seen.Add(source.Identity))
                {
                    continue;
                }
                lastFailure?.Dispose();
                lastFailure = await loader.LoadAsync(
                    new ImageLoadRequest(source)
                    {
                        Options = options,
                        DecodePixelWidth = width,
                        DecodePixelHeight = height,
                        Priority = priority,
                        Progress = new CallbackProgress<ImageLoadProgress>(reportProgress)
                    },
                    cancellationToken).ConfigureAwait(false);
                if (lastFailure.IsSuccess)
                {
                    return lastFailure;
                }
            }
            return lastFailure ?? new ImageLoadResult(new ImageLoadError(
                ImageLoadErrorCode.InvalidSource,
                "No image source is configured."));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new ImageLoadResult(new ImageLoadError(
                ImageLoadErrorCode.InvalidSource,
                "The image loader is not available.",
                Exception: exception));
        }
    }

    private void CommitFull(long generation, PixelSize requestSize, ImageLoadResult result)
    {
        if (_disposed || generation != _fullGeneration)
        {
            result.Dispose();
            return;
        }
        FullProgress = null;
        if (result.IsSuccess)
        {
            var previous = _fullResult;
            _fullResult = result;
            _fullResultRequestSize = requestSize;
            _fullRequestSize = requestSize;
            _fullCacheSource = result.CacheSource;
            FullError = null;
            FullState = ImageLoadState.Loaded;
            RaisePropertyChanged(nameof(FullImage));
            RaisePropertyChanged(nameof(ThumbnailImage));
            previous?.Dispose();
        }
        else
        {
            FullError = result.Error;
            FullState = ImageLoadState.Failed;
            var previous = Interlocked.Exchange(ref _fullResult, null);
            _fullResultRequestSize = null;
            RaisePropertyChanged(nameof(FullImage));
            RaisePropertyChanged(nameof(ThumbnailImage));
            previous?.Dispose();
            result.Dispose();
        }
    }

    private void CommitThumbnail(long generation, PixelSize requestSize, ImageLoadResult result)
    {
        if (_disposed || generation != _thumbnailGeneration)
        {
            result.Dispose();
            return;
        }
        ThumbnailProgress = null;
        if (result.IsSuccess)
        {
            var previous = _thumbnailResult;
            _thumbnailResult = result;
            _thumbnailResultRequestSize = requestSize;
            _thumbnailRequestSize = requestSize;
            ThumbnailError = null;
            ThumbnailState = ImageLoadState.Loaded;
            RaisePropertyChanged(nameof(ThumbnailImage));
            previous?.Dispose();
        }
        else
        {
            ThumbnailError = result.Error;
            ThumbnailState = ImageLoadState.Failed;
            var previous = Interlocked.Exchange(ref _thumbnailResult, null);
            _thumbnailResultRequestSize = null;
            RaisePropertyChanged(nameof(ThumbnailImage));
            previous?.Dispose();
            result.Dispose();
        }
    }

    private void PublishFullProgress(long generation, ImageLoadProgress progress)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (!_disposed && generation == _fullGeneration && FullState == ImageLoadState.Loading)
            {
                FullProgress = progress;
            }
        });
    }

    private void PublishThumbnailProgress(long generation, ImageLoadProgress progress)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (!_disposed && generation == _thumbnailGeneration && ThumbnailState == ImageLoadState.Loading)
            {
                ThumbnailProgress = progress;
            }
        });
    }

    private static void Cancel(ref CancellationTokenSource? cancellation)
    {
        var current = Interlocked.Exchange(ref cancellation, null);
        if (current is null)
        {
            return;
        }
        current.Cancel();
        current.Dispose();
    }

    private static void CompleteCancellation(
        ref CancellationTokenSource? field,
        CancellationTokenSource cancellation)
    {
        if (ReferenceEquals(Interlocked.CompareExchange(ref field, null, cancellation), cancellation))
        {
            cancellation.Dispose();
        }
    }

    private static bool RequiresLoad(
        ImageLoadState state,
        PixelSize requestSize,
        PixelSize? activeRequestSize,
        PixelSize? resultRequestSize,
        ImageRequestPriority priority,
        ImageRequestPriority activePriority,
        bool reload)
    {
        if (reload)
        {
            return true;
        }
        return state switch
        {
            ImageLoadState.Loading => activeRequestSize != requestSize || priority < activePriority,
            ImageLoadState.Loaded => resultRequestSize != requestSize,
            _ => true
        };
    }

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
