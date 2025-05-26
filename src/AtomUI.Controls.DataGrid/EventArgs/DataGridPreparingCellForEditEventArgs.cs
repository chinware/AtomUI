using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

/// <summary>
/// Provides data for the <see cref="E:AtomUI.Controls.DataGrid.PreparingCellForEdit" /> event.
/// </summary>
public class DataGridPreparingCellForEditEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Controls.DataGridPreparingCellForEditEventArgs" /> class.
    /// </summary>
    /// <param name="column">The column that contains the cell to be edited.</param>
    /// <param name="row">The row that contains the cell to be edited.</param>
    /// <param name="editingEventArgs">Information about the user gesture that caused the cell to enter edit mode.</param>
    /// <param name="editingElement">The element that the column displays for a cell in editing mode.</param>
    public DataGridPreparingCellForEditEventArgs(DataGridColumn column,
                                                 DataGridRow row,
                                                 RoutedEventArgs editingEventArgs,
                                                 Control editingElement)
    {
        Column           = column;
        Row              = row;
        EditingEventArgs = editingEventArgs;
        EditingElement   = editingElement;
    }

    /// <summary>
    /// Gets the column that contains the cell to be edited.
    /// </summary>
    public DataGridColumn Column { get; private set; }

    /// <summary>
    /// Gets the element that the column displays for a cell in editing mode.
    /// </summary>
    public Control EditingElement { get; private set; }

    /// <summary>
    /// Gets information about the user gesture that caused the cell to enter edit mode.
    /// </summary>
    public RoutedEventArgs EditingEventArgs { get; private set; }

    /// <summary>
    /// Gets the row that contains the cell to be edited.
    /// </summary>
    public DataGridRow Row { get; private set; }
}