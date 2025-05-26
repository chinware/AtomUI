namespace AtomUI.Controls;

/// <summary>
/// Provides information just after a row has exited edit mode.
/// </summary>
public class DataGridRowEditEndedEventArgs : EventArgs
{
    /// <summary>
    /// Instantiates a new instance of this class.
    /// </summary>
    /// <param name="row">The row container of the cell container that has just exited edit mode.</param>
    /// <param name="editAction">The editing action that has been taken.</param>
    public DataGridRowEditEndedEventArgs(DataGridRow row, DataGridEditAction editAction)
    {
        Row        = row;
        EditAction = editAction;
    }

    /// <summary>
    /// The editing action that has been taken.
    /// </summary>
    public DataGridEditAction EditAction { get; private set; }

    /// <summary>
    /// The row container of the cell container that has just exited edit mode.
    /// </summary>
    public DataGridRow Row { get; private set; }
}