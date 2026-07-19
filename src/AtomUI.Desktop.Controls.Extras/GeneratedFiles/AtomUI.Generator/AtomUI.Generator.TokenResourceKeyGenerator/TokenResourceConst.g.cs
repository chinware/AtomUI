using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Desktop.Controls.DesignTokens
{
    public enum SplashTokenKind
    {
        AccentColor,
        ContentGap,
        ContentPadding,
        DetailFontSize,
        ErrorColor,
        FooterMarginTop,
        IndicatorSize,
        LogoSize,
        MessageFontSize,
        ProgressBarHeight,
        ProgressMarginTop,
        SubtitleFontSize,
        SubtleForeground,
        SuccessColor,
        SurfaceBackground,
        SurfaceBoxShadow,
        SurfaceCornerRadius,
        TitleFontSize,
        TitleLineHeight,
        WindowMinHeight,
        WindowWidth
    }

    public class SplashTokenResourceExtension : TokenResourceExtension<SplashTokenKind>
    {
        public SplashTokenResourceExtension()
        {
        }

        public SplashTokenResourceExtension(SplashTokenKind kind) : base(kind)
        {
        }
    }
}