namespace AtomUI.Controls;

/// <summary>
/// EventArgs used for the DataGrid's LoadingRowGroup and UnloadingRowGroup events
/// </summary>
public class DataGridRowGroupHeaderEventArgs : EventArgs
{
    /// <summary>
    /// Constructs a DataGridRowGroupHeaderEventArgs instance
    /// </summary>
    /// <param name="rowGroupHeader"></param>
    public DataGridRowGroupHeaderEventArgs(DataGridRowGroupHeader rowGroupHeader)
    {
        RowGroupHeader = rowGroupHeader;
    }

    /// <summary>
    /// DataGridRowGroupHeader associated with this instance
    /// </summary>
    public DataGridRowGroupHeader RowGroupHeader { get; private set; }
}