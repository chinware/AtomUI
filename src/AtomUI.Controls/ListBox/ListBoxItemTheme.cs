using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
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
                ContentControl.ContentProperty);
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
        commonStyle.Add(ListBoxItem.MarginProperty, ListBoxTokenResourceKey.ItemMargin);
        commonStyle.Add(ListBoxItem.ForegroundProperty, ListBoxTokenResourceKey.ItemColor);
        commonStyle.Add(ListBoxItem.BackgroundProperty, ListBoxTokenResourceKey.ItemBgColor);

        var disabledItemHoverStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ListBoxItem.DisabledItemHoverEffectProperty, false).Class(StdPseudoClass.PointerOver));
        disabledItemHoverStyle.Add(ListBoxItem.ForegroundProperty, ListBoxTokenResourceKey.ItemHoverColor);
        disabledItemHoverStyle.Add(ListBoxItem.BackgroundProperty, ListBoxTokenResourceKey.ItemHoverBgColor);
        
        commonStyle.Add(disabledItemHoverStyle);

        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(ListBoxItem.ForegroundProperty, ListBoxTokenResourceKey.ItemSelectedColor);
        selectedStyle.Add(ListBoxItem.BackgroundProperty, ListBoxTokenResourceKey.ItemSelectedBgColor);
        commonStyle.Add(selectedStyle);
        Add(commonStyle);
    }

    private void BuildSizeTypeStyle()
    {
        var largeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        largeStyle.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.ControlHeightLG);
        largeStyle.Add(TemplatedControl.PaddingProperty, ListBoxTokenResourceKey.ItemPaddingLG);

        Add(largeStyle);

        var middleStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        middleStyle.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.ControlHeight);
        middleStyle.Add(TemplatedControl.PaddingProperty, ListBoxTokenResourceKey.ItemPadding);

        Add(middleStyle);

        var smallStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(ListBoxItem.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusXS);
        smallStyle.Add(Layoutable.MinHeightProperty, GlobalTokenResourceKey.ControlHeightSM);
        smallStyle.Add(TemplatedControl.PaddingProperty, ListBoxTokenResourceKey.ItemPaddingSM);
        Add(smallStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle         = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
        disabledStyle.Add(contentPresenterStyle);
        Add(disabledStyle);
    }
}