using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunSkeletonStateVerification()
    {
        var failures = new List<string>();
        VerifySkeletonContentLogicalChildren(failures);
        VerifySkeletonParagraphLineWidths(failures);
        VerifySkeletonParagraphLineRebuildCleanup(failures);
        VerifyInactiveSkeletonDefersAnimation(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Skeleton state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Skeleton state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifySkeletonContentLogicalChildren(ICollection<string> failures)
    {
        _ = new Skeleton();
        _ = new Skeleton();
        var skeleton = new Skeleton();
        var first    = new StackPanel();
        var second   = new StackPanel();

        skeleton.Content = first;
        Expect(skeleton.GetLogicalChildren().Count() == 1 &&
               ReferenceEquals(skeleton.GetLogicalChildren().Single(), first),
            "Skeleton should add content as a single logical child even after multiple Skeleton instances are created.",
            failures);

        skeleton.Content = second;
        var logicalChildren = skeleton.GetLogicalChildren().ToList();
        Expect(logicalChildren.Count == 1 && ReferenceEquals(logicalChildren[0], second),
            "Skeleton should replace the old logical content child with the new one.",
            failures);

        skeleton.Content = null;
        Expect(!skeleton.GetLogicalChildren().Any(),
            "Skeleton should remove logical content when Content is cleared.",
            failures);
    }

    private static void VerifySkeletonParagraphLineWidths(ICollection<string> failures)
    {
        var paragraph = new SkeletonParagraph
        {
            Rows       = 3,
            LineWidths =
            [
                new Dimension(20, DimensionUnitType.Percentage),
                new Dimension(40, DimensionUnitType.Percentage),
                new Dimension(60, DimensionUnitType.Percentage)
            ]
        };

        using var realized = RealizeControl(paragraph);
        ExpectLineWidths(paragraph, [20, 40, 60], "initial paragraph line widths", failures);

        paragraph.LineWidths =
        [
            new Dimension(30, DimensionUnitType.Percentage),
            new Dimension(50, DimensionUnitType.Percentage),
            new Dimension(70, DimensionUnitType.Percentage)
        ];
        RefreshLayout(realized.Window);
        ExpectLineWidths(paragraph, [30, 50, 70], "updated paragraph line widths", failures);

        paragraph.LineWidths    = [new Dimension(25, DimensionUnitType.Percentage)];
        paragraph.LastLineWidth = new Dimension(65, DimensionUnitType.Percentage);
        RefreshLayout(realized.Window);
        ExpectLineWidths(paragraph, [25, 100, 65], "short paragraph line width list", failures);
    }

    private static void VerifySkeletonParagraphLineRebuildCleanup(ICollection<string> failures)
    {
        var paragraph = new SkeletonParagraph
        {
            Rows     = 2,
            IsActive = false
        };
        using var realized = RealizeControl(paragraph);
        var oldLine = GetSkeletonLines(paragraph).FirstOrDefault();
        Expect(oldLine != null,
            "SkeletonParagraph should create initial SkeletonLine children.",
            failures);

        paragraph.Rows = 3;
        RefreshLayout(realized.Window);
        paragraph.IsActive = true;
        RefreshLayout(realized.Window);

        Expect(oldLine?.GetVisualParent() == null,
            "Removed SkeletonLine should be detached from the paragraph visual tree.",
            failures);
        Expect(oldLine?.IsActive == false,
            "Removed SkeletonLine should no longer follow SkeletonParagraph IsActive changes.",
            failures);
        Expect(oldLine is null ||
               GetPrivateField(oldLine, "AtomUI.Desktop.Controls.AbstractSkeleton", "_followTarget") == null,
            "Removed SkeletonLine should clear its follow target.",
            failures);
    }

    private static void VerifyInactiveSkeletonDefersAnimation(ICollection<string> failures)
    {
        var line = new SkeletonLine();
        using var realized = RealizeControl(line);
        Expect(GetPrivateField(line, "AtomUI.Desktop.Controls.AbstractSkeleton", "_animation") == null,
            "Inactive SkeletonLine should not build an Animation during template application.",
            failures);

        line.IsActive = true;
        RefreshLayout(realized.Window);
        Expect(GetPrivateField(line, "AtomUI.Desktop.Controls.AbstractSkeleton", "_animation") != null,
            "Activating SkeletonLine should lazily create the active Animation.",
            failures);
    }

    private static void ExpectLineWidths(SkeletonParagraph paragraph,
                                         IReadOnlyList<double> expected,
                                         string label,
                                         ICollection<string> failures)
    {
        var lines = GetSkeletonLines(paragraph);
        Expect(lines.Count == expected.Count,
            $"{label}: expected {expected.Count} lines, actual {lines.Count}.",
            failures);

        for (var i = 0; i < Math.Min(lines.Count, expected.Count); i++)
        {
            Expect(lines[i].LineWidth.IsPercentage && Math.Abs(lines[i].LineWidth.Value - expected[i]) < 0.001,
                $"{label}: expected line {i} width {expected[i]}%, actual {lines[i].LineWidth}.",
                failures);
        }
    }

    private static List<SkeletonLine> GetSkeletonLines(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<SkeletonLine>()
                   .Where(line => line.GetType() == typeof(SkeletonLine))
                   .ToList();
    }
}
