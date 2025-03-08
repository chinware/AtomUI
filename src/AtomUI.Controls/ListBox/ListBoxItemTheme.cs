using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ListBoxItemTheme : BaseControlTheme
{
    public const string ContentPresenterPart = "PART_ContentPresenter";

    public ListBoxItemTheme() : this(typeof(ListBoxItem))
    {
    }

    protected ListBoxItemTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ListBoxItem>((listBoxItem, scope) =>
        {
            var contentPresenter = new ContentPresenter
            {
                Name = ContentPresenterPart
            };

            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty,
                TemplatedControl.ForegroundProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.BackgroundProperty,
                TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty, 
                BindingMode.Default,
                new FuncValueConverter<object?, object?>(
                    o =>
                    {
                        if (o is string str)
                        {
                            return new TextBlock()
                            {
                                Text              = str,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }

                        return o;
                    }));
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, Layoutable.MinHeightProperty, Layoutable.MinHeightProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.PaddingProperty,
                TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty,
                ContentControl.HorizontalContentAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty,
                ContentControl.VerticalContentAlignmentProperty);
            return contentPresenter;
        });
    }

    protected override void BuildStyles()
    {
        BuildCommonStyle();
        BuildSizeTypeStyle();
        BuildDisabledStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(ListBoxItem.MarginProperty, ListBoxTokenKey.ItemMargin);
        commonStyle.Add(ListBoxItem.ForegroundProperty, ListBoxTokenKey.ItemColor);
        commonStyle.Add(ListBoxItem.BackgroundProperty, ListBoxTokenKey.ItemBgColor);

        var disabledItemHoverStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ListBoxItem.DisabledItemHoverEffectProperty, false).Class(StdPseudoClass.PointerOver));
        disabledItemHoverStyle.Add(ListBoxItem.ForegroundProperty, ListBoxTokenKey.ItemHoverColor);
        disabledItemHoverStyle.Add(ListBoxItem.BackgroundProperty, ListBoxTokenKey.ItemHoverBgColor);
        
        commonStyle.Add(disabledItemHoverStyle);

        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(ListBoxItem.ForegroundProperty, ListBoxTokenKey.ItemSelectedColor);
        selectedStyle.Add(ListBoxItem.BackgroundProperty, ListBoxTokenKey.ItemSelectedBgColor);
        commonStyle.Add(selectedStyle);
        Add(commonStyle);
    }

    private void BuildSizeTypeStyle()
    {
        var largeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        largeStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeightLG);
        largeStyle.Add(TemplatedControl.PaddingProperty, ListBoxTokenKey.ItemPaddingLG);

        Add(largeStyle);

        var middleStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        middleStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeight);
        middleStyle.Add(TemplatedControl.PaddingProperty, ListBoxTokenKey.ItemPadding);

        Add(middleStyle);

        var smallStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusXS);
        smallStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeightSM);
        smallStyle.Add(TemplatedControl.PaddingProperty, ListBoxTokenKey.ItemPaddingSM);
        Add(smallStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle         = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        disabledStyle.Add(contentPresenterStyle);
        Add(disabledStyle);
    }
}