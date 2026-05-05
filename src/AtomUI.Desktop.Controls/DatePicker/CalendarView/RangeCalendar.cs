using System.Diagnostics;
using System.Text;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls.CalendarView;

public class RangeDateSelectedEventArgs : EventArgs
{
    public CalendarDateRange Range { get; }
    public bool IsFixedRange { get; }
    
    public RangeDateSelectedEventArgs(CalendarDateRange range, bool isFixedRange)
    {
        Range        = range;
        IsFixedRange = isFixedRange;
    }
}

internal class RangeCalendar : Calendar
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime?> SecondarySelectedDateProperty =
        AvaloniaProperty.Register<RangeCalendar, DateTime?>(nameof(SecondarySelectedDate));

    public DateTime? SecondarySelectedDate
    {
        get => GetValue(SecondarySelectedDateProperty);
        set => SetValue(SecondarySelectedDateProperty, value);
    }
    
    /// <summary>
    /// 是否修复选择结果，当开始日期大于结束日期的时候进行位置调换
    /// </summary>
    public bool IsRepairReverseRange { get; set; } = true;
    
    /// <summary>
    /// 当前是否在选择范围开始日期
    /// </summary>
    public bool IsSelectRangeStart { get; set; } = true;
    
    #endregion

    #region 内部属性定义

    internal DateTime SecondaryDisplayDateInternal { get; set; }
    internal DateTime? HoverDateTime { get; set; }

    #endregion

    #region 公共事件定义

    /// <summary>
    /// 当范围选择完成的时候派发这个事件
    /// </summary>
    public event EventHandler<RangeDateSelectedEventArgs>? RangeDateSelected;

    #endregion
    
    protected override void SetupDisplayDateInternal(DateTime displayDate)
    {
        base.SetupDisplayDateInternal(displayDate);
        SecondaryDisplayDateInternal = DateTimeHelper.AddMonths(DisplayDateInternal, 1) ?? DisplayDateInternal;
    }

    internal override void UpdateHighlightDays()
    {
        DateTime? rangeStart = default;
        DateTime? rangeEnd   = default;
        SortHoverIndexes(out rangeStart, out rangeEnd);
        Debug.Assert(CalendarItem is not null);
        if (CalendarItem.MonthView is not null)
        { 
            UpdateHighlightDays(CalendarItem.MonthView, rangeStart, rangeEnd);
        }
    }

    protected void UpdateHighlightDays(Grid targetMonthView, DateTime? rangeStart, DateTime? rangeEnd)
    {
        var count = targetMonthView.Children.Count;
        for (var i = 0; i < count; i++)
        {
            if (targetMonthView.Children[i] is CalendarDayButton dayButton)
            {
                if (dayButton.DataContext is DateTime d)
                {
                    if (rangeStart is not null && rangeEnd is not null)
                    {
                        dayButton.IsSelected    = DateTimeHelper.InRange(d, rangeStart.Value, rangeEnd.Value);
                        if (dayButton.IsSelected)
                        {
                            if (DateTimeHelper.CompareDays(d, rangeStart.Value) == 0)
                            {
                                dayButton.IsRangeStart  = true;
                                dayButton.IsRangeMiddle = false;
                                dayButton.IsRangeEnd    = false;
                            }
                            else if (DateTimeHelper.CompareDays(d, rangeEnd.Value) == 0)
                            {
                                dayButton.IsRangeEnd    = true;
                                dayButton.IsRangeStart  = false;
                                dayButton.IsRangeMiddle = false;
                            }
                            else
                            {
                                dayButton.IsRangeMiddle = true;
                                dayButton.IsRangeStart  = false;
                                dayButton.IsRangeEnd    = false;
                            }
                        }
                        else
                        {
                            dayButton.IsRangeMiddle = false;
                            dayButton.IsRangeStart  = false;
                            dayButton.IsRangeEnd    = false;
                        }
                    } else if (SelectedDate is not null)
                    {
                        dayButton.IsSelected = DateTimeHelper.CompareDays(SelectedDate.Value, d) == 0;
                    }
                    else if (SecondarySelectedDate is not null)
                    {
                        dayButton.IsSelected = DateTimeHelper.CompareDays(SecondarySelectedDate.Value, d) == 0;
                    }
                    else
                    {
                        dayButton.IsSelected    = false;
                        dayButton.IsRangeMiddle = false;
                        dayButton.IsRangeStart  = false;
                        dayButton.IsRangeEnd    = false;
                    }

                    if (dayButton.IsSelected)
                    {
                        if (FocusButton != null)
                        {
                            FocusButton.IsCurrent = false;
                        }
                        
                        dayButton.IsCurrent     = HasFocusInternal;
                        FocusButton             = dayButton;
                    }
                }
                else
                {
                    dayButton.IsSelected    = false;
                    dayButton.IsRangeStart  = false;
                    dayButton.IsRangeEnd    = false;
                    dayButton.IsRangeMiddle = false;
                }
            }
        }
    }
    
    internal void SortHoverIndexes(out DateTime? rangeStart, out DateTime? rangeEnd)
    {
        if (IsSelectRangeStart)
        {
            rangeStart = HoverDateTime ?? SelectedDate;
            rangeEnd   = SecondarySelectedDate;
        }
        else
        {
            rangeStart = SelectedDate;
            rangeEnd   = HoverDateTime ?? SecondarySelectedDate;
        }
        if (rangeStart is not null && rangeEnd is not null)
        {
            if (DateTimeHelper.CompareDateTime(rangeEnd.Value, rangeStart.Value) < 0)
            {
                var temp = rangeStart.Value;
                rangeStart = rangeEnd;
                rangeEnd   = temp;
            }
        }
    }
    
    public override string ToString()
    {
        if (SelectedDate != null || SecondarySelectedDate != null)
        {
            var builder = new StringBuilder();
            if (SelectedDate != null)
            {
                builder.Append(SelectedDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat()));
            }
            else
            {
                builder.Append('?');
            }

            builder.Append(" - ");
            if (SecondarySelectedDate != null)
            {
                builder.Append(SecondarySelectedDate.Value.ToString(DateTimeHelper.GetCurrentDateFormat()));
            }
            else
            {
                builder.Append('?');
            }

            return builder.ToString();
        }

        return string.Empty;
    }
    
    internal void NotifyRangeDateSelected()
    {
        if (SelectedDate is not null && SecondarySelectedDate is not null)
        {
            var  rangeStart   = SelectedDate.Value;
            var  rangeEnd     = SecondarySelectedDate.Value;
            bool isFixedRange = false;
            if (DateTimeHelper.CompareDays(SelectedDate.Value, SecondarySelectedDate.Value) > 0 && IsRepairReverseRange)
            {
                rangeStart   = SecondarySelectedDate.Value;
                rangeEnd     = SelectedDate.Value;
                isFixedRange = true;
            }
            RangeDateSelected?.Invoke(this, new RangeDateSelectedEventArgs(new CalendarDateRange(rangeStart, rangeEnd), isFixedRange));
        }
    }

    internal override void NotifyHoverDateChanged(DateTime? hoverDate)
    {
        base.NotifyHoverDateChanged(hoverDate);
        HoverDateTime = hoverDate;
    }
    
}