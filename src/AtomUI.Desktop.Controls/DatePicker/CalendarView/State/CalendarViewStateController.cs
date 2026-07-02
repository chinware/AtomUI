namespace AtomUI.Desktop.Controls.CalendarView.State;

internal sealed class CalendarViewStateController
{
    public CalendarViewStateController(CalendarViewState initialState)
    {
        State = initialState;
    }

    public CalendarViewState State { get; private set; }

    public void Apply(CalendarViewAction action)
    {
        State = action switch
        {
            CalendarViewAction.SetDisplayDateAction setDisplayDate =>
                State.WithDisplayDate(setDisplayDate.Date),
            CalendarViewAction.SetDisplayRangeAction setDisplayRange =>
                State.WithDisplayRange(setDisplayRange.Start, setDisplayRange.End),
            CalendarViewAction.SelectDateAction selectDate =>
                State.WithSelectedDate(selectDate.Date).WithDisplayDate(selectDate.Date),
            CalendarViewAction.SetSelectedDateAction setSelectedDate =>
                State.WithSelectedDate(setSelectedDate.Date),
            CalendarViewAction.SetSelectedMonthAction setSelectedMonth =>
                State.WithSelectedMonth(setSelectedMonth.Date),
            CalendarViewAction.SetSelectedYearAction setSelectedYear =>
                State.WithSelectedYear(setSelectedYear.Date),
            CalendarViewAction.SetFocusedDateAction setFocusedDate =>
                State.WithFocusedDate(setFocusedDate.Date),
            CalendarViewAction.SetBlackoutDatesAction setBlackoutDates =>
                State.WithBlackoutDates(setBlackoutDates.Dates),
            CalendarViewAction.SetRangeSelectionAction setRangeSelection =>
                State.WithRangeSelection(
                    setRangeSelection.Start,
                    setRangeSelection.End,
                    setRangeSelection.HoverDate,
                    setRangeSelection.ActivePart,
                    setRangeSelection.RepairReverseRange),
            CalendarViewAction.SetDisplayModeAction setMode =>
                State.WithDisplayMode(setMode.Mode),
            CalendarViewAction.SetPickerModeAction setPickerMode =>
                State.WithPickerMode(setPickerMode.PickerMode),
            CalendarViewAction.SetFirstDayOfWeekAction setFirstDayOfWeek =>
                State.WithFirstDayOfWeek(setFirstDayOfWeek.FirstDayOfWeek),
            CalendarViewAction.SetTodayHighlightedAction setTodayHighlighted =>
                State.WithTodayHighlighted(setTodayHighlighted.IsTodayHighlighted),
            CalendarViewAction.SetCultureAction setCulture =>
                State.WithCulture(setCulture.Culture),
            _ => State
        };
    }
}
