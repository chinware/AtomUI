using AtomUI.Desktop.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class CalendarItemTheme : ControlTheme
{
    public static BorderThicknessFilterConverter BorderThicknessFilterConverter = new()
    {
        Top    = false,
        Bottom = true,
        Left   = false,
        Right  = false,
    };
}