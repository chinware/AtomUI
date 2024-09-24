using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
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
    
    protected override Control BuildMenuIndicatorIcon(INameScope scope)
    {
        var indicatorIcon   = base.BuildMenuIndicatorIcon(scope);
        var menuIndicatorIconPresenter = new Border()
        {
            Name = MenuIndicatorIconLayoutPart,
            Transitions = new Transitions()
            {
                AnimationUtils.CreateTransition<TransformOperationsTransition>(ContentPresenter.RenderTransformProperty)
            }
        };
        menuIndicatorIconPresenter.Child = indicatorIcon;
        menuIndicatorIconPresenter.RegisterInNameScope(scope);
        return menuIndicatorIconPresenter;
    }

    protected override Control BuildMenuItemContent(NavMenuItem navMenuItem, INameScope scope)
    {
        var rootLayout = new StackPanel()
        {
            Orientation = Orientation.Vertical
        };
        
        var headerContent = base.BuildMenuItemContent(navMenuItem, scope);
        
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

    protected override Grid BuildMenuItemInfoGrid(NavMenuItem navMenuItem, INameScope scope)
    {
        var infoGrid = base.BuildMenuItemInfoGrid(navMenuItem, scope);
        var indentConverter = new MarginMultiplierConverter
        {
            Left   = true,
            Indent = navMenuItem.InlineItemIndentUnit
        };
        CreateTemplateParentBinding(infoGrid, Grid.MarginProperty,
            NavMenuItem.LevelProperty, BindingMode.OneWay,
            indentConverter);

        return infoGrid;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        BuildMenuIndicatorStyle();
    }

    private void BuildMenuIndicatorStyle()
    {
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));
            var transformOptions   = new TransformOperations.Builder(1);
            transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
            menuIndicatorStyle.Add(Border.RenderTransformProperty, transformOptions.Build());
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, true);
            Add(menuIndicatorStyle);
        }
        var openSubMenuStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Open));
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));
            var transformOptions   = new TransformOperations.Builder(1);
            transformOptions.AppendRotate(MathUtils.Deg2Rad(-90));
            menuIndicatorStyle.Add(Border.RenderTransformProperty, transformOptions.Build());
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
}