using AtomUI.Controls.Utils;
using Avalonia.Layout;
using Avalonia.Styling;
using MarginMultiplierConverter = Avalonia.Controls.Converters.MarginMultiplierConverter;

namespace AtomUI.Controls.Themes;

public class TreeViewItemTheme : ControlTheme
{
    public static readonly MarginMultiplierConverter MarginMultiplierConverter = new ()
    {
        Left   = true,
        Indent = 16
    };

    public static readonly TreeViewItemRadioVisible TreeViewItemRadioVisible = new ();
    
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
}