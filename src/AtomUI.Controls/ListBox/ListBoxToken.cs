﻿using AtomUI.Theme.TokenSystem;
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

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var colorTextDisabled  = _globalToken.ColorTextDisabled;
        var colorTextSecondary = _globalToken.ColorTextSecondary;
        var colorBgContainer   = _globalToken.ColorBgContainer;
        var colorBgTextHover   = _globalToken.ColorBgTextHover;

        ItemColor         = colorTextSecondary;
        ItemHoverColor    = colorTextSecondary;
        ItemSelectedColor = _globalToken.ColorText;

        ItemBgColor         = colorBgContainer;
        ItemHoverBgColor    = colorBgTextHover;
        ItemSelectedBgColor = _globalToken.ControlItemBgActive;

        ItemDisabledColor = colorTextDisabled;

        ItemPaddingLG = new Thickness(_globalToken.Padding);
        ItemPaddingSM = new Thickness(_globalToken.PaddingXS, _globalToken.PaddingXS);
        ItemPadding   = new Thickness(_globalToken.PaddingSM, _globalToken.PaddingXS);

        ContentPadding = new Thickness(_globalToken.PaddingXXS / 2);
        ItemMargin     = new Thickness(0, 0.5);
    }
}