using AtomUI.Desktop.Controls.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class BaseTabItemTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
}