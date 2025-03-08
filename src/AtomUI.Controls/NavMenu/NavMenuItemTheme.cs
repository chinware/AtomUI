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
    public const string PopupPart = "PART_Popup";

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
            Name                       = PopupPart,
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
            Placement                  = PlacementMode.RightEdgeAlignedTop,
        };
        CreateTemplateParentBinding(popup, Popup.IsOpenProperty, NavMenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);
        
        var popupFrame = new Border()
        {
            Name = PopupFramePart
        };

        var scrollViewer = new MenuScrollViewer();
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        Grid.SetIsSharedSizeScope(itemsPresenter, true);
        scrollViewer.Content = itemsPresenter;
        popupFrame.Child     = scrollViewer;

        popup.Child = popupFrame;
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
            var popupStyle = new Style(selector => selector.Nesting().Template().Name(PopupPart));
            popupStyle.Add(Popup.MarginToAnchorProperty, NavMenuTokenKey.TopLevelItemPopupMarginToAnchor);
            popupStyle.Add(Popup.MaskShadowsProperty, NavMenuTokenKey.MenuPopupBoxShadows);
            Add(popupStyle);
        }
        {
            var popupFrameStyle = new Style(selector => selector.Nesting().Template().Name(PopupFramePart));
            popupFrameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
            popupFrameStyle.Add(Border.CornerRadiusProperty, NavMenuTokenKey.MenuPopupBorderRadius);
            popupFrameStyle.Add(Layoutable.MinWidthProperty, NavMenuTokenKey.MenuPopupMinWidth);
            popupFrameStyle.Add(Layoutable.MaxWidthProperty, NavMenuTokenKey.MenuPopupMaxWidth);
            popupFrameStyle.Add(Layoutable.MaxHeightProperty, NavMenuTokenKey.MenuPopupMaxHeight);
            popupFrameStyle.Add(Decorator.PaddingProperty, NavMenuTokenKey.MenuPopupContentPadding);
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