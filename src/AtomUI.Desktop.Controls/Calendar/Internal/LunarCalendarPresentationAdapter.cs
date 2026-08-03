using System.Globalization;
using System.Windows.Input;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

internal sealed class LunarCalendarPresentationAdapter : ICalendarPresentationAdapter
{
    private readonly LunarCalendar _owner;
    private LunarCalendarPanelDataKey? _panelDataKey;
    private LunarCalendarPanelData? _panelData;
    private long _providerRevision;

    internal LunarCalendarPresentationAdapter(LunarCalendar owner)
    {
        _owner = owner;
    }

    public CalendarPresentationMetrics Metrics { get; } = new(
        LunarCalendarTokenKind.MiniContentHeight,
        LunarCalendarTokenKind.FullCellMinHeight,
        LunarCalendarTokenKind.RangeBarTopOffset);

    public CalendarEffectiveRange GetEffectiveRange(CalendarDateRange? validRange)
    {
        var start = validRange?.Start > LunarCalendar.SupportedRange.Start
            ? validRange.Start
            : LunarCalendar.SupportedRange.Start;
        var end = validRange?.End < LunarCalendar.SupportedRange.End
            ? validRange.End
            : LunarCalendar.SupportedRange.End;
        return end < start
            ? new CalendarEffectiveRange(null, null, true)
            : new CalendarEffectiveRange(start, end, false);
    }

    public CalendarViewCell CreateCell() => new LunarCalendarViewCell();

    public CalendarCellContext CreateCellContext(CalendarView owner, CalendarViewCellModel model) =>
        CreateLunarContext(owner, model);

    public void ApplyCellPresentation(CalendarViewCell cell, CalendarView owner, CalendarViewCellModel model)
    {
        if (cell is LunarCalendarViewCell lunarCell)
        {
            lunarCell.ApplyLunarContext(
                cell.Context as LunarCalendarCellContext ?? CreateLunarContext(owner, model),
                _owner.HighlightWeekends);
        }
    }

    public void ClearCellPresentation(CalendarViewCell cell)
    {
        if (cell is LunarCalendarViewCell lunarCell)
        {
            lunarCell.ClearLunarContext();
        }
    }

    public string GetAutomationName(CalendarView owner, CalendarViewCellModel model)
    {
        var baseName = DefaultCalendarPresentationAdapter.Instance.GetAutomationName(owner, model);
        if (model.Kind == CalendarViewCellKind.Week)
        {
            return baseName;
        }

        var context = CreateLunarContext(owner, model);
        if (model.Kind == CalendarViewCellKind.Month)
        {
            var fullRange = string.Join(", ", context.LunarMonths.Select(month =>
                LunarCalendarFormatter.FormatLunarMonth(month)));
            return string.IsNullOrEmpty(fullRange) ? baseName : $"{baseName}, {fullRange}";
        }

        return string.IsNullOrEmpty(context.SecondaryText)
            ? baseName
            : $"{baseName}, {context.SecondaryText}";
    }

    public string FormatYearOption(int year, CultureInfo culture)
    {
        var info = LunarCalendarDateProjector.Create(new DateTime(year, 7, 1));
        var stemBranch = LunarCalendarFormatter.FormatStemBranch(info.HeavenlyStem, info.EarthlyBranch);
        var zodiac = LunarCalendarFormatter.FormatZodiac(info.Zodiac);
        return $"{year} ({stemBranch}{zodiac}年)";
    }

    public string FormatMonthOption(int year, int month, CultureInfo culture)
    {
        var solarMonth = culture.DateTimeFormat.AbbreviatedMonthNames[month - 1];
        var lunarRange = LunarCalendarFormatter.FormatMonthRange(
            LunarCalendarMonthIntersectionResolver.Resolve(year, month));
        return $"{solarMonth} ({lunarRange})";
    }

    public CalendarHeaderContext CreateHeaderContext(
        AtomUI.Desktop.Controls.Calendar owner,
        ICommand changeValueCommand,
        ICommand changeModeCommand)
    {
        var culture = owner.CurrentCulture;
        var info = _owner.SelectedLunarDateInfo;
        var yearText = FormatYearOption(owner.Value.Year, culture);
        var monthText = LunarCalendarFormatter.FormatMonthRange(
            LunarCalendarMonthIntersectionResolver.Resolve(owner.Value.Year, owner.Value.Month));
        return new LunarCalendarHeaderContext(
            owner.Value.Date,
            owner.Mode,
            changeValueCommand,
            changeModeCommand,
            info,
            LunarCalendar.SupportedRange,
            yearText,
            monthText);
    }

    internal void InvalidatePanelData()
    {
        _panelDataKey = null;
        _panelData = null;
    }

    internal void RefreshHolidayData()
    {
        _providerRevision++;
        InvalidatePanelData();
    }

    private LunarCalendarCellContext CreateLunarContext(CalendarView owner, CalendarViewCellModel model)
    {
        var panelData = GetPanelData(owner);
        var culture = owner.Culture ?? CultureInfo.CurrentCulture;
        panelData.Dates.TryGetValue(model.Value.Date, out var dateInfo);
        panelData.Holidays.TryGetValue(model.Value.Date, out var holiday);
        var months = model.Kind == CalendarViewCellKind.Month && panelData.Months.TryGetValue(model.Value.Month, out var value)
            ? value
            : Array.Empty<LunarCalendarMonthInfo>();
        var (secondaryText, secondaryKind) = ResolveSecondaryText(dateInfo, months, holiday, culture);
        var isAdjustedWorkday = holiday?.Kind == LunarCalendarHolidayKind.Workday;
        var isWeekend = model.Kind == CalendarViewCellKind.Date &&
                        model.Value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday &&
                        !isAdjustedWorkday;

        return new LunarCalendarCellContext(
            model.Value,
            owner.Today == default ? DateTime.Today : owner.Today.Date,
            model.Kind == CalendarViewCellKind.Month ? CalendarCellType.Month : CalendarCellType.Date,
            model.DisplayText,
            model.IsToday,
            model.IsInView,
            model.IsSelected,
            model.IsDisabled,
            dateInfo,
            months,
            holiday,
            secondaryText,
            secondaryKind,
            isWeekend,
            holiday?.Kind == LunarCalendarHolidayKind.Holiday,
            isAdjustedWorkday);
    }

