using AtomUI.Controls.Utils;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridRowTheme : BaseControlTheme
{
    public const string BottomGridLinePart = "PART_BottomGridLine";
    public const string CellsPresenterPart = "PART_CellsPresenter";
    public const string DetailsPresenterPart = "PART_DetailsPresenter";
    public const string FramePart = "PART_Frame";
    public const string RootLayoutPart = "PART_RootLayout";
    public const string RowHeaderPart = "PART_RowHeader";
    public const string RowBackgroundPart = "PART_RowBackground";
    public const string InvalidVisualElementPart = "PART_InvalidVisualElement";

    public DataGridRowTheme() : base(typeof(DataGridRow))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DataGridRow>((dataGridRow, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart
            };

            var rootLayout = new DataGridFrozenGrid()
            {
                Name = RootLayoutPart,
                ColumnDefinitions = [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star),
                ],
                RowDefinitions = [
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto),
                ]
            };

            var rowBackground = new Rectangle()
            {
                Name = RowBackgroundPart,
            };
            Grid.SetRow(rowBackground, 0);
            Grid.SetColumn(rowBackground, 0);
            Grid.SetColumnSpan(rowBackground, 2);
            Grid.SetRowSpan(rowBackground, 2);
            rootLayout.Children.Add(rowBackground);
            
            var invalidVisualElement = new Rectangle()
            {
                Name = InvalidVisualElementPart,
                Opacity = 0
            };
            Grid.SetRow(rowBackground, 0);
            Grid.SetColumn(rowBackground, 0);
            Grid.SetColumnSpan(rowBackground, 2);
            rootLayout.Children.Add(invalidVisualElement);

            var rowHeader = new DataGridRowHeader()
            {
                Name = RowHeaderPart,
            };
            Grid.SetRow(rowHeader, 0);
            Grid.SetColumn(rowHeader, 0);
            Grid.SetRowSpan(rowHeader, 3);
            DataGridFrozenGrid.SetIsFrozen(rowHeader, true);
            rootLayout.Children.Add(rowHeader);

            var cellsPresenter = new DataGridCellsPresenter()
            {
                Name = CellsPresenterPart,
            };
            Grid.SetRow(cellsPresenter, 0);
            Grid.SetColumn(cellsPresenter, 1);
            DataGridFrozenGrid.SetIsFrozen(cellsPresenter, true);
            rootLayout.Children.Add(cellsPresenter);

            var detailsPresenter = new DataGridDetailsPresenter()
            {
                Name = DetailsPresenterPart,
            };
            Grid.SetRow(detailsPresenter, 1);
            Grid.SetColumn(detailsPresenter, 1);
            rootLayout.Children.Add(detailsPresenter);

            var bottomGridLine = new Rectangle()
            {
                Name = BottomGridLinePart,
                Height = 1,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            Grid.SetRow(bottomGridLine, 2);
            Grid.SetColumn(bottomGridLine, 1);
            rootLayout.Children.Add(bottomGridLine);
      
            frame.Child = rootLayout;
            return frame;
        });
    }
}