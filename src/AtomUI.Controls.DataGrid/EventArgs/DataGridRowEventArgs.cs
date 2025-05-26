namespace AtomUI.Controls;

/// <summary>
/// Provides data for <see cref="T:AtomUI.Controls.DataGrid" /> row-related events.
/// </summary>
public class DataGridRowEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Controls.DataGridRowEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridRow">The row that the event occurs for.</param>
    public DataGridRowEventArgs(DataGridRow dataGridRow)
    {
        this.Row = dataGridRow;
    }

    /// <summary>
    /// Gets the row that the event occurs for.
    /// </summary>
    public DataGridRow Row { get; private set; }
}