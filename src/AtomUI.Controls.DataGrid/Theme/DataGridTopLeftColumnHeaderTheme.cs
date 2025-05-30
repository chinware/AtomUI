using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridTopLeftColumnHeaderTheme : DataGridColumnHeaderTheme
{
    public DataGridTopLeftColumnHeaderTheme()
        : base(typeof(DataGridTopLeftColumnHeader))
    {
    }
}