using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TabItemTheme : BaseTabItemTheme
{
   public const string ID = "TabItem";
   
   public TabItemTheme() : base(typeof(TabItem)) { }
   
   public override string ThemeResourceKey()
   {
      return ID;
   }
}