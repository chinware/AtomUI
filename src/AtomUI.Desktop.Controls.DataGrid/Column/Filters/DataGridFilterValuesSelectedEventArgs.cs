namespace AtomUI.Desktop.Controls;

internal class DataGridFilterValuesSelectedEventArgs
{
    public bool IsConfirmed { get; }
    public List<String> Values { get; }

    public DataGridFilterValuesSelectedEventArgs(bool isConfirmed, List<String> values)
    {
        IsConfirmed = isConfirmed;
        Values = values;
    }
}