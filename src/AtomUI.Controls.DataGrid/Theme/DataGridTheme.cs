using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridTheme : BaseControlTheme
{
    public const string RowsPresenterPart = "PART_RowPresenter";
    public const string ColumnHeadersPresenterPart = "PART_ColumnHeadersPresenter";
    public const string FrozenColumnScrollBarSpacerPart = "PART_FrozenColumnScrollBarSpacer";
    public const string HorizontalScrollbarPart = "PART_HorizontalScrollbar";
    public const string VerticalScrollbarPart = "PART_VerticalScrollbar";
    public const string TopLeftCornerPart = "PART_TopLeftCorner";
    public const string TopRightCornerPart = "PART_TopRightCorner";
    public const string BottomRightCornerPart = "PART_BottomRightCorner";
    public const string ColumnHeadersAndRowsSeparatorPart = "PART_ColumnHeadersAndRowsSeparator";
    public const string FramePart = "PART_Frame";
    public const string RootLayoutPart = "PART_RootLayout";
    public const string DisabledVisualElementPart = "PART_DisabledVisualElement";
    
    public DataGridTheme()
        : base(typeof(DataGrid))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGrid>((dataGrid, scope) =>
        {
            var frame = new Border()
            {
                Name = FramePart
            };
            var rootLayout = new Grid()
            {
                Name = RootLayoutPart,
                RowDefinitions = [
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                ],
                ColumnDefinitions = [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                ],
                ClipToBounds = true
            };

            var topLeftCornerHeader = new DataGridTopLeftColumnHeader()
            {
                Name = TopLeftCornerPart,
            };
            topLeftCornerHeader.RegisterInNameScope(scope);
            Grid.SetRow(topLeftCornerHeader, 0);
            Grid.SetColumn(topLeftCornerHeader, 0);
            rootLayout.Children.Add(topLeftCornerHeader);

            var columnHeadersPresenter = new DataGridColumnHeadersPresenter()
            {
                Name = ColumnHeadersPresenterPart,
            };

            columnHeadersPresenter.RegisterInNameScope(scope);
            Grid.SetRow(columnHeadersPresenter, 0);
            Grid.SetColumn(columnHeadersPresenter, 1);
            Grid.SetColumnSpan(columnHeadersPresenter, 2);
            rootLayout.Children.Add(columnHeadersPresenter);

            var columnHeadersAndRowsSeparator = new Rectangle()
            {
                Name = ColumnHeadersAndRowsSeparatorPart,
                VerticalAlignment = VerticalAlignment.Bottom,
            };
            
            CreateTemplateParentBinding(columnHeadersAndRowsSeparator, Rectangle.HeightProperty, DataGrid.BorderThicknessProperty,
                BindingMode.Default, new FuncValueConverter<Thickness, double>(thickness => thickness.Left));
            
            Grid.SetRow(columnHeadersAndRowsSeparator, 0);
            Grid.SetColumn(columnHeadersAndRowsSeparator, 0);
            Grid.SetColumnSpan(columnHeadersAndRowsSeparator, 3);
            rootLayout.Children.Add(columnHeadersAndRowsSeparator);

            var rowsPresenter = new DataGridRowsPresenter()
            {
                Name = RowsPresenterPart,
            };
            rowsPresenter.RegisterInNameScope(scope);
            Grid.SetRow(rowsPresenter, 1);
            Grid.SetColumn(rowsPresenter, 0);
            Grid.SetRowSpan(rowsPresenter, 2);
            Grid.SetColumnSpan(rowsPresenter, 3);
            CreateTemplateParentBinding(rowsPresenter, ScrollViewer.IsScrollInertiaEnabledProperty, DataGrid.IsScrollInertiaEnabledProperty);
            var scrollGestureRecognizer = new ScrollGestureRecognizer()
            {
                CanHorizontallyScroll = true,
                CanVerticallyScroll = true,
            };
            BindUtils.RelayBind(rowsPresenter, ScrollViewer.IsScrollInertiaEnabledProperty, scrollGestureRecognizer, ScrollGestureRecognizer.IsScrollInertiaEnabledProperty);
            rowsPresenter.GestureRecognizers.Add(scrollGestureRecognizer);
            rootLayout.Children.Add(rowsPresenter);

            var bottomRightCorner = new Rectangle()
            {
                Name = BottomRightCornerPart,
            };
            bottomRightCorner.RegisterInNameScope(scope);
            Grid.SetRow(bottomRightCorner, 2);
            Grid.SetColumn(bottomRightCorner, 2);
            rootLayout.Children.Add(bottomRightCorner);

            var verticalScrollbar = new ScrollBar()
            {
                Name = VerticalScrollbarPart,
                Orientation = Orientation.Vertical,
            };
            verticalScrollbar.RegisterInNameScope(scope);
            Grid.SetRow(verticalScrollbar, 1);
            Grid.SetColumn(verticalScrollbar, 2);
            rootLayout.Children.Add(verticalScrollbar);

            BuildHorizontalScrollbar(rootLayout, scope);

            var disabledVisualElement = new Border()
            {
                Name = DisabledVisualElementPart,
                IsHitTestVisible = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                CornerRadius = new CornerRadius(2)
            };
            CreateTemplateParentBinding(disabledVisualElement, Border.IsEnabledProperty, Border.IsEnabledProperty);
            Grid.SetRow(disabledVisualElement, 0);
            Grid.SetColumn(disabledVisualElement, 0);
            Grid.SetRowSpan(disabledVisualElement, 4);
            Grid.SetColumnSpan(disabledVisualElement, 3);
            rootLayout.Children.Add(disabledVisualElement);
            
            frame.Child = rootLayout;
            return frame;
        });
    }

    private void BuildHorizontalScrollbar(Grid rootLayout, INameScope scope)
    {
        var horizontalScrollbarLayout = new Grid()
        {
            ColumnDefinitions = [
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
            ]
        };
        Grid.SetRow(horizontalScrollbarLayout, 2);
        Grid.SetColumn(horizontalScrollbarLayout, 1);
        var frozenColumnScrollBarSpacer = new Rectangle()
        {
            Name = FrozenColumnScrollBarSpacerPart,
        };
        frozenColumnScrollBarSpacer.RegisterInNameScope(scope);
        Grid.SetColumn(frozenColumnScrollBarSpacer, 0);
        horizontalScrollbarLayout.Children.Add(frozenColumnScrollBarSpacer);

        var horizontalScrollbar = new ScrollBar()
        {
            Name        = HorizontalScrollbarPart,
            Orientation = Orientation.Horizontal,
        };
        horizontalScrollbar.RegisterInNameScope(scope);
        Grid.SetColumn(horizontalScrollbar, 1);
        horizontalScrollbarLayout.Children.Add(horizontalScrollbar);
     
        rootLayout.Children.Add(horizontalScrollbarLayout);
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DataGrid.HeadersVisibilityProperty, DataGridHeadersVisibility.Column);
        commonStyle.Add(DataGrid.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
        commonStyle.Add(DataGrid.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
        commonStyle.Add(DataGrid.SelectionModeProperty, DataGridSelectionMode.Extended);
        commonStyle.Add(DataGrid.FocusAdornerProperty, null);
        commonStyle.Add(DataGrid.HorizontalGridLinesBrushProperty, DataGridTokenKey.TableBorderColor);
        commonStyle.Add(DataGrid.VerticalGridLinesBrushProperty, DataGridTokenKey.TableBorderColor);

        var columnHeadersAndRowsSeparatorStyle = new Style(selector => selector.Nesting().Template().Name(ColumnHeadersAndRowsSeparatorPart));
        columnHeadersAndRowsSeparatorStyle.Add(Rectangle.FillProperty, DataGridTokenKey.TableBorderColor);
        commonStyle.Add(columnHeadersAndRowsSeparatorStyle);
        
        Add(commonStyle);
    }
}