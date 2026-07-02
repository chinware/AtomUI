namespace AtomUI.Desktop.Controls.CalendarView;

internal class RangeCalendarItem : CalendarItem
{
    protected override Type StyleKeyOverride => typeof(CalendarItem);

    protected override void NotifyCellMouseEntered(CalendarDayButton dayButton, DateTime selectedDate)
    {
        if (Owner is RangeCalendar owner)
        {
            owner.NotifyHoverDateChanged(selectedDate);
        }
    }

    protected override void NotifyMonthMouseEntered(CalendarButton calendarButton, DateTime selectedDate)
    {
        if (Owner is RangeCalendar owner)
        {
            owner.NotifyHoverDateChanged(selectedDate);
        }
    }

    protected override void NotifyCellMouseLeftButtonDown(CalendarDayButton dayButton)
    {
        if (Owner is RangeCalendar owner)
        {
            if (dayButton.IsEnabled && !dayButton.IsBlackout && dayButton.DataContext is DateTime selectedDate)
            {
                owner.SelectPickerDate(selectedDate);
            }
        }
    }

    protected override void NotifyPointerOutMonthView(bool originInMonthView)
    {
        if (originInMonthView && Owner is RangeCalendar owner)
        {
            owner.NotifyHoverDateChanged(null);
        }
    }
}
