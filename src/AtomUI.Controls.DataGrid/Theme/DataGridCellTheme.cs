using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridCellTheme : BaseControlTheme
{
    public DataGridCellTheme() 
        : base(typeof(DataGridCell))
    {
    }
}