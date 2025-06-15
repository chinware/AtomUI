using AtomUI.Controls.Themes;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum PresetEmptyImage
{
    Simple,
    Default
}

public class EmptyIndicator : TemplatedControl,
                              IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<PresetEmptyImage?> PresetEmptyImageProperty =
        AvaloniaProperty.Register<EmptyIndicator, PresetEmptyImage?>(nameof(PresetImage));

    public static readonly StyledProperty<string?> ImagePathProperty =
        AvaloniaProperty.Register<EmptyIndicator, string?>(nameof(ImagePath));

    public static readonly StyledProperty<string?> ImageSourceProperty =
        AvaloniaProperty.Register<EmptyIndicator, string?>(nameof(ImageSource));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<EmptyIndicator, string?>(nameof(Description), "No data");

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<EmptyIndicator>();

    public static readonly StyledProperty<bool> IsShowDescriptionProperty =
        AvaloniaProperty.Register<EmptyIndicator, bool>(nameof(IsShowDescription), true);

    public PresetEmptyImage? PresetImage
    {
        get => GetValue(PresetEmptyImageProperty);
        set => SetValue(PresetEmptyImageProperty, value);
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

    public bool IsShowDescription
    {
        get => GetValue(IsShowDescriptionProperty);
        set => SetValue(IsShowDescriptionProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillProperty =
        AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
            nameof(_colorFill),
            o => o._colorFill,
            (o, v) => o._colorFill = v);

    internal static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillTertiaryProperty =
        AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
            nameof(_colorFillTertiary),
            o => o._colorFillTertiary,
            (o, v) => o._colorFillTertiary = v);

    internal static readonly DirectProperty<EmptyIndicator, IBrush?> ColorFillQuaternaryProperty =
        AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
            nameof(_colorFillQuaternary),
            o => o._colorFillQuaternary,
            (o, v) => o._colorFillQuaternary = v);

    internal static readonly DirectProperty<EmptyIndicator, IBrush?> ColorBgContainerProperty =
        AvaloniaProperty.RegisterDirect<EmptyIndicator, IBrush?>(
            nameof(_colorBgContainer),
            o => o._colorBgContainer,
            (o, v) => o._colorBgContainer = v);

    private IBrush? _colorFill;

    internal IBrush? ColorFill
    {
        get => _colorFill;
        set => SetAndRaise(ColorFillProperty, ref _colorFill, value);
    }

    private IBrush? _colorFillTertiary;

    internal IBrush? ColorFillTertiary
    {
        get => _colorFillTertiary;
        set => SetAndRaise(ColorFillTertiaryProperty, ref _colorFillTertiary, value);
    }

    private IBrush? _colorFillQuaternary;

    internal IBrush? ColorFillQuaternary
    {
        get => _colorFillQuaternary;
        set => SetAndRaise(ColorFillQuaternaryProperty, ref _colorFillQuaternary, value);
    }

    private IBrush? _colorBgContainer;

    internal IBrush? ColorBgContainer
    {
        get => _colorBgContainer;
        set => SetAndRaise(ColorBgContainerProperty, ref _colorBgContainer, value);
    }


    string IControlSharedTokenResourcesHost.TokenId => EmptyIndicatorToken.ID;
    Control IControlSharedTokenResourcesHost.HostControl => this;

    #endregion
    
    private Avalonia.Svg.Svg? _svg;

    static EmptyIndicator()
    {
        AffectsMeasure<EmptyIndicator>(PresetEmptyImageProperty,
            ImagePathProperty,
            ImageSourceProperty,
            DescriptionProperty,
            IsShowDescriptionProperty,
            ColorFillProperty,
            ColorFillTertiaryProperty,
            ColorFillQuaternaryProperty,
            ColorBgContainerProperty);
    }

    public EmptyIndicator()
    {
        this.RegisterResources();
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
            throw new ApplicationException(
                "ImagePath, ImageSource and PresetEmptyImage cannot be set at the same time.");
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == ColorFillProperty ||
                change.Property == ColorFillTertiaryProperty ||
                change.Property == ColorFillQuaternaryProperty ||
                change.Property == ColorBgContainerProperty)
            {
                SetupImage();
            }
        }
    }

    private void SetupImage()
    {
        if (_svg is null)
        {
            return;
        }

        if (PresetImage is not null)
        {
            if (PresetImage.Value == PresetEmptyImage.Default)
            {
                _svg.Source = BuiltInImageBuilder.BuildDefaultImage();
            }
            else
            {
                var colorFill           = ((ISolidColorBrush)ColorFill!).Color;
                var colorFillTertiary   = ((ISolidColorBrush)ColorFillTertiary!).Color;
                var colorFillQuaternary = ((ISolidColorBrush)ColorFillQuaternary!).Color;
                var colorBgContainer    = ((ISolidColorBrush)ColorBgContainer!).Color;
                var borderColor         = ColorUtils.OnBackground(colorFill, colorBgContainer);
                var shadowColor         = ColorUtils.OnBackground(colorFillTertiary, colorBgContainer);
                var contentColor        = ColorUtils.OnBackground(colorFillQuaternary, colorBgContainer);
                _svg.Source = BuiltInImageBuilder.BuildSimpleImage(contentColor, borderColor, shadowColor);
            }
        }
        else if (ImageSource is not null)
        {
            _svg.Source = ImageSource;
        }
        else if (ImagePath is not null)
        {
            _svg.Path = ImagePath;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _svg                = e.NameScope.Find<Avalonia.Svg.Svg>(EmptyIndicatorThemeConstants.SvgImagePart);
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment   = VerticalAlignment.Center;
        CheckImageSource();
        SetupImage();
    }
}