// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics;
using AtomUI.Controls.Data;
using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Input;

namespace AtomUI.Controls;

internal partial class DataGridColumnHeader
{
    internal static readonly DirectProperty<DataGridColumnHeader, bool> CanUserSortProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, bool>(
            nameof(CanUserSort),
            o => o.CanUserSort,
            (o, v) => o.CanUserSort = v);
    
    internal static readonly DirectProperty<DataGridColumnHeader, ListSortDirection?> CurrentSortingStateProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, ListSortDirection?>(
            nameof(CurrentSortingState),
            o => o.CurrentSortingState,
            (o, v) => o.CurrentSortingState = v);
    
    internal static readonly StyledProperty<DataGridSortDirections> SupportedSortDirectionsProperty =
        DataGridColumn.SupportedSortDirectionsProperty.AddOwner<DataGridColumnHeader>();
    
    private bool _canUserSort = false;
    internal bool CanUserSort
    {
        get => _canUserSort;
        set => SetAndRaise(CanUserSortProperty, ref _canUserSort, value);
    }
    
    private ListSortDirection? _currentSortingState;
    internal ListSortDirection? CurrentSortingState
    {
        get => _currentSortingState;
        set => SetAndRaise(CurrentSortingStateProperty, ref _currentSortingState, value);
    }
    
    public DataGridSortDirections SupportedSortDirections
    {
        get => GetValue(SupportedSortDirectionsProperty);
        set => SetValue(SupportedSortDirectionsProperty, value);
    }
    
    internal void InvokeProcessSort(KeyModifiers keyModifiers, ListSortDirection? forcedDirection = null)
    {
        Debug.Assert(OwningGrid != null);
        if (OwningGrid.WaitForLostFocus(() => InvokeProcessSort(keyModifiers, forcedDirection)))
        {
            return;
        }
        if (OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => ProcessSort(keyModifiers, forcedDirection));
        }
    }
    
    //TODO GroupSorting
    internal void ProcessSort(KeyModifiers keyModifiers, ListSortDirection? forcedDirection = null)
    {
        // if we can sort:
        //  - AllowUserToSortColumns and CanSort are true, and
        //  - OwningColumn is bound
        // then try to sort
        if (OwningColumn != null &&
            OwningGrid != null &&
            OwningGrid.EditingRow == null &&
            OwningColumn != OwningGrid.ColumnsInternal.FillerColumn &&
            (OwningColumn.CanUserSort || OwningGrid.CanUserSortColumns))
        {
            
            var ea = new DataGridColumnEventArgs(OwningColumn);
            OwningGrid.HandleColumnSorting(ea);

            if (!ea.Handled && OwningGrid.DataConnection.AllowSort && OwningGrid.DataConnection.SortDescriptions != null)
            {
                // - DataConnection.AllowSort is true, and
                // - SortDescriptionsCollection exists, and
                // - the column's data type is comparable

                DataGrid                 owningGrid = OwningGrid;
                DataGridSortDescription? newSort;

                KeyboardHelper.GetMetaKeyState(this, keyModifiers, out bool ctrl, out bool shift);

                DataGridSortDescription? sort           = OwningColumn.GetSortDescription();
                IDataGridCollectionView? collectionView = owningGrid.DataConnection.CollectionView;
                Debug.Assert(collectionView != null);

                using (collectionView.DeferRefresh())
                {
                    // if shift is held down, we multi-sort, therefore if it isn't, we'll clear the sorts beforehand
                    if (!shift || owningGrid.DataConnection.SortDescriptions.Count == 0)
                    {
                        owningGrid.DataConnection.SortDescriptions.Clear();
                    }
                    // if ctrl is held down, we only clear the sort directions
                    if (!ctrl)
                    {
                        if (sort != null)
                        {
                            if (forcedDirection == null || sort.Direction != forcedDirection)
                            {
                                newSort = sort.SwitchSortDirection();
                            }
                            else
                            {
                                newSort = sort;
                            }

                            // changing direction should not affect sort order, so we replace this column's
                            // sort description instead of just adding it to the end of the collection
                            int oldIndex = owningGrid.DataConnection.SortDescriptions.IndexOf(sort);
                            if (oldIndex >= 0)
                            {
                                owningGrid.DataConnection.SortDescriptions.Remove(sort);
                                owningGrid.DataConnection.SortDescriptions.Insert(oldIndex, newSort);
                            }
                            else
                            {
                                owningGrid.DataConnection.SortDescriptions.Add(newSort);
                            }
                        }
                        else if (OwningColumn.CustomSortComparer != null)
                        {
                            newSort = forcedDirection != null ?
                                DataGridSortDescription.FromComparer(OwningColumn.CustomSortComparer, forcedDirection.Value) :
                                DataGridSortDescription.FromComparer(OwningColumn.CustomSortComparer);
                            owningGrid.DataConnection.SortDescriptions.Add(newSort);
                        }
                        else
                        {
                            string? propertyName = OwningColumn.GetSortPropertyName();
                            // no-opt if we couldn't find a property to sort on
                            if (string.IsNullOrEmpty(propertyName))
                            {
                                return;
                            }

                            newSort = DataGridSortDescription.FromPath(propertyName, culture: collectionView.Culture);
                            if (forcedDirection != null && newSort.Direction != forcedDirection)
                            {
                                newSort = newSort.SwitchSortDirection();
                            }
                            owningGrid.DataConnection.SortDescriptions.Add(newSort);
                        }
                    }
                }
            }
        }
    }
}