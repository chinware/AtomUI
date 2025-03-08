using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class EmptyIndicatorTheme : BaseControlTheme
{
    public const string SvgImagePart = "PART_SvgImage";
    public const string DescriptionPart = "PART_Description";

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
                Name = DescriptionPart,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping        = TextWrapping.Wrap,
            };
            CreateTemplateParentBinding(description, TextBlock.TextProperty, EmptyIndicator.DescriptionProperty);
            layout.Children.Add(description);

            return layout;
        });
    }

    protected override void BuildStyles()
    {
        // 设置本身样式
        BuildSvgStyle();

        this.Add(EmptyIndicator.ColorFillProperty, SharedTokenKey.ColorFill);
        this.Add(EmptyIndicator.ColorFillTertiaryProperty, SharedTokenKey.ColorFillTertiary);
        this.Add(EmptyIndicator.ColorFillQuaternaryProperty, SharedTokenKey.ColorFillQuaternary);
        this.Add(EmptyIndicator.ColorBgContainerProperty, SharedTokenKey.ColorBgContainer);
        
        var sizeSmallAndMiddleStyle = new Style(selector => Selectors.Or(
            selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Middle),
            selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Small)));
        {
            var descriptionStyle = new Style(selector => selector.Nesting().Template().Name(DescriptionPart));
            descriptionStyle.Add(TextBlock.MarginProperty, EmptyIndicatorTokenKey.DescriptionMarginSM);
            Add(descriptionStyle);
        }
        Add(sizeSmallAndMiddleStyle);

        var sizeLargeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(EmptyIndicator.SizeTypeProperty, SizeType.Large));
        {
            var descriptionStyle = new Style(selector => selector.Nesting().Template().Name(DescriptionPart));
            descriptionStyle.Add(TextBlock.MarginProperty, EmptyIndicatorTokenKey.DescriptionMargin);
            Add(descriptionStyle);
        }
        Add(sizeLargeStyle);

        {
            var descriptionStyle = new Style(selector => selector.Nesting().Template().Name(DescriptionPart));
            descriptionStyle.Add(TextBlock.ForegroundProperty, SharedTokenKey.ColorTextDescription);
            Add(descriptionStyle);
        }
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