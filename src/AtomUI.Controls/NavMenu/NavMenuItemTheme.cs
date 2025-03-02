﻿using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NavMenuItemTheme : BaseNavMenuItemTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string PopupFramePart = "PART_PopupFrame";

    public NavMenuItemTheme()
        : base(typeof(NavMenuItem))
    {
    }

    protected NavMenuItemTheme(Type targetType) : base(targetType)
    {
    }

    protected override void BuildExtraItem(NavMenuItem navMenuItem, Panel layout, INameScope scope)
    {
        var popup = CreateMenuPopup(navMenuItem);
        popup.RegisterInNameScope(scope);
        layout.Children.Add(popup);
    }

    protected Popup CreateMenuPopup(NavMenuItem navMenuItem)
    {
        var popup = new Popup
        {
            Name                       = ThemeConstants.PopupPart,
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
            Placement                  = PlacementMode.RightEdgeAlignedTop,
        };

        var border = new Border()
        {
            Name = PopupFramePart
        };

        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(popup,
            Popup.MarginToAnchorProperty,
            NavMenuTokenKey.TopLevelItemPopupMarginToAnchor));
        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border,
            Border.CornerRadiusProperty,
            NavMenuTokenKey.MenuPopupBorderRadius));
        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border,
            Layoutable.MinWidthProperty,
            NavMenuTokenKey.MenuPopupMinWidth));
        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border,
            Layoutable.MaxWidthProperty,
            NavMenuTokenKey.MenuPopupMaxWidth));
        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border,
            Layoutable.MaxHeightProperty,
            NavMenuTokenKey.MenuPopupMaxHeight));
        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(border, Decorator.PaddingProperty,
            NavMenuTokenKey.MenuPopupContentPadding));

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

        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(popup,
            Popup.MarginToAnchorProperty,
            NavMenuTokenKey.TopLevelItemPopupMarginToAnchor));
        navMenuItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(popup, Popup.MaskShadowsProperty,
            NavMenuTokenKey.MenuPopupBoxShadows));
        CreateTemplateParentBinding(popup, Popup.IsOpenProperty,
            NavMenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);

        return popup;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var itemsPanelStyle = new Style(selector =>
            selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
        itemsPanelStyle.Add(StackPanel.SpacingProperty, NavMenuTokenKey.VerticalItemsPanelSpacing);
        Add(itemsPanelStyle);

        {
            var popupFrameStyle = new Style(selector => selector.Nesting().Template().Name(PopupFramePart));
            popupFrameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
            Add(popupFrameStyle);
        }

        var darkCommonStyle =
            new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.IsDarkStyleProperty, true));
        {
            var popupFrameStyle = new Style(selector => selector.Nesting().Template().Name(PopupFramePart));
            popupFrameStyle.Add(Border.BackgroundProperty, NavMenuTokenKey.DarkItemBg);
            darkCommonStyle.Add(popupFrameStyle);
        }
        Add(darkCommonStyle);
    }
}