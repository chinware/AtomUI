using AtomUI.Controls.Converters;
using Avalonia.Controls.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

internal class BaseNavMenuItemTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
    public static readonly PlatformKeyGestureConverter KeyGestureConverter = new();
}