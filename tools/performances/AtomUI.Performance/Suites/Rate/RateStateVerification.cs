using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.VisualTree;
using AtomRate = AtomUI.Desktop.Controls.Rate;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunRateStateVerification()
    {
        var failures = new List<string>();
        VerifyRatePointerSelection(failures);
        VerifyRateToolTipsAreSynchronized(failures);
        VerifyRateCountReuseAndFocusBorder(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Rate state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Rate state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyRatePointerSelection(ICollection<string> failures)
    {
        var rate = new AtomRate();
        using var realized = RealizeControl(rate);

        ClickRateItem(realized.Window, rate, 2, 0.75, failures, "third item");
        RefreshLayout(realized.Window);
        Expect(Math.Abs(rate.Value - 3.0) < 0.001,
            $"Rate should select value 3 on third item click. Actual={rate.Value:0.###}.",
            failures);

        ClickRateItem(realized.Window, rate, 2, 0.75, failures, "third item clear");
        RefreshLayout(realized.Window);
        Expect(Math.Abs(rate.Value) < 0.001,
            $"Rate should clear when clicking the same value and IsAllowClear=true. Actual={rate.Value:0.###}.",
            failures);

        var halfRate = new AtomRate
        {
            IsAllowHalf = true
        };
        using var halfRealized = RealizeControl(halfRate);
        ClickRateItem(halfRealized.Window, halfRate, 1, 0.25, failures, "half-rate second item");
        RefreshLayout(halfRealized.Window);
        Expect(Math.Abs(halfRate.Value - 1.5) < 0.001,
            $"Rate should select half value 1.5 on second item left-half click. Actual={halfRate.Value:0.###}.",
            failures);

        var disabledRate = new AtomRate
        {
            DefaultValue = 2,
            IsEnabled    = false
        };
        using var disabledRealized = RealizeControl(disabledRate);
        ClickRateItem(disabledRealized.Window, disabledRate, 4, 0.75, failures, "disabled fifth item");
        RefreshLayout(disabledRealized.Window);
        Expect(Math.Abs(disabledRate.Value - 2.0) < 0.001,
            $"Disabled Rate should ignore pointer clicks. Actual={disabledRate.Value:0.###}.",
            failures);

        var canceledRate = new AtomRate();
        using var canceledRealized = RealizeControl(canceledRate);
        PressRateItemAndReleaseOutside(canceledRealized.Window, canceledRate, 3, failures);
        RefreshLayout(canceledRealized.Window);
        Expect(Math.Abs(canceledRate.Value) < 0.001,
            $"Rate should not commit selection when pointer releases outside. Actual={canceledRate.Value:0.###}.",
            failures);
    }

    private static void VerifyRateToolTipsAreSynchronized(ICollection<string> failures)
    {
        var rate = new AtomRate
        {
            ToolTips = ["Terrible", "Bad", "Normal", "Good", "Wonderful"]
        };
        using var realized = RealizeControl(rate);
        var items = GetRateItems(rate);
        Expect(items.Count == 5,
            $"Rate should realize five items for default Count. Actual={items.Count}.",
            failures);
        Expect(Equals(ToolTip.GetTip(items[4]), "Wonderful"),
            "Rate should apply initial tooltip text to the fifth item.",
            failures);

        rate.SetCurrentValue(AtomRate.ToolTipsProperty, new List<string> { "Only first" });
        RefreshLayout(realized.Window);
        items = GetRateItems(rate);
        Expect(Equals(ToolTip.GetTip(items[0]), "Only first"),
            "Rate should update the first tooltip when ToolTips changes.",
            failures);
        Expect(ToolTip.GetTip(items[1]) is null && ToolTip.GetTip(items[4]) is null,
            "Rate should clear stale tooltip values when the new ToolTips list is shorter.",
            failures);

        rate.SetCurrentValue(AtomRate.ToolTipsProperty, null);
        RefreshLayout(realized.Window);
        Expect(GetRateItems(rate).All(item => ToolTip.GetTip(item) is null),
            "Rate should clear all tooltip values when ToolTips becomes null.",
            failures);
    }

    private static void VerifyRateCountReuseAndFocusBorder(ICollection<string> failures)
    {
        var rate = new AtomRate();
        using var realized = RealizeControl(rate);
        var initialItems = GetRateItems(rate);
        Expect(initialItems.Count == 5,
            $"Rate should realize five items initially. Actual={initialItems.Count}.",
            failures);
        Expect(!HasVisualByTypeName(rate, "DashedBorder"),
            "Rate should render keyboard focus border without hidden DashedBorder visuals.",
            failures);

        rate.SetCurrentValue(AtomRate.CountProperty, 3);
        RefreshLayout(realized.Window);
        var reducedItems = GetRateItems(rate);
        Expect(reducedItems.Count == 3,
            $"Rate should realize three items after Count=3. Actual={reducedItems.Count}.",
            failures);
        Expect(reducedItems.SequenceEqual(initialItems.Take(3)),
            "Rate should reuse existing RateItems when Count is reduced.",
            failures);

        rate.SetCurrentValue(AtomRate.CountProperty, 5);
        RefreshLayout(realized.Window);
        var expandedItems = GetRateItems(rate);
        Expect(expandedItems.Count == 5,
            $"Rate should realize five items after Count=5. Actual={expandedItems.Count}.",
            failures);
        Expect(expandedItems.Take(3).SequenceEqual(reducedItems),
            "Rate should keep reused RateItems when Count is expanded again.",
            failures);
    }

    private static void ClickRateItem(Avalonia.Controls.Window window,
                                      Control rate,
                                      int itemIndex,
                                      double horizontalRatio,
                                      ICollection<string> failures,
                                      string label)
    {
        var items = GetRateItems(rate);
        if (itemIndex < 0 || itemIndex >= items.Count)
        {
            failures.Add($"{label}: Rate item index {itemIndex} is out of range. Count={items.Count}.");
            return;
        }

        var item = items[itemIndex];
        var point = item.TranslatePoint(new Point(item.Bounds.Width * horizontalRatio, item.Bounds.Height / 2), window);
        if (!point.HasValue)
        {
            failures.Add($"{label}: Rate item point should be available for pointer verification.");
            return;
        }

        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private static void PressRateItemAndReleaseOutside(Avalonia.Controls.Window window,
                                                       Control rate,
                                                       int itemIndex,
                                                       ICollection<string> failures)
    {
        var items = GetRateItems(rate);
        if (itemIndex < 0 || itemIndex >= items.Count)
        {
            failures.Add($"release outside: Rate item index {itemIndex} is out of range. Count={items.Count}.");
            return;
        }

        var item = items[itemIndex];
        var point = item.TranslatePoint(new Point(item.Bounds.Width * 0.75, item.Bounds.Height / 2), window);
        if (!point.HasValue)
        {
            failures.Add("release outside: Rate item point should be available for pointer verification.");
            return;
        }

        var outsidePoint = rate.TranslatePoint(new Point(rate.Bounds.Width + 40, rate.Bounds.Height + 40), window);
        if (!outsidePoint.HasValue)
        {
            failures.Add("release outside: outside point should be available for pointer verification.");
            return;
        }

        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseMove(outsidePoint.Value);
        window.MouseUp(outsidePoint.Value, MouseButton.Left);
    }

    private static List<Control> GetRateItems(Control rate)
    {
        return rate.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Where(control => control.GetType().Name == "RateItem")
                   .ToList();
    }

    private static bool HasVisualByTypeName(Control control, string typeName)
    {
        return control.GetSelfAndVisualDescendants()
                      .Any(visual => visual.GetType().Name == typeName);
    }
}
