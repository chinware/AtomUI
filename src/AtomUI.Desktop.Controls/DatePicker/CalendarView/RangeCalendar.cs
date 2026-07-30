using System.Text;
using AtomUI.Desktop.Controls.CalendarView.State;
using Avalonia;

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

    public static readonly StyledProperty<bool> IsSelectRangeStartProperty =
        AvaloniaProperty.Register<RangeCalendar, bool>(nameof(IsSelectRangeStart), true);

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
    public bool IsSelectRangeStart
    {
        get => GetValue(IsSelectRangeStartProperty);
        set => SetValue(IsSelectRangeStartProperty, value);
    }
    
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

    static RangeCalendar()
    {
        SecondarySelectedDateProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnSecondarySelectedDateChanged(e));
        IsSelectRangeStartProperty.Changed.AddClassHandler<RangeCalendar>((x, e) => x.OnIsSelectRangeStartChanged(e));
    }

    protected virtual void OnSecondarySelectedDateChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var selectedDate = change.NewValue as DateTime?;
        if (selectedDate.HasValue)
        {
            var normalizedDate = NormalizePickerDate(selectedDate.Value);
            if (DateTimeHelper.CompareDays(normalizedDate, selectedDate.Value) != 0)
            {
                SetCurrentValue(SecondarySelectedDateProperty, normalizedDate);
                return;
            }
        }

        if (!IsValidDateSelection(this, selectedDate))
        {
            SetCurrentValue(SecondarySelectedDateProperty, change.OldValue as DateTime?);
            SyncViewStateFromCurrentProperties();
            throw new ArgumentOutOfRangeException(nameof(change), "SecondarySelectedDate value is not valid.");
        }

        UpdateMonths();
        SyncViewStateFromCurrentProperties();
    }

    protected virtual void OnIsSelectRangeStartChanged(AvaloniaPropertyChangedEventArgs change)
    {
        UpdateHighlightDays();
    }

    protected override void SyncViewStateFromCurrentProperties()
    {
        base.SyncViewStateFromCurrentProperties();
        ApplyViewStateAction(CalendarViewAction.SetRangeSelection(
            SelectedDate,
            SecondarySelectedDate,
            HoverDateTime,
            IsSelectRangeStart ? CalendarRangeActivePart.Start : CalendarRangeActivePart.End,
            IsRepairReverseRange));
    }

    protected override bool ShouldSelectedDateUpdateDisplayDate(DateTime selectedDate)
    {
        return IsSelectRangeStart;
    }
    
    protected override void SetupDisplayDateInternal(DateTime displayDate)
    {
        base.SetupDisplayDateInternal(displayDate);
        SecondaryDisplayDateInternal = DateTimeHelper.AddMonths(DisplayDateInternal, 1) ?? DisplayDateInternal;
    }

    internal override void UpdateHighlightDays()
    {
        SyncViewStateFromCurrentProperties();
        UpdateMonths();
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
        HoverDateTime = HoverDate;
        UpdateHighlightDays();
    }

    internal override void SelectPickerDate(DateTime date)
    {
        var normalizedDate = NormalizePickerDate(date);
        if (!IsValidDateSelection(this, normalizedDate))
        {
            return;
        }

        if (IsSelectRangeStart)
        {
            SetCurrentValue(SelectedDateProperty, normalizedDate);
        }
        else
        {
            SetCurrentValue(SecondarySelectedDateProperty, normalizedDate);
        }

        NotifyDateSelected(normalizedDate);
        if (SelectedDate is not null && SecondarySelectedDate is not null)
        {
            NotifyRangeDateSelected();
        }

        HoverDateTime = null;
        UpdateHighlightDays();
    }
    
}
