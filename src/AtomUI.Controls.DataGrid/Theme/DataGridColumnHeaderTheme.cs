using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridColumnHeaderTheme : BaseControlTheme
{
    public DataGridColumnHeaderTheme()
        : base(typeof(DataGridColumnHeader))
    {
    }
}