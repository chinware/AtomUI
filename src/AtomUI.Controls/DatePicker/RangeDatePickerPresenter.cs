﻿using AtomUI.Controls.CalendarView;
using Avalonia;

namespace AtomUI.Controls;

internal class RangeDatePickerPresenter : DatePickerPresenter
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime?> SecondarySelectedDateTimeProperty =
        AvaloniaProperty.Register<RangeDatePickerPresenter, DateTime?>(nameof(SecondarySelectedDateTime));
    
    public DateTime? SecondarySelectedDateTime
    {
        get => GetValue(SecondarySelectedDateTimeProperty);
        set => SetValue(SecondarySelectedDateTimeProperty, value);
    }

    #endregion

    internal void NotifySelectRangeStart(bool isStart)
    {
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            rangeCalendar.IsSelectRangeStart = isStart;
        }
    }

    internal void NotifyRepairReverseRange(bool isRepair)
    {
        if (_calendarView is RangeCalendar rangeCalendar)
        {
            rangeCalendar.IsRepairReverseRange = isRepair;
        }
    }
}