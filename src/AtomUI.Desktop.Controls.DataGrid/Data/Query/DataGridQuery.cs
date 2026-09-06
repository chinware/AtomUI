using System.Collections.Immutable;

namespace AtomUI.Desktop.Controls;

public enum DataGridSortDirection
{
    Ascending,
    Descending
}
public enum DataGridSortUpdateMode
{
    Replace,
    AppendOrReplace
}

public readonly struct DataGridSort : IEquatable<DataGridSort>
{
    public DataGridSort(DataGridFieldId field, DataGridSortDirection direction)
    {
        if (!field.IsValid)
        {
            throw new ArgumentException("The field identity must be valid.", nameof(field));
        }
        DataGridQueryValidation.ValidateDirection(direction, nameof(direction));
        Field = field;
        Direction = direction;
    }

    public DataGridFieldId Field { get; }

    public DataGridSortDirection Direction { get; }

    public bool Equals(DataGridSort other) => Field == other.Field && Direction == other.Direction;

    public override bool Equals(object? obj) => obj is DataGridSort other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Field, Direction);

    public static bool operator ==(DataGridSort left, DataGridSort right) => left.Equals(right);

    public static bool operator !=(DataGridSort left, DataGridSort right) => !left.Equals(right);
}

public readonly struct DataGridFilter : IEquatable<DataGridFilter>
{
    public DataGridFilter(
        DataGridFieldId field,
        DataGridOperatorId @operator,
        ImmutableArray<DataGridScalar> values)
    {
        if (!field.IsValid)
        {
            throw new ArgumentException("The field identity must be valid.", nameof(field));
        }
        if (!@operator.IsValid)
        {
            throw new ArgumentException("The operator identity must be valid.", nameof(@operator));
        }
        Field = field;
        Operator = @operator;
        Values = values.IsDefault ? ImmutableArray<DataGridScalar>.Empty : values;
    }

    public DataGridFieldId Field { get; }

    public DataGridOperatorId Operator { get; }

    public ImmutableArray<DataGridScalar> Values { get; }

    public bool Equals(DataGridFilter other) =>
        Field == other.Field &&
        Operator == other.Operator &&
        DataGridQueryValidation.SequenceEqual(Values, other.Values);

    public override bool Equals(object? obj) => obj is DataGridFilter other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Field);
        hash.Add(Operator);
        foreach (var value in Values)
        {
            hash.Add(value);
        }
        return hash.ToHashCode();
    }

    public static bool operator ==(DataGridFilter left, DataGridFilter right) => left.Equals(right);

    public static bool operator !=(DataGridFilter left, DataGridFilter right) => !left.Equals(right);
}

public readonly struct DataGridGroup : IEquatable<DataGridGroup>
{
    public DataGridGroup(DataGridFieldId field, DataGridSortDirection direction)
    {
        if (!field.IsValid)
        {
            throw new ArgumentException("The field identity must be valid.", nameof(field));
        }
        DataGridQueryValidation.ValidateDirection(direction, nameof(direction));
        Field = field;
        Direction = direction;
    }

    public DataGridFieldId Field { get; }

    public DataGridSortDirection Direction { get; }

    public bool Equals(DataGridGroup other) => Field == other.Field && Direction == other.Direction;

    public override bool Equals(object? obj) => obj is DataGridGroup other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Field, Direction);

    public static bool operator ==(DataGridGroup left, DataGridGroup right) => left.Equals(right);

    public static bool operator !=(DataGridGroup left, DataGridGroup right) => !left.Equals(right);
}

public sealed class DataGridQuery : IEquatable<DataGridQuery>
{
    private readonly int _structuralHashCode;

    public static DataGridQuery Empty { get; } = new(default, default, default);

    public DataGridQuery(
        ImmutableArray<DataGridSort> sorts,
        ImmutableArray<DataGridFilter> filters,
        ImmutableArray<DataGridGroup> groups)
    {
        Sorts = sorts.IsDefault ? ImmutableArray<DataGridSort>.Empty : sorts;
        Filters = filters.IsDefault ? ImmutableArray<DataGridFilter>.Empty : filters;
        Groups = groups.IsDefault ? ImmutableArray<DataGridGroup>.Empty : groups;

        DataGridQueryValidation.ValidateUniqueFields(Sorts, static sort => sort.Field, nameof(sorts));
        DataGridQueryValidation.ValidateUniqueFields(Filters, static filter => filter.Field, nameof(filters));
        DataGridQueryValidation.ValidateUniqueFields(Groups, static group => group.Field, nameof(groups));
        ValidateGroupSortOverlap();
        _structuralHashCode = ComputeStructuralHashCode();
    }

