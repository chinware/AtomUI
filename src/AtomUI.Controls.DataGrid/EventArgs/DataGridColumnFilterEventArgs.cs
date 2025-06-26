namespace AtomUI.Controls;

public class DataGridColumnFilterEventArgs : EventArgs
{
    public DataGridFilterMode FilterMode { get; }
    public List<string> FilterValues { get; }

    public DataGridColumn Column { get; }

    public DataGridColumnFilterEventArgs(DataGridColumn column, DataGridFilterMode filterMode,
                                         List<string> filterValues)
    {
        Column       = column;
        FilterMode   = filterMode;
        FilterValues = filterValues;
    }
}