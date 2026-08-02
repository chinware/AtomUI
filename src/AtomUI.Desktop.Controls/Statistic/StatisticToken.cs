using AtomUI.Theme.DesignTokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class StatisticToken : AbstractControlDesignToken
{
    
    /// <summary>
    /// 标题字体大小
    /// Title font size
    /// </summary>
    public double TitleFontSize { get; set; }
    
    /// <summary>
    /// 内容字体大小
    /// Content font size
    /// </summary>
    public double ContentFontSize { get; set; }
    
    public StatisticToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        TitleFontSize = EffectiveGlobalToken.FontSize;
        ContentFontSize = EffectiveGlobalToken.FontSizeHeading3;
    }
    
}