using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DrawerContainerTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string MaskPart = "PART_Mask";
    public const string InfoContainerPart = "PART_InfoContainer";
    public const string InfoContainerMotionActorPart = "PART_InfoContainerMotionActor";
    
    public DrawerContainerTheme() : base(typeof(DrawerContainerX))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DrawerContainerX>((drawerContainer, scope) =>
        {
            var rootLayout = new Panel
            {
                Name = RootLayoutPart,
            };
            
            var mask = new Border
            {
                Name = MaskPart,
            };
            CreateTemplateParentBinding(mask, Border.IsVisibleProperty, DrawerContainerX.IsShowMaskProperty);
            CreateTemplateParentBinding(mask, Border.BackgroundProperty, DrawerContainerX.BackgroundProperty);
            mask.RegisterInNameScope(scope);
            rootLayout.Children.Add(mask);
            
            var motionActor = new MotionActorControl
            {
                Name         = InfoContainerMotionActorPart,
                Opacity      = 0.0
            };
            
            motionActor.RegisterInNameScope(scope);
            
            var infoContainer = new DrawerInfoContainerX()
            {
                Name = InfoContainerPart
            };
            
            motionActor.Child = infoContainer;
            
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.PlacementProperty, DrawerContainerX.PlacementProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.TitleProperty, DrawerContainerX.TitleProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.FooterProperty, DrawerContainerX.FooterProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.FooterTemplateProperty, DrawerContainerX.FooterTemplateProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.ExtraProperty, DrawerContainerX.ExtraProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.ExtraTemplateProperty, DrawerContainerX.ExtraTemplateProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.SizeTypeProperty, DrawerContainerX.SizeTypeProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.ContentProperty, DrawerContainerX.ContentProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.ContentTemplateProperty, DrawerContainerX.ContentTemplateProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainerX.IsMotionEnabledProperty, DrawerContainerX.IsMotionEnabledProperty);
            infoContainer.RegisterInNameScope(scope);
            rootLayout.Children.Add(motionActor);
            return rootLayout;
        });
    }
    
    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(Border.BackgroundProperty, Brushes.Transparent);
        {
            var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerContainerX.IsMotionEnabledProperty, true));
            isMotionEnabledStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
            }));
            commonStyle.Add(isMotionEnabledStyle);
        }
        Add(commonStyle);
    }
}