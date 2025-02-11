using AtomUI.Media;
using AtomUI.Theme.Palette;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

public partial class DesignToken : AbstractDesignToken
{
     /// <summary>
    /// 现在这里的实现是写死的主色，后面是不是可以读取配置
    /// </summary>
    private readonly IDictionary<PresetColorType, PresetPrimaryColor> _defaultPresetColors;
     
    [NotTokenDefinition]
    public IDictionary<PresetPrimaryColor, ColorMap> ColorPalettes { get; set; }

    public DesignToken()
    {
        InitSeedTokenValues();
        ColorPalettes        = new Dictionary<PresetPrimaryColor, ColorMap>();
        _defaultPresetColors = new Dictionary<PresetColorType, PresetPrimaryColor>();
        foreach (var color in PresetPrimaryColor.AllColorTypes())
        {
            _defaultPresetColors[color.Type] = color;
        }
    }

    private void InitSeedTokenValues()
    {
        ColorPrimary = Color.Parse("#1677ff");
        ColorSuccess = Color.Parse("#52c41a");
        ColorWarning = Color.Parse("#faad14");
        ColorError   = Color.Parse("#ff4d4f");
        ColorInfo    = Color.Parse("#1677ff");
        FontFamily = new List<string>
        {
            "-apple-system",
            "BlinkMacSystemFont",
            "Segoe UI",
            "Roboto",
            "Helvetica Neue",
            "Arial",
            "Noto Sans",
            "sans-serif",
            "Apple Color Emoji",
            "Segoe UI Emoji",
            "Segoe UI Symbol",
            "Microsoft YaHei"
        };
        FontFamilyCode = new List<string>
        {
            "SFMono-Regular",
            "Consolas",
            "Liberation Mono",
            "Menlo",
            "Courier",
            "monospace"
        };
        BorderRadius     = new CornerRadius(6);
        ColorTransparent = Colors.Transparent;
    }
    
    public PresetPrimaryColor GetPresetPrimaryColor(PresetColorType colorType)
    {
        return _defaultPresetColors[colorType];
    }
    
    public void SetColorPalette(PresetPrimaryColor primaryColor, ColorMap colorMap)
    {
        ColorPalettes[primaryColor] = colorMap;
    }

    public ColorMap? GetColorPalette(PresetPrimaryColor primaryColor)
    {
        ColorMap? value;
        if (ColorPalettes.TryGetValue(primaryColor, out value))
        {
            return value;
        }

        return null;
    }

