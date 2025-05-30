using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridColumnHeaderTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string HeaderRootLayoutPart = "PART_HeaderRootLayout";
    public const string VerticalSeparatorPart = "PART_VerticalSeparator";
    public const string ContentDecoratorPart = "PART_ContentDecorator";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string SortIconPresenterPart = "PART_SortIconPresenter";
    public const string FocusVisualPart = "PART_FocusVisual";
    public const string FocusVisualPrimaryPart = "PART_FocusVisualPrimary";
    public const string FocusVisualSecondaryPart = "PART_FocusVisualSecondary";

    public DataGridColumnHeaderTheme()
        : base(typeof(DataGridColumnHeader))
    {
    }
    
    protected DataGridColumnHeaderTheme(Type targetType)
        : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridColumnHeader>((columnHeader, scope) =>
        {
            var headerFrame = new Border()
            {
                Name = FramePart
            };
            CreateTemplateParentBinding(headerFrame, Border.BackgroundProperty, DataGridColumnHeader.BackgroundProperty);
            CreateTemplateParentBinding(headerFrame, Border.PaddingProperty, DataGridColumnHeader.PaddingProperty);

            var headerRootLayout = new Panel()
            {
                Name = HeaderRootLayoutPart
            };

            BuildContentPresenter(headerRootLayout);

            var verticalSeparator = new Rectangle()
            {
                Name              = VerticalSeparatorPart,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            headerRootLayout.Children.Add(verticalSeparator);

            BuildFocusVisual(headerRootLayout);

            headerFrame.Child = headerRootLayout;
            return headerFrame;
        });
    }

    private void BuildContentPresenter(Panel rootLayout)
    {
        var decorator = new Decorator
        {
            Name = ContentDecoratorPart
        };
        var gridLayout = new Grid
        {
            ColumnDefinitions =
            [
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
            ]
        };
        CreateTemplateParentBinding(gridLayout, Grid.HorizontalAlignmentProperty, DataGridColumnHeader.HorizontalAlignmentProperty);
        CreateTemplateParentBinding(gridLayout, Grid.VerticalAlignmentProperty, DataGridColumnHeader.VerticalAlignmentProperty);
        var contentPresenter = new ContentPresenter()
        {
            Name = ContentPresenterPart,
        };
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, DataGridColumnHeader.ContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, DataGridColumnHeader.ContentTemplateProperty);
        Grid.SetColumn(contentPresenter, 0);
        gridLayout.Children.Add(contentPresenter);

        var iconPresenter = new IconPresenter()
        {
            Name                = SortIconPresenterPart,
            IsVisible           = false
        };
        Grid.SetColumn(iconPresenter, 1);
        gridLayout.Children.Add(iconPresenter);
        decorator.Child = gridLayout;
        rootLayout.Children.Add(decorator);
    }

    private void BuildFocusVisual(Panel rootLayout)
    {
        var focusVisualLayout = new Panel()
        {
            Name      = FocusVisualPart,
            IsVisible = false,
            IsHitTestVisible = false,
        };

        var primaryFocusVisual = new Rectangle
        {
            Name                = FocusVisualPrimaryPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            Fill                = Brushes.Transparent,
            IsHitTestVisible    = false,
            StrokeThickness     = 2
        };

        var secondaryFocusVisual = new Rectangle
        {
            Name                = FocusVisualSecondaryPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            Fill                = Brushes.Transparent,
            IsHitTestVisible    = false,
            StrokeThickness     = 1
        };
        focusVisualLayout.Children.Add(primaryFocusVisual);
        focusVisualLayout.Children.Add(secondaryFocusVisual);
        
        rootLayout.Children.Add(focusVisualLayout);
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DataGridColumnHeader.ForegroundProperty, DataGridTokenKey.TableHeaderTextColor);
        commonStyle.Add(DataGridColumnHeader.BackgroundProperty, DataGridTokenKey.TableHeaderBg);
        commonStyle.Add(DataGridColumnHeader.FontWeightProperty, SharedTokenKey.FontWeightStrong);
        commonStyle.Add(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Left);
        commonStyle.Add(DataGridColumnHeader.VerticalContentAlignmentProperty, VerticalAlignment.Center);
        BuildSizeTypeStyle(commonStyle);
        BuildVerticalSeparatorStyle(commonStyle);
        Add(commonStyle);
    }

    private void BuildVerticalSeparatorStyle(Style commonStyle)
    {
        var scopeStyle     = new Style(selector => Selectors.Or(
            selector.Nesting().PropertyEquals(DataGridColumnHeader.IsFirstVisibleProperty, true),
            selector.Nesting().PropertyEquals(DataGridColumnHeader.IsMiddleVisibleProperty, true)));
        var separatorStyle = new Style(selector => selector.Nesting().Template().Name(VerticalSeparatorPart));
        separatorStyle.Add(Rectangle.FillProperty, DataGridTokenKey.HeaderSplitColor);
        separatorStyle.Add(Rectangle.WidthProperty, 1d);
        separatorStyle.Add(Rectangle.HeightProperty, SharedTokenKey.FontHeight);
        scopeStyle.Add(separatorStyle);
        commonStyle.Add(scopeStyle);
    }

    private void BuildSizeTypeStyle(Style commonStyle)
    {
        var largeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ListBox.SizeTypeProperty, SizeType.Large));
        {
            var contentDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(ContentDecoratorPart));
            contentDecoratorStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePadding);
            largeStyle.Add(contentDecoratorStyle);
        }
        largeStyle.Add(DataGridColumnHeader.FontSizeProperty, DataGridTokenKey.TableFontSize);
        commonStyle.Add(largeStyle);
        
        var middleStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ListBox.SizeTypeProperty, SizeType.Middle));
        {
            var contentDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(ContentDecoratorPart));
            contentDecoratorStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePaddingMiddle);
            middleStyle.Add(contentDecoratorStyle);
        }
        middleStyle.Add(DataGridColumnHeader.FontSizeProperty, DataGridTokenKey.TableFontSizeMiddle);
        commonStyle.Add(middleStyle);
        
        var smallStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ListBox.SizeTypeProperty, SizeType.Small));
        {
            var contentDecoratorStyle = new Style(selector => selector.Nesting().Template().Name(ContentDecoratorPart));
            contentDecoratorStyle.Add(Decorator.PaddingProperty, DataGridTokenKey.TablePaddingSmall);
            smallStyle.Add(contentDecoratorStyle);
        }
        smallStyle.Add(DataGridColumnHeader.FontSizeProperty, DataGridTokenKey.TableFontSizeSmall);
        commonStyle.Add(smallStyle);
    }
}