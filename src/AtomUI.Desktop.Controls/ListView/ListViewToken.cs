using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class ListViewToken : AbstractControlDesignToken
{
    public const string ID = "ListView";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    public ListViewToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 列表项文字颜色
    /// </summary>
    public Color ItemColor { get; set; }

    /// <summary>
    /// 列表项文字悬浮颜色
    /// </summary>
    public Color ItemHoverColor { get; set; }

    /// <summary>
    /// 列表项文字选中颜色
    /// </summary>
    public Color ItemSelectedColor { get; set; }

    /// <summary>
    /// 列表项背景色
    /// </summary>
    public Color ItemBgColor { get; set; }

    /// <summary>
    /// 列表项悬浮态背景色
    /// </summary>
    public Color ItemHoverBgColor { get; set; }

    /// <summary>
    /// 列表项选中背景色
    /// </summary>
    public Color ItemSelectedBgColor { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        ItemColor           = SharedToken.ColorText;
        ItemHoverColor      = SharedToken.ColorText;
        ItemSelectedColor   = SharedToken.ColorText;
        ItemBgColor         = Colors.Transparent;
        ItemHoverBgColor    = SharedToken.ControlItemBgHover;
        ItemSelectedBgColor = SharedToken.ControlItemBgActive;
    }

    protected override Type GetTokenKindType() => typeof(ListViewTokenKind);
}
