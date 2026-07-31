using System;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Calendar;

public class CalendarViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Calendar";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    /// <summary>示例锚点日期，跟随 Calendar 默认值。</summary>
    public DateTime SampleDate { get; } = DateTime.Today;

    /// <summary>ValidRange 示例：围绕初始日期展示前后边界。</summary>
    public CalendarDateRange SampleValidRange { get; } =
        new(DateTime.Today.AddDays(-10), DateTime.Today.AddDays(10));

    /// <summary>DisabledDate 示例：禁用周末。</summary>
    public Func<DateTime, bool> DisableWeekends { get; } =
        date => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    private string _eventLog = string.Empty;

    /// <summary>事件示例的日志文本，由 View 事件处理写入。</summary>
    public string EventLog
    {
        get => _eventLog;
        set => this.RaiseAndSetIfChanged(ref _eventLog, value);
    }

    public CalendarViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
