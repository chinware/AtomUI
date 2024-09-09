using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ComboBoxToken : ButtonSpinnerToken
{
    public new const string ID = "ComboBox";

    public ComboBoxToken()
        : base(ID)
    {
    }

    /// <summary>
    ///     打开按钮宽度
    /// </summary>
    public double OpenIndicatorWidth { get; set; }

    /// <summary>
    ///     菜单的圆角
    /// </summary>
    public CornerRadius PopupBorderRadius { get; set; }

    /// <summary>
    ///     菜单 Popup 阴影
    /// </summary>
    public BoxShadows PopupBoxShadows { get; set; }

    /// <summary>
    ///     菜单内容边距
    /// </summary>
    public Thickness PopupContentPadding { get; set; }

    /// <summary>
    ///     列表项文字颜色
    /// </summary>
    public Color ItemColor { get; set; }

    /// <summary>
    ///     列表项文字悬浮颜色
    /// </summary>
    public Color ItemHoverColor { get; set; }

    /// <summary>
    ///     列表项文字选中颜色
    /// </summary>
    public Color ItemSelectedColor { get; set; }

    /// <summary>
    ///     列表项文字禁用颜色
    /// </summary>
    public Color ItemDisabledColor { get; set; }

    /// <summary>
    ///     列表项背景色
    /// </summary>
    public Color ItemBgColor { get; set; }

    /// <summary>
    ///     列表项悬浮态背景色
    /// </summary>
    public Color ItemHoverBgColor { get; set; }

    /// <summary>
    ///     列表项选中背景色
    /// </summary>
    public Color ItemSelectedBgColor { get; set; }

    /// <summary>
    ///     列表项内间距
    /// </summary>
    public Thickness ItemPadding { get; set; }

    /// <summary>
    ///     列表项外边距
    /// </summary>
    public Thickness ItemMargin { get; set; }

    /// <summary>
    ///     顶层弹出菜单，距离顶层菜单项的边距
    /// </summary>
    public double PopupMarginToAnchor { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        OpenIndicatorWidth  = _globalToken.IconSizeSM * 2.5;
        PopupBorderRadius   = _globalToken.StyleToken.BorderRadiusLG;
        PopupContentPadding = new Thickness(_globalToken.PaddingXXS, PopupBorderRadius.TopLeft / 2);
        PopupBoxShadows     = _globalToken.BoxShadowsSecondary;
        PopupMarginToAnchor = _globalToken.MarginXXS;

        var colorNeutralToken  = _globalToken.ColorToken.ColorNeutralToken;
        var colorTextDisabled  = _globalToken.ColorTextDisabled;
        var colorTextSecondary = colorNeutralToken.ColorTextSecondary;
        var colorBgContainer   = colorNeutralToken.ColorBgContainer;
        var colorBgTextHover   = _globalToken.ColorBgTextHover;

        ItemColor         = colorTextSecondary;
        ItemHoverColor    = colorTextSecondary;
        ItemSelectedColor = colorNeutralToken.ColorText;

        ItemBgColor         = colorBgContainer;
        ItemHoverBgColor    = colorBgTextHover;
        ItemSelectedBgColor = _globalToken.ControlItemBgActive;

        ItemDisabledColor = colorTextDisabled;

        ItemPadding = new Thickness(_globalToken.PaddingSM, _globalToken.PaddingXS);
        ItemMargin  = new Thickness(0, 0.5);
    }
}