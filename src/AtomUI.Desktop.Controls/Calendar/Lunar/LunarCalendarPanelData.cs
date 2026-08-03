namespace AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

internal sealed class LunarCalendarPanelData
{
    internal LunarCalendarPanelData(
        IReadOnlyDictionary<DateTime, LunarCalendarDateInfo> dates,
        IReadOnlyDictionary<int, IReadOnlyList<LunarCalendarMonthInfo>> months,
        IReadOnlyDictionary<DateTime, LunarCalendarHoliday> holidays)
    {
        Dates = dates;
        Months = months;
        Holidays = holidays;
    }

    internal IReadOnlyDictionary<DateTime, LunarCalendarDateInfo> Dates { get; }
    internal IReadOnlyDictionary<int, IReadOnlyList<LunarCalendarMonthInfo>> Months { get; }
    internal IReadOnlyDictionary<DateTime, LunarCalendarHoliday> Holidays { get; }
}

internal sealed class LunarCalendarPanelDataKey
{
    internal LunarCalendarPanelDataKey(
        CalendarViewMode viewMode,
        int year,
        int month,
        string cultureName,
        bool showSolarTerms,
        bool showTraditionalFestivals,
        bool showHolidays,
        bool highlightWeekends,
        ILunarCalendarHolidayProvider? provider,
        long providerRevision)
    {
        ViewMode = viewMode;
        Year = year;
        Month = month;
        CultureName = cultureName;
        ShowSolarTerms = showSolarTerms;
        ShowTraditionalFestivals = showTraditionalFestivals;
        ShowHolidays = showHolidays;
        HighlightWeekends = highlightWeekends;
        Provider = provider;
        ProviderRevision = providerRevision;
    }

    private CalendarViewMode ViewMode { get; }
    private int Year { get; }
    private int Month { get; }
    private string CultureName { get; }
    private bool ShowSolarTerms { get; }
    private bool ShowTraditionalFestivals { get; }
    private bool ShowHolidays { get; }
    private bool HighlightWeekends { get; }
    private ILunarCalendarHolidayProvider? Provider { get; }
    private long ProviderRevision { get; }

    internal bool Matches(LunarCalendarPanelDataKey other)
    {
        if (ViewMode == CalendarViewMode.Month || other.ViewMode == CalendarViewMode.Month)
        {
            return ViewMode == other.ViewMode &&
                   Year == other.Year &&
                   CultureName == other.CultureName;
        }

        return ViewMode == other.ViewMode &&
               Year == other.Year &&
               Month == other.Month &&
               CultureName == other.CultureName &&
               ShowSolarTerms == other.ShowSolarTerms &&
               ShowTraditionalFestivals == other.ShowTraditionalFestivals &&
               ShowHolidays == other.ShowHolidays &&
               HighlightWeekends == other.HighlightWeekends &&
               ReferenceEquals(Provider, other.Provider) &&
               ProviderRevision == other.ProviderRevision;
    }
}
