namespace AtomUI.Controls;

public class DataGridColumnFilterEventArgs : EventArgs
{
    public List<string> FilterValues { get; }

    public DataGridColumn Column { get; }

    public DataGridColumnFilterEventArgs(DataGridColumn column, List<string> filterValues)
    {
        Column       = column;
        FilterValues = filterValues;
    }
}