    public ImmutableArray<DataGridSort> Sorts { get; }

    public ImmutableArray<DataGridFilter> Filters { get; }

    public ImmutableArray<DataGridGroup> Groups { get; }

    public DataGridQuery WithSorts(ImmutableArray<DataGridSort> sorts)
    {
        var normalized = sorts.IsDefault ? ImmutableArray<DataGridSort>.Empty : sorts;
        return DataGridQueryValidation.SequenceEqual(Sorts, normalized)
            ? this
            : new DataGridQuery(normalized, Filters, Groups);
    }

    public DataGridQuery WithFilters(ImmutableArray<DataGridFilter> filters)
    {
        var normalized = filters.IsDefault ? ImmutableArray<DataGridFilter>.Empty : filters;
        return DataGridQueryValidation.SequenceEqual(Filters, normalized)
            ? this
            : new DataGridQuery(Sorts, normalized, Groups);
    }

    public DataGridQuery WithGroups(ImmutableArray<DataGridGroup> groups)
    {
        var normalized = groups.IsDefault ? ImmutableArray<DataGridGroup>.Empty : groups;
        return DataGridQueryValidation.SequenceEqual(Groups, normalized)
            ? this
            : new DataGridQuery(Sorts, Filters, normalized);
    }

    public bool Equals(DataGridQuery? other) =>
        ReferenceEquals(this, other) ||
        other is not null &&
        _structuralHashCode == other._structuralHashCode &&
        DataGridQueryValidation.SequenceEqual(Sorts, other.Sorts) &&
        DataGridQueryValidation.SequenceEqual(Filters, other.Filters) &&
        DataGridQueryValidation.SequenceEqual(Groups, other.Groups);

    public override bool Equals(object? obj) => obj is DataGridQuery other && Equals(other);

    public override int GetHashCode() => _structuralHashCode;

    public static bool operator ==(DataGridQuery? left, DataGridQuery? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(DataGridQuery? left, DataGridQuery? right) => !(left == right);

    private void ValidateGroupSortOverlap()
    {
        if (Groups.IsEmpty || Sorts.IsEmpty)
        {
            return;
        }
        var groupFields = new HashSet<DataGridFieldId>();
        foreach (var group in Groups)
        {
            groupFields.Add(group.Field);
        }
        foreach (var sort in Sorts)
        {
            if (groupFields.Contains(sort.Field))
            {
                throw new ArgumentException(
                    $"Field '{sort.Field}' cannot be present in both groups and sorts.",
                    nameof(Sorts));
            }
        }
    }

    private int ComputeStructuralHashCode()
    {
        var hash = new HashCode();
        hash.Add(Sorts.Length);
        foreach (var sort in Sorts)
        {
            hash.Add(sort);
        }
        hash.Add(Filters.Length);
        foreach (var filter in Filters)
        {
            hash.Add(filter);
        }
        hash.Add(Groups.Length);
        foreach (var group in Groups)
        {
            hash.Add(group);
        }
        return hash.ToHashCode();
    }
}

internal static class DataGridQueryValidation
{
    public static void ValidateDirection(DataGridSortDirection direction, string parameterName)
    {
        if (direction is not DataGridSortDirection.Ascending and not DataGridSortDirection.Descending)
        {
            throw new ArgumentOutOfRangeException(parameterName, direction, "Unknown sort direction.");
        }
    }

    public static void ValidateUniqueFields<T>(
        ImmutableArray<T> values,
        Func<T, DataGridFieldId> getField,
        string parameterName)
    {
        var fields = new HashSet<DataGridFieldId>();
        foreach (var value in values)
        {
            var field = getField(value);
            if (!field.IsValid)
            {
                throw new ArgumentException("Every field identity must be valid.", parameterName);
            }
            if (!fields.Add(field))
            {
                throw new ArgumentException($"Field '{field}' occurs more than once.", parameterName);
            }
        }
    }

    public static bool SequenceEqual<T>(ImmutableArray<T> left, ImmutableArray<T> right)
        where T : IEquatable<T>
    {
        if (left.Length != right.Length)
        {
            return false;
        }
        for (var index = 0; index < left.Length; index++)
        {
            if (!left[index].Equals(right[index]))
            {
                return false;
            }
        }
        return true;
    }
}
