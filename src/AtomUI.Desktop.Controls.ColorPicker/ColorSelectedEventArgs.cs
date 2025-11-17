using Avalonia.Media;

namespace AtomUI.Controls;

public class ColorSelectedEventArgs : EventArgs
{
    public Color Value { get; }

    public ColorSelectedEventArgs(Color value)
    {
        Value = value;
    }
}