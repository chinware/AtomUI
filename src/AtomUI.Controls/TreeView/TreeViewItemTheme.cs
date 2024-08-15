using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Presenters;
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
      : base(typeof(TreeViewItem)) { }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<TreeViewItem>((treeViewItem, scope) =>
      {
         var stackPanel = new StackPanel()
         {
            Orientation = Orientation.Vertical
         };
         var frameDecorator = new Border()
         {
            Name = FrameDecoratorPart,
            Focusable = true
         };
         CreateTemplateParentBinding(frameDecorator, Border.BorderThicknessProperty, TreeViewItem.DragFrameBorderThicknessProperty);
         frameDecorator.RegisterInNameScope(scope);

         var treeItemLayout = new Grid()
         {
            Name = ItemsLayoutPart,
            ColumnDefinitions = new ColumnDefinitions()
            {
               new ColumnDefinition(GridLength.Auto),
               new ColumnDefinition(GridLength.Auto),
               new ColumnDefinition(GridLength.Auto),
               new ColumnDefinition(GridLength.Star)
            }
         };

         var indentConverter = new MarginMultiplierConverter()
         {
            Left = true,
            Indent = treeViewItem.TitleHeight
         };

         CreateTemplateParentBinding(treeItemLayout, Grid.MarginProperty,
                                     TreeViewItem.LevelProperty, BindingMode.OneWay,
                                     indentConverter);

         var nodeSwitcherButton = new NodeSwitcherButton()
         {
            Name = NodeSwitcherButtonPart,
            Focusable = false,
         };
         nodeSwitcherButton.RegisterInNameScope(scope);
         CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.CheckedIconProperty,
                                     TreeViewItem.SwitcherCollapseIconProperty);
         CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.UnCheckedIconProperty,
                                     TreeViewItem.SwitcherExpandIconProperty);
         CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.IsCheckedProperty,
                                     TreeViewItem.IsExpandedProperty, BindingMode.TwoWay);
         CreateTemplateParentBinding(nodeSwitcherButton, NodeSwitcherButton.IsLeafProperty,
                                     TreeViewItem.IsLeafProperty);
         
         treeItemLayout.Children.Add(nodeSwitcherButton);
         Grid.SetColumn(nodeSwitcherButton, 0);

         var checkbox = new CheckBox()
         {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
         };

         CreateTemplateParentBinding(checkbox, CheckBox.IsVisibleProperty, TreeViewItem.IsCheckboxVisibleProperty);
         CreateTemplateParentBinding(checkbox, CheckBox.IsEnabledProperty, TreeViewItem.IsCheckboxEnableProperty);
         CreateTemplateParentBinding(checkbox, CheckBox.IsCheckedProperty, TreeViewItem.IsCheckedProperty, BindingMode.TwoWay);
         
         treeItemLayout.Children.Add(checkbox);
         Grid.SetColumn(checkbox, 1);

         var iconContentPresenter = new ContentPresenter()
         {
            Name = IconPresenterPart,
            Cursor = new Cursor(StandardCursorType.Hand)
         };
         iconContentPresenter.RegisterInNameScope(scope);
         CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.ContentProperty, TreeViewItem.IconProperty);
         CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.IsEnabledProperty, TreeViewItem.IsEnabledProperty);
         CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.IsVisibleProperty, TreeViewItem.IconEffectiveVisibleProperty);
         
         treeItemLayout.Children.Add(iconContentPresenter);
         Grid.SetColumn(iconContentPresenter, 2);

         var contentPresenter = new ContentPresenter()
         {
            Name = HeaderPresenterPart,
            Cursor = new Cursor(StandardCursorType.Hand),
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
         };

         CreateTemplateParentBinding(contentPresenter, ContentPresenter.IsEnabledProperty, TreeViewItem.IsEnabledProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, TreeViewItem.HeaderProperty);
         CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                                     TreeViewItem.HeaderTemplateProperty);
         contentPresenter.RegisterInNameScope(scope);
         Grid.SetColumn(contentPresenter, 3);
         treeItemLayout.Children.Add(contentPresenter);

         frameDecorator.Child = treeItemLayout;
         var itemsPresenter = new ItemsPresenter()
         {
            Name = ItemsPresenterPart,
            Focusable = false
         };
         itemsPresenter.RegisterInNameScope(scope);
         CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty,
                                     TreeViewItem.ItemsPanelProperty);
         CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.IsVisibleProperty,
                                     TreeViewItem.IsExpandedProperty);
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
      commonStyle.Add(TreeViewItem.EffectiveNodeCornerRadiusProperty, GlobalResourceKey.BorderRadius);
      commonStyle.Add(TreeViewItem.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      commonStyle.Add(TreeViewItem.BorderThicknessProperty, GlobalResourceKey.BorderThickness);
      var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
      frameDecoratorStyle.Add(Border.HeightProperty, TreeViewResourceKey.TitleHeight);
      frameDecoratorStyle.Add(Border.MarginProperty, TreeViewResourceKey.TreeItemMargin);
      commonStyle.Add(frameDecoratorStyle);
      
      // 节点 Icon 的大小
      var treeItemIconStyle = new Style(selector => selector.Nesting().Template().Name(IconPresenterPart).Descendant().OfType<PathIcon>());
      treeItemIconStyle.Add(PathIcon.WidthProperty, GlobalResourceKey.IconSize);
      treeItemIconStyle.Add(PathIcon.HeightProperty, GlobalResourceKey.IconSize);
      treeItemIconStyle.Add(PathIcon.MarginProperty, TreeViewResourceKey.TreeNodeIconMargin);
      commonStyle.Add(treeItemIconStyle);
      
      // 设置 NodeHoverMode 为 Block 的情况
      {
         var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
         headerPresenterStyle.Add(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         commonStyle.Add(headerPresenterStyle);
      }

      var enabledStyle = new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsEnabledProperty, true));
      var blockNodeHoverModeStyle = new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.NodeHoverModeProperty, TreeItemHoverMode.Block));
      {
         var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
         headerPresenterStyle.Add(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
         headerPresenterStyle.Add(ContentPresenter.BackgroundProperty, GlobalResourceKey.ColorTransparent);
         headerPresenterStyle.Add(ContentPresenter.FontSizeProperty, GlobalResourceKey.FontSize);
         blockNodeHoverModeStyle.Add(headerPresenterStyle);
      }
      enabledStyle.Add(blockNodeHoverModeStyle);
      
      // header 样式
      {
         var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
         headerPresenterStyle.Add(ContentPresenter.PaddingProperty, TreeViewResourceKey.TreeItemHeaderPadding);
         headerPresenterStyle.Add(TreeViewItem.EffectiveNodeBgProperty, GlobalResourceKey.ColorBgContainer);
         enabledStyle.Add(headerPresenterStyle);
      }
      
      var hoverStyle = new Style(selector => selector.Nesting().Class(TreeViewItem.TreeNodeHoverPC));
      hoverStyle.Add(TreeViewItem.EffectiveNodeBgProperty, TreeViewResourceKey.NodeHoverBg);
      enabledStyle.Add(hoverStyle);

      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(TreeViewItem.EffectiveNodeBgProperty, TreeViewResourceKey.NodeSelectedBg);
      enabledStyle.Add(selectedStyle);

      commonStyle.Add(enabledStyle);
      
      Add(commonStyle);
   }

   private void BuildSwitcherButtonStyle()
   {
      {
         var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
         switcherButtonStyle.Add(NodeSwitcherButton.HeightProperty, TreeViewResourceKey.TitleHeight);
         switcherButtonStyle.Add(NodeSwitcherButton.WidthProperty, TreeViewResourceKey.TitleHeight);
         switcherButtonStyle.Add(NodeSwitcherButton.CursorProperty, new Cursor(StandardCursorType.Hand));
         Add(switcherButtonStyle);
      }

      var leafSwitcherButtonStyle = new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsLeafProperty, true));
      {
         var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
         switcherButtonStyle.Add(NodeSwitcherButton.CursorProperty, new Cursor(StandardCursorType.Arrow));
         leafSwitcherButtonStyle.Add(switcherButtonStyle);
      }
      Add(leafSwitcherButtonStyle);
      
      var leafAndHideButtonStyle = new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsLeafProperty, true)
                                                            .PropertyEquals(TreeViewItem.IsShowLeafSwitcherProperty, false));
      {
         var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
         switcherButtonStyle.Add(NodeSwitcherButton.IsIconVisibleProperty, false);
         leafAndHideButtonStyle.Add(switcherButtonStyle);
      }
      Add(leafAndHideButtonStyle);
      
      var checkboxVisibleStyle = new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsCheckboxVisibleProperty, true));
      {
         var switcherButtonStyle = new Style(selector => selector.Nesting().Template().Name(NodeSwitcherButtonPart));
         switcherButtonStyle.Add(NodeSwitcherButton.MarginProperty, TreeViewResourceKey.TreeNodeSwitcherMargin);
         checkboxVisibleStyle.Add(switcherButtonStyle);
      }
      Add(checkboxVisibleStyle);
   }

   private void BuildDisabledStyle()
   {
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      var headerPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
      headerPresenterStyle.Add(ContentPresenter.ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      disabledStyle.Add(headerPresenterStyle);
      Add(disabledStyle);
   }

   private void BuildDraggingStyle()
   {
      var draggingStyle = new Style(selector => selector.Nesting().PropertyEquals(TreeViewItem.IsDraggingProperty, true));
      var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
      frameDecoratorStyle.Add(Border.BorderBrushProperty, GlobalResourceKey.ColorPrimary);
      draggingStyle.Add(frameDecoratorStyle);
      Add(draggingStyle);
   }
}