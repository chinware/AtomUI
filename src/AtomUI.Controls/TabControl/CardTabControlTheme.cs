using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CardTabControlTheme : BaseTabControlTheme
{
   public const string AddTabButtonPart             = "PART_AddTabButton";
   public const string CardTabStripScrollViewerPart = "PART_CardTabStripScrollViewer";
   
   public CardTabControlTheme() : base(typeof(CardTabControl)) { }
   
    protected override void NotifyBuildTabStripTemplate(BaseTabControl baseTabControl, INameScope scope, DockPanel container)
    {
       var alignWrapper = new Panel()
       {
          Name = AlignWrapperPart
       };
       alignWrapper.RegisterInNameScope(scope);
       CreateTemplateParentBinding(alignWrapper, DockPanel.DockProperty, BaseTabControl.TabStripPlacementProperty);
       CreateTemplateParentBinding(alignWrapper, Panel.MarginProperty,TabControl.TabStripMarginProperty);
       
      var cardTabControlContainer = new TabsContainerPanel()
      {
         Name = TabsContainerPart,
      };
      cardTabControlContainer.RegisterInNameScope(scope);
      CreateTemplateParentBinding(cardTabControlContainer, TabsContainerPanel.TabStripPlacementProperty, TabStrip.TabStripPlacementProperty);
      
      var tabScrollViewer = new TabControlScrollViewer()
      {
         Name = CardTabStripScrollViewerPart
      };
      CreateTemplateParentBinding(tabScrollViewer, TabControlScrollViewer.TabStripPlacementProperty, BaseTabControl.TabStripPlacementProperty);
      tabScrollViewer.RegisterInNameScope(scope);
      var contentPanel = CreateTabStripContentPanel(scope);
      tabScrollViewer.Content = contentPanel;
      tabScrollViewer.TabControl = baseTabControl;

      var addTabIcon = new PathIcon()
      {
         Kind = "PlusOutlined"
      };

      TokenResourceBinder.CreateTokenBinding(addTabIcon, PathIcon.NormalFilledBrushProperty, TabControlTokenResourceKey.ItemColor);
      TokenResourceBinder.CreateTokenBinding(addTabIcon, PathIcon.ActiveFilledBrushProperty, TabControlTokenResourceKey.ItemHoverColor);
      TokenResourceBinder.CreateTokenBinding(addTabIcon, PathIcon.DisabledFilledBrushProperty, GlobalTokenResourceKey.ColorTextDisabled);
      
      TokenResourceBinder.CreateGlobalResourceBinding(addTabIcon, PathIcon.WidthProperty, GlobalTokenResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalResourceBinding(addTabIcon, PathIcon.HeightProperty, GlobalTokenResourceKey.IconSize);

      var addTabButton = new IconButton
      {
         Name = AddTabButtonPart,
         BorderThickness = new Thickness(1),
         Icon = addTabIcon
      };
      DockPanel.SetDock(addTabButton, Dock.Right);
      
      CreateTemplateParentBinding(addTabButton, IconButton.BorderThicknessProperty, CardTabControl.CardBorderThicknessProperty);
      CreateTemplateParentBinding(addTabButton, IconButton.CornerRadiusProperty, CardTabControl.CardBorderRadiusProperty);
      CreateTemplateParentBinding(addTabButton, IconButton.IsVisibleProperty, CardTabControl.IsShowAddTabButtonProperty);
      
      TokenResourceBinder.CreateGlobalResourceBinding(addTabButton, IconButton.BorderBrushProperty, GlobalTokenResourceKey.ColorBorderSecondary);
      
      addTabButton.RegisterInNameScope(scope);
      
      cardTabControlContainer.TabScrollViewer = tabScrollViewer;
      cardTabControlContainer.AddTabButton = addTabButton;
      
      alignWrapper.Children.Add(cardTabControlContainer); 
      
      container.Children.Add(alignWrapper);
   }
   
   private ItemsPresenter CreateTabStripContentPanel(INameScope scope)
   {
      var itemsPresenter = new ItemsPresenter
      {
         Name = ItemsPresenterPart,
      };
      itemsPresenter.RegisterInNameScope(scope);
      
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, TabControl.ItemsPanelProperty);
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
         var topStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.TopPC));
         var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
         topStyle.Add(containerStyle);
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenResourceKey.CardGutter);

         var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
         addTabButtonStyle.Add(IconButton.MarginProperty, TabControlTokenResourceKey.AddTabButtonMarginHorizontal);
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
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenResourceKey.CardGutter);
         rightStyle.Add(itemPresenterPanelStyle);
         
         var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
         addTabButtonStyle.Add(IconButton.MarginProperty, TabControlTokenResourceKey.AddTabButtonMarginVertical);
         rightStyle.Add(addTabButtonStyle);
         
         commonStyle.Add(rightStyle);
      }
      {
         // 下
         var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.BottomPC));
         
         var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
         containerStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
         bottomStyle.Add(containerStyle);
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Horizontal);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenResourceKey.CardGutter);
         bottomStyle.Add(itemPresenterPanelStyle);
         
         var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
         addTabButtonStyle.Add(IconButton.MarginProperty, TabControlTokenResourceKey.AddTabButtonMarginHorizontal);
         bottomStyle.Add(addTabButtonStyle);
         
         commonStyle.Add(bottomStyle);
      }
      {
         // 左
         var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.LeftPC));
         
         var containerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
         containerStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         leftStyle.Add(containerStyle);
         
         var itemPresenterPanelStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPresenterPart).Child().OfType<StackPanel>());
         itemPresenterPanelStyle.Add(StackPanel.OrientationProperty, Orientation.Vertical);
         itemPresenterPanelStyle.Add(StackPanel.SpacingProperty, TabControlTokenResourceKey.CardGutter);
         leftStyle.Add(itemPresenterPanelStyle);
         
         var addTabButtonStyle = new Style(selector => selector.Nesting().Template().Name(AddTabButtonPart));
         addTabButtonStyle.Add(IconButton.MarginProperty, TabControlTokenResourceKey.AddTabButtonMarginVertical);
         leftStyle.Add(addTabButtonStyle);
         
         commonStyle.Add(leftStyle);
      }

      Add(commonStyle);
   }
}