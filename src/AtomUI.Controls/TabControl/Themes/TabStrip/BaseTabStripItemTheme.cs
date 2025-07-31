using AtomUI.Controls.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

internal class BaseTabStripItemTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
}