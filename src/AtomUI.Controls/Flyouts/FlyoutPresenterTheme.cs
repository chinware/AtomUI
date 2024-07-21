using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class FlyoutPresenterTheme : ControlTheme
{
   public FlyoutPresenterTheme() : base(typeof(FlyoutPresenter)) {}

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<FlyoutPresenter>(((presenter, scope) =>
      {
         Console.WriteLine("xxxxxxxxxxxxxx");
         return new Panel();
      }));
   }
}