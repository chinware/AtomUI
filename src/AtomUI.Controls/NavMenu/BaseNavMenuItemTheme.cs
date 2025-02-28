using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
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
            NavMenuTokenKey.ItemMargin);
        TokenResourceBinder.CreateTokenBinding(iconPresenter, Layoutable.WidthProperty,
            NavMenuTokenKey.ItemIconSize);
        TokenResourceBinder.CreateTokenBinding(iconPresenter, Layoutable.HeightProperty,
            NavMenuTokenKey.ItemIconSize);

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
            NavMenuTokenKey.ItemMargin);
        CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty,
            HeaderedSelectingItemsControl.HeaderProperty);
        CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentTemplateProperty,
            HeaderedSelectingItemsControl.HeaderTemplateProperty);

        itemTextPresenter.RegisterInNameScope(scope);

        var inputGestureText = new SingleLineText()
        {
            Name                = InputGestureTextPart,
            HorizontalAlignment = HorizontalAlignment.Right,
            TextAlignment       = TextAlignment.Right,
            VerticalAlignment   = VerticalAlignment.Center
        };
        Grid.SetColumn(inputGestureText, 2);
        TokenResourceBinder.CreateTokenBinding(inputGestureText, Layoutable.MarginProperty,
            NavMenuTokenKey.ItemMargin);
        CreateTemplateParentBinding(inputGestureText,
            SingleLineText.TextProperty,
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
        var menuIndicatorIcon = AntDesignIconPackage.RightOutlined();
        menuIndicatorIcon.Name                = MenuIndicatorIconPart;
        menuIndicatorIcon.HorizontalAlignment = HorizontalAlignment.Right;
        menuIndicatorIcon.VerticalAlignment   = VerticalAlignment.Center;

        CreateTemplateParentBinding(menuIndicatorIcon, Icon.IsEnabledProperty, NavMenuItem.IsEnabledProperty);

        TokenResourceBinder.CreateTokenBinding(menuIndicatorIcon, Layoutable.WidthProperty,
            NavMenuTokenKey.MenuArrowSize);
        TokenResourceBinder.CreateTokenBinding(menuIndicatorIcon, Layoutable.HeightProperty,
            NavMenuTokenKey.MenuArrowSize);
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
        commonStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.ItemColor);
        {
            var keyGestureStyle = new Style(selector => selector.Nesting().Template().Name(InputGestureTextPart));
            keyGestureStyle.Add(SingleLineText.ForegroundProperty, NavMenuTokenKey.KeyGestureColor);
            commonStyle.Add(keyGestureStyle);
        }

        {
            // 动画设置
            var isMotionEnabledStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(NavMenuItem.IsMotionEnabledProperty, true));
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            borderStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty),
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(TemplatedControl.ForegroundProperty)
            }));
            isMotionEnabledStyle.Add(borderStyle);
            commonStyle.Add(isMotionEnabledStyle);
        }

        BuildHeaderDecorator(commonStyle);
        Add(commonStyle);
    }

    private void BuildHeaderDecorator(Style commonStyle)
    {
        // header 通用设置
        var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
        headerDecoratorStyle.Add(Border.CursorProperty,
            new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        headerDecoratorStyle.Add(Border.MinHeightProperty, NavMenuTokenKey.ItemHeight);
        headerDecoratorStyle.Add(Border.PaddingProperty, NavMenuTokenKey.ItemContentPadding);
        headerDecoratorStyle.Add(Border.CornerRadiusProperty, NavMenuTokenKey.ItemBorderRadius);
        commonStyle.Add(headerDecoratorStyle);
        BuildNormalHeaderDecoratorStyle(commonStyle);
        BuildDarkHeaderDecoratorStyle(commonStyle);
    }

    private void BuildNormalHeaderDecoratorStyle(Style commonStyle)
    {
        // 没有子菜单
        var hasNoSubMenuStyle = new Style(selector => selector
                                                      .Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, false));
        // 正常状态
        {
            var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.ItemColor);
            headerDecoratorStyle.Add(Border.BackgroundProperty, NavMenuTokenKey.ItemBg);
            hasNoSubMenuStyle.Add(headerDecoratorStyle);
        }

        // Hover 效果
        {
            var headerDecoratorStyle = new Style(selector =>
                selector.Nesting().Template().Name(HeaderDecoratorPart).Class(StdPseudoClass.PointerOver));
            headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.ItemHoverColor);
            headerDecoratorStyle.Add(TemplatedControl.BackgroundProperty, NavMenuTokenKey.ItemHoverBg);
            hasNoSubMenuStyle.Add(headerDecoratorStyle);
        }
        {
            var selectedStyle = new Style(selector => selector
                                                      .Nesting().Class(StdPseudoClass.Selected));
            // 选中状态
            {
                var headerDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
                headerDecoratorStyle.Add(Border.BackgroundProperty, NavMenuTokenKey.ItemSelectedBg);
                headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty,
                    NavMenuTokenKey.ItemSelectedColor);
                selectedStyle.Add(headerDecoratorStyle);
            }
            hasNoSubMenuStyle.Add(selectedStyle);
        }
        commonStyle.Add(hasNoSubMenuStyle);

        // 有子菜单
        var hasSubMenuStyle = new Style(selector => selector
                                                    .Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, true));
        // 正常状态
        {
            var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.ItemColor);
            headerDecoratorStyle.Add(Border.BackgroundProperty, NavMenuTokenKey.ItemBg);
            hasSubMenuStyle.Add(headerDecoratorStyle);
        }
        // Hover 效果
        {
            var headerDecoratorStyle = new Style(selector =>
                selector.Nesting().Template().Name(HeaderDecoratorPart).Class(StdPseudoClass.PointerOver));
            headerDecoratorStyle.Add(TemplatedControl.BackgroundProperty, NavMenuTokenKey.ItemHoverBg);
            hasSubMenuStyle.Add(headerDecoratorStyle);
        }
        {
            var selectedStyle = new Style(selector => selector
                                                      .Nesting().Class(StdPseudoClass.Selected));
            // 选中状态
            {
                var headerDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
                headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty,
                    NavMenuTokenKey.ItemSelectedColor);
                selectedStyle.Add(headerDecoratorStyle);
            }
            hasSubMenuStyle.Add(selectedStyle);
        }
        commonStyle.Add(hasSubMenuStyle);
    }

    private void BuildDarkHeaderDecoratorStyle(Style commonStyle)
    {
        var darkCommonStyle =
            new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.IsDarkStyleProperty, true));
        // 没有子菜单
        var hasNoSubMenuStyle = new Style(selector => selector
                                                      .Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, false));
        // 正常状态
        {
            var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.DarkItemColor);
            headerDecoratorStyle.Add(Border.BackgroundProperty, NavMenuTokenKey.DarkItemBg);
            hasNoSubMenuStyle.Add(headerDecoratorStyle);
        }
        // Hover 效果
        {
            var headerDecoratorStyle = new Style(selector =>
                selector.Nesting().Template().Name(HeaderDecoratorPart).Class(StdPseudoClass.PointerOver));
            headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.DarkItemHoverColor);
            headerDecoratorStyle.Add(TemplatedControl.BackgroundProperty, NavMenuTokenKey.DarkItemHoverBg);
            hasNoSubMenuStyle.Add(headerDecoratorStyle);
        }
        {
            var selectedStyle = new Style(selector => selector
                                                      .Nesting().Class(StdPseudoClass.Selected));
            // 选中状态
            {
                var headerDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
                headerDecoratorStyle.Add(Border.BackgroundProperty, NavMenuTokenKey.DarkItemSelectedBg);
                headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty,
                    NavMenuTokenKey.DarkItemSelectedColor);
                selectedStyle.Add(headerDecoratorStyle);
            }
            hasNoSubMenuStyle.Add(selectedStyle);
        }
        darkCommonStyle.Add(hasNoSubMenuStyle);

        // 有子菜单
        var hasSubMenuStyle = new Style(selector => selector
                                                    .Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, true));
        // 正常状态
        {
            var headerDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
            headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.DarkItemColor);
            headerDecoratorStyle.Add(Border.BackgroundProperty, NavMenuTokenKey.DarkItemBg);
            hasSubMenuStyle.Add(headerDecoratorStyle);
        }
        // Hover 效果
        {
            var headerDecoratorStyle = new Style(selector =>
                selector.Nesting().Template().Name(HeaderDecoratorPart).Class(StdPseudoClass.PointerOver));
            headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.DarkItemHoverColor);
            hasSubMenuStyle.Add(headerDecoratorStyle);
        }
        {
            var selectedStyle = new Style(selector => selector
                                                      .Nesting().Class(StdPseudoClass.Selected));
            // 选中状态
            {
                var headerDecoratorStyle =
                    new Style(selector => selector.Nesting().Template().Name(HeaderDecoratorPart));
                headerDecoratorStyle.Add(TemplatedControl.ForegroundProperty,
                    NavMenuTokenKey.DarkItemSelectedColor);
                selectedStyle.Add(headerDecoratorStyle);
            }
            hasSubMenuStyle.Add(selectedStyle);
        }
        darkCommonStyle.Add(hasSubMenuStyle);
        commonStyle.Add(darkCommonStyle);
    }

    private void BuildMenuIndicatorStyle()
    {
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, true);
            menuIndicatorStyle.Add(Icon.NormalFilledBrushProperty, NavMenuTokenKey.ItemColor);
            menuIndicatorStyle.Add(Icon.SelectedFilledBrushProperty, NavMenuTokenKey.ItemSelectedColor);
            menuIndicatorStyle.Add(Icon.DisabledFilledBrushProperty, NavMenuTokenKey.ItemDisabledColor);
            // 设置颜色

            Add(menuIndicatorStyle);
        }
        {
            var darkCommonStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(NavMenuItem.IsDarkStyleProperty, true));
            {
                var menuIndicatorStyle =
                    new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
                menuIndicatorStyle.Add(Icon.NormalFilledBrushProperty, NavMenuTokenKey.DarkItemColor);
                menuIndicatorStyle.Add(Icon.SelectedFilledBrushProperty, NavMenuTokenKey.DarkItemSelectedColor);
                menuIndicatorStyle.Add(Icon.DisabledFilledBrushProperty, NavMenuTokenKey.DarkItemDisabledColor);
                darkCommonStyle.Add(menuIndicatorStyle);
            }
            Add(darkCommonStyle);
        }

        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
            menuIndicatorStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            selectedStyle.Add(menuIndicatorStyle);
        }
        Add(selectedStyle);

        var hasNoSubMenuStyle =
            new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.HasSubMenuProperty, false));
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
            var iconContentPresenterStyle =
                new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconContentPresenterStyle.Add(Visual.IsVisibleProperty, false);
            Add(iconContentPresenterStyle);
        }

        var hasIconStyle = new Style(selector => selector.Nesting().Class(":icon"));
        {
            var iconContentPresenterStyle =
                new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconContentPresenterStyle.Add(Visual.IsVisibleProperty, true);
            hasIconStyle.Add(iconContentPresenterStyle);
        }
        Add(hasIconStyle);
    }

    private void BuildDisabledStyle()
    {
        {
            var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
            disabledStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.ItemDisabledColor);
            Add(disabledStyle);
        }
        var darkStyle = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.IsDarkStyleProperty, true));
        {
            var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
            disabledStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.DarkItemDisabledColor);
            darkStyle.Add(disabledStyle);
        }
        Add(darkStyle);
    }
}