// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using AtomUI.Controls.Utils;

namespace AtomUI.Controls.Data;

internal class DataGridDataConnection
{
    private int _backupSlotForCurrentChanged;
    private int _columnForCurrentChanged;
    private PropertyInfo[] _dataProperties;
    private IEnumerable? _dataSource;
    private Type? _dataType;
    private bool _expectingCurrentChanged;
    private object? _itemToSelectOnCurrentChanged;
    private DataGrid _owner;
    private bool _scrollForCurrentChanged;
    private DataGridSelectionAction? _selectionActionForCurrentChanged;
    
    public DataGridDataConnection(DataGrid owner)
    {
        _owner          = owner;
        _dataProperties = [];
    }
    
    public IEnumerable? DataSource
    {
        get => _dataSource;
        set
        {
            _dataSource = value;
            // Because the DataSource is changing, we need to reset our cached values for DataType and DataProperties,
            // which are dependent on the current DataSource
            _dataType = null;
            UpdateDataProperties();
        }
    }
    
    public Type? DataType
    {
        get
        {
            // We need to use the raw ItemsSource as opposed to DataSource because DataSource
            // may be the ItemsSource wrapped in a collection view, in which case we wouldn't
            // be able to take T to be the type if we're given IEnumerable<T>
            if (_dataType == null && _owner.ItemsSource != null)
            {
                _dataType = _owner.ItemsSource.GetItemType();
            }
            return _dataType;
        }
    }
    
    public bool EventsWired
    {
        get;
        private set;
    }

    private bool IsGrouping => (CollectionView != null)
                               && CollectionView.CanGroup
                               && CollectionView.IsGrouping
                               && (CollectionView.GroupingDepth > 0);

    
    public IList? List => DataSource as IList;
    
    public bool AllowEdit
    {
        get
        {
            if (List == null)
            {
                return true;
            }
            return !List.IsReadOnly;
        }
    }
    
    /// <summary>
    /// True if the collection view says it can sort.
    /// </summary>
    public bool AllowSort
    {
        get
        {
            if (CollectionView == null ||
                (EditableCollectionView != null && (EditableCollectionView.IsAddingNew || EditableCollectionView.IsEditingItem)))
            {
                return false;
            }
            return CollectionView.CanSort;
        }
    }
    
    public bool CommittingEdit { get; private set; }
    
    public int Count => TryGetCount(true, false, out var count) ? count : 0;
    
    public bool DataIsPrimitive => DataTypeIsPrimitive(DataType);
    
    internal static bool DataTypeIsPrimitive(Type? dataType)
    {
        if (dataType != null)
        {
            Type type = TypeHelper.GetNonNullableType(dataType);  // no-opt if dataType isn't nullable
            return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(Decimal);
        }
        return false;
    }
    
    public PropertyInfo[] DataProperties
    {
        get
        {
            UpdateDataProperties();
            return _dataProperties;
        }
    }

    public bool ShouldAutoGenerateColumns => false;

    public IDataGridCollectionView? CollectionView => DataSource as IDataGridCollectionView;
    
    public IDataGridEditableCollectionView? EditableCollectionView => DataSource as IDataGridEditableCollectionView;
    
    public DataGridSortDescriptionCollection? SortDescriptions
    {
        get
        {
            if (CollectionView != null && CollectionView.CanSort)
            {
                return CollectionView.SortDescriptions;
            }
            return null;
        }
    }
    
    /// <summary>Try get number of DataSource items.</summary>
    /// <param name="allowSlow">When "allowSlow" is false, method will not use Linq.Count() method and will return 0 or 1 instead.</param>
    /// <param name="getAny">If "getAny" is true, method can use Linq.Any() method to speedup.</param>
    /// <param name="count">number of DataSource items.</param>
    /// <returns>true if able to retrieve number of DataSource items; otherwise, false.</returns>
    internal bool TryGetCount(bool allowSlow, bool getAny, out int count)
    {
        bool result;
        (result, count) = DataSource switch
        {
            ICollection collection => (true, collection.Count),
            IEnumerable enumerable when allowSlow && !getAny => (true, enumerable.Cast<object>().Count()),
            IEnumerable enumerable when getAny => (true, enumerable.Cast<object>().Any() ? 1 : 0),
            _ => (false, 0)
        };
        return result;
    }
    
    internal bool Any()
    {
        return TryGetCount(false, true, out var count) && count > 0;
    }
    
