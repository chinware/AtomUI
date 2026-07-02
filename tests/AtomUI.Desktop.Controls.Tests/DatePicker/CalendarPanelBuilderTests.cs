using System;
using System.Globalization;
using System.Linq;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.CalendarView.Rendering;
using AtomUI.Desktop.Controls.CalendarView.State;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DatePickers;

public class CalendarPanelBuilderTests
{
    [Fact]
    public void BuildMonthPanel_Generates_Seven_Titles_And_FortyTwo_Day_Cells()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithFirstDayOfWeek(DayOfWeek.Monday);

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        panel.DayTitles.Count.ShouldBe(7);
        panel.Cells.Count.ShouldBe(42);
        panel.Cells[0].Date.ShouldBe(new DateTime(2026, 6, 1));
        panel.Cells[0].Text.ShouldBe("1");
    }

    [Fact]
    public void BuildMonthPanel_Marks_Selected_Today_And_Inactive_Cells_From_State()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithSelectedDate(new DateTime(2026, 6, 10))
                                     .WithToday(new DateTime(2026, 6, 10));

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        var selectedCell = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 10));
        selectedCell.IsSelected.ShouldBeTrue();
        selectedCell.IsToday.ShouldBeTrue();
        panel.Cells.Any(cell => cell.IsInactive).ShouldBeTrue();
    }

    [Fact]
    public void BuildMonthPanel_Uses_FirstDayOfWeek_To_Position_First_Cell()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithFirstDayOfWeek(DayOfWeek.Sunday);

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        panel.Cells[0].Date.ShouldBe(new DateTime(2026, 5, 31));
        panel.Cells[1].Date.ShouldBe(new DateTime(2026, 6, 1));
    }

    [Fact]
    public void BuildMonthPanel_Marks_Cells_Outside_Display_Range_Disabled_And_Hidden()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithDisplayRange(new DateTime(2026, 6, 5), new DateTime(2026, 6, 20));

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        var beforeStart = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 4));
        var afterEnd    = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 21));
        beforeStart.IsDisabled.ShouldBeTrue();
        beforeStart.IsHidden.ShouldBeTrue();
        afterEnd.IsDisabled.ShouldBeTrue();
        afterEnd.IsHidden.ShouldBeTrue();
    }

    [Fact]
    public void BuildMonthPanel_Renders_Range_Start_Middle_And_End_From_State()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithRangeSelection(new DateTime(2026, 6, 10),
                                         new DateTime(2026, 6, 12),
                                         null,
                                         CalendarRangeActivePart.End,
                                         true);

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 10)).IsRangeStart.ShouldBeTrue();
        panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 11)).IsRangeMiddle.ShouldBeTrue();
        panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 12)).IsRangeEnd.ShouldBeTrue();
    }

    [Fact]
    public void BuildMonthPanel_Uses_HoverDate_For_Active_Range_End_Preview()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithRangeSelection(new DateTime(2026, 6, 10),
                                         null,
                                         new DateTime(2026, 6, 13),
                                         CalendarRangeActivePart.End,
                                         true);

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        var startCell = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 10));
        var middleCell = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 11));
        var hoverCell = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 13));

        startCell.IsSelected.ShouldBeTrue();
        startCell.IsRangeStart.ShouldBeFalse();
        startCell.IsRangePreviewStart.ShouldBeTrue();
        middleCell.IsRangePreviewMiddle.ShouldBeTrue();
        hoverCell.IsSelected.ShouldBeFalse();
        hoverCell.IsRangeEnd.ShouldBeFalse();
        hoverCell.IsRangePreviewEnd.ShouldBeTrue();
    }

    [Fact]
    public void BuildMonthPanel_Does_Not_Treat_Hover_Endpoint_As_Committed_Range_End()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithRangeSelection(new DateTime(2026, 7, 15),
                                         null,
                                         new DateTime(2026, 7, 16),
                                         CalendarRangeActivePart.End,
                                         true);

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        var startCell = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 7, 15));
        var hoverCell = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 7, 16));

        startCell.IsSelected.ShouldBeTrue();
        hoverCell.IsSelected.ShouldBeFalse();
        hoverCell.IsRangeEnd.ShouldBeFalse();
        hoverCell.IsRangePreviewEnd.ShouldBeTrue();
    }

    [Fact]
    public void BuildMonthPanel_Does_Not_Treat_Hover_Endpoint_As_Committed_Range_Start()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithRangeSelection(null,
                                         new DateTime(2026, 7, 16),
                                         new DateTime(2026, 7, 8),
                                         CalendarRangeActivePart.Start,
                                         true);

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        var hoverCell = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 7, 8));
        var endCell   = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 7, 16));

        hoverCell.IsSelected.ShouldBeFalse();
        hoverCell.IsRangeStart.ShouldBeFalse();
        hoverCell.IsRangePreviewStart.ShouldBeTrue();
        endCell.IsSelected.ShouldBeTrue();
        endCell.IsRangeEnd.ShouldBeFalse();
        endCell.IsRangePreviewEnd.ShouldBeTrue();
    }

    [Fact]
    public void BuildMonthPanel_Marks_Blackout_Dates_From_State()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithBlackoutDates(new[]
                                     {
                                         new CalendarDateRange(new DateTime(2026, 6, 12))
                                     });

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        var blackoutCell = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 12));
        blackoutCell.IsBlackout.ShouldBeTrue();
        blackoutCell.IsDisabled.ShouldBeFalse();
    }

    [Fact]
    public void BuildYearPanel_Marks_Selected_Focused_And_Out_Of_Range_Months()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithDisplayRange(new DateTime(2026, 3, 1), new DateTime(2026, 10, 1))
                                     .WithSelectedMonth(new DateTime(2026, 8, 1));

        var panel = CalendarPanelBuilder.BuildYearPanel(state, new DateTime(2026, 1, 1));

        panel.Months.Count.ShouldBe(12);
        panel.Months.Single(cell => cell.Date == new DateTime(2026, 2, 1)).IsDisabled.ShouldBeTrue();
        panel.Months.Single(cell => cell.Date == new DateTime(2026, 6, 1)).IsSelected.ShouldBeTrue();
        panel.Months.Single(cell => cell.Date == new DateTime(2026, 8, 1)).IsFocused.ShouldBeTrue();
    }

    [Fact]
    public void BuildDecadePanel_Marks_Inactive_Selected_And_Focused_Years()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithSelectedYear(new DateTime(2028, 1, 1));

        var panel = CalendarPanelBuilder.BuildDecadePanel(state, new DateTime(2028, 1, 1));

        panel.Years.Count.ShouldBe(12);
        panel.DecadeStart.ShouldBe(2020);
        panel.Years.First().IsInactive.ShouldBeTrue();
        panel.Years.Single(cell => cell.Date == new DateTime(2026, 1, 1)).IsSelected.ShouldBeTrue();
        panel.Years.Single(cell => cell.Date == new DateTime(2028, 1, 1)).IsFocused.ShouldBeTrue();
        panel.Years.Last().IsInactive.ShouldBeTrue();
    }
}
