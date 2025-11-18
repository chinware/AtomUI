using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class GradientColorSelectedEventArgs : EventArgs
{
    public LinearGradientBrush? Value { get; }

    public GradientColorSelectedEventArgs(LinearGradientBrush value)
    {
        Value = value;
    }
}