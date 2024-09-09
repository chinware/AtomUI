using AtomUI.Media;
using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

public static class AliasDesignTokenExtensions
{
    public static void CalculateTokenValues(this AliasDesignToken aliasToken)
    {
        var screenXS  = 480;
        var screenSM  = 576;
        var screenMD  = 768;
        var screenLG  = 992;
        var screenXL  = 1200;
        var screenXXL = 1600;

        var seedToken = aliasToken.SeedToken;

        // Motion
        if (!seedToken.Motion)
        {
            aliasToken.StyleToken.MotionDurationFast     = TimeSpan.FromMilliseconds(0);
            aliasToken.StyleToken.MotionDurationMid      = TimeSpan.FromMilliseconds(0);
            aliasToken.StyleToken.MotionDurationSlow     = TimeSpan.FromMilliseconds(0);
            aliasToken.StyleToken.MotionDurationVerySlow = TimeSpan.FromMilliseconds(0);
        }

        // setup alias token
        // ============== Background ============== //
        var colorToken        = aliasToken.ColorToken;
        var colorNeutralToken = colorToken.ColorNeutralToken;
        var colorErrorToken   = colorToken.ColorErrorToken;
        var colorWarningToken = colorToken.ColorWarningToken;
        var colorPrimaryToken = colorToken.ColorPrimaryToken;
        var sizeToken         = aliasToken.SizeToken;

        aliasToken.ColorFillContent         = colorNeutralToken.ColorFillSecondary;
        aliasToken.ColorFillContentHover    = colorNeutralToken.ColorFill;
        aliasToken.ColorFillAlter           = colorNeutralToken.ColorFillQuaternary;
        aliasToken.ColorBgContainerDisabled = colorNeutralToken.ColorFillTertiary;

        // ============== Split ============== //
        aliasToken.ColorBorderBg = colorNeutralToken.ColorBgContainer;
        aliasToken.ColorSplit =
            ColorUtils.AlphaColor(colorNeutralToken.ColorBorderSecondary, colorNeutralToken.ColorBgContainer);

        // ============== Text ============== //
        aliasToken.ColorTextPlaceholder = colorNeutralToken.ColorTextQuaternary;
        aliasToken.ColorTextDisabled    = colorNeutralToken.ColorTextQuaternary;
        aliasToken.ColorTextHeading     = colorNeutralToken.ColorText;
        aliasToken.ColorTextLabel       = colorNeutralToken.ColorTextSecondary;
        aliasToken.ColorTextDescription = colorNeutralToken.ColorTextTertiary;
        aliasToken.ColorTextLightSolid  = colorToken.ColorWhite;
        aliasToken.ColorHighlight       = colorToken.ColorErrorToken.ColorError;
        aliasToken.ColorBgTextHover     = colorNeutralToken.ColorFillSecondary;
        aliasToken.ColorBgTextActive    = colorNeutralToken.ColorFill;

        aliasToken.ColorIcon      = colorNeutralToken.ColorTextTertiary;
        aliasToken.ColorIconHover = colorNeutralToken.ColorText;

        aliasToken.ColorErrorOutline =
            ColorUtils.AlphaColor(colorErrorToken.ColorErrorBg, colorNeutralToken.ColorBgContainer);
        aliasToken.ColorWarningOutline =
            ColorUtils.AlphaColor(colorWarningToken.ColorWarningBg, colorNeutralToken.ColorBgContainer);

        // Font
        aliasToken.FontSizeIcon = aliasToken.FontToken.FontSizeSM;

        // icon
        aliasToken.IconSizeXS = (int)aliasToken.FontToken.FontSizeSM - 2;
        aliasToken.IconSizeSM = (int)aliasToken.FontToken.FontSizeSM;
        aliasToken.IconSize   = (int)aliasToken.FontToken.FontSize;
        aliasToken.IconSizeLG = (int)aliasToken.FontToken.FontSizeLG;

        // Line
        aliasToken.LineWidthFocus     = seedToken.LineWidth * 2;
        aliasToken.WaveAnimationRange = seedToken.LineWidth * 6;
        aliasToken.WaveStartOpacity   = 0.4;

        // Control
        aliasToken.ControlOutlineWidth = seedToken.LineWidth * 2;

        // Checkbox size and expand icon size
        aliasToken.ControlInteractiveSize = seedToken.ControlHeight / 2;

        aliasToken.ControlItemBgHover          = colorNeutralToken.ColorFillTertiary;
        aliasToken.ControlItemBgActive         = colorPrimaryToken.ColorPrimaryBg;
        aliasToken.ControlItemBgActiveHover    = colorPrimaryToken.ColorPrimaryBgHover;
        aliasToken.ControlItemBgActiveDisabled = colorNeutralToken.ColorFill;
        aliasToken.ColorControlOutline =
            ColorUtils.AlphaColor(colorPrimaryToken.ColorPrimaryBg, colorNeutralToken.ColorBgContainer);

        aliasToken.FontWeightStrong    = 600;
        aliasToken.OpacityLoading      = 0.65;
        aliasToken.LinkDecoration      = null;
        aliasToken.LinkHoverDecoration = null;
        aliasToken.LinkFocusDecoration = null;

        aliasToken.ControlPadding   = 12;
        aliasToken.ControlPaddingSM = 8;

        aliasToken.PaddingXXS = sizeToken.SizeXXS;
        aliasToken.PaddingXS  = sizeToken.SizeXS;
        aliasToken.PaddingSM  = sizeToken.SizeSM;
        aliasToken.Padding    = sizeToken.Size;
        aliasToken.PaddingMD  = sizeToken.SizeMD;
        aliasToken.PaddingLG  = sizeToken.SizeLG;
        aliasToken.PaddingXL  = sizeToken.SizeXL;

        aliasToken.PaddingContentHorizontalLG = sizeToken.SizeLG;
        aliasToken.PaddingContentVerticalLG   = sizeToken.SizeMS;
        aliasToken.PaddingContentHorizontal   = sizeToken.SizeMS;
        aliasToken.PaddingContentVertical     = sizeToken.SizeSM;
        aliasToken.PaddingContentHorizontalSM = sizeToken.SizeSM;
        aliasToken.PaddingContentHorizontalXS = sizeToken.SizeXS;
        aliasToken.PaddingContentVerticalSM   = sizeToken.SizeXS;

        aliasToken.MarginXXS = sizeToken.SizeXXS;
        aliasToken.MarginXS  = sizeToken.SizeXS;
        aliasToken.MarginSM  = sizeToken.SizeSM;
        aliasToken.Margin    = sizeToken.Size;
        aliasToken.MarginMD  = sizeToken.SizeMD;
        aliasToken.MarginLG  = sizeToken.SizeLG;
        aliasToken.MarginXL  = sizeToken.SizeXL;
        aliasToken.MarginXXL = sizeToken.SizeXXL;

        aliasToken.ScreenXS     = screenXS;
        aliasToken.ScreenXSMin  = screenXS;
        aliasToken.ScreenXSMax  = screenSM - 1;
        aliasToken.ScreenSM     = screenSM;
        aliasToken.ScreenSMMin  = screenSM;
        aliasToken.ScreenSMMax  = screenMD - 1;
        aliasToken.ScreenMD     = screenMD;
        aliasToken.ScreenMDMin  = screenMD;
        aliasToken.ScreenMDMax  = screenLG - 1;
        aliasToken.ScreenLG     = screenLG;
        aliasToken.ScreenLGMin  = screenLG;
        aliasToken.ScreenLGMax  = screenXL - 1;
        aliasToken.ScreenXL     = screenXL;
        aliasToken.ScreenXLMin  = screenXL;
        aliasToken.ScreenXLMax  = screenXXL - 1;
        aliasToken.ScreenXXL    = screenXXL;
        aliasToken.ScreenXXLMin = screenXXL;

        aliasToken.BoxShadows = new BoxShadows(new BoxShadow
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

        aliasToken.BoxShadowsSecondary = new BoxShadows(new BoxShadow
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

        aliasToken.BoxShadowsTertiary = new BoxShadows(new BoxShadow
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