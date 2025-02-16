using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ComboBoxSpinnerInnerBoxTheme : AddOnDecoratedInnerBoxTheme
{
    public const string SpinnerHandlePart = "PART_SpinnerHandle";

    public ComboBoxSpinnerInnerBoxTheme() : base(typeof(ComboBoxSpinnerInnerBox))
    {
    }

    public ComboBoxSpinnerInnerBoxTheme(Type targetType) : base(targetType)
    {
    }

    protected override void NotifyBuildExtraChild(Panel layout, AddOnDecoratedInnerBox decoratedBox, INameScope scope)
    {
        var contentPresenter = new ContentPresenter
        {
            Name                = SpinnerHandlePart,
            ZIndex              = AddOnDecoratedBoxTheme.ActivatedZIndex,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        contentPresenter.RegisterInNameScope(scope);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
            ComboBoxSpinnerInnerBox.SpinnerContentProperty);
        layout.Children.Add(contentPresenter);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var notFilledStyle =
            new Style(selector => selector.Nesting().Not(innerSelector =>
                innerSelector.Nesting()
                             .PropertyEquals(AddOnDecoratedBox.StyleVariantProperty, AddOnDecoratedVariant.Filled)));

        {
            var innerBoxDecoratorStyle =
                new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, Brushes.Transparent);
            notFilledStyle.Add(innerBoxDecoratorStyle);
        }
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var innerBoxDecoratorStyle =
                new Style(selector => selector.Nesting().Template().Name(InnerBoxDecoratorPart));
            innerBoxDecoratorStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorFillTertiary);
            hoverStyle.Add(innerBoxDecoratorStyle);
            notFilledStyle.Add(hoverStyle);
        }
        Add(notFilledStyle);
    }
}