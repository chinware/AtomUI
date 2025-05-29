using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridRowGroupHeaderTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string ExpanderButtonPart = "PART_ExpanderButton";
    public const string IndentSpacerPart = "PART_IndentSpacer";
    public const string ItemCountElementPart = "PART_ItemCountElement";
    public const string PropertyNameElementPart = "PART_PropertyNameElement";
    public const string CurrencyVisualPart = "PART_CurrencyVisual";
    public const string FocusVisualPart = "PART_FocusVisual";
    public const string FocusVisualPrimaryPart = "PART_FocusVisualPrimary";
    public const string FocusVisualSecondaryPart = "PART_FocusVisualSecondary";
    public const string RowHeaderPart = "PART_RowHeader";
    public const string BottomGridLinePart = "PART_BottomGridLine";

    public DataGridRowGroupHeaderTheme()
        : base(typeof(DataGridRowGroupHeader))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridRowGroupHeader>((rowGroupHeader, scope) =>
        {
            var rootLayout = new DataGridFrozenGrid()
            {
                Name = RootLayoutPart,
                ColumnDefinitions =
                [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                ],
                RowDefinitions =
                [
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                ]
            };

            var indentSpacer = new Rectangle()
            {
                Name = IndentSpacerPart,
            };
            Grid.SetRow(indentSpacer, 0);
            Grid.SetColumn(indentSpacer, 1);
            rootLayout.Children.Add(indentSpacer);

            var expanderButton = new ToggleButton()
            {
                Name      = ExpanderButtonPart,
                Width     = 12,
                Height    = 12,
                Margin    = new Thickness(12, 0, 0, 0),
                IsTabStop = false
            };
            Grid.SetRow(expanderButton, 0);
            Grid.SetColumn(expanderButton, 2);
            rootLayout.Children.Add(expanderButton);

            BuildPropertyInfo(rootLayout);

            var currencyVisual = new Rectangle()
            {
                Name                = CurrencyVisualPart,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment   = VerticalAlignment.Stretch,
                Fill = Brushes.Transparent,
                IsHitTestVisible = false,
                StrokeThickness = 1
            };
            Grid.SetRow(currencyVisual, 0);
            Grid.SetColumn(currencyVisual, 0);
            Grid.SetColumnSpan(currencyVisual, 5);
            rootLayout.Children.Add(currencyVisual);

            BuildFocusVisual(rootLayout);

            var rowHeader = new DataGridRowHeader()
            {
                Name = RowHeaderPart
            };
            Grid.SetRow(rowHeader, 0);
            Grid.SetColumn(rowHeader, 0);
            Grid.SetRowSpan(rowHeader, 2);
            DataGridFrozenGrid.SetIsFrozen(rowHeader, true);
            
            rootLayout.Children.Add(rowHeader);

            var bottomGridLine = new Rectangle()
            {
                Name = BottomGridLinePart,
                Height = 1
            };
            Grid.SetRow(rowHeader, 1);
            Grid.SetColumn(rowHeader, 0);
            Grid.SetRowSpan(rowHeader, 5);
            rootLayout.Children.Add(bottomGridLine);
            
            return rootLayout;
        });
    }

    private void BuildPropertyInfo(DataGridFrozenGrid rootLayout)
    {
        var stackLayout = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(12, 0, 0, 0),
        };

        var propertyNameTextBlock = new TextBlock()
        {
            Name   = PropertyNameElementPart,
            Margin = new Thickness(4, 0, 0, 0),
        };
        stackLayout.Children.Add(propertyNameTextBlock);

        var keyTextBlock = new TextBlock()
        {
            Margin = new Thickness(4, 0, 0, 0),
        };
        stackLayout.Children.Add(keyTextBlock);

        var itemCountTextBlock = new TextBlock()
        {
            Margin = new Thickness(4, 0, 0, 0),
        };
        stackLayout.Children.Add(itemCountTextBlock);
        
        Grid.SetRow(stackLayout, 0);
        Grid.SetColumn(stackLayout, 3);
        rootLayout.Children.Add(stackLayout);
    }
    
    private void BuildFocusVisual(DataGridFrozenGrid rootLayout)
    {
        var focusVisualLayout = new Grid()
        {
            IsVisible        = false,
            Name             = FocusVisualPart,
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

        Grid.SetRow(focusVisualLayout, 0);
        Grid.SetColumn(focusVisualLayout, 0);
        Grid.SetColumnSpan(focusVisualLayout, 5);
        rootLayout.Children.Add(focusVisualLayout);
    }
}