using AtomUI.Theme.Styling;
using Avalonia.Controls;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CardTabStripTheme : BaseTabStripTheme
{
   public CardTabStripTheme() : base(typeof(CardTabStrip)) { }

   protected override void NotifyBuildControlTemplate(BaseTabStrip baseTabStrip, INameScope scope, Border container)
   {
      
   }
}