using Avalonia;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

public class RadioButton : AvaloniaRadioButton,
                           ICustomHitTest
{
    public bool HitTest(Point point)
    {
        return true;
    }
}