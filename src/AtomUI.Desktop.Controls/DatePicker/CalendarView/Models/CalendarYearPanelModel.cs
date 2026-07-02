namespace AtomUI.Desktop.Controls.CalendarView.Models;

internal sealed class CalendarYearPanelModel
{
    public CalendarYearPanelModel(DateTime displayYear, IReadOnlyList<CalendarCellState> months)
    {
        DisplayYear = displayYear;
        Months      = months;
    }

    public DateTime DisplayYear { get; }
    public IReadOnlyList<CalendarCellState> Months { get; }
}
