namespace AtomUI.Desktop.Controls;

internal class DataGridFilterValuesSelectedEventArgs
{
    internal static readonly List<string> EmptyValues = new(0);

    public bool IsConfirmed { get; }
    public List<string> Values { get; }

    public DataGridFilterValuesSelectedEventArgs(bool isConfirmed, List<string> values)
    {
        IsConfirmed = isConfirmed;
        Values = values;
    }
}
