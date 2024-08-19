using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TextBoxInnerBoxTheme : AddOnDecoratedInnerBoxTheme
{
   public TextBoxInnerBoxTheme() : base(typeof(TextBoxInnerBox)) {}
   
   protected override void BuildDisabledStyle()
   {
      var embedModeStyle = new Style(selector => selector.Nesting().PropertyEquals(TextBoxInnerBox.EmbedModeProperty, false));
      var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
      var decoratorStyle = new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
      decoratorStyle.Add(Border.BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      disabledStyle.Add(decoratorStyle);
      embedModeStyle.Add(disabledStyle);
      Add(embedModeStyle);
   }
}