using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.CalendarView.Models;
using AtomUI.Desktop.Controls.CalendarView.State;

namespace AtomUI.Desktop.Controls.CalendarView.Rendering;

internal static class CalendarPanelBuilder
{
    public static CalendarMonthPanelModel BuildMonthPanel(CalendarViewState state, DateTime displayMonth)
    {
        var monthStart = DateTimeHelper.DiscardDayTime(displayMonth);
        var dayTitles  = BuildDayTitles(state);
        var firstCell  = GetFirstVisibleDate(monthStart, state.FirstDayOfWeek);
        var cells      = new List<CalendarCellState>(42);

        for (var i = 0; i < 42; i++)
        {
            var date = firstCell.AddDays(i);
            cells.Add(BuildDayCell(state, monthStart, date));
        }

        return new CalendarMonthPanelModel(monthStart, dayTitles, cells);
    }

    public static CalendarYearPanelModel BuildYearPanel(CalendarViewState state, DateTime displayYear)
    {
        var yearStart = new DateTime(displayYear.Year, 1, 1);
        var months    = new List<CalendarCellState>(12);
        var selectedReference = state.SelectedDate ?? state.DisplayDate;
        for (var month = 1; month <= 12; month++)
        {
            var date       = new DateTime(yearStart.Year, month, 1);
            var isDisabled = DateTimeHelper.CompareYearMonth(date, state.DisplayDateStart) < 0 ||
                             DateTimeHelper.CompareYearMonth(date, state.DisplayDateEnd) > 0;
            months.Add(new CalendarCellState(
                Date: date,
                Text: state.Culture.AbbreviatedMonthNames[month - 1],
                IsToday: false,
                IsBlackout: false,
                IsDisabled: isDisabled,
                IsInactive: false,
                IsSelected: IsSamePickerUnit(date, selectedReference, DatePickerMode.Month),
                IsRangeStart: false,
                IsRangeEnd: false,
                IsRangeMiddle: false,
                IsFocused: DateTimeHelper.CompareYearMonth(date, state.SelectedMonth) == 0,
                IsHidden: isDisabled));
        }

        return new CalendarYearPanelModel(yearStart, months);
    }

    public static CalendarQuarterPanelModel BuildQuarterPanel(CalendarViewState state, DateTime displayYear)
    {
        var yearStart         = new DateTime(displayYear.Year, 1, 1);
        var selectedReference = state.SelectedDate ?? state.DisplayDate;
        var quarters          = new List<CalendarCellState>(4);
        for (var quarter = 1; quarter <= 4; quarter++)
        {
            var startMonth = ((quarter - 1) * 3) + 1;
            var date       = new DateTime(yearStart.Year, startMonth, 1);
            var endMonth   = new DateTime(yearStart.Year, startMonth + 2, 1);
            var isDisabled = DateTimeHelper.CompareYearMonth(endMonth, state.DisplayDateStart) < 0 ||
                             DateTimeHelper.CompareYearMonth(date, state.DisplayDateEnd) > 0;
            quarters.Add(new CalendarCellState(
                Date: date,
                Text: $"Q{quarter}",
                IsToday: false,
                IsBlackout: false,
                IsDisabled: isDisabled,
                IsInactive: false,
                IsSelected: IsSamePickerUnit(date, selectedReference, DatePickerMode.Quarter),
                IsRangeStart: false,
                IsRangeEnd: false,
                IsRangeMiddle: false,
                IsFocused: IsSamePickerUnit(date, state.SelectedMonth, DatePickerMode.Quarter),
                IsHidden: isDisabled));
        }

        return new CalendarQuarterPanelModel(yearStart, quarters);
    }

    public static CalendarDecadePanelModel BuildDecadePanel(CalendarViewState state, DateTime selectedYear)
    {
        var decadeStart = DateTimeHelper.DecadeOfDate(selectedYear);
        var decadeEnd   = DateTimeHelper.EndOfDecade(selectedYear);
        var years       = new List<CalendarCellState>(12);
        for (var offset = -1; offset <= 10; offset++)
        {
            var year    = decadeStart + offset;
            var isValid = year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year;
            var date    = isValid ? new DateTime(year, 1, 1) : DateTime.MinValue;
            var isDisabled = !isValid ||
                             year < state.DisplayDateStart.Year ||
                             year > state.DisplayDateEnd.Year;
            years.Add(new CalendarCellState(
                Date: date,
                Text: isValid ? year.ToString(state.Culture) : string.Empty,
                IsToday: false,
                IsBlackout: false,
                IsDisabled: isDisabled,
                IsInactive: year < decadeStart || year > decadeEnd,
                IsSelected: isValid && state.DisplayDate.Year == year,
                IsRangeStart: false,
                IsRangeEnd: false,
                IsRangeMiddle: false,
                IsFocused: isValid && state.SelectedYear.Year == year,
                IsHidden: isDisabled));
        }

        return new CalendarDecadePanelModel(decadeStart, years);
    }

