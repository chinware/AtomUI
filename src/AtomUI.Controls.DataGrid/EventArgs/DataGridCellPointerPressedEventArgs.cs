using Avalonia.Input;

namespace AtomUI.Controls;

/// <summary>
/// Provides information after the cell has been pressed.
/// </summary>
public class DataGridCellPointerPressedEventArgs : EventArgs
{
    /// <summary>
    /// Instantiates a new instance of this class.
    /// </summary>
    /// <param name="cell">The cell that has been pressed.</param>
    /// <param name="row">The row container of the cell that has been pressed.</param>
    /// <param name="column">The column of the cell that has been pressed.</param>
    /// <param name="e">The pointer action that has been taken.</param>
    public DataGridCellPointerPressedEventArgs(DataGridCell cell,
                                               DataGridRow row,
                                               DataGridColumn column,
                                               PointerPressedEventArgs e)
    {
        Cell                    = cell;
        Row                     = row;
        Column                  = column;
        PointerPressedEventArgs = e;
    }

    /// <summary>
    /// The cell that has been pressed.
    /// </summary> 
    public DataGridCell Cell { get; }

    /// <summary>
    /// The row container of the cell that has been pressed.
    /// </summary> 
    public DataGridRow Row { get; }

    /// <summary>
    /// The column of the cell that has been pressed.
    /// </summary> 
    public DataGridColumn Column { get; }

    /// <summary>
    /// The pointer action that has been taken.
    /// </summary> 
    public PointerPressedEventArgs PointerPressedEventArgs { get; }
}