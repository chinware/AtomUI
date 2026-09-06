namespace AtomUI.Desktop.Controls;

internal readonly struct DataGridViewportPlan
{
    public DataGridViewportPlan(
        DataGridRange? visibleRange,
        DataGridRange? prefetchRange,
        DataGridDesiredViewport desiredViewport,
        double extentHeight)
    {
        VisibleRange = visibleRange;
        PrefetchRange = prefetchRange;
        DesiredViewport = desiredViewport;
        ExtentHeight = extentHeight;
    }

    public DataGridRange? VisibleRange { get; }

    public DataGridRange? PrefetchRange { get; }

    public DataGridDesiredViewport DesiredViewport { get; }

    public double ExtentHeight { get; }
}

internal sealed class DataGridViewportPlanner
{
    private readonly double _rowHeight;
    private readonly int _overscanViewports;
    private readonly SparseHeightDeltaIndex? _heightIndex;

    public DataGridViewportPlanner(
        double rowHeight,
        int overscanViewports,
        SparseHeightDeltaIndex? heightIndex = null)
    {
        if (!double.IsFinite(rowHeight) || rowHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rowHeight));
        }
        if (overscanViewports < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(overscanViewports));
        }
        if (heightIndex is not null && heightIndex.DefaultHeight != rowHeight)
        {
            throw new ArgumentException(
                "The sparse height index must use the planner's row-height estimate.",
                nameof(heightIndex));
        }
        _rowHeight = rowHeight;
        _overscanViewports = overscanViewports;
        _heightIndex = heightIndex;
    }

    public int LastProbeCount { get; private set; }

    public DataGridViewportPlan Plan(
        double offset,
        double viewportHeight,
        int totalEntryCount,
        int scrollDirection = 0)
    {
        if (!double.IsFinite(offset) || offset < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }
        if (!double.IsFinite(viewportHeight) || viewportHeight < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(viewportHeight));
        }
        if (totalEntryCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalEntryCount));
        }
        if (scrollDirection is < -1 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(scrollDirection));
        }

        var extent = _heightIndex?.GetExtent(totalEntryCount) ??
                     SparseHeightDeltaIndex.SafeMultiply(totalEntryCount, _rowHeight);
        if (totalEntryCount == 0 || viewportHeight == 0)
        {
            LastProbeCount = 1;
            return new DataGridViewportPlan(
                null,
                null,
                new DataGridDesiredViewport(0, 0, scrollDirection),
                extent);
        }

        int firstSlot;
        int lastSlot;
        if (_heightIndex is null || _heightIndex.MeasuredCount == 0)
        {
            firstSlot = ToSlot(Math.Floor(offset / _rowHeight), totalEntryCount);
            var endOffset = Math.BitDecrement(SparseHeightDeltaIndex.SafeAdd(offset, viewportHeight));
            lastSlot = ToSlot(Math.Floor(endOffset / _rowHeight), totalEntryCount);
            LastProbeCount = 2;
        }
        else
        {
            firstSlot = _heightIndex.FindSlotAtOffset(offset, totalEntryCount);
            var firstProbes = _heightIndex.LastProbeCount;
            var endOffset = Math.BitDecrement(SparseHeightDeltaIndex.SafeAdd(offset, viewportHeight));
            lastSlot = _heightIndex.FindSlotAtOffset(endOffset, totalEntryCount);
            LastProbeCount = Math.Max(firstProbes, _heightIndex.LastProbeCount);
        }
        if (lastSlot < firstSlot)
        {
            lastSlot = firstSlot;
        }

        var visibleCount = checked(lastSlot - firstSlot + 1);
        var overscanCount = Math.Min(
            (long)int.MaxValue,
            (long)visibleCount * _overscanViewports);
        var prefetchStart = (int)Math.Max(0, firstSlot - overscanCount);
        var prefetchEnd = (int)Math.Min(
            totalEntryCount,
            (long)lastSlot + 1 + overscanCount);
        return new DataGridViewportPlan(
            new DataGridRange(firstSlot, visibleCount),
            new DataGridRange(prefetchStart, prefetchEnd - prefetchStart),
            new DataGridDesiredViewport(firstSlot, visibleCount, scrollDirection),
            extent);
    }

    private static int ToSlot(double value, int totalEntryCount)
    {
        if (value <= 0)
        {
            return 0;
        }
        if (value >= totalEntryCount)
        {
            return totalEntryCount - 1;
        }
        return (int)value;
    }
}
