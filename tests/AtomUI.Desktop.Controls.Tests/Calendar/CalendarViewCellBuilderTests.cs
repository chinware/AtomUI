using System;
using System.Globalization;
using System.Linq;
using AtomUI.Desktop.Controls.Internal.Calendar;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class CalendarViewCellBuilderTests
{
    // 2026-07 锚点,周日为周首:7/1 是周三,网格起点应为 6/28(周日)
    [Fact]
    public void BuildDateCells_Returns42Cells_StartingAtGridStart()
    {
        var anchor = new DateTime(2026, 7, 15);
        var cells = CalendarViewCellBuilder.BuildDateCells(
            anchor, new DateTime(2026, 7, 30), DayOfWeek.Sunday, null, null, null);

        cells.Count.ShouldBe(42);
        cells[0].Value.ShouldBe(new DateTime(2026, 6, 28));
        cells[^1].Value.ShouldBe(new DateTime(2026, 8, 8));
    }

    [Fact]
    public void BuildDateCells_MarksInViewAndDisplayText()
    {
        var anchor = new DateTime(2026, 7, 15);
        var cells = CalendarViewCellBuilder.BuildDateCells(
            anchor, new DateTime(2026, 7, 30), DayOfWeek.Sunday, null, null, null);

        var jul1 = cells.First(c => c.Value == new DateTime(2026, 7, 1));
        jul1.IsInView.ShouldBeTrue();
        jul1.DisplayText.ShouldBe("01");
        cells.First(c => c.Value == new DateTime(2026, 6, 28)).IsInView.ShouldBeFalse();
    }

    [Fact]
    public void BuildDateCells_MarksTodayAndSelected()
    {
        var anchor = new DateTime(2026, 7, 30);
        var cells = CalendarViewCellBuilder.BuildDateCells(
            anchor, new DateTime(2026, 7, 30), DayOfWeek.Sunday, null, null, null);

        var cell = cells.First(c => c.Value == new DateTime(2026, 7, 30));
        cell.IsToday.ShouldBeTrue();
        cell.IsSelected.ShouldBeTrue();
    }

    [Fact]
    public void BuildDateCells_DisabledByValidRangeAndDisabledDate()
    {
        var anchor = new DateTime(2026, 7, 15);
        var cells = CalendarViewCellBuilder.BuildDateCells(
            anchor, new DateTime(2026, 7, 30), DayOfWeek.Sunday,
            new DateTime(2026, 7, 10), new DateTime(2026, 7, 20),
            d => d == new DateTime(2026, 7, 15));

        cells.First(c => c.Value == new DateTime(2026, 7, 5)).IsDisabled.ShouldBeTrue();   // 范围外
        cells.First(c => c.Value == new DateTime(2026, 7, 10)).IsDisabled.ShouldBeFalse(); // 边界包含
        cells.First(c => c.Value == new DateTime(2026, 7, 20)).IsDisabled.ShouldBeFalse(); // 边界包含
        cells.First(c => c.Value == new DateTime(2026, 7, 15)).IsDisabled.ShouldBeTrue();  // DisabledDate
    }

    [Fact]
    public void BuildDateCells_MondayFirst_ShiftsGridStart()
    {
        var anchor = new DateTime(2026, 7, 15); // 7/1 周三,周一为首 → 起点 6/29(周一)
        var cells = CalendarViewCellBuilder.BuildDateCells(
            anchor, new DateTime(2026, 7, 30), DayOfWeek.Monday, null, null, null);
        cells[0].Value.ShouldBe(new DateTime(2026, 6, 29));
    }
}
