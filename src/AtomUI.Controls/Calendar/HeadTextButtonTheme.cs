using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class HeadTextButtonTheme : BaseControlTheme
{
    private const string ContentPart = "PART_Content";

    public HeadTextButtonTheme() : base(typeof(HeadTextButton))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<HeadTextButton>((headTextButton, scope) =>
        {
            var content = new ContentPresenter
            {
                Name                       = ContentPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center
            };
            CreateTemplateParentBinding(content, ContentPresenter.ContentProperty, ContentControl.ContentProperty);
            CreateTemplateParentBinding(content, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            CreateTemplateParentBinding(content, ContentPresenter.BackgroundProperty,
                TemplatedControl.BackgroundProperty);
            return content;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        commonStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
        commonStyle.Add(TemplatedControl.FontWeightProperty, FontWeight.SemiBold);
        commonStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        commonStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));

        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimary);
        commonStyle.Add(hoverStyle);

        Add(commonStyle);
    }
}