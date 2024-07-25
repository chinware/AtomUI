using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuItemTheme : ControlTheme
{
   public MenuItemTheme()
      : base(typeof(MenuItem))
   {
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<MenuItem>((item, scope) =>
      {
         return new Border()
         {
            Height = 30,
            Background = new SolidColorBrush(Colors.Chocolate),
         };
      });
   }
}