using AtomUI.Controls.Data;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Describes the AOT-safe member access used only to display an item field.
/// </summary>
public sealed class DataGridFieldDisplayAccessor
{
    private readonly Func<object, object?> _getter;
    private readonly Action<object, object?>? _setter;

    private DataGridFieldDisplayAccessor(
        Type itemType,
        Type valueType,
        Func<object, object?> getter,
        Action<object, object?>? setter,
        string? propertyChangedName)
    {
        ItemType = itemType;
        ValueType = valueType;
        _getter = getter;
        _setter = setter;
        PropertyChangedName = ValidatePropertyChangedName(propertyChangedName);
    }

    public Type ItemType { get; }

    public Type ValueType { get; }

    public bool CanWrite => _setter is not null;

    /// <summary>
    /// Gets the CLR property name observed through INotifyPropertyChanged. Null observes every change.
    /// </summary>
    public string? PropertyChangedName { get; }

    public static DataGridFieldDisplayAccessor Create<TItem, TValue>(
        Func<TItem, TValue> getter,
        Action<TItem, TValue>? setter = null,
        string? propertyChangedName = null)
    {
        ArgumentNullException.ThrowIfNull(getter);
        return new DataGridFieldDisplayAccessor(
            typeof(TItem),
            typeof(TValue),
            item => getter((TItem)item),
            setter is null
                ? null
                : (item, value) => setter((TItem)item, CastValue<TValue>(value)),
            propertyChangedName);
    }

    public static DataGridFieldDisplayAccessor FromDataMember<TItem>(IDataMemberAccessor accessor) =>
        FromDataMember<TItem>(accessor, accessor?.Path);

    public static DataGridFieldDisplayAccessor FromDataMember<TItem>(
        IDataMemberAccessor accessor,
        string? propertyChangedName)
    {
        ArgumentNullException.ThrowIfNull(accessor);
        return new DataGridFieldDisplayAccessor(
            typeof(TItem),
            accessor.ValueType,
            accessor.GetValue,
            accessor.CanWrite ? accessor.SetValue : null,
            propertyChangedName);
    }

    internal object? GetValue(object item) => _getter(item);

    internal void SetValue(object item, object? value)
    {
        if (_setter is null)
        {
            throw new InvalidOperationException("The display accessor is read-only.");
        }
        _setter(item, value);
    }

    private static TValue CastValue<TValue>(object? value) =>
        value is null ? default! : (TValue)value;

    private static string? ValidatePropertyChangedName(string? propertyChangedName)
    {
        if (propertyChangedName is null)
        {
            return null;
        }
        if (propertyChangedName.Length == 0 ||
            !string.Equals(propertyChangedName, propertyChangedName.Trim(), StringComparison.Ordinal) ||
            propertyChangedName.Any(char.IsControl))
        {
            throw new ArgumentException(
                "The property-changed name must be non-empty, trimmed, and contain no control characters.",
                nameof(propertyChangedName));
        }
        return propertyChangedName;
    }
}
