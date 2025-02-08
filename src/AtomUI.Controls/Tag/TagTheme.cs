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
        this.Add(TemplatedControl.BackgroundProperty, TagTokenKey.DefaultBg);
        this.Add(TemplatedControl.ForegroundProperty, TagTokenKey.DefaultColor);
        this.Add(TemplatedControl.FontSizeProperty, TagTokenKey.TagFontSize);
        this.Add(TemplatedControl.PaddingProperty, TagTokenKey.TagPadding);
        this.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorder);
        this.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        this.Add(Tag.TagTextPaddingInlineProperty, TagTokenKey.TagTextPaddingInline);
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
                SharedTokenKey.IconSizeXS);
            TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.IconHeightProperty,
                SharedTokenKey.IconSizeXS);
            TokenResourceBinder.CreateTokenBinding(textBlock, Layoutable.HeightProperty,
                TagTokenKey.TagLineHeight);

            CreateTemplateParentBinding(closeBtn, Visual.IsVisibleProperty, Tag.IsClosableProperty);
            CreateTemplateParentBinding(closeBtn, IconButton.IconProperty, Tag.CloseIconProperty);

            layout.Children.Add(closeBtn);

            return layout;
        });
    }
}