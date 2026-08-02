using AtomUI.Theme.DesignTokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class LineEditToken : AbstractControlDesignToken
{
    /// <summary>
    /// 字体大小
    /// </summary>
    public double InputFontSize { get; set; }

    /// <summary>
    /// 大号字体大小
    /// </summary>
    public double InputFontSizeLG { get; set; }

    /// <summary>
    /// 小号字体大小
    /// </summary>
    public double InputFontSizeSM { get; set; }
    
    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        InputFontSize   = EffectiveGlobalToken.FontSize;
        InputFontSizeLG = EffectiveGlobalToken.FontSizeLG;
        InputFontSizeSM = EffectiveGlobalToken.FontSizeSM;
    }

}
