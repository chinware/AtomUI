using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using ExCSS;
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
        return new FuncControlTemplate<DataGrid>((dataGridCell, scope) =>
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
                Name = ContentPresenterPart
            };
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
                Width = 1,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Grid.SetColumn(contentPresenter, 1);
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

        var primaryFocusVisual = new Rectangle()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Fill = Brushes.Transparent,
            IsHitTestVisible = false,
            StrokeThickness = 2
        };
        
        var secondaryFocusVisual = new Rectangle()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch,
            Fill                = Brushes.Transparent,
            IsHitTestVisible    = false,
            StrokeThickness     = 1
        };
        focusVisualLayout.Children.Add(primaryFocusVisual);
        focusVisualLayout.Children.Add(secondaryFocusVisual);
        
        Grid.SetColumn(focusVisualLayout, 0);
        rootLayout.Children.Add(focusVisualLayout);
    }
}