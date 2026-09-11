using System.Collections.Immutable;

namespace AtomUI.Desktop.Controls;

public readonly struct DataGridLocalFilter<TValue>
{
    public DataGridLocalFilter(
        DataGridOperatorId @operator,
        int minimumValueCount,
        int maximumValueCount,
        DataGridScalarKinds acceptedKinds,
        Func<TValue, ImmutableArray<DataGridScalar>, bool> evaluate)
    {
        ArgumentNullException.ThrowIfNull(evaluate);
        Schema = new DataGridFilterOperatorSchema(
            @operator,
            minimumValueCount,
            maximumValueCount,
            acceptedKinds);
        Evaluate = evaluate;
    }

    public DataGridOperatorId Operator => Schema.Id;

    public int MinimumValueCount => Schema.MinimumValueCount;

    public int MaximumValueCount => Schema.MaximumValueCount;

    public DataGridScalarKinds AcceptedKinds => Schema.AcceptedKinds;

    public Func<TValue, ImmutableArray<DataGridScalar>, bool> Evaluate { get; }

    internal DataGridFilterOperatorSchema Schema { get; }
}

internal interface IDataGridLocalField<T>
{
    DataGridFieldSchema Schema { get; }

    int Compare(T left, T right);

    DataGridScalar GetScalar(T item);

    bool Evaluate(T item, DataGridFilter filter);
}

public sealed class DataGridLocalField<T, TValue> : IDataGridLocalField<T>
{
    private readonly Dictionary<DataGridOperatorId, DataGridLocalFilter<TValue>> _filtersById;

    public DataGridLocalField(
        DataGridFieldId fieldId,
        Func<T, TValue> getter,
        IComparer<TValue> sortComparer,
        Func<TValue, DataGridScalar> toScalar,
        ImmutableArray<DataGridLocalFilter<TValue>> filters = default,
        DataGridSortDirections sortDirections = DataGridSortDirections.All,
        bool canGroup = false,
        string? propertyChangedName = null)
    {
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(sortComparer);
        ArgumentNullException.ThrowIfNull(toScalar);

        FieldId = fieldId;
        Getter = getter;
        SortComparer = sortComparer;
        ToScalar = toScalar;
        Filters = filters.IsDefault ? ImmutableArray<DataGridLocalFilter<TValue>>.Empty : filters;

        var filterSchemas = ImmutableArray.CreateBuilder<DataGridFilterOperatorSchema>(Filters.Length);
        _filtersById = new Dictionary<DataGridOperatorId, DataGridLocalFilter<TValue>>(Filters.Length);
        foreach (var filter in Filters)
        {
            if (filter.Evaluate is null || filter.Schema is null)
            {
                throw new ArgumentException("Local filters cannot contain default values.", nameof(filters));
            }
            if (!_filtersById.TryAdd(filter.Operator, filter))
            {
                throw new ArgumentException(
                    $"Filter operator '{filter.Operator}' occurs more than once.", nameof(filters));
            }
            filterSchemas.Add(filter.Schema);
        }
        Schema = new DataGridFieldSchema(
            fieldId,
            typeof(TValue),
            sortDirections,
            filterSchemas.MoveToImmutable(),
            canGroup,
            DataGridFieldDisplayAccessor.Create(getter, propertyChangedName: propertyChangedName));
    }

    public DataGridFieldId FieldId { get; }

    public Func<T, TValue> Getter { get; }

    public IComparer<TValue> SortComparer { get; }

    public Func<TValue, DataGridScalar> ToScalar { get; }

    public ImmutableArray<DataGridLocalFilter<TValue>> Filters { get; }

    DataGridFieldSchema IDataGridLocalField<T>.Schema => Schema;

    internal DataGridFieldSchema Schema { get; }

    int IDataGridLocalField<T>.Compare(T left, T right) =>
        SortComparer.Compare(Getter(left), Getter(right));

    DataGridScalar IDataGridLocalField<T>.GetScalar(T item) => ToScalar(Getter(item));

