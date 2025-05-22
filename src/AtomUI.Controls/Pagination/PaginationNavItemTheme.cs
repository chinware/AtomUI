using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
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
using Avalonia.Styling;

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
            CreateTemplateParentBinding(mainFrame, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(mainFrame, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
            
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
        
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        enabledStyle.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
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
        
        indicatorStyle.Add(selectedStyle);
        var notSelectedStyle =
            new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(StdPseudoClass.Selected)));
        notSelectedStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorTransparent);
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