namespace AtomUI.Desktop.Controls.CalendarView.Models;

internal sealed class CalendarQuarterPanelModel
{
    public CalendarQuarterPanelModel(DateTime displayYear, IReadOnlyList<CalendarCellState> quarters)
    {
        DisplayYear = displayYear;
        Quarters    = quarters;
    }

    public DateTime DisplayYear { get; }
    public IReadOnlyList<CalendarCellState> Quarters { get; }
}
