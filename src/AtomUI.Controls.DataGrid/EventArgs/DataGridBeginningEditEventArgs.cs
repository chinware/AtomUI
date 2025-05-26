using System.ComponentModel;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

/// <summary>
/// Provides data for the <see cref="E:AtomUI.Controls.DataGrid.BeginningEdit" /> event.
/// </summary>
public class DataGridBeginningEditEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="T:AtomUI.Controls.DataGridBeginningEditEventArgs" /> class.
    /// </summary>
    /// <param name="column">
    /// The column that contains the cell to be edited.
    /// </param>
    /// <param name="row">
    /// The row that contains the cell to be edited.
    /// </param>
    /// <param name="editingEventArgs">
    /// Information about the user gesture that caused the cell to enter edit mode.
    /// </param>
    public DataGridBeginningEditEventArgs(DataGridColumn column,
                                          DataGridRow row,
                                          RoutedEventArgs? editingEventArgs)
    {
        Column           = column;
        Row              = row;
        EditingEventArgs = editingEventArgs;
    }

    /// <summary>
    /// Gets the column that contains the cell to be edited.
    /// </summary>
    public DataGridColumn Column { get; private set; }

    /// <summary>
    /// Gets information about the user gesture that caused the cell to enter edit mode.
    /// </summary>
    public RoutedEventArgs? EditingEventArgs { get; private set; }

    /// <summary>
    /// Gets the row that contains the cell to be edited.
    /// </summary>
    public DataGridRow Row { get; private set; }
}