using AtomUI.Controls.Utils;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

internal class BaseTabItemTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
}