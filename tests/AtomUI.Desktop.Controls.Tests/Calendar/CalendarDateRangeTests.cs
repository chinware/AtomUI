using System;
using AtomUI.Desktop.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarDateRangeTests
{
    [Fact]
    public void Constructor_NormalizesToDate()
    {
        var r = new CalendarDateRange(new DateTime(2026, 7, 1, 13, 30, 0), new DateTime(2026, 7, 31, 9, 0, 0));
        r.Start.ShouldBe(new DateTime(2026, 7, 1));
        r.End.ShouldBe(new DateTime(2026, 7, 31));
    }

    [Fact]
    public void Constructor_ThrowsWhenEndBeforeStart()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
            new CalendarDateRange(new DateTime(2026, 7, 31), new DateTime(2026, 7, 1)));
    }

    [Fact]
    public void Constructor_AllowsSameStartAndEnd()
    {
        var r = new CalendarDateRange(new DateTime(2026, 7, 15), new DateTime(2026, 7, 15));
        r.Start.ShouldBe(new DateTime(2026, 7, 15));
        r.End.ShouldBe(new DateTime(2026, 7, 15));
    }
}
