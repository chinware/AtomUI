using AtomUI.Controls.Commons;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateBadgeScenarios()
    {
        return
        [
            new PerfScenario("Badge.Count.Target", _ => CreateCountBadge(5, true)),
            new PerfScenario("Badge.Count.ZeroHiddenTarget", _ => CreateCountBadge(0, true)),
            new PerfScenario("Badge.Count.StandaloneZeroHidden", _ => new CountBadge { Count = 0 }),
            new PerfScenario("Badge.Count.StandaloneVisible", _ => new CountBadge { Count = 5 }),
            new PerfScenario("Badge.Dot.TargetNoText", _ => CreateDotBadge(target: true, text: null)),
            new PerfScenario("Badge.Dot.StandaloneStatus", _ => new DotBadge { Status = DotBadgeStatus.Success }),
            new PerfScenario("Badge.Dot.StandaloneText", _ => new DotBadge { Status = DotBadgeStatus.Success, Text = "Success" }),
            new PerfScenario("Badge.Ribbon.Target", _ => CreateRibbonBadge("Hippies", "purple")),
            new PerfScenario("Badge.GalleryShape", _ => CreateBadgeGalleryShape())
        ];
    }

    private static CountBadge CreateCountBadge(int count, bool target)
    {
        var badge = new CountBadge
        {
            Count = count
        };
        if (target)
        {
            badge.DecoratedTarget = CreateBadgeTarget();
        }

        return badge;
    }

    private static DotBadge CreateDotBadge(bool target, string? text)
    {
        var badge = new DotBadge
        {
            Text = text
        };
        if (target)
        {
            badge.DecoratedTarget = CreateBadgeTarget();
        }

        return badge;
    }

    private static RibbonBadge CreateRibbonBadge(string text, string? ribbonColor = null)
    {
        return new RibbonBadge
        {
            Text            = text,
            RibbonColor     = ribbonColor,
            DecoratedTarget = CreateRibbonTarget()
        };
    }

    private static Control CreateBadgeGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 20
        };

        root.Children.Add(CreateBadgeRow(
            CreateCountBadge(5, true),
            new CountBadge { Count = 0, IsZeroVisible = true, DecoratedTarget = CreateBadgeTarget() }));

        root.Children.Add(CreateBadgeRow(
            CreateCountBadge(99, true),
            CreateCountBadge(100, true),
            new CountBadge { Count = 99, OverflowCount = 10, DecoratedTarget = CreateBadgeTarget() },
            new CountBadge { Count = 1000, OverflowCount = 999, DecoratedTarget = CreateBadgeTarget() }));

        root.Children.Add(CreateBadgeRow(new CountBadge
        {
            Count           = 5,
            Offset          = new Avalonia.Point(10, 10),
            DecoratedTarget = CreateBadgeTarget()
        }));

        root.Children.Add(CreateBadgeRow(
            CreateCountBadge(5, true),
            new CountBadge { Count = 5, Size = CountBadgeSize.Small, DecoratedTarget = CreateBadgeTarget() }));

        root.Children.Add(CreateBadgeRow(
            new AtomUI.Desktop.Controls.ToggleSwitch(),
            new CountBadge { BadgeColor = "#faad14", Count = 11, IsZeroVisible = true },
            new CountBadge { Count = 25 },
            new CountBadge { BadgeColor = "#52c41a", Count = 109 }));

        root.Children.Add(CreateBadgeRow(
            new CountBadge { Count = 5, OverflowCount = 99, DecoratedTarget = CreateBadgeTarget() },
            new AtomUI.Desktop.Controls.Button { Content = "Add", SizeType = SizeType.Small },
            new AtomUI.Desktop.Controls.Button { Content = "Sub", SizeType = SizeType.Small },
            new AtomUI.Desktop.Controls.Button { Content = "Random", SizeType = SizeType.Small }));

        root.Children.Add(CreateBadgeRow(
            new CountBadge { Count = 9, DecoratedTarget = CreateBadgeTarget() },
            CreateDotBadge(target: true, text: null),
            new AtomUI.Desktop.Controls.ToggleSwitch()));

        root.Children.Add(CreateBadgeRow(
            new DotBadge
            {
                Offset = new Avalonia.Point(-7, 8),
                DecoratedTarget = new AtomUI.Desktop.Controls.Button
                {
                    ButtonType = ButtonType.Link,
                    Icon       = new NotificationOutlined()
                }
            },
            new DotBadge
            {
                Offset = new Avalonia.Point(-14, 12),
                DecoratedTarget = new AtomUI.Desktop.Controls.Button
                {
                    ButtonType = ButtonType.Link,
                    Content    = "Link something"
                }
            },
            new AtomUI.Desktop.Controls.Button
            {
                ButtonType = ButtonType.Link,
                Content    = "Link something"
            }));

        root.Children.Add(CreateBadgeRow(
            new DotBadge { Status = DotBadgeStatus.Success },
            new DotBadge { Status = DotBadgeStatus.Error },
            new DotBadge { Status = DotBadgeStatus.Default },
            new DotBadge { Status = DotBadgeStatus.Processing },
            new DotBadge { Status = DotBadgeStatus.Warning }));

        root.Children.Add(new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new DotBadge { Status = DotBadgeStatus.Success, Text = "Success" },
                new DotBadge { Status = DotBadgeStatus.Error, Text = "Error" },
                new DotBadge { Status = DotBadgeStatus.Default, Text = "Default" },
                new DotBadge { Status = DotBadgeStatus.Processing, Text = "Processing" },
                new DotBadge { Status = DotBadgeStatus.Warning, Text = "Warning" }
            }
        });

        var ribbonPanel = new StackPanel
        {
            Spacing = 20
        };
        ribbonPanel.Children.Add(CreateRibbonBadge("精益求精，打造体验优秀的 UISDK"));
        ribbonPanel.Children.Add(CreateRibbonBadge("甲辰计划雄起", "Pink"));
        ribbonPanel.Children.Add(CreateRibbonBadge("Avalonia 非常优秀", "Cyan"));
        ribbonPanel.Children.Add(CreateRibbonBadge("Hippies", "Green"));
        ribbonPanel.Children.Add(CreateRibbonBadge("Hippies", "purple", RibbonBadgePlacement.Start));
        ribbonPanel.Children.Add(CreateRibbonBadge("Hippies", "volcano", RibbonBadgePlacement.Start));
        ribbonPanel.Children.Add(CreateRibbonBadge("Hippies", "magenta", RibbonBadgePlacement.Start));
        root.Children.Add(ribbonPanel);

        var colors = new[]
        {
            "Pink", "Red", "Yellow", "Orange", "Cyan", "Green", "Blue",
            "Purple", "GeekBlue", "Magenta", "Volcano", "Gold", "Lime",
            "#f50", "rgb(45, 183, 245)", "hsl(102, 53%, 61%)", "rgb(15, 141, 230)"
        };
        var colorPanel = new StackPanel
        {
            Spacing = 10
        };
        foreach (var color in colors)
        {
            colorPanel.Children.Add(new DotBadge { DotColor = color, Text = color });
        }
        root.Children.Add(colorPanel);

        return root;
    }

    private static StackPanel CreateBadgeRow(params Control[] controls)
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 20
        };
        foreach (var control in controls)
        {
            row.Children.Add(control);
        }

        return row;
    }

    private static Border CreateBadgeTarget()
    {
        return new Border
        {
            Width        = 40,
            Height       = 40,
            Background   = Avalonia.Media.Brush.Parse("rgb(191,191,191)"),
            CornerRadius = new Avalonia.CornerRadius(8)
        };
    }

    private static Border CreateRibbonTarget()
    {
        return new Border
        {
            Padding         = new Avalonia.Thickness(10, 0),
            BorderBrush     = Avalonia.Media.Brush.Parse("#d9d9d9"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius    = new Avalonia.CornerRadius(6),
            Child = new StackPanel
            {
                Children =
                {
                    new Avalonia.Controls.TextBlock
                    {
                        Height     = 38,
                        FontWeight = FontWeight.Bold,
                        LineHeight = 38,
                        Text       = "Pushes open the window"
                    },
                    new Avalonia.Controls.TextBlock
                    {
                        Margin = new Avalonia.Thickness(0, 10, 0, 0),
                        Text   = "and raises the spyglass."
                    }
                }
            }
        };
    }

    private static RibbonBadge CreateRibbonBadge(string text,
                                                 string? ribbonColor,
                                                 RibbonBadgePlacement placement)
    {
        var badge = CreateRibbonBadge(text, ribbonColor);
        badge.Placement = placement;
        return badge;
    }
}
