using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using AtomButton = AtomUI.Desktop.Controls.Button;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateStatisticScenarios()
    {
        return
        [
            new PerfScenario("Statistic.Basic", _ => CreateBasicStatistic()),
            new PerfScenario("Statistic.Precision", _ => CreatePrecisionStatistic()),
            new PerfScenario("Statistic.Loading", _ => CreateLoadingStatistic()),
            new PerfScenario("Statistic.PrefixSuffix", _ => CreateStatisticWithAddOns()),
            new PerfScenario("Statistic.CountUp", _ => CreateStatisticCountUp()),
            new PerfScenario("TimerStatistic.Default", _ => CreateTimerStatistic()),
            new PerfScenario("TimerStatistic.MillisecondFormat", _ => CreateTimerStatistic("hh\\:mm\\:ss\\.fff")),
            new PerfScenario("Statistic.GalleryShape", _ => CreateStatisticGalleryShape())
        ];
    }

    private static Statistic CreateBasicStatistic()
    {
        return new Statistic
        {
            Header = "Active Users",
            Value  = 112893,
            Width  = 220
        };
    }

    private static Statistic CreatePrecisionStatistic()
    {
        return new Statistic
        {
            Header    = "Account Balance (CNY)",
            Value     = 112893,
            Precision = 2,
            Width     = 220
        };
    }

    private static Statistic CreateLoadingStatistic()
    {
        return new Statistic
        {
            Header    = "Active Users",
            Value     = 112893,
            IsLoading = true,
            Width     = 220
        };
    }

    private static Statistic CreateStatisticWithAddOns()
    {
        return new Statistic
        {
            Header           = "Active",
            Value            = 11.28,
            Precision        = 2,
            ValuePrefixAddOn = new ProbeIcon(),
            ValueSuffixAddOn = "%",
            ContentForeground = Brushes.ForestGreen,
            Width            = 220
        };
    }

    private static Statistic CreateStatisticCountUp()
    {
        return new Statistic
        {
            Header  = "Active Users",
            Value   = 112893,
            Width   = 220,
            Content = new StatisticCountUp
            {
                EndValue  = 112893,
                Precision = 2
            }
        };
    }

    private static TimerStatistic CreateTimerStatistic(string? format = null)
    {
        return new TimerStatistic
        {
            Header = format is null ? "Countdown" : "Million Seconds",
            Value  = DateTime.Now.AddDays(2).AddSeconds(30),
            Format = format,
            Width  = 220
        };
    }

    private static Control CreateStatisticGalleryShape()
    {
        var deadline        = DateTime.Now.AddDays(2).AddSeconds(30);
        var before          = DateTime.Now.AddDays(-2).AddSeconds(-30);
        var tenSecondsLater = DateTime.Now.AddSeconds(10);

        return new StackPanel
        {
            Spacing  = 12,
            Children =
            {
                new UniformGrid
                {
                    Columns = 2,
                    Rows    = 2,
                    Children =
                    {
                        CreateBasicStatistic(),
                        new StackPanel
                        {
                            Spacing   = 16,
                            Children =
                            {
                                CreatePrecisionStatistic(),
                                new AtomButton
                                {
                                    ButtonType = ButtonType.Primary,
                                    Content    = "Recharge"
                                }
                            }
                        },
                        CreateLoadingStatistic()
                    }
                },
                new UniformGrid
                {
                    Columns = 2,
                    Rows    = 1,
                    Children =
                    {
                        new Statistic
                        {
                            Header           = "Feedback",
                            Value            = 1128,
                            ValuePrefixAddOn = new ProbeIcon()
                        },
                        new Statistic
                        {
                            Header           = "Unmerged",
                            Value            = 93,
                            ValueSuffixAddOn = "/ 100"
                        }
                    }
                },
                new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(240, 242, 245)),
                    Padding    = new Avalonia.Thickness(20, 30),
                    Child      = new UniformGrid
                    {
                        Columns       = 2,
                        Rows          = 1,
                        ColumnSpacing = 20,
                        Children =
                        {
                            new Card
                            {
                                SizeType = SizeType.Large,
                                Content  = CreateStatisticWithAddOns()
                            },
                            new Card
                            {
                                SizeType = SizeType.Large,
                                Content  = new Statistic
                                {
                                    Header           = "Idle",
                                    Value            = 9.3,
                                    Precision        = 2,
                                    ValuePrefixAddOn = new ProbeIcon(),
                                    ValueSuffixAddOn = "%",
                                    ContentForeground = Brushes.Firebrick
                                }
                            }
                        }
                    }
                },
                new UniformGrid
                {
                    Columns = 2,
                    Rows    = 2,
                    Children =
                    {
                        CreateStatisticCountUp(),
                        CreateStatisticCountUp()
                    }
                },
                new UniformGrid
                {
                    Columns    = 2,
                    Rows       = 3,
                    RowSpacing = 10,
                    Children =
                    {
                        new TimerStatistic { Value = deadline },
                        new TimerStatistic { Header = "Million Seconds", Value = deadline, Format = "hh\\:mm\\:ss\\.fff" },
                        new TimerStatistic { Header = "Countdown", Value = tenSecondsLater },
                        new TimerStatistic { Header = "Countup", Value = before },
                        new TimerStatistic { Header = "Day Level (Countdown)", Value = deadline, Format = "d\\ \\天\\ h\\ \\时\\ m\\ \\分\\ s\\ \\秒" },
                        new TimerStatistic { Header = "Day Level (Countup)", Value = before, Format = "d\\ \\天\\ h\\ \\时\\ m\\ \\分\\ s\\ \\秒" }
                    }
                },
                new AtomTextBlock
                {
                    Text = "Statistic gallery shape sentinel"
                }
            }
        };
    }
}
