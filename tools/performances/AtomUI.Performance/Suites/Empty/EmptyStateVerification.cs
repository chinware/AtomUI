using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;
using SvgControl = Avalonia.Svg.Svg;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunEmptyStateVerification()
    {
        var failures = new List<string>();
        VerifyEmptyDescriptionVisibility(failures);
        VerifyEmptyImageSourceExclusivity(failures);
        VerifyEmptyImageStateSwitching(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Empty state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Empty state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyEmptyDescriptionVisibility(ICollection<string> failures)
    {
        var empty = new Empty
        {
            PresetImage          = PresetEmptyImage.Default,
            IsDescriptionVisible = false
        };

        using var realized = RealizeControl(empty);
        var description = FindVisualByName<Avalonia.Controls.TextBlock>(empty, "Description");
        Expect(description is null || !description.IsVisible,
            "Empty should not show Description when IsDescriptionVisible is false.",
            failures);

        empty.SetCurrentValue(Empty.IsDescriptionVisibleProperty, true);
        RefreshLayout(realized.Window);
        description = FindVisualByName<Avalonia.Controls.TextBlock>(empty, "Description");
        Expect(description is { IsVisible: true },
            "Empty should show Description after IsDescriptionVisible becomes true.",
            failures);

        empty.SetCurrentValue(Empty.IsDescriptionVisibleProperty, false);
        RefreshLayout(realized.Window);
        description = FindVisualByName<Avalonia.Controls.TextBlock>(empty, "Description");
        Expect(description is null || !description.IsVisible,
            "Empty should hide Description again after IsDescriptionVisible becomes false.",
            failures);
    }

    private static void VerifyEmptyImageSourceExclusivity(ICollection<string> failures)
    {
        var empty = new Empty
        {
            PresetImage = PresetEmptyImage.Simple
        };
        using var realized = RealizeControl(empty);
        try
        {
            empty.SetCurrentValue(Empty.ImagePathProperty, GetEmptySvgPath());
            RefreshLayout(realized.Window);
            failures.Add("Empty should reject runtime ImagePath while PresetImage is set.");
        }
        catch (InvalidOperationException)
        {
        }
    }

    private static void VerifyEmptyImageStateSwitching(ICollection<string> failures)
    {
        var empty = new Empty
        {
            PresetImage = PresetEmptyImage.Simple
        };

        using var realized = RealizeControl(empty);
        var firstSvg = FindVisualByName<SvgControl>(empty, "PART_SvgImage");
        Expect(firstSvg != null,
            "Empty should create Svg presenter when PresetImage is set.",
            failures);
        Expect(firstSvg is { Source: not null, Path: null },
            "Empty built-in preset should use Source and clear Path.",
            failures);

        empty.SetCurrentValue(Empty.PresetImageProperty, null);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstSvg, FindVisualByName<SvgControl>(empty, "PART_SvgImage")),
            "Empty should keep the template Svg presenter when image source is cleared.",
            failures);
        Expect(firstSvg is { Source: null, Path: null },
            "Empty should clear Source and Path when image source is cleared.",
            failures);

        empty.SetCurrentValue(Empty.ImageSourceProperty, EmptyInlineSvgSource);
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstSvg, FindVisualByName<SvgControl>(empty, "PART_SvgImage")),
            "Empty should reuse the template Svg presenter when switching to ImageSource.",
            failures);
        Expect(firstSvg is { Source: EmptyInlineSvgSource, Path: null },
            "Empty ImageSource should use Source and clear Path.",
            failures);

        empty.SetCurrentValue(Empty.ImageSourceProperty, null);
        empty.SetCurrentValue(Empty.ImagePathProperty, GetEmptySvgPath());
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstSvg, FindVisualByName<SvgControl>(empty, "PART_SvgImage")),
            "Empty should reuse the template Svg presenter when switching to ImagePath.",
            failures);
        Expect(firstSvg is { Source: null } && firstSvg.Path == GetEmptySvgPath(),
            "Empty ImagePath should use Path and clear Source.",
            failures);
    }
}
