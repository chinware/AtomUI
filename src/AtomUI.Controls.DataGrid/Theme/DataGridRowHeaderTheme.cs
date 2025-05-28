using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridRowHeaderTheme : BaseControlTheme
{
    public DataGridRowHeaderTheme() 
        : base(typeof(DataGridRowHeader))
    {
    }
}