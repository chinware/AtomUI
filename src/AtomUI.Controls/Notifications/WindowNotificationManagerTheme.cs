using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class WindowNotificationManagerTheme : BaseControlTheme
{
   public const string ItemsPart = "PART_Items";
   
   public WindowNotificationManagerTheme() : base(typeof(WindowNotificationManager))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<WindowNotificationManager>((manager, scope) =>
      {
         var mainLayout = new ReversibleStackPanel()
         {
            Name = ItemsPart,
         };
         mainLayout.RegisterInNameScope(scope);
         return mainLayout;
      });
   }

   protected override void BuildStyles()
   {
      var topLeftStyle = new Style(selector => selector.Nesting().Class(WindowNotificationManager.TopLeftPC));
      {
         var itemsStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPart));
         itemsStyle.Add(ReversibleStackPanel.ReverseOrderProperty, true);
         itemsStyle.Add(ReversibleStackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         itemsStyle.Add(ReversibleStackPanel.VerticalAlignmentProperty, VerticalAlignment.Top);
         topLeftStyle.Add(itemsStyle);
      }
      
      Add(topLeftStyle);
      
      var topRightStyle = new Style(selector => selector.Nesting().Class(WindowNotificationManager.TopRightPC));
      {
         var itemsStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPart));
         itemsStyle.Add(ReversibleStackPanel.ReverseOrderProperty, true);
         itemsStyle.Add(ReversibleStackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Right);
         itemsStyle.Add(ReversibleStackPanel.VerticalAlignmentProperty, VerticalAlignment.Top);
         topRightStyle.Add(itemsStyle);
      }
      Add(topRightStyle);
      
      var topCenterStyle = new Style(selector => selector.Nesting().Class(WindowNotificationManager.TopCenterPC));
      {
         var itemsStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPart));
         itemsStyle.Add(ReversibleStackPanel.ReverseOrderProperty, true);
         itemsStyle.Add(ReversibleStackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         itemsStyle.Add(ReversibleStackPanel.VerticalAlignmentProperty, VerticalAlignment.Top);
         topCenterStyle.Add(itemsStyle);
      }
      Add(topCenterStyle);
      
      var bottomLeftStyle = new Style(selector => selector.Nesting().Class(WindowNotificationManager.BottomLeftPC));
      {
         var itemsStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPart));
         itemsStyle.Add(ReversibleStackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Left);
         itemsStyle.Add(ReversibleStackPanel.VerticalAlignmentProperty, VerticalAlignment.Bottom);
         bottomLeftStyle.Add(itemsStyle);
      }
      
      Add(bottomLeftStyle);
      
      var bottomRightStyle = new Style(selector => selector.Nesting().Class(WindowNotificationManager.BottomRightPC));
      {
         var itemsStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPart));
         itemsStyle.Add(ReversibleStackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Right);
         itemsStyle.Add(ReversibleStackPanel.VerticalAlignmentProperty, VerticalAlignment.Bottom);
         bottomRightStyle.Add(itemsStyle);
      }
      Add(bottomRightStyle);
      
      var bottomCenterStyle = new Style(selector => selector.Nesting().Class(WindowNotificationManager.BottomCenterPC));
      {
         var itemsStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPart));
         itemsStyle.Add(ReversibleStackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         itemsStyle.Add(ReversibleStackPanel.VerticalAlignmentProperty, VerticalAlignment.Bottom);
         bottomCenterStyle.Add(itemsStyle);
      }
      Add(bottomCenterStyle);
   }
}