using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public sealed class DataGridDetailExpanderColumn : DataGridColumn
{
    public DataGridDetailExpanderColumn()
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
        return new DataGridRowExpander();
    }
    
    protected override object? PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
    {
        return null;
    }

    protected override void NotifyOwningGridAttached(DataGrid? owningGrid)
    {
    }
    
    internal override DataGridColumnHeader CreateHeader()
    { 
        DataGridColumnHeader headerCell = base.CreateHeader();
        headerCell.IsEnabled              = false;
        headerCell.IndicatorLayoutVisible = false;
        return headerCell;
    }
}