using System.ComponentModel;

namespace AtomUI.Controls;

/// <summary>
/// Provides information just before a row exits editing mode.
/// </summary>
public class DataGridRowEditEndingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Instantiates a new instance of this class.
    /// </summary>
    /// <param name="row">The row container of the cell container that is about to exit edit mode.</param>
    /// <param name="editAction">The editing action that will be taken.</param>
    public DataGridRowEditEndingEventArgs(DataGridRow row, DataGridEditAction editAction)
    {
        this.Row        = row;
        this.EditAction = editAction;
    }

    /// <summary>
    /// The editing action that will be taken.
    /// </summary>
    public DataGridEditAction EditAction { get; private set; }

    /// <summary>
    /// The row container of the cell container that is about to exit edit mode.
    /// </summary>
    public DataGridRow Row { get; private set; }
}