using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
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
internal class TopLevelMenuItemTheme : BaseControlTheme
{
    public const string ID = "TopLevelMenuItem";

    public const string PopupPart = "PART_Popup";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public TopLevelMenuItemTheme() : base(typeof(MenuItem))
    {
    }

    public override string ThemeResourceKey()
    {
        return ID;
    }

    protected override IControlTemplate? BuildControlTemplate()
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

            // TODO 后面需要评估一下，能直接绑定到对象，是否还需要这样通过模板绑定
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

            var popup = CreateMenuPopup();
            popup.RegisterInNameScope(scope);
            panel.Children.Add(popup);
            return panel;
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
        KeyboardNavigation.SetTabNavigation(itemsPresenter, KeyboardNavigationMode.Continue);
        scrollViewer.Content = itemsPresenter;
        border.Child         = scrollViewer;
        popup.Child          = border;

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
        var topLevelStyle = new Style(selector => selector.Nesting().Class(MenuItem.TopLevelPC));
        BuildCommonStyle(topLevelStyle);
        BuildSizeTypeStyle(topLevelStyle);
        BuildDisabledStyle(topLevelStyle);
        Add(topLevelStyle);
    }

    private void BuildCommonStyle(Style topLevelStyle)
    {
        var commonStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        commonStyle.Add(TemplatedControl.BackgroundProperty, DesignTokenKey.ColorTransparent);

        // 正常状态
        {
            var contentPresenterStyle = new Style(selector =>
                selector.Nesting().Template().OfType<ContentPresenter>().Name(HeaderPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, DesignTokenKey.ColorTransparent);
            commonStyle.Add(contentPresenterStyle);
        }

        // hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var contentPresenterHoverStyle = new Style(selector =>
                selector.Nesting().Template().OfType<ContentPresenter>().Name(HeaderPresenterPart));
            contentPresenterHoverStyle.Add(ContentPresenter.BackgroundProperty,
                MenuTokenResourceKey.TopLevelItemHoverBg);
            contentPresenterHoverStyle.Add(ContentPresenter.ForegroundProperty,
                MenuTokenResourceKey.TopLevelItemHoverColor);
            contentPresenterHoverStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));
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
                MenuTokenResourceKey.TopLevelItemHoverBg);
            contentPresenterHoverStyle.Add(ContentPresenter.ForegroundProperty,
                MenuTokenResourceKey.TopLevelItemHoverColor);
            contentPresenterHoverStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));
            openedStyle.Add(contentPresenterHoverStyle);
        }

        commonStyle.Add(openedStyle);

        topLevelStyle.Add(commonStyle);
    }

    private void BuildSizeTypeStyle(Style topLevelStyle)
    {
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, MenuTokenResourceKey.TopLevelItemBorderRadiusLG);
        largeSizeStyle.Add(Layoutable.MinHeightProperty, DesignTokenKey.ControlHeightLG);
        largeSizeStyle.Add(TemplatedControl.PaddingProperty, MenuTokenResourceKey.TopLevelItemPaddingLG);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, MenuTokenResourceKey.TopLevelItemFontSizeLG);
        {
            var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));

            presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenResourceKey.TopLevelItemLineHeightLG);
            largeSizeStyle.Add(presenterStyle);
        }
        topLevelStyle.Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, MenuTokenResourceKey.TopLevelItemBorderRadius);
        middleSizeStyle.Add(Layoutable.MinHeightProperty, DesignTokenKey.ControlHeight);
        middleSizeStyle.Add(TemplatedControl.PaddingProperty, MenuTokenResourceKey.TopLevelItemPadding);
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, MenuTokenResourceKey.TopLevelItemFontSize);
        {
            var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenResourceKey.TopLevelItemLineHeight);
            middleSizeStyle.Add(presenterStyle);
        }
        topLevelStyle.Add(middleSizeStyle);

        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(MenuItem.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, MenuTokenResourceKey.TopLevelItemBorderRadiusSM);
        smallSizeStyle.Add(Layoutable.MinHeightProperty, DesignTokenKey.ControlHeightSM);
        smallSizeStyle.Add(TemplatedControl.PaddingProperty, MenuTokenResourceKey.TopLevelItemPaddingSM);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, MenuTokenResourceKey.TopLevelItemFontSizeSM);
        {
            var presenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            presenterStyle.Add(ContentPresenter.LineHeightProperty, MenuTokenResourceKey.TopLevelItemLineHeightSM);
            smallSizeStyle.Add(presenterStyle);
        }
        topLevelStyle.Add(smallSizeStyle);
    }

    private void BuildDisabledStyle(Style topLevelStyle)
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenResourceKey.ItemDisabledColor);
        topLevelStyle.Add(disabledStyle);
    }
}