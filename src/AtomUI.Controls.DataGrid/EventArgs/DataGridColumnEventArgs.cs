using System.ComponentModel;

namespace AtomUI.Controls;

/// <summary>
/// Provides data for <see cref="T:AtomUI.Controls.DataGrid" /> column-related events.
/// </summary>
public class DataGridColumnEventArgs : HandledEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Controls.DataGridColumnEventArgs" /> class.
    /// </summary>
    /// <param name="column">The column that the event occurs for.</param>
    public DataGridColumnEventArgs(DataGridColumn column)
    {
        Column = column ?? throw new ArgumentNullException(nameof(column));
    }

    /// <summary>
    /// Gets the column that the event occurs for.
    /// </summary>
    public DataGridColumn Column { get; private set; }
}