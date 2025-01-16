// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace AtomUI.Controls;

/// <summary>
/// Defines modes that indicates how DataGrid content is copied to the Clipboard. 
/// </summary>
public enum DataGridClipboardCopyMode
{
    /// <summary>
    /// Disable the DataGrid's ability to copy selected items as text.
    /// </summary>
    None,

    /// <summary>
    /// Enable the DataGrid's ability to copy selected items as text, but do not include
    /// the column header content as the first line in the text that gets copied to the Clipboard.
    /// </summary>
    ExcludeHeader,

    /// <summary>
    /// Enable the DataGrid's ability to copy selected items as text, and include
    /// the column header content as the first line in the text that gets copied to the Clipboard.
    /// </summary>
    IncludeHeader
}
