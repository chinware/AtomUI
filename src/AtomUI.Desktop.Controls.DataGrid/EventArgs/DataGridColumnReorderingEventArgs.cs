// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public class DataGridColumnReorderingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Desktop.Controls.DataGridColumnReorderingEventArgs" /> class.
    /// </summary>
    /// <param name="dataGridColumn"></param>
    public DataGridColumnReorderingEventArgs(DataGridColumn dataGridColumn)
    {
        Column = dataGridColumn;
    }

    /// <summary>
    /// The column being moved.
    /// </summary>
    public DataGridColumn Column { get; private set; }

    /// <summary>
    /// The popup indicator displayed while dragging.  If null and Handled = true, then do not display a tooltip.
    /// </summary>
    public Control? DragIndicator { get; set; }
}