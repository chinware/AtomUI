using AtomUI.Desktop.Controls.CalendarView.Infrastructure;
namespace AtomUI.Desktop.Controls.CalendarView.State;

internal enum CalendarRangeActivePart
{
    Start,
    End
}

internal readonly record struct CalendarRangeSelectionState(
    DateTime? Start,
    DateTime? End,
    DateTime? HoverDate,
    CalendarRangeActivePart ActivePart,
    bool RepairReverseRange)
{
    public bool TryGetCommittedRange(out DateTime visualStart, out DateTime visualEnd)
    {
        return TryNormalizeRange(Start, End, out visualStart, out visualEnd);
    }

    public bool TryGetPreviewRange(out DateTime visualStart, out DateTime visualEnd)
    {
        if (HoverDate is null)
        {
            visualStart = default;
            visualEnd   = default;
            return false;
        }

        if (ActivePart == CalendarRangeActivePart.Start)
        {
            return TryNormalizePreviewRange(HoverDate, End, out visualStart, out visualEnd);
        }

        return TryNormalizePreviewRange(Start, HoverDate, out visualStart, out visualEnd);
    }

    private static bool TryNormalizeRange(DateTime? start, DateTime? end, out DateTime visualStart, out DateTime visualEnd)
    {
        if (start is null || end is null)
        {
            visualStart = default;
            visualEnd   = default;
            return false;
        }

        visualStart = start.Value;
        visualEnd   = end.Value;
        if (DateTimeHelper.CompareDays(visualStart, visualEnd) > 0)
        {
            (visualStart, visualEnd) = (visualEnd, visualStart);
        }

        return DateTimeHelper.CompareDays(visualStart, visualEnd) != 0;
    }

    private static bool TryNormalizePreviewRange(DateTime? start, DateTime? end, out DateTime visualStart, out DateTime visualEnd)
    {
        if (start is null && end is null)
        {
            visualStart = default;
            visualEnd   = default;
            return false;
        }

        start ??= end;
        end   ??= start;

        visualStart = start!.Value;
        visualEnd   = end!.Value;
        if (DateTimeHelper.CompareDays(visualStart, visualEnd) > 0)
        {
            (visualStart, visualEnd) = (visualEnd, visualStart);
        }

        return DateTimeHelper.CompareDays(visualStart, visualEnd) != 0;
    }
}
