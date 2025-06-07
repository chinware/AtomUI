using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class BaseOverflowMenuItemTheme : BaseControlTheme
{
    public const string ItemDecoratorPart = "PART_ItemDecorator";
    public const string MainContainerPart = "PART_MainContainer";
    public const string ItemTextPresenterPart = "PART_ItemTextPresenter";
    public const string ItemCloseButtonPart = "PART_ItemCloseIcon";

    public BaseOverflowMenuItemTheme()
        : base(typeof(BaseOverflowMenuItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<BaseOverflowMenuItem>((menuItem, scope) =>
        {
            var container = new Border
            {
                Name = ItemDecoratorPart
            };

            var layout = new Grid
            {
                Name = MainContainerPart,
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                    {
                        SharedSizeGroup = "MenuCloseIcon"
                    }
                }
            };

            var itemTextPresenter = new ContentPresenter
            {
                Name = ItemTextPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                RecognizesAccessKey = true,
                IsHitTestVisible = false
            };

            Grid.SetColumn(itemTextPresenter, 0);
            CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty,
                HeaderedSelectingItemsControl.HeaderProperty,
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
            CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentTemplateProperty,
                HeaderedSelectingItemsControl.HeaderTemplateProperty);

            itemTextPresenter.RegisterInNameScope(scope);

            var menuCloseIcon = AntDesignIconPackage.CloseOutlined();
            menuCloseIcon.HorizontalAlignment = HorizontalAlignment.Right;
            menuCloseIcon.VerticalAlignment = VerticalAlignment.Center;

            var closeButton = new IconButton
            {
                Name = ItemCloseButtonPart,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Icon = menuCloseIcon
            };

            CreateTemplateParentBinding(closeButton, Visual.IsVisibleProperty, BaseOverflowMenuItem.IsClosableProperty);

            Grid.SetColumn(menuCloseIcon, 4);
            closeButton.RegisterInNameScope(scope);

            layout.Children.Add(itemTextPresenter);
            layout.Children.Add(closeButton);
            container.Child = layout;

            return container;
        });
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildDisabledStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemColor);
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Layoutable.MinHeightProperty, MenuTokenKey.ItemHeight);
            borderStyle.Add(Decorator.PaddingProperty, MenuTokenKey.ItemPaddingInline);
            borderStyle.Add(Border.BackgroundProperty, MenuTokenKey.ItemBg);
            borderStyle.Add(Border.CornerRadiusProperty, MenuTokenKey.ItemBorderRadius);
            commonStyle.Add(borderStyle);
        }
        {
            // 动画设置
            var isMotionEnabledStyle =
                new Style(selector => selector.Nesting().PropertyEquals(BaseOverflowMenuItem.IsMotionEnabledProperty, true));
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
            }));
            isMotionEnabledStyle.Add(borderStyle);
            commonStyle.Add(isMotionEnabledStyle);
        }

        // Hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemHoverColor);
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Border.BackgroundProperty, MenuTokenKey.ItemHoverBg);
            hoverStyle.Add(borderStyle);
        }
        
        commonStyle.Add(hoverStyle);
        
        var itemTextPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ItemTextPresenterPart));
        itemTextPresenterStyle.Add(Layoutable.MarginProperty, MenuTokenKey.ItemMargin);
        commonStyle.Add(itemTextPresenterStyle);
        
        var closeButtonStyle = new Style(selector => selector.Nesting().Template().Name(ItemCloseButtonPart));
        closeButtonStyle.Add(IconButton.NormalIconBrushProperty, SharedTokenKey.ColorIcon);
        closeButtonStyle.Add(IconButton.ActiveIconBrushProperty, SharedTokenKey.ColorIconHover);
        closeButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeSM);
        closeButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeSM);
        commonStyle.Add(closeButtonStyle);
        
        Add(commonStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemDisabledColor);
        Add(disabledStyle);
    }
}