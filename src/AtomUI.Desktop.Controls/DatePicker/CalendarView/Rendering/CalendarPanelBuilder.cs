using System.Globalization;
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
        var weekCells  = state.PickerMode == DatePickerMode.Week
            ? BuildWeekCells(state, monthStart, firstCell)
            : Array.Empty<CalendarCellState>();

        for (var i = 0; i < 42; i++)
        {
            var date = firstCell.AddDays(i);
            cells.Add(BuildDayCell(state, monthStart, date));
        }

        return new CalendarMonthPanelModel(monthStart, dayTitles, cells, weekCells);
    }

    public static CalendarYearPanelModel BuildYearPanel(CalendarViewState state, DateTime displayYear)
    {
        var yearStart = new DateTime(displayYear.Year, 1, 1);
        var months    = new List<CalendarCellState>(12);
        var hasSelectedMonth = state.SelectedDate is not null || state.SecondarySelectedDate is not null;
        var hasPreviewRange   = HasPickerUnitPreviewRange(state, DatePickerMode.Month);
        for (var month = 1; month <= 12; month++)
        {
            var date       = new DateTime(yearStart.Year, month, 1);
            var isDisabled = DateTimeHelper.CompareYearMonth(date, state.DisplayDateStart) < 0 ||
                             DateTimeHelper.CompareYearMonth(date, state.DisplayDateEnd) > 0;
            var isSelected = !hasPreviewRange &&
                             (IsSelectedPickerUnit(state, date, DatePickerMode.Month) ||
                              (!hasSelectedMonth && IsSamePickerUnit(date, state.DisplayDate, DatePickerMode.Month)));
            var rangeState = GetPickerUnitRangeState(state, date, DatePickerMode.Month);
            months.Add(new CalendarCellState(
                Date: date,
                Text: state.Culture.AbbreviatedMonthNames[month - 1],
                IsToday: false,
                IsBlackout: false,
                IsDisabled: isDisabled,
                IsInactive: false,
                IsSelected: isSelected,
                IsRangeStart: rangeState.IsRangeStart,
                IsRangeEnd: rangeState.IsRangeEnd,
                IsRangeMiddle: rangeState.IsRangeMiddle,
                IsFocused: DateTimeHelper.CompareYearMonth(date, state.SelectedMonth) == 0,
                IsHidden: isDisabled,
                IsRangePreviewStart: rangeState.IsRangePreviewStart,
                IsRangePreviewEnd: rangeState.IsRangePreviewEnd,
                IsRangePreviewMiddle: rangeState.IsRangePreviewMiddle));
        }

        return new CalendarYearPanelModel(yearStart, months);
    }

    public static CalendarQuarterPanelModel BuildQuarterPanel(CalendarViewState state, DateTime displayYear)
    {
        var yearStart         = new DateTime(displayYear.Year, 1, 1);
        var hasSelectedQuarter = state.SelectedDate is not null || state.SecondarySelectedDate is not null;
        var hasPreviewRange    = HasPickerUnitPreviewRange(state, DatePickerMode.Quarter);
        var quarters          = new List<CalendarCellState>(4);
        for (var quarter = 1; quarter <= 4; quarter++)
        {
            var startMonth = ((quarter - 1) * 3) + 1;
            var date       = new DateTime(yearStart.Year, startMonth, 1);
            var endMonth   = new DateTime(yearStart.Year, startMonth + 2, 1);
            var isDisabled = DateTimeHelper.CompareYearMonth(endMonth, state.DisplayDateStart) < 0 ||
                             DateTimeHelper.CompareYearMonth(date, state.DisplayDateEnd) > 0;
            var isSelected = !hasPreviewRange &&
                             (IsSelectedPickerUnit(state, date, DatePickerMode.Quarter) ||
                              (!hasSelectedQuarter && IsSamePickerUnit(date, state.DisplayDate, DatePickerMode.Quarter)));
            var rangeState = GetPickerUnitRangeState(state, date, DatePickerMode.Quarter);
            quarters.Add(new CalendarCellState(
                Date: date,
                Text: $"Q{quarter}",
                IsToday: false,
                IsBlackout: false,
                IsDisabled: isDisabled,
                IsInactive: false,
                IsSelected: isSelected,
                IsRangeStart: rangeState.IsRangeStart,
                IsRangeEnd: rangeState.IsRangeEnd,
                IsRangeMiddle: rangeState.IsRangeMiddle,
                IsFocused: IsSamePickerUnit(date, state.SelectedMonth, DatePickerMode.Quarter),
                IsHidden: isDisabled,
                IsRangePreviewStart: rangeState.IsRangePreviewStart,
                IsRangePreviewEnd: rangeState.IsRangePreviewEnd,
                IsRangePreviewMiddle: rangeState.IsRangePreviewMiddle));
        }

        return new CalendarQuarterPanelModel(yearStart, quarters);
    }

    public static CalendarDecadePanelModel BuildDecadePanel(CalendarViewState state, DateTime selectedYear)
    {
        var decadeStart    = DateTimeHelper.DecadeOfDate(selectedYear);
        var decadeEnd      = DateTimeHelper.EndOfDecade(selectedYear);
        var years          = new List<CalendarCellState>(12);
        var hasSelectedYear = state.SelectedDate is not null || state.SecondarySelectedDate is not null;
        var hasPreviewRange = HasPickerUnitPreviewRange(state, DatePickerMode.Year);
        for (var offset = -1; offset <= 10; offset++)
        {
            var year    = decadeStart + offset;
            var isValid = year >= DateTime.MinValue.Year && year <= DateTime.MaxValue.Year;
            var date    = isValid ? new DateTime(year, 1, 1) : DateTime.MinValue;
            var isDisabled = !isValid ||
                             year < state.DisplayDateStart.Year ||
                             year > state.DisplayDateEnd.Year;
            var isSelected = isValid &&
                             !hasPreviewRange &&
                             (IsSelectedPickerUnit(state, date, DatePickerMode.Year) ||
                              (!hasSelectedYear && state.DisplayDate.Year == year));
            var rangeState = isValid
                ? GetPickerUnitRangeState(state, date, DatePickerMode.Year)
                : default;
            years.Add(new CalendarCellState(
                Date: date,
                Text: isValid ? year.ToString(state.Culture) : string.Empty,
                IsToday: false,
                IsBlackout: false,
                IsDisabled: isDisabled,
                IsInactive: year < decadeStart || year > decadeEnd,
                IsSelected: isSelected,
                IsRangeStart: rangeState.IsRangeStart,
                IsRangeEnd: rangeState.IsRangeEnd,
                IsRangeMiddle: rangeState.IsRangeMiddle,
                IsFocused: isValid && state.SelectedYear.Year == year,
                IsHidden: isDisabled,
                IsRangePreviewStart: rangeState.IsRangePreviewStart,
                IsRangePreviewEnd: rangeState.IsRangePreviewEnd,
                IsRangePreviewMiddle: rangeState.IsRangePreviewMiddle));
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

    private static IReadOnlyList<CalendarCellState> BuildWeekCells(
        CalendarViewState state,
        DateTime displayMonth,
        DateTime firstCell)
    {
        var weekCells = new List<CalendarCellState>(Calendar.RowsPerMonth - 1);
        var hasPreviewRange = state.RangeSelection.TryGetPreviewRange(out _, out _);
        for (var row = 0; row < Calendar.RowsPerMonth - 1; row++)
        {
            var weekStart = firstCell.AddDays(row * Calendar.ColumnsPerMonth);
            var weekDays  = Enumerable.Range(0, Calendar.ColumnsPerMonth)
                                      .Select(offset => weekStart.AddDays(offset))
                                      .ToArray();
            var isOutOfRange = weekDays.All(date =>
                DateTimeHelper.CompareDays(date, state.DisplayDateStart) < 0 ||
                DateTimeHelper.CompareDays(date, state.DisplayDateEnd) > 0);
            var isInactive = weekDays.All(date => DateTimeHelper.CompareYearMonth(date, displayMonth) != 0);
            var isSelected = !hasPreviewRange &&
                             IsSelectedPickerUnit(state, weekStart, DatePickerMode.Week);
            var isWeekSelection = isSelected || IsWeekRangePreviewEndpoint(state, weekStart);
            var isWeekRange = IsWeekInsideVisualRange(state, weekStart);
            var isHovered = !isWeekSelection &&
                            state.HoverDate is not null &&
                            IsSamePickerUnit(state.HoverDate.Value, weekStart, DatePickerMode.Week);

            weekCells.Add(new CalendarCellState(
                Date: weekStart,
                Text: ISOWeek.GetWeekOfYear(weekStart).ToString(state.Culture),
                IsToday: false,
                IsBlackout: false,
                IsDisabled: isOutOfRange,
                IsInactive: isInactive,
                IsSelected: isSelected,
                IsRangeStart: false,
                IsRangeEnd: false,
                IsRangeMiddle: false,
                IsFocused: state.FocusedDate is not null &&
                           IsSamePickerUnit(state.FocusedDate.Value, weekStart, DatePickerMode.Week),
                IsHidden: isOutOfRange,
                IsWeekNumber: true,
                IsWeekSelectionStart: isWeekSelection,
                IsWeekRangeStart: isWeekRange,
                IsWeekHoverStart: isHovered));
        }

        return weekCells;
    }

    private static CalendarCellState BuildDayCell(CalendarViewState state, DateTime displayMonth, DateTime date)
    {
        var isOutOfRange = DateTimeHelper.CompareDays(date, state.DisplayDateStart) < 0 ||
                           DateTimeHelper.CompareDays(date, state.DisplayDateEnd) > 0;
        var isToday = state.IsTodayHighlighted &&
                      DateTimeHelper.CompareDays(state.Today, date) == 0;
        var isBlackout = state.BlackoutDates.Any(range => DateTimeHelper.InRange(date, range));

        var shouldRenderDateRange = state.PickerMode != DatePickerMode.Week;
        var rangeStart            = default(DateTime);
        var rangeEnd              = default(DateTime);
        var previewRangeStart     = default(DateTime);
        var previewRangeEnd       = default(DateTime);
        var hasCommittedRange = shouldRenderDateRange &&
                                state.RangeSelection.TryGetCommittedRange(out rangeStart, out rangeEnd);
        var hasPreviewRange = shouldRenderDateRange &&
                              state.RangeSelection.TryGetPreviewRange(out previewRangeStart, out previewRangeEnd);
        var isRangeStart = !hasPreviewRange &&
                           hasCommittedRange &&
                           DateTimeHelper.CompareDays(rangeStart, date) == 0;
        var isRangeEnd = !hasPreviewRange &&
                         hasCommittedRange &&
                         DateTimeHelper.CompareDays(rangeEnd, date) == 0;
        var isRangeMiddle = !hasPreviewRange &&
                            hasCommittedRange &&
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
        var isSelected = !hasPreviewRange &&
                         IsSelectedPickerUnit(state, date, state.PickerMode);
        var weekStart = GetWeekStart(date, state.FirstDayOfWeek);
        var isWeekSelection = state.PickerMode == DatePickerMode.Week &&
                              (isSelected || IsWeekRangePreviewEndpoint(state, weekStart));
        var isWeekSelectionEnd = isWeekSelection &&
                                 DateTimeHelper.CompareDays(date, weekStart.AddDays(6)) == 0;
        var isWeekRange = state.PickerMode == DatePickerMode.Week &&
                          IsWeekInsideVisualRange(state, weekStart);
        var isWeekRangeEnd = isWeekRange &&
                             DateTimeHelper.CompareDays(date, weekStart.AddDays(6)) == 0;
        var isWeekHover = state.PickerMode == DatePickerMode.Week &&
                          !isWeekSelection &&
                          state.HoverDate is not null &&
                          IsSamePickerUnit(state.HoverDate.Value, date, DatePickerMode.Week);
        var isWeekHoverEnd = isWeekHover &&
                             DateTimeHelper.CompareDays(date, weekStart.AddDays(6)) == 0;

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
            IsRangePreviewMiddle: isRangePreviewMiddle,
            IsWeekSelectionMiddle: isWeekSelection && !isWeekSelectionEnd,
            IsWeekSelectionEnd: isWeekSelectionEnd,
            IsWeekRangeMiddle: isWeekRange && !isWeekRangeEnd,
            IsWeekRangeEnd: isWeekRangeEnd,
            IsWeekHoverMiddle: isWeekHover && !isWeekHoverEnd,
            IsWeekHoverEnd: isWeekHoverEnd);
    }

    private static bool IsSamePickerUnit(DateTime first, DateTime second, DatePickerMode pickerMode)
    {
        return DatePickerFormattingHelper.IsSamePickerUnit(first, second, pickerMode);
    }

    private static bool IsSelectedPickerUnit(CalendarViewState state, DateTime date, DatePickerMode pickerMode)
    {
        return (state.SelectedDate is not null &&
                IsSamePickerUnit(state.SelectedDate.Value, date, pickerMode)) ||
               (state.SecondarySelectedDate is not null &&
                IsSamePickerUnit(state.SecondarySelectedDate.Value, date, pickerMode));
    }

    private static bool HasPickerUnitPreviewRange(CalendarViewState state, DatePickerMode pickerMode)
    {
        return state.PickerMode == pickerMode &&
               state.RangeSelection.TryGetPreviewRange(out _, out _);
    }

    private static (
        bool IsRangeStart,
        bool IsRangeEnd,
        bool IsRangeMiddle,
        bool IsRangePreviewStart,
        bool IsRangePreviewEnd,
        bool IsRangePreviewMiddle) GetPickerUnitRangeState(
            CalendarViewState state,
            DateTime date,
            DatePickerMode pickerMode)
    {
        if (state.PickerMode != pickerMode)
        {
            return default;
        }

        var rangeStart = default(DateTime);
        var rangeEnd   = default(DateTime);
        var previewRangeStart = default(DateTime);
        var previewRangeEnd   = default(DateTime);
        var hasCommittedRange = state.RangeSelection.TryGetCommittedRange(out rangeStart, out rangeEnd);
        var hasPreviewRange   = state.RangeSelection.TryGetPreviewRange(out previewRangeStart, out previewRangeEnd);

        if (hasPreviewRange)
        {
            return (
                IsRangeStart: false,
                IsRangeEnd: false,
                IsRangeMiddle: false,
                IsRangePreviewStart: IsSamePickerUnit(previewRangeStart, date, pickerMode),
                IsRangePreviewEnd: IsSamePickerUnit(previewRangeEnd, date, pickerMode),
                IsRangePreviewMiddle: IsPickerUnitInsideRange(state, date, previewRangeStart, previewRangeEnd, pickerMode));
        }

        return (
            IsRangeStart: hasCommittedRange && IsSamePickerUnit(rangeStart, date, pickerMode),
            IsRangeEnd: hasCommittedRange && IsSamePickerUnit(rangeEnd, date, pickerMode),
            IsRangeMiddle: hasCommittedRange && IsPickerUnitInsideRange(state, date, rangeStart, rangeEnd, pickerMode),
            IsRangePreviewStart: false,
            IsRangePreviewEnd: false,
            IsRangePreviewMiddle: false);
    }

    private static bool IsPickerUnitInsideRange(
        CalendarViewState state,
        DateTime date,
        DateTime rangeStart,
        DateTime rangeEnd,
        DatePickerMode pickerMode)
    {
        var unitDate  = NormalizePickerUnit(state, date, pickerMode);
        var unitStart = NormalizePickerUnit(state, rangeStart, pickerMode);
        var unitEnd   = NormalizePickerUnit(state, rangeEnd, pickerMode);

        return DateTimeHelper.CompareDays(unitDate, unitStart) > 0 &&
               DateTimeHelper.CompareDays(unitDate, unitEnd) < 0;
    }

    private static DateTime NormalizePickerUnit(CalendarViewState state, DateTime date, DatePickerMode pickerMode)
    {
        return DatePickerFormattingHelper.NormalizeDateTime(date, pickerMode, state.FirstDayOfWeek);
    }

    private static bool IsWeekInsideVisualRange(CalendarViewState state, DateTime weekStart)
    {
        if (!TryGetWeekVisualRange(state, out var rangeStart, out var rangeEnd))
        {
            return false;
        }

        var normalizedWeekStart  = GetWeekStart(weekStart, state.FirstDayOfWeek);
        var normalizedRangeStart = GetWeekStart(rangeStart, state.FirstDayOfWeek);
        var normalizedRangeEnd   = GetWeekStart(rangeEnd, state.FirstDayOfWeek);

        return DateTimeHelper.CompareDays(normalizedWeekStart, normalizedRangeStart) > 0 &&
               DateTimeHelper.CompareDays(normalizedWeekStart, normalizedRangeEnd) < 0;
    }

    private static bool IsWeekRangePreviewEndpoint(CalendarViewState state, DateTime weekStart)
    {
        if (state.PickerMode != DatePickerMode.Week ||
            !state.RangeSelection.TryGetPreviewRange(out var rangeStart, out var rangeEnd))
        {
            return false;
        }

        var normalizedWeekStart  = GetWeekStart(weekStart, state.FirstDayOfWeek);
        var normalizedRangeStart = GetWeekStart(rangeStart, state.FirstDayOfWeek);
        var normalizedRangeEnd   = GetWeekStart(rangeEnd, state.FirstDayOfWeek);

        return DateTimeHelper.CompareDays(normalizedWeekStart, normalizedRangeStart) == 0 ||
               DateTimeHelper.CompareDays(normalizedWeekStart, normalizedRangeEnd) == 0;
    }

    private static bool TryGetWeekVisualRange(CalendarViewState state, out DateTime rangeStart, out DateTime rangeEnd)
    {
        if (state.PickerMode != DatePickerMode.Week)
        {
            rangeStart = default;
            rangeEnd   = default;
            return false;
        }

        return state.RangeSelection.TryGetPreviewRange(out rangeStart, out rangeEnd) ||
               state.RangeSelection.TryGetCommittedRange(out rangeStart, out rangeEnd);
    }

    private static DateTime GetWeekStart(DateTime date, DayOfWeek firstDayOfWeek)
    {
        var offset = ((int)date.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        return DateTimeHelper.DiscardTime(date).AddDays(-offset);
    }
}
