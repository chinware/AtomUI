namespace AtomUI.Desktop.Controls;

public class RateValueChangedEventArgs : EventArgs
{
    public double OldValue { get; }
    public double NewValue { get; set; }

    public RateValueChangedEventArgs(double oldValue, double newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}