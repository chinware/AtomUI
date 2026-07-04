namespace AtomUI.Desktop.Controls;

public class DataGridColumnFilterEventArgs : EventArgs
{
    public List<object> FilterValues { get; }

    public DataGridColumn Column { get; }

    public DataGridColumnFilterEventArgs(DataGridColumn column, List<object> filterValues)
    {
        Column       = column;
        FilterValues = filterValues;
    }
}
