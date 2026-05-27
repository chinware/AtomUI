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
        var maskLayout = FindVisualByName<Panel>(spin, "MaskLayout");
        var indicator  = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(maskLayout != null,
            "Spin should keep its static MaskLayout template part materialized.",
            failures);
        Expect(maskLayout?.IsVisible == false,
            "Non-spinning Spin should hide MaskLayout.",
            failures);
        Expect(indicator != null,
            "Spin should keep its static SpinIndicator template part materialized.",
            failures);
        Expect(indicator?.IsVisible == false,
            "Non-spinning Spin should hide SpinIndicator.",
            failures);
        Expect(indicator == null || GetSpinIndicatorAnimation(indicator) == null,
            "Hidden static SpinIndicator should not build animation.",
            failures);

        spin.IsSpinning = true;
        RefreshLayout(realized.Window);

        maskLayout = FindVisualByName<Panel>(spin, "MaskLayout");
        indicator  = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(maskLayout?.IsVisible == true,
            "Spinning Spin should show MaskLayout.",
            failures);
        Expect(indicator?.IsVisible == true,
            "Spinning Spin should show SpinIndicator.",
            failures);
        Expect(indicator == null || GetSpinIndicatorAnimation(indicator) != null,
            "Visible static SpinIndicator should lazily build animation.",
            failures);
        Expect(indicator == null || GetSpinIndicatorCancellationTokenSource(indicator) != null,
            "Visible static SpinIndicator should start animation.",
            failures);
        Expect(FindVisualByName<AtomTextBlock>(spin, "Tip") != null,
            "Spin should keep Tip TextBlock available from the static template.",
            failures);

        var updatedIcon = new LoadingOutlined();
        spin.SizeType        = SizeType.Large;
        spin.Tip             = "Still loading...";
        spin.IsTipVisible    = false;
        spin.CustomIndicator = updatedIcon;
        RefreshLayout(realized.Window);

        Expect(indicator?.SizeType == SizeType.Large,
            "Static SpinIndicator should sync SizeType changes from Spin.",
            failures);
        Expect(ReferenceEquals(indicator?.CustomIndicator, updatedIcon),
            "Static SpinIndicator should sync CustomIndicator changes from Spin.",
            failures);
        var tip = FindVisualByName<AtomTextBlock>(spin, "Tip");
        Expect(tip?.Text == "Still loading...",
            "Static Spin tip should sync Tip text changes from Spin.",
            failures);
        Expect(tip?.IsVisible == false,
            "Static Spin tip should sync IsTipVisible changes from Spin.",
            failures);

        spin.IsSpinning = false;
        RefreshLayout(realized.Window);

        maskLayout = FindVisualByName<Panel>(spin, "MaskLayout");
        indicator  = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(maskLayout?.IsVisible == false,
            "Spin should hide MaskLayout when spinning stops.",
            failures);
        Expect(indicator?.IsVisible == false,
            "Spin should hide SpinIndicator when spinning stops.",
            failures);
        Expect(indicator == null || GetSpinIndicatorCancellationTokenSource(indicator) == null,
            "Hidden static SpinIndicator should stop animation when spinning stops.",
            failures);

        spin.IsSpinning = true;
        RefreshLayout(realized.Window);
        indicator = FindVisualByName<SpinIndicator>(spin, "Indicator");
        Expect(indicator?.IsVisible == true,
            "Spin should show SpinIndicator when spinning starts again.",
            failures);
        Expect(indicator == null || GetSpinIndicatorCancellationTokenSource(indicator) != null,
            "Spin should restart SpinIndicator animation when spinning starts again.",
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
        var firstAnimation = GetSpinIndicatorAnimation(indicator);
        var firstToken     = GetSpinIndicatorCancellationTokenSource(indicator);
        Expect(firstAnimation != null,
            "Visible SpinIndicator should lazily build animation.",
            failures);
        Expect(firstToken != null,
            "Visible SpinIndicator should start animation.",
            failures);

        indicator.MotionDuration = TimeSpan.FromMilliseconds(1234);
        RefreshLayout(realized.Window);
        Expect(!ReferenceEquals(GetSpinIndicatorAnimation(indicator), firstAnimation),
            "Materialized SpinIndicator animation should rebuild when MotionDuration changes.",
            failures);
        Expect(!ReferenceEquals(GetSpinIndicatorCancellationTokenSource(indicator), firstToken),
            "Running SpinIndicator animation should restart when MotionDuration changes.",
            failures);
        Expect(GetSpinIndicatorCancellationTokenSource(indicator) != null,
            "Running SpinIndicator animation should stay active after MotionDuration changes.",
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

    private static object? GetSpinIndicatorAnimation(SpinIndicator indicator)
    {
        return GetPrivateField(indicator, "AtomUI.Controls.Commons.AbstractSpinIndicator", "_animation");
    }

    private static object? GetSpinIndicatorCancellationTokenSource(SpinIndicator indicator)
    {
        return GetPrivateField(indicator, "AtomUI.Controls.Commons.AbstractSpinIndicator", "_cancellationTokenSource");
    }
}
