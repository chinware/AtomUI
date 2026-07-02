using System.Globalization;

namespace AtomUI.Desktop.Controls.CalendarView.State;

internal sealed record CalendarViewState
{
    private CalendarViewState(DateTime displayDate, DateTimeFormatInfo culture)
    {
        DisplayDate          = DateTimeHelper.DiscardDayTime(displayDate);
        SecondaryDisplayDate = DateTimeHelper.AddMonths(DisplayDate, 1) ?? DisplayDate;
        DisplayDateStart     = DateTime.MinValue;
        DisplayDateEnd       = DateTime.MaxValue;
        SelectedMonth        = DisplayDate;
        SelectedYear         = DisplayDate;
        FirstDayOfWeek       = culture.FirstDayOfWeek;
        IsTodayHighlighted   = true;
        Culture              = culture;
        Today                = DateTime.Today;
        BlackoutDates        = Array.Empty<CalendarDateRange>();
        RangeSelection       = new CalendarRangeSelectionState(null, null, null, CalendarRangeActivePart.Start, true);
    }

    public CalendarMode DisplayMode { get; init; } = CalendarMode.Month;
    public DateTime DisplayDate { get; init; }
    public DateTime SecondaryDisplayDate { get; init; }
    public DateTime DisplayDateStart { get; init; }
    public DateTime DisplayDateEnd { get; init; }
    public DateTime? SelectedDate { get; init; }
    public DateTime? SecondarySelectedDate { get; init; }
    public DateTime? FocusedDate { get; init; }
    public DateTime SelectedMonth { get; init; }
    public DateTime SelectedYear { get; init; }
    public DayOfWeek FirstDayOfWeek { get; init; }
    public bool IsTodayHighlighted { get; init; }
    public DateTime Today { get; init; }
    public DateTimeFormatInfo Culture { get; init; }
    public IReadOnlyList<CalendarDateRange> BlackoutDates { get; init; }
    public CalendarRangeSelectionState RangeSelection { get; init; }

    public static CalendarViewState CreateDefault(DateTime displayDate, DateTimeFormatInfo culture)
    {
        return new CalendarViewState(displayDate, culture);
    }

    public CalendarViewState WithFirstDayOfWeek(DayOfWeek firstDayOfWeek)
    {
        return this with { FirstDayOfWeek = firstDayOfWeek };
    }

    public CalendarViewState WithDisplayDate(DateTime displayDate)
    {
        var normalizedDisplayDate = DateTimeHelper.DiscardDayTime(displayDate);
        if (DateTimeHelper.CompareDays(normalizedDisplayDate, DisplayDateStart) < 0)
        {
            normalizedDisplayDate = DateTimeHelper.DiscardDayTime(DisplayDateStart);
        }
        else if (DateTimeHelper.CompareDays(normalizedDisplayDate, DisplayDateEnd) > 0)
        {
            normalizedDisplayDate = DateTimeHelper.DiscardDayTime(DisplayDateEnd);
        }

        return this with
        {
            DisplayDate          = normalizedDisplayDate,
            SecondaryDisplayDate = DateTimeHelper.AddMonths(normalizedDisplayDate, 1) ?? normalizedDisplayDate,
            SelectedMonth        = normalizedDisplayDate,
            SelectedYear         = normalizedDisplayDate
        };
    }

    public CalendarViewState WithDisplayRange(DateTime? start, DateTime? end)
    {
        var normalizedStart = DateTimeHelper.DiscardTime(start ?? DateTime.MinValue);
        var normalizedEnd   = DateTimeHelper.DiscardTime(end ?? DateTime.MaxValue);
        if (DateTime.Compare(normalizedStart, normalizedEnd) > 0)
        {
            normalizedEnd = normalizedStart;
        }

        return (this with
        {
            DisplayDateStart = normalizedStart,
            DisplayDateEnd   = normalizedEnd
        }).WithDisplayDate(DisplayDate);
    }

    public CalendarViewState WithSelectedDate(DateTime? selectedDate)
    {
        return this with { SelectedDate = selectedDate };
    }

    public CalendarViewState WithSelectedMonth(DateTime selectedMonth)
    {
        return this with { SelectedMonth = DateTimeHelper.DiscardDayTime(selectedMonth) };
    }

    public CalendarViewState WithSelectedYear(DateTime selectedYear)
    {
        return this with { SelectedYear = DateTimeHelper.DiscardDayTime(selectedYear) };
    }

    public CalendarViewState WithFocusedDate(DateTime? focusedDate)
    {
        return this with { FocusedDate = focusedDate };
    }

    public CalendarViewState WithBlackoutDates(IEnumerable<CalendarDateRange> blackoutDates)
    {
        return this with { BlackoutDates = blackoutDates.ToArray() };
    }

    public CalendarViewState WithToday(DateTime today)
    {
        return this with { Today = DateTimeHelper.DiscardTime(today) };
    }

    public CalendarViewState WithTodayHighlighted(bool isTodayHighlighted)
    {
        return this with { IsTodayHighlighted = isTodayHighlighted };
    }

    public CalendarViewState WithDisplayMode(CalendarMode mode)
    {
        return this with { DisplayMode = mode };
    }

    public CalendarViewState WithCulture(DateTimeFormatInfo culture)
    {
        return this with { Culture = culture };
    }

    public CalendarViewState WithRangeSelection(
        DateTime? start,
        DateTime? end,
        DateTime? hoverDate,
        CalendarRangeActivePart activePart,
        bool repairReverseRange)
    {
        return this with
        {
            SelectedDate          = start,
            SecondarySelectedDate = end,
            RangeSelection        = new CalendarRangeSelectionState(start, end, hoverDate, activePart, repairReverseRange)
        };
    }
}
