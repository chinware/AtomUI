using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class VerticalNavMenuItemTheme : NavMenuItemTheme
{ 
    public const string ID = "VerticalNavMenuItem";
    
    public VerticalNavMenuItemTheme() : this(typeof(NavMenuItem))
    {
    }
    
    protected VerticalNavMenuItemTheme(Type targetType) : base(targetType)
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
}