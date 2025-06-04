using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CardTabStripTheme : BaseTabStripTheme
{
    public const string AddTabButtonPart = "PART_AddTabButton";
    public const string CardTabStripContainerPart = "PART_CardTabStripContainer";
    public const string CardTabStripScrollViewerPart = "PART_CardTabStripScrollViewer";

    public CardTabStripTheme() : base(typeof(CardTabStrip))
    {
    }

    protected override void NotifyBuildControlTemplate(BaseTabStrip baseTabStrip, INameScope scope, Border container)
    {
        var alignWrapper = new Panel
        {
            Name = AlignWrapperPart
        };
        var cardTabStripContainer = new TabsContainerPanel
        {
            Name = TabsContainerPart
        };
        cardTabStripContainer.RegisterInNameScope(scope);
        CreateTemplateParentBinding(cardTabStripContainer, TabsContainerPanel.TabStripPlacementProperty,
            BaseTabStrip.TabStripPlacementProperty);

        var tabScrollViewer = new TabStripScrollViewer
        {
            Name = CardTabStripScrollViewerPart
        };
        tabScrollViewer.IsScrollChainingEnabled = false;
        tabScrollViewer.RegisterInNameScope(scope);
        CreateTemplateParentBinding(tabScrollViewer, BaseTabScrollViewer.TabStripPlacementProperty,
            BaseTabStrip.TabStripPlacementProperty);
        CreateTemplateParentBinding(tabScrollViewer, BaseTabScrollViewer.IsMotionEnabledProperty,
            BaseTabStrip.IsMotionEnabledProperty);
        var contentPanel = CreateTabStripContentPanel(scope);
        tabScrollViewer.Content  = contentPanel;
        tabScrollViewer.TabStrip = baseTabStrip;
        
        var addTabButton = new IconButton
        {
            Name            = AddTabButtonPart,
            Icon            = AntDesignIconPackage.PlusOutlined()
        };

        CreateTemplateParentBinding(addTabButton, TemplatedControl.BorderThicknessProperty,
            CardTabStrip.CardBorderThicknessProperty);
        CreateTemplateParentBinding(addTabButton, TemplatedControl.CornerRadiusProperty,
            CardTabStrip.CardBorderRadiusProperty);
        CreateTemplateParentBinding(addTabButton, Visual.IsVisibleProperty, CardTabStrip.IsShowAddTabButtonProperty);

        addTabButton.RegisterInNameScope(scope);

        cardTabStripContainer.TabScrollViewer = tabScrollViewer;
        cardTabStripContainer.AddTabButton    = addTabButton;

        alignWrapper.Children.Add(cardTabStripContainer);

        container.Child = alignWrapper;
    }

    private ItemsPresenter CreateTabStripContentPanel(INameScope scope)
    {
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        itemsPresenter.RegisterInNameScope(scope);

        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        return itemsPresenter;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());

        {
            var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
            addTabButtonStyle.Add(IconButton.NormalIconBrushProperty, TabControlTokenKey.ItemColor);
            addTabButtonStyle.Add(IconButton.ActiveIconBrushProperty, TabControlTokenKey.ItemHoverColor);
            addTabButtonStyle.Add(IconButton.DisabledIconBrushProperty, SharedTokenKey.ColorTextDisabled);
            addTabButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSize);
            addTabButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSize);
            addTabButtonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorderSecondary);

            commonStyle.Add(addTabButtonStyle);
        }
        
        // 设置 items presenter 面板样式
        // 分为上、右、下、左
        {
            // 上
            var topStyle       = new Style(selector => selector.Nesting().Class(BaseTabStrip.TopPC));
            var containerStyle = new Style(selector => selector.Nesting().Template().Name(CardTabStripContainerPart));
            topStyle.Add(containerStyle);

            var itemPresenterPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
            itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenKey.CardGutter);

            var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
            addTabButtonStyle.Add(Layoutable.MarginProperty, TabControlTokenKey.AddTabButtonMarginHorizontal);
            topStyle.Add(addTabButtonStyle);

            topStyle.Add(itemPresenterPanelStyle);
            commonStyle.Add(topStyle);
        }

        {
            // 右
            var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.RightPC));

            var containerStyle = new Style(selector => selector.Nesting().Template().Name(CardTabStripContainerPart));
            containerStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
            rightStyle.Add(containerStyle);

            var itemPresenterPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
            itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenKey.CardGutter);
            rightStyle.Add(itemPresenterPanelStyle);

            var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
            addTabButtonStyle.Add(Layoutable.MarginProperty, TabControlTokenKey.AddTabButtonMarginVertical);
            rightStyle.Add(addTabButtonStyle);

            commonStyle.Add(rightStyle);
        }
        {
            // 下
            var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.BottomPC));

            var containerStyle = new Style(selector => selector.Nesting().Template().Name(CardTabStripContainerPart));
            containerStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
            bottomStyle.Add(containerStyle);

            var itemPresenterPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
            itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenKey.CardGutter);
            bottomStyle.Add(itemPresenterPanelStyle);

            var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
            addTabButtonStyle.Add(Layoutable.MarginProperty, TabControlTokenKey.AddTabButtonMarginHorizontal);
            bottomStyle.Add(addTabButtonStyle);

            commonStyle.Add(bottomStyle);
        }
        {
            // 左
            var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.LeftPC));

            var containerStyle = new Style(selector => selector.Nesting().Template().Name(CardTabStripContainerPart));
            containerStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
            leftStyle.Add(containerStyle);

            var itemPresenterPanelStyle = new Style(selector =>
                selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
            itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
            itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenKey.CardGutter);
            leftStyle.Add(itemPresenterPanelStyle);

            var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
            addTabButtonStyle.Add(Layoutable.MarginProperty, TabControlTokenKey.AddTabButtonMarginVertical);
            leftStyle.Add(addTabButtonStyle);

            commonStyle.Add(leftStyle);
        }

        Add(commonStyle);
    }
}