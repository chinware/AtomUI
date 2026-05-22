using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using AtomCalendar = AtomUI.Desktop.Controls.Calendar;
using AtomCalendarDateRange = AtomUI.Desktop.Controls.CalendarDateRange;
using AtomCalendarMode = AtomUI.Desktop.Controls.CalendarMode;
using AtomCalendarSelectionMode = AtomUI.Desktop.Controls.CalendarSelectionMode;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateCalendarScenarios()
    {
        return
        [
            new PerfScenario("Calendar.Default", _ => CreateCalendar()),
            new PerfScenario("Calendar.SingleDate.Selected", _ => CreateCalendar(
                selectionMode: AtomCalendarSelectionMode.SingleDate,
                selectedDate: new DateTime(2024, 1, 20))),
            new PerfScenario("Calendar.SingleRange.Selected", _ => CreateCalendar(
                selectionMode: AtomCalendarSelectionMode.SingleRange,
                rangeStart: new DateTime(2024, 1, 12),
                rangeEnd: new DateTime(2024, 1, 20))),
            new PerfScenario("Calendar.MultipleRange.Blackout", _ => CreateCalendarWithBlackout()),
            new PerfScenario("Calendar.YearMode", _ => CreateCalendar(displayMode: AtomCalendarMode.Year)),
            new PerfScenario("Calendar.DecadeMode", _ => CreateCalendar(displayMode: AtomCalendarMode.Decade)),
            new PerfScenario("Calendar.RangeRestricted", _ => CreateCalendar(
                displayDateStart: new DateTime(2024, 1, 10),
                displayDateEnd: new DateTime(2024, 3, 20))),
            new PerfScenario("Calendar.MotionDisabled", _ => CreateCalendar(isMotionEnabled: false)),
            new PerfScenario("Calendar.Batch4", _ => CreateCalendarBatch())
        ];
    }

    private static AtomCalendar CreateCalendar(
        AtomCalendarMode displayMode = AtomCalendarMode.Month,
        AtomCalendarSelectionMode selectionMode = AtomCalendarSelectionMode.SingleRange,
        DateTime? selectedDate = null,
        DateTime? rangeStart = null,
        DateTime? rangeEnd = null,
        DateTime? displayDateStart = null,
        DateTime? displayDateEnd = null,
        bool isMotionEnabled = true)
    {
        var calendar = new AtomCalendar
        {
            DisplayDate      = new DateTime(2024, 1, 1),
            DisplayDateStart = displayDateStart,
            DisplayDateEnd   = displayDateEnd,
            DisplayMode      = displayMode,
            SelectionMode    = selectionMode,
            IsMotionEnabled  = isMotionEnabled
        };

        if (selectedDate.HasValue)
        {
            calendar.SelectedDate = selectedDate.Value;
        }

        if (rangeStart.HasValue && rangeEnd.HasValue)
        {
            calendar.SelectedDates.AddRange(rangeStart.Value, rangeEnd.Value);
        }

        return calendar;
    }

    private static AtomCalendar CreateCalendarWithBlackout()
    {
        var calendar = CreateCalendar(selectionMode: AtomCalendarSelectionMode.MultipleRange);
        calendar.BlackoutDates.Add(new AtomCalendarDateRange(new DateTime(2024, 1, 5), new DateTime(2024, 1, 7)));
        calendar.BlackoutDates.Add(new AtomCalendarDateRange(new DateTime(2024, 1, 21), new DateTime(2024, 1, 25)));
        calendar.SelectedDates.AddRange(new DateTime(2024, 1, 12), new DateTime(2024, 1, 16));
        return calendar;
    }

    private static Control CreateCalendarBatch()
    {
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateCalendar(),
                CreateCalendar(
                    selectionMode: AtomCalendarSelectionMode.SingleDate,
                    selectedDate: new DateTime(2024, 1, 20)),
                CreateCalendar(displayMode: AtomCalendarMode.Year),
                CreateCalendar(displayMode: AtomCalendarMode.Decade, isMotionEnabled: false)
            }
        };
    }
}
