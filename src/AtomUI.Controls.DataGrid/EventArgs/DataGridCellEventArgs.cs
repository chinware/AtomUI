using System.Diagnostics;

namespace AtomUI.Controls;

internal class DataGridCellEventArgs : EventArgs
{
    internal DataGridCellEventArgs(DataGridCell dataGridCell)
    {
        Debug.Assert(dataGridCell != null);
        Cell = dataGridCell;
    }

    internal DataGridCell Cell { get; private set; }
}
