using System;
using System.Globalization;
using System.Linq;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.CalendarView.Models;
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
    public void BuildMonthPanel_Week_Mode_Adds_Week_Number_Cells_And_Row_Selection()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Week)
                                     .WithFirstDayOfWeek(DayOfWeek.Monday)
                                     .WithSelectedDate(new DateTime(2026, 7, 6));

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        panel.WeekCells.Count.ShouldBe(6);
        panel.WeekCells.Select(cell => cell.Text)
             .ShouldBe(new[] { "27", "28", "29", "30", "31", "32" });

        var selectedWeekCell = panel.WeekCells.Single(cell => cell.Date == new DateTime(2026, 7, 6));
        selectedWeekCell.IsWeekSelectionStart.ShouldBeTrue();
        selectedWeekCell.IsWeekSelectionMiddle.ShouldBeFalse();
        selectedWeekCell.IsWeekSelectionEnd.ShouldBeFalse();

        var selectedDates = panel.Cells
                                 .Where(cell => cell.Date >= new DateTime(2026, 7, 6) &&
                                                cell.Date <= new DateTime(2026, 7, 12))
                                 .ToArray();
        selectedDates.Length.ShouldBe(7);
        selectedDates.Take(6).ShouldAllBe(cell => cell.IsWeekSelectionMiddle);
        selectedDates.Last().IsWeekSelectionEnd.ShouldBeTrue();
        selectedDates.ShouldAllBe(cell => cell.IsSelected);
    }

    [Fact]
    public void BuildMonthPanel_Week_Mode_Uses_Hover_Date_For_Row_Hover()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Week)
                                     .WithFirstDayOfWeek(DayOfWeek.Monday)
                                     .WithHoverDate(new DateTime(2026, 7, 22));

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        var hoverWeekCell = panel.WeekCells.Single(cell => cell.Date == new DateTime(2026, 7, 20));
        hoverWeekCell.IsWeekHoverStart.ShouldBeTrue();
        hoverWeekCell.IsSelected.ShouldBeFalse();

        var hoverDates = panel.Cells
                              .Where(cell => cell.Date >= new DateTime(2026, 7, 20) &&
                                             cell.Date <= new DateTime(2026, 7, 26))
                              .ToArray();

        hoverDates.Length.ShouldBe(7);
        hoverDates.Take(6).ShouldAllBe(cell => cell.IsWeekHoverMiddle);
        hoverDates.Last().IsWeekHoverEnd.ShouldBeTrue();
        hoverDates.ShouldAllBe(cell => !cell.IsSelected);
    }

    [Fact]
    public void BuildMonthPanel_Week_Mode_Range_Uses_Row_Range_States_Without_Date_Range_States()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Week)
                                     .WithFirstDayOfWeek(DayOfWeek.Monday)
                                     .WithRangeSelection(new DateTime(2026, 7, 13),
                                         new DateTime(2026, 8, 17),
                                         null,
                                         CalendarRangeActivePart.End,
                                         true);

        var julyPanel   = CalendarPanelBuilder.BuildMonthPanel(state, new DateTime(2026, 7, 1));
        var augustPanel = CalendarPanelBuilder.BuildMonthPanel(state, new DateTime(2026, 8, 1));

        AssertSelectedWeekRow(julyPanel, "29", new DateTime(2026, 7, 13), new DateTime(2026, 7, 19));
        AssertWeekRangeRow(julyPanel, "30", new DateTime(2026, 7, 20), new DateTime(2026, 7, 26));
        AssertWeekRangeRow(augustPanel, "33", new DateTime(2026, 8, 10), new DateTime(2026, 8, 16));
        AssertSelectedWeekRow(augustPanel, "34", new DateTime(2026, 8, 17), new DateTime(2026, 8, 23));
    }

    [Fact]
    public void BuildMonthPanel_Week_Mode_Range_Preview_Uses_Row_Selection_Endpoint()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Week)
                                     .WithFirstDayOfWeek(DayOfWeek.Monday)
                                     .WithRangeSelection(new DateTime(2026, 7, 13),
                                         null,
                                         new DateTime(2026, 8, 10),
                                         CalendarRangeActivePart.End,
                                         true);

        var julyPanel   = CalendarPanelBuilder.BuildMonthPanel(state, new DateTime(2026, 7, 1));
        var augustPanel = CalendarPanelBuilder.BuildMonthPanel(state, new DateTime(2026, 8, 1));

        AssertSelectedWeekRow(julyPanel, "29", new DateTime(2026, 7, 13), new DateTime(2026, 7, 19));
        AssertWeekRangeRow(julyPanel, "30", new DateTime(2026, 7, 20), new DateTime(2026, 7, 26));
        AssertWeekRangeRow(augustPanel, "32", new DateTime(2026, 8, 3), new DateTime(2026, 8, 9));
        AssertPreviewWeekEndpointRow(augustPanel, "33", new DateTime(2026, 8, 10), new DateTime(2026, 8, 16));
    }

    [Fact]
    public void BuildMonthPanel_Keeps_Cells_Outside_Display_Range_Visible_And_Disabled()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 6, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithDisplayRange(new DateTime(2026, 6, 5), new DateTime(2026, 6, 20));

        var panel = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);

        var beforeStart = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 4));
        var afterEnd    = panel.Cells.Single(cell => cell.Date == new DateTime(2026, 6, 21));
        beforeStart.IsDisabled.ShouldBeTrue();
        beforeStart.IsHidden.ShouldBeFalse();
        afterEnd.IsDisabled.ShouldBeTrue();
        afterEnd.IsHidden.ShouldBeFalse();
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

        startCell.IsSelected.ShouldBeFalse();
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

        startCell.IsSelected.ShouldBeFalse();
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
        endCell.IsSelected.ShouldBeFalse();
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
        panel.Months.Single(cell => cell.Date == new DateTime(2026, 2, 1))
             .ShouldSatisfyAllConditions(
                 cell => cell.IsDisabled.ShouldBeTrue(),
                 cell => cell.IsHidden.ShouldBeFalse());
        panel.Months.Single(cell => cell.Date == new DateTime(2026, 6, 1)).IsSelected.ShouldBeTrue();
        panel.Months.Single(cell => cell.Date == new DateTime(2026, 8, 1)).IsFocused.ShouldBeTrue();
    }

    [Fact]
    public void BuildQuarterPanel_Keeps_Out_Of_Range_Quarters_Visible_And_Disabled()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Quarter)
                                     .WithDisplayRange(new DateTime(2026, 4, 1), new DateTime(2026, 9, 30));

        var panel = CalendarPanelBuilder.BuildQuarterPanel(state, state.DisplayDate);

        panel.Quarters[0].ShouldSatisfyAllConditions(
            cell => cell.IsDisabled.ShouldBeTrue(),
            cell => cell.IsHidden.ShouldBeFalse());
        panel.Quarters[3].ShouldSatisfyAllConditions(
            cell => cell.IsDisabled.ShouldBeTrue(),
            cell => cell.IsHidden.ShouldBeFalse());
    }

    [Fact]
    public void BuildDecadePanel_Keeps_Out_Of_Range_Years_Visible_And_Disabled()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 1, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Year)
                                     .WithDisplayRange(new DateTime(2025, 1, 1), new DateTime(2028, 1, 1));

        var panel = CalendarPanelBuilder.BuildDecadePanel(state, state.DisplayDate);
        var year  = panel.Years.Single(cell => cell.Date == new DateTime(2029, 1, 1));

        year.IsDisabled.ShouldBeTrue();
        year.IsHidden.ShouldBeFalse();
    }

    [Fact]
    public void BuildMonthPanel_Keeps_Out_Of_Range_Week_Number_Visible_And_Disabled()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Week)
                                     .WithFirstDayOfWeek(DayOfWeek.Monday)
                                     .WithDisplayRange(new DateTime(2026, 7, 13), new DateTime(2026, 7, 19));

        var panel    = CalendarPanelBuilder.BuildMonthPanel(state, state.DisplayDate);
        var weekCell = panel.WeekCells.Single(cell => cell.Date == new DateTime(2026, 7, 6));

        weekCell.IsDisabled.ShouldBeTrue();
        weekCell.IsHidden.ShouldBeFalse();
    }

    [Fact]
    public void BuildYearPanel_Month_Mode_Uses_Range_Preview_For_Month_Cells()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 4, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Month)
                                     .WithRangeSelection(new DateTime(2026, 4, 1),
                                         null,
                                         new DateTime(2027, 8, 1),
                                         CalendarRangeActivePart.End,
                                         true);

        var firstPanel  = CalendarPanelBuilder.BuildYearPanel(state, new DateTime(2026, 1, 1));
        var secondPanel = CalendarPanelBuilder.BuildYearPanel(state, new DateTime(2027, 1, 1));

        var start = firstPanel.Months.Single(cell => cell.Date == new DateTime(2026, 4, 1));
        start.IsSelected.ShouldBeFalse();
        start.IsRangePreviewStart.ShouldBeTrue();
        start.IsRangeStart.ShouldBeFalse();

        firstPanel.Months.Single(cell => cell.Date == new DateTime(2026, 3, 1))
                  .ShouldSatisfyAllConditions(
                      cell => cell.IsRangePreviewMiddle.ShouldBeFalse(),
                      cell => cell.IsRangeMiddle.ShouldBeFalse());
        firstPanel.Months.Single(cell => cell.Date == new DateTime(2026, 5, 1))
                  .IsRangePreviewMiddle.ShouldBeTrue();
        secondPanel.Months.Single(cell => cell.Date == new DateTime(2027, 7, 1))
                   .IsRangePreviewMiddle.ShouldBeTrue();

        var end = secondPanel.Months.Single(cell => cell.Date == new DateTime(2027, 8, 1));
        end.IsSelected.ShouldBeFalse();
        end.IsRangePreviewEnd.ShouldBeTrue();
        end.IsRangeEnd.ShouldBeFalse();

        secondPanel.Months.Single(cell => cell.Date == new DateTime(2027, 9, 1))
                   .ShouldSatisfyAllConditions(
                       cell => cell.IsRangePreviewMiddle.ShouldBeFalse(),
                       cell => cell.IsRangeMiddle.ShouldBeFalse());
    }

    [Fact]
    public void BuildQuarterPanel_Generates_Four_Quarter_Cells()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 7, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Quarter)
                                     .WithSelectedDate(new DateTime(2026, 7, 1));

        var panel = CalendarPanelBuilder.BuildQuarterPanel(state, new DateTime(2026, 1, 1));

        panel.Quarters.Count.ShouldBe(4);
        panel.Quarters.Select(cell => cell.Text).ShouldBe(new[] { "Q1", "Q2", "Q3", "Q4" });
        panel.Quarters.Single(cell => cell.Date == new DateTime(2026, 7, 1)).IsSelected.ShouldBeTrue();
    }

    [Fact]
    public void BuildQuarterPanel_Quarter_Mode_Uses_Range_Preview_For_Quarter_Cells()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 4, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Quarter)
                                     .WithRangeSelection(new DateTime(2026, 4, 1),
                                         null,
                                         new DateTime(2027, 7, 1),
                                         CalendarRangeActivePart.End,
                                         true);

        var firstPanel  = CalendarPanelBuilder.BuildQuarterPanel(state, new DateTime(2026, 1, 1));
        var secondPanel = CalendarPanelBuilder.BuildQuarterPanel(state, new DateTime(2027, 1, 1));

        var start = firstPanel.Quarters.Single(cell => cell.Date == new DateTime(2026, 4, 1));
        start.IsSelected.ShouldBeFalse();
        start.IsRangePreviewStart.ShouldBeTrue();
        start.IsRangeStart.ShouldBeFalse();

        firstPanel.Quarters.Single(cell => cell.Date == new DateTime(2026, 7, 1))
                  .IsRangePreviewMiddle.ShouldBeTrue();
        secondPanel.Quarters.Single(cell => cell.Date == new DateTime(2027, 4, 1))
                   .IsRangePreviewMiddle.ShouldBeTrue();

        var end = secondPanel.Quarters.Single(cell => cell.Date == new DateTime(2027, 7, 1));
        end.IsSelected.ShouldBeFalse();
        end.IsRangePreviewEnd.ShouldBeTrue();
        end.IsRangeEnd.ShouldBeFalse();

        secondPanel.Quarters.Single(cell => cell.Date == new DateTime(2027, 10, 1))
                   .ShouldSatisfyAllConditions(
                       cell => cell.IsRangePreviewMiddle.ShouldBeFalse(),
                       cell => cell.IsRangeMiddle.ShouldBeFalse());
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

    [Fact]
    public void BuildDecadePanel_Year_Mode_Uses_Range_Preview_For_Year_Cells()
    {
        var state = CalendarViewState.CreateDefault(new DateTime(2026, 1, 1),
            CultureInfo.InvariantCulture.DateTimeFormat)
                                     .WithPickerMode(DatePickerMode.Year)
                                     .WithRangeSelection(new DateTime(2026, 1, 1),
                                         null,
                                         new DateTime(2032, 1, 1),
                                         CalendarRangeActivePart.End,
                                         true);

        var firstPanel  = CalendarPanelBuilder.BuildDecadePanel(state, new DateTime(2026, 1, 1));
        var secondPanel = CalendarPanelBuilder.BuildDecadePanel(state, new DateTime(2032, 1, 1));

        var start = firstPanel.Years.Single(cell => cell.Date == new DateTime(2026, 1, 1));
        start.IsSelected.ShouldBeFalse();
        start.IsRangePreviewStart.ShouldBeTrue();
        start.IsRangeStart.ShouldBeFalse();

        firstPanel.Years.Single(cell => cell.Date == new DateTime(2025, 1, 1))
                  .ShouldSatisfyAllConditions(
                      cell => cell.IsRangePreviewMiddle.ShouldBeFalse(),
                      cell => cell.IsRangeMiddle.ShouldBeFalse());
        firstPanel.Years.Single(cell => cell.Date == new DateTime(2027, 1, 1))
                  .IsRangePreviewMiddle.ShouldBeTrue();
        secondPanel.Years.Single(cell => cell.Date == new DateTime(2031, 1, 1))
                   .IsRangePreviewMiddle.ShouldBeTrue();

        var end = secondPanel.Years.Single(cell => cell.Date == new DateTime(2032, 1, 1));
        end.IsSelected.ShouldBeFalse();
        end.IsRangePreviewEnd.ShouldBeTrue();
        end.IsRangeEnd.ShouldBeFalse();

        secondPanel.Years.Single(cell => cell.Date == new DateTime(2033, 1, 1))
                   .ShouldSatisfyAllConditions(
                       cell => cell.IsRangePreviewMiddle.ShouldBeFalse(),
                       cell => cell.IsRangeMiddle.ShouldBeFalse());
    }

    private static void AssertSelectedWeekRow(
        CalendarMonthPanelModel panel,
        string weekText,
        DateTime startDate,
        DateTime endDate)
    {
        var weekCell = panel.WeekCells.Single(cell => cell.Text == weekText);
        weekCell.IsWeekSelectionStart.ShouldBeTrue();
        AssertNoDateRangeState(weekCell);

        var dates = panel.Cells
                         .Where(cell => cell.Date >= startDate && cell.Date <= endDate)
                         .OrderBy(cell => cell.Date)
                         .ToArray();

        dates.Length.ShouldBe(7);
        dates.Take(6).ShouldAllBe(cell => cell.IsWeekSelectionMiddle);
        dates.Last().IsWeekSelectionEnd.ShouldBeTrue();
        dates.ShouldAllBe(cell => cell.IsSelected);
        dates.ShouldAllBe(cell => HasNoDateRangeState(cell));
        dates.ShouldAllBe(cell => !cell.IsWeekRangeStart &&
                                  !cell.IsWeekRangeMiddle &&
                                  !cell.IsWeekRangeEnd);
    }

    private static void AssertWeekRangeRow(
        CalendarMonthPanelModel panel,
        string weekText,
        DateTime startDate,
        DateTime endDate)
    {
        var weekCell = panel.WeekCells.Single(cell => cell.Text == weekText);
        weekCell.IsWeekRangeStart.ShouldBeTrue();
        weekCell.IsWeekSelectionStart.ShouldBeFalse();
        AssertNoDateRangeState(weekCell);

        var dates = panel.Cells
                         .Where(cell => cell.Date >= startDate && cell.Date <= endDate)
                         .OrderBy(cell => cell.Date)
                         .ToArray();

        dates.Length.ShouldBe(7);
        dates.Take(6).ShouldAllBe(cell => cell.IsWeekRangeMiddle);
        dates.Last().IsWeekRangeEnd.ShouldBeTrue();
        dates.ShouldAllBe(cell => !cell.IsSelected);
        dates.ShouldAllBe(cell => HasNoDateRangeState(cell));
    }

    private static void AssertPreviewWeekEndpointRow(
        CalendarMonthPanelModel panel,
        string weekText,
        DateTime startDate,
        DateTime endDate)
    {
        var weekCell = panel.WeekCells.Single(cell => cell.Text == weekText);
        weekCell.IsWeekSelectionStart.ShouldBeTrue();
        weekCell.IsSelected.ShouldBeFalse();
        AssertNoDateRangeState(weekCell);

        var dates = panel.Cells
                         .Where(cell => cell.Date >= startDate && cell.Date <= endDate)
                         .OrderBy(cell => cell.Date)
                         .ToArray();

        dates.Length.ShouldBe(7);
        dates.Take(6).ShouldAllBe(cell => cell.IsWeekSelectionMiddle);
        dates.Last().IsWeekSelectionEnd.ShouldBeTrue();
        dates.ShouldAllBe(cell => !cell.IsSelected);
        dates.ShouldAllBe(cell => HasNoDateRangeState(cell));
        dates.ShouldAllBe(cell => !cell.IsWeekRangeStart &&
                                  !cell.IsWeekRangeMiddle &&
                                  !cell.IsWeekRangeEnd);
    }

    private static void AssertNoDateRangeState(CalendarCellState cell)
    {
        HasNoDateRangeState(cell).ShouldBeTrue();
    }

    private static bool HasNoDateRangeState(CalendarCellState cell)
    {
        return !cell.IsRangeStart &&
               !cell.IsRangeEnd &&
               !cell.IsRangeMiddle &&
               !cell.IsRangePreviewStart &&
               !cell.IsRangePreviewEnd &&
               !cell.IsRangePreviewMiddle;
    }

}
