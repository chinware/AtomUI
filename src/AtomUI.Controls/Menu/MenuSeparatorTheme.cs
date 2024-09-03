using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class MenuSeparatorTheme : BaseControlTheme
{
   public MenuSeparatorTheme()
      : base(typeof(MenuSeparator))
   {
   }
   
   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(MenuSeparator.MinHeightProperty, MenuTokenResourceKey.SeparatorItemHeight);
      commonStyle.Add(MenuSeparator.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      Add(commonStyle);
   }
}