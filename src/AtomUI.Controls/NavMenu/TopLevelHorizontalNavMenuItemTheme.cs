using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TopLevelHorizontalNavMenuItemTheme : BaseControlTheme
{
    public const string ID = "TopLevelHorizontalNavMenuItem";
    public const string PopupPart = "PART_Popup";
    public const string FramePart = "PART_Frame";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string ItemIconPresenterPart = "PART_ItemIconPresenter";
    
    public TopLevelHorizontalNavMenuItemTheme() : base(typeof(NavMenuItem))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
    
    protected override IControlTemplate? BuildControlTemplate()
    {
        return new FuncControlTemplate<NavMenuItem>((menuItem, scope) =>
        {
            BuildInstanceStyles(menuItem);

            var frame = new Border()
            {
                Name = FramePart
            };
            
            var rootContainerLayout = new DockPanel()
            {
                LastChildFill = true
            };
            
            var popup = CreateMenuPopup();
            popup.RegisterInNameScope(scope);
            rootContainerLayout.Children.Add(popup);
            
            var iconPresenter = new ContentPresenter()
            {
                Name                = ItemIconPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
            };
            iconPresenter.RegisterInNameScope(scope);
            DockPanel.SetDock(iconPresenter, Dock.Left);
            
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, NavMenuItem.IconProperty);
            TokenResourceBinder.CreateTokenBinding(iconPresenter, Layoutable.MarginProperty,
                NavMenuTokenResourceKey.IconMargin);
            
            rootContainerLayout.Children.Add(iconPresenter);
            
            var contentPresenter = new ContentPresenter
            {
                Name                = HeaderPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Center,
                RecognizesAccessKey = true,
            };
    
            // TODO 后面需要评估一下，能直接绑定到对象，是否还需要这样通过模板绑定
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                HeaderedSelectingItemsControl.HeaderProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                HeaderedSelectingItemsControl.HeaderTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.MinHeightProperty, Layoutable.MinHeightProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty,
                TemplatedControl.FontSizeProperty);
    
            contentPresenter.RegisterInNameScope(scope);
            rootContainerLayout.Children.Add(contentPresenter);

            frame.Child = rootContainerLayout;
            
            return frame;
        });
    }

    private Popup CreateMenuPopup()
    {
        var popup = new Popup
        {
            Name                       = PopupPart,
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
            Placement                  = PlacementMode.BottomEdgeAlignedLeft
        };
    
        var border = new Border();
    
        TokenResourceBinder.CreateTokenBinding(border, Border.BackgroundProperty,
            GlobalTokenResourceKey.ColorBgContainer);
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
        KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
        scrollViewer.Content = itemsPresenter;
        border.Child         = scrollViewer;
        popup.Child          = border;
    
        TokenResourceBinder.CreateTokenBinding(popup, Popup.MarginToAnchorProperty,
            MenuTokenResourceKey.TopLevelItemPopupMarginToAnchor);
        TokenResourceBinder.CreateTokenBinding(popup, Popup.MaskShadowsProperty,
            MenuTokenResourceKey.MenuPopupBoxShadows);
    
        CreateTemplateParentBinding(popup, Avalonia.Controls.Primitives.Popup.IsOpenProperty,
            NavMenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);
    
        return popup;
    }

    protected override void BuildStyles()
    {
        var topLevelStyle = new Style(selector => selector.Nesting().Class(NavMenuItem.TopLevelPC));
        topLevelStyle.Add(NavMenuItem.CursorProperty, new Cursor(StandardCursorType.Hand));
        BuildCommonStyle(topLevelStyle);
        BuildDisabledStyle(topLevelStyle);
        Add(topLevelStyle);
    }
    
    private void BuildCommonStyle(Style topLevelStyle)
    {
        var commonStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        commonStyle.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
    
        // hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
        }
        commonStyle.Add(hoverStyle);
        
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        
        frameStyle.Add(Border.PaddingProperty, NavMenuTokenResourceKey.ItemPadding);
        commonStyle.Add(frameStyle);
        
        commonStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSize);
        
        var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        presenterStyle.Add(ContentPresenter.LineHeightProperty, NavMenuTokenResourceKey.HorizontalLineHeight);
        commonStyle.Add(presenterStyle);
        
        topLevelStyle.Add(commonStyle);
    }
    
    private void BuildDisabledStyle(Style topLevelStyle)
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenResourceKey.ItemDisabledColor);
        topLevelStyle.Add(disabledStyle);
    }
    
    protected override void BuildInstanceStyles(Control control)
    {
        var iconStyle = new Style(selector => selector.OfType<PathIcon>());
        iconStyle.Add(PathIcon.WidthProperty, NavMenuTokenResourceKey.ItemIconSize);
        iconStyle.Add(PathIcon.HeightProperty, NavMenuTokenResourceKey.ItemIconSize);
        iconStyle.Add(PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorText);
        control.Styles.Add(iconStyle);
    }
}