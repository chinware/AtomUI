using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateGridScenarios()
    {
        return
        [
            new PerfScenario("Grid.Row.Col12x2", _ => CreateGridRow(count: 2, span: 12, width: 480)),
            new PerfScenario("Grid.Row.Col8x6", _ => CreateGridRow(count: 6, span: 8, width: 480)),
            new PerfScenario("Grid.Row.Wrap.Col6x12", _ => CreateGridRow(count: 12, span: 6, width: 480)),
            new PerfScenario("Grid.Row.Ordered.Col6x8", _ => CreateOrderedGridRow()),
            new PerfScenario("Grid.Row.Responsive.Col8x6", _ => CreateResponsiveGridRow()),
            new PerfScenario("Grid.Row.Gutter.Col6x8", _ => CreateGridRow(
                count: 8,
                span: 6,
                width: 480,
                gutter: new GridGutter(new GridGutterInfo(16), new GridGutterInfo(8))))
        ];
    }

    private static Row CreateGridRow(int count, int span, double width, GridGutter? gutter = null)
    {
        var row = new Row
        {
            Width  = width,
            Gutter = gutter ?? new GridGutter()
        };

        for (var i = 0; i < count; i++)
        {
            row.Children.Add(CreateGridCol(span, i));
        }

        return row;
    }

    private static Row CreateOrderedGridRow()
    {
        var row = CreateGridRow(count: 8, span: 6, width: 480);
        for (var i = 0; i < row.Children.Count; i++)
        {
            if (row.Children[i] is Col col)
            {
                col.Order = i % 3 == 0 ? -1 : i % 2;
            }
        }

        return row;
    }

    private static Row CreateResponsiveGridRow()
    {
        var row = new Row
        {
            Width = 480
        };

        for (var i = 0; i < 6; i++)
        {
            var col = CreateGridCol(24, i);
            col.Sm = new GridColSize { Span = 12 };
            col.Md = new GridColSize { Span = 8 };
            row.Children.Add(col);
        }

        return row;
    }

    private static Col CreateGridCol(int span, int index)
    {
        return new Col
        {
            Span    = new GridColSpanInfo(span),
            Content = new Border
            {
                Height     = 32,
                Background = index % 2 == 0 ? Brushes.DodgerBlue : Brushes.RoyalBlue
            }
        };
    }
}
