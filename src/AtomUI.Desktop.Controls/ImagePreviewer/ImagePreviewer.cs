using Avalonia;
using Avalonia.Controls.Templates;
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
    private string? _effectiveCoverImageKey;

    internal PreviewImageSource? EffectiveCoverImage
    {
        get => _effectiveCoverImage;
        set => SetAndRaise(EffectiveCoverImageProperty, ref _effectiveCoverImage, value);
    }
    #endregion
    
    static ImagePreviewer()
    {
        AffectsRender<ImagePreviewer>(CoverImageSrcProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CoverImageSrcProperty)
        {
            if (!string.IsNullOrEmpty(CoverImageSrc))
            {
                SetOwnedEffectiveCoverImage(CoverImageSrc, $"cover:{CoverImageSrc}");
            }
            else
            {
                ConfigureEffectiveCoverImage();
            }
        }
        
        else if (change.Property == EffectiveSourcesProperty)
        {
            if (string.IsNullOrEmpty(CoverImageSrc))
            {
                ConfigureEffectiveCoverImage();
            }
        }
    }

    private protected override void HandleSourceChanged()
    {
        if (ShouldKeepDialogSourcesMaterialized())
        {
            MaterializeEffectiveSourcesFromItemsSource();
        }
        else
        {
            ClearEffectiveSources();
        }

        if (string.IsNullOrEmpty(CoverImageSrc))
        {
            ConfigureEffectiveCoverImage();
        }
    }

    private protected override void HandleLoadedFallbackSource()
    {
        if (EffectiveCoverImage == null && string.IsNullOrEmpty(CoverImageSrc))
        {
            ConfigureEffectiveCoverImage();
        }
    }

    private protected override void PrepareDialogOpen()
    {
        if (ItemsSource is { Count: > 0 })
        {
            MaterializeEffectiveSourcesFromItemsSource();
        }
        else
        {
            MaterializeFallbackEffectiveSource();
        }
    }

    private bool ShouldKeepDialogSourcesMaterialized()
    {
        return IsOpen || EffectiveSources is { Count: > 0 };
    }

    private void ConfigureEffectiveCoverImage()
    {
        if (EffectiveSources is { Count: > 0 })
        {
            SetEffectiveCoverImage(EffectiveSources[0], ownsImage: false, sourceKey: null);
            return;
        }

        if (TryGetFirstSourcePath(out var firstSourcePath))
        {
            SetOwnedEffectiveCoverImage(firstSourcePath, $"source:{firstSourcePath}");
            return;
        }

        if (!string.IsNullOrEmpty(FallbackImageSrc))
        {
            SetOwnedEffectiveCoverImage(FallbackImageSrc, $"fallback:{FallbackImageSrc}");
        }
        else
        {
            SetEffectiveCoverImage(null, ownsImage: false, sourceKey: null);
        }
    }

    private bool TryGetFirstSourcePath(out string sourcePath)
    {
        var itemsSource = ItemsSource;
        if (itemsSource != null)
        {
            foreach (var source in itemsSource)
            {
                if (!string.IsNullOrEmpty(source))
                {
                    sourcePath = source;
                    return true;
                }
            }
        }

        sourcePath = string.Empty;
        return false;
    }

    private void SetOwnedEffectiveCoverImage(string sourcePath, string sourceKey)
    {
        if (_ownsEffectiveCoverImage &&
            _effectiveCoverImageKey == sourceKey &&
            EffectiveCoverImage != null)
        {
            return;
        }

        try
        {
            SetEffectiveCoverImage(LoadImageSource(sourcePath), ownsImage: true, sourceKey);
        }
        catch (Exception)
        {
            SetEffectiveCoverImage(null, ownsImage: false, sourceKey: null);
        }
    }

    private void SetEffectiveCoverImage(PreviewImageSource? coverImage, bool ownsImage, string? sourceKey)
    {
        var oldCoverImage     = EffectiveCoverImage;
        var disposeOldCover   = _ownsEffectiveCoverImage && oldCoverImage != null && !ReferenceEquals(oldCoverImage, coverImage);
        _ownsEffectiveCoverImage = ownsImage;
        _effectiveCoverImageKey  = sourceKey;

        SetCurrentValue(EffectiveCoverImageProperty, coverImage);

        if (disposeOldCover)
        {
            oldCoverImage?.Dispose();
        }
    }
}
