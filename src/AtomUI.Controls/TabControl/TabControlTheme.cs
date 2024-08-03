using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TabControlTheme : BaseTabControlTheme
{
   public const string SelectedItemIndicatorPart = "PART_SelectedItemIndicator";
   
   public TabControlTheme() : base(typeof(TabControl)) { }
   
   protected override void NotifyBuildTabStripTemplate(BaseTabControl baseTabControl, INameScope scope, DockPanel container)
   {
      var tabScrollViewer = new TabControlScrollViewer();
      CreateTemplateParentBinding(tabScrollViewer, BaseTabScrollViewer.TabStripPlacementProperty, TabStrip.TabStripPlacementProperty);
      var contentPanel = CreateTabStripContentPanel(scope);
      tabScrollViewer.Content = contentPanel;
      tabScrollViewer.TabControl = baseTabControl;
      container.Children.Add(tabScrollViewer);
   }
   
   private Panel CreateTabStripContentPanel(INameScope scope)
   {
      var layout = new Panel();
      var itemsPresenter = new ItemsPresenter
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
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, TabControl.ItemsPanelProperty);
      return layout;
   }
}