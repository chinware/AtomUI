using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunBreadcrumbStateVerification()
    {
        var failures = new List<string>();
        VerifyBreadcrumbLastItemLifecycle(failures);
        VerifyBreadcrumbSeparatorPropagation(failures);
        VerifyBreadcrumbDataItemSeparatorPrecedence(failures);
        VerifyBreadcrumbNavigateResponsiveState(failures);
        VerifyBreadcrumbTemplateContract(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Breadcrumb state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Breadcrumb state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyBreadcrumbLastItemLifecycle(ICollection<string> failures)
    {
        var first  = new BreadcrumbItem { Content = "Home" };
        var second = new BreadcrumbItem { Content = "List" };
        var third  = new BreadcrumbItem { Content = "Detail" };
        var breadcrumb = new Breadcrumb();
        breadcrumb.Items.Add(first);
        breadcrumb.Items.Add(second);

        using var realized = RealizeControl(breadcrumb);
        Expect(!first.IsLast && second.IsLast,
            $"Breadcrumb should mark the second item as last after initial realization, actual first={first.IsLast}, second={second.IsLast}.",
            failures);

        breadcrumb.Items.Add(third);
        RefreshLayout(realized.Window);
        Expect(!first.IsLast && !second.IsLast && third.IsLast,
            $"Breadcrumb should move IsLast to the appended item, actual first={first.IsLast}, second={second.IsLast}, third={third.IsLast}.",
            failures);

        breadcrumb.Items.Remove(third);
        RefreshLayout(realized.Window);
        var currentItems = GetBreadcrumbItems(breadcrumb);
        Expect(currentItems.Count == 2 && currentItems[0] == first && currentItems[1] == second,
            $"Breadcrumb should keep only the remaining two realized items after removal, actual count={currentItems.Count}.",
            failures);
        Expect(!first.IsLast && second.IsLast,
            $"Breadcrumb should restore IsLast to the previous realized item after removal, actual first={first.IsLast}, second={second.IsLast}.",
            failures);
    }

    private static void VerifyBreadcrumbSeparatorPropagation(ICollection<string> failures)
    {
        var inherited = new BreadcrumbItem { Content = "Home" };
        var explicitItem = new BreadcrumbItem { Content = "Detail", Separator = ":" };
        var breadcrumb = new Breadcrumb
        {
            Separator = "/"
        };
        breadcrumb.Items.Add(inherited);
        breadcrumb.Items.Add(explicitItem);

        using var realized = RealizeControl(breadcrumb);
        Expect(Equals(inherited.Separator, "/"),
            $"Breadcrumb direct item should receive initial parent separator, actual {inherited.Separator ?? "<null>"}.",
            failures);
        Expect(Equals(explicitItem.Separator, ":"),
            $"Breadcrumb direct item explicit separator should win initially, actual {explicitItem.Separator ?? "<null>"}.",
            failures);

        breadcrumb.Separator = ">";
        RefreshLayout(realized.Window);
        Expect(Equals(inherited.Separator, ">"),
            $"Breadcrumb direct inherited item should follow parent Separator changes, actual {inherited.Separator ?? "<null>"}.",
            failures);
        Expect(Equals(explicitItem.Separator, ":"),
            $"Breadcrumb direct explicit item separator should survive parent changes, actual {explicitItem.Separator ?? "<null>"}.",
            failures);
    }

    private static void VerifyBreadcrumbDataItemSeparatorPrecedence(ICollection<string> failures)
    {
        var dataItems = new[]
        {
            new BreadcrumbItemData { Content = "Location", Separator = ":" },
            new BreadcrumbItemData { Content = "Application Center", NavigateContext = "#" },
            new BreadcrumbItemData { Content = "Application List", NavigateContext = "#" }
        };
        var breadcrumb = new Breadcrumb
        {
            Separator  = "/",
            ItemsSource = dataItems
        };

        using var realized = RealizeControl(breadcrumb);
        var items = GetBreadcrumbItems(breadcrumb);
        Expect(items.Count == 3,
            $"Breadcrumb should realize three generated items, actual {items.Count}.",
            failures);
        if (items.Count < 3)
        {
            return;
        }

        Expect(Equals(items[0].Separator, ":"),
            $"Breadcrumb generated item data separator should win initially, actual {items[0].Separator ?? "<null>"}.",
            failures);
        Expect(Equals(items[1].Separator, "/"),
            $"Breadcrumb generated item should receive initial parent separator, actual {items[1].Separator ?? "<null>"}.",
            failures);

        breadcrumb.Separator = ">";
        RefreshLayout(realized.Window);
        Expect(Equals(items[0].Separator, ":"),
            $"Breadcrumb generated item data separator should survive parent changes, actual {items[0].Separator ?? "<null>"}.",
            failures);
        Expect(Equals(items[1].Separator, ">") && Equals(items[2].Separator, ">"),
            $"Breadcrumb generated inherited separators should follow parent changes, actual second={items[1].Separator ?? "<null>"}, third={items[2].Separator ?? "<null>"}.",
            failures);
    }

    private static void VerifyBreadcrumbNavigateResponsiveState(ICollection<string> failures)
    {
        var item = new BreadcrumbItem();
        using var realized = RealizeControl(new Breadcrumb { Items = { item } });
        Expect(!item.IsNavigateResponsive,
            "Breadcrumb item without NavigateUri or NavigateContext should not be navigate responsive.",
            failures);

        item.NavigateContext = "#";
        RefreshLayout(realized.Window);
        Expect(item.IsNavigateResponsive,
            "Breadcrumb item with NavigateContext should become navigate responsive.",
            failures);

        item.NavigateContext = null;
        RefreshLayout(realized.Window);
        Expect(!item.IsNavigateResponsive,
            "Breadcrumb item should stop being navigate responsive after NavigateContext is cleared.",
            failures);

        item.NavigateUri = new Uri("https://atomui.net");
        RefreshLayout(realized.Window);
        Expect(item.IsNavigateResponsive,
            "Breadcrumb item with NavigateUri should become navigate responsive.",
            failures);
    }

    private static void VerifyBreadcrumbTemplateContract(ICollection<string> failures)
    {
        var breadcrumb = new Breadcrumb
        {
            ItemTemplate = new FuncDataTemplate<BreadcrumbItemData>((item, _) =>
                new Avalonia.Controls.TextBlock
                {
                    Text = item?.Content?.ToString()
                }),
            ItemsSource = new BreadcrumbItemData[]
            {
                new BreadcrumbItemData { Content = "Home" },
                new BreadcrumbItemData { Content = "Detail" }
            }
        };

        using var _ = RealizeControl(breadcrumb);
        var items = GetBreadcrumbItems(breadcrumb);
        Expect(items.Count == 2,
            $"Templated Breadcrumb should realize two generated BreadcrumbItem containers, actual {items.Count}.",
            failures);
        Expect(items.All(item => FindVisualByName<ContentPresenter>(item, "Content") != null),
            "Templated Breadcrumb items should keep the template Content presenter.",
            failures);
        Expect(items.All(item => FindVisualByName<ContentPresenter>(item, "Separator") != null),
            "Templated Breadcrumb items should keep the template Separator presenter.",
            failures);
        Expect(items.LastOrDefault()?.IsLast == true,
            "Templated Breadcrumb should mark the generated final item as last.",
            failures);
    }

    private static IReadOnlyList<BreadcrumbItem> GetBreadcrumbItems(Breadcrumb breadcrumb)
    {
        return breadcrumb.GetSelfAndVisualDescendants()
                         .OfType<BreadcrumbItem>()
                         .ToList();
    }
}
