using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class ResultToken : AbstractControlDesignToken
{

    /// <summary>
    /// 标题字体大小
    /// </summary>
    public double HeaderFontSize { get; set; }

    /// <summary>
    /// 副标题字体大小
    /// </summary>
    public double SubHeaderFontSize { get; set; }

    /// <summary>
    /// 图标大小
    /// </summary>
    public double StatusIconSize { get; set; }

    /// <summary>
    /// 额外区域外间距
    /// </summary>
    public Thickness ExtraMargin { get; set; }

    public double ImageWidth { get; set; }
    public double ImageHeight { get; set; }

    public Color ResultInfoIconColor { get; set; }
    public Color ResultSuccessIconColor { get; set; }
    public Color ResultWarningIconColor { get; set; }
    public Color ResultErrorIconColor { get; set; }
    public Thickness FramePadding { get; set; }
    public Thickness ContentPadding { get; set; }
    public Thickness ContentMargin { get; set; }
    public Thickness HeaderMargin { get; set; }
    public Thickness StatusImageMargin { get; set; }

    public ResultToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        HeaderFontSize    = EffectiveGlobalToken.FontSizeHeading3;
        SubHeaderFontSize = EffectiveGlobalToken.FontSize;
        StatusIconSize    = EffectiveGlobalToken.FontSizeHeading3 * 3;
        ExtraMargin       = new Thickness(0, EffectiveGlobalToken.UniformlyMargin, 0, 0);

        ImageWidth  = 250;
        ImageHeight = 295;

        ResultInfoIconColor    = EffectiveGlobalToken.ColorInfo;
        ResultSuccessIconColor = EffectiveGlobalToken.ColorSuccess;
        ResultWarningIconColor = EffectiveGlobalToken.ColorWarning;
        ResultErrorIconColor   = EffectiveGlobalToken.ColorError;

        ContentPadding    = new Thickness(EffectiveGlobalToken.UniformlyPadding * 2.5, EffectiveGlobalToken.UniformlyPaddingLG);
        ContentMargin     = new Thickness(0, EffectiveGlobalToken.UniformlyPaddingLG, 0, 0);
        StatusImageMargin = new Thickness(0, 0, 0, EffectiveGlobalToken.UniformlyMargin);
        HeaderMargin      = new Thickness(0, EffectiveGlobalToken.UniformlyMarginXS);
        FramePadding      = new Thickness(EffectiveGlobalToken.UniformlyPaddingLG * 2, EffectiveGlobalToken.UniformlyMarginXL);
    }
    
}
