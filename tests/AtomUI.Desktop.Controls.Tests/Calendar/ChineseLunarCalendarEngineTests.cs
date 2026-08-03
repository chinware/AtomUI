using System.Globalization;
using AtomUI.Desktop.Controls.Internal.Calendar.Lunar;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class ChineseLunarCalendarEngineTests
{
    [Theory]
    [InlineData(1900, 1, 1, 1899, 12, 1, false)]
    [InlineData(1900, 1, 30, 1899, 12, 30, false)]
    [InlineData(1900, 1, 31, 1900, 1, 1, false)]
    [InlineData(2023, 3, 22, 2023, 2, 1, true)]
    [InlineData(2024, 2, 10, 2024, 1, 1, false)]
    [InlineData(2024, 9, 17, 2024, 8, 15, false)]
    [InlineData(2100, 12, 31, 2100, 12, 1, false)]
    public void FromSolar_ReturnsExpectedLunarDate(
        int solarYear,
        int solarMonth,
        int solarDay,
        int lunarYear,
        int lunarMonth,
        int lunarDay,
        bool isLeapMonth)
    {
        var result = ChineseLunarCalendarEngine.FromSolar(
            new DateTime(solarYear, solarMonth, solarDay, 15, 30, 0, DateTimeKind.Utc));

        result.ShouldBe(new ChineseLunarDate(lunarYear, lunarMonth, lunarDay, isLeapMonth));
    }

    [Fact]
    public void SupportedRange_RoundTripsEverySolarDate()
    {
        var current = ChineseLunarCalendarEngine.SupportedStart;
        while (current <= ChineseLunarCalendarEngine.SupportedEnd)
        {
            var lunar = ChineseLunarCalendarEngine.FromSolar(current);
            ChineseLunarCalendarEngine.ToSolar(lunar).ShouldBe(current);
            current = current.AddDays(1);
        }
    }

    [Fact]
    public void OverlapRange_MatchesChineseLunisolarCalendar()
    {
        var reference = new ChineseLunisolarCalendar();
        var start = reference.MinSupportedDateTime.Date > ChineseLunarCalendarEngine.SupportedStart
            ? reference.MinSupportedDateTime.Date
            : ChineseLunarCalendarEngine.SupportedStart;
        var end = reference.MaxSupportedDateTime.Date < ChineseLunarCalendarEngine.SupportedEnd
            ? reference.MaxSupportedDateTime.Date
            : ChineseLunarCalendarEngine.SupportedEnd;

        for (var current = start; current <= end; current = current.AddDays(1))
        {
            var actual = ChineseLunarCalendarEngine.FromSolar(current);
            var referenceYear = reference.GetYear(current);
            var referenceMonth = reference.GetMonth(current);
            var leapMonth = reference.GetLeapMonth(referenceYear);
            var isLeap = leapMonth > 0 && referenceMonth == leapMonth;
            var semanticMonth = leapMonth > 0 && referenceMonth >= leapMonth
                ? referenceMonth - 1
                : referenceMonth;

            actual.ShouldBe(new ChineseLunarDate(
                referenceYear,
                semanticMonth,
                reference.GetDayOfMonth(current),
                isLeap));
        }
    }

    [Fact]
    public void OutOfRangeAndInvalidLunarDates_AreRejected()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            ChineseLunarCalendarEngine.FromSolar(new DateTime(1899, 12, 31)));
        Should.Throw<ArgumentOutOfRangeException>(() =>
            ChineseLunarCalendarEngine.FromSolar(new DateTime(2101, 1, 1)));
        Should.Throw<ArgumentOutOfRangeException>(() =>
            ChineseLunarCalendarEngine.ToSolar(new ChineseLunarDate(2024, 13, 1, false)));
        Should.Throw<ArgumentOutOfRangeException>(() =>
            ChineseLunarCalendarEngine.ToSolar(new ChineseLunarDate(2024, 1, 31, false)));
        Should.Throw<ArgumentOutOfRangeException>(() =>
            ChineseLunarCalendarEngine.ToSolar(new ChineseLunarDate(2024, 1, 1, true)));
    }
}
