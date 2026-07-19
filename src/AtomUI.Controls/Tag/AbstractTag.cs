using AtomUI.Icons.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Algorithms;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

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

[TemplatePart("PART_CloseButton", typeof(AbstractIconButton))]
public abstract class AbstractTag : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<string?> TagColorProperty =
        AvaloniaProperty.Register<AbstractTag, string?>(
            nameof(TagColor));

    public static readonly StyledProperty<bool> IsClosableProperty =
        AvaloniaProperty.Register<AbstractTag, bool>(nameof(IsClosable));

    public static readonly StyledProperty<bool> IsBorderedProperty =
        AvaloniaProperty.Register<AbstractTag, bool>(nameof(IsBordered), true);

    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<AbstractTag, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<PathIcon?> CloseIconProperty =
        AvaloniaProperty.Register<AbstractTag, PathIcon?>(nameof(CloseIcon));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AbstractTag, string?>(
            nameof(Text));

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

    public bool IsBordered
    {
        get => GetValue(IsBorderedProperty);
        set => SetValue(IsBorderedProperty, value);
    }

    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public PathIcon? CloseIcon
    {
        get => GetValue(CloseIconProperty);
        set => SetValue(CloseIconProperty, value);
    }

    [Content]
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
        RoutedEvent.Register<AbstractTag, RoutedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }
    
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<Thickness> TagTextPaddingInlineProperty =
        AvaloniaProperty.Register<AbstractTag, Thickness>(nameof(TagTextPaddingInline));

    internal static readonly DirectProperty<AbstractTag, Thickness> RenderScaleAwareBorderThicknessProperty =
        AvaloniaProperty.RegisterDirect<AbstractTag, Thickness>(nameof(RenderScaleAwareBorderThickness),
            o => o.RenderScaleAwareBorderThickness,
            (o, v) => o.RenderScaleAwareBorderThickness = v);

    internal static readonly DirectProperty<AbstractTag, bool> IsPresetColorTagProperty =
        AvaloniaProperty.RegisterDirect<AbstractTag, bool>(nameof(IsPresetColorTag),
            o => o.IsPresetColorTag,
            (o, v) => o.IsPresetColorTag = v);

    internal static readonly DirectProperty<AbstractTag, bool> IsColorSetProperty =
        AvaloniaProperty.RegisterDirect<AbstractTag, bool>(nameof(IsColorSet),
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
        set => SetAndRaise(IsColorSetProperty, ref _isColorSet, value);
    }
    
    #endregion
    
    private static readonly ThemeTokenResolver s_themeTokenResolver = new();
    private static readonly (string Name, PresetPrimaryColor Color)[] PresetColorEntries =
    [
        (nameof(PresetColorType.Red), PresetPrimaryColor.Red),
        (nameof(PresetColorType.Volcano), PresetPrimaryColor.Volcano),
        (nameof(PresetColorType.Orange), PresetPrimaryColor.Orange),
        (nameof(PresetColorType.Gold), PresetPrimaryColor.Gold),
        (nameof(PresetColorType.Yellow), PresetPrimaryColor.Yellow),
        (nameof(PresetColorType.Lime), PresetPrimaryColor.Lime),
        (nameof(PresetColorType.Green), PresetPrimaryColor.Green),
        (nameof(PresetColorType.Cyan), PresetPrimaryColor.Cyan),
        (nameof(PresetColorType.Blue), PresetPrimaryColor.Blue),
        (nameof(PresetColorType.GeekBlue), PresetPrimaryColor.GeekBlue),
        (nameof(PresetColorType.Purple), PresetPrimaryColor.Purple),
        (nameof(PresetColorType.Pink), PresetPrimaryColor.Pink),
        (nameof(PresetColorType.Magenta), PresetPrimaryColor.Magenta),
        (nameof(PresetColorType.Grey), PresetPrimaryColor.Grey)
    ];
    private static readonly (string Name, TagStatus Status)[] StatusColorEntries =
    [
        (nameof(TagStatus.Success), TagStatus.Success),
        (nameof(TagStatus.Info), TagStatus.Info),
        (nameof(TagStatus.Warning), TagStatus.Warning),
        (nameof(TagStatus.Error), TagStatus.Error)
    ];
    protected AbstractIconButton? CloseButton;
    private IReadOnlyDictionary<PresetColorType, TagCalcColor> _presetColorMap =
        new Dictionary<PresetColorType, TagCalcColor>();
    private IReadOnlyDictionary<TagStatus, TagStatusCalcColor> _statusColorMap =
        new Dictionary<TagStatus, TagStatusCalcColor>();
    private IDisposable? _themeSubscription;
    
    static AbstractTag()
    {
        AffectsMeasure<AbstractTag>(IsBorderedProperty,
            IconProperty,
            IsClosableProperty,
            TextProperty);
        AffectsRender<AbstractTag>(TagColorProperty,
            ForegroundProperty,
            BackgroundProperty,
            BorderBrushProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _themeSubscription?.Dispose();
        _themeSubscription = s_themeTokenResolver.Subscribe(this, ApplyThemeSnapshot);
        if (GetValue(ThemeScope.ContextProperty) is { } context)
        {
            ApplyThemeSnapshot(context.Snapshot);
        }
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _themeSubscription?.Dispose();
        _themeSubscription = null;
        base.OnDetachedFromVisualTree(e);
    }
    
    private void ApplyThemeSnapshot(ThemeSnapshot snapshot)
    {
        SetupStatusColorMap(snapshot);
        SetupPresetColorMap(snapshot);
        if (TagColor is not null)
        {
            SetupTagColorInfo(TagColor);
        }
        InvalidateVisual();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (CloseButton != null)
        {
            CloseButton.Click -= HandleCloseRequest;
        }
        
        CloseButton = e.NameScope.Find<AbstractIconButton>("PART_CloseButton");
        if (CloseButton != null)
        {
            CloseButton.Click += HandleCloseRequest;
        }
        SetupDefaultCloseIcon();
        if (GetValue(ThemeScope.ContextProperty) is { } context)
        {
            ApplyThemeSnapshot(context.Snapshot);
        }
        if (TagColor is not null)
        {
            SetupTagColorInfo(TagColor);
        }
        ConfigureBorderThickness();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CloseIconProperty)
        {
            SetupDefaultCloseIcon();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == TagColorProperty)
            {
                if (TagColor is not null)
                {
                    SetupTagColorInfo(TagColor);
                }
            }
            else if (change.Property == IsBorderedProperty)
            {
                ConfigureBorderThickness();
            }
            else if (change.Property == IsClosableProperty)
            {
                SetupDefaultCloseIcon();
            }
        }
    }

    private void ConfigureBorderThickness()
    {
        if (IsBordered)
        {
            SetValue(BorderThicknessProperty, RenderScaleAwareBorderThickness, BindingPriority.Template);
        }
        else
        {
            SetValue(BorderThicknessProperty, new Thickness(), BindingPriority.Template);
        }
    }

    private void SetupPresetColorMap(ThemeSnapshot snapshot)
    {
        var dict = new Dictionary<PresetColorType, TagCalcColor>(PresetColorEntries.Length);
        foreach (var entry in PresetColorEntries)
        {
            if (!snapshot.PresetColorPalettes.TryGetValue(entry.Color, out var palette))
            {
                continue;
            }

            dict[entry.Color.Type] = new TagCalcColor
            {
                LightColor       = palette.ColorSequence[0],
                LightBorderColor = palette.ColorSequence[2],
                DarkColor        = palette.ColorSequence[5],
                TextColor        = palette.ColorSequence[6]
            };
        }
        _presetColorMap = dict;
    }

    private void SetupStatusColorMap(ThemeSnapshot snapshot)
    {
        Color Global(SharedTokenKind kind)
        {
            return s_themeTokenResolver.GetGlobal<Color>(snapshot, (int)kind);
        }

        _statusColorMap = new Dictionary<TagStatus, TagStatusCalcColor>(4)
        {
            [TagStatus.Success] = new TagStatusCalcColor
            {
                Color       = Global(SharedTokenKind.ColorSuccess),
                Background  = Global(SharedTokenKind.ColorSuccessBg),
                BorderColor = Global(SharedTokenKind.ColorSuccessBorder)
            },
            [TagStatus.Info] = new TagStatusCalcColor
            {
                Color       = Global(SharedTokenKind.ColorInfo),
                Background  = Global(SharedTokenKind.ColorInfoBg),
                BorderColor = Global(SharedTokenKind.ColorInfoBorder)
            },
            [TagStatus.Warning] = new TagStatusCalcColor
            {
                Color       = Global(SharedTokenKind.ColorWarning),
                Background  = Global(SharedTokenKind.ColorWarningBg),
                BorderColor = Global(SharedTokenKind.ColorWarningBorder)
            },
            [TagStatus.Error] = new TagStatusCalcColor
            {
                Color       = Global(SharedTokenKind.ColorError),
                Background  = Global(SharedTokenKind.ColorErrorBg),
                BorderColor = Global(SharedTokenKind.ColorErrorBorder)
            }
        };
    }

    private void SetupTagColorInfo(string colorStr)
    {
        IsPresetColorTag = false;
        IsColorSet       = false;
        var colorSpan    = colorStr.AsSpan().Trim();

        foreach (var entry in PresetColorEntries)
        {
            if (entry.Name.AsSpan().Equals(colorSpan, StringComparison.OrdinalIgnoreCase) &&
                _presetColorMap.TryGetValue(entry.Color.Type, out var colorInfo))
            {
                Foreground       = new SolidColorBrush(colorInfo.TextColor);
                BorderBrush      = new SolidColorBrush(colorInfo.LightBorderColor);
                Background       = new SolidColorBrush(colorInfo.LightColor);
                IsPresetColorTag = true;
                PseudoClasses.Set(TagPseudoClass.PresetColor, true);
                PseudoClasses.Set(TagPseudoClass.StatusColor, false);
                PseudoClasses.Set(TagPseudoClass.CustomColor, false);
                return;
            }
        }

        foreach (var entry in StatusColorEntries)
        {
            if (entry.Name.AsSpan().Equals(colorSpan, StringComparison.OrdinalIgnoreCase) &&
                _statusColorMap.TryGetValue(entry.Status, out var colorInfo))
            {
                Foreground       = new SolidColorBrush(colorInfo.Color);
                BorderBrush      = new SolidColorBrush(colorInfo.BorderColor);
                Background       = new SolidColorBrush(colorInfo.Background);
                IsPresetColorTag = true;
                PseudoClasses.Set(TagPseudoClass.PresetColor, false);
                PseudoClasses.Set(TagPseudoClass.StatusColor, true);
                PseudoClasses.Set(TagPseudoClass.CustomColor, false);
                return;
            }
        }

        if (Color.TryParse(colorSpan, out var color))
        {
            IsBordered = false;
            IsColorSet = true;
            Background = new SolidColorBrush(color);
            PseudoClasses.Set(TagPseudoClass.PresetColor, false);
            PseudoClasses.Set(TagPseudoClass.StatusColor, false);
            PseudoClasses.Set(TagPseudoClass.CustomColor, true);
        }
    }

    private void SetupDefaultCloseIcon()
    {
        if (!IsClosable)
        {
            return;
        }
        if (CloseIcon is null)
        {
            ClearValue(CloseIconProperty);
            SetValue(CloseIconProperty, new CloseOutlined());
        }
    }

    private void HandleCloseRequest(object? sender, EventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
    }
}
