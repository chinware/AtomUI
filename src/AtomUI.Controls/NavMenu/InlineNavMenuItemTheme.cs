using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class InlineNavMenuItemTheme : BaseNavMenuItemTheme
{
    public new const string ID = "InlineNavMenuItem";
    public const string MenuIndicatorIconLayoutPart = "PART_MenuIndicatorIconLayout";
    public const string ChildItemsPresenterPart = "PART_ChildItemsPresenter";
    public const string ChildItemsLayoutTransformPart = "PART_ChildItemsLayoutTransform";
    
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
        var menuIndicatorIconPresenter = new ContentPresenter()
        {
            Name = MenuIndicatorIconLayoutPart,
            Transitions = new Transitions()
            {
                AnimationUtils.CreateTransition<TransformOperationsTransition>(ContentPresenter.RenderTransformProperty)
            }
        };
        menuIndicatorIconPresenter.Content = indicatorIcon;
        return menuIndicatorIconPresenter;
    }

    protected override Control BuildMenuItemContent(INameScope scope)
    {
        var rootLayout = new StackPanel()
        {
            Orientation = Orientation.Vertical
        };
        var headerContent = base.BuildMenuItemContent(scope);

        TokenResourceBinder.CreateTokenBinding(headerContent, Control.MarginProperty, NavMenuTokenResourceKey.VerticalItemsPanelSpacing, BindingPriority.Template,
            (v) =>
            {
                if (v is double dval)
                {
                    return new Thickness(0, 0, 0, dval);
                }

                return new Thickness();
            });
        var childItemsLayoutTransform = new LayoutTransformControl()
        {
            Name = ChildItemsLayoutTransformPart,
        };
        childItemsLayoutTransform.RegisterInNameScope(scope);
        
        var itemsPresenter = new ItemsPresenter
        {
            Name      = ChildItemsPresenterPart,
            Focusable = false,
        };
        itemsPresenter.RegisterInNameScope(scope);
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, NavMenuItem.ItemsPanelProperty);

        childItemsLayoutTransform.Child = itemsPresenter;
        
        rootLayout.Children.Add(headerContent);
        rootLayout.Children.Add(childItemsLayoutTransform);
        return rootLayout;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var childItemsPanelStyle = new Style(selector => selector.Nesting().Template().Name(ChildItemsPresenterPart).Child().OfType<StackPanel>());
        childItemsPanelStyle.Add(StackPanel.SpacingProperty, NavMenuTokenResourceKey.VerticalItemsPanelSpacing);
        Add(childItemsPanelStyle);

        BuildMenuIndicatorStyle();
    }

    private void BuildMenuIndicatorStyle()
    {
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));
            var transformOptions   = new TransformOperations.Builder(1);
            transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
            menuIndicatorStyle.Add(ContentPresenter.RenderTransformProperty, transformOptions.Build());
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, true);
            Add(menuIndicatorStyle);
        }
        var openSubMenuStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Open));
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));
            var transformOptions   = new TransformOperations.Builder(1);
            transformOptions.AppendRotate(MathUtils.Deg2Rad(-90));
            menuIndicatorStyle.Add(ContentPresenter.RenderTransformProperty, transformOptions.Build());
            openSubMenuStyle.Add(menuIndicatorStyle);
        }
        Add(openSubMenuStyle);
        var emptySubMenuStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Empty));
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, false);
            emptySubMenuStyle.Add(menuIndicatorStyle);
        }
        Add(emptySubMenuStyle);
    }

    // private void BuildAnimationStyle()
    // {
    //     var closeSubMenuStyle = new Style(selector => selector.Nesting().Not(selector.Nesting().Class(StdPseudoClass.Open)));
    //     {
    //         var layoutTransformStyle =
    //             new Style(selector => selector.Nesting().Template().Name(ChildItemsLayoutTransformPart));
    //         var slideDownInMotionConfig = MotionFactory.BuildSlideUpOutMotion(TimeSpan.FromMilliseconds(300), new QuadraticEaseIn(),
    //             FillMode.Forward);
    //         foreach (var animation in slideDownInMotionConfig.Animations)
    //         {
    //             layoutTransformStyle.Animations.Add(animation);
    //         }
    //         layoutTransformStyle.Add(LayoutTransformControl.RenderTransformOriginProperty, slideDownInMotionConfig.RenderTransformOrigin);
    //         layoutTransformStyle.Add(LayoutTransformControl.IsVisibleProperty, false);
    //         closeSubMenuStyle.Add(layoutTransformStyle);
    //     }
    //     Add(closeSubMenuStyle);
    //     
    //     var openSubMenuStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Open));
    //     {
    //         var layoutTransformStyle =
    //             new Style(selector => selector.Nesting().Template().Name(ChildItemsLayoutTransformPart));
    //         var slideDownInMotionConfig = MotionFactory.BuildSlideUpInMotion(TimeSpan.FromMilliseconds(300), new QuadraticEaseIn(),
    //             FillMode.Forward);
    //         foreach (var animation in slideDownInMotionConfig.Animations)
    //         {
    //             layoutTransformStyle.Animations.Add(animation);
    //         }
    //
    //         layoutTransformStyle.Add(LayoutTransformControl.RenderTransformOriginProperty, slideDownInMotionConfig.RenderTransformOrigin);
    //         layoutTransformStyle.Add(LayoutTransformControl.IsVisibleProperty, true);
    //         openSubMenuStyle.Add(layoutTransformStyle);
    //     }
    //     Add(openSubMenuStyle);
    // }
}