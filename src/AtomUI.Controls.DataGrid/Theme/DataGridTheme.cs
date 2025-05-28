using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Layout;

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
    public const string TopRightCornePart = "PART_TopRightCorner";
    public const string BottomRightCornerPart = "PART_BottomRightCorner";
    public const string ColumnHeadersAndRowsSeparatorPart = "PART_ColumnHeadersAndRowsSeparator";
    public const string FramePart = "PART_Frame";
    public const string RootLayoutPart = "PART_RootLayout";
    public const string DisabledVisualElementPart = "PART_DisabledVisualElement";
    
    public DataGridTheme() : base(typeof(DataGrid))
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
                ColumnDefinitions = [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto)
                ],
                RowDefinitions = [
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                ],
                ClipToBounds = true
            };

            var topLeftCornerHeader = new DataGridColumnHeader()
            {
                Name = TopLeftCornerPart,
            };
            Grid.SetRow(topLeftCornerHeader, 0);
            Grid.SetColumn(topLeftCornerHeader, 0);
            rootLayout.Children.Add(topLeftCornerHeader);

            var columnHeadersPresenter = new DataGridColumnHeadersPresenter()
            {
                Name = ColumnHeadersPresenterPart,
            };
            Grid.SetRow(columnHeadersPresenter, 0);
            Grid.SetColumn(columnHeadersPresenter, 1);
            Grid.SetColumnSpan(columnHeadersPresenter, 2);
            rootLayout.Children.Add(columnHeadersPresenter);

            var columnHeadersAndRowsSeparator = new Rectangle()
            {
                Name = ColumnHeadersAndRowsSeparatorPart,
                VerticalAlignment = VerticalAlignment.Bottom,
                Height = 1
            };
            Grid.SetRow(columnHeadersAndRowsSeparator, 0);
            Grid.SetColumn(columnHeadersAndRowsSeparator, 0);
            Grid.SetColumnSpan(columnHeadersAndRowsSeparator, 3);
            rootLayout.Children.Add(columnHeadersAndRowsSeparator);

            var rowsPresenter = new DataGridRowsPresenter()
            {
                Name = RowsPresenterPart,
            };
            Grid.SetRow(rowsPresenter, 1);
            Grid.SetColumn(rowsPresenter, 0);
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
            Grid.SetRow(bottomRightCorner, 2);
            Grid.SetColumn(bottomRightCorner, 2);
            rootLayout.Children.Add(bottomRightCorner);

            var verticalScrollbar = new ScrollBar()
            {
                Name = VerticalScrollbarPart,
                Orientation = Orientation.Vertical,
            };
            Grid.SetRow(verticalScrollbar, 1);
            Grid.SetColumn(verticalScrollbar, 2);
            rootLayout.Children.Add(verticalScrollbar);

            var horizontalScrollbarLayout = new Grid()
            {
                ColumnDefinitions = [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                ]
            };
            Grid.SetRow(verticalScrollbar, 2);
            Grid.SetColumn(verticalScrollbar, 1);
            
            var frozenColumnScrollBarSpacer = new Rectangle()
            {
                Name = FrozenColumnScrollBarSpacerPart,
            };
            Grid.SetColumn(frozenColumnScrollBarSpacer, 0);
            horizontalScrollbarLayout.Children.Add(frozenColumnScrollBarSpacer);

            var horizontalScrollbar = new ScrollBar()
            {
                Name = HorizontalScrollbarPart,
                Orientation = Orientation.Horizontal,
            };
            Grid.SetColumn(frozenColumnScrollBarSpacer, 1);
            horizontalScrollbarLayout.Children.Add(horizontalScrollbar);
     
            rootLayout.Children.Add(horizontalScrollbarLayout);

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
}