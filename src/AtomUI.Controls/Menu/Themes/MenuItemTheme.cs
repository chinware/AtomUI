using AtomUI.Controls.Converters;
using AtomUI.Controls.Utils;
using Avalonia.Controls.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

public class MenuItemTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new ()
    {
        VerticalAlignment = VerticalAlignment.Center
    };
    public static readonly PlatformKeyGestureConverter KeyGestureConverter = new();
    public static readonly ToggleItemsLayoutVisibleConverter ToggleItemsLayoutVisibleConverter = new();
}