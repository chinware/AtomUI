using System;
using AtomUI.Controls;
using AtomUI.Data;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Calendar;

public class CalendarViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Calendar";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    /// <summary>示例锚点日期，固定值避免示例随当天漂移。</summary>
    public DateTime SampleDate { get; } = new(2026, 7, 15);

    /// <summary>ValidRange 示例：2026 年 7 月 5 日至 7 月 25 日。</summary>
    public DateTime ValidRangeStart { get; } = new(2026, 7, 5);
    public DateTime ValidRangeEnd { get; } = new(2026, 7, 25);

    /// <summary>DisabledDate 示例：禁用周末。</summary>
    public Func<DateTime, bool> DisableWeekends { get; } =
        date => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public CalendarViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
