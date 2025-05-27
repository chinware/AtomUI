// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace AtomUI.Controls.XDataGrid.Data;

public interface IDataGridCollectionViewFactory
{
    /// <summary>Returns a custom view for specialized sorting, filtering, grouping, and currency.</summary>
    /// <returns>A custom view for specialized sorting, filtering, grouping, and currency.</returns>
    IDataGridCollectionView CreateView();
}