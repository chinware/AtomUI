using AtomUI.Theme.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DropdownButtonTheme : BaseButtonTheme
{
   public const string ID = "DropdownButton";
   
   public DropdownButtonTheme()
      : base(typeof(DropdownButton))
   {
   }
   
}