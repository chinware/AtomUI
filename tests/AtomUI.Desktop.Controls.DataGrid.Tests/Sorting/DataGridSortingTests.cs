using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Sorting;

public class DataGridSortingTests
{
    static DataGridSortingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Sort_Reorders_Rows_When_ItemsSource_Is_Plain_ObservableCollection()
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
            CanUserSortColumns = true,
            ItemsSource = rows,
            Width = 480,
            Height = 240
        };

        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Age",
            Binding = new Binding(nameof(SortRow.Age))
        });

        var window = new Window
        {
            Width = 520,
            Height = 280,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            grid.Sort(0, ListSortDirection.Ascending);
            Dispatcher.UIThread.RunJobs();

            grid.CollectionView.ShouldNotBeNull();
            grid.CollectionView.Cast<SortRow>().Select(row => row.Age).ShouldBe([30, 32, 42]);
            GetDisplayedAges(grid).ShouldBe([30, 32, 42]);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Clicking_Sort_Indicator_Reorders_Rows()
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
            CanUserSortColumns = true,
            ItemsSource = rows,
            Width = 480,
            Height = 240
        };

        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Age",
            Binding = new Binding(nameof(SortRow.Age))
        });

        var window = new Window
        {
            Width = 520,
            Height = 280,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var sortIndicator = grid.GetVisualDescendants()
                                    .OfType<DataGridSortIndicator>()
                                    .Single(indicator => indicator.IsVisible &&
                                                         indicator.Bounds.Width > 0 &&
                                                         indicator.Bounds.Height > 0);
            Click(sortIndicator, window);
            Dispatcher.UIThread.RunJobs();

            grid.CollectionView.ShouldNotBeNull();
            grid.CollectionView.Cast<SortRow>().Select(row => row.Age).ShouldBe([30, 32, 42]);
            GetDisplayedAges(grid).ShouldBe([30, 32, 42]);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Clicking_Column_Level_Sort_Indicator_Reorders_Rows_When_Grid_Level_Sorting_Is_Disabled()
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
            ItemsSource = rows,
            Width = 480,
            Height = 240
        };

        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Age",
            Binding = new Binding(nameof(SortRow.Age)),
            CanUserSort = true
        });

        var window = new Window
        {
            Width = 520,
            Height = 280,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var sortIndicator = grid.GetVisualDescendants()
                                    .OfType<DataGridSortIndicator>()
                                    .Single(indicator => indicator.IsVisible &&
                                                         indicator.Bounds.Width > 0 &&
                                                         indicator.Bounds.Height > 0);
            Click(sortIndicator, window);
            Dispatcher.UIThread.RunJobs();

            grid.CollectionView.ShouldNotBeNull();
            grid.CollectionView.Cast<SortRow>().Select(row => row.Age).ShouldBe([30, 32, 42]);
            GetDisplayedAges(grid).ShouldBe([30, 32, 42]);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Clicking_Right_Edge_Of_Sort_Indicator_Reorders_Rows_When_Column_Can_Resize()
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
            CanUserResizeColumns = true,
            ItemsSource = rows,
            Width = 360,
            Height = 240
        };

        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Age",
            Binding = new Binding(nameof(SortRow.Age)),
            CanUserResize = true,
            CanUserSort = true,
            Width = new DataGridLength(72)
        });

        var window = new Window
        {
            Width = 400,
            Height = 280,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var sortIndicator = grid.GetVisualDescendants()
                                    .OfType<DataGridSortIndicator>()
                                    .Single(indicator => indicator.IsVisible &&
                                                         indicator.Bounds.Width > 0 &&
                                                         indicator.Bounds.Height > 0);
            Click(sortIndicator, window, new Point(sortIndicator.Bounds.Width - 1, sortIndicator.Bounds.Height / 2));
            Dispatcher.UIThread.RunJobs();

            grid.CollectionView.ShouldNotBeNull();
            grid.CollectionView.Cast<SortRow>().Select(row => row.Age).ShouldBe([30, 32, 42]);
            GetDisplayedAges(grid).ShouldBe([30, 32, 42]);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Clicking_Sort_Indicator_Reorders_Gallery_Data_With_Duplicate_Ages()
    {
        var rows = new ObservableCollection<SortRow>
        {
            new("John Brown", 32),
            new("Jim Green", 42),
            new("Joe Black", 32)
        };
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            ItemsSource = rows,
            Width = 720,
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
            CanUserResize = true,
            CanUserSort = true,
            Width = new DataGridLength(120)
        });

        var window = new Window
        {
            Width = 760,
            Height = 280,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var sortIndicator = grid.GetVisualDescendants()
                                    .OfType<DataGridSortIndicator>()
                                    .Single(indicator => indicator.IsVisible &&
                                                         indicator.Bounds.Width > 0 &&
                                                         indicator.Bounds.Height > 0);
            Click(sortIndicator, window);
            Dispatcher.UIThread.RunJobs();

            grid.CollectionView.ShouldNotBeNull();
            grid.CollectionView.Cast<SortRow>().Select(row => row.Name).ShouldBe(["John Brown", "Joe Black", "Jim Green"]);
            GetDisplayedNames(grid).ShouldBe(["John Brown", "Joe Black", "Jim Green"]);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Clicking_Sort_Indicator_Reorders_Rows_When_Binding_Is_Compiled()
    {
        var rows = new ObservableCollection<SortRow>
        {
            new("John Brown", 32),
            new("Jim Green", 42),
            new("Joe Black", 32)
        };
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns = false,
            CanUserResizeColumns = true,
            ItemsSource = rows,
            Width = 720,
            Height = 240
        };

        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Name",
            Binding = CompiledBinding.Create<SortRow, string>(row => row.Name),
            Width = new DataGridLength(160)
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header = "Age",
            Binding = CompiledBinding.Create<SortRow, int>(row => row.Age),
            CanUserResize = true,
            CanUserSort = true,
            Width = new DataGridLength(120)
        });

        var window = new Window
        {
            Width = 760,
            Height = 280,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var sortIndicator = grid.GetVisualDescendants()
                                    .OfType<DataGridSortIndicator>()
                                    .Single(indicator => indicator.IsVisible &&
                                                         indicator.Bounds.Width > 0 &&
                                                         indicator.Bounds.Height > 0);
            Click(sortIndicator, window);
            Dispatcher.UIThread.RunJobs();

            grid.CollectionView.ShouldNotBeNull();
            grid.CollectionView.Cast<SortRow>().Select(row => row.Name).ShouldBe(["John Brown", "Joe Black", "Jim Green"]);
            GetDisplayedNames(grid).ShouldBe(["John Brown", "Joe Black", "Jim Green"]);
        }
        finally
        {
            window.Close();
        }
    }

    private static void Click(Control control, Window window)
    {
        Click(control, window, new Point(control.Bounds.Width / 2, control.Bounds.Height / 2));
    }

    private static void Click(Control control, Window window, Point localPoint)
    {
        var point = control.TranslatePoint(
            localPoint,
            window);

        point.ShouldNotBeNull();
        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private static int[] GetDisplayedAges(global::AtomUI.Desktop.Controls.DataGrid grid)
    {
        return grid.GetVisualDescendants()
                   .OfType<DataGridRow>()
                   .Select(row => new
                   {
                       Row = row,
                       Position = row.TranslatePoint(new Point(0, 0), grid)
                   })
                   .Where(entry => entry.Position is not null)
                   .OrderBy(entry => entry.Position!.Value.Y)
                   .Select(entry => entry.Row.DataContext)
                   .OfType<SortRow>()
                   .Select(row => row.Age)
                   .ToArray();
    }

    private static string[] GetDisplayedNames(global::AtomUI.Desktop.Controls.DataGrid grid)
    {
        return grid.GetVisualDescendants()
                   .OfType<DataGridRow>()
                   .Select(row => new
                   {
                       Row = row,
                       Position = row.TranslatePoint(new Point(0, 0), grid)
                   })
                   .Where(entry => entry.Position is not null)
                   .OrderBy(entry => entry.Position!.Value.Y)
                   .Select(entry => entry.Row.DataContext)
                   .OfType<SortRow>()
                   .Select(row => row.Name)
                   .ToArray();
    }

    private sealed record SortRow(string Name, int Age);
}
