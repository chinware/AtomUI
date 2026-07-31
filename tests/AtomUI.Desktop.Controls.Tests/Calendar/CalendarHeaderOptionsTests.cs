using System;
using System.Linq;
using AtomUI.Desktop.Controls.Internal.Calendar;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarHeaderOptionsTests
{
    [Fact]
    public void BuildYearOptions_NoRange_Returns20YearWindow()
    {
        var years = CalendarHeaderOptions.BuildYearOptions(2026, null, null);
        years.Count.ShouldBe(20);
        years[0].ShouldBe(2016);   // 2026 - 10
        years[^1].ShouldBe(2035);  // 2026 + 9
    }

    [Fact]
    public void BuildYearOptions_WithRange_InclusiveStartToEnd()
    {
        var years = CalendarHeaderOptions.BuildYearOptions(2026, 2024, 2028);
        years.ShouldBe(new[] { 2024, 2025, 2026, 2027, 2028 });
    }

    [Theory]
    [InlineData(1, 1, 10)]
    [InlineData(9999, 9989, 9999)]
    public void BuildYearOptions_DateTimeBoundary_OnlyReturnsValidYears(
        int currentYear,
        int expectedFirst,
        int expectedLast)
    {
        var years = CalendarHeaderOptions.BuildYearOptions(currentYear, null, null);

        years[0].ShouldBe(expectedFirst);
        years[^1].ShouldBe(expectedLast);
        years.ShouldAllBe(year => year >= 1 && year <= 9999);
    }

    [Fact]
    public void BuildMonthOptions_NoRange_ReturnsAll12()
    {
        var months = CalendarHeaderOptions.BuildMonthOptions(2026, null, null);
        months.ShouldBe(Enumerable.Range(1, 12).ToArray());
    }

    [Fact]
    public void BuildMonthOptions_StartBoundaryYear_TrimsLeadingMonths()
    {
        var months = CalendarHeaderOptions.BuildMonthOptions(
            2026, new DateTime(2026, 5, 10), new DateTime(2027, 12, 31));
        months[0].ShouldBe(5);
        months[^1].ShouldBe(12);
    }

    [Fact]
    public void BuildMonthOptions_EndBoundaryYear_TrimsTrailingMonths()
    {
        var months = CalendarHeaderOptions.BuildMonthOptions(
            2027, new DateTime(2026, 1, 1), new DateTime(2027, 8, 15));
        months[0].ShouldBe(1);
        months[^1].ShouldBe(8);
    }

    [Fact]
    public void BuildMonthOptions_YearOutsideRange_ReturnsEmpty()
    {
        var before = CalendarHeaderOptions.BuildMonthOptions(
            2020, new DateTime(2026, 1, 1), new DateTime(2027, 12, 31));
        before.ShouldBeEmpty();
    }

    [Fact]
    public void ClampMonthToYear_ClampsToBoundary()
    {
        // 2026 年只允许 5~12 月，当前想要 3 月 → 收敛到 5
        CalendarHeaderOptions.ClampMonthToYear(
            3, 2026, new DateTime(2026, 5, 1), new DateTime(2027, 12, 31)).ShouldBe(5);
        // 当前想要 8 月,在范围内 → 保持 8
        CalendarHeaderOptions.ClampMonthToYear(
            8, 2026, new DateTime(2026, 5, 1), new DateTime(2027, 12, 31)).ShouldBe(8);
    }
}
