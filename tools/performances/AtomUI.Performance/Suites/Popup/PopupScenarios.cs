using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreatePopupScenarios()
    {
        // Headless 测量主要捕获 Popup 的构造 + 模板/主题 attach 成本，
        // IsOpen=true 需要 OverlayLayer/IPopupImpl,在 headless 环境里跑不起来。
        return
        [
            new PerfScenario("Popup.Empty", _ => CreatePopupHost(child: null)),
            new PerfScenario("Popup.WithTextChild", _ => CreatePopupHost(new AvaloniaTextBlock { Text = "Popup content" })),
            new PerfScenario("Popup.WithComposite", _ => CreatePopupHost(CreateCompositePopupChild())),
            new PerfScenario("Popup.MotionDisabled", _ =>
            {
                var popup = new Popup { IsMotionEnabled = false, Child = new AvaloniaTextBlock { Text = "Static" } };
                return WrapPopup(popup);
            })
        ];
    }

    private static Control CreatePopupHost(Control? child)
    {
        var popup = new Popup
        {
            Child = child ?? new Border { Width = 80, Height = 24 }
        };
        return WrapPopup(popup);
    }

    private static Control WrapPopup(Popup popup)
    {
        var anchor = new Border
        {
            Width  = 60,
            Height = 24
        };
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };
        panel.Children.Add(anchor);
        popup.PlacementTarget = anchor;
        panel.Children.Add(popup);
        return panel;
    }

    private static Control CreateCompositePopupChild()
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 4
        };
        for (var i = 0; i < 5; i++)
        {
            stack.Children.Add(new AvaloniaTextBlock { Text = $"Item {i}" });
        }
        return stack;
    }
}
