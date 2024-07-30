using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CardTabStripItemTheme : BaseTabStripItemTheme
{
   public const string ID = "CardTabStripItem";
   
   public CardTabStripItemTheme() : base(typeof(TabStripItem)) { }
   
   public override string ThemeResourceKey()
   {
      return ID;
   }
}