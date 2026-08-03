using System.Globalization;
using AtomUIGallery.Localization;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Calendar;

public class CalendarViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Calendar";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    /// <summary>示例锚点日期，跟随 Calendar 默认值。</summary>
    public DateTime SampleDate { get; } = DateTime.Today;

    /// <summary>农历示例固定使用春节，确保节日和农历次级文案可见。</summary>
    public DateTime LunarCalendarSampleDate { get; } = new(2024, 2, 10);

    /// <summary>完整农历示例使用的应用级节假日数据 Provider。</summary>
    public ILunarCalendarHolidayProvider LunarCalendarHolidayProvider { get; } =
        new GalleryLunarCalendarHolidayProvider();

    /// <summary>跨日期事件示例固定展示 2026 年 1 月。</summary>
    public DateTime CrossDateEventsSampleDate { get; } = new(2026, 1, 1);

    private DateTime _selectableCalendarValue = new(2017, 1, 25);

    /// <summary>可选择日历当前显示和选中的 Calendar 值。</summary>
    public DateTime SelectableCalendarValue
    {
        get => _selectableCalendarValue;
        set => this.RaiseAndSetIfChanged(ref _selectableCalendarValue, value.Date);
    }

    private DateTime _selectableCalendarSelectedValue = new(2017, 1, 25);

    /// <summary>可选择日历最后一次实际选择，用于 Alert 反馈。</summary>
    public DateTime SelectableCalendarSelectedValue
    {
        get => _selectableCalendarSelectedValue;
        private set
        {
            value = value.Date;
            if (_selectableCalendarSelectedValue != value)
            {
                this.RaiseAndSetIfChanged(ref _selectableCalendarSelectedValue, value);
                this.RaisePropertyChanged(nameof(SelectableCalendarSelectedText));
            }
        }
    }

    public string SelectableCalendarSelectedText => string.Format(
        CultureInfo.CurrentCulture,
        Lang(CalendarShowCaseLangResourceKind.SelectableCalendarSelectedMessage),
        SelectableCalendarSelectedValue);

    public CalendarViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void SelectSelectableCalendarDate(DateTime value)
    {
        SelectableCalendarValue = value;
        SelectableCalendarSelectedValue = value;
    }

    public void RefreshSelectableCalendarText()
    {
        this.RaisePropertyChanged(nameof(SelectableCalendarSelectedText));
    }

    private static string Lang(CalendarShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(CalendarShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            CalendarShowCaseLangResourceKind.SelectableCalendarSelectedMessage =>
                en_US.SelectableCalendarSelectedMessage,
            _ => kind.ToString()
        };
    }
}

internal sealed class GalleryLunarCalendarHolidayProvider : ILunarCalendarHolidayProvider
{
    private static readonly HolidayDefinition[] Definitions =
    [
        new(new DateTime(2024, 2, 9), LunarCalendarHolidayKind.Holiday, "除夕"),
        new(new DateTime(2024, 2, 10), LunarCalendarHolidayKind.Holiday, "春节假期"),
        new(new DateTime(2024, 2, 18), LunarCalendarHolidayKind.Workday, "调休工作日"),
        new(new DateTime(2024, 4, 4), LunarCalendarHolidayKind.Holiday, "清明假期"),
        new(new DateTime(2024, 5, 1), LunarCalendarHolidayKind.Holiday, "劳动节假期")
    ];

    public bool TryGetHolidays(
        CalendarDateRange visibleRange,
        CultureInfo _,
        out IReadOnlyList<LunarCalendarHoliday> holidays)
    {
        var start = visibleRange.Start.Date;
        var end = visibleRange.End.Date;
        var rows = Definitions
            .Where(definition => definition.Date >= start && definition.Date <= end)
            .Select(definition => new LunarCalendarHoliday(
                definition.Date,
                definition.Name,
                definition.Kind))
            .ToArray();

        holidays = rows;
        return rows.Length > 0;
    }

    private sealed record HolidayDefinition(
        DateTime Date,
        LunarCalendarHolidayKind Kind,
        string Name);
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
