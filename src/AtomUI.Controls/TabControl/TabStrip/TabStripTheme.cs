﻿using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TabStripTheme : BaseTabStripTheme
{
    public const string SelectedItemIndicatorPart = "PART_SelectedItemIndicator";

    public TabStripTheme() : base(typeof(TabStrip))
    {
    }

    protected override void NotifyBuildControlTemplate(BaseTabStrip baseTabStrip, INameScope scope, Border container)
    {
        var alignWrapper = new Panel
        {
            Name = AlignWrapperPart
        };
        alignWrapper.RegisterInNameScope(scope);

        var tabScrollViewer = new TabStripScrollViewer
        {
            Name = TabsContainerPart
        };
        CreateTemplateParentBinding(tabScrollViewer, BaseTabScrollViewer.TabStripPlacementProperty,
            BaseTabStrip.TabStripPlacementProperty);
        var contentPanel = CreateTabStripContentPanel(scope);
        tabScrollViewer.Content  = contentPanel;
        tabScrollViewer.TabStrip = baseTabStrip;

        alignWrapper.Children.Add(tabScrollViewer);
        container.Child = alignWrapper;
    }

    private Panel CreateTabStripContentPanel(INameScope scope)
    {
        var layout = new Panel();
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        itemsPresenter.RegisterInNameScope(scope);
        var border = new Border
        {
            Name = SelectedItemIndicatorPart
        };
        border.RegisterInNameScope(scope);
        TokenResourceBinder.CreateTokenBinding(border, Border.BackgroundProperty,
            TabControlTokenResourceKey.InkBarColor);

        layout.Children.Add(itemsPresenter);
        layout.Children.Add(border);
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        return layout;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());

        // 设置 items presenter 面板样式
        // 分为上、右、下、左
        {
            // 上
            var topStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.TopPC));

            var itemPresenterPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenResourceKey.HorizontalItemGutter);
            itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);

            topStyle.Add(itemPresenterPanelStyle);

            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(SelectedItemIndicatorPart));
            indicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            indicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom);
            topStyle.Add(indicatorStyle);

            topStyle.Add(itemPresenterPanelStyle);
            commonStyle.Add(topStyle);
        }

        {
            // 右
            var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.RightPC));

            var itemPresenterPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
            itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenResourceKey.VerticalItemGutter);
            rightStyle.Add(itemPresenterPanelStyle);

            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(SelectedItemIndicatorPart));
            indicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            indicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            rightStyle.Add(indicatorStyle);

            commonStyle.Add(rightStyle);
        }
        {
            // 下
            var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.BottomPC));

            var itemPresenterPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenResourceKey.HorizontalItemGutter);
            itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
            bottomStyle.Add(itemPresenterPanelStyle);

            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(SelectedItemIndicatorPart));
            indicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            indicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            bottomStyle.Add(indicatorStyle);

            commonStyle.Add(bottomStyle);
        }
        {
            // 左
            var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.LeftPC));

            var itemPresenterPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
            itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenResourceKey.VerticalItemGutter);
            leftStyle.Add(itemPresenterPanelStyle);

            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(SelectedItemIndicatorPart));
            indicatorStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            indicatorStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
            leftStyle.Add(indicatorStyle);

            commonStyle.Add(leftStyle);
        }

        Add(commonStyle);
    }
}