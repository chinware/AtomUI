namespace AtomUI.Desktop.Controls.CalendarView.Models;

internal sealed class CalendarMonthPanelModel
{
    public CalendarMonthPanelModel(
        DateTime displayMonth,
        IReadOnlyList<string> dayTitles,
        IReadOnlyList<CalendarCellState> cells)
    {
        DisplayMonth = displayMonth;
        DayTitles    = dayTitles;
        Cells        = cells;
    }

    public DateTime DisplayMonth { get; }
    public IReadOnlyList<string> DayTitles { get; }
    public IReadOnlyList<CalendarCellState> Cells { get; }
}
