using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.VisualTree;
using AtomSpin = AtomUI.Desktop.Controls.Spin;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunSpinStateVerification()
    {
        var failures = new List<string>();
        VerifySpinLazyMaskLifecycle(failures);
        VerifySpinIndicatorAnimationLifecycle(failures);
        VerifySpinCustomIndicatorSizeSync(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Spin state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Spin state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifySpinLazyMaskLifecycle(ICollection<string> failures)
    {
        var spin = new AtomSpin
        {
            Width        = 220,
            Height       = 120,
            IsSpinning   = false,
            IsTipVisible = true,
            Tip          = "Loading...",
            Content      = new Border
            {
                Width  = 220,
                Height = 120
            }
        };

        using var realized = RealizeControl(spin);
        Expect(FindVisualByName<Panel>(spin, "MaskLayout") == null,
            "Non-spinning Spin should not create MaskLayout.",
            failures);
        Expect(CountSpinIndicators(spin) == 0,
            "Non-spinning Spin should not create SpinIndicator.",
            failures);

        spin.IsSpinning = true;
        RefreshLayout(realized.Window);

        var indicator = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(FindVisualByName<Panel>(spin, "MaskLayout") != null,
            "Spinning Spin should create MaskLayout.",
            failures);
        Expect(indicator != null,
            "Spinning Spin should create SpinIndicator.",
            failures);
        Expect(FindVisualByName<AtomTextBlock>(spin, "Tip") != null,
            "Spinning Spin should create Tip TextBlock.",
            failures);

        var updatedIcon = new LoadingOutlined();
        spin.SizeType        = SizeType.Large;
        spin.Tip             = "Still loading...";
        spin.IsTipVisible    = false;
        spin.CustomIndicator = updatedIcon;
        RefreshLayout(realized.Window);

        Expect(indicator?.SizeType == SizeType.Large,
            "Lazy SpinIndicator should sync SizeType changes from Spin.",
            failures);
        Expect(ReferenceEquals(indicator?.CustomIndicator, updatedIcon),
            "Lazy SpinIndicator should sync CustomIndicator changes from Spin.",
            failures);
        var tip = FindVisualByName<AtomTextBlock>(spin, "Tip");
        Expect(tip?.Text == "Still loading...",
            "Lazy Spin tip should sync Tip text changes from Spin.",
            failures);
        Expect(tip?.IsVisible == false,
            "Lazy Spin tip should sync IsTipVisible changes from Spin.",
            failures);

        spin.IsSpinning = false;
        RefreshLayout(realized.Window);

        Expect(FindVisualByName<Panel>(spin, "MaskLayout") == null,
            "Spin should remove MaskLayout when spinning stops.",
            failures);
        Expect(CountSpinIndicators(spin) == 0,
            "Spin should remove SpinIndicator when spinning stops.",
            failures);
        Expect(indicator?.GetVisualParent() == null,
            "Removed SpinIndicator should not keep a visual parent.",
            failures);

        spin.IsSpinning = true;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<SpinIndicator>(spin, "Indicator") != null,
            "Spin should recreate SpinIndicator when spinning starts again.",
            failures);
    }

    private static void VerifySpinIndicatorAnimationLifecycle(ICollection<string> failures)
    {
        var indicator = new SpinIndicator
        {
            IsVisible = false
        };

        using var realized = RealizeControl(indicator);
        Expect(GetSpinIndicatorAnimation(indicator) == null,
            "Invisible SpinIndicator should not build animation during attach.",
            failures);

        indicator.IsVisible = true;
        RefreshLayout(realized.Window);
        Expect(GetSpinIndicatorAnimation(indicator) != null,
            "Visible SpinIndicator should lazily build animation.",
            failures);
        Expect(GetSpinIndicatorCancellationTokenSource(indicator) != null,
            "Visible SpinIndicator should start animation.",
            failures);

        indicator.IsVisible = false;
        RefreshLayout(realized.Window);
        Expect(GetSpinIndicatorCancellationTokenSource(indicator) == null,
            "Invisible SpinIndicator should stop animation.",
            failures);
    }

    private static void VerifySpinCustomIndicatorSizeSync(ICollection<string> failures)
    {
        var indicator = new SpinIndicator
        {
            SizeType         = SizeType.Large,
            CustomIndicator = new LoadingOutlined()
        };

        using var realized = RealizeControl(indicator);
        var icon = indicator.GetSelfAndVisualDescendants().OfType<LoadingOutlined>().FirstOrDefault();
        Expect(icon != null,
            "Custom SpinIndicator should create the custom LoadingOutlined icon.",
            failures);
        if (icon == null)
        {
            return;
        }

        var largeWidth = icon.Width;
        indicator.SizeType = SizeType.Small;
        RefreshLayout(realized.Window);
        Expect(icon.Width < largeWidth && icon.Height < largeWidth,
            $"Custom SpinIndicator icon should shrink on SizeType change, large {largeWidth}, actual {icon.Width}x{icon.Height}.",
            failures);
    }

    private static int CountSpinIndicators(Control root)
    {
        return root.GetSelfAndVisualDescendants().OfType<SpinIndicator>().Count();
    }

    private static object? GetSpinIndicatorAnimation(SpinIndicator indicator)
    {
        return GetPrivateField(indicator, "AtomUI.Controls.Commons.AbstractSpinIndicator", "_animation");
    }

    private static object? GetSpinIndicatorCancellationTokenSource(SpinIndicator indicator)
    {
        return GetPrivateField(indicator, "AtomUI.Controls.Commons.AbstractSpinIndicator", "_cancellationTokenSource");
    }
}
