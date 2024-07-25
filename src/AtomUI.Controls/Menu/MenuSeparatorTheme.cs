using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
public class MenuSeparatorTheme : ControlTheme
{
   public MenuSeparatorTheme()
      : base(typeof(MenuSeparator))
   {
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(MenuSeparator.MinHeightProperty, MenuResourceKey.SeparatorItemHeight);
      commonStyle.Add(MenuSeparator.BorderBrushProperty, GlobalResourceKey.ColorBorder);
      Add(commonStyle);
   }
}