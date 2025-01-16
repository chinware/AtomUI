// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace AtomUI.Controls.Collections;

public interface IDataGridEditableCollectionView : IDataGridCollectionView
{
    bool CanAddNew { get; }
    object AddNew();
    void CommitNew();
    void CancelNew();
    bool IsAddingNew { get; }
    object? CurrentAddItem { get; }
    bool CanRemove { get; }
    void RemoveAt(int index);
    void Remove(object item);
    void EditItem(object item);
    void CommitEdit();
    void CancelEdit();
    bool CanCancelEdit { get; }
    bool IsEditingItem { get; }
    object? CurrentEditItem { get; }
}