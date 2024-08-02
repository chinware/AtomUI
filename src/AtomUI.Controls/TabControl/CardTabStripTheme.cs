using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CardTabStripTheme : BaseTabStripTheme
{
   public CardTabStripTheme() : base(typeof(CardTabStrip)) { }

   protected override void NotifyBuildControlTemplate(BaseTabStrip baseTabStrip, INameScope scope, Border container)
   {
      var tabScrollViewer = new TabScrollViewer();
      CreateTemplateParentBinding(tabScrollViewer, TabScrollViewer.TabStripPlacementProperty, TabStrip.TabStripPlacementProperty);
      var contentPanel = CreateTabStripContentPanel(scope);
      tabScrollViewer.Content = contentPanel;
      tabScrollViewer.TabStrip = baseTabStrip;
      container.Child = tabScrollViewer;
   }
   
   private ItemsPresenter CreateTabStripContentPanel(INameScope scope)
   {
      var itemsPresenter = new ItemsPresenter
      {
         Name = ItemsPresenterPart,
      };
      itemsPresenter.RegisterInNameScope(scope);
      
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, TabStrip.ItemsPanelProperty);
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
         var topStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.TopPC));
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.CardGutter);
         topStyle.Add(itemPresenterPanelStyle);
         
         commonStyle.Add(topStyle);
      }

      {
         // 右
         var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.RightPC));
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.CardVerticalGutter);
         rightStyle.Add(itemPresenterPanelStyle);
         
         commonStyle.Add(rightStyle);
      }
      {
         // 下
         var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.BottomPC));
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.CardGutter);
         bottomStyle.Add(itemPresenterPanelStyle);
         
         commonStyle.Add(bottomStyle);
      }
      {
         // 左
         var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.LeftPC));
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlResourceKey.CardVerticalGutter);
         leftStyle.Add(itemPresenterPanelStyle);
         
         commonStyle.Add(leftStyle);
      }

      Add(commonStyle);
   }
}