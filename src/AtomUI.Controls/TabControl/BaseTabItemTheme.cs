using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class BaseTabItemTheme : BaseControlTheme
{
    public const string FramePart = "PART_Decorator";
    public const string ContentLayoutPart = "PART_ContentLayout";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string ItemIconPresenterPart = "PART_ItemIconPresenter";
    public const string ItemCloseButtonPart = "PART_ItemCloseButton";

    public BaseTabItemTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TabItem>((tabItem, scope) =>
        {
            // 做边框
            var frame = new Border
            {
                Name = FramePart
            };
            frame.RegisterInNameScope(scope);
            NotifyBuildControlTemplate(tabItem, scope, frame);
            return frame;
        });
    }

    protected virtual void NotifyBuildControlTemplate(TabItem tabItem, INameScope scope, Border container)
    {
        var containerLayout = new StackPanel
        {
            Name        = ContentLayoutPart,
            Orientation = Orientation.Horizontal
        };
        containerLayout.RegisterInNameScope(scope);

        var iconPresenter = new ContentPresenter()
        {
            Name = ItemIconPresenterPart,
        };
        CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, TabItem.IconProperty);
        CreateTemplateParentBinding(iconPresenter, ContentPresenter.IsVisibleProperty, TabItem.IconProperty,
            BindingMode.Default,
            ObjectConverters.IsNotNull);
        containerLayout.Children.Add(iconPresenter);
        var contentPresenter = new ContentPresenter
        {
            Name = ContentPresenterPart
        };
        containerLayout.Children.Add(contentPresenter);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
            HeaderedContentControl.HeaderProperty,
            BindingMode.Default,
            new FuncValueConverter<object?, object?>(
                o =>
                {
                    if (o is string str)
                    {
                        return new TextBlock()
                        {
                            Text              = str,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                    }

                    return o;
                }));
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            HeaderedContentControl.HeaderTemplateProperty);

        var closeButton = new IconButton
        {
            Name = ItemCloseButtonPart
        };
        closeButton.RegisterInNameScope(scope);

        CreateTemplateParentBinding(closeButton, IconButton.IconProperty, TabItem.CloseIconProperty);
        CreateTemplateParentBinding(closeButton, Visual.IsVisibleProperty, TabItem.IsClosableProperty);

        containerLayout.Children.Add(closeButton);
        container.Child = containerLayout;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(InputElement.CursorProperty,
            new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        commonStyle.Add(TemplatedControl.ForegroundProperty, TabControlTokenKey.ItemColor);

        Add(commonStyle);
        BuildItemIconStyle();
        BuildCloseButtonStyle();
        BuildPlacementStyle();
        BuildDisabledStyle();
    }
    
    private void BuildItemIconStyle()
    {
        // Icon 一些通用属性
        var iconPresenterStyle = new Style(selector =>
            selector.Nesting().Template().Name(ItemIconPresenterPart));
        iconPresenterStyle.Add(Layoutable.MarginProperty, TabControlTokenKey.ItemIconMargin);
        Add(iconPresenterStyle);

        {
            var iconStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemIconPresenterPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.NormalFilledBrushProperty, TabControlTokenKey.ItemColor);
            iconStyle.Add(Icon.ActiveFilledBrushProperty, TabControlTokenKey.ItemHoverColor);
            iconStyle.Add(Icon.SelectedFilledBrushProperty, TabControlTokenKey.ItemSelectedColor);
            iconStyle.Add(Icon.DisabledFilledBrushProperty, SharedTokenKey.ColorTextDisabled);
            Add(iconStyle);
        }
        
        // hover
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.ForegroundProperty, TabControlTokenKey.ItemHoverColor);
        {
            var iconStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemIconPresenterPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Active);
            hoverStyle.Add(iconStyle);
        }

        Add(hoverStyle);

        // 选中
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(TemplatedControl.ForegroundProperty, TabControlTokenKey.ItemSelectedColor);
        {
            var iconStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemIconPresenterPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            selectedStyle.Add(iconStyle);
        }
        Add(selectedStyle);
        
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Large));

        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, TabControlTokenKey.TitleFontSizeLG);
        {
            // Icon
            var iconStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemIconPresenterPart).Descendant().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
            largeSizeStyle.Add(iconStyle);
        }

        Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Middle));
        {
            // Icon
            var iconStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemIconPresenterPart).Descendant().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
            middleSizeStyle.Add(iconStyle);
        }
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, TabControlTokenKey.TitleFontSize);
        Add(middleSizeStyle);

        var smallSizeType = new Style(selector =>
            selector.Nesting().PropertyEquals(TabItem.SizeTypeProperty, SizeType.Small));

        {
            // Icon
            var iconStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemIconPresenterPart).Descendant().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeSM);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeSM);
            smallSizeType.Add(iconStyle);
        }

        smallSizeType.Add(TemplatedControl.FontSizeProperty, TabControlTokenKey.TitleFontSizeSM);
        Add(smallSizeType);
    }

    private void BuildCloseButtonStyle()
    {
        var itemCloseButtonStyle = new Style(selector => selector.Nesting().Template().Name(ItemCloseButtonPart));
        itemCloseButtonStyle.Add(Layoutable.MarginProperty, TabControlTokenKey.CloseIconMargin);
        itemCloseButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeSM);
        itemCloseButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeSM);
        itemCloseButtonStyle.Add(IconButton.NormalIconColorProperty, SharedTokenKey.ColorIcon);
        itemCloseButtonStyle.Add(IconButton.ActiveIconColorProperty, SharedTokenKey.ColorIconHover);
        itemCloseButtonStyle.Add(IconButton.SelectedIconColorProperty, SharedTokenKey.ColorIconHover);
        itemCloseButtonStyle.Add(IconButton.DisabledIconColorProperty, SharedTokenKey.ColorTextDisabled);
        
        Add(itemCloseButtonStyle);
    }

    private void BuildPlacementStyle()
    {
        // 设置 items presenter 面板样式
        // 分为上、右、下、左
        {
            // 上
            var topStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Avalonia.Controls.TabItem.TabStripPlacementProperty, Dock.Top));
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            topStyle.Add(iconStyle);
            Add(topStyle);
        }

        {
            // 右
            var rightStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Avalonia.Controls.TabItem.TabStripPlacementProperty, Dock.Right));
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            rightStyle.Add(iconStyle);
            Add(rightStyle);
        }
        {
            // 下
            var bottomStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Avalonia.Controls.TabItem.TabStripPlacementProperty, Dock.Bottom));

            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            bottomStyle.Add(iconStyle);
            Add(bottomStyle);
        }
        {
            // 左
            var leftStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Avalonia.Controls.TabItem.TabStripPlacementProperty, Dock.Left));
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            iconStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            leftStyle.Add(iconStyle);
            Add(leftStyle);
        }
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        Add(disabledStyle);
    }
}