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
    
    public DrawerContainerTheme() : base(typeof(DrawerContainer))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DrawerContainer>((drawerContainer, scope) =>
        {
            var rootLayout = new Panel
            {
                Name = RootLayoutPart,
            };
            
            var mask = new Border
            {
                Name = MaskPart,
            };
            CreateTemplateParentBinding(mask, Border.IsVisibleProperty, DrawerContainer.IsShowMaskProperty);
            CreateTemplateParentBinding(mask, Border.BackgroundProperty, DrawerContainer.BackgroundProperty);
            mask.RegisterInNameScope(scope);
            rootLayout.Children.Add(mask);
            
            var motionActor = new MotionActorControl
            {
                Name         = InfoContainerMotionActorPart,
                Opacity      = 0.0
            };
            
            motionActor.RegisterInNameScope(scope);
            
            var infoContainer = new DrawerInfoContainer()
            {
                Name = InfoContainerPart
            };
            
            motionActor.Child = infoContainer;
            
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.PlacementProperty, DrawerContainer.PlacementProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.TitleProperty, DrawerContainer.TitleProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.FooterProperty, DrawerContainer.FooterProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.FooterTemplateProperty, DrawerContainer.FooterTemplateProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.ExtraProperty, DrawerContainer.ExtraProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.ExtraTemplateProperty, DrawerContainer.ExtraTemplateProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.ContentProperty, DrawerContainer.ContentProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.ContentTemplateProperty, DrawerContainer.ContentTemplateProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.IsMotionEnabledProperty, DrawerContainer.IsMotionEnabledProperty);
            CreateTemplateParentBinding(infoContainer, DrawerInfoContainer.DialogSizeProperty, DrawerContainer.DialogSizeProperty);
            infoContainer.RegisterInNameScope(scope);
            rootLayout.Children.Add(motionActor);
            return rootLayout;
        });
    }
    
    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DrawerContainer.MotionDurationProperty, SharedTokenKey.MotionDurationSlow);
        commonStyle.Add(DrawerContainer.MaskBgColorProperty, SharedTokenKey.ColorBgMask);
        commonStyle.Add(Border.BackgroundProperty, Brushes.Transparent);
        {
            var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(DrawerContainer.IsMotionEnabledProperty, true));
            isMotionEnabledStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
            }));
            commonStyle.Add(isMotionEnabledStyle);
        }
        Add(commonStyle);
    }
}