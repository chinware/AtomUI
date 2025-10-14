using Avalonia.Media;

namespace AtomUI.Controls;

public class GradientColorSelectedEventArgs : EventArgs
{
    public LinearGradientBrush? Value { get; }

    public GradientColorSelectedEventArgs(LinearGradientBrush value)
    {
        Value = value;
    }
}