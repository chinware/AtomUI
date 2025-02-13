using AtomUI.Controls.Internal;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TagTheme : BaseControlTheme
{
    public const string ElementsLayoutPart = "PART_ElementsLayout";
    public const string FramePart = "PART_Frame";
    public const string IconPart = "PART_Icon";
    public const string CloseButtonPart = "PART_CloseButton";
    public const string TagTextLabelPart = "PART_TagTextLabel";

    public TagTheme()
        : base(typeof(Tag))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Tag>((tag, scope) =>
        {
            var rootLayout = new Panel();

            var frameDecorator = new Border()
            {
                Name = FramePart
            };
            CreateTemplateParentBinding(frameDecorator, Border.BackgroundProperty, Tag.BackgroundProperty);
            CreateTemplateParentBinding(frameDecorator, Border.BorderBrushProperty, Tag.BorderBrushProperty);
            CreateTemplateParentBinding(frameDecorator, Border.CornerRadiusProperty, Tag.CornerRadiusProperty);
            CreateTemplateParentBinding(frameDecorator, Border.BorderThicknessProperty, Tag.BorderThicknessProperty);
            
            rootLayout.Children.Add(frameDecorator);
            
            var elementsLayout = new Grid
            {
                Name = ElementsLayoutPart,
                ColumnDefinitions =
                {
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                }
            };
            CreateTemplateParentBinding(elementsLayout, Grid.MarginProperty, Tag.PaddingProperty);
            elementsLayout.RegisterInNameScope(scope);
            rootLayout.Children.Add(elementsLayout);
            
            var iconContentPresenter = new ContentPresenter()
            {
                Name = IconPart
            };
            Grid.SetColumn(iconContentPresenter, 0);
            iconContentPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.ContentProperty, Tag.IconProperty);
            elementsLayout.Children.Add(iconContentPresenter);

            var lineText = new SingleLineText()
            {
                Name              = TagTextLabelPart,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetColumn(lineText, 1);
            lineText.RegisterInNameScope(scope);
            CreateTemplateParentBinding(lineText, SingleLineText.TextProperty, Tag.TagTextProperty);
            CreateTemplateParentBinding(lineText, SingleLineText.PaddingProperty, Tag.TagTextPaddingInlineProperty);
            elementsLayout.Children.Add(lineText);

            var closeBtn = new IconButton
            {
                Name = CloseButtonPart
            };
            closeBtn.RegisterInNameScope(scope);
            Grid.SetColumn(closeBtn, 2);
            TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.IconWidthProperty,
                SharedTokenKey.IconSizeXS);
            TokenResourceBinder.CreateTokenBinding(closeBtn, IconButton.IconHeightProperty,
                SharedTokenKey.IconSizeXS);

            CreateTemplateParentBinding(closeBtn, Visual.IsVisibleProperty, Tag.IsClosableProperty);
            CreateTemplateParentBinding(closeBtn, IconButton.IconProperty, Tag.CloseIconProperty);

            elementsLayout.Children.Add(closeBtn);

            return rootLayout;
        });
    }
    
    protected override void BuildStyles()
    {
        this.Add(Tag.BackgroundProperty, TagTokenKey.DefaultBg);
        this.Add(Tag.ForegroundProperty, TagTokenKey.DefaultColor);
        this.Add(Tag.FontSizeProperty, TagTokenKey.TagFontSize);
        this.Add(Tag.PaddingProperty, TagTokenKey.TagPadding);
        this.Add(Tag.BorderBrushProperty, SharedTokenKey.ColorBorder);
        this.Add(Tag.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        this.Add(Tag.TagTextPaddingInlineProperty, TagTokenKey.TagTextPaddingInline);

        var lineTextStyle = new Style(selector => selector.Nesting().Template().Name(TagTextLabelPart));
        lineTextStyle.Add(SingleLineText.LineHeightProperty, TagTokenKey.TagLineHeight);
        Add(lineTextStyle);
    }
}