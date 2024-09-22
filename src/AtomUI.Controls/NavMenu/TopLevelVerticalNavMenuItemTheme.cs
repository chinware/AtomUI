using AtomUI.Theme;
using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TopLevelVerticalNavMenuItemTheme : BaseControlTheme
{
    public const string ID = "TopLevelVerticalNavMenuItem";
    
    public TopLevelVerticalNavMenuItemTheme() : base(typeof(NavMenuItem))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
}