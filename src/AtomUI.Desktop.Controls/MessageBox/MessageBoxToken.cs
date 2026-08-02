using AtomUI.Theme.DesignTokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class MessageBoxToken : AbstractControlDesignToken
{

    /// <summary>
    /// Style Icon 的大小
    /// </summary>
    public double StyleIconSize { get; set; }
    
    /// <summary>
    /// 最小宽度
    /// </summary>
    public double MinWidth { get; set; }

    public MessageBoxToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        StyleIconSize = EffectiveGlobalToken.SizeLG * 1.2;
        MinWidth      = 410;
    }

}
