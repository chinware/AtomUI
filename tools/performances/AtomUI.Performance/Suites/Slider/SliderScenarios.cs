using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomSlider = AtomUI.Desktop.Controls.Slider;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSliderScenarios()
    {
        return
        [
            new PerfScenario("Slider.Default", _ => CreateSlider()),
            new PerfScenario("Slider.Range", _ => CreateSlider(isRangeMode: true)),
            new PerfScenario("Slider.Vertical.Default", _ => CreateSlider(orientation: Orientation.Vertical)),
            new PerfScenario("Slider.Vertical.Range", _ => CreateSlider(isRangeMode: true, orientation: Orientation.Vertical)),
            new PerfScenario("Slider.WithMarks", _ => CreateSlider(marks: CreateSliderMarks())),
            new PerfScenario("Slider.RangeWithMarks", _ => CreateSlider(isRangeMode: true, marks: CreateSliderMarks())),
            new PerfScenario("Slider.Disabled", _ => CreateSlider(isEnabled: false)),
            new PerfScenario("Slider.GalleryShape.SliderShowCase", _ => CreateSliderShowCaseShape())
        ];
    }

    private static AtomSlider CreateSlider(bool isRangeMode = false,
                                           Orientation orientation = Orientation.Horizontal,
                                           List<SliderMark>? marks = null,
                                           bool isEnabled = true,
                                           bool isIncluded = true)
    {
        var slider = new AtomSlider
        {
            Minimum       = 0,
            Maximum       = 100,
            TickFrequency = isRangeMode ? 5 : 1,
            IsRangeMode   = isRangeMode,
            Orientation   = orientation,
            Marks         = marks,
            IsEnabled     = isEnabled,
            IsIncluded    = isIncluded,
            Value         = 20,
            RangeValue    = new SliderRangeValue
            {
                StartValue = 20,
                EndValue   = 80
            }
        };

        if (orientation == Orientation.Vertical)
        {
            slider.Height = 300;
        }

        return slider;
    }

    private static Control CreateSliderShowCaseShape()
    {
        var sharedMarks = CreateSliderMarks();
        return new StackPanel
        {
            Spacing = 20,
            Children =
            {
                CreateSlider(),
                CreateSlider(isRangeMode: true),
                CreateSlider(isRangeMode: true, marks: sharedMarks),
                CreateSlider(),
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing     = 20,
                    Height      = 300,
                    Children =
                    {
                        CreateSlider(orientation: Orientation.Vertical),
                        CreateSlider(isRangeMode: true, orientation: Orientation.Vertical),
                        CreateSlider(orientation: Orientation.Vertical, marks: sharedMarks),
                        CreateSlider(isRangeMode: true, orientation: Orientation.Vertical, marks: sharedMarks)
                    }
                },
                CreateSlider(marks: sharedMarks),
                CreateSlider(isRangeMode: true, marks: sharedMarks),
                CreateSlider(marks: sharedMarks, isIncluded: false),
                CreateSlider(isRangeMode: true, marks: sharedMarks, isIncluded: false)
            }
        };
    }

    private static List<SliderMark> CreateSliderMarks()
    {
        return
        [
            new SliderMark("0°C", 0),
            new SliderMark("26°C", 26),
            new SliderMark("37°C", 37),
            new SliderMark("100°C", 100)
            {
                LabelFontWeight = FontWeight.Bold,
                LabelBrush      = new SolidColorBrush(Colors.Red)
            }
        ];
    }
}
