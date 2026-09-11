// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using AtomUI.Data;
using Avalonia;

namespace AtomUI.Desktop.Controls;

internal partial class DataGridColumnHeader
{
    private static readonly List<object> EmptyFilterValues = new(0);

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
            ProcessFilter(filterValues);
        }
    }

    internal void ProcessFilter(List<object> filterValues)
    {
        if (OwningGrid is not null)
        {
            if (OwningColumn is null || !OwningColumn.EffectiveCanUserFilter)
            {
                return;
            }
            var eventArgs = new DataGridColumnEventArgs(OwningColumn);
            OwningGrid.NotifyColumnFiltering(eventArgs);
            if (!eventArgs.Handled)
            {
                OwningGrid.ApplyFilterGesture(OwningColumn, filterValues);
            }
        }
    }

    private void ConfigureFilterIndicator()
    {
        if (_filterIndicator != null)
        {
            _filterIndicator.FilterRequest += HandleFilterRequest;
            _popupPinnedOpenRelay = BindUtils.RelayBind(
                this,
                IsPopupPinnedOpenProperty,
                _filterIndicator,
                DataGridFilterIndicator.IsPopupPinnedOpenProperty);
        }
    }

    internal void CloseFilterPopupForLifecycle()
    {
        _filterIndicator?.ClosePopupForLifecycle();
    }

    internal void NotifyFilterConfigurationChanged()
    {
        if (_filterIndicator is null || OwningColumn is null)
        {
            return;
        }

        _filterIndicator.FilterPresenterMode = OwningColumn.FilterPresenterMode;
        _filterIndicator.IsMultipleSelectionEnabled =
            OwningColumn.FilterSelectionMode == DataGridFilterSelectionMode.Multiple;
    }

    private void HandleFilterRequest(object? sender, DataGridColumnFilterEventArgs args)
    {
        InvokeProcessFilter(CopyFilterValues(args.FilterValues));
    }

    private static List<object> CopyFilterValues(List<string> filterValues)
    {
        if (filterValues.Count == 0)
        {
            return EmptyFilterValues;
        }

        var values = new List<object>(filterValues.Count);
        foreach (var filterValue in filterValues)
        {
            values.Add(filterValue);
        }
        return values;
    }

    private static List<object> CopyFilterValues(List<object> filterValues)
    {
        if (filterValues.Count == 0)
        {
            return EmptyFilterValues;
        }

        var values = new List<object>(filterValues.Count);
        foreach (var filterValue in filterValues)
        {
            values.Add(filterValue);
        }
        return values;
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
            ProcessClearFilter();
        }
    }

    internal void ProcessClearFilter()
    {
        if (OwningGrid is not null)
        {
            if (OwningColumn is null || !OwningColumn.EffectiveCanUserFilter)
            {
                return;
            }
            var eventArgs = new DataGridColumnEventArgs(OwningColumn);
            OwningGrid.NotifyColumnFiltering(eventArgs);
            if (!eventArgs.Handled)
            {
                OwningGrid.ApplyFilterGesture(OwningColumn, EmptyFilterValues);
            }
        }
    }
}
