using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class NumericUpDownTheme : BaseControlTheme
{
   public const string SpinnerPart = "PART_Spinner";
   public const string TextBoxPart = "PART_TextBox";
   
   public NumericUpDownTheme() : base(typeof(NumericUpDown)) {}
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<NumericUpDown>((numericUpDown, scope) =>
      {
         var buttonSpinner = BuildButtonSpinner(numericUpDown, scope);
         var textBlock = BuildTextBox(numericUpDown, scope);
         buttonSpinner.Content = textBlock;
         
         return buttonSpinner;
      });
   }

   private ButtonSpinner BuildButtonSpinner(NumericUpDown numericUpDown, INameScope scope)
   {
      var buttonSpinner = new ButtonSpinner()
      {
         Name = SpinnerPart
      };
      buttonSpinner.RegisterInNameScope(scope);
      return buttonSpinner;
   }

   private TextBox BuildTextBox(NumericUpDown numericUpDown, INameScope scope)
   {
      var textBox = new TextBox()
      {
         Name = TextBoxPart,
         Padding = new Thickness(0),
         VerticalContentAlignment = VerticalAlignment.Center,
         VerticalAlignment = VerticalAlignment.Stretch,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         BorderThickness = new Thickness(0),
         TextWrapping = TextWrapping.NoWrap,
         DisabledInnerBoxPadding = true
      };
      textBox.RegisterInNameScope(scope);
      return textBox;
   }
}