﻿using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
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
        return new FuncControlTemplate<BaseOverflowMenuItem>((item, scope) =>
        {
            var container = new Border
            {
                Name = ItemDecoratorPart
            };
            var transitions = new Transitions();
            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty));
            container.Transitions = transitions;

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
            TokenResourceBinder.CreateTokenBinding(itemTextPresenter, Layoutable.MarginProperty,
                MenuTokenKey.ItemMargin);
            CreateTemplateParentBinding(itemTextPresenter, ContentPresenter.ContentProperty,
                HeaderedSelectingItemsControl.HeaderProperty);
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
            TokenResourceBinder.CreateSharedTokenBinding(menuCloseIcon, Icon.NormalFilledBrushProperty,
                SharedTokenKey.ColorIcon);
            TokenResourceBinder.CreateSharedTokenBinding(menuCloseIcon, Icon.ActiveFilledBrushProperty,
                SharedTokenKey.ColorIconHover);

            TokenResourceBinder.CreateSharedTokenBinding(menuCloseIcon, Layoutable.WidthProperty,
                SharedTokenKey.IconSizeSM);
            TokenResourceBinder.CreateSharedTokenBinding(menuCloseIcon, Layoutable.HeightProperty,
                SharedTokenKey.IconSizeSM);

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
        var commonStyle = new Style(selector => selector.Nesting());
        BuildCommonStyle(commonStyle);
        BuildDisabledStyle();
        Add(commonStyle);
    }

    private void BuildCommonStyle(Style commonStyle)
    {
        commonStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemColor);
        {
            var borderStyle = new Style(selector => selector.Nesting().Template().Name(ItemDecoratorPart));
            borderStyle.Add(Layoutable.MinHeightProperty, MenuTokenKey.ItemHeight);
            borderStyle.Add(Decorator.PaddingProperty, MenuTokenKey.ItemPaddingInline);
            borderStyle.Add(Border.BackgroundProperty, MenuTokenKey.ItemBg);
            borderStyle.Add(Border.CornerRadiusProperty, MenuTokenKey.ItemBorderRadius);
            commonStyle.Add(borderStyle);
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
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, MenuTokenKey.ItemDisabledColor);
        Add(disabledStyle);
    }
}