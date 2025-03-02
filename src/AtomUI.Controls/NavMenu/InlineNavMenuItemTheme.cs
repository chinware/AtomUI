using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class InlineNavMenuItemTheme : BaseNavMenuItemTheme
{
    public const string ID = "InlineNavMenuItem";
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

    protected override Control BuildMenuIndicatorIcon(NavMenuItem navMenuItem, INameScope scope)
    {
        var indicatorIcon = base.BuildMenuIndicatorIcon(navMenuItem, scope);
        var menuIndicatorIconPresenter = new Border()
        {
            Name = MenuIndicatorIconLayoutPart
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

        var childItemsLayoutTransform = new MotionActorControl()
        {
            Name = ChildItemsLayoutTransformPart,
        };
        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(childItemsLayoutTransform,
            MotionActorControl.MarginProperty,
            NavMenuTokenKey.VerticalItemsPanelSpacing, BindingPriority.Template,
            (v) =>
            {
                if (v is double dval)
                {
                    return new Thickness(0, dval, 0, 0);
                }

                return new Thickness();
            }));
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
            NavMenuItem.LevelProperty,
            BindingMode.OneWay,
            indentConverter);

        return infoGrid;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        BuildMenuIndicatorStyle();

        var itemsPanelStyle = new Style(selector =>
            selector.Nesting().Template().Name(ChildItemsPresenterPart).Child().OfType<StackPanel>());
        itemsPanelStyle.Add(StackPanel.SpacingProperty, NavMenuTokenKey.VerticalItemsPanelSpacing);
        Add(itemsPanelStyle);
    }

    private void BuildMenuIndicatorStyle()
    {
        {
            // 动画设置
            var isMotionEnabledStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(NavMenuItem.IsMotionEnabledProperty, true));
            var menuIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));
            menuIndicatorStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() =>
                new Transitions
                {
                    AnimationUtils.CreateTransition<TransformOperationsTransition>(ContentPresenter
                        .RenderTransformProperty)
                }));
            isMotionEnabledStyle.Add(menuIndicatorStyle);
            Add(isMotionEnabledStyle);
        }
        {
            var menuIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));

            menuIndicatorStyle.Add(Border.RenderTransformProperty, new SetterValueFactory<ITransform>(() =>
            {
                var transformOptions = new TransformOperations.Builder(1);
                transformOptions.AppendRotate(MathUtils.Deg2Rad(90));
                return transformOptions.Build();
            }));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, true);
            Add(menuIndicatorStyle);
        }
        var openSubMenuStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Open));
        {
            var menuIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));

            menuIndicatorStyle.Add(Border.RenderTransformProperty, new SetterValueFactory<ITransform>(() =>
            {
                var transformOptions = new TransformOperations.Builder(1);
                transformOptions.AppendRotate(MathUtils.Deg2Rad(-90));
                return transformOptions.Build();
            }));
            openSubMenuStyle.Add(menuIndicatorStyle);
        }
        Add(openSubMenuStyle);
        var emptySubMenuStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Empty));
        {
            var menuIndicatorStyle =
                new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconLayoutPart));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, false);
            emptySubMenuStyle.Add(menuIndicatorStyle);
        }
        Add(emptySubMenuStyle);
    }
}