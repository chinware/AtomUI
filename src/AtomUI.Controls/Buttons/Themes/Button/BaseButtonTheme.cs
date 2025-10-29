using AtomUI.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

internal class BaseButtonTheme : ControlTheme
{
    public static readonly ButtonIconVisibleConverter ButtonIconVisibleConverter = new();
}