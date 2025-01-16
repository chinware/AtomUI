using AtomUI.Theme.Styling;
using AtomUI.Utils;
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

    protected override void NotifyBuildExtraChild(AddOnDecoratedInnerBox decoratedBox, Panel layout, INameScope scope)
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
        
        this.Add(ComboBoxSpinnerInnerBox.SpinnerBorderBrushProperty, SharedTokenKey.ColorBorder);
        this.Add(ComboBoxSpinnerInnerBox.SpinnerHandleWidthTokenProperty, ButtonSpinnerTokenKey.HandleWidth);
        
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