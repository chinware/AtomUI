using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using System.ComponentModel;

namespace AtomUI.Desktop.Controls;

public class ImagePreviewer : AbstractImagePreviewer
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> CoverIndicatorContentProperty =
        AvaloniaProperty.Register<ImagePreviewer, object?>(nameof(CoverIndicatorContent));
    
    public static readonly StyledProperty<IDataTemplate?> CoverIndicatorContentTemplateProperty =
        AvaloniaProperty.Register<ImagePreviewer, IDataTemplate?>(nameof(CoverIndicatorContentTemplate));
    
    public static readonly StyledProperty<bool> IsShowCoverMaskProperty =
        AvaloniaProperty.Register<ImagePreviewer, bool>(nameof(IsShowCoverMask), true);
    
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

    public ImagePreviewer()
    {
    }

    internal ImagePreviewer(IImageSourceLoader imageSourceLoader)
        : base(imageSourceLoader)
    {
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == EffectiveItemsProperty ||
            change.Property == CoverIndexProperty)
        {
            ConfigureEffectiveCoverItem();
        }
    }

    private protected override void HandleSourceChanged()
    {
        MaterializeEffectiveItemsFromSources();
        ConfigureEffectiveCoverItem();
    }

    private protected override void HandleFallbackSourceChanged()
    {
        base.HandleFallbackSourceChanged();
        ConfigureEffectiveCoverItem();
    }

    private protected override void HandleLoadedFallbackSource()
    {
        if (EffectiveCoverImage == null)
        {
            ConfigureEffectiveCoverItem();
        }
    }

    private void ConfigureEffectiveCoverItem()
    {
        var currentCoverItem = ResolveCurrentCoverItem();
        if (currentCoverItem is not null)
        {
            SetCoverItem(currentCoverItem);
            return;
        }

        SetCoverItem(null);
    }

    private ImagePreviewItem? ResolveCurrentCoverItem()
    {
        if (EffectiveItems is not { Count: > 0 } effectiveItems)
        {
            return null;
        }

        return effectiveItems[ClampIndex(CoverIndex, effectiveItems.Count)];
    }

    private void SetCoverItem(ImagePreviewItem? item)
    {
        if (ReferenceEquals(_coverItem, item))
        {
            UpdateCoverImageState();
            RequestCoverItemLoadIfNeeded();
            return;
        }

        var oldCoverItem = _coverItem;
        if (oldCoverItem != null)
        {
            oldCoverItem.PropertyChanged -= HandleCoverItemPropertyChanged;
        }

        _coverItem = item;
        if (item != null)
        {
            item.PropertyChanged += HandleCoverItemPropertyChanged;
        }
        UpdateCoverImageState();

        RequestCoverItemLoadIfNeeded();
    }

    private void RequestCoverItemLoadIfNeeded()
    {
        if (_coverItem is { State: ImagePreviewItemState.Pending })
        {
            RequestItemLoad(_coverItem, ImagePreviewLoadPriority.Cover);
        }
    }

    private protected override void RequestClosedStateLoads()
    {
        ConfigureEffectiveCoverItem();
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

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        SetCoverItem(null);
    }
}
