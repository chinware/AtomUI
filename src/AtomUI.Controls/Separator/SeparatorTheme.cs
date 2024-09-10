using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SeparatorTheme : BaseControlTheme
{
    public const string TitlePart = "PART_Title";

    public SeparatorTheme()
        : base(typeof(Separator))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Separator>((separator, scope) =>
        {
            var titleLabel = new Label
            {
                Name                       = TitlePart,
                HorizontalAlignment        = HorizontalAlignment.Left,
                VerticalAlignment          = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
                Padding                    = new Thickness(0)
            };
            CreateTemplateParentBinding(titleLabel, ContentControl.ContentProperty, Separator.TitleProperty);
            CreateTemplateParentBinding(titleLabel, TemplatedControl.FontSizeProperty,
                TemplatedControl.FontSizeProperty);
            CreateTemplateParentBinding(titleLabel, TemplatedControl.ForegroundProperty, Separator.TitleColorProperty);
            CreateTemplateParentBinding(titleLabel, TemplatedControl.FontStyleProperty,
                TemplatedControl.FontStyleProperty);
            CreateTemplateParentBinding(titleLabel, TemplatedControl.FontWeightProperty,
                TemplatedControl.FontWeightProperty);
            titleLabel.RegisterInNameScope(scope);
            return titleLabel;
        });
    }

    protected override void BuildStyles()
    {
        // 默认的一些样式
        this.Add(Separator.TitleColorProperty, GlobalTokenResourceKey.ColorText);
        this.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSize);
        this.Add(Separator.LineColorProperty, GlobalTokenResourceKey.ColorSplit);
        this.Add(Separator.TextPaddingInlineProperty, SeparatorTokenResourceKey.TextPaddingInline);
        this.Add(Separator.OrientationMarginPercentProperty, SeparatorTokenResourceKey.OrientationMarginPercent);
        this.Add(Separator.VerticalMarginInlineProperty, SeparatorTokenResourceKey.VerticalMarginInline);

        var titleSelector = default(Selector).Nesting().Template().OfType<Label>().Name(TitlePart);
        var horizontalStyle =
            new Style(selector => selector.Nesting()
                                          .PropertyEquals(Separator.OrientationProperty, Orientation.Horizontal));
        horizontalStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        horizontalStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        {
            var titleStyle = new Style(selector => titleSelector);
            titleStyle.Add(Visual.IsVisibleProperty, true);
            horizontalStyle.Add(titleStyle);
        }
        Add(horizontalStyle);

        var verticalStyle =
            new Style(
                selector => selector.Nesting().PropertyEquals(Separator.OrientationProperty, Orientation.Vertical));
        verticalStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        verticalStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        {
            var titleStyle = new Style(selector => titleSelector);
            titleStyle.Add(Visual.IsVisibleProperty, false);
            verticalStyle.Add(titleStyle);
        }
        Add(verticalStyle);
    }
}