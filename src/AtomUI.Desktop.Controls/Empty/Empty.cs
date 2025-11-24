using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public enum PresetEmptyImage
{
    Simple,
    Default
}

public class Empty : TemplatedControl, IControlSharedTokenResourcesHost, ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<PresetEmptyImage?> PresetEmptyImageProperty =
        AvaloniaProperty.Register<Empty, PresetEmptyImage?>(nameof(PresetImage));

    public static readonly StyledProperty<string?> ImagePathProperty =
        AvaloniaProperty.Register<Empty, string?>(nameof(ImagePath));

    public static readonly StyledProperty<string?> ImageSourceProperty =
        AvaloniaProperty.Register<Empty, string?>(nameof(ImageSource));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<Empty, string?>(nameof(Description));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Empty>();

    public static readonly StyledProperty<bool> IsShowDescriptionProperty =
        AvaloniaProperty.Register<Empty, bool>(nameof(IsShowDescription), true);

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
    
    internal static readonly StyledProperty<IBrush?> BorderColorProperty =
        AvaloniaProperty.Register<RibbonBadge, IBrush?>(nameof(BorderColor));
    
    internal static readonly StyledProperty<IBrush?> BorderColorSecondaryProperty =
        AvaloniaProperty.Register<RibbonBadge, IBrush?>(nameof(BorderColorSecondary));
    
    internal static readonly StyledProperty<IBrush?> ShadowColorProperty =
        AvaloniaProperty.Register<RibbonBadge, IBrush?>(nameof(ShadowColor));
    
    internal static readonly StyledProperty<IBrush?> ContentColorProperty =
        AvaloniaProperty.Register<RibbonBadge, IBrush?>(nameof(ContentColor));
    
    internal static readonly StyledProperty<IBrush?> BgColorProperty =
        AvaloniaProperty.Register<RibbonBadge, IBrush?>(nameof(BgColor));
    
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

    string IControlSharedTokenResourcesHost.TokenId => EmptyToken.ID;
    Control IControlSharedTokenResourcesHost.HostControl => this;

    #endregion
    
    private Avalonia.Svg.Svg? _svg;

    static Empty()
    {
        AffectsMeasure<Empty>(PresetEmptyImageProperty,
            ImagePathProperty,
            ImageSourceProperty,
            DescriptionProperty,
            IsShowDescriptionProperty,
            BorderColorProperty,
            BorderColorSecondaryProperty,
            ShadowColorProperty,
            ContentColorProperty,
            BgColorProperty);
    }

    public Empty()
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
            if (change.Property == BorderColorProperty ||
                change.Property == BorderColorSecondaryProperty ||
                change.Property == ShadowColorProperty ||
                change.Property == ContentColorProperty ||
                change.Property == BgColorProperty)
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
            if (BorderColor != null && ShadowColor != null && ContentColor != null && BgColor != null && BorderColorSecondary != null)
            {
                var colorBgContainer    = ((ISolidColorBrush)BgColor!).Color;
                var borderColor         = ColorUtils.OnBackground(((ISolidColorBrush)BorderColor).Color, colorBgContainer);
                var borderColorSecondary         = ColorUtils.OnBackground(((ISolidColorBrush)BorderColorSecondary).Color, colorBgContainer);
                var shadowColor         = ColorUtils.OnBackground(((ISolidColorBrush)ShadowColor).Color, colorBgContainer);
                var contentColor        = ColorUtils.OnBackground(((ISolidColorBrush)ContentColor).Color, colorBgContainer);
                if (PresetImage.Value == PresetEmptyImage.Default)
                {
                    _svg.Source = BuiltInImageBuilder.BuildDefaultImage(shadowColor, borderColor, borderColorSecondary);
                }
                else
                {
                    _svg.Source = BuiltInImageBuilder.BuildSimpleImage(contentColor, borderColor, shadowColor);
                }
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
        _svg = e.NameScope.Find<Avalonia.Svg.Svg>(EmptyThemeConstants.SvgImagePart);
        CheckImageSource();
        SetupImage();
    }
}