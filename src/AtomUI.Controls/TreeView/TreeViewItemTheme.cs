using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TreeViewItemTheme : BaseControlTheme
{
    public const string FrameDecoratorPart = "PART_FrameDecorator";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string ItemsLayoutPart = "PART_ItemsLayoutPart";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string IconPresenterPart = "PART_IconPresenter";
    public const string NodeSwitcherButtonPart = "PART_NodeSwitcherButton";

    public TreeViewItemTheme()
        : base(typeof(TreeViewItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TreeViewItem>((treeViewItem, scope) =>
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            var frameDecorator = new Border
            {
                Name      = FrameDecoratorPart,
                Focusable = true
            };
            CreateTemplateParentBinding(frameDecorator, Border.BorderThicknessProperty,
                TreeViewItem.DragFrameBorderThicknessProperty);
            frameDecorator.RegisterInNameScope(scope);

            var treeItemLayout = new Grid
            {
                Name = ItemsLayoutPart,
                ColumnDefinitions = new ColumnDefinitions
                {
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Star)
                }
            };

            var indentConverter = new MarginMultiplierConverter
            {
                Left   = true,
                Indent = treeViewItem.TitleHeight
            };

            CreateTemplateParentBinding(treeItemLayout, Layoutable.MarginProperty,
                Avalonia.Controls.TreeViewItem.LevelProperty, BindingMode.OneWay,
                indentConverter);

            var nodeSwitcherButton = new NodeSwitcherButton
            {
                Name      = NodeSwitcherButtonPart,
                Focusable = false
            };
            nodeSwitcherButton.RegisterInNameScope(scope);
            CreateTemplateParentBinding(nodeSwitcherButton, ToggleIconButton.CheckedIconProperty,
                TreeViewItem.SwitcherCollapseIconProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, ToggleIconButton.UnCheckedIconProperty,
                TreeViewItem.SwitcherExpandIconProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, ToggleButton.IsCheckedProperty,
                Avalonia.Controls.TreeViewItem.IsExpandedProperty, BindingMode.TwoWay);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.IsLeafProperty,
                TreeViewItem.IsLeafProperty);

            treeItemLayout.Children.Add(nodeSwitcherButton);
            Grid.SetColumn(nodeSwitcherButton, 0);

            var checkbox = new CheckBox
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Center
            };

            CreateTemplateParentBinding(checkbox, Visual.IsVisibleProperty, TreeViewItem.IsCheckboxVisibleProperty);
            CreateTemplateParentBinding(checkbox, InputElement.IsEnabledProperty,
                TreeViewItem.IsCheckboxEnableProperty);
            CreateTemplateParentBinding(checkbox, ToggleButton.IsCheckedProperty, TreeViewItem.IsCheckedProperty,
                BindingMode.TwoWay);

            treeItemLayout.Children.Add(checkbox);
            Grid.SetColumn(checkbox, 1);

            var iconContentPresenter = new ContentPresenter
            {
                Name   = IconPresenterPart,
                Cursor = new Cursor(StandardCursorType.Hand)
            };
            iconContentPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.ContentProperty,
                TreeViewItem.IconProperty);
            CreateTemplateParentBinding(iconContentPresenter, InputElement.IsEnabledProperty,
                InputElement.IsEnabledProperty);
            CreateTemplateParentBinding(iconContentPresenter, Visual.IsVisibleProperty,
                TreeViewItem.IconEffectiveVisibleProperty);

            treeItemLayout.Children.Add(iconContentPresenter);
            Grid.SetColumn(iconContentPresenter, 2);

            var contentPresenter = new ContentPresenter
            {
                Name                       = HeaderPresenterPart,
                Cursor                     = new Cursor(StandardCursorType.Hand),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalAlignment          = VerticalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
            };

            CreateTemplateParentBinding(contentPresenter, InputElement.IsEnabledProperty,
                InputElement.IsEnabledProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                HeaderedItemsControl.HeaderProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                HeaderedItemsControl.HeaderTemplateProperty);
            contentPresenter.RegisterInNameScope(scope);
            Grid.SetColumn(contentPresenter, 3);
            treeItemLayout.Children.Add(contentPresenter);

            frameDecorator.Child = treeItemLayout;
            var itemsPresenter = new ItemsPresenter
            {
                Name      = ItemsPresenterPart,
                Focusable = false
            };
            itemsPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty,
                ItemsControl.ItemsPanelProperty);
            CreateTemplateParentBinding(itemsPresenter, Visual.IsVisibleProperty,
                Avalonia.Controls.TreeViewItem.IsExpandedProperty);
            stackPanel.Children.Add(frameDecorator);
            stackPanel.Children.Add(itemsPresenter);
            return stackPanel;
        });
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildSwitcherButtonStyle();
        BuildDisabledStyle();
        BuildDraggingStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TreeViewItem.EffectiveNodeCornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
        commonStyle.Add(TemplatedControl.BorderThicknessProperty, GlobalTokenResourceKey.BorderThickness);
        commonStyle.Add(TreeViewItem.EffectiveNodeBgProperty, GlobalTokenResourceKey.ColorTransparent);
        var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
        frameDecoratorStyle.Add(Layoutable.HeightProperty, TreeViewTokenResourceKey.TitleHeight);
        frameDecoratorStyle.Add(Layoutable.MarginProperty, TreeViewTokenResourceKey.TreeItemMargin);
        commonStyle.Add(frameDecoratorStyle);

        // 节点 Icon 的大小
        var treeItemIconStyle = new Style(selector =>
            selector.Nesting().Template().Name(IconPresenterPart).Descendant().OfType<Icon>());
        treeItemIconStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSize);
        treeItemIconStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSize);
        treeItemIconStyle.Add(Layoutable.MarginProperty, TreeViewTokenResourceKey.TreeNodeIconMargin);
        commonStyle.Add(treeItemIconStyle);

        // 设置 NodeHoverMode 为 Block 的情况
        {
            var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            headerPresenterStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            headerPresenterStyle.Add(Layoutable.MarginProperty, TreeViewTokenResourceKey.TreeItemHeaderMargin);
            commonStyle.Add(headerPresenterStyle);
        }

        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        var blockNodeHoverModeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TreeViewItem.NodeHoverModeProperty, TreeItemHoverMode.Block));
        {
            var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            headerPresenterStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            headerPresenterStyle.Add(ContentPresenter.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
            headerPresenterStyle.Add(ContentPresenter.FontSizeProperty, GlobalTokenResourceKey.FontSize);
            blockNodeHoverModeStyle.Add(headerPresenterStyle);
        }
        enabledStyle.Add(blockNodeHoverModeStyle);

        // header 样式
        {
            var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            headerPresenterStyle.Add(ContentPresenter.PaddingProperty, TreeViewTokenResourceKey.TreeItemHeaderPadding);
            commonStyle.Add(headerPresenterStyle);
        }

        var hoverStyle = new Style(selector => selector.Nesting().Class(TreeViewItem.TreeNodeHoverPC));
        hoverStyle.Add(TreeViewItem.EffectiveNodeBgProperty, TreeViewTokenResourceKey.NodeHoverBg);
        enabledStyle.Add(hoverStyle);

        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(TreeViewItem.EffectiveNodeBgProperty, TreeViewTokenResourceKey.NodeSelectedBg);
        enabledStyle.Add(selectedStyle);

        commonStyle.Add(enabledStyle);

        Add(commonStyle);
    }

    private void BuildSwitcherButtonStyle()
    {
        {
            var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
            switcherButtonStyle.Add(Layoutable.HeightProperty, TreeViewTokenResourceKey.TitleHeight);
            switcherButtonStyle.Add(Layoutable.WidthProperty, TreeViewTokenResourceKey.TitleHeight);
            switcherButtonStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));
            Add(switcherButtonStyle);
        }

        var leafSwitcherButtonStyle =
            new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsLeafProperty, true));
        {
            var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
            switcherButtonStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Arrow));
            leafSwitcherButtonStyle.Add(switcherButtonStyle);
        }
        Add(leafSwitcherButtonStyle);

        var leafAndHideButtonStyle = new Style(selector => selector
                                                           .Nesting().PropertyEquals(TreeViewItem.IsLeafProperty, true)
                                                           .PropertyEquals(TreeViewItem.IsShowLeafSwitcherProperty,
                                                               false));
        {
            var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
            switcherButtonStyle.Add(NodeSwitcherButton.IsIconVisibleProperty, false);
            leafAndHideButtonStyle.Add(switcherButtonStyle);
        }
        Add(leafAndHideButtonStyle);

        var checkboxVisibleStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TreeViewItem.IsCheckboxVisibleProperty, true));
        {
            var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
            switcherButtonStyle.Add(Layoutable.MarginProperty, TreeViewTokenResourceKey.TreeNodeSwitcherMargin);
            checkboxVisibleStyle.Add(switcherButtonStyle);
        }
        Add(checkboxVisibleStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle        = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        headerPresenterStyle.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
        disabledStyle.Add(headerPresenterStyle);
        Add(disabledStyle);
    }

    private void BuildDraggingStyle()
    {
        var draggingStyle =
            new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsDraggingProperty, true));
        var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
        frameDecoratorStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorPrimary);
        draggingStyle.Add(frameDecoratorStyle);
        Add(draggingStyle);
    }
}