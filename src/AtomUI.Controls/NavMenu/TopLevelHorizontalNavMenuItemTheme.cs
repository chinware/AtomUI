using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TopLevelHorizontalNavMenuItemTheme : BaseControlTheme
{
    public const string ID = "TopLevelHorizontalNavMenuItem";
    public const string PopupPart = "PART_Popup";
    public const string PopupFramePart = "PART_PopupFrame";
    public const string FramePart = "PART_Frame";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string ItemIconPresenterPart = "PART_ItemIconPresenter";
    public const string ActiveIndicatorPart = "PART_ActiveIndicator";
    
    public TopLevelHorizontalNavMenuItemTheme() : base(typeof(NavMenuItem))
    {
    }
    
    public override string ThemeResourceKey()
    {
        return ID;
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<NavMenuItem>((menuItem, scope) =>
        {
            var rootLayout = new Panel();

            var frame = new Border()
            {
                Name = FramePart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            frame.RegisterInNameScope(scope);
            
            var contentLayout = new DockPanel()
            {
                LastChildFill = true
            };
            
            var popup = CreateMenuPopup();
            popup.RegisterInNameScope(scope);
            rootLayout.Children.Add(popup);
            
            var iconPresenter = new ContentPresenter()
            {
                Name                = ItemIconPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
            };
            iconPresenter.RegisterInNameScope(scope);
            DockPanel.SetDock(iconPresenter, Dock.Left);
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.IsEnabledProperty, NavMenuItem.IsEnabledProperty);
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, NavMenuItem.IconProperty);
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.IsVisibleProperty, NavMenuItem.IconProperty,
                BindingMode.Default,
                ObjectConverters.IsNotNull);
            
            contentLayout.Children.Add(iconPresenter);
            
            var contentPresenter = new ContentPresenter
            {
                Name                = HeaderPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Center,
                RecognizesAccessKey = true,
            };
    
            // TODO 后面需要评估一下，能直接绑定到对象，是否还需要这样通过模板绑定
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
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
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                HeaderedSelectingItemsControl.HeaderTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.MinHeightProperty, Layoutable.MinHeightProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty,
                TemplatedControl.FontSizeProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty,
                TemplatedControl.ForegroundProperty);
    
            contentPresenter.RegisterInNameScope(scope);
            contentLayout.Children.Add(contentPresenter);

            frame.Child = contentLayout;
            
            rootLayout.Children.Add(frame);

            var activeIndicator = new Rectangle()
            {
                Name = ActiveIndicatorPart,
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            CreateTemplateParentBinding(activeIndicator, Rectangle.HeightProperty, NavMenuItem.ActiveBarHeightProperty);
            CreateTemplateParentBinding(activeIndicator, Rectangle.WidthProperty, NavMenuItem.EffectiveActiveBarWidthProperty);
            
            rootLayout.Children.Add(activeIndicator);
            
            return rootLayout;
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

        var popupFrame = new Border()
        {
            Name = PopupFramePart
        };
        
        CreateTemplateParentBinding(popupFrame, Border.MinWidthProperty, NavMenuItem.EffectivePopupMinWidthProperty);
        
        var scrollViewer = new MenuScrollViewer();
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        Grid.SetIsSharedSizeScope(itemsPresenter, true);
        KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
        scrollViewer.Content = itemsPresenter;
        popupFrame.Child     = scrollViewer;
        popup.Child          = popupFrame;
    
        CreateTemplateParentBinding(popup, Popup.IsOpenProperty,
            NavMenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);
    
        return popup;
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildPopupStyle();
        BuildActiveIndicatorStyle();
        BuildMenuIconStyle();
        BuildDisabledStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle =
            new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.BackgroundProperty, Brushes.Transparent);
        commonStyle.Add(NavMenuItem.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        commonStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.MarginProperty, NavMenuTokenKey.HorizontalItemMargin);
            commonStyle.Add(frameStyle);
        }
        
        var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        presenterStyle.Add(ContentPresenter.LineHeightProperty, NavMenuTokenKey.HorizontalLineHeight);
        commonStyle.Add(presenterStyle);

        var iconPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
        iconPresenterStyle.Add(Layoutable.MarginProperty, NavMenuTokenKey.IconMargin);
        commonStyle.Add(iconPresenterStyle);
        Add(commonStyle);
    }

    private void BuildPopupStyle()
    {
        var popupStyle = new Style(selector => selector.Nesting().Template().Name(PopupPart));
        popupStyle.Add(Popup.MarginToAnchorProperty, NavMenuTokenKey.TopLevelItemPopupMarginToAnchor);
        popupStyle.Add(Popup.MaskShadowsProperty, NavMenuTokenKey.MenuPopupBoxShadows);
        Add(popupStyle);
        var popupFrameStyle = new Style(selector => selector.Nesting().Template().Name(PopupFramePart));
        popupFrameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        popupFrameStyle.Add(Border.CornerRadiusProperty, NavMenuTokenKey.MenuPopupBorderRadius);
        popupFrameStyle.Add(Layoutable.MinWidthProperty, NavMenuTokenKey.MenuPopupMinWidth);
        popupFrameStyle.Add(Layoutable.MaxWidthProperty, NavMenuTokenKey.MenuPopupMaxWidth);
        popupFrameStyle.Add(Layoutable.MaxHeightProperty, NavMenuTokenKey.MenuPopupMaxHeight);
        popupFrameStyle.Add(Decorator.PaddingProperty, NavMenuTokenKey.MenuPopupContentPadding);
        Add(popupFrameStyle);
    }
    
    private void BuildActiveIndicatorStyle()
    {
        {
            // 动画设置
            var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.IsMotionEnabledProperty, true));
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(ActiveIndicatorPart));
            indicatorStyle.Add(Rectangle.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Rectangle.FillProperty)
            }));
            isMotionEnabledStyle.Add(indicatorStyle);
            Add(isMotionEnabledStyle);
        }
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(ActiveIndicatorPart));
            indicatorStyle.Add(Rectangle.FillProperty, Brushes.Transparent);
            Add(indicatorStyle);
        }
        var hoverStyle = new Style(selector => Selectors.Or(selector.Nesting().Class(StdPseudoClass.PointerOver),
            selector.Nesting().Class(StdPseudoClass.Open),
            selector.Nesting().Class(StdPseudoClass.Selected)));
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(ActiveIndicatorPart));
            indicatorStyle.Add(Rectangle.FillProperty, SharedTokenKey.ColorPrimary);
            hoverStyle.Add(indicatorStyle);
        }
        Add(hoverStyle);
        
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        {
            selectedStyle.Add(NavMenuItem.ForegroundProperty, SharedTokenKey.ColorPrimary);
        }
        Add(selectedStyle);
    }
    
    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, NavMenuTokenKey.ItemDisabledColor);
        Add(disabledStyle);
    }
    
     private void BuildMenuIconStyle()
    {
        {
            var iconContentPresenterStyle =
                new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconContentPresenterStyle.Add(Visual.IsVisibleProperty, false);
            iconContentPresenterStyle.Add(Layoutable.MarginProperty, NavMenuTokenKey.ItemMargin);
            iconContentPresenterStyle.Add(Layoutable.WidthProperty, NavMenuTokenKey.ItemIconSize);
            iconContentPresenterStyle.Add(Layoutable.HeightProperty, NavMenuTokenKey.ItemIconSize);
            
            Add(iconContentPresenterStyle);
        }

        var hasIconStyle = new Style(selector => selector.Nesting().Class(":icon"));
        {
            var iconContentPresenterStyle =
                new Style(selector => selector.Nesting().Template().Name(ItemIconPresenterPart));
            iconContentPresenterStyle.Add(Visual.IsVisibleProperty, true);
            hasIconStyle.Add(iconContentPresenterStyle);

            {
                var iconStyle = new Style(selector =>
                    selector.Nesting().Template().Name(ItemIconPresenterPart).Child().OfType<Icon>());
                iconStyle.Add(Icon.WidthProperty, NavMenuTokenKey.ItemIconSize);
                iconStyle.Add(Icon.HeightProperty, NavMenuTokenKey.ItemIconSize);
                
                iconStyle.Add(Icon.NormalFilledBrushProperty, NavMenuTokenKey.ItemColor);
                iconStyle.Add(Icon.ActiveFilledBrushProperty, NavMenuTokenKey.ItemHoverColor);
                iconStyle.Add(Icon.SelectedFilledBrushProperty, NavMenuTokenKey.ItemSelectedColor);
                iconStyle.Add(Icon.DisabledFilledBrushProperty, NavMenuTokenKey.ItemDisabledColor);
                hasIconStyle.Add(iconStyle);
            }
            var darkStyle = new Style(selector => selector.Nesting().PropertyEquals(NavMenuItem.IsDarkStyleProperty, true));
            {
                var iconStyle = new Style(selector =>
                    selector.Nesting().Template().Name(ItemIconPresenterPart).Child().OfType<Icon>());
                iconStyle.Add(Icon.NormalFilledBrushProperty, NavMenuTokenKey.DarkItemColor);
                iconStyle.Add(Icon.ActiveFilledBrushProperty, NavMenuTokenKey.DarkItemHoverColor);
                iconStyle.Add(Icon.SelectedFilledBrushProperty, NavMenuTokenKey.DarkItemSelectedColor);
                iconStyle.Add(Icon.DisabledFilledBrushProperty, NavMenuTokenKey.DarkItemDisabledColor);
                darkStyle.Add(iconStyle);
            }
            hasIconStyle.Add(darkStyle);
        }
        Add(hasIconStyle);
    }
    
}