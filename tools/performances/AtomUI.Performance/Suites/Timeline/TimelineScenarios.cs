using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Media;
using AtomButton = AtomUI.Desktop.Controls.Button;
using AtomRadioButton = AtomUI.Desktop.Controls.RadioButton;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateTimelineScenarios()
    {
        return
        [
            new PerfScenario("Timeline.Basic.Items3", _ => CreateBasicTimeline()),
            new PerfScenario("Timeline.Color.Items5", _ => CreateColorTimeline()),
            new PerfScenario("Timeline.Pending.Items3", _ => CreatePendingTimeline()),
            new PerfScenario("Timeline.Alternate.Items4", _ => CreateModeTimeline(TimelineMode.Alternate)),
            new PerfScenario("Timeline.Label.Items4", _ => CreateLabelTimeline()),
            new PerfScenario("Timeline.Right.Items4", _ => CreateModeTimeline(TimelineMode.Right)),
            new PerfScenario("Timeline.Icon.Items4", _ => CreateIconTimeline()),
            new PerfScenario("Timeline.GalleryShape", _ => CreateTimelineGalleryShape())
        ];
    }

    private static Timeline CreateBasicTimeline()
    {
        var timeline = new Timeline
        {
            Width = 760
        };
        timeline.Items.Add(new TimelineItem { Content = "2024-01-01 AtomUI Officially Initiated" });
        timeline.Items.Add(new TimelineItem
        {
            IndicatorColor = Brushes.Green,
            Content        = "2024-08-12 AtomUI is officially open-source."
        });
        timeline.Items.Add(new TimelineItem
        {
            IndicatorColor = Brushes.Red,
            Content        = "2024-10-01 Release of the 0.0.1 Preview Version"
        });
        return timeline;
    }

    private static Timeline CreateColorTimeline()
    {
        var timeline = new Timeline
        {
            Width = 760
        };
        timeline.Items.Add(new TimelineItem { IndicatorColor = Brushes.Green, Content = "Green item" });
        timeline.Items.Add(new TimelineItem { IndicatorColor = Brushes.Blue, Content = "Blue item" });
        timeline.Items.Add(new TimelineItem { IndicatorColor = Brushes.Red, Content = "Red item" });
        timeline.Items.Add(new TimelineItem { IndicatorColor = Brushes.Gray, Content = "Gray item" });
        timeline.Items.Add(new TimelineItem { IndicatorColor = new SolidColorBrush(Color.FromRgb(0, 204, 255)), Content = "Custom item" });
        return timeline;
    }

    private static Timeline CreatePendingTimeline()
    {
        var timeline = new Timeline
        {
            Width     = 760,
            Pending   = "Recording...",
            IsReverse = false
        };
        timeline.Items.Add(new TimelineItem { Label = "2024-01-01", Content = "AtomUI Officially Initiated. 1" });
        timeline.Items.Add(new TimelineItem { Label = "2024-08-12", Content = "AtomUI Officially Initiated. 2" });
        timeline.Items.Add(new TimelineItem { Label = "2024-10-01", Content = "AtomUI Officially Initiated. 3" });
        return timeline;
    }

    private static Timeline CreateModeTimeline(TimelineMode mode)
    {
        var timeline = new Timeline
        {
            Width = 760,
            Mode  = mode
        };
        for (var i = 0; i < 4; i++)
        {
            timeline.Items.Add(new TimelineItem
            {
                Label   = i == 0 || i == 3 ? "2024-01-01" : null,
                Content = "2024-01-01 AtomUI Officially Initiated"
            });
        }

        return timeline;
    }

    private static Timeline CreateLabelTimeline()
    {
        var timeline = new Timeline
        {
            Width = 760,
            Mode  = TimelineMode.Left
        };
        timeline.Items.Add(new TimelineItem { Label = "2024-01-01", Content = "AtomUI Officially Initiated" });
        timeline.Items.Add(new TimelineItem { Label = "2015-09-01 09:12:11", Content = "Create a services site" });
        timeline.Items.Add(new TimelineItem { Content = "Qinware website online" });
        timeline.Items.Add(new TimelineItem { Label = "2029-09-01", Content = "Network problems being solved" });
        return timeline;
    }

    private static Timeline CreateIconTimeline()
    {
        var timeline = new Timeline
        {
            Width = 760,
            Mode  = TimelineMode.Alternate
        };
        timeline.Items.Add(new TimelineItem { Content = "2024-01-01 AtomUI Officially Initiated" });
        timeline.Items.Add(new TimelineItem { Content = "2024-01-01 AtomUI Officially Initiated" });
        timeline.Items.Add(new TimelineItem { Content = "2024-01-01 AtomUI Officially Initiated" });
        timeline.Items.Add(new TimelineItem
        {
            IndicatorIcon  = new ClockCircleOutlined(),
            IndicatorColor = Brushes.Red,
            Label          = "2024-01-01",
            Content        = "2024-01-01 AtomUI Officially Initiated"
        });
        return timeline;
    }

    private static Control CreateTimelineGalleryShape()
    {
        return new StackPanel
        {
            Spacing  = 12,
            Children =
            {
                CreateBasicTimeline(),
                CreateColorTimeline(),
                new StackPanel
                {
                    Children =
                    {
                        CreatePendingTimeline(),
                        new DockPanel
                        {
                            Children =
                            {
                                new AtomButton
                                {
                                    ButtonType = ButtonType.Primary,
                                    Content    = "Toggle Reverse"
                                }
                            }
                        }
                    }
                },
                CreateIconTimeline(),
                new StackPanel
                {
                    Children =
                    {
                        new WrapPanel
                        {
                            Children =
                            {
                                new AtomRadioButton { Content = "Left", IsChecked = true },
                                new AtomRadioButton { Content = "Right" },
                                new AtomRadioButton { Content = "Alternate" }
                            }
                        },
                        CreateLabelTimeline()
                    }
                },
                CreateModeTimeline(TimelineMode.Right)
            }
        };
    }
}
