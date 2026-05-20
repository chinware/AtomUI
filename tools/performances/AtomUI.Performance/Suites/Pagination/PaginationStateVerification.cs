using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunPaginationStateVerification()
    {
        var failures = new List<string>();
        VerifySimplePaginationQuickJumperLifecycle(failures);
        VerifyPaginationFeatureLifecycle(failures);
        VerifyPaginationNavItemSlots(failures);
        VerifyQuickJumperBarLineEditLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Pagination state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Pagination state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifySimplePaginationQuickJumperLifecycle(ICollection<string> failures)
    {
        var pagination = new SimplePagination
        {
            Total      = 50,
            IsReadOnly = true
        };

        using var realized = RealizeControl(pagination);
        Expect(PaginationCountVisualByTypeName(pagination, "QuickJumpEdit") == 0,
            "Read-only SimplePagination should not create QuickJumpEdit.",
            failures);

        var nextItem = FindVisualByName<PaginationNavItem>(pagination, "PART_NextNavItem");
        Expect(nextItem?.PaginationItemType == PaginationItemType.Next,
            $"SimplePagination next item should be PaginationItemType.Next. Actual: {nextItem?.PaginationItemType}.",
            failures);

        pagination.IsReadOnly = false;
        RefreshLayout(realized.Window);
        var quickJumper = FindVisualByName<QuickJumpEdit>(pagination, "PART_QuickJumper");
        Expect(quickJumper != null,
            "Editable SimplePagination should create QuickJumpEdit.",
            failures);
        Expect(quickJumper?.GetVisualParent() is Panel,
            "Editable SimplePagination QuickJumpEdit should be attached to the root panel.",
            failures);
        Expect(quickJumper is null || quickJumper.MinWidth > 0,
            "Editable SimplePagination QuickJumpEdit should keep template width styling.",
            failures);

        pagination.IsReadOnly = true;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<QuickJumpEdit>(pagination, "PART_QuickJumper") == null,
            "SimplePagination should remove QuickJumpEdit when switching back to read-only.",
            failures);
        Expect(quickJumper?.GetVisualParent() == null,
            "Removed SimplePagination QuickJumpEdit should not keep a visual parent.",
            failures);

        pagination.IsReadOnly = false;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<QuickJumpEdit>(pagination, "PART_QuickJumper") != null,
            "SimplePagination should recreate QuickJumpEdit after read-only toggle.",
            failures);
    }

    private static void VerifyPaginationFeatureLifecycle(ICollection<string> failures)
    {
        var pagination = new Pagination
        {
            Total             = 500,
            IsShowSizeChanger = true,
            IsShowQuickJumper = true
        };

        using var realized = RealizeControl(pagination);
        var sizeChanger = pagination.SizeChanger;
        var quickJumper = pagination.QuickJumperBar;
        Expect(sizeChanger != null,
            "Pagination should create SizeChanger when IsShowSizeChanger is true.",
            failures);
        Expect(quickJumper != null,
            "Pagination should create QuickJumperBar when IsShowQuickJumper is true.",
            failures);

        pagination.IsShowSizeChanger = false;
        pagination.IsShowQuickJumper = false;
        RefreshLayout(realized.Window);
        Expect(pagination.SizeChanger == null,
            "Pagination should release SizeChanger when IsShowSizeChanger is false.",
            failures);
        Expect(pagination.QuickJumperBar == null,
            "Pagination should release QuickJumperBar when IsShowQuickJumper is false.",
            failures);
        Expect(sizeChanger?.GetVisualParent() == null,
            "Released Pagination SizeChanger should not keep a visual parent.",
            failures);
        Expect(quickJumper?.GetVisualParent() == null,
            "Released Pagination QuickJumperBar should not keep a visual parent.",
            failures);

        pagination.IsShowSizeChanger = true;
        pagination.IsShowQuickJumper = true;
        RefreshLayout(realized.Window);
        Expect(pagination.SizeChanger != null,
            "Pagination should recreate SizeChanger after re-enabling IsShowSizeChanger.",
            failures);
        Expect(pagination.QuickJumperBar != null,
            "Pagination should recreate QuickJumperBar after re-enabling IsShowQuickJumper.",
            failures);
    }

    private static void VerifyPaginationNavItemSlots(ICollection<string> failures)
    {
        var pagination = new Pagination
        {
            Total       = 500,
            CurrentPage = 6
        };

        using var _ = RealizeControl(pagination);
        var pageItem = pagination.GetSelfAndVisualDescendants()
                                 .OfType<PaginationNavItem>()
                                 .FirstOrDefault(item => item.PaginationItemType == PaginationItemType.PageIndicator && item.PageNumber == 6);
        Expect(pageItem != null,
            "Pagination should realize the active page indicator.",
            failures);
        if (pageItem is not null)
        {
            Expect(FindVisualByName<IconPresenter>(pageItem, "IconPresenter") == null,
                "Page indicator item should not create an IconPresenter.",
                failures);
            Expect(FindVisualByName<Avalonia.Controls.TextBlock>(pageItem, "PART_ContentTextBlock") != null,
                "Page indicator item should create a text slot.",
                failures);
        }

        var previousItem = FindVisualByName<PaginationNavItem>(pagination, "PART_PreviousNavItem");
        previousItem ??= pagination.GetSelfAndVisualDescendants()
                                   .OfType<PaginationNavItem>()
                                   .FirstOrDefault(item => item.PaginationItemType == PaginationItemType.Previous);
        Expect(previousItem != null,
            "Pagination should realize the previous item.",
            failures);
        if (previousItem is not null)
        {
            Expect(FindVisualByName<IconPresenter>(previousItem, "IconPresenter") != null,
                "Previous item should create an IconPresenter.",
                failures);
            Expect(FindVisualByName<Avalonia.Controls.TextBlock>(previousItem, "PART_ContentTextBlock") == null,
                "Previous item should not create a text slot.",
                failures);
        }
    }

    private static void VerifyQuickJumperBarLineEditLifecycle(ICollection<string> failures)
    {
        var quickJumperBar = new QuickJumperBar();
        using (var _ = RealizeControl(quickJumperBar))
        {
            Expect(GetPrivateField(quickJumperBar, "AtomUI.Desktop.Controls.QuickJumperBar", "_lineEdit") != null,
                "QuickJumperBar should keep the active LineEdit while attached.",
                failures);
        }

        Expect(GetPrivateField(quickJumperBar, "AtomUI.Desktop.Controls.QuickJumperBar", "_lineEdit") == null,
            "QuickJumperBar should release LineEdit after detach.",
            failures);
    }

    private static int PaginationCountVisualByTypeName(Control root, string typeName)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Count(control => control.GetType().Name == typeName);
    }
}
