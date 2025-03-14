﻿using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
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
internal class TopLevelMenuItemTheme : BaseControlTheme
{
    public const string ID = "TopLevelMenuItem";

    public const string PopupPart = "PART_Popup";
    public const string PopupFramePart = "PART_PopupFrame";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public TopLevelMenuItemTheme() : base(typeof(MenuItem))
    {
    }

    public override string ThemeResourceKey()
    {
        return ID;
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<MenuItem>((menuItem, scope) =>
        {
            var panel = new Panel();
            var contentPresenter = new ContentPresenter
            {
                Name                = HeaderPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Center,
                RecognizesAccessKey = true
            };
            
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                HeaderedSelectingItemsControl.HeaderProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                HeaderedSelectingItemsControl.HeaderTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty,
                TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.MinHeightProperty, Layoutable.MinHeightProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.FontSizeProperty,
                TemplatedControl.FontSizeProperty);

            contentPresenter.RegisterInNameScope(scope);
            panel.Children.Add(contentPresenter);

            var popup = CreateMenuPopup(menuItem);
            popup.RegisterInNameScope(scope);
            panel.Children.Add(popup);
            return panel;
        });
    }

    private Popup CreateMenuPopup(MenuItem menuItem)
    {
        var popup = new Popup
        {
            Name                       = PopupPart,
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
            Placement                  = PlacementMode.BottomEdgeAlignedLeft
        };

        var border = new Border()
        {
            Name = PopupFramePart
        };
        
        var scrollViewer = new MenuScrollViewer();
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        scrollViewer.IsScrollChainingEnabled = false;
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        Grid.SetIsSharedSizeScope(itemsPresenter, true);
        KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
        scrollViewer.Content = itemsPresenter;
        border.Child         = scrollViewer;
        popup.Child          = border;
        
        CreateTemplateParentBinding(popup, Avalonia.Controls.Primitives.Popup.IsOpenProperty,
            Avalonia.Controls.MenuItem.IsSubMenuOpenProperty, BindingMode.TwoWay);
        
        return popup;
    }

    protected override void BuildStyles()
    {
        var topLevelStyle = new Style(selector => selector.Nesting().Class(MenuItem.TopLevelPC));
        BuildCommonStyle(topLevelStyle);
        BuildSizeTypeStyle(topLevelStyle);
        BuildDisabledStyle(topLevelStyle);
        BuildPopupStyle(topLevelStyle);
        BuildMotionStyle(topLevelStyle);
        Add(topLevelStyle);
    }

    private void BuildMotionStyle(Style topLevelStyle)
    {
        var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(MenuItem.IsMotionEnabledProperty, true));
        var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        headerPresenterStyle.Add(ContentPresenter.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.BackgroundProperty),
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.ForegroundProperty)
        }));
        isMotionEnabledStyle.Add(headerPresenterStyle);
        topLevelStyle.Add(isMotionEnabledStyle);
    }
    
    private void BuildPopupStyle(Style topLevelStyle)
    {
        var popupFrameStyle = new Style(selector => selector.Nesting().Template().Name(PopupFramePart));
        popupFrameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        popupFrameStyle.Add(Border.CornerRadiusProperty, MenuTokenKey.MenuPopupBorderRadius);
        popupFrameStyle.Add(Layoutable.MinWidthProperty, MenuTokenKey.MenuPopupMinWidth);
        popupFrameStyle.Add(Layoutable.MaxWidthProperty, MenuTokenKey.MenuPopupMaxWidth);
        popupFrameStyle.Add(Layoutable.MinHeightProperty, MenuTokenKey.MenuPopupMinHeight);
        popupFrameStyle.Add(Layoutable.MaxHeightProperty, MenuTokenKey.MenuPopupMaxHeight);
        popupFrameStyle.Add(Decorator.PaddingProperty, MenuTokenKey.MenuPopupContentPadding);
        
        topLevelStyle.Add(popupFrameStyle);
        
        var popupStyle = new Style(selector => selector.Nesting().Template().Name(PopupPart));
        popupStyle.Add(Popup.MarginToAnchorProperty, MenuTokenKey.TopLevelItemPopupMarginToAnchor);
        popupStyle.Add(Popup.MaskShadowsProperty, MenuTokenKey.MenuPopupBoxShadows);
        topLevelStyle.Add(popupStyle);
    }

    private void BuildCommonStyle(Style topLevelStyle)
    {
        var commonStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        commonStyle.Add(TemplatedControl.BackgroundProperty, Brushes.Transparent);

        // 正常状态
        {
            var contentPresenterStyle = new Style(selector =>
                selector.Nesting().Template().OfType<ContentPresenter>().Name(HeaderPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, Brushes.Transparent);
            commonStyle.Add(contentPresenterStyle);
        }

        // hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var contentPresenterHoverStyle = new Style(selector =>
                selector.Nesting().Template().OfType<ContentPresenter>().Name(HeaderPresenterPart));
            contentPresenterHoverStyle.Add(ContentPresenter.BackgroundProperty,
                MenuTokenKey.TopLevelItemHoverBg);
            contentPresenterHoverStyle.Add(ContentPresenter.ForegroundProperty,
                MenuTokenKey.TopLevelItemHoverColor);
            contentPresenterHoverStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
            hoverStyle.Add(contentPresenterHoverStyle);
        }
        commonStyle.Add(hoverStyle);

        // 在打开状态下的
        var openedStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Avalonia.Controls.MenuItem.IsSubMenuOpenProperty, true));
        {
            var contentPresenterHoverStyle = new Style(selector =>
                selector.Nesting().Template().OfType<ContentPresenter>().Name(HeaderPresenterPart));
            contentPresenterHoverStyle.Add(ContentPresenter.BackgroundProperty,
                MenuTokenKey.TopLevelItemHoverBg);
            contentPresenterHoverStyle.Add(ContentPresenter.ForegroundProperty,
                MenuTokenKey.TopLevelItemHoverColor);
            contentPresenterHoverStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
            openedStyle.Add(contentPresenterHoverStyle);
        }

        commonStyle.Add(openedStyle);

        topLevelStyle.Add(commonStyle);
    }

    private void BuildSizeTypeStyle(Style topLevelStyle)
    {
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, MenuTokenKey.TopLevelItemBorderRadiusLG);
        largeSizeStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeightLG);
        largeSizeStyle.Add(TemplatedControl.PaddingProperty, MenuTokenKey.TopLevelItemPaddingLG);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, MenuTokenKey.TopLevelItemFontSizeLG);
        {
            var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));

            presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenKey.TopLevelItemLineHeightLG);
            largeSizeStyle.Add(presenterStyle);
        }
        topLevelStyle.Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, MenuTokenKey.TopLevelItemBorderRadius);
        middleSizeStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeight);
        middleSizeStyle.Add(TemplatedControl.PaddingProperty, MenuTokenKey.TopLevelItemPadding);
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, MenuTokenKey.TopLevelItemFontSize);
        {
            var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenKey.TopLevelItemLineHeight);
            middleSizeStyle.Add(presenterStyle);
        }
        topLevelStyle.Add(middleSizeStyle);

        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, MenuTokenKey.TopLevelItemBorderRadiusSM);
        smallSizeStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeightSM);
        smallSizeStyle.Add(TemplatedControl.PaddingProperty, MenuTokenKey.TopLevelItemPaddingSM);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, MenuTokenKey.TopLevelItemFontSizeSM);
        {
            var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenKey.TopLevelItemLineHeightSM);
            smallSizeStyle.Add(presenterStyle);
        }
        topLevelStyle.Add(smallSizeStyle);
    }

    private void BuildDisabledStyle(Style topLevelStyle)
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemDisabledColor);
        topLevelStyle.Add(disabledStyle);
    }
}