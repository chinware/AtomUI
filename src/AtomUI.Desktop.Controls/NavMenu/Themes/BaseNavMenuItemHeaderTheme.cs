using AtomUI.Controls.Converters;
using Avalonia.Controls.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class BaseNavMenuItemHeaderTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
    public static readonly PlatformKeyGestureConverter KeyGestureConverter = new();
}