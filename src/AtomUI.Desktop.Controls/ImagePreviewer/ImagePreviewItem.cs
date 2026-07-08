using System.ComponentModel;

namespace AtomUI.Desktop.Controls;

public enum ImagePreviewItemState
{
    Pending,
    Loading,
    Loaded,
    Failed
}

internal sealed class ImagePreviewItem : INotifyPropertyChanged, IDisposable
{
    private long _loadVersion;
    private ImagePreviewItemState _state = ImagePreviewItemState.Pending;
    private LoadedImageSource? _loadedSource;
    private Exception? _error;

    public ImagePreviewItem(ImageSourceUri sourceUri)
    {
        SourceUri = sourceUri;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ImageSourceUri SourceUri { get; }

    public ImagePreviewItemState State
    {
        get => _state;
        private set
        {
            if (_state != value)
            {
                _state = value;
                RaisePropertyChanged(nameof(State));
                RaisePropertyChanged(nameof(IsLoading));
                RaisePropertyChanged(nameof(IsLoaded));
                RaisePropertyChanged(nameof(IsFailed));
            }
        }
    }

    public LoadedImageSource? LoadedSource
    {
        get => _loadedSource;
        private set
        {
            if (!ReferenceEquals(_loadedSource, value))
            {
                _loadedSource = value;
                RaisePropertyChanged(nameof(LoadedSource));
            }
        }
    }

    public Exception? Error
    {
        get => _error;
        private set
        {
            if (!ReferenceEquals(_error, value))
            {
                _error = value;
                RaisePropertyChanged(nameof(Error));
            }
        }
    }

    public bool IsLoading => State == ImagePreviewItemState.Loading;

    public bool IsLoaded => State == ImagePreviewItemState.Loaded;

    public bool IsFailed => State == ImagePreviewItemState.Failed;

    public long BeginLoading()
    {
        var oldSource = LoadedSource;
        LoadedSource = null;
        oldSource?.Dispose();
        Error = null;
        State = ImagePreviewItemState.Loading;
        return ++_loadVersion;
    }

    public bool CompleteLoading(long version, LoadedImageSource loadedSource)
    {
        if (version != _loadVersion)
        {
            loadedSource.Dispose();
            return false;
        }

        LoadedSource = loadedSource;
        Error        = null;
        State        = ImagePreviewItemState.Loaded;
        return true;
    }

    public bool FailLoading(long version, Exception error)
    {
        if (version != _loadVersion)
        {
            return false;
        }

        var oldSource = LoadedSource;
        LoadedSource = null;
        oldSource?.Dispose();
        Error = error;
        State = ImagePreviewItemState.Failed;
        return true;
    }

    public bool CancelLoading(long version)
    {
        if (version != _loadVersion)
        {
            return false;
        }

        LoadedSource = null;
        Error        = null;
        State        = ImagePreviewItemState.Pending;
        return true;
    }

    public void Dispose()
    {
        LoadedSource?.Dispose();
        LoadedSource = null;
    }

    private void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
