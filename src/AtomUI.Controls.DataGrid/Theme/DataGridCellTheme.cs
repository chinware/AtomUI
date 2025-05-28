using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridCellTheme : BaseControlTheme
{
    public const string RightGridLinePart = "PART_RightGridLine";
    
    public DataGridCellTheme() 
        : base(typeof(DataGridCell))
    {
    }
}