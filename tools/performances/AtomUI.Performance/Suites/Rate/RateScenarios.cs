using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomRate = AtomUI.Desktop.Controls.Rate;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateRateScenarios()
    {
        return
        [
            new PerfScenario("Rate.Default.Empty", _ => new AtomRate()),
            new PerfScenario("Rate.Default.Value3", _ => new AtomRate
            {
                DefaultValue = 3
            }),
            new PerfScenario("Rate.Half.Value3_5", _ => new AtomRate
            {
                DefaultValue = 3.5,
                IsAllowHalf = true
            }),
            new PerfScenario("Rate.Disabled.Value2", _ => new AtomRate
            {
                DefaultValue = 2,
                IsEnabled    = false
            }),
            new PerfScenario("Rate.NoMotion", _ => new AtomRate
            {
                IsMotionEnabled = false
            }),
            new PerfScenario("Rate.CustomIcon.Heart", _ => new AtomRate
            {
                Character    = new HeartOutlined(),
                IsAllowHalf  = true,
                IsAllowClear = true
            }),
            new PerfScenario("Rate.CustomText.Ascii", _ => new AtomRate
            {
                Character    = "A",
                IsAllowHalf  = true,
                IsAllowClear = true
            }),
            new PerfScenario("Rate.CustomText.Cjk", _ => new AtomRate
            {
                Character    = "秦",
                IsAllowHalf  = true,
                IsAllowClear = true
            }),
            new PerfScenario("Rate.ToolTips5", _ => new AtomRate
            {
                ToolTips = ["Terrible", "Bad", "Normal", "Good", "Wonderful"]
            }),
            new PerfScenario("Rate.Batch20.Default", _ => CreateRateBatch(20)),
            new PerfScenario("Rate.GalleryShape", _ => CreateRateShowCaseShape())
        ];
    }

    private static Control CreateRateBatch(int count)
    {
        var panel = new StackPanel
        {
            Spacing = 10
        };

        for (var i = 0; i < count; i++)
        {
            panel.Children.Add(new AtomRate
            {
                DefaultValue = i % 6,
                IsAllowHalf  = i % 2 == 0
            });
        }

        return panel;
    }

    private static Control CreateRateShowCaseShape()
    {
        var tooltips = new[] { "terrible", "bad", "normal", "good", "wonderful" };
        var activeTooltipText = new TextBlock();
        var toolTipRate = new AtomRate
        {
            ToolTips = tooltips
        };
        toolTipRate.ValueChanged += (_, e) =>
        {
            var index = (int)Math.Round(e.NewValue, MidpointRounding.AwayFromZero) - 1;
            activeTooltipText.Text = index >= 0 && index < tooltips.Length ? tooltips[index] : null;
        };

        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new AtomRate(),
                new AtomRate
                {
                    DefaultValue = 3.5,
                    IsAllowHalf  = true
                },
                new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Spacing     = 10,
                    Children =
                    {
                        toolTipRate,
                        activeTooltipText
                    }
                },
                new AtomRate
                {
                    DefaultValue = 2,
                    IsEnabled    = false
                },
                new StackPanel
                {
                    Spacing = 20,
                    Children =
                    {
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing     = 10,
                            Children =
                            {
                                new AtomRate
                                {
                                    DefaultValue = 3,
                                    IsAllowClear = true
                                },
                                new TextBlock { Text = "IsAllowClear: true" }
                            }
                        },
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Spacing     = 10,
                            Children =
                            {
                                new AtomRate
                                {
                                    DefaultValue = 3,
                                    IsAllowClear = false
                                },
                                new TextBlock { Text = "IsAllowClear: false" }
                            }
                        }
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Spacing     = 10,
                    Children =
                    {
                        new AtomRate
                        {
                            Character    = new HeartOutlined(),
                            IsAllowHalf  = true,
                            IsAllowClear = true
                        },
                        new AtomRate
                        {
                            Character    = "A",
                            IsAllowHalf  = true,
                            IsAllowClear = true
                        },
                        new AtomRate
                        {
                            Character    = "秦",
                            IsAllowHalf  = true,
                            IsAllowClear = true
                        }
                    }
                }
            }
        };
    }
}
