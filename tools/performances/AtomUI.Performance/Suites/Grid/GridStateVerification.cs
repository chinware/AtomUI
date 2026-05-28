using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunGridStateVerification()
    {
        var failures = new List<string>();
        VerifyRowReactsToColSpanChange(failures);
        VerifyRowReactsToColOrderChange(failures);
        VerifyRowReactsToChildVisibilityChange(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Grid state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Grid state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }

        return false;
    }

    private static void VerifyRowReactsToColSpanChange(ICollection<string> failures)
    {
        var first = CreateNamedGridCol("first", span: 12);
        var second = CreateNamedGridCol("second", span: 12);
        var row = CreateVerificationGridRow(first, second);

        using var realized = RealizeControl(row);
        RefreshLayout(realized.Window);

        first.Span = new GridColSpanInfo(24);
        RefreshLayout(realized.Window);

        Expect(first.Bounds.Width > 230 && second.Bounds.Y > first.Bounds.Y,
            $"Row should rebuild cached Col layout after Span changes; first width={first.Bounds.Width}, second y={second.Bounds.Y}, first y={first.Bounds.Y}.",
            failures);
    }

    private static void VerifyRowReactsToColOrderChange(ICollection<string> failures)
    {
        var first = CreateNamedGridCol("first", span: 12);
        var second = CreateNamedGridCol("second", span: 12);
        var row = CreateVerificationGridRow(first, second);

        using var realized = RealizeControl(row);
        RefreshLayout(realized.Window);

        second.Order = -1;
        RefreshLayout(realized.Window);

        Expect(second.Bounds.X < first.Bounds.X,
            $"Row should rebuild cached Col order after Order changes; first x={first.Bounds.X}, second x={second.Bounds.X}.",
            failures);
    }

    private static void VerifyRowReactsToChildVisibilityChange(ICollection<string> failures)
    {
        var first = CreateNamedGridCol("first", span: 12);
        var second = CreateNamedGridCol("second", span: 12);
        var row = CreateVerificationGridRow(first, second);

        using var realized = RealizeControl(row);
        RefreshLayout(realized.Window);

        first.IsVisible = false;
        RefreshLayout(realized.Window);

        Expect(Math.Abs(second.Bounds.X) < 0.001,
            $"Row should rebuild visible children after IsVisible changes; second x={second.Bounds.X}.",
            failures);
    }

    private static Row CreateVerificationGridRow(params Col[] children)
    {
        var row = new Row
        {
            Width = 240
        };

        foreach (var child in children)
        {
            row.Children.Add(child);
        }

        return row;
    }

    private static Col CreateNamedGridCol(string name, int span)
    {
        return new Col
        {
            Name    = name,
            Span    = new GridColSpanInfo(span),
            Content = new Border
            {
                Height     = 20,
                Background = Brushes.DodgerBlue
            }
        };
    }
}
