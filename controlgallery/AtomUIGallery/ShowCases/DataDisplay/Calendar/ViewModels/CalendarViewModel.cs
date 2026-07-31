using System;
using System.Globalization;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using Avalonia.Data.Converters;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Calendar;

public class CalendarViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Calendar";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    /// <summary>示例锚点日期，跟随 Calendar 默认值。</summary>
    public DateTime SampleDate { get; } = DateTime.Today;

    /// <summary>跨日期事件示例固定展示 2026 年 1 月。</summary>
    public DateTime CrossDateEventsSampleDate { get; } = new(2026, 1, 1);

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

public sealed class NoticeCalendarDateEventVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not CalendarCellContext { CellType: CalendarCellType.Date } context ||
            parameter is not string days)
        {
            return false;
        }

        foreach (var dayText in days.Split(
                     new[] { ',', '|' },
                     StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (int.TryParse(dayText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var day) &&
                day == context.Value.Day)
            {
                return true;
            }
        }

        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public sealed class NoticeCalendarDateCellVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is CalendarCellContext { CellType: CalendarCellType.Date };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public sealed class NoticeCalendarMonthBacklogVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is CalendarCellContext { CellType: CalendarCellType.Month, Value.Month: 9 };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
