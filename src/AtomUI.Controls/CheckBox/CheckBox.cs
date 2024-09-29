using Avalonia;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaCheckBox = Avalonia.Controls.CheckBox;

public class CheckBox : AvaloniaCheckBox,
                        ICustomHitTest
{
    public bool HitTest(Point point)
    {
        return true;
    }
}