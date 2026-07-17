namespace AtomUI.Desktop.Controls;

public sealed class StepsCurrentChangeRequestedEventArgs : EventArgs
{
    public StepsCurrentChangeRequestedEventArgs(int current)
    {
        Current = current;
    }

    public int Current { get; }
}
