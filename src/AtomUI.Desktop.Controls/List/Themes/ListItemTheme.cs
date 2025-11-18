using AtomUI.Desktop.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class ListItemTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new ();
}