    internal void CalculateAliasTokenValues()
    {
        var screenXS  = 480;
        var screenSM  = 576;
        var screenMD  = 768;
        var screenLG  = 992;
        var screenXL  = 1200;
        var screenXXL = 1600;
        
        // Motion
        if (!EnableMotion)
        {
            MotionDurationFast     = TimeSpan.FromMilliseconds(0);
            MotionDurationMid      = TimeSpan.FromMilliseconds(0);
            MotionDurationSlow     = TimeSpan.FromMilliseconds(0);
            MotionDurationVerySlow = TimeSpan.FromMilliseconds(0);
        }
        
        // setup alias token
        // ============== Background ============== //
        ColorFillContent         = ColorFillSecondary;
        ColorFillContentHover    = ColorFill;
        ColorFillAlter           = ColorFillQuaternary;
        ColorBgContainerDisabled = ColorFillTertiary;
        
        // ============== Split ============== //
        ColorBorderBg = ColorBgContainer;
        ColorSplit    = ColorUtils.AlphaColor(ColorBorderSecondary, ColorBgContainer);
        
        // ============== Text ============== //
        ColorTextPlaceholder = ColorTextQuaternary;
        ColorTextDisabled    = ColorTextQuaternary;
        ColorTextHeading     = ColorText;
        ColorTextLabel       = ColorTextSecondary;
        ColorTextDescription = ColorTextTertiary;
        ColorTextLightSolid  = ColorWhite;
        ColorHighlight       = ColorError;
        ColorBgTextHover     = ColorFillSecondary;
        ColorBgTextActive    = ColorFill;
        
        ColorIcon      = ColorTextTertiary;
        ColorIconHover = ColorText;
        
        ColorErrorOutline   = ColorUtils.AlphaColor(ColorErrorBg, ColorBgContainer);
        ColorWarningOutline = ColorUtils.AlphaColor(ColorWarningBg, ColorBgContainer);
        
        // Font
        FontSizeIcon = FontSizeSM;
        
        // icon
        IconSizeXS = (int)FontSizeSM - 2;
        IconSizeSM = (int)FontSizeSM;
        IconSize   = (int)FontSize;
        IconSizeLG = (int)FontSizeLG;
        
        // Line
        LineWidthFocus     = LineWidth * 2;
        WaveAnimationRange = LineWidth * 6;
        WaveStartOpacity   = 0.4;
        
        // Control
        ControlOutlineWidth = LineWidth * 2;
        
        // Checkbox size and expand icon size
        ControlInteractiveSize = ControlHeight / 2;
        
        ControlItemBgHover          = ColorFillTertiary;
        ControlItemBgActive         = ColorPrimaryBg;
        ControlItemBgActiveHover    = ColorPrimaryBgHover;
        ControlItemBgActiveDisabled = ColorFill;
        ColorControlOutline         = ColorUtils.AlphaColor(ColorPrimaryBg, ColorBgContainer);
        
        FontWeightStrong    = 600;
        OpacityLoading      = 0.65;
        LinkDecoration      = null;
        LinkHoverDecoration = null;
        LinkFocusDecoration = null;
        
        ControlPadding   = 12;
        ControlPaddingSM = 8;
        
        PaddingXXS = SizeXXS;
        PaddingXS  = SizeXS;
        PaddingSM  = SizeSM;
        Padding    = Size;
        PaddingMD  = SizeMD;
        PaddingLG  = SizeLG;
        PaddingXL  = SizeXL;
        
        PaddingContentHorizontalLG = SizeLG;
        PaddingContentVerticalLG   = SizeMS;
        PaddingContentHorizontal   = SizeMS;
        PaddingContentVertical     = SizeSM;
        PaddingContentHorizontalSM = SizeSM;
        PaddingContentHorizontalXS = SizeXS;
        PaddingContentVerticalSM   = SizeXS;
        
        MarginXXS = SizeXXS;
        MarginXS  = SizeXS;
        MarginSM  = SizeSM;
        Margin    = Size;
        MarginMD  = SizeMD;
        MarginLG  = SizeLG;
        MarginXL  = SizeXL;
        MarginXXL = SizeXXL;
        
        ScreenXS     = screenXS;
        ScreenXSMin  = screenXS;
        ScreenXSMax  = screenSM - 1;
        ScreenSM     = screenSM;
        ScreenSMMin  = screenSM;
        ScreenSMMax  = screenMD - 1;
        ScreenMD     = screenMD;
        ScreenMDMin  = screenMD;
        ScreenMDMax  = screenLG - 1;
        ScreenLG     = screenLG;
        ScreenLGMin  = screenLG;
        ScreenLGMax  = screenXL - 1;
        ScreenXL     = screenXL;
        ScreenXLMin  = screenXL;
        ScreenXLMax  = screenXXL - 1;
        ScreenXXL    = screenXXL;
        ScreenXXLMin = screenXXL;
        
        BoxShadows = new BoxShadows(new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 6,
            Blur    = 16,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.08, 0, 0, 0)
        }, new[]
        {
            new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 3,
                Blur    = 6,
                Spread  = -4,
                Color   = ColorUtils.FromRgbF(0.12, 0, 0, 0)
            },
            new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 9,
                Blur    = 28,
                Spread  = 8,
                Color   = ColorUtils.FromRgbF(0.05, 0, 0, 0)
            }
        });
        
        BoxShadowsSecondary = new BoxShadows(new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 6,
            Blur    = 16,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.10, 0, 0, 0)
        }, new[]
        {
            new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 3,
                Blur    = 6,
                Spread  = -4,
                Color   = ColorUtils.FromRgbF(0.12, 0, 0, 0)
            },
            new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 9,
                Blur    = 28,
                Spread  = 8,
                Color   = ColorUtils.FromRgbF(0.07, 0, 0, 0)
            }
        });
        
        BoxShadowsTertiary = new BoxShadows(new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 1,
            Blur    = 2,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.03, 0, 0, 0)
        }, new[]
        {
            new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 1,
                Blur    = 6,
                Spread  = -1,
                Color   = ColorUtils.FromRgbF(0.02, 0, 0, 0)
            },
            new BoxShadow
            {
                OffsetX = 0,
                OffsetY = 2,
                Blur    = 4,
                Spread  = 0,
                Color   = ColorUtils.FromRgbF(0.02, 0, 0, 0)
            }
        });
    }
}