using Avalonia.Controls;
using AtomCalendar = AtomUI.Desktop.Controls.Calendar;
using AtomCalendarDateRange = AtomUI.Desktop.Controls.CalendarDateRange;
using AtomCalendarMode = AtomUI.Desktop.Controls.CalendarMode;
using AtomCalendarRangeBar = AtomUI.Desktop.Controls.CalendarRangeBar;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateCalendarScenarios()
    {
        return
        [
            new PerfScenario("Calendar.Default", _ => CreateCalendar()),
            new PerfScenario("Calendar.Value.Selected", _ => CreateCalendar(value: new DateTime(2024, 1, 20))),
            new PerfScenario("Calendar.RangeBars", _ => CreateCalendarWithRangeBars()),
            new PerfScenario("Calendar.YearMode", _ => CreateCalendar(mode: AtomCalendarMode.Year)),
            new PerfScenario("Calendar.ValidRange", _ => CreateCalendar(
                validRange: new AtomCalendarDateRange(new DateTime(2024, 1, 10), new DateTime(2024, 3, 20)))),
            new PerfScenario("Calendar.DisabledDate", _ => CreateCalendar(
                disabledDate: date => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)),
            new PerfScenario("Calendar.Compact", _ => CreateCalendar(fullscreen: false)),
            new PerfScenario("Calendar.ShowWeek", _ => CreateCalendar(showWeek: true)),
            new PerfScenario("Calendar.Batch4", _ => CreateCalendarBatch())
        ];
    }

    private static AtomCalendar CreateCalendar(
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

    private static AtomCalendar CreateCalendarWithRangeBars()
    {
        var calendar = CreateCalendar(value: new DateTime(2024, 1, 12));
        calendar.RangeBars.Add(new AtomCalendarRangeBar
        {
            StartDate = new DateTime(2024, 1, 5),
            EndDate   = new DateTime(2024, 1, 7),
            Label     = "Planning"
        });
        calendar.RangeBars.Add(new AtomCalendarRangeBar
        {
            StartDate = new DateTime(2024, 1, 21),
            EndDate   = new DateTime(2024, 1, 25),
            Label     = "Review"
        });
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
                CreateCalendar(value: new DateTime(2024, 1, 20)),
                CreateCalendar(mode: AtomCalendarMode.Year),
                CreateCalendar(showWeek: true, fullscreen: false)
            }
        };
    }
}
