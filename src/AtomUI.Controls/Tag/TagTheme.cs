using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TagTheme : BaseControlTheme
{
    public const string MainContainerPart = "PART_MainContainer";
    public const string IconPart = "PART_Icon";
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
        this.Add(TemplatedControl.BorderBrushProperty, DesignTokenKey.ColorBorder);
        this.Add(TemplatedControl.CornerRadiusProperty, DesignTokenKey.BorderRadiusSM);
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
            
            var iconContentPresenter = new ContentPresenter()
            {
                Name = IconPart
            };
            iconContentPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.ContentProperty, Tag.IconProperty);
            layout.Children.Add(iconContentPresenter);

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

            TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.IconWidthProperty,
                DesignTokenKey.IconSizeXS);
            TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.IconHeightProperty,
                DesignTokenKey.IconSizeXS);
            TokenResourceBinder.CreateTokenBinding(textBlock, Layoutable.HeightProperty,
                TagTokenResourceKey.TagLineHeight);

            CreateTemplateParentBinding(closeBtn, Visual.IsVisibleProperty, Tag.IsClosableProperty);
            CreateTemplateParentBinding(closeBtn, IconButton.IconProperty, Tag.CloseIconProperty);

            layout.Children.Add(closeBtn);

            return layout;
        });
    }
}