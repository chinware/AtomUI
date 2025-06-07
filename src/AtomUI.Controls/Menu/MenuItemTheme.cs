using AtomUI.Animations;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
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
    public const string ToggleItemsLayoutPart = "PART_ToggleItemsLayout";
    public const string ToggleCheckboxPart = "PART_ToggleCheckboxPresenter";
    public const string ToggleRadioPart = "PART_ToggleRadio";
    public const string ItemIconPresenterPart = "PART_ItemIconPresenter";
    public const string ItemTextPresenterPart = "PART_ItemTextPresenter";
    public const string InputGestureTextPart = "PART_InputGestureText";
    public const string MenuIndicatorIconPart = "PART_MenuIndicatorIcon";
    public const string PopupPart = "PART_Popup";
    public const string PopupFramePart = "PART_PopupFrame";
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
                        SharedSizeGroup = "ToggleItemsLayout"
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

            var toggleItemsLayout = new Panel()
            {
                Name = ToggleItemsLayoutPart,
            };
            toggleItemsLayout.Bind(Panel.IsVisibleProperty, new MultiBinding()
            {
                Bindings =
                {
                    new Binding("IsTopLevel")
                    {
                        RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                    },
                    new TemplateBinding(MenuItem.ToggleTypeProperty)
                },
                Converter = new FuncMultiValueConverter<object?, bool>(objects =>
                {
                    var items      = objects.ToList();
                    if (items[0] is bool isTopLevel && items[1] is MenuItemToggleType toggleType)
                    {
                        return !isTopLevel && toggleType != MenuItemToggleType.None;
                    }

                    return false;
                })
            });
            CreateTemplateParentBinding(toggleItemsLayout, Panel.IsEnabledProperty,
                MenuItem.IsEnabledProperty);
            
            Grid.SetColumn(toggleItemsLayout, 0);
            toggleItemsLayout.RegisterInNameScope(scope);
            
            var toggleCheckbox = new CheckBox()
            {
                Name = ToggleCheckboxPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
            };
            CreateTemplateParentBinding(toggleCheckbox, CheckBox.IsCheckedProperty, MenuItem.IsCheckedProperty);
            CreateTemplateParentBinding(toggleCheckbox, CheckBox.IsVisibleProperty,
                MenuItem.ToggleTypeProperty, BindingMode.Default,
                new FuncValueConverter<MenuItemToggleType, bool>(type => type == MenuItemToggleType.CheckBox));
            toggleItemsLayout.Children.Add(toggleCheckbox);

            var toggleRadio = new RadioButton()
            {
                Name                = ToggleRadioPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
            };
            CreateTemplateParentBinding(toggleCheckbox, RadioButton.IsCheckedProperty, MenuItem.IsCheckedProperty);
            CreateTemplateParentBinding(toggleCheckbox, RadioButton.GroupNameProperty, MenuItem.GroupNameProperty);
            CreateTemplateParentBinding(toggleRadio, ContentPresenter.IsVisibleProperty,
                MenuItem.ToggleTypeProperty, BindingMode.Default,
                new FuncValueConverter<MenuItemToggleType, bool>(type => type == MenuItemToggleType.Radio));
            toggleItemsLayout.Children.Add(toggleRadio);
            
            var iconPresenter = new ContentPresenter()
            {
                Name                = ItemIconPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
            };

            Grid.SetColumn(iconPresenter, 1);
            iconPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, MenuItem.IconProperty);
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.IsVisibleProperty, MenuItem.IconProperty,
                BindingMode.Default, ObjectConverters.IsNotNull);

            var itemTextPresenter = new ContentPresenter
            {
                Name                = ItemTextPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Center,
                RecognizesAccessKey = true,
                IsHitTestVisible    = false
            };
            Grid.SetColumn(itemTextPresenter, 2);

            CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty,
                HeaderedSelectingItemsControl.HeaderProperty,
                BindingMode.Default,
                new FuncValueConverter<object?, object?>(
                    o =>
                    {
                        if (o is string str)
                        {
                            return new TextBlock
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

            var inputGestureText = new TextBlock
            {
                Name                = InputGestureTextPart,
                HorizontalAlignment = HorizontalAlignment.Right,
                TextAlignment       = TextAlignment.Right,
                VerticalAlignment   = VerticalAlignment.Center
            };
            Grid.SetColumn(inputGestureText, 3);

            CreateTemplateParentBinding(inputGestureText,
                TextBlock.TextProperty,
                Avalonia.Controls.MenuItem.InputGestureProperty,
                BindingMode.Default,
                MenuItem.KeyGestureConverter);

            inputGestureText.RegisterInNameScope(scope);

            var menuIndicatorIcon = AntDesignIconPackage.RightOutlined();
            menuIndicatorIcon.Name                = MenuIndicatorIconPart;
            menuIndicatorIcon.HorizontalAlignment = HorizontalAlignment.Right;
            menuIndicatorIcon.VerticalAlignment   = VerticalAlignment.Center;
            
            Grid.SetColumn(menuIndicatorIcon, 4);
            menuIndicatorIcon.RegisterInNameScope(scope);

            var popup = CreateMenuPopup(menuItem);
            popup.RegisterInNameScope(scope);

            layout.Children.Add(toggleItemsLayout);
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

        var border = new Border
        {
            Name = PopupFramePart
        };
        
        var scrollViewer = new MenuScrollViewer();
        scrollViewer.IsScrollChainingEnabled = false;
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        Grid.SetIsSharedSizeScope(itemsPresenter, true);
        scrollViewer.Content = itemsPresenter;
        border.Child         = scrollViewer;

        popup.Child = border;

        CreateTemplateParentBinding(popup, Avalonia.Controls.Primitives.Popup.IsOpenProperty,
            Avalonia.Controls.MenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);

        return popup;
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildMenuIndicatorStyle();
        BuildMenuIconStyle();
        BuildPopupStyle();
        BuildDisabledStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemColor);
        commonStyle.Add(TemplatedControl.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        commonStyle.Add(TemplatedControl.CursorProperty,
            new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        
        // 设置元素外间距
        var toggleItemsLayoutStyle = new Style(selector => selector.Nesting().Template().Name(ToggleItemsLayoutPart));
        toggleItemsLayoutStyle.Add(Layoutable.MarginProperty, MenuTokenKey.ItemMargin);
        commonStyle.Add(toggleItemsLayoutStyle);

        var inputGestureTextStyle = new Style(selector => selector.Nesting().Template().Name(InputGestureTextPart));
        inputGestureTextStyle.Add(Layoutable.MarginProperty, MenuTokenKey.ItemMargin);
        commonStyle.Add(inputGestureTextStyle);

        var iconPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
        iconPresenterStyle.Add(Layoutable.MarginProperty, MenuTokenKey.ItemMargin);
        commonStyle.Add(iconPresenterStyle);
        
        var itemTextPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemTextPresenterPart));
        itemTextPresenterStyle.Add(Layoutable.MarginProperty, MenuTokenKey.ItemMargin);
        commonStyle.Add(itemTextPresenterStyle);
        
        {
            var keyGestureStyle = new Style(selector => selector.Nesting().Template().Name(InputGestureTextPart));
            keyGestureStyle.Add(TextBlock.ForegroundProperty, MenuTokenKey.KeyGestureColor);
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
            var isMotionEnabledStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(MenuItem.IsMotionEnabledProperty, true));
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                BaseTransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
            }));
            isMotionEnabledStyle.Add(borderStyle);
            commonStyle.Add(isMotionEnabledStyle);
        }
        commonStyle.Add(hoverStyle);
        Add(commonStyle);
    }

    private void BuildMenuIndicatorStyle()
    {
        {
            var menuIndicatorStyle = new Style(selector => selector.Nesting().Template().Name(MenuIndicatorIconPart));
            menuIndicatorStyle.Add(Visual.IsVisibleProperty, true);
            menuIndicatorStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeXS);
            menuIndicatorStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeXS);
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
        var iconStyle = new Style(selector =>
            selector.Nesting().Template().Name(ItemIconPresenterPart).Descendant().OfType<Icon>());
        iconStyle.Add(Icon.WidthProperty, MenuTokenKey.ItemIconSize);
        iconStyle.Add(Icon.HeightProperty, MenuTokenKey.ItemIconSize);
        iconStyle.Add(Icon.NormalFilledBrushProperty, MenuTokenKey.ItemColor);
        Add(iconStyle);
    }

    private void BuildPopupStyle()
    {
        var popupFrameStyle = new Style(selector => selector.Nesting().Template().Name(PopupFramePart));
        popupFrameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        popupFrameStyle.Add(Border.CornerRadiusProperty, MenuTokenKey.MenuPopupBorderRadius);
        popupFrameStyle.Add(Layoutable.MinWidthProperty, MenuTokenKey.MenuPopupMinWidth);
        popupFrameStyle.Add(Layoutable.MaxWidthProperty, MenuTokenKey.MenuPopupMaxWidth);
        popupFrameStyle.Add(Layoutable.MinHeightProperty, MenuTokenKey.MenuPopupMinHeight);
        popupFrameStyle.Add(Layoutable.MaxHeightProperty, MenuTokenKey.MenuPopupMaxHeight);
        popupFrameStyle.Add(Decorator.PaddingProperty, MenuTokenKey.MenuPopupContentPadding);
        
        Add(popupFrameStyle);
        
        var popupStyle = new Style(selector => selector.Nesting().Template().Name(PopupPart));
        popupStyle.Add(Popup.MarginToAnchorProperty, MenuTokenKey.TopLevelItemPopupMarginToAnchor);
        popupStyle.Add(Popup.MaskShadowsProperty, MenuTokenKey.MenuPopupBoxShadows);
        Add(popupStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemDisabledColor);
        Add(disabledStyle);
    }
}