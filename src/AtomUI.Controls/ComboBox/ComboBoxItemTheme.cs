using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ComboBoxItemTheme : BaseControlTheme
{
    public const string ContentPresenterPart = "PART_ContentPresenter";

    public ComboBoxItemTheme() : base(typeof(ComboBoxItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ComboBoxItem>((listBoxItem, scope) =>
        {
            var contentPresenter = new ContentPresenter
            {
                Name = ContentPresenterPart
            };

            contentPresenter.Transitions = new Transitions
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.BackgroundProperty)
            };

            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
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
        BuildDisabledStyle();
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        commonStyle.Add(Layoutable.MarginProperty, ComboBoxTokenKey.ItemMargin);

        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ComboBoxTokenKey.ItemColor);
            contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, ComboBoxTokenKey.ItemBgColor);
            contentPresenterStyle.Add(Layoutable.MinHeightProperty, SharedTokenKey.ControlHeight);
            contentPresenterStyle.Add(ContentPresenter.PaddingProperty, ComboBoxTokenKey.ItemPadding);
            contentPresenterStyle.Add(ContentPresenter.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
            commonStyle.Add(contentPresenterStyle);
        }

        var hoveredStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ComboBoxTokenKey.ItemHoverColor);
            contentPresenterStyle.Add(ContentPresenter.BackgroundProperty, ComboBoxTokenKey.ItemHoverBgColor);
            hoveredStyle.Add(contentPresenterStyle);
        }
        commonStyle.Add(hoveredStyle);

        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        {
            var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
            contentPresenterStyle.Add(ContentPresenter.ForegroundProperty, ComboBoxTokenKey.ItemSelectedColor);
            contentPresenterStyle.Add(ContentPresenter.BackgroundProperty,
                ComboBoxTokenKey.ItemSelectedBgColor);
            selectedStyle.Add(contentPresenterStyle);
        }
        commonStyle.Add(selectedStyle);
        Add(commonStyle);
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