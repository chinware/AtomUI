using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
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
internal class CardTabControlTheme : BaseTabControlTheme
{
    public const string AddTabButtonPart = "PART_AddTabButton";
    public const string CardTabStripScrollViewerPart = "PART_CardTabStripScrollViewer";

    public CardTabControlTheme() : base(typeof(CardTabControl))
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

        var cardTabControlContainer = new TabsContainerPanel
        {
            Name = TabsContainerPart
        };
        cardTabControlContainer.RegisterInNameScope(scope);
        CreateTemplateParentBinding(cardTabControlContainer, TabsContainerPanel.TabStripPlacementProperty,
            BaseTabStrip.TabStripPlacementProperty);

        var tabScrollViewer = new TabControlScrollViewer
        {
            Name = CardTabStripScrollViewerPart
        };
        CreateTemplateParentBinding(tabScrollViewer, BaseTabScrollViewer.TabStripPlacementProperty,
            Avalonia.Controls.TabControl.TabStripPlacementProperty);
        tabScrollViewer.RegisterInNameScope(scope);
        var contentPanel = CreateTabStripContentPanel(scope);
        tabScrollViewer.Content    = contentPanel;
        tabScrollViewer.TabControl = baseTabControl;
        
        var addTabIcon = AntDesignIconPackage.PlusOutlined();

        baseTabControl.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(addTabIcon, Icon.NormalFilledBrushProperty,
            TabControlTokenKey.ItemColor));
        baseTabControl.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(addTabIcon, Icon.ActiveFilledBrushProperty,
            TabControlTokenKey.ItemHoverColor));
        baseTabControl.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(addTabIcon, Icon.DisabledFilledBrushProperty,
            SharedTokenKey.ColorTextDisabled));

        var addTabButton = new IconButton
        {
            Name            = AddTabButtonPart,
            BorderThickness = new Thickness(1),
            Icon            = addTabIcon
        };
        DockPanel.SetDock(addTabButton, Dock.Right);
        baseTabControl.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(addTabButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSize));
        baseTabControl.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(addTabButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSize));
        CreateTemplateParentBinding(addTabButton, TemplatedControl.BorderThicknessProperty,
            CardTabControl.CardBorderThicknessProperty);
        CreateTemplateParentBinding(addTabButton, TemplatedControl.CornerRadiusProperty,
            CardTabControl.CardBorderRadiusProperty);
        CreateTemplateParentBinding(addTabButton, Visual.IsVisibleProperty, CardTabControl.IsShowAddTabButtonProperty);

        baseTabControl.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(addTabButton, TemplatedControl.BorderBrushProperty,
            SharedTokenKey.ColorBorderSecondary));

        addTabButton.RegisterInNameScope(scope);

        cardTabControlContainer.TabScrollViewer = tabScrollViewer;
        cardTabControlContainer.AddTabButton    = addTabButton;

        alignWrapper.Children.Add(cardTabControlContainer);

        container.Children.Add(alignWrapper);
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

        // 设置 items presenter 面板样式
        // 分为上、右、下、左
        {
            // 上
            var topStyle       = new Style(selector => selector.Nesting().Class(BaseTabControl.TopPC));
            var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
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
            var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.RightPC));

            var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
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
            var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.BottomPC));

            var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
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
            var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.LeftPC));

            var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
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