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
   
   public TabStripTheme() : base(typeof(TabStrip)) { }
   
   protected override void NotifyBuildControlTemplate(BaseTabStrip baseTabStrip, INameScope scope, Border container)
   {
      var layout = new Panel();
      var itemsPresenter = new ItemsPresenter()
      {
         Name = ItemsPresenterPart,
      };
      itemsPresenter.RegisterInNameScope(scope);
      var border = new Border
      {
         Name = SelectedItemIndicatorPart,
      };
      border.RegisterInNameScope(scope);
      TokenResourceBinder.CreateTokenBinding(border, Border.BackgroundProperty, TabControlResourceKey.InkBarColor);
      
      layout.Children.Add(itemsPresenter);
      layout.Children.Add(border);
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, TabStrip.ItemsPanelProperty);
      container.Child = layout;
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
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().OfType<ItemsPresenter>().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.HorizontalItemGutter);

         var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(SelectedItemIndicatorPart));
         indicatorStyle.Add(Border.HeightProperty, GlobalResourceKey.LineWidthBold);
         indicatorStyle.Add(Border.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         indicatorStyle.Add(Border.VerticalAlignmentProperty, VerticalAlignment.Bottom);
         topStyle.Add(indicatorStyle);
         
         topStyle.Add(itemPresenterPanelStyle);
         commonStyle.Add(topStyle);
      }

      {
         // 右
         var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.RightPC));
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().OfType<ItemsPresenter>().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.HorizontalItemGutter);
         rightStyle.Add(itemPresenterPanelStyle);
         
         var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(SelectedItemIndicatorPart));
         indicatorStyle.Add(Border.WidthProperty, GlobalResourceKey.LineWidthBold);
         indicatorStyle.Add(Border.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         indicatorStyle.Add(Border.VerticalAlignmentProperty, VerticalAlignment.Top);
         rightStyle.Add(indicatorStyle);
         
         commonStyle.Add(rightStyle);
      }
      {
         // 下
         var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.BottomPC));
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().OfType<ItemsPresenter>().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.HorizontalItemGutter);
         bottomStyle.Add(itemPresenterPanelStyle);
         
         var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(SelectedItemIndicatorPart));
         indicatorStyle.Add(Border.HeightProperty, GlobalResourceKey.LineWidthBold);
         indicatorStyle.Add(Border.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         indicatorStyle.Add(Border.VerticalAlignmentProperty, VerticalAlignment.Top);
         bottomStyle.Add(indicatorStyle);
         
         commonStyle.Add(bottomStyle);
      }
      {
         // 左
         var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.LeftPC));
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().OfType<ItemsPresenter>().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.HorizontalItemGutter);
         leftStyle.Add(itemPresenterPanelStyle);
         
         var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(SelectedItemIndicatorPart));
         indicatorStyle.Add(Border.WidthProperty, GlobalResourceKey.LineWidthBold);
         indicatorStyle.Add(Border.HorizontalAlignmentProperty, HorizontalAlignment.Right);
         indicatorStyle.Add(Border.VerticalAlignmentProperty, VerticalAlignment.Top);
         leftStyle.Add(indicatorStyle);
         
         commonStyle.Add(leftStyle);
      }

      Add(commonStyle);
   }
}