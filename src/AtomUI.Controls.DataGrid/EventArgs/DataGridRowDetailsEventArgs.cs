using Avalonia.Controls;

namespace AtomUI.Controls;

/// <summary>
/// Provides data for the <see cref="E:AtomUI.Controls.DataGrid.LoadingRowDetails" />, <see cref="E:AtomUI.Controls.DataGrid.UnloadingRowDetails" />, 
/// and <see cref="E:AtomUI.Controls.DataGrid.RowDetailsVisibilityChanged" /> events.
/// </summary>
public class DataGridRowDetailsEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Controls.DataGridRowDetailsEventArgs" /> class. 
    /// </summary>
    /// <param name="row">The row that the event occurs for.</param>
    /// <param name="detailsElement">The row details section as a framework element.</param>
    public DataGridRowDetailsEventArgs(DataGridRow row, Control detailsElement)
    {
        Row            = row;
        DetailsElement = detailsElement;
    }

    /// <summary>
    /// Gets the row details section as a framework element.
    /// </summary>
    public Control DetailsElement { get; private set; }

    /// <summary>
    /// Gets the row that the event occurs for.
    /// </summary>
    public DataGridRow Row { get; private set; }
}