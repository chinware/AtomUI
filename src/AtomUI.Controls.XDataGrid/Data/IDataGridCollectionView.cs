// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace AtomUI.Controls.XDataGrid.Data;

public interface IDataGridCollectionView : IEnumerable, INotifyCollectionChanged
{
    /// <summary>Gets or sets the cultural information for any operations of the view that may differ by culture, such as sorting.</summary>
    /// <returns>The culture information to use during culture-sensitive operations. </returns>
    CultureInfo Culture { get; set; }
    
    /// <summary>Indicates whether the specified item belongs to this collection view. </summary>
    /// <returns>true if the item belongs to this collection view; otherwise, false.</returns>
    /// <param name="item">The object to check. </param>
    bool Contains(object item);
    
    /// <summary>Gets the underlying collection.</summary>
    /// <returns>The underlying collection.</returns>
    IEnumerable SourceCollection { get; }
    
    /// <summary>Gets or sets a callback that is used to determine whether an item is appropriate for inclusion in the view. </summary>
    /// <returns>A method that is used to determine whether an item is appropriate for inclusion in the view.</returns>
    Func<object, bool> Filter { get; set; }
    
    /// <summary>Gets a value that indicates whether this view supports filtering by way of the <see cref="P:System.ComponentModel.ICollectionView.Filter" /> property.</summary>
    /// <returns>true if this view supports filtering; otherwise, false.</returns>
    bool CanFilter { get; }
    
    /// <summary>Gets a collection of <see cref="T:System.ComponentModel.SortDescription" /> instances that describe how the items in the collection are sorted in the view.</summary>
    /// <returns>A collection of values that describe how the items in the collection are sorted in the view.</returns>
    DataGridSortDescriptionCollection SortDescriptions { get; }
}