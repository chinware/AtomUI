using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SegmentedItemTheme : BaseControlTheme
{
    public const string MainFramePart = "PART_MainFrame";
    public const string IconContentPart = "PART_IconContent";
    public const string ContentPart = "PART_Content";

    public SegmentedItemTheme() : base(typeof(SegmentedItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<SegmentedItem>((segmentedItem, scope) =>
        {
            var mainFrame = new Border
            {
                Name = MainFramePart
            };
            CreateTemplateParentBinding(mainFrame, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(mainFrame, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(mainFrame, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);

            var contentLayout = new DockPanel
            {
                LastChildFill = true
            };

            var iconContent = new ContentPresenter
            {
                Name = IconContentPart
            };
            iconContent.RegisterInNameScope(scope);
            CreateTemplateParentBinding(iconContent, ContentPresenter.ContentProperty, SegmentedItem.IconProperty);
            CreateTemplateParentBinding(iconContent, Visual.IsVisibleProperty, SegmentedItem.IconProperty,
                BindingMode.Default,
                ObjectConverters.IsNotNull);

            contentLayout.Children.Add(iconContent);

            var contentPresenter = new ContentPresenter
            {
                Name                       = ContentPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center
            };
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, Visual.IsVisibleProperty, ContentControl.ContentProperty,
                BindingMode.Default,
                ObjectConverters.IsNotNull);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);
            contentLayout.Children.Add(contentPresenter);

            mainFrame.Child = contentLayout;

            return mainFrame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        // 没有被选择的正常状态
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        enabledStyle.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));

        // 选中状态
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(TemplatedControl.ForegroundProperty, SegmentedTokenResourceKey.ItemSelectedColor);
        selectedStyle.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
        enabledStyle.Add(selectedStyle);

        // 没有被选中的状态
        var notSelectedStyle =
            new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(StdPseudoClass.Selected)));
        notSelectedStyle.Add(TemplatedControl.BackgroundProperty, GlobalTokenResourceKey.ColorTransparent);
        notSelectedStyle.Add(TemplatedControl.ForegroundProperty, SegmentedTokenResourceKey.ItemColor);

        // Hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.BackgroundProperty, SegmentedTokenResourceKey.ItemHoverBg);
        hoverStyle.Add(TemplatedControl.ForegroundProperty, SegmentedTokenResourceKey.ItemHoverColor);
        notSelectedStyle.Add(hoverStyle);

        // Pressed 状态
        var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
        pressedStyle.Add(TemplatedControl.BackgroundProperty, SegmentedTokenResourceKey.ItemActiveBg);
        notSelectedStyle.Add(pressedStyle);

        enabledStyle.Add(notSelectedStyle);
        commonStyle.Add(enabledStyle);
        Add(commonStyle);

        BuildSizeTypeStyle();
        BuildIconStyle();
        BuildDisabledStyle();
    }

    private void BuildSizeTypeStyle()
    {
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSizeLG);
        largeSizeStyle.Add(Layoutable.MinHeightProperty, SegmentedTokenResourceKey.ItemMinHeightLG);
        largeSizeStyle.Add(TemplatedControl.PaddingProperty, SegmentedTokenResourceKey.SegmentedItemPadding);
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSize);
        middleSizeStyle.Add(Layoutable.MinHeightProperty, SegmentedTokenResourceKey.ItemMinHeight);
        middleSizeStyle.Add(TemplatedControl.PaddingProperty, SegmentedTokenResourceKey.SegmentedItemPadding);
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusXS);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSize);
        smallSizeStyle.Add(Layoutable.MinHeightProperty, SegmentedTokenResourceKey.ItemMinHeightSM);
        smallSizeStyle.Add(TemplatedControl.PaddingProperty, SegmentedTokenResourceKey.SegmentedItemPaddingSM);

        Add(smallSizeStyle);
    }

    private void BuildIconStyle()
    {
        var hasIconStyle =
            new Style(selector => selector.Nesting()
                                          .Not(x => x.Nesting().PropertyEquals(SegmentedItem.IconProperty, null)));
        {
            var labelStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
            labelStyle.Add(Layoutable.MarginProperty, SegmentedTokenResourceKey.SegmentedItemContentMargin);
            hasIconStyle.Add(labelStyle);
        }

        Add(hasIconStyle);

        var iconSelector = default(Selector).Nesting().Template().Name(IconContentPart).Child().OfType<Icon>();
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Large));
        {
            var iconStyle = new Style(selector => iconSelector);
            iconStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSizeLG);
            iconStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSizeLG);
            largeSizeStyle.Add(iconStyle);
        }
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Middle));
        {
            var iconStyle = new Style(selector => iconSelector);
            iconStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSize);
            iconStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSize);
            middleSizeStyle.Add(iconStyle);
        }
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Small));
        {
            var iconStyle = new Style(selector => iconSelector);
            iconStyle.Add(Layoutable.WidthProperty, GlobalTokenResourceKey.IconSizeSM);
            iconStyle.Add(Layoutable.HeightProperty, GlobalTokenResourceKey.IconSizeSM);
            smallSizeStyle.Add(iconStyle);
        }
        Add(smallSizeStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, GlobalTokenResourceKey.ColorTextDisabled);
        Add(disabledStyle);
    }
}