namespace AtomUI.Desktop.Controls.CalendarView.Models;

internal sealed class CalendarDecadePanelModel
{
    public CalendarDecadePanelModel(int decadeStart, IReadOnlyList<CalendarCellState> years)
    {
        DecadeStart = decadeStart;
        Years       = years;
    }

    public int DecadeStart { get; }
    public IReadOnlyList<CalendarCellState> Years { get; }
}
