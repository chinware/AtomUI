// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Diagnostics;
using AtomUI.Controls.Data;
using AtomUI.Controls.DataGridLang;
using AtomUI.Controls.Utils;
using AtomUI.Theme.Data;
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

    internal static readonly DirectProperty<DataGridColumnHeader, bool> ShowSorterTooltipProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, bool>(
            nameof(ShowSorterTooltip),
            o => o.ShowSorterTooltip,
            (o, v) => o.ShowSorterTooltip = v);

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

    private bool _showSorterTooltip;

    internal bool ShowSorterTooltip
    {
        get => _showSorterTooltip;
        set => SetAndRaise(ShowSorterTooltipProperty, ref _showSorterTooltip, value);
    }

    private IDisposable? _showSorterTooltipDisposable;

    private void NotifyPropertyChangedForSorting(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ShowSorterTooltipProperty || change.Property == CurrentSortingStateProperty)
        {
            ConfigureShowSorterTooltip();
        }
    }

    private void ConfigureShowSorterTooltip()
    {
        if (!ShowSorterTooltip || SupportedSortDirections == DataGridSortDirections.None)
        {
            return;
        }

        ListSortDirection? nextDirection = null;
        if (CurrentSortingState != null)
        {
            if ((SupportedSortDirections & DataGridSortDirections.All) == DataGridSortDirections.All)
            {
                if (CurrentSortingState == ListSortDirection.Ascending)
                {
                    nextDirection = ListSortDirection.Descending;
                }
                else if (CurrentSortingState == ListSortDirection.Descending)
                {
                    nextDirection = null;
                }
            }
            else if ((SupportedSortDirections & DataGridSortDirections.Ascending) == DataGridSortDirections.Ascending)
            {
                nextDirection = null;
            }
            else if ((SupportedSortDirections & DataGridSortDirections.Descending) ==
                     DataGridSortDirections.Descending)
            {
                nextDirection = null;
            }
        }
        else
        {
            if ((SupportedSortDirections & DataGridSortDirections.All) == DataGridSortDirections.All)
            {
                nextDirection = ListSortDirection.Ascending;
            }
            else if ((SupportedSortDirections & DataGridSortDirections.Ascending) == DataGridSortDirections.Ascending)
            {
                nextDirection = ListSortDirection.Ascending;
            }
            else if ((SupportedSortDirections & DataGridSortDirections.Descending) ==
                     DataGridSortDirections.Descending)
            {
                nextDirection = ListSortDirection.Descending;
            }
        }

        _showSorterTooltipDisposable?.Dispose();
        if (nextDirection == null)
        {
            _showSorterTooltipDisposable =
                LanguageResourceBinder.CreateBinding(this, ToolTip.TipProperty, DataGridLangResourceKey.CancelTooltip);
        }
        else if (nextDirection == ListSortDirection.Ascending)
        {
            _showSorterTooltipDisposable =
                LanguageResourceBinder.CreateBinding(this, ToolTip.TipProperty, DataGridLangResourceKey.AscendTooltip);
        }
        else if (nextDirection == ListSortDirection.Descending)
        {
            _showSorterTooltipDisposable =
                LanguageResourceBinder.CreateBinding(this, ToolTip.TipProperty, DataGridLangResourceKey.DescendTooltip);
        }
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        if (CanUserSort)
        {
            ToolTip.SetIsOpen(this, true);
        }
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        if (CanUserSort)
        {
            ToolTip.SetIsOpen(this, false);
        }
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
            OwningGrid.NotifyColumnSorting(ea);
            if (!ea.Handled && OwningGrid.DataConnection.AllowSort &&
                OwningGrid.DataConnection.SortDescriptions != null)
            {
                // - DataConnection.AllowSort is true, and
                // - SortDescriptionsCollection exists, and
                // - the column's data type is comparable

                DataGrid                 owningGrid = OwningGrid;
                DataGridSortDescription? newSort    = null;

                KeyboardHelper.GetMetaKeyState(this, keyModifiers, out bool ctrl, out bool shift);
                DataGridSortDescription? sort           = OwningColumn.GetSortDescription();
                IDataGridCollectionView? collectionView = owningGrid.DataConnection.CollectionView;
                Debug.Assert(collectionView != null);
                var supportedSortDirections = OwningColumn.SupportedSortDirections;

                using (collectionView.DeferRefresh())
                {
                    // 获取下一个方向
                    ListSortDirection? nextDirection = null;
                    if (!shift || owningGrid.DataConnection.SortDescriptions.Count == 0)
                    {
                        owningGrid.DataConnection.SortDescriptions.Clear();
                    }

                    if (forcedDirection != null)
                    {
                        nextDirection = forcedDirection.Value;
                    }
                    else
                    {
                        if (sort != null)
                        {
                            var currentDirection = sort.Direction;
                            if ((supportedSortDirections & DataGridSortDirections.All) == DataGridSortDirections.All)
                            {
                                if (currentDirection == ListSortDirection.Ascending)
                                {
                                    nextDirection = ListSortDirection.Descending;
                                }
                                else if (currentDirection == ListSortDirection.Descending)
                                {
                                    nextDirection = null;
                                }
                            }
                            else if ((supportedSortDirections & DataGridSortDirections.Ascending) ==
                                     DataGridSortDirections.Ascending)
                            {
                                nextDirection = null;
                            }
                            else if ((supportedSortDirections & DataGridSortDirections.Descending) ==
                                     DataGridSortDirections.Descending)
                            {
                                nextDirection = null;
                            }
                        }
                        else
                        {
                            if ((supportedSortDirections & DataGridSortDirections.All) == DataGridSortDirections.All)
                            {
                                nextDirection = ListSortDirection.Ascending;
                            }
                            else if ((supportedSortDirections & DataGridSortDirections.Ascending) ==
                                     DataGridSortDirections.Ascending)
                            {
                                nextDirection = ListSortDirection.Ascending;
                            }
                            else if ((supportedSortDirections & DataGridSortDirections.Descending) ==
                                     DataGridSortDirections.Descending)
                            {
                                nextDirection = ListSortDirection.Descending;
                            }
                        }
                    }
                    
                    if (nextDirection != null)
                    {
                        newSort = BuildSortDescription(nextDirection);
                    }

                    if (sort != null && newSort != null)
                    {
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
                    else if (newSort != null)
                    {
                        owningGrid.DataConnection.SortDescriptions.Add(newSort);
                    }
                    else if (sort != null)
                    {
                        owningGrid.DataConnection.SortDescriptions.Remove(sort);
                    }
                }
            }
        }
    }

    private DataGridSortDescription? BuildSortDescription(ListSortDirection? direction)
    {
        Debug.Assert(OwningGrid != null);
        Debug.Assert(OwningColumn != null);
        DataGrid                 owningGrid     = OwningGrid;
        IDataGridCollectionView? collectionView = owningGrid.DataConnection.CollectionView;
        Debug.Assert(collectionView != null);
        if (OwningColumn.CustomSortComparer != null)
        {
            return direction != null
                ? DataGridSortDescription.FromComparer(OwningColumn.CustomSortComparer, direction.Value)
                : DataGridSortDescription.FromComparer(OwningColumn.CustomSortComparer);
        }

        string? propertyName = OwningColumn.GetSortPropertyName();
        // no-opt if we couldn't find a property to sort on
        if (string.IsNullOrEmpty(propertyName))
        {
            return null;
        }

        var newSort = DataGridSortDescription.FromPath(propertyName, culture: collectionView.Culture);
        if (direction != null && newSort.Direction != direction)
        {
            newSort = newSort.SwitchSortDirection();
        }

        return newSort;
    }

    internal void InvokeClearSort()
    {
        Debug.Assert(OwningGrid != null);
        if (OwningGrid.WaitForLostFocus(InvokeClearSort))
        {
            return;
        }

        if (OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(ProcessClearSort);
        }
    }

    internal void ProcessClearSort()
    {
        if (OwningColumn != null &&
            OwningGrid != null &&
            OwningGrid.EditingRow == null &&
            OwningColumn != OwningGrid.ColumnsInternal.FillerColumn &&
            (OwningColumn.CanUserSort || OwningGrid.CanUserSortColumns))
        {
            var ea = new DataGridColumnEventArgs(OwningColumn);
            OwningGrid.NotifyColumnSorting(ea);
            if (!ea.Handled && OwningGrid.DataConnection.AllowSort &&
                OwningGrid.DataConnection.SortDescriptions != null)
            {
                DataGrid                 owningGrid     = OwningGrid;
                DataGridSortDescription? sort           = OwningColumn.GetSortDescription();
                IDataGridCollectionView? collectionView = owningGrid.DataConnection.CollectionView;
                Debug.Assert(collectionView != null);

                using (collectionView.DeferRefresh())
                {
                    if (sort != null)
                    {
                        owningGrid.DataConnection.SortDescriptions.Remove(sort);
                    }
                }
            }
        }
    }
}