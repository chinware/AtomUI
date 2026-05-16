using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;
using SvgControl = Avalonia.Svg.Svg;

namespace AtomUI.Controls.Commons;

[TemplatePart("PART_SvgImage", typeof(SvgControl))]
public abstract class AbstractEmpty : TemplatedControl, ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<PresetEmptyImage?> PresetImageProperty =
        AvaloniaProperty.Register<AbstractEmpty, PresetEmptyImage?>(nameof(PresetImage));

    public static readonly StyledProperty<string?> ImagePathProperty =
        AvaloniaProperty.Register<AbstractEmpty, string?>(nameof(ImagePath));

    public static readonly StyledProperty<string?> ImageSourceProperty =
        AvaloniaProperty.Register<AbstractEmpty, string?>(nameof(ImageSource));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<AbstractEmpty, string?>(nameof(Description));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractEmpty>();

    public static readonly StyledProperty<bool> IsDescriptionVisibleProperty =
        AvaloniaProperty.Register<AbstractEmpty, bool>(nameof(IsDescriptionVisible), true);

    public PresetEmptyImage? PresetImage
    {
        get => GetValue(PresetImageProperty);
        set => SetValue(PresetImageProperty, value);
    }

    public string? ImagePath
    {
        get => GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public string? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool IsDescriptionVisible
    {
        get => GetValue(IsDescriptionVisibleProperty);
        set => SetValue(IsDescriptionVisibleProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<IBrush?> BorderColorProperty =
        AvaloniaProperty.Register<AbstractEmpty, IBrush?>(nameof(BorderColor));
    
    internal static readonly StyledProperty<IBrush?> BorderColorSecondaryProperty =
        AvaloniaProperty.Register<AbstractEmpty, IBrush?>(nameof(BorderColorSecondary));
    
    internal static readonly StyledProperty<IBrush?> ShadowColorProperty =
        AvaloniaProperty.Register<AbstractEmpty, IBrush?>(nameof(ShadowColor));
    
    internal static readonly StyledProperty<IBrush?> ContentColorProperty =
        AvaloniaProperty.Register<AbstractEmpty, IBrush?>(nameof(ContentColor));
    
    internal static readonly StyledProperty<IBrush?> BgColorProperty =
        AvaloniaProperty.Register<AbstractEmpty, IBrush?>(nameof(BgColor));

    internal IBrush? BorderColor
    {
        get => GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }
    
    internal IBrush? BorderColorSecondary
    {
        get => GetValue(BorderColorSecondaryProperty);
        set => SetValue(BorderColorSecondaryProperty, value);
    }
    
    internal IBrush? ShadowColor
    {
        get => GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    internal IBrush? ContentColor
    {
        get => GetValue(ContentColorProperty);
        set => SetValue(ContentColorProperty, value);
    }
    
    internal IBrush? BgColor
    {
        get => GetValue(BgColorProperty);
        set => SetValue(BgColorProperty, value);
    }

    #endregion

    private SvgControl? _svg;
    private TextBlock? _descriptionTextBlock;

    static AbstractEmpty()
    {
        AffectsMeasure<AbstractEmpty>(PresetImageProperty,
            ImagePathProperty,
            ImageSourceProperty,
            DescriptionProperty,
            IsDescriptionVisibleProperty);
    }

    private void CheckImageSource()
    {
        var imageSetCount = 0;
        if (PresetImage is not null)
        {
            imageSetCount++;
        }

        if (ImagePath is not null)
        {
            imageSetCount++;
        }

        if (ImageSource is not null)
        {
            imageSetCount++;
        }

        if (imageSetCount > 1)
        {
            throw new InvalidOperationException(
                "ImagePath, ImageSource and PresetImage cannot be set at the same time.");
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PresetImageProperty ||
            change.Property == ImagePathProperty ||
            change.Property == ImageSourceProperty)
        {
            if (_svg is not null)
            {
                CheckImageSource();
                SetupImage();
            }
        }
        else if (change.Property == IsDescriptionVisibleProperty)
        {
            if (_svg is not null)
            {
                SyncDescriptionVisibility();
            }
        }
        else if (IsImageColorProperty(change.Property) && _svg is not null)
        {
            SetupImage();
        }
    }

    private static bool IsImageColorProperty(AvaloniaProperty property)
    {
        return property == BorderColorProperty ||
               property == BorderColorSecondaryProperty ||
               property == ShadowColorProperty ||
               property == ContentColorProperty ||
               property == BgColorProperty;
    }

    private void SetupImage()
    {
        if (_svg is null)
        {
            return;
        }

        if (PresetImage is not null)
        {
            SetSvgPath(null);
            SetSvgSource(TryCreateBuiltInImageSource(PresetImage.Value, out var source) ? source : null);
        }
        else if (ImageSource is not null)
        {
            SetSvgPath(null);
            SetSvgSource(ImageSource);
        }
        else if (ImagePath is not null)
        {
            SetSvgSource(null);
            SetSvgPath(ImagePath);
        }
        else
        {
            SetSvgSource(null);
            SetSvgPath(null);
        }
    }

    private void SetSvgSource(string? source)
    {
        if (_svg is not null && _svg.Source != source)
        {
            _svg.Source = source;
        }
    }

    private void SetSvgPath(string? path)
    {
        if (_svg is not null && _svg.Path != path)
        {
            _svg.Path = path;
        }
    }

    private void SyncDescriptionVisibility()
    {
        if (_descriptionTextBlock is null)
        {
            if (IsDescriptionVisible)
            {
                return;
            }

            EnsureDescriptionTextBlock();
        }

        if (_descriptionTextBlock is not null)
        {
            _descriptionTextBlock.IsVisible = IsDescriptionVisible;
        }
    }

    private void EnsureDescriptionTextBlock()
    {
        foreach (var descendant in this.GetVisualDescendants())
        {
            if (descendant is TextBlock { Name: "Description" } descriptionTextBlock)
            {
                _descriptionTextBlock = descriptionTextBlock;
                return;
            }
        }
    }

    private bool TryCreateBuiltInImageSource(PresetEmptyImage presetImage,
                                             out string? source)
    {
        source = null;
        if (!TryGetSolidColor(BgColor, out var bgColor) ||
            !TryGetSolidColor(BorderColor, out var rawBorderColor) ||
            !TryGetSolidColor(BorderColorSecondary, out var rawBorderColorSecondary) ||
            !TryGetSolidColor(ShadowColor, out var rawShadowColor) ||
            !TryGetSolidColor(ContentColor, out var rawContentColor))
        {
            return false;
        }

        var borderColor          = ColorUtils.OnBackground(rawBorderColor, bgColor);
        var borderColorSecondary = ColorUtils.OnBackground(rawBorderColorSecondary, bgColor);
        var shadowColor          = ColorUtils.OnBackground(rawShadowColor, bgColor);
        var contentColor         = ColorUtils.OnBackground(rawContentColor, bgColor);
        if (presetImage == PresetEmptyImage.Default)
        {
            source = BuiltInImageBuilder.BuildDefaultImage(shadowColor, borderColor, borderColorSecondary);
        }
        else
        {
            source = BuiltInImageBuilder.BuildSimpleImage(contentColor, borderColor, shadowColor);
        }
        return true;
    }

    private static bool TryGetSolidColor(IBrush? brush, out Color color)
    {
        if (brush is ISolidColorBrush solidColorBrush)
        {
            color = solidColorBrush.Color;
            return true;
        }

        color = default;
        return false;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _svg = e.NameScope.Find<SvgControl>("PART_SvgImage");
        _descriptionTextBlock = null;
        if (!IsDescriptionVisible)
        {
            _descriptionTextBlock = e.NameScope.Find<TextBlock>("Description");
        }
        CheckImageSource();
        SyncDescriptionVisibility();
        SetupImage();
    }
}
