namespace AtomUI.Desktop.Controls;

// Bounded projection helper for legacy row-layout algorithms. It owns no data,
// subscriptions, query state, cache or current-item state.
internal sealed class DataGridRangeDataAccess
{
    private readonly DataGrid _owner;

    public DataGridRangeDataAccess(DataGrid owner)
    {
        _owner = owner;
    }

    public int Count => _owner.RangeWindowDataCount;

    public Type? DataType => _owner.ItemsSource?.Schema.ItemType;

    public bool Any() => Count != 0;

    public bool TryGetCount(bool allowSlowCount, bool allowRetry, out int count)
    {
        count = Count;
        return _owner.ItemsSource is not null;
    }

    public object? GetDataItem(int rowIndex)
    {
        var slot = _owner.GetCommittedRangeSlot(rowIndex);
        return slot >= 0 && _owner.TryGetCommittedRangeEntry(slot, out var entry)
            ? entry.Item
            : null;
    }

    public int IndexOf(object? item)
    {
        if (item is null)
        {
            return -1;
        }
        var slot = _owner.FindCommittedRangeSlot(item);
        return slot >= 0
            ? _owner.GetCommittedRangeRowIndex(slot)
            : -1;
    }

    public Type? GetPropertyType(string? fieldName)
    {
        if (string.IsNullOrEmpty(fieldName) || _owner.ItemsSource is null)
        {
            return null;
        }
        return _owner.ItemsSource.Schema.TryGetField(new DataGridFieldId(fieldName), out var field)
            ? field.ValueType
            : null;
    }

    public bool GetPropertyIsReadOnly(string? fieldName) => false;

    public string? GetDisplayName(string? fieldName) => fieldName;
}
