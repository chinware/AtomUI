using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using AtomSegmented = AtomUI.Desktop.Controls.Segmented;
using AtomSegmentedItem = AtomUI.Desktop.Controls.SegmentedItem;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunSegmentedStateVerification()
    {
        var failures = new List<string>();
        VerifySegmentedIconPresenterLifecycle(failures);
        VerifySegmentedIconPresenterStyling(failures);
        VerifySegmentedDefaultSelection(failures);
        VerifySegmentedHiddenItemLayout(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Segmented state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Segmented state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifySegmentedIconPresenterLifecycle(ICollection<string> failures)
    {
        var item      = new AtomSegmentedItem { Content = "Daily" };
        var segmented = CreateVerificationSegmented(item);
        using var realized = RealizeControl(segmented);

        Expect(FindVisualByName<IconPresenter>(item, "PART_IconPresenter") == null,
            "Text-only SegmentedItem should not create an IconPresenter.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(item, "Content") != null,
            "Text SegmentedItem should keep its ContentPresenter.",
            failures);

        var firstIcon = new BarsOutlined();
        item.SetCurrentValue(AtomSegmentedItem.IconProperty, firstIcon);
        RefreshLayout(realized.Window);
        var firstPresenter = FindVisualByName<IconPresenter>(item, "PART_IconPresenter");
        Expect(firstPresenter != null,
            "SegmentedItem should create PART_IconPresenter when Icon is assigned.",
            failures);
        Expect(ReferenceEquals(firstPresenter?.Icon, firstIcon),
            "SegmentedItem icon presenter should bind to the item Icon property.",
            failures);
        Expect((firstPresenter?.GetVisualParent() as Control)?.Name == "PART_ContentLayout",
            "SegmentedItem icon presenter should be attached directly to PART_ContentLayout.",
            failures);

        var secondIcon = new AppstoreOutlined();
        item.SetCurrentValue(AtomSegmentedItem.IconProperty, secondIcon);
        RefreshLayout(realized.Window);
        var currentPresenter = FindVisualByName<IconPresenter>(item, "PART_IconPresenter");
        Expect(ReferenceEquals(currentPresenter, firstPresenter),
            "Changing SegmentedItem Icon should reuse the existing presenter.",
            failures);
        Expect(firstIcon.GetVisualParent() == null,
            "Replacing SegmentedItem Icon should detach the previous PathIcon.",
            failures);
        Expect(ReferenceEquals(currentPresenter?.Icon, secondIcon),
            "Changing SegmentedItem Icon should update the existing presenter.",
            failures);

        item.SetCurrentValue(AtomSegmentedItem.IconProperty, null);
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(item, "PART_IconPresenter") == null,
            "Clearing SegmentedItem Icon should remove PART_IconPresenter.",
            failures);
        Expect(firstPresenter?.GetVisualParent() == null,
            "Removed SegmentedItem icon presenter should not keep a visual parent.",
            failures);
        Expect(firstPresenter == null || firstPresenter.TemplatedParent == null,
            "Removed SegmentedItem icon presenter should clear TemplatedParent.",
            failures);
        Expect(secondIcon.GetVisualParent() == null,
            "Clearing SegmentedItem Icon should detach the current PathIcon.",
            failures);

        item.SetCurrentValue(AtomSegmentedItem.IconProperty, new WindowsOutlined());
        RefreshLayout(realized.Window);
        var recreatedPresenter = FindVisualByName<IconPresenter>(item, "PART_IconPresenter");
        Expect(recreatedPresenter != null && !ReferenceEquals(recreatedPresenter, firstPresenter),
            "SegmentedItem should recreate an icon presenter after Icon is assigned again.",
            failures);
    }

    private static void VerifySegmentedIconPresenterStyling(ICollection<string> failures)
    {
        var largeItem = new AtomSegmentedItem
        {
            Content = "Large",
            Icon    = new BarsOutlined()
        };
        var largeSegmented = CreateVerificationSegmented(SizeType.Large, largeItem);
        using var _ = RealizeControl(largeSegmented);
        var largePresenter = FindVisualByName<IconPresenter>(largeItem, "PART_IconPresenter");
        Expect(largePresenter is { Width: > 0, Height: > 0 } &&
               MathUtils.AreClose(largePresenter.Width, largePresenter.Height),
            "Large SegmentedItem icon presenter should receive square themed sizing.",
            failures);

        var smallItem = new AtomSegmentedItem
        {
            Content = "Small",
            Icon    = new BarsOutlined()
        };
        var smallSegmented = CreateVerificationSegmented(SizeType.Small, smallItem);
        using var __ = RealizeControl(smallSegmented);
        var smallPresenter = FindVisualByName<IconPresenter>(smallItem, "PART_IconPresenter");
        Expect(smallPresenter is { Width: > 0, Height: > 0 } &&
               MathUtils.AreClose(smallPresenter.Width, smallPresenter.Height),
            "Small SegmentedItem icon presenter should receive square themed sizing.",
            failures);
        if (largePresenter != null && smallPresenter != null)
        {
            Expect(MathUtils.GreaterThanOrClose(largePresenter.Width, smallPresenter.Width),
                "Large SegmentedItem icon presenter should not be smaller than Small.",
                failures);
        }

        var disabledItem = new AtomSegmentedItem
        {
            Content   = "Disabled",
            Icon      = new BarsOutlined(),
            IsEnabled = false
        };
        var disabledSegmented = CreateVerificationSegmented(disabledItem);
        using var ___ = RealizeControl(disabledSegmented);
        var disabledPresenter = FindVisualByName<IconPresenter>(disabledItem, "PART_IconPresenter");
        ExpectBrush(disabledPresenter?.IconBrush, disabledItem.Foreground, "Disabled SegmentedItem icon brush", failures);

        var selectedItem = new AtomSegmentedItem
        {
            Content    = "Selected",
            Icon       = new BarsOutlined(),
            IsSelected = true
        };
        var selectedSegmented = CreateVerificationSegmented(
            new AtomSegmentedItem { Content = "Default" },
            selectedItem);
        using var ____ = RealizeControl(selectedSegmented);
        var selectedPresenter = FindVisualByName<IconPresenter>(selectedItem, "PART_IconPresenter");
        ExpectBrush(selectedPresenter?.IconBrush, selectedItem.Foreground, "Selected SegmentedItem icon brush", failures);
    }

    private static void VerifySegmentedDefaultSelection(ICollection<string> failures)
    {
        var first = new AtomSegmentedItem { Content = "Daily" };
        var second = new AtomSegmentedItem { Content = "Weekly" };
        var defaultSegmented = CreateVerificationSegmented(first, second);
        using var _ = RealizeControl(defaultSegmented);
        Expect(defaultSegmented.SelectedIndex == 0,
            $"Segmented should select index 0 by default, actual {defaultSegmented.SelectedIndex}.",
            failures);
        Expect(first.IsSelected,
            "Default Segmented selection should mark the first item as selected.",
            failures);

        var explicitFirst = new AtomSegmentedItem { Content = "Daily" };
        var explicitSecond = new AtomSegmentedItem
        {
            Content    = "Weekly",
            IsSelected = true
        };
        var explicitSegmented = CreateVerificationSegmented(explicitFirst, explicitSecond);
        using var __ = RealizeControl(explicitSegmented);
        Expect(explicitSegmented.SelectedIndex == 1,
            $"Segmented should preserve explicit item selection, actual {explicitSegmented.SelectedIndex}.",
            failures);
        Expect(!explicitFirst.IsSelected && explicitSecond.IsSelected,
            "Explicit Segmented selection should not be overwritten by default selection.",
            failures);
    }

    private static void VerifySegmentedHiddenItemLayout(ICollection<string> failures)
    {
        var first = new AtomSegmentedItem { Content = "One" };
        var hidden = new AtomSegmentedItem
        {
            Content   = "Hidden",
            IsVisible = false
        };
        var third = new AtomSegmentedItem { Content = "Three" };
        var segmented = CreateVerificationSegmented(first, hidden, third);
        segmented.IsExpanding = true;
        using var _ = RealizeControl(segmented);

        Expect(MathUtils.AreClose(first.Bounds.Width, third.Bounds.Width, 0.5),
            $"Expanding Segmented should divide width between visible items, first={first.Bounds.Width:0.###}, third={third.Bounds.Width:0.###}.",
            failures);
        Expect(MathUtils.AreClose(hidden.Bounds.Width, 0, 0.5),
            $"Hidden SegmentedItem should not occupy arranged width, actual {hidden.Bounds.Width:0.###}.",
            failures);

        var allHidden = CreateVerificationSegmented(
            new AtomSegmentedItem { Content = "A", IsVisible = false },
            new AtomSegmentedItem { Content = "B", IsVisible = false });
        allHidden.IsExpanding = true;
        using var __ = RealizeControl(allHidden);
        Expect(allHidden.Bounds.Width >= 0 && allHidden.Bounds.Height >= 0,
            "Expanding Segmented with all items hidden should complete layout without invalid bounds.",
            failures);
    }

    private static AtomSegmented CreateVerificationSegmented(params AtomSegmentedItem[] items)
    {
        return CreateVerificationSegmented(sizeType: null, items);
    }

    private static AtomSegmented CreateVerificationSegmented(SizeType sizeType, params AtomSegmentedItem[] items)
    {
        return CreateVerificationSegmented((SizeType?)sizeType, items);
    }

    private static AtomSegmented CreateVerificationSegmented(SizeType? sizeType, params AtomSegmentedItem[] items)
    {
        var segmented = new AtomSegmented();
        if (sizeType is { } value)
        {
            segmented.SizeType = value;
        }
        foreach (var item in items)
        {
            segmented.Items.Add(item);
        }
        return segmented;
    }
}
