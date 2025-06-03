using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridCellTheme : BaseControlTheme
{
    public const string CellFramePart = "PART_CellFrame";
    public const string CellRootLayoutPart = "PART_CellRootLayout";
    public const string RightGridLinePart = "PART_RightGridLine";
    public const string CurrencyVisualPart = "PART_CurrencyVisual";
    public const string FocusVisualPart = "PART_FocusVisual";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string InvalidVisualElementPart = "PART_InvalidVisualElement";
    
    public DataGridCellTheme()
        : base(typeof(DataGridCell))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridCell>((dataGridCell, scope) =>
        {
            var frame = new Border
            {
                Name = CellFramePart
            };

            var rootLayout = new Grid()
            {
                Name = CellRootLayoutPart,
                ColumnDefinitions = [
                    new ColumnDefinition(GridLength.Star), 
                    new ColumnDefinition(GridLength.Auto) 
                ]
            };

            var currencyVisual = new Rectangle()
            {
                Name = CurrencyVisualPart,
                IsVisible = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Fill = Brushes.Transparent,
                IsHitTestVisible = false,
                StrokeThickness = 1
            };
            Grid.SetColumn(currencyVisual, 0);
            rootLayout.Children.Add(currencyVisual);

            BuildFocusVisual(rootLayout);

            var contentPresenter = new ContentPresenter()
            {
                Name = ContentPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.MarginProperty, DataGridCell.PaddingProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty, DataGridCell.HorizontalContentAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty, DataGridCell.VerticalContentAlignmentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty, DataGridCell.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty, DataGridCell.ContentTemplateProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ForegroundProperty, DataGridCell.ForegroundProperty);
            
            Grid.SetColumn(contentPresenter, 0);
            rootLayout.Children.Add(contentPresenter);

            var invalidVisualElement = new Rectangle()
            {
                Name = InvalidVisualElementPart,
                IsVisible = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsHitTestVisible = false,
                StrokeThickness = 1
            };
            Grid.SetColumn(contentPresenter, 0);
            rootLayout.Children.Add(invalidVisualElement);

            var rightGridLine = new Rectangle()
            {
                Name = RightGridLinePart,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            rightGridLine.RegisterInNameScope(scope);
            Grid.SetColumn(rightGridLine, 1);
            rootLayout.Children.Add(rightGridLine);
            
            frame.Child = rootLayout;
            return frame;
        });
    }

    private void BuildFocusVisual(Grid rootLayout)
    {
        var focusVisualLayout = new Grid()
        {
            IsVisible        = false,
            Name             = FocusVisualPart,
            IsHitTestVisible = false,
        };

        var primaryFocusVisual = new Rectangle
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Fill = Brushes.Transparent,
            IsHitTestVisible = false,
            StrokeThickness = 1
        };
        
        focusVisualLayout.Children.Add(primaryFocusVisual);
        Grid.SetColumn(focusVisualLayout, 0);
        rootLayout.Children.Add(focusVisualLayout);
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DataGridCell.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        commonStyle.Add(DataGridCell.VerticalAlignmentProperty, VerticalAlignment.Stretch);
        commonStyle.Add(DataGridCell.FocusableProperty, false);
        
        var rightGridLineStyle = new Style(selector => selector.Nesting().Template().Name(RightGridLinePart));
        rightGridLineStyle.Add(Rectangle.FillProperty, DataGridTokenKey.TableBorderColor);
        commonStyle.Add(rightGridLineStyle);
        BuildSizeTypeStyle(commonStyle);
        Add(commonStyle);
    }

    private void BuildSizeTypeStyle(Style commonStyle)
    {
        var largeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridCell.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(DataGridCell.PaddingProperty, DataGridTokenKey.TablePadding);
        largeStyle.Add(DataGridCell.FontSizeProperty, DataGridTokenKey.TableFontSize);
        commonStyle.Add(largeStyle);

        var middleStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridCell.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(DataGridCell.PaddingProperty, DataGridTokenKey.TablePaddingMiddle);
        middleStyle.Add(DataGridCell.FontSizeProperty, DataGridTokenKey.TableFontSizeMiddle);
        commonStyle.Add(middleStyle);

        var smallStyle =
            new Style(selector => selector.Nesting().PropertyEquals(DataGridCell.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(DataGridCell.PaddingProperty, DataGridTokenKey.TablePaddingSmall);
        smallStyle.Add(DataGridCell.FontSizeProperty, DataGridTokenKey.TableFontSizeSmall);
        commonStyle.Add(smallStyle);
    }
}