using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Layout;

public class DataGridStarColumnLayoutTests
{
    static DataGridStarColumnLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Star_Column_Consumes_Remaining_Width_Instead_Of_Leaving_Filler()
    {
        var descriptionColumn = new DataGridTextColumn
        {
            Header   = "Description",
            Binding  = new Binding(nameof(GridRow.Description)),
            MinWidth = 520,
            Width    = new DataGridLength(1, DataGridLengthUnitType.Star)
        };
        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns          = false,
            HeadersVisibility            = DataGridHeadersVisibility.Column,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            LeftFrozenColumnCount        = 1,
            ItemsSource = new[]
            {
                new GridRow("Size", "Used to control the component dimensions.", "double", "32")
            },
            Width  = 1280,
            Height = 260
        };

        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Property",
            Binding = new Binding(nameof(GridRow.Property)),
            Width   = new DataGridLength(160)
        });
        grid.Columns.Add(descriptionColumn);
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Type",
            Binding = new Binding(nameof(GridRow.Type)),
            Width   = new DataGridLength(180)
        });
        grid.Columns.Add(new DataGridTextColumn
        {
            Header  = "Default",
            Binding = new Binding(nameof(GridRow.Default)),
            Width   = new DataGridLength(120)
        });

        var window = new Window
        {
            Width   = 1320,
            Height  = 360,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            descriptionColumn.ActualWidth.ShouldBeGreaterThan(520);
            grid.ColumnsInternal.FillerColumn.ShouldNotBeNull();
            grid.ColumnsInternal.FillerColumn.FillerWidth.ShouldBe(0, 1);
        }
        finally
        {
            window.Close();
        }
    }

    private sealed record GridRow(string Property, string Description, string Type, string Default);
}
