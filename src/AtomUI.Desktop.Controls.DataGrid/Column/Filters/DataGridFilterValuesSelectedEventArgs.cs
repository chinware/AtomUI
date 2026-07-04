namespace AtomUI.Desktop.Controls;

internal enum DataGridFilterValuesCommitKind
{
    Confirmed,
    PassiveClose,
    SelectionChanged
}

internal class DataGridFilterValuesSelectedEventArgs
{
    internal static readonly List<object> EmptyValues = new(0);

    public DataGridFilterValuesCommitKind CommitKind { get; }
    public bool IsConfirmed => CommitKind == DataGridFilterValuesCommitKind.Confirmed;
    public bool IsPassiveClose => CommitKind == DataGridFilterValuesCommitKind.PassiveClose;
    public bool IsSelectionChanged => CommitKind == DataGridFilterValuesCommitKind.SelectionChanged;
    public List<object> Values { get; }

    public DataGridFilterValuesSelectedEventArgs(DataGridFilterValuesCommitKind commitKind, List<object> values)
    {
        CommitKind = commitKind;
        Values     = values;
    }
}
