using Avalonia.Media;

namespace AtomUI.Controls;

public class GradientColorChangedEventArgs : EventArgs
{
    public GradientColorChangedEventArgs(LinearGradientBrush oldColor, LinearGradientBrush newColor)
    {
        OldColor = oldColor;
        NewColor = newColor;
    }
    
    public LinearGradientBrush OldColor { get; private set; }
    
    public LinearGradientBrush NewColor { get; private set; }
}