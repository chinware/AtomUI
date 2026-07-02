namespace AtomUI.Desktop.Controls.CalendarView;

internal class RangeCalendarItem : CalendarItem
{
    protected override Type StyleKeyOverride => typeof(CalendarItem);

    protected override void NotifyCellMouseEntered(CalendarDayButton dayButton, DateTime selectedDate)
    {
        if (Owner is RangeCalendar owner)
        {
            owner.NotifyHoverDateChanged(selectedDate);
            owner.UpdateHighlightDays();
        }
    }

    protected override void NotifyCellMouseLeftButtonDown(CalendarDayButton dayButton)
    {
        if (Owner is RangeCalendar owner)
        {
            if (dayButton.IsEnabled && !dayButton.IsBlackout && dayButton.DataContext is DateTime selectedDate)
            {
                // Set the start or end of the selection
                // range
                if (owner.IsSelectRangeStart)
                {
                    owner.SelectedDate = selectedDate;
                    owner.NotifyDateSelected(selectedDate);
                }
                else
                {
                    owner.SecondarySelectedDate = selectedDate;
                    owner.NotifyDateSelected(selectedDate);
                }

                if (owner.SelectedDate is not null && owner.SecondarySelectedDate is not null)
                {
                    owner.NotifyRangeDateSelected();
                }

                owner.HoverDateTime = null;
                owner.UpdateHighlightDays();
            }
        }
    }

    protected override void NotifyPointerOutMonthView(bool originInMonthView)
    {
        if (Owner is RangeCalendar owner)
        {
            owner.HoverDateTime = null;
            Owner.UpdateHighlightDays();
        }
    }
}
