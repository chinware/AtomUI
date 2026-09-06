namespace AtomUI.Desktop.Controls;

public enum DataGridQueryChangeReason
{
    External,
    SortGesture,
    FilterGesture,
    GroupChange,
    LoadRollback
}

public sealed class DataGridQueryChangedEventArgs : EventArgs
{
    public DataGridQueryChangedEventArgs(
        DataGridQuery oldQuery,
        DataGridQuery newQuery,
        long revision,
        DataGridQueryChangeReason reason)
    {
        ArgumentNullException.ThrowIfNull(oldQuery);
        ArgumentNullException.ThrowIfNull(newQuery);
        if (revision <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision));
        }
        OldQuery = oldQuery;
        NewQuery = newQuery;
        Revision = revision;
        Reason = reason;
    }

    public DataGridQuery OldQuery { get; }

    public DataGridQuery NewQuery { get; }

    public long Revision { get; }

    public DataGridQueryChangeReason Reason { get; }
}
