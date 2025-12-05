namespace AtomUI.Desktop.Controls;

public class RateValueChangedEventArgs : EventArgs
{
    public double Value { get; set; }

    public RateValueChangedEventArgs(double value)
    {
        Value = value;
    }
}