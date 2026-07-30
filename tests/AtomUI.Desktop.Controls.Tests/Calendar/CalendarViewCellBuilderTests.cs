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

    [Fact]
    public void BuildWeekNumberCells_ReturnsSixNonFocusableCells()
    {
        var anchor = new DateTime(2026, 7, 15);
        var dateCells = CalendarViewCellBuilder.BuildDateCells(
            anchor, new DateTime(2026, 7, 30), DayOfWeek.Monday, null, null, null);

        var weeks = CalendarViewCellBuilder.BuildWeekNumberCells(
            dateCells, CultureInfo.InvariantCulture,
            CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

        weeks.Count.ShouldBe(6);
        weeks.ShouldAllBe(w => w.Kind == CalendarViewCellKind.Week);
        weeks.ShouldAllBe(w => !w.IsFocusable && !w.IsSelected && !w.IsDisabled);
        // 第一行起点 2026-06-29,ISO 周序号 27
        weeks[0].DisplayText.ShouldBe("27");
    }

    [Fact]
    public void BuildMonthCells_Returns12Cells_WithDayTruncation()
    {
        // anchor 1/31 → February 截断到当年 2 月末
        var anchor = new DateTime(2028, 1, 31); // 闰年
        var cells = CalendarViewCellBuilder.BuildMonthCells(
            anchor, new DateTime(2028, 1, 31), CultureInfo.InvariantCulture, null, null, null);

        cells.Count.ShouldBe(12);
        cells[1].Value.ShouldBe(new DateTime(2028, 2, 29)); // 闰年 2 月末
        cells[0].IsSelected.ShouldBeTrue();                 // anchor 所在 1 月
    }

    [Fact]
    public void BuildMonthCells_NonLeapFebruaryTruncatesTo28()
    {
        var anchor = new DateTime(2027, 1, 31);
        var cells = CalendarViewCellBuilder.BuildMonthCells(
            anchor, new DateTime(2027, 1, 31), CultureInfo.InvariantCulture, null, null, null);
        cells[1].Value.ShouldBe(new DateTime(2027, 2, 28));
    }

    [Fact]
    public void BuildMonthCells_DisabledWhenMonthOutsideValidRange()
    {
        var anchor = new DateTime(2026, 6, 15);
        // 有效范围只覆盖 2026 年 5~7 月
        var cells = CalendarViewCellBuilder.BuildMonthCells(
            anchor, new DateTime(2026, 6, 15), CultureInfo.InvariantCulture,
            new DateTime(2026, 5, 1), new DateTime(2026, 7, 31), null);

        cells[0].IsDisabled.ShouldBeTrue();  // 1 月无交集
        cells[4].IsDisabled.ShouldBeFalse(); // 5 月相交
        cells[11].IsDisabled.ShouldBeTrue(); // 12 月无交集
    }
}
