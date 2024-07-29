using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

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
         VerticalAlignment = VerticalAlignment.Bottom
      };
      border.RegisterInNameScope(scope);
      layout.Children.Add(itemsPresenter);
      layout.Children.Add(border);
      CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, TabStrip.ItemsPanelProperty);
      container.Child = layout;
   }
}