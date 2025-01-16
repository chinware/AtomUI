using AtomUI.Controls.Utils;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class DataGridFillerColumn : DataGridColumn
{
    public DataGridFillerColumn(DataGrid owningGrid)
    {
        IsReadOnly = true;
        OwningGrid = owningGrid;
        MinWidth   = 0;
        MaxWidth   = int.MaxValue;
    }

    internal double FillerWidth { get; set; }

    // True if there is room for the filler column; otherwise, false
    internal bool IsActive => FillerWidth > 0;

    // True if the FillerColumn's header cell is contained in the visual tree
    internal bool IsRepresented { get; set; }

    internal override DataGridColumnHeader CreateHeader()
    {
        DataGridColumnHeader headerCell = base.CreateHeader();
        headerCell.IsEnabled = false;
        return headerCell;
    }

    protected override Control? GenerateElement(DataGridCell cell, object dataItem)
    {
        return null;
    }

    protected override Control? GenerateEditingElement(DataGridCell cell, object dataItem,
                                                       out ICellEditBinding? editBinding)
    {
        editBinding = null;
        return null;
    }

    protected override object? PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
    {
        return null;
    }
}