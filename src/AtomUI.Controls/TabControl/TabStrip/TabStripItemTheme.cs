﻿using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TabStripItemTheme : BaseTabStripItemTheme
{
    public const string ID = "TabStripItem";

    public TabStripItemTheme() : base(typeof(TabStripItem))
    {
    }

    public override string ThemeResourceKey()
    {
        return ID;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        BuildSizeTypeStyle();
    }

    protected void BuildSizeTypeStyle()
    {
        var topOrBottomStyle = new Style(selector => Selectors.Or(
            selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Top),
            selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Bottom)));

        {
            topOrBottomStyle.Add(Layoutable.MarginProperty, TabControlTokenResourceKey.HorizontalItemMargin);

            var largeSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Large));
            {
                var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
                decoratorStyle.Add(Decorator.PaddingProperty, TabControlTokenResourceKey.HorizontalItemPaddingLG);
                largeSizeStyle.Add(decoratorStyle);
            }

            topOrBottomStyle.Add(largeSizeStyle);

            var middleSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Middle));
            {
                var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
                decoratorStyle.Add(Decorator.PaddingProperty, TabControlTokenResourceKey.HorizontalItemPadding);
                middleSizeStyle.Add(decoratorStyle);
            }

            topOrBottomStyle.Add(middleSizeStyle);

            var smallSizeType = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Small));
            {
                var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
                decoratorStyle.Add(Decorator.PaddingProperty, TabControlTokenResourceKey.HorizontalItemPaddingSM);
                smallSizeType.Add(decoratorStyle);
            }

            topOrBottomStyle.Add(smallSizeType);

            Add(topOrBottomStyle);
        }

        var leftOrRightStyle = new Style(selector => Selectors.Or(
            selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Left),
            selector.Nesting().PropertyEquals(TabStripItem.TabStripPlacementProperty, Dock.Right)));
        {
            // 貌似没必要分大小，但是先放着吧，万一需要难得再加
            var largeSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Large));
            {
                var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
                decoratorStyle.Add(Decorator.PaddingProperty, TabControlTokenResourceKey.VerticalItemPadding);
                largeSizeStyle.Add(decoratorStyle);
            }

            leftOrRightStyle.Add(largeSizeStyle);

            var middleSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Middle));
            {
                var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
                decoratorStyle.Add(Decorator.PaddingProperty, TabControlTokenResourceKey.VerticalItemPadding);
                middleSizeStyle.Add(decoratorStyle);
            }

            leftOrRightStyle.Add(middleSizeStyle);

            var smallSizeType = new Style(selector =>
                selector.Nesting().PropertyEquals(TabStripItem.SizeTypeProperty, SizeType.Small));
            {
                var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(DecoratorPart));
                decoratorStyle.Add(Decorator.PaddingProperty, TabControlTokenResourceKey.VerticalItemPadding);
                smallSizeType.Add(decoratorStyle);
            }

            leftOrRightStyle.Add(smallSizeType);

            Add(leftOrRightStyle);
        }
    }
}