    private (string Text, LunarCalendarSecondaryContentKind Kind) ResolveSecondaryText(
        LunarCalendarDateInfo? dateInfo,
        IReadOnlyList<LunarCalendarMonthInfo> months,
        LunarCalendarHoliday? holiday,
        CultureInfo culture)
    {
        if (months.Count > 0)
        {
            return (LunarCalendarFormatter.FormatMonthRange(months), LunarCalendarSecondaryContentKind.LunarDay);
        }

        if (dateInfo is null)
        {
            return (string.Empty, LunarCalendarSecondaryContentKind.LunarDay);
        }

        if (_owner.ShowHolidays && holiday is not null && !string.IsNullOrEmpty(holiday.Name))
        {
            return (holiday.Name, holiday.Kind == LunarCalendarHolidayKind.Holiday
                ? LunarCalendarSecondaryContentKind.Holiday
                : LunarCalendarSecondaryContentKind.Workday);
        }

        if (_owner.ShowTraditionalFestivals && dateInfo.TraditionalFestivals.Count > 0)
        {
            return (LunarCalendarFormatter.FormatFestival(dateInfo.TraditionalFestivals[0]),
                LunarCalendarSecondaryContentKind.TraditionalFestival);
        }

        if (_owner.ShowSolarTerms && dateInfo.SolarTerm is { } solarTerm)
        {
            return (LunarCalendarFormatter.FormatSolarTerm(solarTerm),
                LunarCalendarSecondaryContentKind.SolarTerm);
        }

        return (LunarCalendarFormatter.FormatLunarDay(dateInfo),
            LunarCalendarSecondaryContentKind.LunarDay);
    }

    private LunarCalendarPanelData GetPanelData(CalendarView view)
    {
        var culture = view.Culture ?? CultureInfo.CurrentCulture;
        var key = new LunarCalendarPanelDataKey(
            view.ViewMode,
            view.Value.Year,
            view.ViewMode == CalendarViewMode.Date ? view.Value.Month : 0,
            culture.Name,
            _owner.ShowSolarTerms,
            _owner.ShowTraditionalFestivals,
            _owner.ShowHolidays,
            _owner.HighlightWeekends,
            _owner.HolidayProvider,
            _providerRevision);
        if (_panelData is not null && _panelDataKey?.Matches(key) == true)
        {
            return _panelData;
        }

        _panelData = view.ViewMode == CalendarViewMode.Date
            ? BuildMonthPanelData(view, culture)
            : BuildYearPanelData(view.Value.Year);
        _panelDataKey = key;
        return _panelData;
    }

    private LunarCalendarPanelData BuildMonthPanelData(CalendarView view, CultureInfo culture)
    {
        var dates = new Dictionary<DateTime, LunarCalendarDateInfo>();
        var supportedDates = view.CellModels
            .Where(model => model.Kind == CalendarViewCellKind.Date &&
                            model.Value >= LunarCalendar.SupportedRange.Start &&
                            model.Value <= LunarCalendar.SupportedRange.End)
            .Select(model => model.Value.Date)
            .Distinct()
            .Order()
            .ToArray();
        foreach (var date in supportedDates)
        {
            dates.Add(date, LunarCalendarDateProjector.Create(date));
        }

        var holidays = new Dictionary<DateTime, LunarCalendarHoliday>();
        if (_owner.ShowHolidays && _owner.HolidayProvider is { } provider && supportedDates.Length > 0)
        {
            var visibleRange = new CalendarDateRange(supportedDates[0], supportedDates[^1]);
            if (provider.TryGetHolidays(visibleRange, culture, out var providerRows))
            {
                if (providerRows is null)
                {
                    throw new InvalidOperationException("A successful holiday provider query must return a non-null collection.");
                }

                foreach (var row in providerRows)
                {
                    if (row.Kind is not LunarCalendarHolidayKind.Holiday and not LunarCalendarHolidayKind.Workday)
                    {
                        throw new ArgumentOutOfRangeException(nameof(row.Kind));
                    }

                    var date = row.Date.Date;
                    if (date < visibleRange.Start || date > visibleRange.End ||
                        date < LunarCalendar.SupportedRange.Start || date > LunarCalendar.SupportedRange.End)
                    {
                        continue;
                    }

                    holidays[date] = new LunarCalendarHoliday(date, row.Name, row.Kind);
                }
            }
        }

        return new LunarCalendarPanelData(dates, new Dictionary<int, IReadOnlyList<LunarCalendarMonthInfo>>(), holidays);
    }

    private static LunarCalendarPanelData BuildYearPanelData(int year)
    {
        var months = new Dictionary<int, IReadOnlyList<LunarCalendarMonthInfo>>(12);
        for (var month = 1; month <= 12; month++)
        {
            months.Add(month, LunarCalendarMonthIntersectionResolver.Resolve(year, month));
        }

        return new LunarCalendarPanelData(
            new Dictionary<DateTime, LunarCalendarDateInfo>(),
            months,
            new Dictionary<DateTime, LunarCalendarHoliday>());
    }
}
