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
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuItemTheme : BaseControlTheme
{
    public const string ItemDecoratorPart = "PART_ItemDecorator";
    public const string MainContainerPart = "PART_MainContainer";
    public const string TogglePresenterPart = "PART_TogglePresenter";
    public const string ItemIconPresenterPart = "PART_ItemIconPresenter";
    public const string ItemTextPresenterPart = "PART_ItemTextPresenter";
    public const string InputGestureTextPart = "PART_InputGestureText";
    public const string MenuIndicatorIconPart = "PART_MenuIndicatorIcon";
    public const string PopupPart = "PART_Popup";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public MenuItemTheme()
        : base(typeof(MenuItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<MenuItem>((menuItem, scope) =>
        {
            // 仅仅为了把 Popup 包进来，没有其他什么作用
            var layoutWrapper = new Panel();
            var container = new Border
            {
                Name = ItemDecoratorPart
            };

            var layout = new Grid
            {
                Name = MainContainerPart,
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto)
                    {
                        SharedSizeGroup = "TogglePresenter"
                    },
                    new ColumnDefinition(GridLength.Auto)
                    {
                        SharedSizeGroup = "IconPresenter"
                    },
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                    {
                        SharedSizeGroup = "InputGestureText"
                    },
                    new ColumnDefinition(GridLength.Auto)
                    {
                        SharedSizeGroup = "MenuIndicatorIcon"
                    }
                }
            };
            layout.RegisterInNameScope(scope);

            var togglePresenter = new ContentControl
            {
                Name                = TogglePresenterPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                IsVisible           = false
            };
            CreateTemplateParentBinding(togglePresenter, InputElement.IsEnabledProperty,
                InputElement.IsEnabledProperty);
            menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(togglePresenter, Layoutable.MarginProperty,
                MenuTokenKey.ItemMargin));
            Grid.SetColumn(togglePresenter, 0);
            togglePresenter.RegisterInNameScope(scope);

            var iconPresenter = new ContentControl()
            {
                Name                = ItemIconPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
            };

            Grid.SetColumn(iconPresenter, 1);
            iconPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(iconPresenter, ContentControl.ContentProperty, MenuItem.IconProperty);
            menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(iconPresenter, Layoutable.MarginProperty,
                MenuTokenKey.ItemMargin));
            menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(iconPresenter, Layoutable.WidthProperty,
                MenuTokenKey.ItemIconSize));
            menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(iconPresenter, Layoutable.HeightProperty,
                MenuTokenKey.ItemIconSize));

            var itemTextPresenter = new ContentPresenter
            {
                Name                = ItemTextPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Center,
                RecognizesAccessKey = true,
                IsHitTestVisible    = false
            };
            Grid.SetColumn(itemTextPresenter, 2);
            menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(itemTextPresenter, Layoutable.MarginProperty,
                MenuTokenKey.ItemMargin));
            CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty,
                HeaderedSelectingItemsControl.HeaderProperty,
                BindingMode.Default,
                new FuncValueConverter<object?, object?>(
                    o =>
                    {
                        if (o is string str)
                        {
                            return new SingleLineText()
                            {
                                Text              = str,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }

                        return o;
                    }));
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
            Grid.SetColumn(inputGestureText, 3);
            menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(inputGestureText, Layoutable.MarginProperty,
                MenuTokenKey.ItemMargin));
            CreateTemplateParentBinding(inputGestureText,
                SingleLineText.TextProperty,
                Avalonia.Controls.MenuItem.InputGestureProperty,
                BindingMode.Default,
                MenuItem.KeyGestureConverter);

            inputGestureText.RegisterInNameScope(scope);

            var menuIndicatorIcon = AntDesignIconPackage.RightOutlined();
            menuIndicatorIcon.Name                = MenuIndicatorIconPart;
            menuIndicatorIcon.HorizontalAlignment = HorizontalAlignment.Right;
            menuIndicatorIcon.VerticalAlignment   = VerticalAlignment.Center;

            menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(menuIndicatorIcon, Layoutable.WidthProperty,
                SharedTokenKey.IconSizeXS));
            menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(menuIndicatorIcon, Layoutable.HeightProperty,
                SharedTokenKey.IconSizeXS));
            Grid.SetColumn(menuIndicatorIcon, 4);
            menuIndicatorIcon.RegisterInNameScope(scope);

            var popup = CreateMenuPopup(menuItem);
            popup.RegisterInNameScope(scope);

            layout.Children.Add(togglePresenter);
            layout.Children.Add(iconPresenter);
            layout.Children.Add(itemTextPresenter);
            layout.Children.Add(inputGestureText);
            layout.Children.Add(menuIndicatorIcon);
            layout.Children.Add(popup);

            container.Child = layout;
            layoutWrapper.Children.Add(container);
            return layoutWrapper;
        });
    }

    private Popup CreateMenuPopup(MenuItem menuItem)
    {
        var popup = new Popup
        {
            Name                       = PopupPart,
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
            Placement                  = PlacementMode.RightEdgeAlignedTop
        };

        var border = new Border();
        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border, Border.BackgroundProperty,
            SharedTokenKey.ColorBgContainer));
        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border, Border.CornerRadiusProperty,
            MenuTokenKey.MenuPopupBorderRadius));
        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border, Layoutable.MinWidthProperty,
            MenuTokenKey.MenuPopupMinWidth));
        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border, Layoutable.MaxWidthProperty,
            MenuTokenKey.MenuPopupMaxWidth));
        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border, Layoutable.MinHeightProperty,
            MenuTokenKey.MenuPopupMinHeight));
        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border, Layoutable.MaxHeightProperty,
            MenuTokenKey.MenuPopupMaxHeight));
        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border, Decorator.PaddingProperty,
            MenuTokenKey.MenuPopupContentPadding));

        var scrollViewer = new MenuScrollViewer();
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        Grid.SetIsSharedSizeScope(itemsPresenter, true);
        scrollViewer.Content = itemsPresenter;
        border.Child         = scrollViewer;

        popup.Child = border;

        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(popup, Popup.MarginToAnchorProperty,
            MenuTokenKey.TopLevelItemPopupMarginToAnchor));
        menuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(popup, Popup.MaskShadowsProperty,
            MenuTokenKey.MenuPopupBoxShadows));
        CreateTemplateParentBinding(popup, Avalonia.Controls.Primitives.Popup.IsOpenProperty,
            Avalonia.Controls.MenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);

        return popup;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        BuildCommonStyle(commonStyle);
        BuildMenuIndicatorStyle();
        BuildMenuIconStyle();
        Add(commonStyle);
        BuildDisabledStyle();
    }

    private void BuildCommonStyle(Style commonStyle)
    {
        commonStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemColor);
        commonStyle.Add(TemplatedControl.CursorProperty,
            new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        {
            var keyGestureStyle = new Style(selector => selector.Nesting().Template().Name(InputGestureTextPart));
            keyGestureStyle.Add(SingleLineText.ForegroundProperty, MenuTokenKey.KeyGestureColor);
            commonStyle.Add(keyGestureStyle);
        }
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Layoutable.MinHeightProperty, MenuTokenKey.ItemHeight);
            borderStyle.Add(Decorator.PaddingProperty, MenuTokenKey.ItemPaddingInline);
            borderStyle.Add(Border.BackgroundProperty, MenuTokenKey.ItemBg);
            borderStyle.Add(Border.CornerRadiusProperty, MenuTokenKey.ItemBorderRadius);
            commonStyle.Add(borderStyle);
        }

        // Hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemHoverColor);
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Border.BackgroundProperty, MenuTokenKey.ItemHoverBg);
            hoverStyle.Add(borderStyle);
        }
        
        // 动画设置
        {
            var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.IsMotionEnabledProperty, true));
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
            }));
            isMotionEnabledStyle.Add(borderStyle);
            commonStyle.Add(isMotionEnabledStyle);
        }
        commonStyle.Add(hoverStyle);
    }

    private void BuildMenuIndicatorStyle()
    {
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, true);
            Add(menuIndicatorStyle);
        }
        var hasSubMenuStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Empty));
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, false);
            hasSubMenuStyle.Add(menuIndicatorStyle);
        }
        Add(hasSubMenuStyle);
    }

    private void BuildMenuIconStyle()
    {
        {
            var iconPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconPresenterStyle.Add(Visual.IsVisibleProperty, false);
            Add(iconPresenterStyle);
        }

        var hasIconStyle = new Style(selector => selector.Nesting().Class(":icon"));
        {
            var iconPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconPresenterStyle.Add(Visual.IsVisibleProperty, true);
            hasIconStyle.Add(iconPresenterStyle);
        }
        Add(hasIconStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemDisabledColor);
        Add(disabledStyle);
    }
}