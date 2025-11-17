using AtomUI.Controls.Converters;
using Avalonia.Layout;
using Avalonia.Styling;
using MarginMultiplierConverter = Avalonia.Controls.Converters.MarginMultiplierConverter;

namespace AtomUI.Controls.Themes;

internal class TreeViewItemTheme : ControlTheme
{
    public static readonly MarginMultiplierConverter MarginMultiplierConverter = new ()
    {
        Left   = true,
        Indent = 16
    };

    public static readonly TreeViewItemIndicatorEnabledConverter TreeViewItemIndicatorEnabledConverter = new ();
    
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
}