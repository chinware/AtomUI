namespace AtomUI.Desktop.Controls;

public sealed class CalendarDateRange
{
    public CalendarDateRange(DateTime start, DateTime end)
    {
        var s = start.Date;
        var e = end.Date;
        if (e < s)
        {
            throw new ArgumentOutOfRangeException(nameof(end), "range end must not be earlier than start");
        }

        Start = s;
        End   = e;
    }

    public DateTime Start { get; }
    public DateTime End { get; }

    // Legacy method for old DatePicker CalendarView (will be removed in later phases)
    internal bool ContainsAny(CalendarDateRange range)
    {
        _ = range ?? throw new ArgumentNullException(nameof(range));

        var start = DateTime.Compare(Start, range.Start);

        // Check if any part of the supplied range is contained by this
        // range or if the supplied range completely covers this range.
        return (start <= 0 && DateTime.Compare(End, range.Start) >= 0) ||
               (start >= 0 && DateTime.Compare(Start, range.End) <= 0);
    }
}
