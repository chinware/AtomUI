using System.Collections.Immutable;

namespace AtomUI.Desktop.Controls;

internal sealed class DataGridSourceResultIdentity
{
    public DataGridSourceResultIdentity(DataGridFetchRequest request, DataGridRangeResult result)
    {
        Query = request.Query;
        PageRequest = request.PageRequest;
        GroupExpansion = request.GroupExpansion;
        QueryRevision = request.QueryRevision;
        DataGeneration = request.DataGeneration;
        Snapshot = result.Snapshot;
        TotalEntryCount = result.TotalEntryCount;
        WindowDataCount = result.WindowDataCount;
        TotalDataCount = result.TotalDataCount;
    }

    public DataGridQuery Query { get; }

    public DataGridPageRequest? PageRequest { get; }

    public DataGridGroupExpansion GroupExpansion { get; }

    public long QueryRevision { get; }

    public long DataGeneration { get; }

    public DataGridSnapshotId Snapshot { get; }

    public int TotalEntryCount { get; }

    public int WindowDataCount { get; }

    public long TotalDataCount { get; }
}

internal sealed class DataGridValidatedRangeBlock
{
    public DataGridValidatedRangeBlock(
        DataGridRange range,
        ImmutableArray<DataGridSourceEntry> entries,
        DataGridSourceResultIdentity identity)
    {
        Range = range;
        Entries = entries;
        Identity = identity;
    }

    public DataGridRange Range { get; }

    public ImmutableArray<DataGridSourceEntry> Entries { get; }

    public DataGridSourceResultIdentity Identity { get; }

    public DataGridRangeBlockIdentity CreateCacheIdentity(object sourceIdentity) =>
        new(
            sourceIdentity,
            Identity.QueryRevision,
            Identity.DataGeneration,
            Identity.Snapshot,
            Range.StartIndex);
}

internal static class DataGridSourceContractValidator
{
    public static void ValidateRequest(DataGridFetchRequest request, DataGridSourceSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);
        if (request.Query is null || request.GroupExpansion is null || request.Range.Count <= 0)
        {
            throw Contract("The request is not fully initialized.");
        }
        if (request.QueryRevision < 0 || request.DataGeneration < 0)
        {
            throw Contract("Request revisions and generations cannot be negative.");
        }
        if (request.Range.Count > schema.MaximumRangeSize)
        {
            throw Contract(
                $"Requested range count {request.Range.Count} exceeds schema maximum {schema.MaximumRangeSize}.");
        }
        if (request.ExpectedSnapshot is { IsValid: false })
        {
            throw Contract("The expected snapshot identity is invalid.");
        }
        if (request.Query.Groups.IsEmpty && !request.GroupExpansion.CollapsedGroups.IsEmpty)
        {
            throw Contract("Collapsed groups require a grouped query.");
        }

