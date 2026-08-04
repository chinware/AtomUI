namespace AtomUI.Desktop.Controls.Data;

/// <summary>
/// Provides optional item movement support for a DataGrid collection view.
/// </summary>
public interface IDataGridCollectionViewMoveSupport
{
    /// <summary>
    /// Gets whether the current view state supports moving items.
    /// </summary>
    bool CanMove { get; }

    /// <summary>
    /// Moves an item between two zero-based indexes in the current view.
    /// </summary>
    /// <param name="sourceIndex">The current view index of the item to move.</param>
    /// <param name="targetIndex">The target view index for the item.</param>
    /// <returns><see langword="true"/> when the item order changed; otherwise, <see langword="false"/>.</returns>
    bool TryMove(int sourceIndex, int targetIndex);
}
