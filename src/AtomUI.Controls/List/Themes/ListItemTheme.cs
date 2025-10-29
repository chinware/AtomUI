using AtomUI.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

internal class ListItemTheme : ControlTheme
{
    public static readonly StringToTextBlockConverter StringToTextBlockConverter = new ();
}
