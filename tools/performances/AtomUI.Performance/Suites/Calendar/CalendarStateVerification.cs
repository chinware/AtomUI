using Avalonia.Controls;
using Avalonia.VisualTree;
using AtomUI.Desktop.Controls.Internal.Calendar;
using AtomCalendar = AtomUI.Desktop.Controls.Calendar;
using AtomCalendarDateRange = AtomUI.Desktop.Controls.CalendarDateRange;
using AtomCalendarMode = AtomUI.Desktop.Controls.CalendarMode;
using AtomCalendarRangeBar = AtomUI.Desktop.Controls.CalendarRangeBar;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunCalendarStateVerification()
    {
        var failures = new List<string>();
        VerifyMonthAndYearModesPreserveValue(failures);
        VerifyFullscreenAndShowWeekProjection(failures);
        VerifyValidRangeAndDisabledDateProjection(failures);
        VerifyRangeBarsVisibility(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Calendar state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Calendar state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyMonthAndYearModesPreserveValue(ICollection<string> failures)
    {
        var value    = new DateTime(2024, 1, 20);
        var calendar = CreateVerificationCalendar(value: value);

        using var realized = RealizeControl(calendar);
        ExpectCalendarShape(calendar, 42, 0, 0, "Month Calendar", failures);
        ExpectSelectedDateCell(calendar, value, failures);

        calendar.Mode = AtomCalendarMode.Year;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 0, 12, 0, "Month -> Year Calendar", failures);
        ExpectSelectedMonthCell(calendar, value, failures);

        calendar.Mode = AtomCalendarMode.Month;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 0, 0, "Year -> Month Calendar", failures);
        ExpectSelectedDateCell(calendar, value, failures);
    }

    private static void VerifyFullscreenAndShowWeekProjection(ICollection<string> failures)
    {
        var calendar = CreateVerificationCalendar(
            value: new DateTime(2024, 1, 20),
            fullscreen: false,
            showWeek: true);

        using var realized = RealizeControl(calendar);
        Expect(!calendar.Fullscreen && calendar.ShowWeek,
            "Compact calendar should retain the configured Fullscreen and ShowWeek public values.",
            failures);
        ExpectCalendarShape(calendar, 42, 0, 6, "Compact Calendar With Week Numbers", failures);

        calendar.Fullscreen = true;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 0, 6, "Fullscreen Calendar With Week Numbers", failures);

        calendar.ShowWeek = false;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 0, 0, "Fullscreen Calendar Without Week Numbers", failures);
    }

    private static void VerifyValidRangeAndDisabledDateProjection(ICollection<string> failures)
    {
        var validRange = new AtomCalendarDateRange(new DateTime(2024, 1, 10), new DateTime(2024, 1, 20));
        var disabledDate = new Func<DateTime, bool>(date => date.Date == new DateTime(2024, 1, 16));
        var calendar = CreateVerificationCalendar(
            value: new DateTime(2024, 1, 15),
            validRange: validRange,
            disabledDate: disabledDate);

        using var realized = RealizeControl(calendar);
        Expect(ReferenceEquals(calendar.ValidRange, validRange),
            "Calendar should retain the configured ValidRange instance.",
            failures);
        Expect(ReferenceEquals(calendar.DisabledDate, disabledDate),
            "Calendar should retain the configured DisabledDate delegate.",
            failures);
        ExpectDateCellDisabled(calendar, new DateTime(2024, 1, 9), true, failures);
        ExpectDateCellDisabled(calendar, new DateTime(2024, 1, 10), false, failures);
        ExpectDateCellDisabled(calendar, new DateTime(2024, 1, 16), true, failures);
        ExpectDateCellDisabled(calendar, new DateTime(2024, 1, 21), true, failures);
    }

    private static void VerifyRangeBarsVisibility(ICollection<string> failures)
    {
        var calendar = CreateVerificationCalendar(value: new DateTime(2024, 1, 15));
        calendar.RangeBars.Add(new AtomCalendarRangeBar
        {
            StartDate = new DateTime(2024, 1, 12),
            EndDate   = new DateTime(2024, 1, 16),
            Label     = "Release"
        });

        using var realized = RealizeControl(calendar);
        var rangeBarPanel = calendar.GetSelfAndVisualDescendants()
            .OfType<CalendarRangeBarPanel>()
            .FirstOrDefault();
        Expect(rangeBarPanel is not null,
            "Calendar should realize its range-bar panel.",
            failures);
        if (rangeBarPanel is null)
        {
            return;
        }

        Expect(calendar.RangeBars.Count == 1 && rangeBarPanel.IsVisible,
            "Fullscreen Month Calendar with range bars should show the range-bar panel.",
            failures);

        calendar.Fullscreen = false;
        RefreshLayout(realized.Window);
        Expect(!rangeBarPanel.IsVisible,
            "Compact Calendar should hide the range-bar panel.",
            failures);

        calendar.Fullscreen = true;
        calendar.Mode = AtomCalendarMode.Year;
        RefreshLayout(realized.Window);
        Expect(!rangeBarPanel.IsVisible,
            "Year Calendar should hide the range-bar panel.",
            failures);

        calendar.Mode = AtomCalendarMode.Month;
        RefreshLayout(realized.Window);
        Expect(rangeBarPanel.IsVisible,
            "Returning to fullscreen Month mode should restore range-bar visibility.",
            failures);
    }

    private static AtomCalendar CreateVerificationCalendar(
        AtomCalendarMode mode = AtomCalendarMode.Month,
        DateTime? value = null,
        bool fullscreen = true,
        bool showWeek = false,
        AtomCalendarDateRange? validRange = null,
        Func<DateTime, bool>? disabledDate = null)
    {
        return new AtomCalendar
        {
            Value        = value ?? new DateTime(2024, 1, 1),
            Mode         = mode,
            Fullscreen   = fullscreen,
            ShowWeek     = showWeek,
            ValidRange   = validRange,
            DisabledDate = disabledDate
        };
    }

    private static void ExpectCalendarShape(
        AtomCalendar calendar,
        int expectedDateCells,
        int expectedMonthCells,
        int expectedWeekCells,
        string label,
        ICollection<string> failures)
    {
        var cells = calendar.GetSelfAndVisualDescendants().OfType<CalendarViewCell>().ToList();
        var dateCellCount = cells.Count(cell => cell.Model?.Kind == CalendarViewCellKind.Date);
        var monthCellCount = cells.Count(cell => cell.Model?.Kind == CalendarViewCellKind.Month);
        var weekCellCount = cells.Count(cell => cell.Model?.Kind == CalendarViewCellKind.Week);

        Expect(dateCellCount == expectedDateCells,
            $"{label} should have {expectedDateCells} date cells, actual {dateCellCount}.",
            failures);
        Expect(monthCellCount == expectedMonthCells,
            $"{label} should have {expectedMonthCells} month cells, actual {monthCellCount}.",
            failures);
        Expect(weekCellCount == expectedWeekCells,
            $"{label} should have {expectedWeekCells} week-number cells, actual {weekCellCount}.",
            failures);
    }

    private static void ExpectSelectedDateCell(
        AtomCalendar calendar,
        DateTime value,
        ICollection<string> failures)
    {
        Expect(calendar.Value == value,
            $"Calendar Value should remain {value:yyyy-MM-dd}, actual {calendar.Value:yyyy-MM-dd}.",
            failures);
        var selectedCell = FindCell(calendar, value, CalendarViewCellKind.Date);
        Expect(selectedCell?.Model?.IsSelected == true,
            $"Calendar date cell for {value:yyyy-MM-dd} should remain selected after mode changes.",
            failures);
    }

    private static void ExpectSelectedMonthCell(
        AtomCalendar calendar,
        DateTime value,
        ICollection<string> failures)
    {
        var selectedCell = FindCell(calendar, value, CalendarViewCellKind.Month);
        Expect(selectedCell?.Model?.IsSelected == true,
            $"Calendar month cell for {value:yyyy-MM} should reflect the current Value in Year mode.",
            failures);
    }

    private static void ExpectDateCellDisabled(
        AtomCalendar calendar,
        DateTime date,
        bool expectedDisabled,
        ICollection<string> failures)
    {
        var cell = FindCell(calendar, date, CalendarViewCellKind.Date);
        Expect(cell is not null,
            $"Calendar should realize a date cell for {date:yyyy-MM-dd}.",
            failures);
        Expect(cell?.Model?.IsDisabled == expectedDisabled,
            $"Calendar date cell for {date:yyyy-MM-dd} should have IsDisabled={expectedDisabled}.",
            failures);
    }

    private static CalendarViewCell? FindCell(
        AtomCalendar calendar,
        DateTime date,
        CalendarViewCellKind kind)
    {
        return calendar.GetSelfAndVisualDescendants()
            .OfType<CalendarViewCell>()
            .FirstOrDefault(cell =>
                cell.Model is { Kind: var modelKind, Value: var value } &&
                modelKind == kind &&
                (kind == CalendarViewCellKind.Month
                    ? value.Year == date.Year && value.Month == date.Month
                    : value.Date == date.Date));
    }
}
