using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunCollapseStateVerification()
    {
        var failures = new List<string>();
        VerifyCollapseClosedSlotsAreLazy(failures);
        VerifyCollapseNoArrowLifecycle(failures);
        VerifyCollapseAddOnLifecycle(failures);
        VerifyCollapseContentMotionLifecycle(failures);
        VerifyCollapsePaddingAndBorderSync(failures);
        VerifyCollapsePaddingBindingsReleaseOnRemove(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Collapse state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Collapse state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyCollapseClosedSlotsAreLazy(ICollection<string> failures)
    {
        var item = CreateVerificationItem();
        var collapse = CreateVerificationCollapse(item);
        using var _ = RealizeControl(collapse);

        Expect(FindVisualByName<Control>(item, "PART_ContentMotionActor") == null,
            "Closed CollapseItem should not create PART_ContentMotionActor before first expand.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(item, "PART_ContentPresenter") == null,
            "Closed CollapseItem should not create PART_ContentPresenter before first expand.",
            failures);
        Expect(FindVisualByName<ContentPresenter>(item, "PART_AddOnContentPresenter") == null,
            "CollapseItem without AddOnContent should not create PART_AddOnContentPresenter.",
            failures);
        Expect(FindVisualByName<IconButton>(item, "PART_ExpandButton") != null,
            "Default CollapseItem should still create PART_ExpandButton.",
            failures);
    }

    private static void VerifyCollapseNoArrowLifecycle(ICollection<string> failures)
    {
        var item = CreateVerificationItem();
        item.IsShowExpandIcon = false;
        var collapse = CreateVerificationCollapse(item);
        using var realized = RealizeControl(collapse);

        Expect(FindVisualByName<IconButton>(item, "PART_ExpandButton") == null,
            "No-arrow CollapseItem should not create PART_ExpandButton.",
            failures);
        Expect(item.ExpandIcon == null,
            "No-arrow CollapseItem should not create the default expand icon.",
            failures);

        item.IsShowExpandIcon = true;
        RefreshLayout(realized.Window);
        var firstButton = FindVisualByName<IconButton>(item, "PART_ExpandButton");
        Expect(firstButton != null,
            "CollapseItem should create PART_ExpandButton when IsShowExpandIcon becomes true.",
            failures);
        Expect(item.ExpandIcon is RightOutlined,
            "CollapseItem should create the default RightOutlined icon when expand button is needed.",
            failures);

        item.IsShowExpandIcon = false;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconButton>(item, "PART_ExpandButton") == null,
            "CollapseItem should remove PART_ExpandButton when IsShowExpandIcon becomes false.",
            failures);
        Expect(firstButton?.GetVisualParent() == null,
            "Removed PART_ExpandButton should not keep a visual parent.",
            failures);
        Expect(firstButton == null || firstButton.TemplatedParent == null,
            "Removed PART_ExpandButton should clear templated parent.",
            failures);
        Expect(item.ExpandIcon == null,
            "CollapseItem should release the generated default expand icon when no arrow is shown.",
            failures);

        item.IsShowExpandIcon = true;
        RefreshLayout(realized.Window);
        var secondButton = FindVisualByName<IconButton>(item, "PART_ExpandButton");
        Expect(secondButton != null && !ReferenceEquals(firstButton, secondButton),
            "CollapseItem should recreate PART_ExpandButton cleanly after it was removed.",
            failures);
    }

    private static void VerifyCollapseAddOnLifecycle(ICollection<string> failures)
    {
        var item = CreateVerificationItem();
        var collapse = CreateVerificationCollapse(item);
        using var realized = RealizeControl(collapse);

        Expect(FindVisualByName<ContentPresenter>(item, "PART_AddOnContentPresenter") == null,
            "CollapseItem without addon should not create addon presenter.",
            failures);

        var addOn = new SettingOutlined();
        item.AddOnContent = addOn;
        RefreshLayout(realized.Window);
        var firstPresenter = FindVisualByName<ContentPresenter>(item, "PART_AddOnContentPresenter");
        Expect(firstPresenter != null,
            "CollapseItem should create addon presenter when AddOnContent is assigned.",
            failures);
        Expect(ReferenceEquals(firstPresenter?.Content, addOn),
            "Addon presenter should bind to AddOnContent.",
            failures);

        item.AddOnContent = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<ContentPresenter>(item, "PART_AddOnContentPresenter") == null,
            "CollapseItem should remove addon presenter when AddOnContent is cleared.",
            failures);
        Expect(firstPresenter?.GetVisualParent() == null,
            "Removed addon presenter should not keep a visual parent.",
            failures);
        Expect(firstPresenter == null || firstPresenter.TemplatedParent == null,
            "Removed addon presenter should clear templated parent.",
            failures);
        Expect(firstPresenter?.Content == null,
            "Removed addon presenter should clear Content.",
            failures);
        Expect(addOn.GetVisualParent() == null,
            "Removed addon content should not keep a visual parent.",
            failures);

        item.AddOnContent = new SettingOutlined();
        RefreshLayout(realized.Window);
        var secondPresenter = FindVisualByName<ContentPresenter>(item, "PART_AddOnContentPresenter");
        Expect(secondPresenter != null && !ReferenceEquals(firstPresenter, secondPresenter),
            "CollapseItem should recreate addon presenter cleanly after removal.",
            failures);
    }

    private static void VerifyCollapseContentMotionLifecycle(ICollection<string> failures)
    {
        var item = CreateVerificationItem();
        var collapse = CreateVerificationCollapse(item);
        collapse.IsMotionEnabled = false;
        using var realized = RealizeControl(collapse);

        Expect(FindVisualByName<Control>(item, "PART_ContentMotionActor") == null,
            "Closed CollapseItem should not create content motion actor before first expand.",
            failures);

        item.IsSelected = true;
        RefreshLayout(realized.Window);
        var firstActor = FindVisualByName<Control>(item, "PART_ContentMotionActor");
        var firstPresenter = (firstActor as ContentControl)?.Content as ContentPresenter;
        Expect(firstActor != null,
            "CollapseItem should create content motion actor on first expand.",
            failures);
        Expect(firstPresenter != null,
            "CollapseItem should create content presenter on first expand.",
            failures);

        item.IsSelected = false;
        RefreshLayout(realized.Window);
        Expect(CountCollapseVisualsByName(item, "PART_ContentMotionActor") == 1,
            "CollapseItem should keep one materialized motion actor after collapse, not duplicate it.",
            failures);

        item.IsSelected = true;
        RefreshLayout(realized.Window);
        var secondActor = FindVisualByName<Control>(item, "PART_ContentMotionActor");
        Expect(ReferenceEquals(firstActor, secondActor),
            "CollapseItem should reuse the materialized motion actor on second expand.",
            failures);
        Expect(ReferenceEquals(firstPresenter, (secondActor as ContentControl)?.Content),
            "CollapseItem should not duplicate content presenter across expand/collapse toggles.",
            failures);
        Expect(firstPresenter == null || firstPresenter.TemplatedParent == item,
            "Materialized content presenter should use CollapseItem as templated parent.",
            failures);
    }

    private static void VerifyCollapsePaddingAndBorderSync(ICollection<string> failures)
    {
        var normalItem = CreateVerificationItem();
        var explicitItem = CreateVerificationItem();
        explicitItem.HeaderPadding = new Thickness(3);
        explicitItem.ContentPadding = new Thickness(4);
        var collapse = CreateVerificationCollapse(normalItem, explicitItem);
        using var realized = RealizeControl(collapse);

        collapse.ItemHeaderPadding = new Thickness(7);
        collapse.ItemContentPadding = new Thickness(9);
        RefreshLayout(realized.Window);
        Expect(normalItem.HeaderPadding == new Thickness(7),
            "Collapse.ItemHeaderPadding should update prepared items dynamically.",
            failures);
        Expect(normalItem.ContentPadding == new Thickness(9),
            "Collapse.ItemContentPadding should update prepared items dynamically.",
            failures);
        Expect(explicitItem.HeaderPadding == new Thickness(3),
            "Collapse should not override an item-level HeaderPadding local value.",
            failures);
        Expect(explicitItem.ContentPadding == new Thickness(4),
            "Collapse should not override an item-level ContentPadding local value.",
            failures);

        collapse.ItemHeaderPadding = new Thickness(11);
        collapse.ItemContentPadding = new Thickness(13);
        RefreshLayout(realized.Window);
        Expect(normalItem.HeaderPadding == new Thickness(11),
            "Collapse.ItemHeaderPadding binding should update after the first assignment.",
            failures);
        Expect(normalItem.ContentPadding == new Thickness(13),
            "Collapse.ItemContentPadding binding should update after the first assignment.",
            failures);

        var frame = FindVisualByName<Border>(collapse, "PART_Frame");
        collapse.BorderThickness = new Thickness(5);
        collapse.IsGhostStyle = true;
        RefreshLayout(realized.Window);
        Expect(frame?.BorderThickness == new Thickness(0),
            $"Collapse.IsGhostStyle should update effective frame border at runtime, actual {DescribeThickness(frame?.BorderThickness)}.",
            failures);

        collapse.IsGhostStyle = false;
        collapse.IsBorderless = true;
        RefreshLayout(realized.Window);
        Expect(frame?.BorderThickness == new Thickness(0),
            $"Collapse.IsBorderless should update effective frame border at runtime, actual {DescribeThickness(frame?.BorderThickness)}.",
            failures);

        collapse.IsBorderless = false;
        RefreshLayout(realized.Window);
        Expect(frame?.BorderThickness == new Thickness(5),
            $"Collapse should restore effective frame border when ghost/borderless are cleared, actual {DescribeThickness(frame?.BorderThickness)}.",
            failures);
    }

    private static void VerifyCollapsePaddingBindingsReleaseOnRemove(ICollection<string> failures)
    {
        var item = CreateVerificationItem();
        var collapse = CreateVerificationCollapse(item);
        using var realized = RealizeControl(collapse);

        collapse.ItemHeaderPadding = new Thickness(7);
        collapse.ItemContentPadding = new Thickness(9);
        RefreshLayout(realized.Window);
        Expect(item.HeaderPadding == new Thickness(7),
            "Collapse.ItemHeaderPadding should bind to a realized item before removal.",
            failures);
        Expect(item.ContentPadding == new Thickness(9),
            "Collapse.ItemContentPadding should bind to a realized item before removal.",
            failures);

        collapse.Items.Remove(item);
        RefreshLayout(realized.Window);
        var detachedHeaderPadding  = item.HeaderPadding;
        var detachedContentPadding = item.ContentPadding;

        collapse.ItemHeaderPadding = new Thickness(21);
        collapse.ItemContentPadding = new Thickness(23);
        RefreshLayout(realized.Window);
        Expect(item.HeaderPadding == detachedHeaderPadding,
            "Removed CollapseItem should not keep a live ItemHeaderPadding binding.",
            failures);
        Expect(item.ContentPadding == detachedContentPadding,
            "Removed CollapseItem should not keep a live ItemContentPadding binding.",
            failures);
    }

    private static Collapse CreateVerificationCollapse(params CollapseItem[] items)
    {
        var collapse = new Collapse();
        foreach (var item in items)
        {
            collapse.Items.Add(item);
        }
        return collapse;
    }

    private static CollapseItem CreateVerificationItem()
    {
        return new CollapseItem
        {
            Header = "Header",
            Content = "Content"
        };
    }

    private static int CountCollapseVisualsByName(Control root, string name)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Count(control => control.Name == name);
    }

    private static string DescribeThickness(Thickness? thickness)
    {
        return thickness is null
            ? "<null>"
            : $"{thickness.Value.Left},{thickness.Value.Top},{thickness.Value.Right},{thickness.Value.Bottom}";
    }
}
