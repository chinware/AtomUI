using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ListBoxToken : AbstractControlDesignToken
{
    public const string ID = "ListBox";

    public ListBoxToken()
        : this(ID)
    {
    }

    protected ListBoxToken(string id)
        : base(id)
    {
    }

    /// <summary>
    /// ListBox 内边距
    /// </summary>
    public Thickness ContentPadding { get; set; }

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
    /// 列表项文字禁用颜色
    /// </summary>
    public Color ItemDisabledColor { get; set; }

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

    /// <summary>
    /// 列表项小号内间距
    /// </summary>
    public Thickness ItemPaddingSM { get; set; }

    /// <summary>
    /// 列表项内间距
    /// </summary>
    public Thickness ItemPadding { get; set; }

    /// <summary>
    /// 列表项大号内间距
    /// </summary>
    public Thickness ItemPaddingLG { get; set; }

    /// <summary>
    /// 列表项外边距
    /// </summary>
    public Thickness ItemMargin { get; set; }

    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var colorTextDisabled  = SharedToken.ColorTextDisabled;
        var colorTextSecondary = SharedToken.ColorTextSecondary;
        var colorBgContainer   = SharedToken.ColorBgContainer;
        var colorBgTextHover   = SharedToken.ColorBgTextHover;

        ItemColor         = colorTextSecondary;
        ItemHoverColor    = colorTextSecondary;
        ItemSelectedColor = SharedToken.ColorText;

        ItemBgColor         = colorBgContainer;
        ItemHoverBgColor    = colorBgTextHover;
        ItemSelectedBgColor = SharedToken.ControlItemBgActive;

        ItemDisabledColor = colorTextDisabled;

        ItemPaddingLG = SharedToken.Padding;
        ItemPaddingSM = new Thickness(SharedToken.UniformlyPaddingXS, SharedToken.UniformlyPaddingXS);
        ItemPadding   = new Thickness(SharedToken.UniformlyPaddingSM, SharedToken.UniformlyPaddingXS);

        ContentPadding = new Thickness(SharedToken.UniformlyPaddingXXS / 2);
        ItemMargin     = new Thickness(0, 0.5);
    }
}