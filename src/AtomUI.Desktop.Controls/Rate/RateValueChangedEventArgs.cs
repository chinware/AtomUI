namespace AtomUI.Desktop.Controls;

public class RateValueChangedEventArgs : EventArgs
{
    public int Value { get; set; }

    public RateValueChangedEventArgs(int value)
    {
        Value = value;
    }
}