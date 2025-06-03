using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DataGridRowTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string RowCellsFramePart = "PART_CellsFrame";
    public const string BottomGridLinePart = "PART_BottomGridLine";
    public const string CellsPresenterPart = "PART_CellsPresenter";
    public const string DetailsPresenterPart = "PART_DetailsPresenter";
    public const string RootLayoutPart = "PART_RootLayout";
    public const string RowHeaderPart = "PART_RowHeader";
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
                RowDefinitions = [
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto),
                    new RowDefinition(GridLength.Auto)
                ],
                ColumnDefinitions = [
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Star)
                ]
            };

            var rowCellsFrame = new Border
            {
                Name = RowCellsFramePart,
            };
            Grid.SetRow(rowCellsFrame, 0);
            Grid.SetColumn(rowCellsFrame, 0);
            Grid.SetColumnSpan(rowCellsFrame, 2);
            Grid.SetRowSpan(rowCellsFrame, 2);
            rootLayout.Children.Add(rowCellsFrame);
            
            var invalidVisualElement = new Rectangle
            {
                Name = InvalidVisualElementPart,
                Opacity = 0
            };
            Grid.SetRow(invalidVisualElement, 0);
            Grid.SetColumn(invalidVisualElement, 0);
            Grid.SetColumnSpan(invalidVisualElement, 2);
            rootLayout.Children.Add(invalidVisualElement);

            var rowHeader = new DataGridRowHeader()
            {
                Name = RowHeaderPart,
            };
            rowHeader.RegisterInNameScope(scope);
            Grid.SetRow(rowHeader, 0);
            Grid.SetColumn(rowHeader, 0);
            Grid.SetRowSpan(rowHeader, 3);
            DataGridFrozenGrid.SetIsFrozen(rowHeader, true);
            rootLayout.Children.Add(rowHeader);

            var cellsPresenter = new DataGridCellsPresenter
            {
                Name = CellsPresenterPart,
            };
            cellsPresenter.RegisterInNameScope(scope);
            Grid.SetRow(cellsPresenter, 0);
            Grid.SetColumn(cellsPresenter, 1);
            DataGridFrozenGrid.SetIsFrozen(cellsPresenter, true);
            rootLayout.Children.Add(cellsPresenter);

            var detailsPresenter = new DataGridDetailsPresenter
            {
                Name = DetailsPresenterPart,
            };
            detailsPresenter.RegisterInNameScope(scope);
            Grid.SetRow(detailsPresenter, 1);
            Grid.SetColumn(detailsPresenter, 1);
            rootLayout.Children.Add(detailsPresenter);

            var bottomGridLine = new Rectangle
            {
                Name = BottomGridLinePart,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            bottomGridLine.RegisterInNameScope(scope);
            Grid.SetRow(bottomGridLine, 2);
            Grid.SetColumn(bottomGridLine, 1);
            rootLayout.Children.Add(bottomGridLine);
      
            frame.Child = rootLayout;
            return frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DataGridRow.FocusableProperty, false);
        var bottomGridLineStyle = new Style(selector => selector.Nesting().Template().Name(BottomGridLinePart));
        bottomGridLineStyle.Add(Rectangle.FillProperty, DataGridTokenKey.TableBorderColor);
        commonStyle.Add(bottomGridLineStyle);
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, DataGridTokenKey.TableBg);
            commonStyle.Add(frameStyle);
        }
        
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.BackgroundProperty, DataGridTokenKey.TableRowHoverBg);
            hoverStyle.Add(frameStyle);
        }
        commonStyle.Add(hoverStyle);
        
        // 动画设置
        var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(DataGridRow.IsMotionEnabledProperty, true));
        {
            var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
            frameStyle.Add(Border.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
            {
                AnimationUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty, SharedTokenKey.MotionDurationSlow)
            }));
            isMotionEnabledStyle.Add(frameStyle);
        }
        commonStyle.Add(isMotionEnabledStyle);
        
        Add(commonStyle);
    }
}