    /// <summary>
    /// Puts the entity into editing mode if possible
    /// </summary>
    /// <param name="dataItem">The entity to edit</param>
    /// <returns>True if editing was started</returns>
    public bool BeginEdit(object? dataItem)
    {
        if (dataItem == null)
        {
            return false;
        }

        IDataGridEditableCollectionView? editableCollectionView = EditableCollectionView;
        if (editableCollectionView != null)
        {
            if (editableCollectionView.IsEditingItem && (dataItem == editableCollectionView.CurrentEditItem))
            {
                return true;
            }
            editableCollectionView.EditItem(dataItem);
            return editableCollectionView.IsEditingItem || editableCollectionView.IsAddingNew;
        }

        if (dataItem is IEditableObject editableDataItem)
        {
            editableDataItem.BeginEdit();
        }

        return true;
    }
    
    /// <summary>
    /// Cancels the current entity editing and exits the editing mode.
    /// </summary>
    /// <param name="dataItem">The entity being edited</param>
    /// <returns>True if a cancellation operation was invoked.</returns>
    public bool CancelEdit(object? dataItem)
    {
        IDataGridEditableCollectionView? editableCollectionView = EditableCollectionView;
        if (editableCollectionView != null)
        {
            if (editableCollectionView.CanCancelEdit)
            {
                editableCollectionView.CancelEdit();
                return true;
            }
            return false;
        }

        if (dataItem is IEditableObject editableDataItem)
        {
            editableDataItem.CancelEdit();
        }

        return true;
    }
    
    public static bool CanEdit(Type type)
    {
        Debug.Assert(type != null);

        type = type.GetNonNullableType();

        return
            type.IsEnum
            || type == typeof(String)
            || type == typeof(Char)
            || type == typeof(DateTime)
            || type == typeof(Boolean)
            || type == typeof(Byte)
            || type == typeof(SByte)
            || type == typeof(Single)
            || type == typeof(Double)
            || type == typeof(Decimal)
            || type == typeof(Int16)
            || type == typeof(Int32)
            || type == typeof(Int64)
            || type == typeof(UInt16)
            || type == typeof(UInt32)
            || type == typeof(UInt64);
    }
    
    /// <summary>
    /// Commits the current entity editing and exits the editing mode.
    /// </summary>
    /// <param name="dataItem">The entity being edited</param>
    /// <returns>True if a commit operation was invoked.</returns>
    public bool EndEdit(object? dataItem)
    {
        IDataGridEditableCollectionView? editableCollectionView = EditableCollectionView;
        if (editableCollectionView != null)
        {
            // IEditableCollectionView.CommitEdit can potentially change currency. If it does,
            // we don't want to attempt a second commit inside our CurrentChanging event handler.
            _owner.NoCurrentCellChangeCount++;
            CommittingEdit = true;
            try
            {
                if (editableCollectionView.IsAddingNew)
                {
                    editableCollectionView.CommitNew();
                }
                else
                {
                    editableCollectionView.CommitEdit();
                }                    
            }
            finally
            {
                _owner.NoCurrentCellChangeCount--;
                CommittingEdit = false;
            }
            return true;
        }

        if (dataItem is IEditableObject editableDataItem)
        {
            editableDataItem.EndEdit();
        }

        return true;
    }
    
    private void UpdateDataProperties()
    {
        Type? dataType = DataType;

        if (DataSource != null && dataType != null && !DataTypeIsPrimitive(dataType))
        {
            _dataProperties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Debug.Assert(_dataProperties != null);
        }
        else
        {
            _dataProperties = [];
        }
    }
    
    public bool GetPropertyIsReadOnly(string propertyName)
    {
        if (DataType != null)
        {
            if (!String.IsNullOrEmpty(propertyName))
            {
                Type          propertyType  = DataType;
                PropertyInfo? propertyInfo  = null;
                List<string>  propertyNames = TypeHelper.SplitPropertyPath(propertyName);
                for (int i = 0; i < propertyNames.Count; i++)
                {
                    propertyInfo = propertyType.GetPropertyOrIndexer(propertyNames[i], out _);
                    if (propertyInfo == null || propertyType.GetIsReadOnly() || propertyInfo.GetIsReadOnly())
                    {
                        // Either the data type is read-only, the property doesn't exist, or it does exist but is read-only
                        return true;
                    }

                    // Check if EditableAttribute is defined on the property and if it indicates uneditable
                    var attributes = propertyInfo.GetCustomAttributes(typeof(EditableAttribute), true);
                    if (attributes.Length > 0)
                    {
                        var editableAttribute = (EditableAttribute)attributes[0];
                        if (!editableAttribute.AllowEdit)
                        {
                            return true;
                        }
                    }
                    propertyType = propertyInfo.PropertyType.GetNonNullableType();
                }
                return propertyInfo == null || !propertyInfo.CanWrite || !AllowEdit || !CanEdit(propertyType);
            }
            if (DataType.GetIsReadOnly())
            {
                return true;
            }
        }
        return !AllowEdit;
    }

