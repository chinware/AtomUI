using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SearchEditTheme : LineEditTheme
{
   public SearchEditTheme() : base(typeof(SearchEdit)) { }
   
   protected override AddOnDecoratedBox BuildAddOnDecoratedBox(TextBox textBox, INameScope scope)
   {
      var decoratedBox = new SearchEditDecoratedBox()
      {
         Name = DecoratedBoxPart,
         Focusable = true
      };
      decoratedBox.RegisterInNameScope(scope);
      
      CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.StyleVariantProperty, SearchEdit.StyleVariantProperty);
      CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.SizeTypeProperty, SearchEdit.SizeTypeProperty);
      CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.StatusProperty, SearchEdit.StatusProperty);
      CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.LeftAddOnProperty, SearchEdit.LeftAddOnProperty);
      CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.RightAddOnBorderThicknessProperty, SearchEdit.BorderThicknessProperty);
      CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.SearchButtonStyleProperty, SearchEdit.SearchButtonStyleProperty);
      CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.SearchButtonTextProperty, SearchEdit.SearchButtonTextProperty);
      return decoratedBox;
   }
}