using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class SplashToken : AbstractControlDesignToken
{

    public double WindowWidth { get; set; }
    public double WindowMinHeight { get; set; }
    public CornerRadius SurfaceCornerRadius { get; set; }
    public BoxShadows SurfaceBoxShadow { get; set; }
    public Color SurfaceBackground { get; set; }
    public Thickness ContentPadding { get; set; }
    public double ContentGap { get; set; }
    public Thickness ProgressMarginTop { get; set; }
    public Thickness FooterMarginTop { get; set; }
    public double LogoSize { get; set; }
    public double TitleFontSize { get; set; }
    public double TitleLineHeight { get; set; }
    public double SubtitleFontSize { get; set; }
    public double MessageFontSize { get; set; }
    public double DetailFontSize { get; set; }
    public Color SubtleForeground { get; set; }
    public double IndicatorSize { get; set; }
    public double ProgressBarHeight { get; set; }
    public Color SuccessColor { get; set; }
    public Color ErrorColor { get; set; }
    public Color AccentColor { get; set; }

    public SplashToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        WindowWidth          = 420;
        WindowMinHeight      = 280;
        SurfaceCornerRadius  = EffectiveGlobalToken.BorderRadiusLG;
        SurfaceBoxShadow     = EffectiveGlobalToken.BoxShadowsSecondary;
        SurfaceBackground    = EffectiveGlobalToken.ColorBgContainer;
        ContentPadding       = new Thickness(EffectiveGlobalToken.SizeLG, EffectiveGlobalToken.SizeLG + EffectiveGlobalToken.SizeSM);
        ContentGap           = EffectiveGlobalToken.UniformlyMarginSM;
        ProgressMarginTop    = new Thickness(0, EffectiveGlobalToken.UniformlyMarginSM, 0, 0);
        FooterMarginTop      = new Thickness(0, EffectiveGlobalToken.UniformlyMargin, 0, 0);
        LogoSize             = EffectiveGlobalToken.ControlHeightLG + EffectiveGlobalToken.SizeXS;
        TitleFontSize        = EffectiveGlobalToken.FontSizeHeading4;
        TitleLineHeight      = EffectiveGlobalToken.FontSizeHeading4 * EffectiveGlobalToken.RelativeLineHeightHeading4;
        SubtitleFontSize     = EffectiveGlobalToken.FontSize;
        MessageFontSize      = EffectiveGlobalToken.FontSize;
        DetailFontSize       = EffectiveGlobalToken.FontSizeSM;
        SubtleForeground     = EffectiveGlobalToken.ColorTextDescription;
        IndicatorSize        = EffectiveGlobalToken.ControlHeight;
        ProgressBarHeight    = Math.Max(4, EffectiveGlobalToken.LineWidth * 4);
        SuccessColor         = EffectiveGlobalToken.ColorSuccess;
        ErrorColor           = EffectiveGlobalToken.ColorError;
        AccentColor          = EffectiveGlobalToken.ColorPrimary;
    }

}
