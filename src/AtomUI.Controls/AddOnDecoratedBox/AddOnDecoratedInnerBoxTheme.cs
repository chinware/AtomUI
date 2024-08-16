using AtomUI.Theme;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls.AddOnDecoratedBox;

internal class AddOnDecoratedInnerBoxTheme : BaseControlTheme
{
   protected override IControlTemplate? BuildControlTemplate()
   {
      return new FuncControlTemplate<AddOnDecoratedInnerBox>((innerBox, scope) =>
      {
         return default!;
      });
   }

   protected override void BuildStyles()
   {
      
   }
}