    bool IDataGridLocalField<T>.Evaluate(T item, DataGridFilter filter)
    {
        if (!_filtersById.TryGetValue(filter.Operator, out var localFilter))
        {
            throw new DataGridSourceContractException(
                $"No local evaluator exists for operator '{filter.Operator}' on field '{FieldId}'.");
        }
        return localFilter.Evaluate(Getter(item), filter.Values);
    }
}

public static class DataGridLocalSourceDescriptor
{
    public static DataGridLocalSourceDescriptor<T> For<T>(Func<T, DataGridRowKey> rowKey) =>
        new(rowKey);
}

public sealed class DataGridLocalSourceDescriptor<T>
{
    private readonly List<IDataGridLocalField<T>> _fields = [];
    private readonly HashSet<DataGridFieldId> _fieldIds = [];

    internal DataGridLocalSourceDescriptor(Func<T, DataGridRowKey> rowKey)
    {
        ArgumentNullException.ThrowIfNull(rowKey);
        RowKey = rowKey;
    }

    internal Func<T, DataGridRowKey> RowKey { get; }

    internal IReadOnlyList<IDataGridLocalField<T>> Fields => _fields;

    public DataGridLocalSourceDescriptor<T> Field<TValue>(
        string fieldId,
        Func<T, TValue> getter,
        IComparer<TValue>? sortComparer = null,
        Func<TValue, DataGridScalar>? toScalar = null,
        ImmutableArray<DataGridLocalFilter<TValue>> filters = default,
        DataGridSortDirections sortDirections = DataGridSortDirections.All,
        bool canGroup = false,
        string? propertyChangedName = null) =>
        Field(
            new DataGridFieldId(fieldId),
            getter,
            sortComparer,
            toScalar,
            filters,
            sortDirections,
            canGroup,
            propertyChangedName);

    public DataGridLocalSourceDescriptor<T> Field<TValue>(
        DataGridFieldId fieldId,
        Func<T, TValue> getter,
        IComparer<TValue>? sortComparer = null,
        Func<TValue, DataGridScalar>? toScalar = null,
        ImmutableArray<DataGridLocalFilter<TValue>> filters = default,
        DataGridSortDirections sortDirections = DataGridSortDirections.All,
        bool canGroup = false,
        string? propertyChangedName = null)
    {
        if (!_fieldIds.Add(fieldId))
        {
            throw new ArgumentException($"Field '{fieldId}' occurs more than once.", nameof(fieldId));
        }
        _fields.Add(new DataGridLocalField<T, TValue>(
            fieldId,
            getter,
            sortComparer ?? Comparer<TValue>.Default,
            toScalar ?? DataGridLocalScalarConverter<TValue>.Convert,
            filters,
            sortDirections,
            canGroup,
            propertyChangedName));
        return this;
    }
}

internal static class DataGridLocalScalarConverter<TValue>
{
    public static DataGridScalar Convert(TValue value)
    {
        object? boxed = value;
        return boxed switch
        {
            null => DataGridScalar.Null,
            bool item => DataGridScalar.FromBoolean(item),
            sbyte item => DataGridScalar.FromInt64(item),
            short item => DataGridScalar.FromInt64(item),
            int item => DataGridScalar.FromInt64(item),
            long item => DataGridScalar.FromInt64(item),
            byte item => DataGridScalar.FromUInt64(item),
            ushort item => DataGridScalar.FromUInt64(item),
            uint item => DataGridScalar.FromUInt64(item),
            ulong item => DataGridScalar.FromUInt64(item),
            float item => DataGridScalar.FromDouble(item),
            double item => DataGridScalar.FromDouble(item),
            decimal item => DataGridScalar.FromDecimal(item),
            string item => DataGridScalar.FromString(item),
            Guid item => DataGridScalar.FromGuid(item),
            DateOnly item => DataGridScalar.FromDateOnly(item),
            TimeOnly item => DataGridScalar.FromTimeOnly(item),
            DateTimeOffset item => DataGridScalar.FromDateTimeOffset(item),
            _ => throw new InvalidOperationException(
                $"No default DataGridScalar conversion exists for '{typeof(TValue)}'. Supply an explicit converter.")
        };
    }
}
