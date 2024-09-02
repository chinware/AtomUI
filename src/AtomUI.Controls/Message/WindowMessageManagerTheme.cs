using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.Message;

[ControlThemeProvider]
internal class WindowMessageManagerTheme : BaseControlTheme
{
    public const string ItemsPart = "PART_Items";
   
   public WindowMessageManagerTheme() : base(typeof(WindowMessageManager))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<WindowMessageManager>((manager, scope) =>
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
      var topCenterStyle = new Style(selector => selector.Nesting().Class(WindowNotificationManager.TopCenterPC));
      {
         var itemsStyle = new Style(selector => selector.Nesting().Template().Name(ItemsPart));
         itemsStyle.Add(ReversibleStackPanel.ReverseOrderProperty, true);
         itemsStyle.Add(ReversibleStackPanel.HorizontalAlignmentProperty, HorizontalAlignment.Center);
         itemsStyle.Add(ReversibleStackPanel.VerticalAlignmentProperty, VerticalAlignment.Top);
         topCenterStyle.Add(itemsStyle);
      }
      Add(topCenterStyle);
   }
}