using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class TopLevelMenuItemTheme : ControlTheme
{
   public const string ID = "TopLevelMenuItem";
   public const string PopupPart = "PART_Popup";
   public const string HeaderPresenterPart = "PART_HeaderPresenter";
   
   public TopLevelMenuItemTheme() : base(typeof(MenuItem)) {}
   
   public override string? ThemeResourceKey()
   {
      return ID;
   }

   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<MenuItem>((item, scope) =>
      {
         var panel = new Panel();
         var contentPresenter = new ContentPresenter()
         {
            Name = HeaderPresenterPart
         };
         contentPresenter.RegisterInNameScope(scope);
         panel.Children.Add(contentPresenter);
         return panel;
      });
   }

   protected override void BuildStyles()
   {
      
   }
}