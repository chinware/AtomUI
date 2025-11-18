using AtomUI.Desktop.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls.Themes;

internal class BaseButtonTheme : ControlTheme
{
    public static readonly ButtonIconVisibleConverter ButtonIconVisibleConverter = new();
}