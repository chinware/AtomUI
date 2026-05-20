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
        VerifySingleSliderAvoidsEndThumb(failures);
        VerifyRangeEndThumbLifecycle(failures);
        VerifyRangeEndThumbTooltipConfiguration(failures);
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

    private static void VerifySingleSliderAvoidsEndThumb(ICollection<string> failures)
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
        Expect(track?.StartSliderThumb is not null,
            "Single Slider should create PART_StartThumb.",
            failures);
        Expect(track?.EndSliderThumb is null,
            "Single Slider should not create PART_EndThumb before range mode is enabled.",
            failures);
        Expect(CountSliderThumbs(slider, "PART_EndThumb") == 0,
            "Single Slider visual tree should not contain PART_EndThumb.",
            failures);
    }

    private static void VerifyRangeEndThumbLifecycle(ICollection<string> failures)
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

        slider.IsRangeMode = true;
        RefreshLayout(realized.Window);
        var firstEndThumb = track.EndSliderThumb;
        Expect(firstEndThumb is not null,
            "Enabling range mode should materialize PART_EndThumb.",
            failures);
        Expect(firstEndThumb?.Name == "PART_EndThumb",
            "Materialized range thumb should keep the PART_EndThumb name.",
            failures);
        Expect(firstEndThumb?.GetVisualParent() == track,
            "Materialized range thumb should be a visual child of SliderTrack.",
            failures);
        Expect(firstEndThumb?.TemplatedParent == slider,
            "Materialized range thumb should keep Slider as templated parent so Slider template selectors still apply.",
            failures);
        Expect(CountSliderThumbs(slider, "PART_EndThumb") == 1,
            "Enabling range mode should create exactly one PART_EndThumb.",
            failures);

        slider.IsRangeMode = false;
        RefreshLayout(realized.Window);
        Expect(track.EndSliderThumb is null,
            "Disabling range mode should release PART_EndThumb from SliderTrack.",
            failures);
        Expect(firstEndThumb?.GetVisualParent() == null,
            "Released PART_EndThumb should not keep a visual parent.",
            failures);
        Expect(firstEndThumb?.TemplatedParent == null,
            "Released PART_EndThumb should clear TemplatedParent.",
            failures);
        Expect(CountSliderThumbs(slider, "PART_EndThumb") == 0,
            "Disabling range mode should remove PART_EndThumb from the visual tree.",
            failures);

        slider.IsRangeMode = true;
        RefreshLayout(realized.Window);
        Expect(track.EndSliderThumb is not null,
            "Re-enabling range mode should materialize PART_EndThumb again.",
            failures);
        Expect(CountSliderThumbs(slider, "PART_EndThumb") == 1,
            "Re-enabling range mode should still keep exactly one PART_EndThumb.",
            failures);
    }

    private static void VerifyRangeEndThumbTooltipConfiguration(ICollection<string> failures)
    {
        var slider = new AtomSlider
        {
            Minimum       = 0,
            Maximum       = 100,
            IsRangeMode   = true,
            Orientation   = Orientation.Vertical,
            RangeValue    = new SliderRangeValue { StartValue = 20, EndValue = 80 },
            ValueFormatTemplate = "{0:0}%"
        };

        using var realized = RealizeControl(slider);
        RefreshLayout(realized.Window);

        var track = FindVisualByName<SliderTrack>(slider, "PART_Track");
        var endThumb = track?.EndSliderThumb;
        Expect(endThumb is not null,
            "Initial range Slider should materialize PART_EndThumb.",
            failures);
        if (endThumb is null)
        {
            return;
        }

        Expect(AtomToolTip.GetShowDelay(endThumb) == 20,
            "Materialized PART_EndThumb should keep Slider tooltip show delay.",
            failures);
        Expect(AtomToolTip.GetPlacement(endThumb) == PlacementMode.Right,
            "Vertical range Slider should place PART_EndThumb tooltip on the right.",
            failures);
        Expect(Equals(AtomToolTip.GetTip(endThumb), "80%"),
            $"PART_EndThumb should get formatted range value tooltip. Actual: {AtomToolTip.GetTip(endThumb)}.",
            failures);
        Expect(AtomToolTip.GetTipHostWidth(endThumb) > 0,
            "PART_EndThumb should get a calculated tooltip host width.",
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

    private static int CountSliderThumbs(Control root, string name)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<SliderThumb>()
                   .Count(thumb => thumb.Name == name);
    }
}
