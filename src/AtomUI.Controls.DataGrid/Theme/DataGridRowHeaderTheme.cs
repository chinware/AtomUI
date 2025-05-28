using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridRowHeaderTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    
    public DataGridRowHeaderTheme() 
        : base(typeof(DataGridRowHeader))
    {
    }
}