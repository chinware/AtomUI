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

         var contentPresenter = new ContentPresenter()
         {
            Name = HeaderPresenterPart,
            Cursor = new Cursor(StandardCursorType.Hand),
            HorizontalAlignment = HorizontalAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center
         };

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
   }

   private void BuildCommonStyle()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(TreeViewItem.EffectiveNodeCornerRadiusProperty, GlobalResourceKey.BorderRadius);
      var frameDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
      frameDecoratorStyle.Add(Border.HeightProperty, TreeViewResourceKey.TitleHeight);
      frameDecoratorStyle.Add(Border.MarginProperty, TreeViewResourceKey.TreeItemMargin);
      commonStyle.Add(frameDecoratorStyle);
      
      // header 样式
      var headerStyle = new Style(selector => selector.Nesting().Template().Name(HeaderPresenterPart));
      headerStyle.Add(ContentPresenter.PaddingProperty, TreeViewResourceKey.TreeItemHeaderPadding);
      commonStyle.Add(headerStyle);

      var hoverStyle = new Style(selector => selector.Nesting().Class(TreeViewItem.TreeNodeHoverPC));
      hoverStyle.Add(TreeViewItem.EffectiveNodeBgProperty, TreeViewResourceKey.NodeHoverBg);
      commonStyle.Add(hoverStyle);

      var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
      selectedStyle.Add(TreeViewItem.EffectiveNodeBgProperty, TreeViewResourceKey.NodeSelectedBg);
      commonStyle.Add(selectedStyle);
      
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
}