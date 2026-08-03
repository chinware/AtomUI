using System.Collections.ObjectModel;

namespace AtomUI.Desktop.Controls;

public sealed record LunarCalendarDateInfo
{
    public LunarCalendarDateInfo(
        DateTime solarDate,
        int lunarYear,
        int lunarMonth,
        int lunarDay,
        bool isLeapMonth,
        ChineseHeavenlyStem heavenlyStem,
        ChineseEarthlyBranch earthlyBranch,
        ChineseZodiac zodiac,
        ChineseSolarTerm? solarTerm,
        IReadOnlyList<ChineseTraditionalFestival> traditionalFestivals)
    {
        ArgumentNullException.ThrowIfNull(traditionalFestivals);
        if (lunarMonth is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(lunarMonth));
        }

        if (lunarDay is < 1 or > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(lunarDay));
        }

        SolarDate = solarDate.Date;
        LunarYear = lunarYear;
        LunarMonth = lunarMonth;
        LunarDay = lunarDay;
        IsLeapMonth = isLeapMonth;
        HeavenlyStem = heavenlyStem;
        EarthlyBranch = earthlyBranch;
        Zodiac = zodiac;
        SolarTerm = solarTerm;
        TraditionalFestivals = new ReadOnlyCollection<ChineseTraditionalFestival>(traditionalFestivals.ToArray());
    }

    public DateTime SolarDate { get; }
    public int LunarYear { get; }
    public int LunarMonth { get; }
    public int LunarDay { get; }
    public bool IsLeapMonth { get; }
    public ChineseHeavenlyStem HeavenlyStem { get; }
    public ChineseEarthlyBranch EarthlyBranch { get; }
    public ChineseZodiac Zodiac { get; }
    public ChineseSolarTerm? SolarTerm { get; }
    public IReadOnlyList<ChineseTraditionalFestival> TraditionalFestivals { get; }
}

public sealed record LunarCalendarMonthInfo(int LunarYear, int LunarMonth, bool IsLeapMonth)
{
    public int LunarMonth { get; } = LunarMonth is >= 1 and <= 12
        ? LunarMonth
        : throw new ArgumentOutOfRangeException(nameof(LunarMonth));
}
