namespace AtomUI.Desktop.Controls.CalendarView.Models;

internal sealed class CalendarMonthPanelModel
{
    public CalendarMonthPanelModel(
        DateTime displayMonth,
        IReadOnlyList<string> dayTitles,
        IReadOnlyList<CalendarCellState> cells,
        IReadOnlyList<CalendarCellState>? weekCells = null)
    {
        DisplayMonth = displayMonth;
        DayTitles    = dayTitles;
        Cells        = cells;
        WeekCells    = weekCells ?? Array.Empty<CalendarCellState>();
    }

    public DateTime DisplayMonth { get; }
    public IReadOnlyList<string> DayTitles { get; }
    public IReadOnlyList<CalendarCellState> Cells { get; }
    public IReadOnlyList<CalendarCellState> WeekCells { get; }
}
