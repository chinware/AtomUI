namespace AtomUI.Controls;

public class DataGridColumnDraggingOverEventArgs : EventArgs
{
    /// <summary>
    /// The current DraggingOver column object
    /// </summary>
    public DataGridColumn? DraggingOverColumn { get; }

    /// <summary>
    /// The column object currently being dragged
    /// </summary>
    public DataGridColumn? DraggedColumn { get; }

    public DataGridColumnDraggingOverEventArgs(DataGridColumn? draggedColumn, DataGridColumn? draggingOverColumn)
    {
        DraggedColumn      = draggedColumn;
        DraggingOverColumn = draggingOverColumn;
    }
}