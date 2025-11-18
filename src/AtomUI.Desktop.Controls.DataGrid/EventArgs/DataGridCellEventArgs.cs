// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

namespace AtomUI.Desktop.Controls;

public class DataGridCellEventArgs : EventArgs
{
    internal DataGridCellEventArgs(DataGridCell dataGridCell)
    {
        Debug.Assert(dataGridCell != null);
        Cell = dataGridCell;
    }

    internal DataGridCell Cell
    {
        get;
        private set;
    }
}