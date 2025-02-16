using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class EmptyIndicatorTheme : BaseControlTheme
{
    public const string SvgImagePart = "PART_SvgImage";

    public EmptyIndicatorTheme()
        : base(typeof(EmptyIndicator))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<EmptyIndicator>((indicator, scope) =>
        {
            var layout = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            var svg = new Avalonia.Svg.Svg(new Uri("https://github.com/avaloniaui"))
            {
                Name = SvgImagePart
            };
            layout.Children.Add(svg);
            svg.RegisterInNameScope(scope);

            var description = new TextBlock
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping        = TextWrapping.Wrap,
                Text                = indicator.Description ?? "No data"
            };

            TokenResourceBinder.CreateTokenBinding(description, TextBlock.ForegroundProperty,
                SharedTokenKey.ColorTextDescription);
            BindUtils.RelayBind(indicator, EmptyIndicator.DescriptionMarginProperty, description,
                Layoutable.MarginProperty,
                d => new Thickness(0, d, 0, 0));
            layout.Children.Add(description);

            return layout;
        });
    }

    protected override void BuildStyles()
    {
        // 设置本身样式
        var sizeSmallAndMiddleStyle = new Style(selector => Selectors.Or(
            selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Middle),
            selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Small)));
        sizeSmallAndMiddleStyle.Add(EmptyIndicator.DescriptionMarginProperty, SharedTokenKey.MarginXS);
        Add(sizeSmallAndMiddleStyle);

        var sizeLargeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Large));
        sizeLargeStyle.Add(EmptyIndicator.DescriptionMarginProperty, SharedTokenKey.MarginSM);
        Add(sizeLargeStyle);

        BuildSvgStyle();
    }

    private void BuildSvgStyle()
    {
        var svgSelector = default(Selector).Nesting().Template().OfType<Avalonia.Svg.Svg>();
        {
            var largeSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Large));
            var svgStyle = new Style(selector => svgSelector);
            svgStyle.Add(Layoutable.HeightProperty, EmptyIndicatorTokenKey.EmptyImgHeight);
            largeSizeStyle.Add(svgStyle);
            Add(largeSizeStyle);
        }

        {
            var middleSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Middle));
            var svgStyle = new Style(selector => svgSelector);
            svgStyle.Add(Layoutable.HeightProperty, EmptyIndicatorTokenKey.EmptyImgHeightMD);
            middleSizeStyle.Add(svgStyle);
            Add(middleSizeStyle);
        }

        {
            var smallSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Small));
            var svgStyle = new Style(selector => svgSelector);
            svgStyle.Add(Layoutable.HeightProperty, EmptyIndicatorTokenKey.EmptyImgHeightSM);
            smallSizeStyle.Add(svgStyle);
            Add(smallSizeStyle);
        }
    }
}