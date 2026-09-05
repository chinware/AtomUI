using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Sorting;

// https://github.com/AtomUI/AtomUI/issues/454 回归测试
// DataGrid 选中某行后点击列排序：行选中背景画在 DataGridRow 上，排序列单元格的
// 不透明 BodySortBg 画在 DataGridCell 上并覆盖行选中背景，导致选中行在排序列处丢失选中色。
// 参照 Ant Design 表格（selection.ts）：选中行背景优先于排序列底色（tableBodySortBg），
// 未选中行保留排序列底色。
public class DataGridSortColumnSelectedRowCellStyleIssue454Tests
{
    static DataGridSortColumnSelectedRowCellStyleIssue454Tests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SortedColumnCell_InSelectedRow_ShouldRenderSelectedRowBackground()
    {
        var (grid, window, rows) = CreateGridWithFirstRowSelectedAndSortedByAge();

        try
        {
            var selectedRow = grid.GetVisualDescendants()
                                  .OfType<DataGridRow>()
                                  .Single(row => ReferenceEquals(row.DataContext, rows[0]));
            selectedRow.IsSelected.ShouldBeTrue();

            var selectedRowBrush = selectedRow.Background as ISolidColorBrush;
            selectedRowBrush.ShouldNotBeNull($"选中行背景应为实色画刷，实际为 {selectedRow.Background}");
            var selectedRowBg = selectedRowBrush!.Color;
            selectedRowBg.A.ShouldBe((byte)255);

            var (sortedCell, unsortedCell) = GetBodyCells(selectedRow);

            // 单元格绘制在行背景之上：未排序列单元格透明，可见颜色 = 行选中背景
            EffectiveCellColor(unsortedCell, selectedRowBg).ShouldBe(selectedRowBg);

            // 期望：排序列单元格在选中行上的可见背景同样是行选中背景（选中优先于排序底色）
            EffectiveCellColor(sortedCell, selectedRowBg).ShouldBe(selectedRowBg,
                $"排序列单元格可见背景应为行选中背景 {selectedRowBg}，实际被排序列背景 {DescribeBrush(sortedCell.Background)} 覆盖");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void SortedColumnCell_InUnselectedRow_KeepsSortTint_AfterSorting()
    {
        var (grid, window, rows) = CreateGridWithFirstRowSelectedAndSortedByAge();

        try
        {
            var unselectedRow = grid.GetVisualDescendants()
                                    .OfType<DataGridRow>()
                                    .Single(row => ReferenceEquals(row.DataContext, rows[1]));
            unselectedRow.IsSelected.ShouldBeFalse();

            var (sortedCell, unsortedCell) = GetBodyCells(unselectedRow);

            // 未选中行保留排序列底色（参照 antd td.ant-table-column-sort / tableBodySortBg）
            var sortTint = sortedCell.Background as ISolidColorBrush;
            sortTint.ShouldNotBeNull($"排序列单元格在未选中行应为实色排序底色，实际为 {sortedCell.Background}");
            sortTint!.Color.A.ShouldBe((byte)255,
                $"排序底色应为不透明色以遮住行背景，实际为 {sortTint.Color}");

            var unsortedBrush = unsortedCell.Background.ShouldBeAssignableTo<ISolidColorBrush>(
                $"未排序列单元格背景应为实色画刷，实际为 {unsortedCell.Background}");
            unsortedBrush.ShouldNotBeNull();
            unsortedBrush.Color.A.ShouldBe((byte)0,
                $"未排序列单元格应保持透明以透出行背景，实际为 {unsortedCell.Background}");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>
    /// 构造 issue #454 场景：三行数据，选中第一行（John Brown）后点击 Age 列排序指示器排序。
    /// </summary>
    private static (global::AtomUI.Desktop.Controls.DataGrid Grid, Window Window,
        ObservableCollection<SortRow> Rows) CreateGridWithFirstRowSelectedAndSortedByAge()
    {
        var rows = new ObservableCollection<SortRow>
        {
            new("John Brown", 32),
            new("Jim Green", 42),
            new("Joe Black", 30)
        };
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns = false,
            SelectionMode = DataGridSelectionMode.Extended,
            ItemsSource = rows,
            Width = 480,
            Height = 240
        };

        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = new Binding(nameof(SortRow.Name)),
            Width = new DataGridLength(160)
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Age",
            Binding = new Binding(nameof(SortRow.Age)),
            CanUserSort = true,
            Width = new DataGridLength(120)
        });

        // 选中某行
        grid.SelectedItems.Add(rows[0]);

        var window = new Window
        {
            Width = 520,
            Height = 280,
            Content = grid
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();

        // 点击列排序（点击 Age 列的排序指示器）
        var sortIndicator = grid.GetVisualDescendants()
                                .OfType<DataGridSortIndicator>()
                                .Single(indicator => indicator.IsVisible &&
                                                     indicator.Bounds.Width > 0 &&
                                                     indicator.Bounds.Height > 0);
        Click(sortIndicator, window);
        Dispatcher.UIThread.RunJobs();

        grid.CollectionView.ShouldNotBeNull();
        grid.CollectionView.Cast<SortRow>().Select(row => row.Age).ShouldBe([30, 32, 42]);

        return (grid, window, rows);
    }

    private static (DataGridCell SortedCell, DataGridCell UnsortedCell) GetBodyCells(DataGridRow row)
    {
        var cells = row.GetVisualDescendants()
                       .OfType<DataGridCell>()
                       .Where(cell => cell.ColumnIndex >= 0)
                       .OrderBy(cell => cell.ColumnIndex)
                       .ToArray();
        cells.Length.ShouldBe(2);

        var sortedCell   = cells[1]; // Age 列，排序后 IsSorting=True
        var unsortedCell = cells[0]; // Name 列
        sortedCell.IsSorting.ShouldBeTrue();
        unsortedCell.IsSorting.ShouldBeFalse();
        return (sortedCell, unsortedCell);
    }

    // 单元格位于行背景之上渲染：单元格自身绘制不透明背景时可见颜色为其自身背景，
    // 否则透出行背景。与 ControlTheme 中 Row/Cell 两个 Border#Frame 的叠加顺序一致。
    private static Color EffectiveCellColor(DataGridCell cell, Color rowBackground)
    {
        if (cell.Background is ISolidColorBrush brush && brush.Color.A > 0)
        {
            return brush.Color;
        }

        return rowBackground;
    }

    private static string DescribeBrush(IBrush? brush)
    {
        return brush switch
        {
            ISolidColorBrush solid => solid.Color.ToString(),
            null => "<null>",
            _ => brush.GetType().Name
        };
    }

    private static void Click(Control control, Window window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private sealed record SortRow(string Name, int Age);
}
