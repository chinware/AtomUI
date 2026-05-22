using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateBreadcrumbScenarios()
    {
        return
        [
            new PerfScenario("Breadcrumb.Basic.Items4", _ => CreateBasicBreadcrumb()),
            new PerfScenario("Breadcrumb.Icon.Items3", _ => CreateIconBreadcrumb()),
            new PerfScenario("Breadcrumb.Navigate.Items2", _ => CreateNavigateBreadcrumb()),
            new PerfScenario("Breadcrumb.CustomSeparator.Items4", _ => CreateCustomSeparatorBreadcrumb()),
            new PerfScenario("Breadcrumb.ItemSeparator.Items4", _ => CreateItemSeparatorBreadcrumb()),
            new PerfScenario("Breadcrumb.DataTemplate.Items4", _ => CreateTemplatedBreadcrumb()),
            new PerfScenario("Breadcrumb.GalleryShape", _ => CreateBreadcrumbGalleryShape())
        ];
    }

    private static Breadcrumb CreateBasicBreadcrumb()
    {
        var breadcrumb = new Breadcrumb();
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Home" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Application Center", NavigateContext = "#" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Application List", NavigateContext = "#" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "An Application" });
        return breadcrumb;
    }

    private static Breadcrumb CreateIconBreadcrumb()
    {
        var breadcrumb = new Breadcrumb();
        breadcrumb.Items.Add(new BreadcrumbItem { Icon = new HomeOutlined() });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Application List", Icon = new UserOutlined(), NavigateContext = "#" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Application" });
        return breadcrumb;
    }

    private static Breadcrumb CreateNavigateBreadcrumb()
    {
        var breadcrumb = new Breadcrumb();
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Users" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Param", NavigateContext = "Param(1)" });
        return breadcrumb;
    }

    private static Breadcrumb CreateCustomSeparatorBreadcrumb()
    {
        var breadcrumb = CreateBasicBreadcrumb();
        breadcrumb.Separator = ">";
        return breadcrumb;
    }

    private static Breadcrumb CreateItemSeparatorBreadcrumb()
    {
        var breadcrumb = new Breadcrumb();
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Location", Separator = ":" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Application Center", NavigateContext = "#" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "Application List", NavigateContext = "#" });
        breadcrumb.Items.Add(new BreadcrumbItem { Content = "An Application" });
        return breadcrumb;
    }

    private static Breadcrumb CreateTemplatedBreadcrumb()
    {
        return new Breadcrumb
        {
            ItemTemplate = new FuncDataTemplate<BreadcrumbItemData>((item, _) =>
                new Avalonia.Controls.TextBlock
                {
                    Text = item?.Content?.ToString()
                }),
            ItemsSource = new BreadcrumbItemData[]
            {
                new BreadcrumbItemData { Content = "Location", Separator = ":" },
                new BreadcrumbItemData { Content = "Application Center", NavigateContext = "#" },
                new BreadcrumbItemData { Content = "Application List", NavigateContext = "#" },
                new BreadcrumbItemData { Content = "An Application" }
            }
        };
    }

    private static Control CreateBreadcrumbGalleryShape()
    {
        return new StackPanel
        {
            Spacing = 12,
            Children =
            {
                CreateBasicBreadcrumb(),
                CreateIconBreadcrumb(),
                CreateNavigateBreadcrumb(),
                CreateCustomSeparatorBreadcrumb(),
                CreateItemSeparatorBreadcrumb(),
                CreateTemplatedBreadcrumb()
            }
        };
    }
}
