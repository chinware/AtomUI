using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TabControlTheme : BaseTabControlTheme
{
    public const string SelectedItemIndicatorPart = "PART_SelectedItemIndicator";

    public TabControlTheme() : base(typeof(TabControl))
    {
    }

    protected override void NotifyBuildTabStripTemplate(BaseTabControl baseTabControl, INameScope scope,
        DockPanel container)
    {
        var alignWrapper = new Panel
        {
            Name = AlignWrapperPart
        };
        alignWrapper.RegisterInNameScope(scope);
        CreateTemplateParentBinding(alignWrapper, DockPanel.DockProperty,
            Avalonia.Controls.TabControl.TabStripPlacementProperty);
        CreateTemplateParentBinding(alignWrapper, Layoutable.MarginProperty, BaseTabControl.TabStripMarginProperty);

        var tabScrollViewer = new TabControlScrollViewer
        {
            Name = TabsContainerPart
        };
        CreateTemplateParentBinding(tabScrollViewer, BaseTabScrollViewer.TabStripPlacementProperty,
            BaseTabStrip.TabStripPlacementProperty);

        var contentPanel = CreateTabStripContentPanel(scope);
        tabScrollViewer.Content    = contentPanel;
        tabScrollViewer.TabControl = baseTabControl;
        tabScrollViewer.RegisterInNameScope(scope);

        alignWrapper.Children.Add(tabScrollViewer);
        container.Children.Add(alignWrapper);
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
            var topStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.TopPC));

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
            var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.RightPC));

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
            var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.BottomPC));

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
            var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.LeftPC));

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