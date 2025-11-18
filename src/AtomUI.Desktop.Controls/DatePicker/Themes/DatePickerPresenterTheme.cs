using AtomUI.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class DatePickerPresenterTheme : ControlTheme
{
    public static BorderThicknessFilterConverter BorderThicknessFilterConverter = new()
    {
        Top    = true,
        Bottom = false,
        Left   = false,
        Right  = false,
    };
}