        ValidateSorts(request.Query.Sorts, schema);
        ValidateFilters(request.Query.Filters, schema);
        ValidateGroups(request.Query.Groups, schema);
    }

    public static DataGridValidatedRangeBlock Validate(
        DataGridFetchRequest request,
        DataGridRangeResult result,
        DataGridSourceSchema schema,
        DataGridSourceResultIdentity? expectedIdentity)
    {
        ArgumentNullException.ThrowIfNull(result);
        ValidateRequest(request, schema);

        if (result.StartIndex != request.Range.StartIndex)
        {
            throw Contract(
                $"Result start {result.StartIndex} does not match request start {request.Range.StartIndex}.");
        }
        if (request.ExpectedSnapshot is { } expectedSnapshot && result.Snapshot != expectedSnapshot)
        {
            throw Contract(
                $"Result snapshot '{result.Snapshot}' does not match expected snapshot '{expectedSnapshot}'.");
        }
        if (request.PageRequest is null && result.TotalDataCount > int.MaxValue)
        {
            throw new DataGridPresentationLimitExceededException(
                $"Continuous DataGrid presentation supports at most {int.MaxValue} data rows; source reported {result.TotalDataCount}.");
        }

        var expectedWindowDataCount = GetExpectedWindowDataCount(request.PageRequest, result.TotalDataCount);
        if (result.WindowDataCount != expectedWindowDataCount)
        {
            throw Contract(
                $"Window data count {result.WindowDataCount} does not match expected count {expectedWindowDataCount}.");
        }
        if (request.Query.Groups.IsEmpty && result.TotalEntryCount != result.WindowDataCount)
        {
            throw Contract(
                "An ungrouped result must have one display entry per window data row.");
        }

        var remaining = result.TotalEntryCount - request.Range.StartIndex;
        var expectedEntryCount = remaining <= 0
            ? 0
            : Math.Min(request.Range.Count, remaining);
        if (result.Entries.Length != expectedEntryCount)
        {
            throw Contract(
                $"Result contains {result.Entries.Length} entries; the exact requested slice requires {expectedEntryCount}.");
        }

        ValidateIdentity(request, result, expectedIdentity);
        ValidateEntries(request, result, schema);

        var identity = expectedIdentity ?? new DataGridSourceResultIdentity(request, result);
        return new DataGridValidatedRangeBlock(request.Range, result.Entries, identity);
    }

    private static void ValidateSorts(
        ImmutableArray<DataGridSort> sorts,
        DataGridSourceSchema schema)
    {
        foreach (var sort in sorts)
        {
            var field = GetField(schema, sort.Field);
            var requiredDirection = sort.Direction == DataGridSortDirection.Ascending
                ? DataGridSortDirections.Ascending
                : DataGridSortDirections.Descending;
            if ((field.SortDirections & requiredDirection) == 0)
            {
                throw Contract(
                    $"Field '{sort.Field}' does not support direction '{sort.Direction}'.");
            }
        }
    }

    private static void ValidateFilters(
        ImmutableArray<DataGridFilter> filters,
        DataGridSourceSchema schema)
    {
        foreach (var filter in filters)
        {
            var field = GetField(schema, filter.Field);
            if (!field.TryGetFilterOperator(filter.Operator, out var filterOperator))
            {
                throw Contract(
                    $"Field '{filter.Field}' does not support operator '{filter.Operator}'.");
            }
            if (filter.Values.Length < filterOperator.MinimumValueCount ||
                filter.Values.Length > filterOperator.MaximumValueCount)
            {
                throw Contract(
                    $"Filter '{filter.Operator}' on field '{filter.Field}' has an invalid value count.");
            }
            foreach (var value in filter.Values)
            {
                var kind = ToKinds(value.Kind);
                if ((filterOperator.AcceptedKinds & kind) == 0)
                {
                    throw Contract(
                        $"Filter '{filter.Operator}' on field '{filter.Field}' does not accept scalar kind '{value.Kind}'.");
                }
            }
        }
    }

    private static void ValidateGroups(
        ImmutableArray<DataGridGroup> groups,
        DataGridSourceSchema schema)
    {
        foreach (var group in groups)
        {
            var field = GetField(schema, group.Field);
            if (!field.CanGroup)
            {
                throw Contract($"Field '{group.Field}' cannot be grouped.");
            }
            var requiredDirection = group.Direction == DataGridSortDirection.Ascending
                ? DataGridSortDirections.Ascending
                : DataGridSortDirections.Descending;
            if ((field.SortDirections & requiredDirection) == 0)
            {
                throw Contract(
                    $"Grouped field '{group.Field}' does not support direction '{group.Direction}'.");
            }
        }
    }

    private static DataGridFieldSchema GetField(
        DataGridSourceSchema schema,
        DataGridFieldId fieldId)
    {
        if (!schema.TryGetField(fieldId, out var field))
        {
            throw Contract($"Query references unknown field '{fieldId}'.");
        }
        return field;
    }

    private static int GetExpectedWindowDataCount(
        DataGridPageRequest? pageRequest,
        long totalDataCount)
    {
        if (pageRequest is null)
        {
            return checked((int)totalDataCount);
        }
        var window = pageRequest.Value;
        if (window.DataStartIndex >= totalDataCount)
        {
            return 0;
        }
        return (int)Math.Min(window.DataCount, totalDataCount - window.DataStartIndex);
    }

    private static void ValidateIdentity(
        DataGridFetchRequest request,
        DataGridRangeResult result,
        DataGridSourceResultIdentity? expectedIdentity)
    {
        if (expectedIdentity is null)
        {
            return;
        }
        if (request.ExpectedSnapshot is null)
        {
            throw Contract("A subsequent request must carry the committed snapshot identity.");
        }
        if (request.QueryRevision != expectedIdentity.QueryRevision ||
            request.DataGeneration != expectedIdentity.DataGeneration ||
            request.Query != expectedIdentity.Query ||
            request.PageRequest != expectedIdentity.PageRequest ||
            !request.GroupExpansion.Equals(expectedIdentity.GroupExpansion))
        {
            throw Contract("The expected result identity belongs to a different request scope.");
        }
        if (result.Snapshot != expectedIdentity.Snapshot ||
            result.TotalDataCount != expectedIdentity.TotalDataCount ||
            result.WindowDataCount != expectedIdentity.WindowDataCount ||
            result.TotalEntryCount != expectedIdentity.TotalEntryCount)
        {
            throw Contract("Snapshot or totals changed within one result identity.");
        }
    }

    private static void ValidateEntries(
        DataGridFetchRequest request,
        DataGridRangeResult result,
        DataGridSourceSchema schema)
    {
        var rowKeys = new HashSet<DataGridRowKey>(result.Entries.Length);
        var groupKeys = new HashSet<DataGridGroupKey>(result.Entries.Length);
        var previousWindowDataIndex = -1;
        long previousDataIndex = -1;

        foreach (var entry in result.Entries)
        {
            switch (entry.Kind)
            {
                case DataGridSourceEntryKind.Data:
                    ValidateDataEntry(
                        request,
                        result,
                        schema,
                        entry,
                        rowKeys,
                        ref previousWindowDataIndex,
                        ref previousDataIndex);
                    break;
                case DataGridSourceEntryKind.GroupHeader:
                    ValidateGroupEntry(request, entry, groupKeys);
                    break;
                default:
                    throw Contract($"Unknown source entry kind '{entry.Kind}'.");
            }
        }
    }

    private static void ValidateDataEntry(
        DataGridFetchRequest request,
        DataGridRangeResult result,
        DataGridSourceSchema schema,
        DataGridSourceEntry entry,
        HashSet<DataGridRowKey> rowKeys,
        ref int previousWindowDataIndex,
        ref long previousDataIndex)
    {
        if (!entry.RowKey.IsValid || entry.Group is not null)
        {
            throw Contract("A data entry must contain a valid row key and no group payload.");
        }
        if (entry.Item is not null && !schema.ItemType.IsInstanceOfType(entry.Item))
        {
            throw Contract(
                $"Data item type '{entry.Item.GetType()}' does not match schema item type '{schema.ItemType}'.");
        }
        if (!rowKeys.Add(entry.RowKey))
        {
            throw Contract($"Row key '{entry.RowKey}' occurs more than once in a range block.");
        }
        if (entry.WindowDataIndex < 0 || entry.WindowDataIndex >= result.WindowDataCount)
        {
            throw Contract("A data entry has an out-of-range window data index.");
        }
        var expectedDataIndex = checked(
            (request.PageRequest?.DataStartIndex ?? 0) + entry.WindowDataIndex);
        if (entry.DataIndex != expectedDataIndex)
        {
            throw Contract(
                $"Data index {entry.DataIndex} does not match expected index {expectedDataIndex}.");
        }
        if (entry.WindowDataIndex <= previousWindowDataIndex || entry.DataIndex <= previousDataIndex)
        {
            throw Contract("Data indices must be strictly increasing within a range block.");
        }
        previousWindowDataIndex = entry.WindowDataIndex;
        previousDataIndex = entry.DataIndex;
    }

    private static void ValidateGroupEntry(
        DataGridFetchRequest request,
        DataGridSourceEntry entry,
        HashSet<DataGridGroupKey> groupKeys)
    {
        if (entry.RowKey.IsValid || entry.Item is not null ||
            entry.WindowDataIndex != -1 || entry.DataIndex != -1 || entry.Group is null)
        {
            throw Contract("A group header entry has an invalid row, item, index, or group payload.");
        }
        var group = entry.Group;
        if (group.Level < 0 || group.Level >= request.Query.Groups.Length ||
            request.Query.Groups[group.Level].Field != group.Field)
        {
            throw Contract("A group header level and field must match the grouped query path.");
        }
        if (!groupKeys.Add(group.Key))
        {
            throw Contract($"Group key '{group.Key}' occurs more than once in a range block.");
        }
    }

    private static DataGridScalarKinds ToKinds(DataGridScalarKind kind) => kind switch
    {
        DataGridScalarKind.Null => DataGridScalarKinds.Null,
        DataGridScalarKind.Boolean => DataGridScalarKinds.Boolean,
        DataGridScalarKind.SignedInteger => DataGridScalarKinds.SignedInteger,
        DataGridScalarKind.UnsignedInteger => DataGridScalarKinds.UnsignedInteger,
        DataGridScalarKind.Double => DataGridScalarKinds.Double,
        DataGridScalarKind.Decimal => DataGridScalarKinds.Decimal,
        DataGridScalarKind.String => DataGridScalarKinds.String,
        DataGridScalarKind.Guid => DataGridScalarKinds.Guid,
        DataGridScalarKind.DateOnly => DataGridScalarKinds.DateOnly,
        DataGridScalarKind.TimeOnly => DataGridScalarKinds.TimeOnly,
        DataGridScalarKind.DateTimeOffset => DataGridScalarKinds.DateTimeOffset,
        _ => throw Contract($"Unknown scalar kind '{kind}'.")
    };

    private static DataGridSourceContractException Contract(string message) => new(message);
}
