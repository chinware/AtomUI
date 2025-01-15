// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using Avalonia.Collections;

namespace AtomUI.Controls.Collections;

public interface IDataGridCollectionView : IEnumerable, INotifyCollectionChanged
{
    CultureInfo Culture { get; set; }
    bool Contains(object item);
    IEnumerable SourceCollection { get; }
    Func<object, bool> Filter { get; set; }
    bool CanFilter { get; }
    DataGridSortDescriptionCollection SortDescriptions { get; }
    bool CanSort { get; }
    bool CanGroup { get; }
    bool IsGrouping { get; }
    int GroupingDepth { get; }
    string GetGroupingPropertyNameAtDepth(int level);
    IAvaloniaReadOnlyList<object> Groups { get; }
    bool IsEmpty { get; }
    void Refresh();
    IDisposable DeferRefresh();
    object CurrentItem { get; }
    int CurrentPosition { get; }
    bool IsCurrentAfterLast { get; }
    bool IsCurrentBeforeFirst { get; }
    bool MoveCurrentToFirst();
    bool MoveCurrentToLast();
    bool MoveCurrentToNext();
    bool MoveCurrentToPrevious();
    bool MoveCurrentTo(object item);
    bool MoveCurrentToPosition(int position);

    event EventHandler<DataGridCurrentChangingEventArgs> CurrentChanging;
    event EventHandler CurrentChanged;
}