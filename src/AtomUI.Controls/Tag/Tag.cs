using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Palette;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
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

public class Tag : TemplatedControl
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

    internal static readonly StyledProperty<double> TagTextPaddingInlineProperty
        = AvaloniaProperty.Register<Tag, double>(nameof(TagTextPaddingInline));

    internal double TagTextPaddingInline
    {
        get => GetValue(TagTextPaddingInlineProperty);
        set => SetValue(TagTextPaddingInlineProperty, value);
    }

    #endregion

    private bool _isPresetColorTag;
    private bool _hasColorSet;
    private static readonly Dictionary<PresetColorType, TagCalcColor> _presetColorMap;
    private static readonly Dictionary<TagStatus, TagStatusCalcColor> _statusColorMap;
    private Canvas? _layoutPanel;
    private TextBlock? _textBlock;
    private ContentPresenter? _iconContentPresenter;
    private IconButton? _closeButton;
    private readonly BorderRenderHelper _borderRenderHelper;

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
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        SetupPresetColorMap();
        SetupStatusColorMap();
        base.OnApplyTemplate(e);
        HandleTemplateApplied(e.NameScope);
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        _layoutPanel          = scope.Find<Canvas>(TagTheme.MainContainerPart);
        _closeButton          = scope.Find<IconButton>(TagTheme.CloseButtonPart);
        _textBlock            = scope.Find<TextBlock>(TagTheme.TagTextLabelPart);
        _iconContentPresenter = scope.Find<ContentPresenter>(TagTheme.IconPart);

        if (TagColor is not null)
        {
            SetupTagColorInfo(TagColor);
        }

        SetupTagClosable();
        SetupTagIcon();
        SetupTokenBindings();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        base.MeasureOverride(availableSize);
        var targetWidth  = 0d;
        var targetHeight = 0d;
        if (_layoutPanel is not null)
        {
            foreach (var child in _layoutPanel.Children)
            {
                targetWidth  += child.DesiredSize.Width;
                targetHeight =  Math.Max(targetHeight, child.DesiredSize.Height);
            }
        }

        if (Icon is not null)
        {
            targetWidth += TagTextPaddingInline;
        }

        if (IsClosable && _closeButton is not null)
        {
            targetWidth += TagTextPaddingInline;
        }

        targetWidth += Padding.Left + Padding.Right;
        return new Size(targetWidth, targetHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_closeButton is not null)
        {
            var offsetX = finalSize.Width - Padding.Right - _closeButton.DesiredSize.Width;
            var offsetY = (finalSize.Height - _closeButton.DesiredSize.Height) / 2;
            Canvas.SetLeft(_closeButton, offsetX);
            Canvas.SetTop(_closeButton, offsetY);
        }

        // icon
        if (_iconContentPresenter is not null)
        {
            var offsetX = Padding.Left;
            var offsetY = (finalSize.Height - _iconContentPresenter.DesiredSize.Height) / 2;
            Canvas.SetLeft(_iconContentPresenter, offsetX);
            Canvas.SetTop(_iconContentPresenter, offsetY);
        }

        // 文字
        if (_textBlock is not null)
        {
            var offsetX = Padding.Left;
            if (Icon is not null)
            {
                offsetX += Icon.DesiredSize.Width + TagTextPaddingInline;
            }

            // 这个时候已经算好了
            var offsetY = (finalSize.Height - _textBlock.Height) / 2;
            Canvas.SetLeft(_textBlock, offsetX);
            Canvas.SetTop(_textBlock, offsetY);
        }

        return base.ArrangeOverride(finalSize);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        HandlePropertyChangedForStyle(e);
    }

    private void SetupTokenBindings()
    {
        TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty, SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this, thickness =>
            {
                if (!Bordered)
                {
                    return new Thickness(0);
                }

                return thickness;
            }));
    }

    private void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
    {
        if (VisualRoot is not null)
        {
            if (e.Property == IsClosableProperty)
            {
                SetupTagClosable();
            }
            else if (e.Property == IconProperty)
            {
                SetupTagIcon();
            }
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
        _isPresetColorTag = false;
        _hasColorSet      = false;
        colorStr          = colorStr.Trim().ToLower();

        foreach (var entry in _presetColorMap)
        {
            if (entry.Key.ToString().ToLower() == colorStr)
            {
                var colorInfo = _presetColorMap[entry.Key];
                Foreground        = new SolidColorBrush(colorInfo.TextColor);
                BorderBrush       = new SolidColorBrush(colorInfo.LightBorderColor);
                Background        = new SolidColorBrush(colorInfo.LightColor);
                _isPresetColorTag = true;
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
                _isPresetColorTag = true;
                return;
            }
        }

        if (Color.TryParse(colorStr, out var color))
        {
            Bordered     = false;
            _hasColorSet = true;
            Background   = new SolidColorBrush(color);
            TokenResourceBinder.CreateTokenBinding(this, ForegroundProperty,
                SharedTokenKey.ColorTextLightSolid);
        }
    }

    private void SetupTagClosable()
    {
        if (IsClosable)
        {
            if (CloseIcon is null)
            {
                CloseIcon = AntDesignIconPackage.CloseOutlined();

                TokenResourceBinder.CreateTokenBinding(CloseIcon, WidthProperty, TagTokenKey.TagCloseIconSize);
                TokenResourceBinder.CreateTokenBinding(CloseIcon, HeightProperty, TagTokenKey.TagCloseIconSize);
                if (_hasColorSet && !_isPresetColorTag)
                {
                    TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.NormalFilledBrushProperty,
                        SharedTokenKey.ColorTextLightSolid);
                }
                else
                {
                    TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.NormalFilledBrushProperty,
                        SharedTokenKey.ColorIcon);
                    TokenResourceBinder.CreateTokenBinding(CloseIcon, Icon.ActiveFilledBrushProperty,
                        SharedTokenKey.ColorIconHover);
                }
            }
        }
    }

    private void SetupTagIcon()
    {
        if (Icon is not null)
        {
            TokenResourceBinder.CreateTokenBinding(Icon, WidthProperty, TagTokenKey.TagIconSize);
            TokenResourceBinder.CreateTokenBinding(Icon, HeightProperty, TagTokenKey.TagIconSize);
            if (_hasColorSet)
            {
                TokenResourceBinder.CreateTokenBinding(Icon, Icon.NormalFilledBrushProperty,
                    SharedTokenKey.ColorTextLightSolid);
            }
            else if (_isPresetColorTag)
            {
                Icon.NormalFilledBrush = Foreground;
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        _borderRenderHelper.Render(context,
            Bounds.Size,
            BorderThickness,
            CornerRadius,
            BackgroundSizing,
            Background,
            BorderBrush,
            default);
    }
}