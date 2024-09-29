﻿using AtomUI.Media;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class CalendarToken : AbstractControlDesignToken
{
    public const string ID = "Calendar";

    public CalendarToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 单元格悬浮态背景色
    /// </summary>
    public Color CellHoverBg { get; set; }

    /// <summary>
    /// 选取范围内的单元格背景色
    /// </summary>
    public Color CellActiveWithRangeBg { get; set; }

    /// <summary>
    /// 选取范围内的单元格悬浮态背景色
    /// </summary>
    public Color CellHoverWithRangeBg { get; set; }

    /// <summary>
    /// 单元格禁用态背景色
    /// </summary>
    public Color CellBgDisabled { get; set; }

    /// <summary>
    /// 选取范围时单元格边框色
    /// </summary>
    public Color CellRangeBorderColor { get; set; }

    /// <summary>
    /// 单元格高度
    /// </summary>
    public double CellHeight { get; set; }

    /// <summary>
    /// 单元格宽度
    /// </summary>
    public double CellWidth { get; set; }

    /// <summary>
    /// 单元格行高
    /// </summary>
    public double CellLineHeight { get; set; }

    /// <summary>
    /// 单元格外边距
    /// </summary>
    public Thickness CellMargin { get; set; }

    /// <summary>
    /// 日历项最小宽度
    /// </summary>
    public double ItemPanelMinWidth { get; set; }

    /// <summary>
    /// 日历项最小高度
    /// </summary>
    public double ItemPanelMinHeight { get; set; }

    /// <summary>
    /// 单元格文本高度
    /// </summary>
    public double TextHeight { get; set; }

    /// <summary>
    /// 面板内容内边距
    /// </summary>
    public Thickness PanelContentPadding { get; set; }

    /// <summary>
    /// 十年/年/季/月/周单元格高度
    /// </summary>
    public double WithoutTimeCellHeight { get; set; }

    /// <summary>
    /// 星期的高度
    /// </summary>
    public GridLength DayTitleHeight { get; set; }

    /// <summary>
    /// Header 头外边距
    /// </summary>
    public Thickness HeaderMargin { get; set; }

    /// <summary>
    /// 范围日历间距
    /// </summary>
    public double RangeCalendarSpacing { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();

        var colorPrimary = _globalToken.ColorPrimary;

        CellHoverBg           = _globalToken.ControlItemBgHover;
        CellActiveWithRangeBg = _globalToken.ControlItemBgActive;
        CellHoverWithRangeBg  = colorPrimary.Lighten(35);
        CellRangeBorderColor  = colorPrimary.Lighten(20);
        CellBgDisabled        = _globalToken.ColorBgContainerDisabled;
        CellWidth             = _globalToken.ControlHeightSM;
        CellHeight            = _globalToken.ControlHeightSM;
        TextHeight            = _globalToken.ControlHeightLG;
        WithoutTimeCellHeight = _globalToken.ControlHeightLG * 1.65;
        CellMargin            = new Thickness(_globalToken.MarginXXS);
        PanelContentPadding   = new Thickness(_globalToken.PaddingSM);
        ItemPanelMinWidth     = 260;
        ItemPanelMinHeight    = 290;
        DayTitleHeight        = new GridLength(40, GridUnitType.Pixel);
        HeaderMargin          = new Thickness(0, 0, 0, _globalToken.MarginXS);
        CellLineHeight        = CellHeight - 2; // 不知道为啥设置成一样，或者不设置文字有些靠下
        RangeCalendarSpacing  = 20;
    }
}