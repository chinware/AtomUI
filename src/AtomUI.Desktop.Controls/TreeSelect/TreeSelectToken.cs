using AtomUI.Theme.DesignTokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class TreeSelectToken : AbstractControlDesignToken
{
    
    /// <summary>
    /// 最小的弹窗的宽度
    /// </summary>
    public double MinPopupWidth { get; set; }

    public TreeSelectToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        MinPopupWidth = 300;
    }
    
}
