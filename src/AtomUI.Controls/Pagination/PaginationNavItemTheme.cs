using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using Selectors = Avalonia.Styling.Selectors;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PaginationNavItemTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayoutPart";
    public const string MainFramePart = "PART_MainFrame";
    public const string ContentPart = "PART_Content";
    
    public PaginationNavItemTheme()
        : base(typeof(PaginationNavItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<PaginationNavItem>((paginationNavItem, scope) =>
        {
            var rootLayout = new Panel
            {
                Name = RootLayoutPart
            };
            var mainFrame = new Border
            {
                Name = MainFramePart
            };
            rootLayout.Children.Add(mainFrame);
            CreateTemplateParentBinding(mainFrame, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(mainFrame, Border.BorderBrushProperty, TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(mainFrame, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(mainFrame, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(mainFrame, Border.BorderThicknessProperty, TemplatedControl.BorderThicknessProperty);
            
            var contentPresenter = new ContentPresenter
            {
                Name                       = ContentPart,
                HorizontalAlignment        = HorizontalAlignment.Center,
                VerticalAlignment          = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
            };
            contentPresenter.RegisterInNameScope(scope);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty,
                BindingMode.Default, 
                new FuncValueConverter<object?, object?>(content =>
                {
                    if (content is string text)
                    {
                        return new TextBlock()
                        {
                            Text              = text,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                    }
                    return content;
                }));
            rootLayout.Children.Add(contentPresenter);
            return rootLayout;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        commonStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);
        commonStyle.Add(TemplatedControl.PaddingProperty, PaginationTokenKey.PaginationItemPaddingInline);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorTransparent);
        
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        var handleStyle = new Style(selector => selector.Nesting().Not(selector.Nesting().PropertyEquals(PaginationNavItem.PaginationItemTypeProperty, PaginationItemType.Ellipses)));
        handleStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        enabledStyle.Add(handleStyle);
        BuildPageIndicatorEnabledStyle(enabledStyle);
        BuildPreviousAndNextItemEnabledStyle(enabledStyle);
        commonStyle.Add(enabledStyle);
        Add(commonStyle);
        BuildSizeTypeStyle();
        BuildDisabledStyle();
    }

    private void BuildPageIndicatorEnabledStyle(Style enabledStyle)
    {
        var indicatorStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(PaginationNavItem.PaginationItemTypeProperty,
                PaginationItemType.PageIndicator));
        var selectedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Selected));
        selectedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimary);
        selectedStyle.Add(TemplatedControl.BackgroundProperty, PaginationTokenKey.ItemActiveBg);
        selectedStyle.Add(TemplatedControl.FontWeightProperty, FontWeight.Bold);
        selectedStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorPrimary);

        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorPrimaryHover);
            hoverStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorPrimaryHover);
            selectedStyle.Add(hoverStyle);
        }
        
        indicatorStyle.Add(selectedStyle);
        var notSelectedStyle =
            new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(StdPseudoClass.Selected)));
        notSelectedStyle.Add(TemplatedControl.BackgroundProperty, PaginationTokenKey.ItemBg);
        notSelectedStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorText);
        {
            var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
            hoverStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgTextHover);
            notSelectedStyle.Add(hoverStyle);
        }
        {
            var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
            pressedStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgTextActive);
            notSelectedStyle.Add(pressedStyle);
        }
        indicatorStyle.Add(notSelectedStyle);
        enabledStyle.Add(indicatorStyle);
    }

    private void BuildPreviousAndNextItemEnabledStyle(Style enabledStyle)
    {
        var previousAndNextStyle = new Style(selector =>
            Selectors.Or(selector.Nesting().PropertyEquals(PaginationNavItem.PaginationItemTypeProperty,
                PaginationItemType.Previous),
                selector.Nesting().PropertyEquals(PaginationNavItem.PaginationItemTypeProperty,
                    PaginationItemType.Next)));
        previousAndNextStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
        previousAndNextStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorText);
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        hoverStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgTextHover);
        previousAndNextStyle.Add(hoverStyle);
        var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
        pressedStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgTextActive);
        previousAndNextStyle.Add(pressedStyle);
        enabledStyle.Add(previousAndNextStyle);
    }
    
    private void BuildSizeTypeStyle()
    {
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(PaginationNavItem.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(Layoutable.HeightProperty, PaginationTokenKey.ItemSize);
        largeSizeStyle.Add(Layoutable.MinWidthProperty, PaginationTokenKey.ItemSize);
        Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(PaginationNavItem.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(Layoutable.HeightProperty, PaginationTokenKey.ItemSize);
        middleSizeStyle.Add(Layoutable.MinWidthProperty, PaginationTokenKey.ItemSize);
        Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(PaginationNavItem.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(Layoutable.HeightProperty, PaginationTokenKey.ItemSizeSM);
        smallSizeStyle.Add(Layoutable.MinWidthProperty, PaginationTokenKey.ItemSizeSM);
        Add(smallSizeStyle);
    }
     
    private void BuildDisabledStyle()
    {
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        Add(disabledStyle);
    }
}