    public int IndexOf(object? dataItem)
    {
        if (DataSource is DataGridCollectionView cv)
        {
            return cv.IndexOf(dataItem);
        }

        IList? list = List;
        if (list != null)
        {
            return list.IndexOf(dataItem);
        }

        IEnumerable? enumerable = DataSource;
        if (enumerable != null && dataItem != null)
        {
            int index = 0;
            foreach (var dataItemTmp in enumerable)
            {
                if (dataItemTmp == null || dataItem.Equals(dataItemTmp))
                {
                    return index;
                }
                index++;
            }
        }
        return -1;
    }

    internal void ClearDataProperties()
    {
        _dataProperties = [];
    }

    /// <summary>
    /// Creates a collection view around the DataGrid's source. ICollectionViewFactory is
    /// used if the source implements it. Otherwise a PagedCollectionView is returned.
    /// </summary>
    /// <param name="source">Enumerable source for which to create a view</param>
    /// <returns>ICollectionView view over the provided source</returns>
    internal static IDataGridCollectionView CreateView(IEnumerable source)
    {
        Debug.Assert(source != null, "source unexpectedly null");
        Debug.Assert(!(source is IDataGridCollectionView), "source is an ICollectionView");

        IDataGridCollectionView? collectionView = null;

        if (source is IDataGridCollectionViewFactory collectionViewFactory)
        {
            // If the source is a collection view factory, give it a chance to produce a custom collection view.
            collectionView = collectionViewFactory.CreateView();
            // Intentionally not catching potential exception thrown by ICollectionViewFactory.CreateView().
        }
        if (collectionView == null)
        {
            // If we still do not have a collection view, default to a PagedCollectionView.
            collectionView = new DataGridCollectionView(source);
        }
        return collectionView;
    }
    
    internal void MoveCurrentTo(object item, int backupSlot, int columnIndex, DataGridSelectionAction action, bool scrollIntoView)
    {
        if (CollectionView != null)
        {
            _expectingCurrentChanged          = true;
            _columnForCurrentChanged          = columnIndex;
            _itemToSelectOnCurrentChanged     = item;
            _selectionActionForCurrentChanged = action;
            _scrollForCurrentChanged          = scrollIntoView;
            _backupSlotForCurrentChanged      = backupSlot;

            CollectionView.MoveCurrentTo(item is DataGridCollectionViewGroup ? null : item);

            _expectingCurrentChanged = false;
        }
    }

    internal void UnWireEvents(IEnumerable value)
    {
        if (value is INotifyCollectionChanged notifyingDataSource)
        {
            notifyingDataSource.CollectionChanged -= HandleDataSourceCollectionChanged;
        }

        if (SortDescriptions != null)
        {
            SortDescriptions.CollectionChanged -= HandleSortDescriptionsCollectionChanged;
        }

        if (CollectionView != null)
        {
            CollectionView.CurrentChanged  -= HandleCurrentChanged;
            CollectionView.CurrentChanging -= HandleCurrentChanging;
        }

        EventsWired = false;
    }
    
    internal void WireEvents(IEnumerable value)
    {
        if (value is INotifyCollectionChanged notifyingDataSource)
        {
            notifyingDataSource.CollectionChanged += HandleDataSourceCollectionChanged;
        }

        if (SortDescriptions != null)
        {
            SortDescriptions.CollectionChanged += HandleSortDescriptionsCollectionChanged;
        }

        if (CollectionView != null)
        {
            CollectionView.CurrentChanged  += HandleCurrentChanged;
            CollectionView.CurrentChanging += HandleCurrentChanging;
        }

        EventsWired = true;
    }

