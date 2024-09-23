using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class InlineNavMenuItemTheme : VerticalNavMenuItemTheme
{
    public new const string ID = "InlineNavMenuItem";
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
}