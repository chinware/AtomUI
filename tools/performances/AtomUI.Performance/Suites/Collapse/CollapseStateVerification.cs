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

        var motionActor = FindVisualByName<Control>(item, "PART_ContentMotionActor");
        Expect(motionActor != null,
            "Closed CollapseItem should keep static PART_ContentMotionActor in the template.",
            failures);
        Expect(motionActor?.IsVisible == false,
            "Closed CollapseItem should hide PART_ContentMotionActor before first expand.",
            failures);
        var contentPresenter = (motionActor as ContentControl)?.Content as ContentPresenter;
        Expect(contentPresenter != null,
            "Closed CollapseItem should keep static PART_ContentPresenter in the template.",
            failures);
        Expect(contentPresenter == null || contentPresenter.TemplatedParent == item,
            "Closed CollapseItem content presenter should use CollapseItem as templated parent.",
            failures);
        var addonPresenter = FindCollapseItemTemplatePart<ContentPresenter>(item, "PART_AddOnContentPresenter");
        Expect(addonPresenter != null,
            "CollapseItem without AddOnContent should keep static PART_AddOnContentPresenter.",
            failures);
        Expect(addonPresenter?.Content is null,
            "CollapseItem without AddOnContent should keep addon presenter empty.",
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

        var firstButton = FindCollapseItemTemplatePart<IconButton>(item, "PART_ExpandButton");
        Expect(firstButton != null,
            "No-arrow CollapseItem should keep static PART_ExpandButton in the template.",
            failures);
        Expect(firstButton?.IsVisible == false,
            "No-arrow CollapseItem should hide PART_ExpandButton.",
            failures);
        Expect(item.ExpandIcon is RightOutlined,
            "No-arrow CollapseItem keeps the default expand icon for static template reuse.",
            failures);

        item.IsShowExpandIcon = true;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstButton, FindCollapseItemTemplatePart<IconButton>(item, "PART_ExpandButton")),
            "CollapseItem should reuse the static PART_ExpandButton when IsShowExpandIcon becomes true.",
            failures);
        Expect(firstButton?.IsVisible == true,
            "CollapseItem should show PART_ExpandButton when IsShowExpandIcon becomes true.",
            failures);
        Expect(item.ExpandIcon is RightOutlined,
            "CollapseItem should keep the default RightOutlined icon when expand button is visible.",
            failures);

        item.IsShowExpandIcon = false;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstButton, FindCollapseItemTemplatePart<IconButton>(item, "PART_ExpandButton")),
            "CollapseItem should keep the static PART_ExpandButton when IsShowExpandIcon becomes false.",
            failures);
        Expect(firstButton?.IsVisible == false,
            "CollapseItem should hide PART_ExpandButton when IsShowExpandIcon becomes false.",
            failures);
        Expect(item.ExpandIcon is RightOutlined,
            "CollapseItem should keep the default expand icon for static template reuse.",
            failures);

        item.IsShowExpandIcon = true;
        RefreshLayout(realized.Window);
        var secondButton = FindCollapseItemTemplatePart<IconButton>(item, "PART_ExpandButton");
        Expect(ReferenceEquals(firstButton, secondButton),
            "CollapseItem should reuse PART_ExpandButton across visibility toggles.",
            failures);
    }

    private static void VerifyCollapseAddOnLifecycle(ICollection<string> failures)
    {
        var item = CreateVerificationItem();
        var collapse = CreateVerificationCollapse(item);
        using var realized = RealizeControl(collapse);

        var firstPresenter = FindCollapseItemTemplatePart<ContentPresenter>(item, "PART_AddOnContentPresenter");
        Expect(firstPresenter != null,
            "CollapseItem without addon should keep static addon presenter.",
            failures);
        Expect(firstPresenter?.Content is null,
            "CollapseItem without addon should keep addon presenter empty.",
            failures);

        var addOn = new SettingOutlined();
        item.AddOnContent = addOn;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstPresenter, FindCollapseItemTemplatePart<ContentPresenter>(item, "PART_AddOnContentPresenter")),
            "CollapseItem should reuse the static addon presenter when AddOnContent is assigned.",
            failures);
        Expect(ReferenceEquals(firstPresenter?.Content, addOn),
            "Addon presenter should bind to AddOnContent.",
            failures);

        item.AddOnContent = null;
        RefreshLayout(realized.Window);
        Expect(ReferenceEquals(firstPresenter, FindCollapseItemTemplatePart<ContentPresenter>(item, "PART_AddOnContentPresenter")),
            "CollapseItem should keep the static addon presenter when AddOnContent is cleared.",
            failures);
        Expect(firstPresenter?.Content == null,
            "Addon presenter should clear Content.",
            failures);
        Expect(addOn.GetVisualParent() == null,
            "Cleared addon content should not keep a visual parent.",
            failures);

        item.AddOnContent = new SettingOutlined();
        RefreshLayout(realized.Window);
        var secondPresenter = FindCollapseItemTemplatePart<ContentPresenter>(item, "PART_AddOnContentPresenter");
        Expect(ReferenceEquals(firstPresenter, secondPresenter),
            "CollapseItem should reuse addon presenter across Content toggles.",
            failures);
    }

    private static void VerifyCollapseContentMotionLifecycle(ICollection<string> failures)
    {
        var item = CreateVerificationItem();
        var collapse = CreateVerificationCollapse(item);
        collapse.IsMotionEnabled = false;
        using var realized = RealizeControl(collapse);

        var firstActor = FindCollapseItemTemplatePart<Control>(item, "PART_ContentMotionActor");
        Expect(firstActor != null,
            "Closed CollapseItem should keep static content motion actor.",
            failures);
        Expect(firstActor?.IsVisible == false,
            "Closed CollapseItem should hide content motion actor before first expand.",
            failures);

        item.IsSelected = true;
        RefreshLayout(realized.Window);
        var firstPresenter = (firstActor as ContentControl)?.Content as ContentPresenter;
        Expect(firstActor != null,
            "CollapseItem should keep content motion actor on first expand.",
            failures);
        Expect(firstPresenter != null,
            "CollapseItem should keep content presenter on first expand.",
            failures);
        Expect(firstActor?.IsVisible == true,
            "Expanded CollapseItem should show content motion actor.",
            failures);

        item.IsSelected = false;
        RefreshLayout(realized.Window);
        Expect(CountCollapseVisualsByName(item, "PART_ContentMotionActor") == 1,
            "CollapseItem should keep one materialized motion actor after collapse, not duplicate it.",
            failures);

        item.IsSelected = true;
        RefreshLayout(realized.Window);
        var secondActor = FindCollapseItemTemplatePart<Control>(item, "PART_ContentMotionActor");
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
        Expect(normalItem.EffectiveHeaderPadding == new Thickness(7),
            "Collapse.ItemHeaderPadding should update prepared items dynamically.",
            failures);
        Expect(normalItem.EffectiveContentPadding == new Thickness(9),
            "Collapse.ItemContentPadding should update prepared items dynamically.",
            failures);
        Expect(explicitItem.EffectiveHeaderPadding == new Thickness(3),
            "Collapse should not override an item-level HeaderPadding local value.",
            failures);
        Expect(explicitItem.EffectiveContentPadding == new Thickness(4),
            "Collapse should not override an item-level ContentPadding local value.",
            failures);

        collapse.ItemHeaderPadding = new Thickness(11);
        collapse.ItemContentPadding = new Thickness(13);
        RefreshLayout(realized.Window);
        Expect(normalItem.EffectiveHeaderPadding == new Thickness(11),
            "Collapse.ItemHeaderPadding binding should update after the first assignment.",
            failures);
        Expect(normalItem.EffectiveContentPadding == new Thickness(13),
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
        Expect(item.EffectiveHeaderPadding == new Thickness(7),
            "Collapse.ItemHeaderPadding should bind to a realized item before removal.",
            failures);
        Expect(item.EffectiveContentPadding == new Thickness(9),
            "Collapse.ItemContentPadding should bind to a realized item before removal.",
            failures);

        collapse.Items.Remove(item);
        RefreshLayout(realized.Window);
        var detachedHeaderPadding  = item.EffectiveHeaderPadding;
        var detachedContentPadding = item.EffectiveContentPadding;

        collapse.ItemHeaderPadding = new Thickness(21);
        collapse.ItemContentPadding = new Thickness(23);
        RefreshLayout(realized.Window);
        Expect(item.EffectiveHeaderPadding == detachedHeaderPadding,
            "Removed CollapseItem should not keep a live ItemHeaderPadding binding.",
            failures);
        Expect(item.EffectiveContentPadding == detachedContentPadding,
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

    private static T? FindCollapseItemTemplatePart<T>(CollapseItem item, string name)
        where T : Control
    {
        return item.GetSelfAndVisualDescendants()
                   .OfType<T>()
                   .FirstOrDefault(control => control.Name == name &&
                                              ReferenceEquals(control.TemplatedParent, item));
    }

    private static string DescribeThickness(Thickness? thickness)
    {
        return thickness is null
            ? "<null>"
            : $"{thickness.Value.Left},{thickness.Value.Top},{thickness.Value.Right},{thickness.Value.Bottom}";
    }
}
