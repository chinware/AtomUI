using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using AtomSlider = AtomUI.Desktop.Controls.Slider;
using AtomToolTip = AtomUI.Desktop.Controls.ToolTip;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunSliderStateVerification()
    {
        var failures = new List<string>();
        VerifySingleSliderCreatesOneThumb(failures);
        VerifyRangeThumbLifecycle(failures);
        VerifyRangeThumbTooltipConfiguration(failures);
        VerifySliderSharedMarksDoNotMutateModels(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Slider state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Slider state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifySingleSliderCreatesOneThumb(ICollection<string> failures)
    {
        var slider = new AtomSlider
        {
            Minimum = 0,
            Maximum = 100,
            Value   = 50
        };

        using var realized = RealizeControl(slider);
        RefreshLayout(realized.Window);

        var track = FindVisualByName<SliderTrack>(slider, "PART_Track");
        Expect(track is not null,
            "Slider should create PART_Track.",
            failures);
        var thumbs = GetSliderThumbs(slider);
        Expect(thumbs.Count == 1,
            "Single Slider should materialize exactly one dynamic SliderThumb.",
            failures);
        Expect(thumbs.FirstOrDefault()?.GetVisualParent() == track,
            "Single Slider thumb should be a visual child of SliderTrack.",
            failures);
        Expect(thumbs.FirstOrDefault()?.TemplatedParent == slider,
            "Single Slider thumb should keep Slider as templated parent.",
            failures);
    }

    private static void VerifyRangeThumbLifecycle(ICollection<string> failures)
    {
        var slider = new AtomSlider
        {
            Minimum = 0,
            Maximum = 100,
            Value   = 50
        };

        using var realized = RealizeControl(slider);
        RefreshLayout(realized.Window);

        var track = FindVisualByName<SliderTrack>(slider, "PART_Track");
        if (track is null)
        {
            failures.Add("Slider range lifecycle verification could not find PART_Track.");
            return;
        }

        var singleThumbs = GetSliderThumbs(slider);
        Expect(singleThumbs.Count == 1,
            "Slider should start with exactly one dynamic thumb before range mode is enabled.",
            failures);
        var singleThumb = singleThumbs.FirstOrDefault();
        slider.IsRangeMode = true;
        RefreshLayout(realized.Window);
        var rangeThumbs = GetSliderThumbs(slider);
        Expect(rangeThumbs.Count == 2,
            "Enabling range mode should materialize two dynamic SliderThumbs.",
            failures);
        Expect(ReferenceEquals(rangeThumbs.FirstOrDefault(), singleThumb),
            "Enabling range mode should preserve the existing first thumb.",
            failures);
        var addedThumb = rangeThumbs.ElementAtOrDefault(1);
        Expect(addedThumb?.GetVisualParent() == track,
            "The added range thumb should be a visual child of SliderTrack.",
            failures);
        Expect(addedThumb?.TemplatedParent == slider,
            "The added range thumb should keep Slider as templated parent so Slider template selectors apply.",
            failures);

        slider.IsRangeMode = false;
        RefreshLayout(realized.Window);
        var collapsedThumbs = GetSliderThumbs(slider);
        Expect(collapsedThumbs.Count == 1,
            "Disabling range mode should remove the additional dynamic thumb.",
            failures);
        Expect(ReferenceEquals(collapsedThumbs.FirstOrDefault(), singleThumb),
            "Disabling range mode should preserve the first thumb.",
            failures);
        Expect(addedThumb?.GetVisualParent() is null,
            "A removed range thumb should leave the SliderTrack visual tree.",
            failures);
        Expect(addedThumb?.TemplatedParent is null,
            "A removed range thumb should clear its templated parent.",
            failures);

        slider.IsRangeMode = true;
        RefreshLayout(realized.Window);
        var restoredThumbs = GetSliderThumbs(slider);
        Expect(restoredThumbs.Count == 2,
            "Re-enabling range mode should materialize a second dynamic thumb again.",
            failures);
        Expect(!ReferenceEquals(restoredThumbs.ElementAtOrDefault(1), addedThumb),
            "Re-enabling range mode should create a fresh second thumb after the old one was removed.",
            failures);
    }

    private static void VerifyRangeThumbTooltipConfiguration(ICollection<string> failures)
    {
        var slider = new AtomSlider
        {
            Minimum       = 0,
            Maximum       = 100,
            IsRangeMode   = true,
            Orientation   = Orientation.Vertical,
            RangeValues   = [20, 80],
            ValueFormatTemplate = "{0:0}%"
        };

        using var realized = RealizeControl(slider);
        RefreshLayout(realized.Window);

        var endThumb = GetSliderThumbs(slider).ElementAtOrDefault(1);
        Expect(endThumb is not null,
            "Initial range Slider should materialize its second dynamic thumb.",
            failures);
        if (endThumb is null)
        {
            return;
        }

        Expect(AtomToolTip.GetShowDelay(endThumb) == 20,
            "Materialized range thumb should keep Slider tooltip show delay.",
            failures);
        Expect(AtomToolTip.GetPlacement(endThumb) == PlacementMode.Right,
            "Vertical range Slider should place its second thumb tooltip on the right.",
            failures);
        Expect(Equals(AtomToolTip.GetTip(endThumb), "80%"),
            $"Second range thumb should get formatted range value tooltip. Actual: {AtomToolTip.GetTip(endThumb)}.",
            failures);
        Expect(AtomToolTip.GetTipHostWidth(endThumb) > 0,
            "Second range thumb should get a calculated tooltip host width.",
            failures);
    }

    private static void VerifySliderSharedMarksDoNotMutateModels(ICollection<string> failures)
    {
        var sharedMarks = CreateSliderMarks();
        var snapshots = sharedMarks.Select(SliderMarkSnapshot.From).ToList();
        var root = new StackPanel
        {
            Children =
            {
                CreateSlider(marks: sharedMarks),
                CreateSlider(isRangeMode: true, orientation: Orientation.Vertical, marks: sharedMarks)
            }
        };

        using var realized = RealizeControl(root);
        RefreshLayout(realized.Window);

        for (var i = 0; i < sharedMarks.Count; i++)
        {
            Expect(snapshots[i].Matches(sharedMarks[i]),
                "SliderMark public state should not be mutated by per-track mark label cache.",
                failures);
        }
    }

    private readonly record struct SliderMarkSnapshot(string Label,
                                                      double Value,
                                                      IBrush? LabelBrush,
                                                      FontStyle LabelFontStyle,
                                                      FontWeight LabelFontWeight)
    {
        public static SliderMarkSnapshot From(SliderMark mark)
        {
            return new SliderMarkSnapshot(mark.Label,
                mark.Value,
                mark.LabelBrush,
                mark.LabelFontStyle,
                mark.LabelFontWeight);
        }

        public bool Matches(SliderMark mark)
        {
            return Label == mark.Label &&
                   Value.Equals(mark.Value) &&
                   ReferenceEquals(LabelBrush, mark.LabelBrush) &&
                   LabelFontStyle == mark.LabelFontStyle &&
                   LabelFontWeight == mark.LabelFontWeight;
        }
    }

    private static IReadOnlyList<SliderThumb> GetSliderThumbs(Control root)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<SliderThumb>()
                   .ToList();
    }
}
