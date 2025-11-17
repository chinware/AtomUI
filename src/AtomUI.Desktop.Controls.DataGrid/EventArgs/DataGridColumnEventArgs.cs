// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;

namespace AtomUI.Controls;

public class DataGridColumnEventArgs : HandledEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Desktop.Controls.DataGridColumnEventArgs" /> class.
    /// </summary>
    /// <param name="column">The column that the event occurs for.</param>
    public DataGridColumnEventArgs(DataGridColumn column)
    {
        Column = column ?? throw new ArgumentNullException(nameof(column));
    }

    /// <summary>
    /// Gets the column that the event occurs for.
    /// </summary>
    public DataGridColumn Column
    {
        get;
        private set;
    }
}