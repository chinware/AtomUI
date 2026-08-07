using Avalonia.Controls;
using Avalonia.VisualTree;
using AtomCalendar = AtomUI.Desktop.Controls.Calendar;
using AtomCalendarButton = AtomUI.Desktop.Controls.CalendarButton;
using AtomCalendarDayButton = AtomUI.Desktop.Controls.CalendarDayButton;
using AtomCalendarMode = AtomUI.Desktop.Controls.CalendarMode;
using AtomCalendarSelectionMode = AtomUI.Desktop.Controls.CalendarSelectionMode;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunCalendarStateVerification()
    {
        var failures = new List<string>();
        VerifyMonthModeLazyYearView(failures);
        VerifyInitialYearModeLazyMonthView(failures);
        VerifyInitialDecadeModeLazyMonthView(failures);

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

    private static void VerifyMonthModeLazyYearView(ICollection<string> failures)
    {
        var selectedDate = new DateTime(2024, 1, 20);
        var calendar     = CreateVerificationCalendar(selectedDate: selectedDate);

        using var realized = RealizeControl(calendar);
        ExpectCalendarShape(calendar, 42, 0, "Default Month Calendar", failures);
        ExpectSelectedDate(calendar, selectedDate, failures);

        calendar.DisplayMode = AtomCalendarMode.Year;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 12, "Month -> Year Calendar", failures);

        calendar.DisplayMode = AtomCalendarMode.Decade;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 12, "Year -> Decade Calendar", failures);

        calendar.DisplayMode = AtomCalendarMode.Year;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 12, "Decade -> Year Calendar", failures);

        calendar.DisplayMode = AtomCalendarMode.Month;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 12, "Year -> Month Calendar", failures);
        ExpectSelectedDate(calendar, selectedDate, failures);
    }

    private static void VerifyInitialYearModeLazyMonthView(ICollection<string> failures)
    {
        var selectedDate = new DateTime(2024, 1, 20);
        var calendar     = CreateVerificationCalendar(AtomCalendarMode.Year, selectedDate);

        using var realized = RealizeControl(calendar);
        ExpectCalendarShape(calendar, 0, 12, "Initial Year Calendar", failures);

        calendar.DisplayMode = AtomCalendarMode.Month;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 12, "Initial Year -> Month Calendar", failures);
        ExpectSelectedDate(calendar, selectedDate, failures);

        calendar.DisplayMode = AtomCalendarMode.Year;
        RefreshLayout(realized.Window);
        calendar.DisplayMode = AtomCalendarMode.Month;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 12, "Repeated Year/Month Calendar", failures);
        ExpectSelectedDate(calendar, selectedDate, failures);
    }

    private static void VerifyInitialDecadeModeLazyMonthView(ICollection<string> failures)
    {
        var selectedDate = new DateTime(2024, 1, 20);
        var calendar     = CreateVerificationCalendar(AtomCalendarMode.Decade, selectedDate);

        using var realized = RealizeControl(calendar);
        ExpectCalendarShape(calendar, 0, 12, "Initial Decade Calendar", failures);

        calendar.DisplayMode = AtomCalendarMode.Year;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 0, 12, "Initial Decade -> Year Calendar", failures);

        calendar.DisplayMode = AtomCalendarMode.Month;
        RefreshLayout(realized.Window);
        ExpectCalendarShape(calendar, 42, 12, "Initial Decade -> Month Calendar", failures);
        ExpectSelectedDate(calendar, selectedDate, failures);
    }

    private static AtomCalendar CreateVerificationCalendar(
        AtomCalendarMode displayMode = AtomCalendarMode.Month,
        DateTime? selectedDate = null)
    {
        var calendar = new AtomCalendar
        {
            DisplayDate     = new DateTime(2024, 1, 1),
            DisplayMode     = displayMode,
            SelectionMode   = AtomCalendarSelectionMode.SingleDate,
            IsMotionEnabled = false
        };

        if (selectedDate.HasValue)
        {
            calendar.SelectedDate = selectedDate.Value;
        }

        return calendar;
    }

    private static void ExpectCalendarShape(
        AtomCalendar calendar,
        int expectedDayButtons,
        int expectedCalendarButtons,
        string label,
        ICollection<string> failures)
    {
        var dayButtonCount      = CountVisuals<AtomCalendarDayButton>(calendar);
        var calendarButtonCount = CountVisuals<AtomCalendarButton>(calendar);

        Expect(dayButtonCount == expectedDayButtons,
            $"{label} should have {expectedDayButtons} CalendarDayButton visuals, actual {dayButtonCount}.",
            failures);
        Expect(calendarButtonCount == expectedCalendarButtons,
            $"{label} should have {expectedCalendarButtons} CalendarButton visuals, actual {calendarButtonCount}.",
            failures);
    }

    private static void ExpectSelectedDate(
        AtomCalendar calendar,
        DateTime selectedDate,
        ICollection<string> failures)
    {
        Expect(calendar.SelectedDate == selectedDate,
            $"Calendar SelectedDate should remain {selectedDate:yyyy-MM-dd}, actual {calendar.SelectedDate:yyyy-MM-dd}.",
            failures);
        Expect(calendar.SelectedDates.Count == 1 && calendar.SelectedDates[0] == selectedDate,
            $"Calendar SelectedDates should keep only {selectedDate:yyyy-MM-dd}, actual count {calendar.SelectedDates.Count}.",
            failures);

        var selectedButton = FindDayButton(calendar, selectedDate);
        Expect(selectedButton?.IsSelected == true,
            $"Calendar day button for {selectedDate:yyyy-MM-dd} should be selected after mode changes.",
            failures);
    }

    private static AtomCalendarDayButton? FindDayButton(AtomCalendar calendar, DateTime date)
    {
        return calendar.GetSelfAndVisualDescendants()
                       .OfType<AtomCalendarDayButton>()
                       .FirstOrDefault(button =>
                           button.DataContext is DateTime day &&
                           day.Date == date.Date);
    }

    private static int CountVisuals<T>(Control control)
        where T : Control
    {
        return control.GetSelfAndVisualDescendants().OfType<T>().Count();
    }
}
