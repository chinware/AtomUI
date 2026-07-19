using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.Tokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class TreeSelectToken : AbstractControlDesignToken
{
    public const string ID = "TreeSelect";
    
    /// <summary>
    /// 最小的弹窗的宽度
    /// </summary>
    public double MinPopupWidth { get; set; }

    public TreeSelectToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        MinPopupWidth = 300;
    }
    
}
