using System.Diagnostics;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Controls.CalendarView;

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
    protected override Type StyleKeyOverride => typeof(RangeCalendar);
    
    #region 公共属性定义

    public static readonly StyledProperty<DateTime?> SecondarySelectedDateProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(nameof(SecondarySelectedDate),
            defaultBindingMode: BindingMode.TwoWay);

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
            if (targetMonthView.Children[i] is CalendarDayButton b)
            {
                var d = b.DataContext as DateTime?;
                if (d.HasValue)
                {
                    if (rangeStart is not null && rangeEnd is not null)
                    {
                        b.IsSelected = DateTimeHelper.InRange(d.Value, rangeStart.Value, rangeEnd.Value);
                    } else if (SelectedDate is not null)
                    {
                        b.IsSelected = DateTimeHelper.CompareDays(SelectedDate.Value, d.Value) == 0;
                    }
                    else if (SecondarySelectedDate is not null)
                    {
                        b.IsSelected = DateTimeHelper.CompareDays(SecondarySelectedDate.Value, d.Value) == 0;
                    }
                    else
                    {
                        b.IsSelected = false;
                    }

                    if (b.IsSelected)
                    {
                        if (FocusButton != null)
                        {
                            FocusButton.IsCurrent = false;
                        }
                        
                        b.IsCurrent = HasFocusInternal;
                        FocusButton = b;
                    }
                }
                else
                {
                    b.IsSelected = false;
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