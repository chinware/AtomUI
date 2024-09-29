﻿using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TabControlToken : AbstractControlDesignToken
{
    public const string ID = "TabControl";

    public TabControlToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 卡片标签页背景色
    /// </summary>
    public Color CardBg { get; set; }

    /// <summary>
    /// 卡片标签页大小
    /// </summary>
    public double CardSize { get; set; }

    /// <summary>
    /// 卡片标签页内间距
    /// </summary>
    public Thickness CardPadding { get; set; }

    /// <summary>
    /// 小号卡片标签页内间距
    /// </summary>
    public Thickness CardPaddingSM { get; set; }

    /// <summary>
    /// 大号卡片标签页内间距
    /// </summary>
    public Thickness CardPaddingLG { get; set; }

    /// <summary>
    /// 标齐页标题文本大小
    /// </summary>
    public double TitleFontSize { get; set; }

    /// <summary>
    /// 大号标签页标题文本大小
    /// </summary>
    public double TitleFontSizeLG { get; set; }

    /// <summary>
    /// 小号标签页标题文本大小
    /// </summary>
    public double TitleFontSizeSM { get; set; }

    /// <summary>
    /// 指示条颜色
    /// </summary>
    public Color InkBarColor { get; set; }

    /// <summary>
    /// 横向标签页外间距
    /// </summary>
    public Thickness HorizontalMargin { get; set; }

    /// <summary>
    /// 横向标签页标签间距
    /// </summary>
    public double HorizontalItemGutter { get; set; }

    /// <summary>
    /// 横向标签页标签外间距
    /// </summary>
    public Thickness HorizontalItemMargin { get; set; }

    /// <summary>
    /// 横向标签页标签内间距
    /// </summary>
    public Thickness HorizontalItemPadding { get; set; }

    /// <summary>
    /// 大号横向标签页标签内间距
    /// </summary>
    public Thickness HorizontalItemPaddingLG { get; set; }

    /// <summary>
    /// 小号横向标签页标签内间距
    /// </summary>
    public Thickness HorizontalItemPaddingSM { get; set; }

    /// <summary>
    /// 纵向标签页标签间距
    /// </summary>
    public double VerticalItemGutter { get; set; }

    /// <summary>
    /// 纵向标签页标签内间距
    /// </summary>
    public Thickness VerticalItemPadding { get; set; }

    /// <summary>
    /// 标签文本颜色
    /// </summary>
    public Color ItemColor { get; set; }

    /// <summary>
    /// 标签悬浮态文本颜色
    /// </summary>
    public Color ItemHoverColor { get; set; }

    /// <summary>
    /// 标签选中态文本颜色
    /// </summary>
    public Color ItemSelectedColor { get; set; }

    /// <summary>
    /// 卡片标签间距
    /// </summary>
    public double CardGutter { get; set; }

    /// <summary>
    /// 标签内容 icon 的外边距
    /// </summary>
    public Thickness ItemIconMargin { get; set; }

    /// <summary>
    /// 水平布局的菜单外边距
    /// </summary>
    public Thickness MenuIndicatorPaddingHorizontal { get; set; }

    /// <summary>
    /// 垂直布局的菜单外边距
    /// </summary>
    public Thickness MenuIndicatorPaddingVertical { get; set; }

    /// <summary>
    /// 滚动边缘的厚度
    /// </summary>
    public double MenuEdgeThickness { get; set; }

    /// <summary>
    /// 水平添加按钮外边距
    /// </summary>
    public Thickness AddTabButtonMarginHorizontal { get; set; }

    /// <summary>
    /// 垂直添加按钮外边距
    /// </summary>
    public Thickness AddTabButtonMarginVertical { get; set; }

    /// <summary>
    /// 关闭按钮外边距
    /// </summary>
    public Thickness CloseIconMargin { get; set; }

    /// <summary>
    /// Tab 标签和内容区域的间距
    /// </summary>
    public double TabAndContentGutter { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var lineHeight = _globalToken.LineHeight;
        var lineWidth  = _globalToken.LineWidth;

        CardBg = _globalToken.ColorFillAlter;

        CardSize = _globalToken.ControlHeightLG;

        CardPadding = new Thickness(_globalToken.Padding,
            (CardSize - Math.Round(_globalToken.FontSize * lineHeight)) / 2 - lineWidth);
        CardPaddingSM = new Thickness(_globalToken.Padding, _globalToken.PaddingXXS * 1.5);
        CardPaddingLG = new Thickness(top: _globalToken.PaddingXS,
            bottom: _globalToken.PaddingXXS * 1.5,
            left: _globalToken.Padding,
            right: _globalToken.Padding);

        TitleFontSize   = _globalToken.FontSize;
        TitleFontSizeLG = _globalToken.FontSizeLG;
        TitleFontSizeSM = _globalToken.FontSize;

        InkBarColor = _globalToken.ColorPrimary;

        HorizontalMargin     = new Thickness(0, 0, _globalToken.Margin, 0);
        HorizontalItemGutter = 32;
        HorizontalItemMargin = new Thickness();

        HorizontalItemPadding   = new Thickness(0, _globalToken.PaddingSM);
        HorizontalItemPaddingSM = new Thickness(0, _globalToken.PaddingXS);
        HorizontalItemPaddingLG = new Thickness(0, _globalToken.Padding);

        VerticalItemGutter  = _globalToken.Margin;
        VerticalItemPadding = new Thickness(_globalToken.PaddingXS, _globalToken.PaddingXS);

        ItemColor         = _globalToken.ColorText;
        ItemSelectedColor = _globalToken.ColorPrimary;
        ItemHoverColor    = _globalToken.ColorPrimaryHover;

        CardGutter                   = _globalToken.MarginXXS / 2;
        AddTabButtonMarginHorizontal = new Thickness(CardGutter, 0, 0, 0);
        AddTabButtonMarginVertical   = new Thickness(0, CardGutter, 0, 0);
        ItemIconMargin               = new Thickness(0, 0, _globalToken.MarginSM, 0);

        MenuIndicatorPaddingHorizontal = new Thickness(_globalToken.PaddingXS, 0, 0, 0);
        MenuIndicatorPaddingVertical   = new Thickness(0, _globalToken.PaddingXS, 0, 0);
        CloseIconMargin                = new Thickness(_globalToken.MarginXXS, 0, 0, 0);

        MenuEdgeThickness   = 20;
        TabAndContentGutter = _globalToken.MarginSM;
    }
}