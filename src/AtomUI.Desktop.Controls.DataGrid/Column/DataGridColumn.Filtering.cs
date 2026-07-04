using System.Collections;
using System.Collections.Specialized;
using AtomUI.Desktop.Controls.Data;

namespace AtomUI.Desktop.Controls;

public abstract partial class DataGridColumn
{
    private INotifyCollectionChanged? _subscribedFilterItemsSource;
    private INotifyCollectionChanged? _subscribedSelectedFilterValues;
    private readonly Dictionary<(Type ItemType, string Path), IDataGridDataMemberPathAccessor> _filterItemAccessors = new();

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
        if (TryUpdateExistingSelectedFilterValues(selectedValues))
        {
            return;
        }

        SelectedFilterValues = selectedValues;
    }

    internal void ApplySelectedFilterValuesToFilterDescriptions()
    {
        if (OwningGrid is not { } owningGrid ||
            owningGrid.DataConnection is not { } dataConnection ||
            dataConnection.FilterDescriptions is not { } filterDescriptions ||
            dataConnection.CollectionView is not { } collectionView ||
            !dataConnection.AllowFilter)
        {
            return;
        }

        var selectedValues = CopyFilterValues(SelectedFilterValues);
        var filter         = GetFilterDescription();
        using (collectionView.DeferRefresh())
        {
            if (selectedValues.Count == 0)
            {
                if (filter != null)
                {
                    filterDescriptions.Remove(filter);
                }
                return;
            }

            var propertyName = GetFilterPropertyName();
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (filter != null && FilterConditionsSetEquals(filter.FilterConditions, selectedValues))
            {
                return;
            }

            var newFilter = new DataGridFilterDescription
            {
                PropertyPath      = propertyName,
                Filter            = GetFilterPredicate(),
                FilterConditions  = selectedValues
            };

            if (filter == null)
            {
                filterDescriptions.Add(newFilter);
                return;
            }

            var oldIndex = filterDescriptions.IndexOf(filter);
            if (oldIndex >= 0)
            {
                filterDescriptions.Remove(filter);
                filterDescriptions.Insert(oldIndex, newFilter);
            }
            else
            {
                filterDescriptions.Add(newFilter);
            }
        }
    }

    internal void RemoveFilterDescriptionProjection()
    {
        if (OwningGrid?.DataConnection is not { } dataConnection ||
            dataConnection.FilterDescriptions is not { } filterDescriptions ||
            dataConnection.CollectionView is not { } collectionView)
        {
            return;
        }

        var filter = GetFilterDescription();
        if (filter == null)
        {
            return;
        }

        using (collectionView.DeferRefresh())
        {
            filterDescriptions.Remove(filter);
        }
    }

    internal bool IsFilterValueSelected(object? value)
    {
        return ContainsValue(SelectedFilterValues, value);
    }

    private void RegisterFilterItemsSource(IEnumerable? oldSource, IEnumerable? newSource)
    {
        if (ReferenceEquals(oldSource, newSource))
        {
            return;
        }

        if (_subscribedFilterItemsSource != null)
        {
            _subscribedFilterItemsSource.CollectionChanged -= HandleFilterItemsSourceCollectionChanged;
            _subscribedFilterItemsSource = null;
        }

        if (newSource is INotifyCollectionChanged notifyCollectionChanged)
        {
            _subscribedFilterItemsSource = notifyCollectionChanged;
            _subscribedFilterItemsSource.CollectionChanged += HandleFilterItemsSourceCollectionChanged;
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
        RegisterFilterItemsSource(_subscribedFilterItemsSource as IEnumerable, Filters);
        RegisterSelectedFilterValues(_subscribedSelectedFilterValues as IList, SelectedFilterValues);
    }

    private void ReleaseFilterCollectionSubscriptions()
    {
        if (_subscribedFilterItemsSource != null)
        {
            _subscribedFilterItemsSource.CollectionChanged -= HandleFilterItemsSourceCollectionChanged;
            _subscribedFilterItemsSource = null;
        }

        if (_subscribedSelectedFilterValues != null)
        {
            _subscribedSelectedFilterValues.CollectionChanged -= HandleSelectedFilterValuesCollectionChanged;
            _subscribedSelectedFilterValues = null;
        }
    }

    private void HandleFilterItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PruneSelectedFilterValuesToFilterItems();
        NotifyFilterItemsChanged();
    }

    private void HandleSelectedFilterValuesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplySelectedFilterValuesToFilterDescriptions();
        NotifySelectedFilterValuesChanged();
    }

    private void NotifyFilterItemsChanged()
    {
        _filterItemAccessors.Clear();
        if (HasHeaderCell)
        {
            HeaderCell.NotifyFilterItemsChanged();
        }
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

        var result = new DataGridFilterItem
        {
            Text  = GetFilterItemText(item),
            Value = GetFilterItemValue(item)
        };

        if (TryGetFilterItemChildren(item, out var children) && children is not null)
        {
            foreach (var child in children)
            {
                if (child is not null)
                {
                    result.Children.Add(CreateFilterItem(child));
                }
            }
        }

        return result;
    }

    private string GetFilterItemText(object item)
    {
        if (!string.IsNullOrEmpty(FilterTextMemberPath))
        {
            return GetFilterItemMemberValue(item, FilterTextMemberPath)?.ToString() ?? string.Empty;
        }

        return item.ToString() ?? string.Empty;
    }

    private object? GetFilterItemValue(object item)
    {
        if (!string.IsNullOrEmpty(FilterValueMemberPath))
        {
            return GetFilterItemMemberValue(item, FilterValueMemberPath);
        }

        return item;
    }

    private bool TryGetFilterItemChildren(object item, out IEnumerable? children)
    {
        children = null;
        if (string.IsNullOrEmpty(FilterChildrenMemberPath))
        {
            return false;
        }

        children = GetFilterItemMemberValue(item, FilterChildrenMemberPath) as IEnumerable;
        return children != null;
    }

    private object? GetFilterItemMemberValue(object item, string propertyPath)
    {
        var itemType = item.GetType();
        var key      = (itemType, propertyPath);
        if (!_filterItemAccessors.TryGetValue(key, out var accessor))
        {
            accessor = DataGridDataMemberPathAccessor.Resolve(
                propertyPath,
                itemType,
                dataMemberAccessorDescriptor: null,
                isDynamicCodeSupported: false);
            _filterItemAccessors.Add(key, accessor);
        }

        return accessor.GetValue(item);
    }

    private Func<object, object, bool>? GetFilterPredicate()
    {
        if (FilterEvaluator != null)
        {
            return (value, condition) => FilterEvaluator(value, condition);
        }

        return OnFilter;
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

    private bool TryUpdateExistingSelectedFilterValues(IReadOnlyList<object> selectedValues)
    {
        if (SelectedFilterValues is not { } currentValues ||
            currentValues.IsReadOnly ||
            currentValues.IsFixedSize)
        {
            return false;
        }

        var currentValueSnapshot = CopyFilterValues(currentValues);
        if (FilterValuesSequenceEqual(currentValueSnapshot, selectedValues))
        {
            ApplySelectedFilterValuesToFilterDescriptions();
            NotifySelectedFilterValuesChanged();
            return true;
        }

        var notifiesCollectionChanges = currentValues is INotifyCollectionChanged;
        currentValues.Clear();
        foreach (var selectedValue in selectedValues)
        {
            currentValues.Add(selectedValue);
        }

        if (!notifiesCollectionChanges)
        {
            ApplySelectedFilterValuesToFilterDescriptions();
            NotifySelectedFilterValuesChanged();
        }

        return true;
    }

    private static bool FilterConditionsSetEquals(List<object> oldFilterValues, List<object> newFilterValues)
    {
        return DistinctValuesAreContained(oldFilterValues, newFilterValues) &&
               DistinctValuesAreContained(newFilterValues, oldFilterValues);
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

    private static bool DistinctValuesAreContained(List<object> source, List<object> target)
    {
        for (var i = 0; i < source.Count; i++)
        {
            var value = source[i];
            if (ContainsValue(source, value, i))
            {
                continue;
            }

            if (!ContainsValue(target, value))
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
