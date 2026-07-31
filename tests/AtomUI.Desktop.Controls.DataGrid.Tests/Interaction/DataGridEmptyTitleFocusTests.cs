using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Interaction;

public class DataGridEmptyTitleFocusTests
{
    static DataGridEmptyTitleFocusTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Title_Button_Click_Does_Not_Crash_When_ItemsSource_Is_Empty()
    {
        var grid = CreateGridWithTitleButton(out var titleButton, out var getClickCount);
        grid.ItemsSource = Array.Empty<GridRow>();
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(GridRow.Name))
        });

        VerifyTitleButtonCanReceiveClick(grid, titleButton, getClickCount);
    }

    [Fact]
    public void Title_Button_Click_Does_Not_Crash_When_ItemsSource_Is_Null()
    {
        var grid = CreateGridWithTitleButton(out var titleButton, out var getClickCount);
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Name",
            Binding = new Binding(nameof(GridRow.Name))
        });

        VerifyTitleButtonCanReceiveClick(grid, titleButton, getClickCount);
    }

    [Fact]
    public void Title_Button_Click_Does_Not_Crash_When_ItemsSource_And_Columns_Are_Missing()
    {
        var grid = CreateGridWithTitleButton(out var titleButton, out var getClickCount);

        VerifyTitleButtonCanReceiveClick(grid, titleButton, getClickCount);
    }

    private static global::AtomUI.Desktop.Controls.DataGrid CreateGridWithTitleButton(
        out Button titleButton,
        out Func<int> getClickCount)
    {
        titleButton = new Button
        {
            Content = "新增",
            Width   = 80,
            Height  = 32
        };
        var clickCount = 0;
        titleButton.Click += (_, _) => clickCount++;
        getClickCount = () => clickCount;

        var titlePanel = new StackPanel();
        titlePanel.Children.Add(titleButton);

        return new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns = false,
            Title               = titlePanel,
            Width               = 480,
            Height              = 260
        };
    }

    private static void VerifyTitleButtonCanReceiveClick(
        global::AtomUI.Desktop.Controls.DataGrid grid,
        Button titleButton,
        Func<int> getClickCount)
    {
        var window = new Window
        {
            Width   = 560,
            Height  = 340,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            titleButton.Focus();
            Dispatcher.UIThread.RunJobs();

            titleButton.IsFocused.ShouldBeTrue();
            grid.ContainsFocus.ShouldBeTrue();
            grid.DisplayData.FirstScrollingSlot.ShouldBe(-1);
            grid.DisplayData.LastScrollingSlot.ShouldBe(-1);
            grid.DisplayData.NumDisplayedScrollingElements.ShouldBe(0);

            Click(titleButton, window);
            Dispatcher.UIThread.RunJobs();

            getClickCount().ShouldBe(1);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
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

    private sealed record GridRow(string Name);
}
