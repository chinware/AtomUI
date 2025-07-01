using AtomUI.Controls.Converters;
using Avalonia.Styling;

namespace AtomUI.Controls.Themes;

internal class DataGridColumnHeaderTheme : ControlTheme
{
    public static readonly DataGridHeaderIndicatorLayoutMarginsConverter HeaderIndicatorLayoutMarginsConverter =
        new();
}