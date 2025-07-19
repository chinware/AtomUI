using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class DataGridOperationColumn : DataGridColumn
{
    private DataGrid? _owningGrid;

    public DataGridOperationColumn()
    {
        IsReadOnly = true;
    }

    protected override Control? GenerateEditingElement(DataGridCell cell, object dataItem, out ICellEditBinding? editBinding)
    {
        editBinding = null;
        return null;
    }

    protected override Control GenerateElement(DataGridCell cell, object dataItem)
    {
        Debug.Assert(OwningGrid != null);
        var operationButtons = new DataGridOperationButtons();
        operationButtons[!DataGridOperationButtons.IsMotionEnabledProperty] = OwningGrid[!DataGrid.IsMotionEnabledProperty];
        return operationButtons;
    }

    protected override object? PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
    {
        return null;
    }

    internal override DataGridColumnHeader CreateHeader()
    {
        DataGridColumnHeader headerCell = base.CreateHeader();
        headerCell.IndicatorLayoutVisible = false;
        return headerCell;
    }

    private void ConfigureOwningGrid()
    {
        if (OwningGrid != null)
        {
            if (OwningGrid != _owningGrid)
            {
                _owningGrid                           =  OwningGrid;
                _owningGrid.Columns.CollectionChanged += HandleColumnsCollectionChanged;
                _owningGrid.LoadingRow                += HandleLoadingRow;
                _owningGrid.UnloadingRow              += HandleUnLoadingRow;
            }
        }
    }

    private void HandleColumnsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Contains(this) && _owningGrid != null)
            {
                _owningGrid.Columns.CollectionChanged -= HandleColumnsCollectionChanged;
                _owningGrid.LoadingRow                -= HandleLoadingRow;
                _owningGrid.UnloadingRow              -= HandleUnLoadingRow;
                _owningGrid                           =  null;
            }
        }
    }

    private void HandleLoadingRow(object? sender, DataGridRowEventArgs e)
    {
        if (OwningGrid != null)
        {
            if (GetCellContent(e.Row) is DataGridRowExpander expander)
            {
                expander.NotifyLoadingRow(e.Row);
            }
        }
    }

    private void HandleUnLoadingRow(object? sender, DataGridRowEventArgs e)
    {
        if (OwningGrid != null)
        {
            if (GetCellContent(e.Row) is DataGridRowExpander expander)
            {
                expander.NotifyUnLoadingRow(e.Row);
            }
        }
    }

    protected override void NotifyOwningGridAttached(DataGrid? owningGrid)
    {
        base.NotifyOwningGridAttached(owningGrid);
        ConfigureOwningGrid();
    }

    public override bool IsEditable()
    {
        return false;
    }
}