    private static IReadOnlyList<string> BuildDayTitles(CalendarViewState state)
    {
        var titles = new List<string>(7);
        for (var i = 0; i < 7; i++)
        {
            var dayIndex = ((int)state.FirstDayOfWeek + i) % 7;
            titles.Add(state.Culture.ShortestDayNames[dayIndex]);
        }

        return titles;
    }

    private static DateTime GetFirstVisibleDate(DateTime monthStart, DayOfWeek firstDayOfWeek)
    {
        var offset = ((int)monthStart.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        return monthStart.AddDays(-offset);
    }

    private static CalendarCellState BuildDayCell(CalendarViewState state, DateTime displayMonth, DateTime date)
    {
        var isOutOfRange = DateTimeHelper.CompareDays(date, state.DisplayDateStart) < 0 ||
                           DateTimeHelper.CompareDays(date, state.DisplayDateEnd) > 0;
        var isToday = state.IsTodayHighlighted &&
                      DateTimeHelper.CompareDays(state.Today, date) == 0;
        var isBlackout = state.BlackoutDates.Any(range => DateTimeHelper.InRange(date, range));

        var hasCommittedRange = state.RangeSelection.TryGetCommittedRange(out var rangeStart, out var rangeEnd);
        var hasPreviewRange   = state.RangeSelection.TryGetPreviewRange(out var previewRangeStart, out var previewRangeEnd);
        var isRangeStart = hasCommittedRange &&
                           DateTimeHelper.CompareDays(rangeStart, date) == 0;
        var isRangeEnd = hasCommittedRange &&
                         DateTimeHelper.CompareDays(rangeEnd, date) == 0;
        var isRangeMiddle = hasCommittedRange &&
                            DateTimeHelper.CompareDays(date, rangeStart) > 0 &&
                            DateTimeHelper.CompareDays(date, rangeEnd) < 0;
        var isRangePreviewStart = hasPreviewRange &&
                                  DateTimeHelper.CompareDays(previewRangeStart, date) == 0;
        var isRangePreviewEnd = hasPreviewRange &&
                                DateTimeHelper.CompareDays(previewRangeEnd, date) == 0;
        var isRangePreviewMiddle = hasPreviewRange &&
                                   DateTimeHelper.CompareDays(date, previewRangeStart) > 0 &&
                                   DateTimeHelper.CompareDays(date, previewRangeEnd) < 0;
        var isFocused = state.FocusedDate is not null &&
                        IsSamePickerUnit(state.FocusedDate.Value, date, state.PickerMode);
        var isSelected = (state.SelectedDate is not null &&
                          IsSamePickerUnit(state.SelectedDate.Value, date, state.PickerMode)) ||
                         (state.SecondarySelectedDate is not null &&
                          IsSamePickerUnit(state.SecondarySelectedDate.Value, date, state.PickerMode));

        return new CalendarCellState(
            Date: date,
            Text: date.Day.ToString(state.Culture),
            IsToday: isToday,
            IsBlackout: isBlackout,
            IsDisabled: isOutOfRange,
            IsInactive: DateTimeHelper.CompareYearMonth(date, displayMonth) != 0,
            IsSelected: isSelected,
            IsRangeStart: isRangeStart,
            IsRangeEnd: isRangeEnd,
            IsRangeMiddle: isRangeMiddle,
            IsFocused: isFocused,
            IsHidden: isOutOfRange,
            IsRangePreviewStart: isRangePreviewStart,
            IsRangePreviewEnd: isRangePreviewEnd,
            IsRangePreviewMiddle: isRangePreviewMiddle);
    }

    private static bool IsSamePickerUnit(DateTime first, DateTime second, DatePickerMode pickerMode)
    {
        return DatePickerFormattingHelper.IsSamePickerUnit(first, second, pickerMode);
    }
}
