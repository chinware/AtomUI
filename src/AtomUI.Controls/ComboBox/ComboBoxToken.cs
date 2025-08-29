﻿using AtomUI.Theme.TokenSystem;
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
    /// 打开按钮宽度内间距
    /// </summary>
    public Thickness OpenIndicatorPadding { get; set; }

    /// <summary>
    /// 菜单的圆角
    /// </summary>
    public CornerRadius PopupBorderRadius { get; set; }

    /// <summary>
    /// 菜单 Popup 阴影
    /// </summary>
    public BoxShadows PopupBoxShadows { get; set; }

    /// <summary>
    /// 菜单内容边距
    /// </summary>
    public Thickness PopupContentPadding { get; set; }

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
    /// 列表项内间距
    /// </summary>
    public Thickness ItemPadding { get; set; }

    /// <summary>
    /// 列表项外边距
    /// </summary>
    public Thickness ItemMargin { get; set; }

    /// <summary>
    /// 顶层弹出菜单，距离顶层菜单项的边距
    /// </summary>
    public double PopupMarginToAnchor { get; set; }

    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        OpenIndicatorPadding = new Thickness(SharedToken.UniformlyPaddingXXS, 0, 0, 0);
        PopupBorderRadius    = SharedToken.BorderRadiusLG;
        PopupContentPadding  = new Thickness(SharedToken.UniformlyPaddingXXS, PopupBorderRadius.TopLeft / 2);
        PopupBoxShadows      = SharedToken.BoxShadowsSecondary;
        PopupMarginToAnchor  = SharedToken.UniformlyMarginXXS;
        
        var colorTextDisabled  = SharedToken.ColorTextDisabled;
        var colorTextSecondary = SharedToken.ColorTextSecondary;
        var colorBgContainer   = SharedToken.ColorBgElevated;
        var colorBgTextHover   = SharedToken.ColorBgTextHover;

        ItemColor         = colorTextSecondary;
        ItemHoverColor    = colorTextSecondary;
        ItemSelectedColor = SharedToken.ColorText;

        ItemBgColor         = colorBgContainer;
        ItemHoverBgColor    = colorBgTextHover;
        ItemSelectedBgColor = SharedToken.ControlItemBgActive;
        ItemDisabledColor = colorTextDisabled;

        ItemPadding = new Thickness(SharedToken.UniformlyPaddingSM, 0);
        ItemMargin  = new Thickness(0, 0.5);
    }
}