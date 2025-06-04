using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TagTheme : BaseControlTheme
{
    public const string ElementsLayoutPart = "PART_ElementsLayout";
    public const string FramePart = "PART_Frame";
    public const string IconPresenterPart = "PART_IconPresenter";
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

            var frame = new Border()
            {
                Name = FramePart
            };
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, Tag.BackgroundProperty);
            CreateTemplateParentBinding(frame, Border.BorderBrushProperty, Tag.BorderBrushProperty);
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, Tag.CornerRadiusProperty);
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty, Tag.BorderThicknessProperty);

            rootLayout.Children.Add(frame);

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
                Name = IconPresenterPart
            };
            Grid.SetColumn(iconContentPresenter, 0);
            iconContentPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(iconContentPresenter, ContentPresenter.ContentProperty, Tag.IconProperty);
            elementsLayout.Children.Add(iconContentPresenter);

            var lineText = new TextBlock()
            {
                Name              = TagTextLabelPart,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetColumn(lineText, 1);
            lineText.RegisterInNameScope(scope);
            CreateTemplateParentBinding(lineText, TextBlock.TextProperty, Tag.TagTextProperty);
            CreateTemplateParentBinding(lineText, TextBlock.PaddingProperty, Tag.TagTextPaddingInlineProperty);
            elementsLayout.Children.Add(lineText);

            var closeBtn = new IconButton
            {
                Name = CloseButtonPart
            };
            closeBtn.RegisterInNameScope(scope);
            Grid.SetColumn(closeBtn, 2);

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
        lineTextStyle.Add(TextBlock.LineHeightProperty, TagTokenKey.TagLineHeight);
        Add(lineTextStyle);

        BuildTagIconStyle();
        BuildCloseButtonStyle();
    }

    private void BuildTagIconStyle()
    {
        {
            var tagIconStyle = new Style(selector =>
                selector.Nesting().Template().Name(IconPresenterPart).Descendant().OfType<Icon>());
            tagIconStyle.Add(Icon.WidthProperty, TagTokenKey.TagIconSize);
            tagIconStyle.Add(Icon.HeightProperty, TagTokenKey.TagIconSize);

            tagIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorIcon);
            tagIconStyle.Add(Icon.ActiveFilledBrushProperty, SharedTokenKey.ColorIconHover);
            Add(tagIconStyle);
        }

        var isColorSetStyle = new Style(selector => selector.Nesting().PropertyEquals(Tag.IsColorSetProperty, true));

        {
            var tagIconStyle = new Style(selector =>
                selector.Nesting().Template().Name(IconPresenterPart).Descendant().OfType<Icon>());
            tagIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorTextLightSolid);
            isColorSetStyle.Add(tagIconStyle);
        }
        Add(isColorSetStyle);
        var isPresetColorTagStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Tag.IsPresetColorTagProperty, true));
        {
            var tagIconStyle = new Style(selector =>
                selector.Nesting().Template().Name(IconPresenterPart).Descendant().OfType<Icon>());
            // TODO 需要评估是否会造成内存泄漏
            tagIconStyle.Add(Icon.NormalFilledBrushProperty, new Binding(Tag.ForegroundProperty.Name)
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent)
            });
            isPresetColorTagStyle.Add(tagIconStyle);
        }
        Add(isPresetColorTagStyle);
    }

    private void BuildCloseButtonStyle()
    {
        {
            var closeButtonStyle = new Style(selector => selector.Nesting().Template().Name(CloseButtonPart));
            closeButtonStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeXS);
            closeButtonStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeXS);
            closeButtonStyle.Add(IconButton.NormalIconBrushProperty, SharedTokenKey.ColorIcon);
            closeButtonStyle.Add(IconButton.ActiveIconBrushProperty, SharedTokenKey.ColorIconHover);
            Add(closeButtonStyle);
        }

        var isColorSetAndNotPresetTagStyle = new Style(selector => selector
                                                                   .Nesting().PropertyEquals(Tag.IsColorSetProperty,
                                                                       true)
                                                                   .PropertyEquals(Tag.IsPresetColorTagProperty,
                                                                       false));
        {
            var closeButtonStyle = new Style(selector => selector.Nesting().Template().Name(CloseButtonPart));
            closeButtonStyle.Add(IconButton.NormalIconBrushProperty, SharedTokenKey.ColorTextLightSolid);
            isColorSetAndNotPresetTagStyle.Add(closeButtonStyle);
        }
        Add(isColorSetAndNotPresetTagStyle);
    }
}