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
                ContentControl.ContentProperty,
                BindingMode.Default, 
                new FuncValueConverter<object?, object?>(content =>
                {
                    if (content is string text)
                    {
                        return new SingleLineText()
                        {
                            Text = text,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                    }
                    return content;
                }));
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
        enabledStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Normal);
            Add(iconStyle);
        }

        // 选中状态
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(TemplatedControl.ForegroundProperty, SegmentedTokenKey.ItemSelectedColor);
        selectedStyle.Add(TemplatedControl.BackgroundProperty, SegmentedTokenKey.ItemSelectedBg);
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            selectedStyle.Add(iconStyle);
        }
        enabledStyle.Add(selectedStyle);

        // 没有被选中的状态
        var notSelectedStyle =
            new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(StdPseudoClass.Selected)));
        notSelectedStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
        notSelectedStyle.Add(TemplatedControl.ForegroundProperty, SegmentedTokenKey.ItemColor);

        // Hover 状态
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.BackgroundProperty, SegmentedTokenKey.ItemHoverBg);
        hoverStyle.Add(TemplatedControl.ForegroundProperty, SegmentedTokenKey.ItemHoverColor);
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Active);
            hoverStyle.Add(iconStyle);
        }
        notSelectedStyle.Add(hoverStyle);

        // Pressed 状态
        var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
        pressedStyle.Add(TemplatedControl.BackgroundProperty, SegmentedTokenKey.ItemActiveBg);
        notSelectedStyle.Add(pressedStyle);
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            pressedStyle.Add(iconStyle);
        }

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
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSizeLG);
        largeSizeStyle.Add(Layoutable.MinHeightProperty, SegmentedTokenKey.ItemMinHeightLG);
        largeSizeStyle.Add(TemplatedControl.PaddingProperty, SegmentedTokenKey.SegmentedItemPadding);
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        middleSizeStyle.Add(Layoutable.MinHeightProperty, SegmentedTokenKey.ItemMinHeight);
        middleSizeStyle.Add(TemplatedControl.PaddingProperty, SegmentedTokenKey.SegmentedItemPadding);
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusXS);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        smallSizeStyle.Add(Layoutable.MinHeightProperty, SegmentedTokenKey.ItemMinHeightSM);
        smallSizeStyle.Add(TemplatedControl.PaddingProperty, SegmentedTokenKey.SegmentedItemPaddingSM);

        Add(smallSizeStyle);
    }

    private void BuildIconStyle()
    {
        var hasIconStyle =
            new Style(selector => selector.Nesting()
                                          .Not(x => x.Nesting().PropertyEquals(SegmentedItem.IconProperty, null)));
        {
            var labelStyle = new Style(selector => selector.Nesting().Template().Name(ContentPart));
            labelStyle.Add(Layoutable.MarginProperty, SegmentedTokenKey.SegmentedItemContentMargin);
            hasIconStyle.Add(labelStyle);
        }

        Add(hasIconStyle);
        
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Icon.NormalFilledBrushProperty, SegmentedTokenKey.ItemColor);
            iconStyle.Add(Icon.ActiveFilledBrushProperty, SegmentedTokenKey.ItemHoverColor);
            iconStyle.Add(Icon.SelectedFilledBrushProperty, SegmentedTokenKey.ItemSelectedColor);
            iconStyle.Add(Icon.DisabledFilledBrushProperty, SharedTokenKey.ColorTextDisabled);
            Add(iconStyle);
        }
        
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Large));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeLG);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeLG);
            largeSizeStyle.Add(iconStyle);
        }
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Middle));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSize);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSize);
            middleSizeStyle.Add(iconStyle);
        }
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(SegmentedItem.SizeTypeProperty, SizeType.Small));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeSM);
            iconStyle.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeSM);
            smallSizeStyle.Add(iconStyle);
        }
        Add(smallSizeStyle);
    }

    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(IconContentPart).Child().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Disabled);
            disabledStyle.Add(iconStyle);
        }
        Add(disabledStyle);
    }
}