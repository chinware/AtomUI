using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace AtomUI.Controls.Data;

internal interface ISelectOptionCollectionView: IEnumerable, INotifyCollectionChanged
{
    CultureInfo Culture { get; set; }
    bool Contains(object item);
    IEnumerable SourceCollection { get; }
    Func<object, bool>? Filter { get; set; }
    SelectFilterDescriptionCollection? FilterDescriptions { get; }
    bool IsEmpty { get; }
    void Refresh();
    IDisposable DeferRefresh();
    object AddNew();
    void Remove(object? item);
    void RemoveAt(int index);
}