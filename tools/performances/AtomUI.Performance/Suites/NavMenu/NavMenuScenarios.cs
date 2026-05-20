using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateNavMenuScenarios()
    {
        return
        [
            new PerfScenario("NavMenu.Inline.Leaf.NoIcon.Closed", _ => CreateNavMenuLeaf(NavMenuMode.Inline)),
            new PerfScenario("NavMenu.Inline.Leaf.Icon.Closed", _ => CreateNavMenuLeaf(NavMenuMode.Inline, withIcon: true)),
            new PerfScenario("NavMenu.Inline.Submenu.Closed", _ => CreateNavMenuWithSubmenu(NavMenuMode.Inline)),
            new PerfScenario("NavMenu.Inline.Submenu.DefaultOpenPath", _ => CreateNavMenuWithDefaultOpenPath()),
            new PerfScenario("NavMenu.Vertical.Submenu.Closed", _ => CreateNavMenuWithSubmenu(NavMenuMode.Vertical)),
            new PerfScenario("NavMenu.Horizontal.TopLevel.Closed", _ => CreateNavMenuWithSubmenu(NavMenuMode.Horizontal)),
            new PerfScenario("NavMenu.WorkspaceNavigationShape", _ => CreateWorkspaceNavigationShape()),
            new PerfScenario("NavMenu.MenuShowCaseShape", _ => CreateMenuShowCaseNavMenuShape())
        ];
    }

    private static NavMenu CreateNavMenuLeaf(NavMenuMode mode, bool withIcon = false)
    {
        var menu = CreateNavMenu(mode);
        menu.Items.Add(CreateNavNode("Navigation One", "1", withIcon ? new MailOutlined() : null));
        menu.Items.Add(CreateNavNode("Navigation Two", "2"));
        menu.Items.Add(CreateNavNode("Navigation Three", "3"));
        return menu;
    }

    private static NavMenu CreateNavMenuWithSubmenu(NavMenuMode mode)
    {
        var menu = CreateNavMenu(mode);
        menu.Items.Add(CreateNavNode("Navigation One", "1", new MailOutlined()));
        menu.Items.Add(CreateNavNode("Navigation Two", "2", new AppstoreOutlined(), isEnabled: false));
        menu.Items.Add(CreateNavSubmenuNode("Navigation Three - Submenu", "3", new SettingOutlined()));
        menu.Items.Add(CreateNavNode("Navigation Four", "4"));
        return menu;
    }

    private static NavMenu CreateNavMenuWithDefaultOpenPath()
    {
        var menu = CreateNavMenuWithSubmenu(NavMenuMode.Inline);
        menu.DefaultOpenPaths =
        [
            new TreeNodePath(["3", "SubGroup2"])
        ];
        menu.DefaultSelectedPath = new TreeNodePath(["3", "SubGroup1", "Option1"]);
        return menu;
    }

    private static Control CreateWorkspaceNavigationShape()
    {
        var root = new StackPanel
        {
            Spacing = 8
        };
        root.Children.Add(CreateNavMenuWithSubmenu(NavMenuMode.Inline));
        root.Children.Add(CreateNavMenuWithSubmenu(NavMenuMode.Inline));
        root.Children.Add(CreateNavMenuWithSubmenu(NavMenuMode.Inline));
        return root;
    }

    private static Control CreateMenuShowCaseNavMenuShape()
    {
        var root = new StackPanel
        {
            Spacing = 12
        };

        var itemsSourceInline = CreateNavMenu(NavMenuMode.Inline);
        itemsSourceInline.ItemsSource = CreateNavMenuNodes();
        root.Children.Add(itemsSourceInline);

        root.Children.Add(CreateNavMenuWithSubmenu(NavMenuMode.Vertical));
        root.Children.Add(CreateNavMenuWithSubmenu(NavMenuMode.Inline));
        root.Children.Add(CreateNavMenuWithSubmenu(NavMenuMode.Horizontal));

        var darkHorizontal = CreateNavMenuWithSubmenu(NavMenuMode.Horizontal);
        darkHorizontal.IsDarkStyle = true;
        root.Children.Add(darkHorizontal);

        root.Children.Add(CreateNavMenuWithSubmenu(NavMenuMode.Inline));
        root.Children.Add(CreateNavMenuWithDefaultOpenPath());

        var selectedInline = CreateNavMenu(NavMenuMode.Inline);
        var selectedNodes  = CreateNavMenuNodes();
        selectedInline.ItemsSource          = selectedNodes;
        selectedInline.DefaultSelectedPath  = new TreeNodePath(["3", "SubGroup1", "Option1"]);
        selectedInline.SelectedItem         = ((NavMenuNode)selectedNodes[2].Children[0]).Children[0];
        root.Children.Add(selectedInline);

        return root;
    }

    private static NavMenu CreateNavMenu(NavMenuMode mode)
    {
        return new NavMenu
        {
            Mode  = mode,
            Width = mode == NavMenuMode.Horizontal ? double.NaN : 300
        };
    }

    private static List<NavMenuNode> CreateNavMenuNodes()
    {
        return
        [
            CreateNavNode("Navigation One", "1", new MailOutlined()),
            CreateNavNode("Navigation Two", "2", new AppstoreOutlined(), isEnabled: false),
            CreateNavSubmenuNode("Navigation Three - Submenu", "3", new SettingOutlined()),
            CreateNavNode("Navigation Four", "4")
        ];
    }

    private static NavMenuNode CreateNavSubmenuNode(string header, EntityKey itemKey, PathIcon? icon)
    {
        var node = CreateNavNode(header, itemKey, icon);
        var item1 = CreateNavNode("Item 1", "SubGroup1");
        item1.Children.Add(CreateNavNode("Option 1", "Option1"));
        item1.Children.Add(CreateNavNode("Option 2", "Option2"));

        var item2 = CreateNavNode("Item 2", "SubGroup2");
        item2.Children.Add(CreateNavNode("Option 3", "Option3"));
        item2.Children.Add(CreateNavNode("Option 4", "Option4"));

        node.Children.Add(item1);
        node.Children.Add(item2);
        return node;
    }

    private static NavMenuNode CreateNavNode(string header,
                                             EntityKey itemKey,
                                             PathIcon? icon = null,
                                             bool isEnabled = true)
    {
        return new NavMenuNode
        {
            Header    = header,
            ItemKey   = itemKey,
            Icon      = icon,
            IsEnabled = isEnabled
        };
    }
}
