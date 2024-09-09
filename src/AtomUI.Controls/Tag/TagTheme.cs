using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TagTheme : BaseControlTheme
{
    public const string MainContainerPart = "PART_MainContainer";
    public const string CloseButtonPart = "PART_CloseButton";
    public const string TagTextLabelPart = "PART_TagTextLabel";

    public TagTheme()
        : base(typeof(Tag))
    {
    }

    protected override void BuildStyles()
    {
        this.Add(TemplatedControl.BackgroundProperty, TagTokenResourceKey.DefaultBg);
        this.Add(TemplatedControl.ForegroundProperty, TagTokenResourceKey.DefaultColor);
        this.Add(TemplatedControl.FontSizeProperty, TagTokenResourceKey.TagFontSize);
        this.Add(TemplatedControl.PaddingProperty, TagTokenResourceKey.TagPadding);
        this.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
        this.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        this.Add(Tag.TagTextPaddingInlineProperty, TagTokenResourceKey.TagTextPaddingInline);
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Tag>((tag, scope) =>
        {
            var layout = new Canvas
            {
                Name = MainContainerPart
            };
            layout.RegisterInNameScope(scope);

            var textBlock = new TextBlock
            {
                Name              = TagTextLabelPart,
                VerticalAlignment = VerticalAlignment.Center
            };
            textBlock.RegisterInNameScope(scope);
            CreateTemplateParentBinding(textBlock, TextBlock.TextProperty, Tag.TagTextProperty);
            layout.Children.Add(textBlock);

            var closeBtn = new IconButton
            {
                Name = CloseButtonPart
            };
            closeBtn.RegisterInNameScope(scope);

            TokenResourceBinder.CreateTokenBinding(closeBtn, Layoutable.WidthProperty,
                GlobalTokenResourceKey.IconSizeSM);
            TokenResourceBinder.CreateTokenBinding(closeBtn, Layoutable.HeightProperty,
                GlobalTokenResourceKey.IconSizeSM);
            TokenResourceBinder.CreateTokenBinding(textBlock, Layoutable.HeightProperty,
                TagTokenResourceKey.TagLineHeight);
            TokenResourceBinder.CreateTokenBinding(textBlock, TextBlock.LineHeightProperty,
                TagTokenResourceKey.TagLineHeight);

            CreateTemplateParentBinding(closeBtn, Visual.IsVisibleProperty, Tag.IsClosableProperty);
            CreateTemplateParentBinding(closeBtn, IconButton.IconProperty, Tag.CloseIconProperty);

            layout.Children.Add(closeBtn);

            return layout;
        });
    }
}