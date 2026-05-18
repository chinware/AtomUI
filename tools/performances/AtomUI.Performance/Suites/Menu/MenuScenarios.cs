using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace AtomUI.Performance;

using AtomContextMenu = AtomUI.Desktop.Controls.ContextMenu;
using AtomMenu = AtomUI.Desktop.Controls.Menu;
using AtomMenuFlyout = AtomUI.Desktop.Controls.MenuFlyout;
using AtomMenuItem = AtomUI.Desktop.Controls.MenuItem;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateMenuScenarios()
    {
        return
        [
            new PerfScenario("MenuItem.Leaf.NoIcon.NoGesture.Closed", _ => CreateMenuLeaf()),
            new PerfScenario("MenuItem.Leaf.Icon.Closed", _ => CreateMenuLeaf(icon: new CopyOutlined())),
            new PerfScenario("MenuItem.Leaf.InputGesture.Closed", _ => CreateMenuLeaf(inputGesture: "Ctrl+C")),
            new PerfScenario("MenuItem.Toggle.CheckBox.Closed", _ => CreateMenuLeaf(toggleType: MenuItemToggleType.CheckBox)),
            new PerfScenario("MenuItem.Toggle.Radio.Closed", _ => CreateMenuLeaf(toggleType: MenuItemToggleType.Radio, groupName: "Group1")),
            new PerfScenario("MenuItem.SubMenu.Closed", _ => CreateSubMenuItem()),
            new PerfScenario("Menu.TopLevel.Basic.Closed", _ => CreateBasicMenu()),
            new PerfScenario("ContextMenu.Attached.Closed", _ => CreateContextMenuHost()),
            new PerfScenario("MenuFlyout.Attached.Closed", _ => CreateMenuFlyoutHost()),
            new PerfScenario("Menu.GalleryMenuOnlyShape", _ => CreateMenuGalleryMenuOnlyShape())
        ];
    }

    private static AtomMenuItem CreateMenuLeaf(PathIcon? icon = null,
                                               string? inputGesture = null,
                                               MenuItemToggleType toggleType = MenuItemToggleType.None,
                                               string? groupName = null)
    {
        return new AtomMenuItem
        {
            Header       = "Menu Item",
            Icon         = icon,
            InputGesture = inputGesture is null ? null : KeyGesture.Parse(inputGesture),
            ToggleType   = toggleType,
            GroupName    = groupName
        };
    }

    private static AtomMenuItem CreateSubMenuItem()
    {
        var item = new AtomMenuItem
        {
            Header = "_File"
        };
        item.Items.Add(CreateMenuLeaf(inputGesture: "Ctrl+N"));
        item.Items.Add(CreateMenuLeaf(inputGesture: "Ctrl+S"));
        item.Items.Add(CreateMenuLeaf());
        return item;
    }

    private static AtomMenu CreateBasicMenu()
    {
        var menu = new AtomMenu();
        menu.Items.Add(CreateTopLevelMenu("_File", 8));
        menu.Items.Add(CreateTopLevelMenu("_Edit", 6));
        menu.Items.Add(new AtomMenuItem
        {
            Header    = "Disabled Item",
            IsEnabled = false
        });
        return menu;
    }

    private static AtomMenuItem CreateTopLevelMenu(string header, int count)
    {
        var item = new AtomMenuItem
        {
            Header = header
        };
        for (var i = 0; i < count; i++)
        {
            item.Items.Add(CreateMenuLeaf(inputGesture: i % 2 == 0 ? "Ctrl+N" : null));
        }
        return item;
    }

    private static Control CreateContextMenuHost()
    {
        var host = new Border
        {
            Width  = 240,
            Height = 32,
            Child  = new AtomTextBlock
            {
                Text = "Right Click to show Context Menu"
            }
        };

        var contextMenu = new AtomContextMenu();
        contextMenu.Items.Add(CreateMenuLeaf(icon: new ScissorOutlined(), inputGesture: "Ctrl+X"));
        contextMenu.Items.Add(CreateMenuLeaf(icon: new CopyOutlined(), inputGesture: "Ctrl+C"));
        contextMenu.Items.Add(CreateSubMenuItem());
        host.ContextMenu = contextMenu;
        return host;
    }

    private static Control CreateMenuFlyoutHost()
    {
        var host = new Border
        {
            Width  = 240,
            Height = 32,
            Child  = new AtomTextBlock
            {
                Text = "Right Click to show Context Flyout"
            }
        };

        var flyout = new AtomMenuFlyout();
        flyout.Items.Add(CreateMenuLeaf(icon: new ScissorOutlined(), inputGesture: "Ctrl+X"));
        flyout.Items.Add(CreateMenuLeaf(icon: new CopyOutlined(), inputGesture: "Ctrl+C"));
        flyout.Items.Add(CreateSubMenuItem());
        host.ContextFlyout = flyout;
        return host;
    }

    private static Control CreateMenuGalleryMenuOnlyShape()
    {
        var root = new StackPanel
        {
            Spacing = 12
        };

        root.Children.Add(CreateBasicMenu());
        root.Children.Add(CreateBasicMenuWithIcons());
        root.Children.Add(CreateToggleMenu());
        root.Children.Add(CreateScrollableMenu());
        root.Children.Add(CreateContextMenuHost());
        root.Children.Add(CreateMenuFlyoutHost());
        return root;
    }

    private static AtomMenu CreateBasicMenuWithIcons()
    {
        var menu = new AtomMenu();
        var file = CreateTopLevelMenu("_File", 8);
        var edit = new AtomMenuItem
        {
            Header = "_Edit"
        };
        edit.Items.Add(CreateMenuLeaf(inputGesture: "Ctrl+Z"));
        edit.Items.Add(CreateMenuLeaf(icon: new ScissorOutlined(), inputGesture: "Ctrl+X"));
        edit.Items.Add(CreateMenuLeaf(icon: new CopyOutlined(), inputGesture: "Ctrl+C"));
        edit.Items.Add(CreateMenuLeaf(icon: new DeleteOutlined(), inputGesture: "Ctrl+D"));
        edit.Items.Add(CreateSubMenuItem());
        menu.Items.Add(file);
        menu.Items.Add(edit);
        return menu;
    }

    private static AtomMenu CreateToggleMenu()
    {
        var menu = new AtomMenu();
        var item = new AtomMenuItem
        {
            Header = "_Menu A"
        };
        item.Items.Add(CreateMenuLeaf(inputGesture: "Ctrl+N", toggleType: MenuItemToggleType.Radio, groupName: "Group1"));
        item.Items.Add(CreateMenuLeaf(inputGesture: "Ctrl+Alt+N", toggleType: MenuItemToggleType.Radio, groupName: "Group1"));
        item.Items.Add(CreateMenuLeaf(inputGesture: "Ctrl+S", toggleType: MenuItemToggleType.CheckBox));
        item.Items.Add(CreateMenuLeaf(icon: new GithubOutlined(), inputGesture: "Ctrl+Shift+S", toggleType: MenuItemToggleType.CheckBox));
        item.Items.Add(CreateMenuLeaf());
        menu.Items.Add(item);
        return menu;
    }

    private static AtomMenu CreateScrollableMenu()
    {
        var menu = new AtomMenu();
        var item = new AtomMenuItem
        {
            Header = "_Menu"
        };
        for (var i = 0; i < 36; i++)
        {
            item.Items.Add(CreateMenuLeaf());
        }
        menu.Items.Add(item);
        return menu;
    }
}
