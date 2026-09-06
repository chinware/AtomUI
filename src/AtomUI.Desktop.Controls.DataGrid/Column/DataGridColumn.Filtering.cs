using System.Collections;
using System.Collections.Specialized;
namespace AtomUI.Desktop.Controls;

public abstract partial class DataGridColumn
{
    private INotifyCollectionChanged? _subscribedFilterItems;
    private INotifyCollectionChanged? _subscribedSelectedFilterValues;

    internal bool HasFilterItems => Filters is not null && GetFilterItemsCount() > 0;

    internal IEnumerable<DataGridFilterItem> GetEffectiveFilterItems()
    {
        if (Filters is null)
        {
            yield break;
        }

        foreach (var item in Filters)
        {
            if (item is null)
            {
                continue;
            }
            yield return CreateFilterItem(item);
        }
    }

    internal void SetSelectedFilterValuesFromFilterRequest(IEnumerable? filterValues)
    {
        var selectedValues = CopyFilterValues(filterValues);
        OwningGrid?.ApplyFilterGesture(this, selectedValues);
    }

    internal void ApplySelectedFilterValuesToQuery()
    {
        OwningGrid?.ApplyFilterGesture(this, CopyFilterValues(SelectedFilterValues));
    }

    internal void RemoveFilterQueryProjection()
    {
        // The DataGrid owns Query. Detaching a visual column must not mutate it.
    }

    internal bool IsFilterValueSelected(object? value)
    {
        if (OwningGrid is not null &&
            FieldId is { IsValid: true } queryField &&
            DataGridScalar.TryFromValue(value, out var scalar))
        {
            foreach (var filter in OwningGrid.Query.Filters)
            {
                if (filter.Field != queryField)
                {
                    continue;
                }
                foreach (var selected in filter.Values)
                {
                    if (selected == scalar)
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        return false;
    }

    internal bool HasActiveQueryFilter
    {
        get
        {
            if (OwningGrid is null || FieldId is not { IsValid: true } queryField)
            {
                return false;
            }
            foreach (var filter in OwningGrid.Query.Filters)
            {
                if (filter.Field == queryField)
                {
                    return true;
                }
            }
            return false;
        }
    }

    private void RegisterFilterItems(IEnumerable? oldItems, IEnumerable? newItems)
    {
        if (ReferenceEquals(oldItems, newItems))
        {
            return;
        }

        if (_subscribedFilterItems != null)
        {
            _subscribedFilterItems.CollectionChanged -= HandleFilterItemsCollectionChanged;
            _subscribedFilterItems = null;
        }

        if (newItems is INotifyCollectionChanged notifyCollectionChanged)
        {
            _subscribedFilterItems = notifyCollectionChanged;
            _subscribedFilterItems.CollectionChanged += HandleFilterItemsCollectionChanged;
        }
    }

    private void RegisterSelectedFilterValues(IList? oldValues, IList? newValues)
    {
        if (ReferenceEquals(oldValues, newValues))
        {
            return;
        }

        if (_subscribedSelectedFilterValues != null)
        {
            _subscribedSelectedFilterValues.CollectionChanged -= HandleSelectedFilterValuesCollectionChanged;
            _subscribedSelectedFilterValues = null;
        }

        if (newValues is INotifyCollectionChanged notifyCollectionChanged)
        {
            _subscribedSelectedFilterValues = notifyCollectionChanged;
            _subscribedSelectedFilterValues.CollectionChanged += HandleSelectedFilterValuesCollectionChanged;
        }
    }

    private void RegisterFilterCollectionSubscriptions()
    {
        RegisterFilterItems(_subscribedFilterItems as IEnumerable, Filters);
        RegisterSelectedFilterValues(_subscribedSelectedFilterValues as IList, SelectedFilterValues);
    }

    private void ReleaseFilterCollectionSubscriptions()
    {
        if (_subscribedFilterItems != null)
        {
            _subscribedFilterItems.CollectionChanged -= HandleFilterItemsCollectionChanged;
            _subscribedFilterItems = null;
        }

        if (_subscribedSelectedFilterValues != null)
        {
            _subscribedSelectedFilterValues.CollectionChanged -= HandleSelectedFilterValuesCollectionChanged;
            _subscribedSelectedFilterValues = null;
        }
    }

    private void HandleFilterItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PruneSelectedFilterValuesToFilterItems();
        NotifyFilterItemsChanged();
    }

    private void HandleSelectedFilterValuesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplySelectedFilterValuesToQuery();
        NotifySelectedFilterValuesChanged();
    }

    private void NotifyFilterItemsChanged()
    {
        if (HasHeaderCell)
        {
            HeaderCell.NotifyFilterItemsChanged();
        }
        OwningGrid?.RefreshPopupPinnedOpenFilterTarget();
    }

    private void NotifySelectedFilterValuesChanged()
    {
        if (HasHeaderCell)
        {
            HeaderCell.NotifySelectedFilterValuesChanged();
        }
    }

    private int GetFilterItemsCount()
    {
        var count = 0;
        foreach (var item in Filters!)
        {
            if (item is not null)
            {
                count++;
            }
        }
        return count;
    }

    private void PruneSelectedFilterValuesToFilterItems()
    {
        var selectedValues = CopyFilterValues(SelectedFilterValues);
        if (selectedValues.Count == 0)
        {
            return;
        }

        var effectiveValues = new List<object>();
        CollectFilterLeafValues(effectiveValues, GetEffectiveFilterItems());

        var prunedValues = new List<object>(selectedValues.Count);
        foreach (var selectedValue in selectedValues)
        {
            if (ContainsValue(effectiveValues, selectedValue))
            {
                prunedValues.Add(selectedValue);
            }
        }

        if (FilterValuesSequenceEqual(selectedValues, prunedValues))
        {
            return;
        }

        SetSelectedFilterValuesFromFilterRequest(prunedValues);
    }

    private static void CollectFilterLeafValues(List<object> values, IEnumerable<DataGridFilterItem> filterItems)
    {
        foreach (var filterItem in filterItems)
        {
            if (filterItem.HasChildren)
            {
                CollectFilterLeafValues(values, filterItem.Children);
            }
            else if (filterItem.Value is not null)
            {
                values.Add(filterItem.Value);
            }
        }
    }

    private DataGridFilterItem CreateFilterItem(object item)
    {
        if (item is DataGridFilterItem filterItem)
        {
            return filterItem;
        }

        return new DataGridFilterItem
        {
            Text  = item.ToString() ?? string.Empty,
            Value = item
        };
    }

    private static List<object> CopyFilterValues(IEnumerable? filterValues)
    {
        if (filterValues == null)
        {
            return [];
        }

        var values = new List<object>();
        foreach (var filterValue in filterValues)
        {
            if (filterValue is not null)
            {
                values.Add(filterValue);
            }
        }
        return values;
    }

    private static bool FilterValuesSequenceEqual(IReadOnlyList<object> oldFilterValues, IReadOnlyList<object> newFilterValues)
    {
        if (oldFilterValues.Count != newFilterValues.Count)
        {
            return false;
        }

        for (var i = 0; i < oldFilterValues.Count; i++)
        {
            if (!Equals(oldFilterValues[i], newFilterValues[i]))
            {
                return false;
            }
        }
        return true;
    }

    private static bool ContainsValue(IEnumerable? values, object? value, int? endExclusive = null)
    {
        if (values == null)
        {
            return false;
        }

        var index = 0;
        foreach (var item in values)
        {
            if (endExclusive.HasValue && index >= endExclusive.Value)
            {
                break;
            }

            if (Equals(item, value))
            {
                return true;
            }

            index++;
        }

        return false;
    }
}
