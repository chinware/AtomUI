using AtomUI.IconPkg;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TreeViewItemTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string ItemsPresenterMotionActorPart = "PART_ItemsPresenterMotionActor";
    public const string ItemsLayoutPart = "PART_ItemsLayoutPart";
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string IconPresenterPart = "PART_IconPresenter";
    public const string NodeSwitcherButtonPart = "PART_NodeSwitcherButton";

    public const double DisableOpacity = 0.3;

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
            var frame = new Border
            {
                Name      = FramePart,
                Focusable = true
            };
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty,
                TreeViewItem.DragFrameBorderThicknessProperty);
            frame.RegisterInNameScope(scope);

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
                Avalonia.Controls.TreeViewItem.LevelProperty, 
                BindingMode.OneWay,
                indentConverter);

            var nodeSwitcherButton = new NodeSwitcherButton
            {
                Name      = NodeSwitcherButtonPart,
                Focusable = false
            };
            nodeSwitcherButton.RegisterInNameScope(scope);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.IsMotionEnabledProperty,
                TreeViewItem.IsMotionEnabledProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.IconModeProperty,
                TreeViewItem.SwitcherModeProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.CollapseIconProperty,
                TreeViewItem.SwitcherCollapseIconProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.ExpandIconProperty,
                TreeViewItem.SwitcherExpandIconProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.RotationIconProperty,
                TreeViewItem.SwitcherRotationIconProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.LoadingIconProperty,
                TreeViewItem.SwitcherLoadingIconProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.LeafIconProperty,
                TreeViewItem.SwitcherLeafIconProperty);
            CreateTemplateParentBinding(nodeSwitcherButton, ToggleButton.IsCheckedProperty,
                Avalonia.Controls.TreeViewItem.IsExpandedProperty, BindingMode.TwoWay);
            CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.IsLoadingProperty,
                TreeViewItem.IsLoadingProperty);

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
                HeaderedItemsControl.HeaderProperty,
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
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                HeaderedItemsControl.HeaderTemplateProperty);
            contentPresenter.RegisterInNameScope(scope);
            Grid.SetColumn(contentPresenter, 3);
            treeItemLayout.Children.Add(contentPresenter);

            frame.Child = treeItemLayout;
            
            var itemsPresenterMotionActor = new MotionActorControl
            {
                Name         = ItemsPresenterMotionActorPart,
                ClipToBounds = true,
                IsVisible = false
            };
            itemsPresenterMotionActor.RegisterInNameScope(scope);
            
            var itemsPresenter = new ItemsPresenter
            {
                Name      = ItemsPresenterPart,
                Focusable = false
            };
            itemsPresenterMotionActor.Child = itemsPresenter;
            itemsPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty,
                ItemsControl.ItemsPanelProperty);
            stackPanel.Children.Add(frame);
            stackPanel.Children.Add(itemsPresenterMotionActor);
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
        commonStyle.Add(TreeViewItem.EffectiveNodeCornerRadiusProperty, SharedTokenKey.BorderRadius);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorder);
        commonStyle.Add(TemplatedControl.BorderThicknessProperty, SharedTokenKey.BorderThickness);
        commonStyle.Add(TreeViewItem.EffectiveNodeBgProperty, Brushes.Transparent);
        commonStyle.Add(TreeViewItem.MotionDurationProperty, SharedTokenKey.MotionDurationSlow);
        commonStyle.Add(TreeViewItem.TitleHeightProperty, TreeViewTokenKey.TitleHeight);
        
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Layoutable.HeightProperty, TreeViewTokenKey.TitleHeight);
        frameStyle.Add(Layoutable.MarginProperty, TreeViewTokenKey.TreeItemMargin);
        commonStyle.Add(frameStyle);

        // 节点 Icon 的大小
        var treeItemIconStyle = new Style(selector =>
            selector.Nesting().Template().Name(IconPresenterPart).Descendant().OfType<Icon>());
        treeItemIconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
        treeItemIconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
        treeItemIconStyle.Add(Layoutable.MarginProperty, TreeViewTokenKey.TreeNodeIconMargin);
        commonStyle.Add(treeItemIconStyle);

        // 设置 NodeHoverMode 为 Block 的情况
        {
            var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            headerPresenterStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            headerPresenterStyle.Add(Layoutable.MarginProperty, TreeViewTokenKey.TreeItemHeaderMargin);
            commonStyle.Add(headerPresenterStyle);
        }

        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        var blockNodeHoverModeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TreeViewItem.NodeHoverModeProperty, TreeItemHoverMode.Block));
        {
            var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            headerPresenterStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            headerPresenterStyle.Add(ContentPresenter.BackgroundProperty, Brushes.Transparent);
            headerPresenterStyle.Add(ContentPresenter.FontSizeProperty, SharedTokenKey.FontSize);
            blockNodeHoverModeStyle.Add(headerPresenterStyle);
        }
        enabledStyle.Add(blockNodeHoverModeStyle);

        // header 样式
        {
            var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
            headerPresenterStyle.Add(ContentPresenter.PaddingProperty, TreeViewTokenKey.TreeItemHeaderPadding);
            commonStyle.Add(headerPresenterStyle);
        }

        var hoverStyle = new Style(selector => selector.Nesting().Class(TreeViewItem.TreeNodeHoverPC));
        hoverStyle.Add(TreeViewItem.EffectiveNodeBgProperty, TreeViewTokenKey.NodeHoverBg);
        enabledStyle.Add(hoverStyle);

        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(TreeViewItem.EffectiveNodeBgProperty, TreeViewTokenKey.NodeSelectedBg);
        enabledStyle.Add(selectedStyle);

        commonStyle.Add(enabledStyle);

        Add(commonStyle);
    }

    private void BuildSwitcherButtonStyle()
    {
        {
            var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
            switcherButtonStyle.Add(Layoutable.HeightProperty, TreeViewTokenKey.TitleHeight);
            switcherButtonStyle.Add(Layoutable.WidthProperty, TreeViewTokenKey.TitleHeight);
            switcherButtonStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
            Add(switcherButtonStyle);
        }

        var leafSwitcherButtonStyle =
            new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsLeafProperty, true));
        {
            var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
            switcherButtonStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Arrow)));
            leafSwitcherButtonStyle.Add(switcherButtonStyle);
        }
        Add(leafSwitcherButtonStyle);

        var leafAndHideButtonStyle = new Style(selector => Selectors.Or(selector
            .Nesting().PropertyEquals(TreeViewItem.IsLeafProperty, true)
            .PropertyEquals(TreeViewItem.IsShowLeafIconProperty,
                false),
            selector
                .Nesting().PropertyEquals(TreeViewItem.IsLeafProperty, false)));
        {
            var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
            switcherButtonStyle.Add(NodeSwitcherButton.IsLeafIconVisibleProperty, false);
            leafAndHideButtonStyle.Add(switcherButtonStyle);
        }
        Add(leafAndHideButtonStyle);

        var checkboxVisibleStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(TreeViewItem.IsCheckboxVisibleProperty, true));
        {
            var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
            switcherButtonStyle.Add(Layoutable.MarginProperty, TreeViewTokenKey.TreeNodeSwitcherMargin);
            checkboxVisibleStyle.Add(switcherButtonStyle);
        }
        Add(checkboxVisibleStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle        = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
        headerPresenterStyle.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        
        disabledStyle.Add(headerPresenterStyle);
        
        var iconPresenterStyle = new Style(selector => selector.Nesting().Template().Name(IconPresenterPart));
        iconPresenterStyle.Add(ContentPresenter.OpacityProperty, DisableOpacity);
        disabledStyle.Add(iconPresenterStyle);

        var nodeSwitcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
        nodeSwitcherButtonStyle.Add(ContentPresenter.OpacityProperty, DisableOpacity);
        disabledStyle.Add(nodeSwitcherButtonStyle);
        
        Add(disabledStyle);
    }

    private void BuildDraggingStyle()
    {
        var draggingStyle =
            new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsDraggingProperty, true));
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorPrimary);
        draggingStyle.Add(frameStyle);
        Add(draggingStyle);
    }
}