// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Avalonia.Controls;

namespace AtomUI.Controls;

public class DataGridRowDetailsEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Avalonia.Controls.DataGridRowDetailsEventArgs" /> class. 
    /// </summary>
    /// <param name="row">The row that the event occurs for.</param>
    /// <param name="detailsElement">The row details section as a framework element.</param>
    public DataGridRowDetailsEventArgs(DataGridRow row, Control detailsElement)
    {
        Row            = row;
        DetailsElement = detailsElement;
    }

    /// <summary>
    /// Gets the row details section as a framework element.
    /// </summary>
    public Control DetailsElement
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the row that the event occurs for.
    /// </summary>
    public DataGridRow Row
    {
        get;
        private set;
    }
}