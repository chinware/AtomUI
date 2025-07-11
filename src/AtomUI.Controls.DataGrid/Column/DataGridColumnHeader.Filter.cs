// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using AtomUI.Controls.Data;
using Avalonia;

namespace AtomUI.Controls;

internal partial class DataGridColumnHeader
{
    internal static readonly DirectProperty<DataGridColumnHeader, bool> CanUserFilterProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnHeader, bool>(
            nameof(CanUserFilter),
            o => o.CanUserFilter,
            (o, v) => o.CanUserFilter = v);

    private bool _canUserFilter = false;

    internal bool CanUserFilter
    {
        get => _canUserFilter;
        set => SetAndRaise(CanUserFilterProperty, ref _canUserFilter, value);
    }

    private DataGridFilterIndicator? _filterIndicator;

    internal void InvokeProcessFilter(List<object> filterValues)
    {
        Debug.Assert(OwningGrid != null);
        if (OwningGrid.WaitForLostFocus(() => InvokeProcessFilter(filterValues)))
        {
            return;
        }

        if (OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => ProcessFilter(filterValues));
        }
    }

    internal void ProcessFilter(List<object> filterValues)
    {
        if (OwningColumn != null &&
            OwningGrid != null &&
            OwningGrid.EditingRow == null &&
            OwningColumn != OwningGrid.ColumnsInternal.FillerColumn &&
            (OwningColumn.CanUserFilter || OwningGrid.CanUserFilterColumns))
        {
            var ea = new DataGridColumnEventArgs(OwningColumn);
            OwningGrid.HandleColumnFiltering(ea);
            if (!ea.Handled && OwningGrid.DataConnection.AllowFilter &&
                OwningGrid.DataConnection.FilterDescriptions != null)
            {
                DataGrid                   owningGrid = OwningGrid;
                DataGridFilterDescription? filter         = OwningColumn.GetFilterDescription();
                IDataGridCollectionView?   collectionView = owningGrid.DataConnection.CollectionView;
                Debug.Assert(collectionView != null);
                using (collectionView.DeferRefresh())
                {
                    DataGridFilterDescription? newFilter;
                    if (owningGrid.DataConnection.FilterDescriptions.Count == 0)
                    {
                        owningGrid.DataConnection.FilterDescriptions.Clear();
                    }

                    if (filter != null)
                    {
                        // 比较一下值，如果过滤的值不相等就重新添加
                        var oldFilterValues = filter.FilterConditions.ToHashSet();
                        var newFilterValues = filterValues.ToHashSet();
                        if (newFilterValues.Count > 0 && !oldFilterValues.SetEquals(newFilterValues))
                        {
                            newFilter = new DataGridFilterDescription()
                            {
                                PropertyPath = filter.PropertyPath,
                                Filter =  filter.Filter,
                                FilterConditions = filterValues.ToList(),
                            };
                            int oldIndex = owningGrid.DataConnection.FilterDescriptions.IndexOf(filter);
                            if (oldIndex >= 0)
                            {
                                owningGrid.DataConnection.FilterDescriptions.Remove(filter);
                                owningGrid.DataConnection.FilterDescriptions.Insert(oldIndex, newFilter);
                            }
                            else
                            {
                                owningGrid.DataConnection.FilterDescriptions.Add(newFilter);
                            }
                        }
                        else if (newFilterValues.Count == 0)
                        {
                            owningGrid.DataConnection.FilterDescriptions.Remove(filter);
                        }
                    }
                    else if (filterValues.Count > 0)
                    {
                        string? propertyName = OwningColumn.GetFilterPropertyName();
                        if (string.IsNullOrEmpty(propertyName))
                        {
                            return;
                        }
                        newFilter = new DataGridFilterDescription()
                        {
                            PropertyPath = propertyName,
                            Filter       =  OwningColumn.OnFilter,
                            FilterConditions = filterValues,
                        };
                        owningGrid.DataConnection.FilterDescriptions.Add(newFilter);
                    }
                }
            }
        }
    }

    private void ConfigureFilterIndicator()
    {
        if (_filterIndicator != null)
        {
            _filterIndicator.FilterRequest += HandleFilterRequest;
        }
    }

    private void HandleFilterRequest(object? sender, DataGridColumnFilterEventArgs args)
    {
        InvokeProcessFilter(args.FilterValues.Cast<object>().ToList());
    }

    internal void InvokeClearFilter()
    {
        Debug.Assert(OwningGrid != null);
        if (OwningGrid.WaitForLostFocus(InvokeClearFilter))
        {
            return;
        }

        if (OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(ProcessClearFilter);
        }
    }

    internal void ProcessClearFilter()
    {
        if (OwningColumn != null &&
            OwningGrid != null &&
            OwningGrid.EditingRow == null &&
            OwningColumn != OwningGrid.ColumnsInternal.FillerColumn &&
            (OwningColumn.CanUserFilter || OwningGrid.CanUserFilterColumns))
        {
            var ea = new DataGridColumnEventArgs(OwningColumn);
            OwningGrid.HandleColumnFiltering(ea);
            if (!ea.Handled && OwningGrid.DataConnection.AllowFilter &&
                OwningGrid.DataConnection.FilterDescriptions != null)
            {
                DataGrid                   owningGrid     = OwningGrid;
                DataGridFilterDescription? filter         = OwningColumn.GetFilterDescription();
                IDataGridCollectionView?   collectionView = owningGrid.DataConnection.CollectionView;
                Debug.Assert(collectionView != null);

                using (collectionView.DeferRefresh())
                {
                    if (filter != null)
                    {
                        owningGrid.DataConnection.FilterDescriptions.Remove(filter);
                    }
                }
            }
        }
    }
}