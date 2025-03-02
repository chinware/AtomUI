using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Data;
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

internal class BaseTabStripItemTheme : BaseControlTheme
{
    public const string DecoratorPart = "PART_Decorator";
    public const string ContentLayoutPart = "PART_ContentLayout";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string ItemIconPart = "PART_ItemIcon";
    public const string ItemCloseButtonPart = "PART_ItemCloseButton";

    public BaseTabStripItemTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TabStripItem>((strip, scope) =>
        {
            // 做边框
            var decorator = new Border
            {
                Name = DecoratorPart
            };
            decorator.RegisterInNameScope(scope);
            NotifyBuildControlTemplate(strip, scope, decorator);
            return decorator;
        });
    }

    protected virtual void NotifyBuildControlTemplate(TabStripItem stripItem, INameScope scope, Border container)
    {
        var containerLayout = new StackPanel
        {
            Name        = ContentLayoutPart,
            Orientation = Orientation.Horizontal
        };
        containerLayout.RegisterInNameScope(scope);

        var contentPresenter = new ContentPresenter
        {
            Name = ContentPresenterPart
        };
        containerLayout.Children.Add(contentPresenter);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, ContentControl.ContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            ContentControl.ContentTemplateProperty,
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

        var iconButton = new IconButton
        {
            Name = ItemCloseButtonPart
        };
        iconButton.RegisterInNameScope(scope);
        stripItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(iconButton,
            Layoutable.MarginProperty, TabControlTokenKey.CloseIconMargin));

        CreateTemplateParentBinding(iconButton, IconButton.IconProperty, TabStripItem.CloseIconProperty);
        CreateTemplateParentBinding(iconButton, Visual.IsVisibleProperty, TabStripItem.IsClosableProperty);

        containerLayout.Children.Add(iconButton);
        container.Child = containerLayout;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(InputElement.CursorProperty,
            new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        commonStyle.Add(TemplatedControl.ForegroundProperty, TabControlTokenKey.ItemColor);

        // Icon 一些通用属性
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
            iconStyle.Add(Layoutable.MarginProperty, TabControlTokenKey.ItemIconMargin);
            commonStyle.Add(iconStyle);
        }

        // hover
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.ForegroundProperty, TabControlTokenKey.ItemHoverColor);
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
            iconStyle.Add(Icon.IconModeProperty, IconMode.Active);
            hoverStyle.Add(iconStyle);
        }

        commonStyle.Add(hoverStyle);

        // 选中
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(TemplatedControl.ForegroundProperty, TabControlTokenKey.ItemSelectedColor);
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
            iconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            selectedStyle.Add(iconStyle);
        }
        commonStyle.Add(selectedStyle);
        Add(commonStyle);
        BuildSizeTypeStyle();
        BuildPlacementStyle();
        BuildDisabledStyle();
    }

    private void BuildSizeTypeStyle()
    {
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Large));

        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, TabControlTokenKey.TitleFontSizeLG);
        {
            // Icon
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
            largeSizeStyle.Add(iconStyle);
        }

        Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Middle));
        {
            // Icon
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
            middleSizeStyle.Add(iconStyle);
        }
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, TabControlTokenKey.TitleFontSize);
        Add(middleSizeStyle);

        var smallSizeType = new Style(selector =>
            selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Small));

        {
            // Icon
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ItemIconPart));
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeSM);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeSM);
            smallSizeType.Add(iconStyle);
        }

        smallSizeType.Add(TemplatedControl.FontSizeProperty, TabControlTokenKey.TitleFontSizeSM);
        Add(smallSizeType);
    }

    private void BuildPlacementStyle()
    {
        // 设置 items presenter 面板样式
        // 分为上、右、下、左
        {
            // 上
            var topStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Top));
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            topStyle.Add(iconStyle);
            Add(topStyle);
        }

        {
            // 右
            var rightStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Right));
            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            rightStyle.Add(iconStyle);
            Add(rightStyle);
        }
        {
            // 下
            var bottomStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Bottom));

            var iconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            iconStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            bottomStyle.Add(iconStyle);
            Add(bottomStyle);
        }
        {
            // 左
            var leftStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Left));
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