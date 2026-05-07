// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace AtomUI.Desktop.Controls.Data;

internal interface IDataGridEditableCollectionView
{
    /// <summary>Gets a value that indicates whether a new item can be added to the collection.</summary>
    /// <returns>true if a new item can be added to the collection; otherwise, false.</returns>
    bool CanAddNew { get; }

    /// <summary>Adds a new item to the underlying collection.</summary>
    /// <returns>The new item that is added to the collection.</returns>
    object AddNew();

    /// <summary>Ends the add transaction and saves the pending new item.</summary>
    void CommitNew();

    /// <summary>Ends the add transaction and discards the pending new item.</summary>
    void CancelNew();

    /// <summary>Gets a value that indicates whether an add transaction is in progress.</summary>
    /// <returns>true if an add transaction is in progress; otherwise, false.</returns>
    bool IsAddingNew { get; }

    /// <summary>Gets the item that is being added during the current add transaction.</summary>
    /// <returns>The item that is being added if <see cref="P:System.ComponentModel.IEditableCollectionView.IsAddingNew" /> is true; otherwise, null.</returns>
    object? CurrentAddItem { get; }

    /// <summary>Gets a value that indicates whether an item can be removed from the collection.</summary>
    /// <returns>true if an item can be removed from the collection; otherwise, false.</returns>
    bool CanRemove { get; }

    /// <summary>Removes the item at the specified position from the collection.</summary>
    /// <param name="index">Index of item to remove.</param>
    void RemoveAt(int index);

    /// <summary>Removes the specified item from the collection.</summary>
    /// <param name="item">The item to remove.</param>
    void Remove(object item);

    /// <summary>Begins an edit transaction on the specified item.</summary>
    /// <param name="item">The item to edit.</param>
    void EditItem(object item);

    /// <summary>Ends the edit transaction and saves the pending changes.</summary>
    void CommitEdit();

    /// <summary>Ends the edit transaction and, if possible, restores the original value of the item.</summary>
    void CancelEdit();

    /// <summary>Gets a value that indicates whether the collection view can discard pending changes and restore the original values of an edited object.</summary>
    /// <returns>true if the collection view can discard pending changes and restore the original values of an edited object; otherwise, false.</returns>
    bool CanCancelEdit { get; }

    /// <summary>Gets a value that indicates whether an edit transaction is in progress.</summary>
    /// <returns>true if an edit transaction is in progress; otherwise, false.</returns>
    bool IsEditingItem { get; }

    /// <summary>Gets the item in the collection that is being edited.</summary>
    /// <returns>The item that is being edited if <see cref="P:System.ComponentModel.IEditableCollectionView.IsEditingItem" /> is true; otherwise, null.</returns>
    object? CurrentEditItem { get; }
}