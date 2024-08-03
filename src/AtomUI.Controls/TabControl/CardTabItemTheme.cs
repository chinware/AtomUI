using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CardTabItemTheme : BaseTabItemTheme
{
   public const string ID = "CardTabItem";
   
   public CardTabItemTheme() : base(typeof(TabItem)) { }
   
   public override string ThemeResourceKey()
   {
      return ID;
   }
}