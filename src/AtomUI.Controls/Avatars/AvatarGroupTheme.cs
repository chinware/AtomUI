
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class AvatarGroupTheme : BaseControlTheme
{
    public const string ContainerPart = "PART_Container";

    public AvatarGroupTheme()
        : base(typeof(AvatarGroup))
    {
    }

    protected override void BuildStyles()
    {
        buildChild();
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        var controlTemplate = new FuncControlTemplate<AvatarGroup>((avatar, scope) =>
        {
            var mainLayout = new StackPanel()
            {
                Name = ContainerPart,
                Orientation = Orientation.Horizontal,
            };
            scope.Register(ContainerPart, mainLayout);
            return mainLayout;
        });

        return controlTemplate;
    }
    
    private void buildChild()
    {
        /*var defaultSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AvatarGroup.AvatarShapeProperty, AvatarShape.Circle));
        defaultSizeStyle.Add(Avatar.AvatarShapeProperty, AvatarShape.Circle);
        Add(defaultSizeStyle);
        
        defaultSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AvatarGroup.AvatarShapeProperty, AvatarShape.Square));
        defaultSizeStyle.Add(Avatar.AvatarShapeProperty, AvatarShape.Square);
        Add(defaultSizeStyle);*/
        
        
        //groupOverlapping
        
        var defaultSizeStyle = new Style(selector =>
            selector.Nesting().Template().OfType<StackPanel>().Name(ContainerPart));
        defaultSizeStyle.Add(StackPanel.SpacingProperty, AvatarTokenKey.GroupOverlapping);
        //defaultSizeStyle.Add(Avatar.BackgroundProperty, AvatarTokenResourceKey.GroupBorderColor);
        Add(defaultSizeStyle);
        
        this.Add(AvatarGroup.HideSpaceProperty,  AvatarTokenKey.GroupSpace);
    }



}