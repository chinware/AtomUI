using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class LineEditTheme : TextBoxTheme
{
   public LineEditTheme(Type targetType) : base(targetType) { }
   public LineEditTheme() : base(typeof(LineEdit)) { }

   protected override Control BuildTextBoxStructure(TextBox textBox, INameScope scope)
   {
      var decoratedBox = BuildAddOnDecoratedBox(textBox, scope);
      var lineEditKernel = BuildTextBoxKernel(textBox, scope);
      decoratedBox.Content = lineEditKernel;
      lineEditKernel.RegisterInNameScope(scope);
      return decoratedBox;
   }
   
   protected virtual AddOnDecoratedBox BuildAddOnDecoratedBox(TextBox textBox, INameScope scope)
   {
      var decoratedBox = new AddOnDecoratedBox()
      {
         Name = DecoratedBoxPart,
         Focusable = true
      };
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty, LineEdit.StyleVariantProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, LineEdit.SizeTypeProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, LineEdit.StatusProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty, LineEdit.LeftAddOnProperty);
      CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnProperty, LineEdit.RightAddOnProperty);
      decoratedBox.RegisterInNameScope(scope);
      return decoratedBox;
   }
}