using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SearchEditTheme : LineEditTheme
{
    public SearchEditTheme() : base(typeof(SearchEdit))
    {
    }

    protected override AddOnDecoratedBox BuildAddOnDecoratedBox(TextBox textBox, INameScope scope)
    {
        var decoratedBox = new SearchEditDecoratedBox
        {
            Name      = DecoratedBoxPart,
            Focusable = true
        };
        decoratedBox.RegisterInNameScope(scope);

        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StyleVariantProperty, TextBox.StyleVariantProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.SizeTypeProperty, TextBox.SizeTypeProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.StatusProperty, TextBox.StatusProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.LeftAddOnProperty, LineEdit.LeftAddOnProperty);
        CreateTemplateParentBinding(decoratedBox, AddOnDecoratedBox.RightAddOnBorderThicknessProperty,
            TemplatedControl.BorderThicknessProperty);
        CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.SearchButtonStyleProperty,
            SearchEdit.SearchButtonStyleProperty);
        CreateTemplateParentBinding(decoratedBox, SearchEditDecoratedBox.SearchButtonTextProperty,
            SearchEdit.SearchButtonTextProperty);
        return decoratedBox;
    }
}