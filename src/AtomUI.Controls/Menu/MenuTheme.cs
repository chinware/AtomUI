using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class MenuTheme : ControlTheme
{
   public const string ItemsPresenterPart = "PART_ItemsPresenter";
   public MenuTheme()
      : base(typeof(Menu))
   {
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<Menu>((menu, scope) =>
      {
         var itemPresenter = new ItemsPresenter()
         {
            Name = ItemsPresenterPart,
            VerticalAlignment = VerticalAlignment.Stretch,
         };

         KeyboardNavigation.SetTabNavigation(itemPresenter, KeyboardNavigationMode.Continue);
         
         var border = new Border()
         {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Child = itemPresenter,
         };
         return border;
      });
   }
}