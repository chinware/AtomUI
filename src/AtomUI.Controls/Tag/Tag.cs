using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Palette;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum TagStatus
{
    // 状态
    Success,
    Info,
    Error,
    Warning
}

internal struct TagCalcColor
{
    public Color LightColor { get; set; } // 1 号色
    public Color LightBorderColor { get; set; } // 3 号色
    public Color DarkColor { get; set; } // 6 号色
    public Color TextColor { get; set; } // 7 号色
}

internal struct TagStatusCalcColor
{
    public Color Color { get; set; }
    public Color Background { get; set; }
    public Color BorderColor { get; set; }
}

public class Tag : TemplatedControl,
                   IControlSharedTokenResourcesHost,
                   IResourceBindingManager
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> TagColorProperty
        = AvaloniaProperty.Register<Tag, string?>(
            nameof(Color));

    public static readonly StyledProperty<bool> IsClosableProperty
        = AvaloniaProperty.Register<Tag, bool>(nameof(IsClosable));

    public static readonly StyledProperty<bool> BorderedProperty
        = AvaloniaProperty.Register<Tag, bool>(nameof(Bordered), true);

    public static readonly StyledProperty<Icon?> IconProperty
        = AvaloniaProperty.Register<Tag, Icon?>(nameof(Icon));

    public static readonly StyledProperty<Icon?> CloseIconProperty
        = AvaloniaProperty.Register<Tag, Icon?>(nameof(CloseIcon));

    public static readonly StyledProperty<string?> TagTextProperty
        = AvaloniaProperty.Register<Tag, string?>(
            nameof(TagText));

    public string? TagColor
    {
        get => GetValue(TagColorProperty);
        set => SetValue(TagColorProperty, value);
    }

    public bool IsClosable
    {
        get => GetValue(IsClosableProperty);
        set => SetValue(IsClosableProperty, value);
    }

    public bool Bordered
    {
        get => GetValue(BorderedProperty);
        set => SetValue(BorderedProperty, value);
    }

    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Icon? CloseIcon
    {
        get => GetValue(CloseIconProperty);
        set => SetValue(CloseIconProperty, value);
    }

    [Content]
    public string? TagText
    {
        get => GetValue(TagTextProperty);
        set => SetValue(TagTextProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<Thickness> TagTextPaddingInlineProperty
        = AvaloniaProperty.Register<Tag, Thickness>(nameof(TagTextPaddingInline));

    internal static readonly DirectProperty<Tag, Thickness> RenderScaleAwareBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<Tag, Thickness>(nameof(RenderScaleAwareBorderThickness),
            o => o.RenderScaleAwareBorderThickness,
            (o, v) => o.RenderScaleAwareBorderThickness = v);

    internal static readonly DirectProperty<Tag, bool> IsPresetColorTagProperty =
        AvaloniaProperty.RegisterDirect<Tag, bool>(nameof(IsPresetColorTag),
            o => o.IsPresetColorTag,
            (o, v) => o.IsPresetColorTag = v);

    internal static readonly DirectProperty<Tag, bool> IsColorSetProperty =
        AvaloniaProperty.RegisterDirect<Tag, bool>(nameof(IsColorSet),
            o => o.IsColorSet,
            (o, v) => o.IsColorSet = v);

    internal Thickness TagTextPaddingInline
    {
        get => GetValue(TagTextPaddingInlineProperty);
        set => SetValue(TagTextPaddingInlineProperty, value);
    }

    private Thickness _renderScaleAwareBorderThickness;

    internal Thickness RenderScaleAwareBorderThickness
    {
        get => _renderScaleAwareBorderThickness;
        set => SetAndRaise(RenderScaleAwareBorderThicknessProperty, ref _renderScaleAwareBorderThickness, value);
    }

    private bool _isPresetColorTag;

    internal bool IsPresetColorTag
    {
        get => _isPresetColorTag;
        set => SetAndRaise(IsPresetColorTagProperty, ref _isPresetColorTag, value);
    }

    private bool _isColorSet;

    internal bool IsColorSet
    {
        get => _isColorSet;
        set => SetAndRaise(IsPresetColorTagProperty, ref _isColorSet, value);
    }

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => TagToken.ID;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }

    #endregion

    private CompositeDisposable? _resourceBindingsDisposable;
    private static readonly Dictionary<PresetColorType, TagCalcColor> _presetColorMap;
    private static readonly Dictionary<TagStatus, TagStatusCalcColor> _statusColorMap;

    static Tag()
    {
        _presetColorMap = new Dictionary<PresetColorType, TagCalcColor>();
        _statusColorMap = new Dictionary<TagStatus, TagStatusCalcColor>();
        AffectsMeasure<Tag>(BorderedProperty,
            IconProperty,
            IsClosableProperty);
        AffectsRender<Tag>(ForegroundProperty,
            BackgroundProperty,
            BorderBrushProperty);
    }

    public Tag()
    {
        this.RegisterResources();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this,
            RenderScaleAwareBorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
        SetupDefaultCloseIcon();
        SetupPresetColorMap();
        SetupStatusColorMap();
        if (TagColor is not null)
        {
            SetupTagColorInfo(TagColor);
        }
        SetupBorderThicknessBinding();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.Property == IconProperty ||
            e.Property == CloseIconProperty)
        {
            if (e.Property == CloseIconProperty)
            {
                SetupDefaultCloseIcon();
            }
        }
    }

    private void SetupBorderThicknessBinding()
    {
        if (Bordered)
        {
            BindUtils.RelayBind(this, RenderScaleAwareBorderThicknessProperty, this, BorderThicknessProperty);
        }
        else
        {
            SetValue(BorderThicknessProperty, new Thickness(), BindingPriority.Template);
        }
    }

    private static void SetupPresetColorMap()
    {
        if (_presetColorMap.Count == 0)
        {
            // TODO 根据当前的主题风格设置，是否需要根据风格不一样进行动态调整呢？
            var activatedTheme = ThemeManager.Current.ActivatedTheme;
            var sharedToken    = activatedTheme?.SharedToken;
            if (sharedToken == null)
            {
                // 是否需要输出日志
                return;
            }

            foreach (var entry in PresetPrimaryColor.AllColorTypes())
            {
                var colorMap = sharedToken.GetColorPalette(entry)!;
                var calcColor = new TagCalcColor
                {
                    LightColor       = colorMap.Color1,
                    LightBorderColor = colorMap.Color3,
                    DarkColor        = colorMap.Color6,
                    TextColor        = colorMap.Color7
                };
                _presetColorMap.Add(entry.Type, calcColor);
            }
        }
    }

    private static void SetupStatusColorMap()
    {
        if (_statusColorMap.Count == 0)
        {
            var activatedTheme = ThemeManager.Current.ActivatedTheme;
            var sharedToken    = activatedTheme?.SharedToken;
            if (sharedToken == null)
            {
                // 是否需要输出日志
                return;
            }

            _statusColorMap.Add(TagStatus.Success, new TagStatusCalcColor
            {
                Color       = sharedToken.ColorSuccess,
                Background  = sharedToken.ColorSuccessBg,
                BorderColor = sharedToken.ColorSuccessBorder
            });

            _statusColorMap.Add(TagStatus.Info, new TagStatusCalcColor
            {
                Color       = sharedToken.ColorInfo,
                Background  = sharedToken.ColorInfoBg,
                BorderColor = sharedToken.ColorInfoBorder
            });

            _statusColorMap.Add(TagStatus.Warning, new TagStatusCalcColor
            {
                Color       = sharedToken.ColorWarning,
                Background  = sharedToken.ColorWarningBg,
                BorderColor = sharedToken.ColorWarningBorder
            });

            _statusColorMap.Add(TagStatus.Error, new TagStatusCalcColor
            {
                Color       = sharedToken.ColorError,
                Background  = sharedToken.ColorErrorBg,
                BorderColor = sharedToken.ColorErrorBorder
            });
        }
    }

    private void SetupTagColorInfo(string colorStr)
    {
        IsPresetColorTag = false;
        IsColorSet       = false;
        colorStr          = colorStr.Trim().ToLower();

        foreach (var entry in _presetColorMap)
        {
            if (entry.Key.ToString().ToLower() == colorStr)
            {
                var colorInfo = _presetColorMap[entry.Key];
                Foreground        = new SolidColorBrush(colorInfo.TextColor);
                BorderBrush       = new SolidColorBrush(colorInfo.LightBorderColor);
                Background        = new SolidColorBrush(colorInfo.LightColor);
                IsPresetColorTag = true;
                return;
            }
        }

        foreach (var entry in _statusColorMap)
        {
            if (entry.Key.ToString().ToLower() == colorStr)
            {
                var colorInfo = _statusColorMap[entry.Key];
                Foreground        = new SolidColorBrush(colorInfo.Color);
                BorderBrush       = new SolidColorBrush(colorInfo.BorderColor);
                Background        = new SolidColorBrush(colorInfo.Background);
                IsPresetColorTag = true;
                return;
            }
        }

        if (Color.TryParse(colorStr, out var color))
        {
            Bordered    = false;
            IsColorSet = true;
            Background  = new SolidColorBrush(color);
            this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, ForegroundProperty,
                SharedTokenKey.ColorTextLightSolid));
        }
    }

    private void SetupDefaultCloseIcon()
    {
        if (CloseIcon is null)
        {
            ClearValue(CloseIconProperty);
            SetValue(CloseIconProperty, AntDesignIconPackage.CloseOutlined());
        }
    }
}