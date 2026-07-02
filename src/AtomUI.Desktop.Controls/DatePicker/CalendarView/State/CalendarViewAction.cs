using System.Globalization;

namespace AtomUI.Desktop.Controls.CalendarView.State;

internal abstract record CalendarViewAction
{
    private CalendarViewAction()
    {
    }

    public sealed record SetDisplayDateAction(DateTime Date) : CalendarViewAction;
    public sealed record SetDisplayRangeAction(DateTime? Start, DateTime? End) : CalendarViewAction;
    public sealed record SelectDateAction(DateTime Date) : CalendarViewAction;
    public sealed record SetSelectedDateAction(DateTime? Date) : CalendarViewAction;
    public sealed record SetSelectedMonthAction(DateTime Date) : CalendarViewAction;
    public sealed record SetSelectedYearAction(DateTime Date) : CalendarViewAction;
    public sealed record SetFocusedDateAction(DateTime? Date) : CalendarViewAction;
    public sealed record SetBlackoutDatesAction(IReadOnlyList<CalendarDateRange> Dates) : CalendarViewAction;
    public sealed record SetRangeSelectionAction(
        DateTime? Start,
        DateTime? End,
        DateTime? HoverDate,
        CalendarRangeActivePart ActivePart,
        bool RepairReverseRange) : CalendarViewAction;
    public sealed record SetDisplayModeAction(CalendarMode Mode) : CalendarViewAction;
    public sealed record SetFirstDayOfWeekAction(DayOfWeek FirstDayOfWeek) : CalendarViewAction;
    public sealed record SetTodayHighlightedAction(bool IsTodayHighlighted) : CalendarViewAction;
    public sealed record SetCultureAction(DateTimeFormatInfo Culture) : CalendarViewAction;

    public static CalendarViewAction SetDisplayDate(DateTime date)
    {
        return new SetDisplayDateAction(date);
    }

    public static CalendarViewAction SetDisplayRange(DateTime? start, DateTime? end)
    {
        return new SetDisplayRangeAction(start, end);
    }

    public static CalendarViewAction SelectDate(DateTime date)
    {
        return new SelectDateAction(date);
    }

    public static CalendarViewAction SetSelectedDate(DateTime? date)
    {
        return new SetSelectedDateAction(date);
    }

    public static CalendarViewAction SetSelectedMonth(DateTime date)
    {
        return new SetSelectedMonthAction(date);
    }

    public static CalendarViewAction SetSelectedYear(DateTime date)
    {
        return new SetSelectedYearAction(date);
    }

    public static CalendarViewAction SetFocusedDate(DateTime? date)
    {
        return new SetFocusedDateAction(date);
    }

    public static CalendarViewAction SetBlackoutDates(IEnumerable<CalendarDateRange> dates)
    {
        return new SetBlackoutDatesAction(dates.ToArray());
    }

    public static CalendarViewAction SetRangeSelection(
        DateTime? start,
        DateTime? end,
        DateTime? hoverDate,
        CalendarRangeActivePart activePart,
        bool repairReverseRange)
    {
        return new SetRangeSelectionAction(start, end, hoverDate, activePart, repairReverseRange);
    }

    public static CalendarViewAction SetDisplayMode(CalendarMode mode)
    {
        return new SetDisplayModeAction(mode);
    }

    public static CalendarViewAction SetFirstDayOfWeek(DayOfWeek firstDayOfWeek)
    {
        return new SetFirstDayOfWeekAction(firstDayOfWeek);
    }

    public static CalendarViewAction SetTodayHighlighted(bool isTodayHighlighted)
    {
        return new SetTodayHighlightedAction(isTodayHighlighted);
    }

    public static CalendarViewAction SetCulture(DateTimeFormatInfo culture)
    {
        return new SetCultureAction(culture);
    }
}
