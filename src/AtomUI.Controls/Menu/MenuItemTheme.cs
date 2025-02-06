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
        return new FuncControlTemplate<MenuItem>((item, scope) =>
        {
            // 仅仅为了把 Popup 包进来，没有其他什么作用
            var layoutWrapper = new Panel();
            var container = new Border
            {
                Name = ItemDecoratorPart
            };

            var transitions = new Transitions();
            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty));
            container.Transitions = transitions;

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
            Grid.SetColumn(togglePresenter, 0);
            togglePresenter.RegisterInNameScope(scope);

            var iconPresenter = new Viewbox
            {
                Name                = ItemIconPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Stretch             = Stretch.Uniform
            };

            Grid.SetColumn(iconPresenter, 1);
            iconPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(iconPresenter, Viewbox.ChildProperty, MenuItem.IconProperty);
            TokenResourceBinder.CreateTokenBinding(iconPresenter, Layoutable.MarginProperty,
                MenuTokenResourceKey.ItemMargin);
            TokenResourceBinder.CreateGlobalTokenBinding(iconPresenter, Layoutable.WidthProperty,
                MenuTokenResourceKey.ItemIconSize);
            TokenResourceBinder.CreateGlobalTokenBinding(iconPresenter, Layoutable.HeightProperty,
                MenuTokenResourceKey.ItemIconSize);

            var itemTextPresenter = new ContentPresenter
            {
                Name                = ItemTextPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Center,
                RecognizesAccessKey = true,
                IsHitTestVisible    = false
            };
            Grid.SetColumn(itemTextPresenter, 2);
            TokenResourceBinder.CreateTokenBinding(itemTextPresenter, Layoutable.MarginProperty,
                MenuTokenResourceKey.ItemMargin);
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
            Grid.SetColumn(inputGestureText, 3);
            TokenResourceBinder.CreateTokenBinding(inputGestureText, Layoutable.MarginProperty,
                MenuTokenResourceKey.ItemMargin);
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
            
            TokenResourceBinder.CreateGlobalTokenBinding(menuIndicatorIcon, Layoutable.WidthProperty,
                DesignTokenKey.IconSizeXS);
            TokenResourceBinder.CreateGlobalTokenBinding(menuIndicatorIcon, Layoutable.HeightProperty,
                DesignTokenKey.IconSizeXS);
            Grid.SetColumn(menuIndicatorIcon, 4);
            menuIndicatorIcon.RegisterInNameScope(scope);

            var popup = CreateMenuPopup();
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

    private Popup CreateMenuPopup()
    {
        var popup = new Popup
        {
            Name                       = PopupPart,
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
            Placement                  = PlacementMode.RightEdgeAlignedTop
        };

        var border = new Border();
        TokenResourceBinder.CreateTokenBinding(border, Border.BackgroundProperty,
            DesignTokenKey.ColorBgContainer);
        TokenResourceBinder.CreateTokenBinding(border, Border.CornerRadiusProperty,
            MenuTokenResourceKey.MenuPopupBorderRadius);
        TokenResourceBinder.CreateTokenBinding(border, Layoutable.MinWidthProperty,
            MenuTokenResourceKey.MenuPopupMinWidth);
        TokenResourceBinder.CreateTokenBinding(border, Layoutable.MaxWidthProperty,
            MenuTokenResourceKey.MenuPopupMaxWidth);
        TokenResourceBinder.CreateTokenBinding(border, Layoutable.MinHeightProperty,
            MenuTokenResourceKey.MenuPopupMinHeight);
        TokenResourceBinder.CreateTokenBinding(border, Layoutable.MaxHeightProperty,
            MenuTokenResourceKey.MenuPopupMaxHeight);
        TokenResourceBinder.CreateTokenBinding(border, Decorator.PaddingProperty,
            MenuTokenResourceKey.MenuPopupContentPadding);

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

        TokenResourceBinder.CreateTokenBinding(popup, Popup.MarginToAnchorProperty,
            MenuTokenResourceKey.TopLevelItemPopupMarginToAnchor);
        TokenResourceBinder.CreateTokenBinding(popup, Popup.MaskShadowsProperty,
            MenuTokenResourceKey.MenuPopupBoxShadows);
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
        commonStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenResourceKey.ItemColor);
        {
            var keyGestureStyle = new Style(selector => selector.Nesting().Template().Name(InputGestureTextPart));
            keyGestureStyle.Add(TextBlock.ForegroundProperty, MenuTokenResourceKey.KeyGestureColor);
            commonStyle.Add(keyGestureStyle);
        }
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Layoutable.MinHeightProperty, MenuTokenResourceKey.ItemHeight);
            borderStyle.Add(Decorator.PaddingProperty, MenuTokenResourceKey.ItemPaddingInline);
            borderStyle.Add(Border.BackgroundProperty, MenuTokenResourceKey.ItemBg);
            borderStyle.Add(Border.CornerRadiusProperty, MenuTokenResourceKey.ItemBorderRadius);
            commonStyle.Add(borderStyle);
        }

        // Hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenResourceKey.ItemHoverColor);
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Border.BackgroundProperty, MenuTokenResourceKey.ItemHoverBg);
            hoverStyle.Add(borderStyle);
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
            var iconViewBoxStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconViewBoxStyle.Add(Visual.IsVisibleProperty, false);
            Add(iconViewBoxStyle);
        }

        var hasIconStyle = new Style(selector => selector.Nesting().Class(":icon"));
        {
            var iconViewBoxStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconViewBoxStyle.Add(Visual.IsVisibleProperty, true);
            hasIconStyle.Add(iconViewBoxStyle);
        }
        Add(hasIconStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenResourceKey.ItemDisabledColor);
        Add(disabledStyle);
    }
}