namespace AtomUI.Desktop.Controls;

public class TourStepNavRequestEventArgs : EventArgs
{
    public int Index { get; }

    public TourStepNavRequestEventArgs(int index)
    {
        Index = index;
    }
}