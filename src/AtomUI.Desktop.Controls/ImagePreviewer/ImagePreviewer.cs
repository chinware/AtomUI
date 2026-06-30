using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using Avalonia.Threading;
using System.ComponentModel;

namespace AtomUI.Desktop.Controls;

public class ImagePreviewer : AbstractImagePreviewer
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> CoverIndicatorContentProperty =
        AvaloniaProperty.Register<ImagePreviewer, object?>(nameof(CoverIndicatorContent));
    
    public static readonly StyledProperty<IDataTemplate?> CoverIndicatorContentTemplateProperty =
        AvaloniaProperty.Register<ImagePreviewer, IDataTemplate?>(nameof(CoverIndicatorContentTemplate));
    
    public static readonly StyledProperty<ImageSourceUri?> CoverSourceUriProperty =
        AvaloniaProperty.Register<ImagePreviewer, ImageSourceUri?>(nameof(CoverSourceUri));
    
    public static readonly StyledProperty<bool> IsShowCoverMaskProperty =
        AvaloniaProperty.Register<ImagePreviewer, bool>(nameof(IsShowCoverMask), true);
    
    public ImageSourceUri? CoverSourceUri
    {
        get => GetValue(CoverSourceUriProperty);
        set => SetValue(CoverSourceUriProperty, value);
    }
    
    [DependsOn(nameof(CoverIndicatorContentTemplate))]
    public object? CoverIndicatorContent
    {
        get => GetValue(CoverIndicatorContentProperty);
        set => SetValue(CoverIndicatorContentProperty, value);
    }
    
    public IDataTemplate? CoverIndicatorContentTemplate
    {
        get => GetValue(CoverIndicatorContentTemplateProperty);
        set => SetValue(CoverIndicatorContentTemplateProperty, value);
    }
    
    public bool IsShowCoverMask
    {
        get => GetValue(IsShowCoverMaskProperty);
        set => SetValue(IsShowCoverMaskProperty, value);
    }
    #endregion
    
    #region 内部属性定义
    
    internal static readonly DirectProperty<ImagePreviewer, LoadedImageSource?> EffectiveCoverImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, LoadedImageSource?>(
            nameof(EffectiveCoverImage),
            o => o.EffectiveCoverImage,
            (o, v) => o.EffectiveCoverImage = v);

    internal static readonly DirectProperty<ImagePreviewer, bool> IsCoverImageLoadingProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, bool>(
            nameof(IsCoverImageLoading),
            o => o.IsCoverImageLoading,
            (o, v) => o.IsCoverImageLoading = v);

    internal static readonly DirectProperty<ImagePreviewer, bool> IsCoverImageFailedProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, bool>(
            nameof(IsCoverImageFailed),
            o => o.IsCoverImageFailed,
            (o, v) => o.IsCoverImageFailed = v);
    
    private LoadedImageSource? _effectiveCoverImage;

    internal LoadedImageSource? EffectiveCoverImage
    {
        get => _effectiveCoverImage;
        set => SetAndRaise(EffectiveCoverImageProperty, ref _effectiveCoverImage, value);
    }

    private bool _isCoverImageLoading;

    internal bool IsCoverImageLoading
    {
        get => _isCoverImageLoading;
        set => SetAndRaise(IsCoverImageLoadingProperty, ref _isCoverImageLoading, value);
    }

    private bool _isCoverImageFailed;

    internal bool IsCoverImageFailed
    {
        get => _isCoverImageFailed;
        set => SetAndRaise(IsCoverImageFailedProperty, ref _isCoverImageFailed, value);
    }
    #endregion

    private ImagePreviewItem? _coverItem;
    private bool _ownsCoverItem;
    private string? _failedCoverSourceKey;
    private CancellationTokenSource? _coverLoadCancellation;
    
    static ImagePreviewer()
    {
        AffectsRender<ImagePreviewer>(CoverSourceUriProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CoverSourceUriProperty)
        {
            _failedCoverSourceKey = null;
            ConfigureEffectiveCoverItem();
        }
        
        else if (change.Property == EffectiveItemsProperty ||
                 change.Property == CurrentIndexProperty)
        {
            ConfigureEffectiveCoverItem();
        }
    }

    private protected override void HandleSourceChanged()
    {
        MaterializeEffectiveItemsFromSourceUris();
        ConfigureEffectiveCoverItem();
    }

    private protected override void HandleFallbackSourceChanged(ImageSourceUri? oldFallbackSourceUri)
    {
        base.HandleFallbackSourceChanged(oldFallbackSourceUri);
        ConfigureEffectiveCoverItem();
    }

    private protected override void HandleLoadedFallbackSource()
    {
        if (EffectiveCoverImage == null && CoverSourceUri is null)
        {
            ConfigureEffectiveCoverItem();
        }
    }

    private void ConfigureEffectiveCoverItem()
    {
        if (CoverSourceUri is not null)
        {
            if (_failedCoverSourceKey == CoverSourceUri.CacheKey && FallbackSourceUri is not null)
            {
                SetOwnedCoverItem(new ImagePreviewItem(FallbackSourceUri), allowFallback: false);
                return;
            }

            SetOwnedCoverItem(new ImagePreviewItem(CoverSourceUri), allowFallback: true);
            return;
        }

        var currentCoverItem = ResolveCurrentCoverItem();
        if (currentCoverItem is not null)
        {
            SetCoverItem(currentCoverItem, ownsItem: false);
            return;
        }

        if (FallbackSourceUri is not null)
        {
            SetOwnedCoverItem(new ImagePreviewItem(FallbackSourceUri), allowFallback: false);
        }
        else
        {
            SetCoverItem(null, ownsItem: false);
        }
    }

    private ImagePreviewItem? ResolveCurrentCoverItem()
    {
        if (EffectiveItems is not { Count: > 0 } effectiveItems)
        {
            return null;
        }

        var currentIndex = CurrentIndex;
        if (currentIndex < 0)
        {
            currentIndex = 0;
        }
        else if (currentIndex >= effectiveItems.Count)
        {
            currentIndex = effectiveItems.Count - 1;
        }

        return effectiveItems[currentIndex];
    }

    private void SetOwnedCoverItem(ImagePreviewItem item, bool allowFallback)
    {
        if (_ownsCoverItem && _coverItem?.SourceUri.CacheKey == item.SourceUri.CacheKey)
        {
            item.Dispose();
            return;
        }

        SetCoverItem(item, ownsItem: true);
        BeginLoadingCoverItem(item, allowFallback);
    }

    private void SetCoverItem(ImagePreviewItem? item, bool ownsItem)
    {
        if (ReferenceEquals(_coverItem, item))
        {
            return;
        }

        var oldCoverItem   = _coverItem;
        var disposeOldItem = _ownsCoverItem && oldCoverItem != null;
        if (oldCoverItem != null)
        {
            oldCoverItem.PropertyChanged -= HandleCoverItemPropertyChanged;
        }

        CancelCoverImageLoad();
        _coverItem     = item;
        _ownsCoverItem = ownsItem;
        if (item != null)
        {
            item.PropertyChanged += HandleCoverItemPropertyChanged;
        }
        UpdateCoverImageState();

        if (disposeOldItem)
        {
            oldCoverItem?.Dispose();
        }
    }

    private void BeginLoadingCoverItem(ImagePreviewItem item, bool allowFallback)
    {
        var cancellation = new CancellationTokenSource();
        _coverLoadCancellation = cancellation;
        _ = LoadCoverItemAsync(item, allowFallback, cancellation.Token);
    }

    private async Task LoadCoverItemAsync(ImagePreviewItem item, bool allowFallback, CancellationToken cancellationToken)
    {
        var version = item.BeginLoading();
        try
        {
            var loadedSource = await ImageSourceLoader.LoadAsync(item.SourceUri, cancellationToken);
            void CompleteLoading()
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    if (item.CompleteLoading(version, loadedSource) &&
                        CoverSourceUri?.CacheKey == item.SourceUri.CacheKey)
                    {
                        _failedCoverSourceKey = null;
                    }
                }
                else
                {
                    loadedSource.Dispose();
                }
            }

            if (Dispatcher.UIThread.CheckAccess())
            {
                CompleteLoading();
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(CompleteLoading);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            void FailLoading()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (item.FailLoading(version, ex) && allowFallback)
                {
                    _failedCoverSourceKey = item.SourceUri.CacheKey;
                    if (FallbackSourceUri is not null)
                    {
                        SetOwnedCoverItem(new ImagePreviewItem(FallbackSourceUri), allowFallback: false);
                    }
                }
            }

            if (Dispatcher.UIThread.CheckAccess())
            {
                FailLoading();
            }
            else
            {
                await Dispatcher.UIThread.InvokeAsync(FailLoading);
            }
        }
    }

    private void HandleCoverItemPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(ImagePreviewItem.LoadedSource) ||
            args.PropertyName == nameof(ImagePreviewItem.State) ||
            args.PropertyName == nameof(ImagePreviewItem.IsLoading) ||
            args.PropertyName == nameof(ImagePreviewItem.IsFailed))
        {
            UpdateCoverImageState();
        }
    }

    private void UpdateCoverImageState()
    {
        SetCurrentValue(EffectiveCoverImageProperty, _coverItem?.LoadedSource);
        SetCurrentValue(IsCoverImageLoadingProperty, _coverItem?.IsLoading == true);
        SetCurrentValue(IsCoverImageFailedProperty, _coverItem?.IsFailed == true);
    }

    private void CancelCoverImageLoad()
    {
        _coverLoadCancellation?.Cancel();
        _coverLoadCancellation?.Dispose();
        _coverLoadCancellation = null;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        CancelCoverImageLoad();
    }
}
