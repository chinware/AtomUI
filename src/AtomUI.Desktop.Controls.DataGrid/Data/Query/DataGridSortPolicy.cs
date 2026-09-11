using System.Collections.Immutable;

namespace AtomUI.Desktop.Controls;

public static class DataGridSortPolicy
{
    public static DataGridQuery Apply(
        DataGridQuery query,
        DataGridFieldId field,
        DataGridSortDirection? requestedDirection,
        DataGridSortUpdateMode mode,
        DataGridSortDirections allowedDirections)
    {
        ArgumentNullException.ThrowIfNull(query);
        ValidateField(field);
        ValidateMode(mode);

        var existingIndex = Find(query.Sorts, field);
        if (requestedDirection is null)
        {
            return existingIndex < 0 ? query : query.WithSorts(RemoveAt(query.Sorts, existingIndex));
        }

        ValidateAllowedDirections(allowedDirections);
        ValidateRequestedDirection(requestedDirection.Value, allowedDirections);
        var replacement = new DataGridSort(field, requestedDirection.Value);

        if (mode == DataGridSortUpdateMode.Replace)
        {
            if (query.Sorts.Length == 1 && query.Sorts[0] == replacement)
            {
                return query;
            }
            return query.WithSorts([replacement]);
        }

        if (existingIndex >= 0)
        {
            if (query.Sorts[existingIndex] == replacement)
            {
                return query;
            }
            return query.WithSorts(query.Sorts.SetItem(existingIndex, replacement));
        }
        return query.WithSorts(query.Sorts.Add(replacement));
    }

    public static DataGridQuery ApplyGesture(
        DataGridQuery query,
        DataGridFieldId field,
        bool append,
        DataGridSortDirections allowedDirections)
    {
        ArgumentNullException.ThrowIfNull(query);
        ValidateField(field);
        ValidateAllowedDirections(allowedDirections);

        var existingIndex = Find(query.Sorts, field);
        var nextDirection = existingIndex < 0
            ? FirstDirection(allowedDirections)
            : NextDirection(query.Sorts[existingIndex].Direction, allowedDirections);

        if (!append)
        {
            return nextDirection is null
                ? query.WithSorts(ImmutableArray<DataGridSort>.Empty)
                : Apply(
                    query,
                    field,
                    nextDirection,
                    DataGridSortUpdateMode.Replace,
                    allowedDirections);
        }

        return nextDirection is null
            ? existingIndex < 0
                ? query
                : query.WithSorts(RemoveAt(query.Sorts, existingIndex))
            : Apply(
                query,
                field,
                nextDirection,
                DataGridSortUpdateMode.AppendOrReplace,
                allowedDirections);
    }

    private static DataGridSortDirection FirstDirection(DataGridSortDirections allowedDirections) =>
        (allowedDirections & DataGridSortDirections.Ascending) != 0
            ? DataGridSortDirection.Ascending
            : DataGridSortDirection.Descending;

    private static DataGridSortDirection? NextDirection(
        DataGridSortDirection current,
        DataGridSortDirections allowedDirections)
    {
        DataGridQueryValidation.ValidateDirection(current, nameof(current));
        return current switch
        {
            DataGridSortDirection.Ascending
                when (allowedDirections & DataGridSortDirections.Descending) != 0 =>
                DataGridSortDirection.Descending,
            DataGridSortDirection.Ascending => null,
            DataGridSortDirection.Descending => null,
            _ => null
        };
    }

    private static int Find(ImmutableArray<DataGridSort> sorts, DataGridFieldId field)
    {
        for (var index = 0; index < sorts.Length; index++)
        {
            if (sorts[index].Field == field)
            {
                return index;
            }
        }
        return -1;
    }

    private static ImmutableArray<DataGridSort> RemoveAt(
        ImmutableArray<DataGridSort> sorts,
        int removeIndex)
    {
        var builder = ImmutableArray.CreateBuilder<DataGridSort>(sorts.Length - 1);
        for (var index = 0; index < sorts.Length; index++)
        {
            if (index != removeIndex)
            {
                builder.Add(sorts[index]);
            }
        }
        return builder.MoveToImmutable();
    }

    private static void ValidateField(DataGridFieldId field)
    {
        if (!field.IsValid)
        {
            throw new ArgumentException("The field identity must be valid.", nameof(field));
        }
    }

    private static void ValidateMode(DataGridSortUpdateMode mode)
    {
        if (mode is not DataGridSortUpdateMode.Replace and not DataGridSortUpdateMode.AppendOrReplace)
        {
            throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown sort update mode.");
        }
    }

    private static void ValidateAllowedDirections(DataGridSortDirections allowedDirections)
    {
        if ((allowedDirections & ~DataGridSortDirections.All) != 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(allowedDirections), allowedDirections, "Unknown allowed sort direction flags.");
        }
        if (allowedDirections == DataGridSortDirections.None)
        {
            throw new ArgumentException("At least one sort direction must be allowed.", nameof(allowedDirections));
        }
    }

    private static void ValidateRequestedDirection(
        DataGridSortDirection requestedDirection,
        DataGridSortDirections allowedDirections)
    {
        DataGridQueryValidation.ValidateDirection(requestedDirection, nameof(requestedDirection));
        var requiredFlag = requestedDirection == DataGridSortDirection.Ascending
            ? DataGridSortDirections.Ascending
            : DataGridSortDirections.Descending;
        if ((allowedDirections & requiredFlag) == 0)
        {
            throw new ArgumentException(
                $"Direction '{requestedDirection}' is not allowed.", nameof(requestedDirection));
        }
    }
}
