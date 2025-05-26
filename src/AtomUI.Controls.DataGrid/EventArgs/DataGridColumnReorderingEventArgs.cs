using System.ComponentModel;
using Avalonia.Controls;

namespace AtomUI.Controls;

/// <summary>
/// Provides data for the <see cref="E:AtomUI.Controls.DataGrid.ColumnReordering" /> event.
/// </summary>
public class DataGridColumnReorderingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Controls.DataGridColumnReorderingEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridColumn"></param>
    public DataGridColumnReorderingEventArgs(DataGridColumn dataGridColumn)
    {
        Column = dataGridColumn;
    }

    /// <summary>
    /// The column being moved.
    /// </summary>
    public DataGridColumn Column { get; private set; }

    /// <summary>
    /// The popup indicator displayed while dragging.  If null and Handled = true, then do not display a tooltip.
    /// </summary>
    public Control? DragIndicator { get; set; }

    /// <summary>
    /// UIElement to display at the insertion position.  If null and Handled = true, then do not display an insertion indicator.
    /// </summary>
    public Control? DropLocationIndicator { get; set; }
}
