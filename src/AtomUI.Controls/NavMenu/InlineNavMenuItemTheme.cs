using AtomUI.Theme.Styling;
using Avalonia.Controls;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class InlineNavMenuItemTheme : VerticalNavMenuItemTheme
{
    public new const string ID = "InlineNavMenuItem";
    public const string MenuIndicatorIconLayoutPart = "PART_MenuIndicatorIconLayout";
    
    public InlineNavMenuItemTheme() : base(typeof(NavMenuItem))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
    
    protected override Control BuildMenuIndicatorIcon()
    {
        var indicatorIcon   = base.BuildMenuIndicatorIcon();
        var layoutTransformControl = new LayoutTransformControl()
        {
            Name = MenuIndicatorIconLayoutPart
        };
        layoutTransformControl.Child = indicatorIcon;
        return layoutTransformControl;
    }
}