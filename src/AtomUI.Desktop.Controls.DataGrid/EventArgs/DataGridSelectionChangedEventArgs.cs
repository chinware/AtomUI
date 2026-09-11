namespace AtomUI.Desktop.Controls;

public sealed class DataGridSelectionChangedEventArgs : EventArgs
{
    public DataGridSelectionChangedEventArgs(
        DataGridSelectionState oldSelection,
        DataGridSelectionState newSelection)
    {
        ArgumentNullException.ThrowIfNull(oldSelection);
        ArgumentNullException.ThrowIfNull(newSelection);
        OldSelection = oldSelection;
        NewSelection = newSelection;
    }

    public DataGridSelectionState OldSelection { get; }

    public DataGridSelectionState NewSelection { get; }
}
