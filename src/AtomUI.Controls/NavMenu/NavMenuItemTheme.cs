using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NavMenuItemTheme : BaseNavMenuItemTheme
{
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    
    public NavMenuItemTheme()
        : base(typeof(NavMenuItem))
    {
    }
    
    protected NavMenuItemTheme(Type targetType) : base(targetType)
    {
    }
    
    protected override void BuildExtraItem(Panel layout, INameScope scope)
    {
        var popup = CreateMenuPopup();
        popup.RegisterInNameScope(scope);
        layout.Children.Add(popup);
    }
    
    protected Popup CreateMenuPopup()
    {
        var popup = new Popup
        {
            Name                       = ThemeConstants.PopupPart,
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
            Placement                  = PlacementMode.RightEdgeAlignedTop
        };

        var border = new Border();
        TokenResourceBinder.CreateTokenBinding(border, Border.BackgroundProperty,
            GlobalTokenResourceKey.ColorBgContainer);
        TokenResourceBinder.CreateTokenBinding(border, Border.CornerRadiusProperty,
            NavMenuTokenResourceKey.MenuPopupBorderRadius);
        TokenResourceBinder.CreateTokenBinding(border, Layoutable.MinWidthProperty,
            NavMenuTokenResourceKey.MenuPopupMinWidth);
        TokenResourceBinder.CreateTokenBinding(border, Layoutable.MaxWidthProperty,
            NavMenuTokenResourceKey.MenuPopupMaxWidth);
        TokenResourceBinder.CreateTokenBinding(border, Layoutable.MaxHeightProperty,
            NavMenuTokenResourceKey.MenuPopupMaxHeight);
        TokenResourceBinder.CreateTokenBinding(border, Decorator.PaddingProperty,
            NavMenuTokenResourceKey.MenuPopupContentPadding);

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
            NavMenuTokenResourceKey.TopLevelItemPopupMarginToAnchor);
        TokenResourceBinder.CreateTokenBinding(popup, Popup.MaskShadowsProperty,
            NavMenuTokenResourceKey.MenuPopupBoxShadows);
        CreateTemplateParentBinding(popup, Avalonia.Controls.Primitives.Popup.IsOpenProperty,
            NavMenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);

        return popup;
    }
}