    private void HandleCurrentChanged(object? sender, EventArgs e)
    {
        // if (_expectingCurrentChanged)
        // {
        //     // Committing Edit could cause our item to move to a group that no longer exists.  In
        //     // this case, we need to update the item.
        //     if (_itemToSelectOnCurrentChanged is DataGridCollectionViewGroup collectionViewGroup)
        //     {
        //         DataGridRowGroupInfo groupInfo = _owner.RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
        //         if (groupInfo == null)
        //         {
        //             // Move to the next slot if the target slot isn't visible                        
        //             if (!_owner.IsSlotVisible(_backupSlotForCurrentChanged))
        //             {
        //                 _backupSlotForCurrentChanged = _owner.GetNextVisibleSlot(_backupSlotForCurrentChanged);
        //             }
        //             // Move to the next best slot if we've moved past all the slots.  This could happen if multiple
        //             // groups were removed.
        //             if (_backupSlotForCurrentChanged >= _owner.SlotCount)
        //             {
        //                 _backupSlotForCurrentChanged = _owner.GetPreviousVisibleSlot(_owner.SlotCount);
        //             }
        //             // Update the itemToSelect
        //             int newCurrentPosition = -1;
        //             _itemToSelectOnCurrentChanged = _owner.ItemFromSlot(_backupSlotForCurrentChanged, ref newCurrentPosition);
        //         }
        //     }
        //
        //     _owner.ProcessSelectionAndCurrency(
        //         _columnForCurrentChanged,
        //         _itemToSelectOnCurrentChanged,
        //         _backupSlotForCurrentChanged,
        //         _selectionActionForCurrentChanged,
        //         _scrollForCurrentChanged);
        // }
        // else if (CollectionView != null)
        // {
        //     _owner.UpdateStateOnCurrentChanged(CollectionView.CurrentItem, CollectionView.CurrentPosition);
        // }
    }

    private void HandleCurrentChanging(object? sender, DataGridCurrentChangingEventArgs e)
    {
        // if (_owner.NoCurrentCellChangeCount == 0 &&
        //     !_expectingCurrentChanged &&
        //     !CommittingEdit &&
        //     !_owner.CommitEdit())
        // {
        //     // If CommitEdit failed, then the user has most likely input invalid data.
        //     // We should cancel the current change if we can, otherwise we have to abort the edit.
        //     if (e.IsCancelable)
        //     {
        //         e.Cancel = true;
        //     }
        //     else
        //     {
        //         _owner.CancelEdit(DataGridEditingUnit.Row, false);
        //     }
        // }
    }

    private void HandleSortDescriptionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // if (_owner.ColumnsItemsInternal.Count == 0)
        // {
        //     return;
        // }
        //
        // // refresh sort description
        // foreach (DataGridColumn column in _owner.ColumnsItemsInternal)
        // {
        //     column.HeaderCell.UpdatePseudoClasses();
        // }
    }

    private void HandleDataSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // if (_owner.LoadingOrUnloadingRow)
        // {
        //     throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
        // }
        // switch (e.Action)
        // {
        //     case NotifyCollectionChangedAction.Add:
        //         Debug.Assert(e.NewItems != null, "Unexpected NotifyCollectionChangedAction.Add notification");
        //         if (ShouldAutoGenerateColumns)
        //         {
        //             // The columns are also affected (not just rows) in this case so we need to reset everything
        //             _owner.InitializeElements(false /*recycleRows*/);
        //         }
        //         else if (!IsGrouping)
        //         {
        //             // If we're grouping then we handle this through the CollectionViewGroup notifications
        //             // According to WPF, Add is a single item operation
        //             Debug.Assert(e.NewItems.Count == 1);
        //             _owner.InsertRowAt(e.NewStartingIndex);
        //         }
        //         break;
        //     case NotifyCollectionChangedAction.Remove:
        //         IList removedItems = e.OldItems;
        //         if (removedItems == null || e.OldStartingIndex < 0)
        //         {
        //             Debug.Assert(false, "Unexpected NotifyCollectionChangedAction.Remove notification");
        //             return;
        //         }
        //         if (!IsGrouping)
        //         {
        //             // If we're grouping then we handle this through the CollectionViewGroup notifications
        //             // According to WPF, Remove is a single item operation
        //             foreach (object item in e.OldItems)
        //             {
        //                 Debug.Assert(item != null);
        //                 _owner.RemoveRowAt(e.OldStartingIndex, item);
        //             }
        //         }
        //         break;
        //     case NotifyCollectionChangedAction.Replace:
        //         throw new NotSupportedException(); // 
        //
        //     case NotifyCollectionChangedAction.Reset:
        //         // Did the data type change during the reset?  If not, we can recycle
        //         // the existing rows instead of having to clear them all.  We still need to clear our cached
        //         // values for DataType and DataProperties, though, because the collection has been reset.
        //         Type previousDataType = _dataType;
        //         _dataType = null;
        //         if (previousDataType != DataType)
        //         {
        //             ClearDataProperties();
        //             _owner.InitializeElements(false /*recycleRows*/);
        //         }
        //         else
        //         {
        //             _owner.InitializeElements(!ShouldAutoGenerateColumns /*recycleRows*/);
        //         }
        //         break;
        // }
        //
        // _owner.UpdatePseudoClasses();
    }

}