using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public class ImagePreviewer : AbstractImagePreviewer
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> CoverIndicatorContentProperty =
        AvaloniaProperty.Register<ImagePreviewer, object?>(nameof(CoverIndicatorContent));
    
    public static readonly StyledProperty<IDataTemplate?> CoverIndicatorContentTemplateProperty =
        AvaloniaProperty.Register<ImagePreviewer, IDataTemplate?>(nameof(CoverIndicatorContentTemplate));
    
    public static readonly StyledProperty<string?> CoverImageSrcProperty =
        AvaloniaProperty.Register<ImagePreviewer, string?>(nameof(CoverImageSrc));
    
    public static readonly StyledProperty<bool> IsShowCoverMaskProperty =
        AvaloniaProperty.Register<ImagePreviewer, bool>(nameof(IsShowCoverMask), true);
    
    public string? CoverImageSrc
    {
        get => GetValue(CoverImageSrcProperty);
        set => SetValue(CoverImageSrcProperty, value);
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
    
    internal static readonly DirectProperty<ImagePreviewer, PreviewImageSource?> EffectiveCoverImageProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewer, PreviewImageSource?>(
            nameof(EffectiveCoverImage),
            o => o.EffectiveCoverImage,
            (o, v) => o.EffectiveCoverImage = v);
    
    private PreviewImageSource? _effectiveCoverImage;
    private bool _ownsEffectiveCoverImage;
    private string? _effectiveCoverImageSrc;
    private bool _isDetaching;

    internal PreviewImageSource? EffectiveCoverImage
    {
        get => _effectiveCoverImage;
        set => SetAndRaise(EffectiveCoverImageProperty, ref _effectiveCoverImage, value);
    }
    #endregion

    protected override bool ShouldEagerLoadSourcesOnItemsSourceChanged => false;
    
    static ImagePreviewer()
    {
        AffectsRender<ImagePreviewer>(CoverImageSrcProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CoverImageSrcProperty ||
            change.Property == ItemsSourceProperty ||
            change.Property == EffectiveSourcesProperty)
        {
            RefreshEffectiveCoverImage();
        }
    }
    
    protected override void OnLoaded(RoutedEventArgs args)
    {
        base.OnLoaded(args);
        RefreshEffectiveCoverImage();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isDetaching = true;
        try
        {
            SetEffectiveCoverImage(null, ownsSource: false, sourcePath: null);
            base.OnDetachedFromVisualTree(e);
        }
        finally
        {
            _isDetaching = false;
        }
    }

    private void RefreshEffectiveCoverImage()
    {
        if (_isDetaching)
        {
            SetEffectiveCoverImage(null, ownsSource: false, sourcePath: null);
            return;
        }

        if (!string.IsNullOrEmpty(CoverImageSrc))
        {
            LoadOwnedEffectiveCoverImage(CoverImageSrc);
            return;
        }

        if (EffectiveSources?.Count > 0)
        {
            SetEffectiveCoverImage(EffectiveSources[0], ownsSource: false, sourcePath: null);
            return;
        }

        if (ItemsSource is { Count: > 0 } itemsSource)
        {
            LoadOwnedEffectiveCoverImage(itemsSource[0]);
            return;
        }

        SetEffectiveCoverImage(null, ownsSource: false, sourcePath: null);
    }

    private void LoadOwnedEffectiveCoverImage(string sourcePath)
    {
        if (_ownsEffectiveCoverImage &&
            _effectiveCoverImageSrc == sourcePath &&
            EffectiveCoverImage is not null)
        {
            return;
        }

        try
        {
            SetEffectiveCoverImage(LoadImageSource(sourcePath), ownsSource: true, sourcePath);
        }
        catch (Exception)
        {
            SetEffectiveCoverImage(null, ownsSource: false, sourcePath: null);
        }
    }

    private void SetEffectiveCoverImage(PreviewImageSource? source, bool ownsSource, string? sourcePath)
    {
        var oldSource       = EffectiveCoverImage;
        var shouldDisposeOld = _ownsEffectiveCoverImage &&
                               oldSource is not null &&
                               !ReferenceEquals(oldSource, source);

        _ownsEffectiveCoverImage = ownsSource;
        _effectiveCoverImageSrc  = sourcePath;
        SetCurrentValue(EffectiveCoverImageProperty, source);

        if (shouldDisposeOld)
        {
            oldSource!.Dispose();
        }
    }
}
