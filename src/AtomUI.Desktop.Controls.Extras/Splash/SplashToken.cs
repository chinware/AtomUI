using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class SplashToken : AbstractControlDesignToken
{
    public const string ID = "Splash";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

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
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        WindowWidth          = 420;
        WindowMinHeight      = 280;
        SurfaceCornerRadius  = SharedToken.BorderRadiusLG;
        SurfaceBoxShadow     = SharedToken.BoxShadowsSecondary;
        SurfaceBackground    = SharedToken.ColorBgContainer;
        ContentPadding       = new Thickness(SharedToken.SizeLG, SharedToken.SizeLG + SharedToken.SizeSM);
        ContentGap           = SharedToken.UniformlyMarginSM;
        ProgressMarginTop    = new Thickness(0, SharedToken.UniformlyMarginSM, 0, 0);
        FooterMarginTop      = new Thickness(0, SharedToken.UniformlyMargin, 0, 0);
        LogoSize             = SharedToken.ControlHeightLG + SharedToken.SizeXS;
        TitleFontSize        = SharedToken.FontSizeHeading4;
        TitleLineHeight      = SharedToken.FontSizeHeading4 * SharedToken.RelativeLineHeightHeading4;
        SubtitleFontSize     = SharedToken.FontSize;
        MessageFontSize      = SharedToken.FontSize;
        DetailFontSize       = SharedToken.FontSizeSM;
        SubtleForeground     = SharedToken.ColorTextDescription;
        IndicatorSize        = SharedToken.ControlHeight;
        ProgressBarHeight    = Math.Max(4, SharedToken.LineWidth * 4);
        SuccessColor         = SharedToken.ColorSuccess;
        ErrorColor           = SharedToken.ColorError;
        AccentColor          = SharedToken.ColorPrimary;
    }

    protected override Type GetTokenKindType() => typeof(SplashTokenKind);
}
