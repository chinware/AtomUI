using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class BaseNavMenuItemTheme : BaseControlTheme
{
    public const string HeaderDecoratorPart = "PART_HeaderDecorator";
    public const string MainContainerPart = "PART_MainContainer";
    public const string ItemIconPresenterPart = "PART_ItemIconPresenter";
    public const string ItemTextPresenterPart = "PART_ItemTextPresenter";
    public const string InputGestureTextPart = "PART_InputGestureText";
    public const string MenuIndicatorIconPart = "PART_MenuIndicatorIcon";
    
    protected BaseNavMenuItemTheme(Type targetType) : base(targetType)
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<NavMenuItem>((item, scope) =>
        {
            // 仅仅为了把 Popup 包进来，没有其他什么作用
            var layoutWrapper = new Panel();
            var header        = BuildMenuItemContent(item, scope);
            BuildExtraItem(layoutWrapper, scope);
            layoutWrapper.Children.Add(header);
            return layoutWrapper;
        });
    }

    protected virtual Control BuildMenuItemContent(NavMenuItem navMenuItem, INameScope scope)
    {
        var headerFrame = new Border
        {
            Name = HeaderDecoratorPart,
            Transitions = new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty),
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(TemplatedControl.ForegroundProperty)
            }
        };
        headerFrame.RegisterInNameScope(scope);
        headerFrame.Child = BuildMenuItemInfoGrid(navMenuItem, scope);
        return headerFrame;
    }

    protected virtual Grid BuildMenuItemInfoGrid(NavMenuItem navMenuItem, INameScope scope)
    {
        var layout = new Grid
        {
            Name = MainContainerPart,
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto)
                {
                    SharedSizeGroup = ThemeConstants.IconPresenterSizeGroup
                },
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
                {
                    SharedSizeGroup = ThemeConstants.InputGestureTextSizeGroup
                },
                new ColumnDefinition(GridLength.Auto)
                {
                    SharedSizeGroup = ThemeConstants.MenuIndicatorIconSizeGroup
                }
            }
        };
        layout.RegisterInNameScope(scope);

        var iconPresenter = new ContentPresenter()
        {
            Name                = ItemIconPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
        };

        Grid.SetColumn(iconPresenter, 0);
        iconPresenter.RegisterInNameScope(scope);
        CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, NavMenuItem.IconProperty);
        TokenResourceBinder.CreateTokenBinding(iconPresenter, Layoutable.MarginProperty,
            NavMenuTokenResourceKey.ItemMargin);
        TokenResourceBinder.CreateGlobalTokenBinding(iconPresenter, Layoutable.WidthProperty,
            NavMenuTokenResourceKey.ItemIconSize);
        TokenResourceBinder.CreateGlobalTokenBinding(iconPresenter, Layoutable.HeightProperty,
            NavMenuTokenResourceKey.ItemIconSize);

        var itemTextPresenter = new ContentPresenter
        {
            Name                = ItemTextPresenterPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Center,
            RecognizesAccessKey = true,
            IsHitTestVisible    = false
        };
        Grid.SetColumn(itemTextPresenter, 1);
        TokenResourceBinder.CreateTokenBinding(itemTextPresenter, Layoutable.MarginProperty,
            NavMenuTokenResourceKey.ItemMargin);
        CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty,
            HeaderedSelectingItemsControl.HeaderProperty);
        CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentTemplateProperty,
            HeaderedSelectingItemsControl.HeaderTemplateProperty);

        itemTextPresenter.RegisterInNameScope(scope);

        var inputGestureText = new TextBlock
        {
            Name                = InputGestureTextPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            TextAlignment       = TextAlignment.Right,
            VerticalAlignment   = VerticalAlignment.Center
        };
        Grid.SetColumn(inputGestureText, 2);
        TokenResourceBinder.CreateTokenBinding(inputGestureText, Layoutable.MarginProperty,
            NavMenuTokenResourceKey.ItemMargin);
        CreateTemplateParentBinding(inputGestureText,
            TextBlock.TextProperty,
            NavMenuItem.InputGestureProperty,
            BindingMode.Default,
            NavMenuItem.KeyGestureConverter);

        inputGestureText.RegisterInNameScope(scope);

        var menuIndicatorIcon = BuildMenuIndicatorIcon(scope);
        Grid.SetColumn(menuIndicatorIcon, 3);
            
        layout.Children.Add(iconPresenter);
        layout.Children.Add(itemTextPresenter);
        layout.Children.Add(inputGestureText);
        layout.Children.Add(menuIndicatorIcon);
        return layout;
    }

    protected virtual void BuildExtraItem(Panel layout, INameScope scope)
    {
    }

    protected virtual Control BuildMenuIndicatorIcon(INameScope scope)
    {
        var menuIndicatorIcon = new PathIcon
        {
            Name                = MenuIndicatorIconPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment   = VerticalAlignment.Center,
            Kind                = "RightOutlined"
        };

        CreateTemplateParentBinding(menuIndicatorIcon, PathIcon.IsEnabledProperty, NavMenuItem.IsEnabledProperty);
        
        TokenResourceBinder.CreateGlobalTokenBinding(menuIndicatorIcon, Layoutable.WidthProperty,
            NavMenuTokenResourceKey.MenuArrowSize);
        TokenResourceBinder.CreateGlobalTokenBinding(menuIndicatorIcon, Layoutable.HeightProperty,
            NavMenuTokenResourceKey.MenuArrowSize);
        menuIndicatorIcon.RegisterInNameScope(scope);
        
        return menuIndicatorIcon;
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildMenuIndicatorStyle();
        BuildMenuIconStyle();
        BuildDisabledStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.ItemColor);
        {
            var keyGestureStyle = new Style(selector => selector.Nesting().Template().Name(InputGestureTextPart));
            keyGestureStyle.Add(TextBlock.ForegroundProperty, NavMenuTokenResourceKey.KeyGestureColor);
            commonStyle.Add(keyGestureStyle);
        }
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            borderStyle.Add(Border.CursorProperty, new Cursor(StandardCursorType.Hand));
            borderStyle.Add(Border.MinHeightProperty, NavMenuTokenResourceKey.ItemHeight);
            borderStyle.Add(Border.PaddingProperty, NavMenuTokenResourceKey.ItemContentPadding);
            borderStyle.Add(Border.BackgroundProperty, NavMenuTokenResourceKey.ItemBg);
            borderStyle.Add(Border.CornerRadiusProperty, NavMenuTokenResourceKey.ItemBorderRadius);
            commonStyle.Add(borderStyle);
        }

        // Hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart).Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.ItemHoverColor);
        hoverStyle.Add(Border.BackgroundProperty, NavMenuTokenResourceKey.ItemHoverBg);
        commonStyle.Add(hoverStyle);
        
        // 选中分两种，一种是有子菜单一种是没有子菜单
        var hasNoSubMenuStyle  = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, false));
        {
            var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
            {
                var itemDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
                itemDecoratorStyle.Add(Border.BackgroundProperty, NavMenuTokenResourceKey.ItemSelectedBg);
                itemDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.ItemSelectedColor);
                selectedStyle.Add(itemDecoratorStyle);
            }
            hasNoSubMenuStyle.Add(selectedStyle);
        }
        commonStyle.Add(hasNoSubMenuStyle);
        
        var hasSubMenuStyle  = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, true));
        {
            var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
            {
                var itemDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
                itemDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.ItemSelectedColor);
                selectedStyle.Add(itemDecoratorStyle);
            }
            hasSubMenuStyle.Add(selectedStyle);
        }
        commonStyle.Add(hasSubMenuStyle);
        Add(commonStyle);
        
        BuildDarkCommonStyle();
    }

    private void BuildDarkCommonStyle()
    {
        var darkCommonStyle = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.IsDarkStyleProperty, true));
        darkCommonStyle.Add(NavMenuItem.ForegroundProperty, NavMenuTokenResourceKey.DarkItemColor);
        
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            borderStyle.Add(Border.BackgroundProperty, NavMenuTokenResourceKey.DarkItemBg);
            darkCommonStyle.Add(borderStyle);
        }
        
        // 选中分两种，一种是有子菜单一种是没有子菜单
        var hasNoSubMenuStyle  = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, false));
        {
            // Hover 状态
            var hoverStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart).Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.DarkItemHoverColor);
            hoverStyle.Add(Border.BackgroundProperty, NavMenuTokenResourceKey.DarkItemHoverBg);
            hasNoSubMenuStyle.Add(hoverStyle);
            
            var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
            {
                var itemDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
                itemDecoratorStyle.Add(Border.BackgroundProperty, NavMenuTokenResourceKey.DarkItemSelectedBg);
                itemDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.DarkItemSelectedColor);
                selectedStyle.Add(itemDecoratorStyle);
            }
            hasNoSubMenuStyle.Add(selectedStyle);
        }
        darkCommonStyle.Add(hasNoSubMenuStyle);
        
        var hasSubMenuStyle  = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, true));
        {
            // Hover 状态
            var hoverStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart).Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.DarkItemColor);
            hoverStyle.Add(Border.BackgroundProperty, NavMenuTokenResourceKey.DarkItemBg);
            hasSubMenuStyle.Add(hoverStyle);
            
            var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
            {
                var itemDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
                itemDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.DarkItemSelectedColor);
                selectedStyle.Add(itemDecoratorStyle);
            }
            hasSubMenuStyle.Add(selectedStyle);
        }
        darkCommonStyle.Add(hasSubMenuStyle);
        
        Add(darkCommonStyle);
    }

    private void BuildMenuIndicatorStyle()
    {
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, true);
            menuIndicatorStyle.Add(PathIcon.NormalFilledBrushProperty, NavMenuTokenResourceKey.ItemColor);
            menuIndicatorStyle.Add(PathIcon.SelectedFilledBrushProperty, NavMenuTokenResourceKey.ItemSelectedColor);
            menuIndicatorStyle.Add(PathIcon.DisabledFilledBrushProperty, NavMenuTokenResourceKey.ItemDisabledColor);
            // 设置颜色
            
            Add(menuIndicatorStyle);
        }
        {
            var darkCommonStyle = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.IsDarkStyleProperty, true));
            {
                var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
                menuIndicatorStyle.Add(PathIcon.NormalFilledBrushProperty, NavMenuTokenResourceKey.DarkItemColor);
                menuIndicatorStyle.Add(PathIcon.SelectedFilledBrushProperty, NavMenuTokenResourceKey.DarkItemSelectedColor);
                menuIndicatorStyle.Add(PathIcon.DisabledFilledBrushProperty, NavMenuTokenResourceKey.DarkItemDisabledColor);
                darkCommonStyle.Add(menuIndicatorStyle);
            }
            Add(darkCommonStyle);
        }

        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
            menuIndicatorStyle.Add(PathIcon.IconModeProperty, IconMode.Selected);
            selectedStyle.Add(menuIndicatorStyle);
        }
        Add(selectedStyle);
        
        var hasNoSubMenuStyle = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, false));
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, false);
            hasNoSubMenuStyle.Add(menuIndicatorStyle);
        }
        Add(hasNoSubMenuStyle);
    }

    private void BuildMenuIconStyle()
    {
        {
            var iconContentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconContentPresenterStyle.Add(Visual.IsVisibleProperty, false);
            Add(iconContentPresenterStyle);
        }

        var hasIconStyle = new Style(selector => selector.Nesting().Class(":icon"));
        {
            var iconContentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconContentPresenterStyle.Add(Visual.IsVisibleProperty, true);
            hasIconStyle.Add(iconContentPresenterStyle);
        }
        Add(hasIconStyle);
    }

    private void BuildDisabledStyle()
    {
        {
            var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
            disabledStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.ItemDisabledColor);
            Add(disabledStyle);
        }
        var darkStyle = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.IsDarkStyleProperty, true));
        {
            var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
            disabledStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.DarkItemDisabledColor);
            darkStyle.Add(disabledStyle);
        }
        Add(darkStyle);
    }
    
}