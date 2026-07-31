using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuKeyboardNavigationTests
{
    static NavMenuKeyboardNavigationTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Inline_Arrow_Keys_Move_Keyboard_Active_Item_Without_Selecting_Until_Enter()
    {
        var first = new NavMenuNode
        {
            Header  = "First",
            ItemKey = "first"
        };
        var second = new NavMenuNode
        {
            Header  = "Second",
            ItemKey = "second"
        };
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(first);
        menu.Items.Add(second);

        var selectedNodes = new List<INavMenuNode>();
        var clickedItems  = new List<INavMenuItem>();
        menu.NavMenuNodeSelected += (_, args) => selectedNodes.Add(args.NavMenuNode);
        menu.NavMenuItemClick    += (_, args) => clickedItems.Add(args.NavMenuItem);

        ShowInWindow(menu, window =>
        {
            var firstContainer  = (NavMenuItem)menu.ContainerFromItem(first)!;
            var secondContainer = (NavMenuItem)menu.ContainerFromItem(second)!;

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue(
                "NavMenu itself must be focusable so keyboard navigation can start before any item is selected.");

            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(firstContainer).ShouldBeTrue();
            IsKeyboardActive(secondContainer).ShouldBeFalse();
            menu.SelectedItem.ShouldBeNull();
            selectedNodes.ShouldBeEmpty();
            clickedItems.ShouldBeEmpty();

            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(firstContainer).ShouldBeFalse();
            IsKeyboardActive(secondContainer).ShouldBeTrue();
            menu.SelectedItem.ShouldBeNull();
            selectedNodes.ShouldBeEmpty();
            clickedItems.ShouldBeEmpty();

            PressKey(window, Key.Enter, PhysicalKey.Enter);
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem.ShouldBeSameAs(second);
            selectedNodes.ShouldBe([second]);
            clickedItems.Count.ShouldBe(1);
        });
    }

    [Fact]
    public void Inline_Initial_Keyboard_Active_Starts_From_First_Item_When_No_Item_Is_Selected()
    {
        var first = new NavMenuNode
        {
            Header  = "First",
            ItemKey = "first"
        };
        var second = new NavMenuNode
        {
            Header  = "Second",
            ItemKey = "second"
        };
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(first);
        menu.Items.Add(second);

        ShowInWindow(menu, window =>
        {
            var firstContainer  = (NavMenuItem)menu.ContainerFromItem(first)!;
            var secondContainer = (NavMenuItem)menu.ContainerFromItem(second)!;

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();
            PressKey(window, Key.Up, PhysicalKey.ArrowUp);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(firstContainer).ShouldBeTrue(
                "When no item is selected, the first keyboard movement should initialize active state from the first visible item, independent of direction.");
            IsKeyboardActive(secondContainer).ShouldBeFalse();
            menu.SelectedItem.ShouldBeNull();
        });
    }

    [Fact]
    public void Inline_First_Down_Key_Moves_From_Selected_Item_To_Next_Item()
    {
        var first = new NavMenuNode
        {
            Header  = "First",
            ItemKey = "first"
        };
        var second = new NavMenuNode
        {
            Header  = "Second",
            ItemKey = "second"
        };
        var third = new NavMenuNode
        {
            Header  = "Third",
            ItemKey = "third"
        };
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(first);
        menu.Items.Add(second);
        menu.Items.Add(third);

        var selectedNodes = new List<INavMenuNode>();
        var clickedItems  = new List<INavMenuItem>();
        menu.NavMenuNodeSelected += (_, args) => selectedNodes.Add(args.NavMenuNode);
        menu.NavMenuItemClick    += (_, args) => clickedItems.Add(args.NavMenuItem);

        ShowInWindow(menu, window =>
        {
            var firstContainer  = (NavMenuItem)menu.ContainerFromItem(first)!;
            var secondContainer = (NavMenuItem)menu.ContainerFromItem(second)!;
            var thirdContainer  = (NavMenuItem)menu.ContainerFromItem(third)!;

            menu.InteractionHandler.ShouldNotBeNull();
            menu.InteractionHandler.Select(secondContainer);
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem.ShouldBeSameAs(second);
            selectedNodes.Clear();
            clickedItems.Clear();

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(firstContainer).ShouldBeFalse();
            IsKeyboardActive(secondContainer).ShouldBeFalse(
                "The selected item is only the initial keyboard navigation anchor; the first Down key must still move to the next item.");
            IsKeyboardActive(thirdContainer).ShouldBeTrue();
            menu.SelectedItem.ShouldBeSameAs(second);
            selectedNodes.ShouldBeEmpty();
            clickedItems.ShouldBeEmpty();
        });
    }

    [Fact]
    public void Inline_First_Up_Key_Moves_From_Selected_Item_To_Previous_Item()
    {
        var first = new NavMenuNode
        {
            Header  = "First",
            ItemKey = "first"
        };
        var second = new NavMenuNode
        {
            Header  = "Second",
            ItemKey = "second"
        };
        var third = new NavMenuNode
        {
            Header  = "Third",
            ItemKey = "third"
        };
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(first);
        menu.Items.Add(second);
        menu.Items.Add(third);

        ShowInWindow(menu, window =>
        {
            var firstContainer  = (NavMenuItem)menu.ContainerFromItem(first)!;
            var secondContainer = (NavMenuItem)menu.ContainerFromItem(second)!;
            var thirdContainer  = (NavMenuItem)menu.ContainerFromItem(third)!;

            menu.InteractionHandler.ShouldNotBeNull();
            menu.InteractionHandler.Select(secondContainer);
            Dispatcher.UIThread.RunJobs();

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();
            PressKey(window, Key.Up, PhysicalKey.ArrowUp);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(firstContainer).ShouldBeTrue();
            IsKeyboardActive(secondContainer).ShouldBeFalse(
                "The selected item is only the initial keyboard navigation anchor; the first Up key must still move to the previous item.");
            IsKeyboardActive(thirdContainer).ShouldBeFalse();
            menu.SelectedItem.ShouldBeSameAs(second);
        });
    }

    [Fact]
    public void Inline_Left_Key_Collapses_Active_Submenu_Without_Moving_Keyboard_Active_Item()
    {
        var option = new NavMenuNode
        {
            Header  = "Option",
            ItemKey = "option"
        };
        var child = new NavMenuNode
        {
            Header  = "Child",
            ItemKey = "child"
        };
        child.Children.Add(option);
        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(child);

        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);

        ShowInWindow(menu, window =>
        {
            var parentContainer = (NavMenuItem)menu.ContainerFromItem(parent)!;
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();

            var childContainer = (NavMenuItem)parentContainer.ContainerFromItem(child)!;
            childContainer.Open();
            Dispatcher.UIThread.RunJobs();

            var optionContainer = (NavMenuItem)childContainer.ContainerFromItem(option)!;

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(childContainer).ShouldBeTrue();
            childContainer.IsSubMenuOpen.ShouldBeTrue();

            PressKey(window, Key.Left, PhysicalKey.ArrowLeft);
            RunDispatcherJobsUntil(() => !childContainer.IsSubMenuOpen);

            childContainer.IsSubMenuOpen.ShouldBeFalse();
            IsKeyboardActive(parentContainer).ShouldBeFalse();
            IsKeyboardActive(childContainer).ShouldBeTrue(
                "Inline Left should collapse the active submenu itself instead of moving active state to its parent.");
            IsKeyboardActive(optionContainer).ShouldBeFalse();
            menu.SelectedItem.ShouldBeNull();
        });
    }

    [Fact]
    public void Inline_Right_Key_Expands_Active_Submenu_Without_Moving_Keyboard_Active_Item()
    {
        var option = new NavMenuNode
        {
            Header  = "Option",
            ItemKey = "option"
        };
        var child = new NavMenuNode
        {
            Header  = "Child",
            ItemKey = "child"
        };
        child.Children.Add(option);
        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(child);

        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);

        ShowInWindow(menu, window =>
        {
            var parentContainer = (NavMenuItem)menu.ContainerFromItem(parent)!;
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();

            var childContainer = (NavMenuItem)parentContainer.ContainerFromItem(child)!;

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(childContainer).ShouldBeTrue();
            childContainer.IsSubMenuOpen.ShouldBeFalse();

            PressKey(window, Key.Right, PhysicalKey.ArrowRight);
            RunDispatcherJobsUntil(() => childContainer.IsSubMenuOpen);

            var optionContainer = (NavMenuItem)childContainer.ContainerFromItem(option)!;
            childContainer.IsSubMenuOpen.ShouldBeTrue();
            IsKeyboardActive(parentContainer).ShouldBeFalse();
            IsKeyboardActive(childContainer).ShouldBeTrue(
                "Inline Right should expand the active submenu itself instead of moving active state to the first child.");
            IsKeyboardActive(optionContainer).ShouldBeFalse();
            menu.SelectedItem.ShouldBeNull();
        });
    }

    [Theory]
    [InlineData(Key.Left, PhysicalKey.ArrowLeft)]
    [InlineData(Key.Right, PhysicalKey.ArrowRight)]
    public void Inline_Left_And_Right_Keys_Do_Not_Move_Active_Leaf_Item(Key key, PhysicalKey physicalKey)
    {
        var option = new NavMenuNode
        {
            Header  = "Option",
            ItemKey = "option"
        };
        var child = new NavMenuNode
        {
            Header  = "Child",
            ItemKey = "child"
        };
        child.Children.Add(option);
        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(child);

        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);

        ShowInWindow(menu, window =>
        {
            var parentContainer = (NavMenuItem)menu.ContainerFromItem(parent)!;
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();

            var childContainer = (NavMenuItem)parentContainer.ContainerFromItem(child)!;
            childContainer.Open();
            Dispatcher.UIThread.RunJobs();

            var optionContainer = (NavMenuItem)childContainer.ContainerFromItem(option)!;

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(optionContainer).ShouldBeTrue();
            childContainer.IsSubMenuOpen.ShouldBeTrue();

            PressKey(window, key, physicalKey);
            Dispatcher.UIThread.RunJobs();

            childContainer.IsSubMenuOpen.ShouldBeTrue();
            IsKeyboardActive(parentContainer).ShouldBeFalse();
            IsKeyboardActive(childContainer).ShouldBeFalse();
            IsKeyboardActive(optionContainer).ShouldBeTrue(
                "Inline Left/Right should only expand or collapse the active item when it has a submenu; leaf active items should not move.");
            menu.SelectedItem.ShouldBeNull();
        });
    }

    [Fact]
    public void Inline_Selected_Path_Parent_Shows_Keyboard_Active_Background()
    {
        var option1 = new NavMenuNode
        {
            Header  = "Option 1",
            ItemKey = "option-1"
        };
        var option2 = new NavMenuNode
        {
            Header  = "Option 2",
            ItemKey = "option-2"
        };
        var item1 = new NavMenuNode
        {
            Header  = "Item 1",
            ItemKey = "item-1"
        };
        item1.Children.Add(option1);
        item1.Children.Add(option2);
        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(item1);

        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);

        ShowInWindow(menu, window =>
        {
            var parentContainer = (NavMenuItem)menu.ContainerFromItem(parent)!;
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();

            var item1Container = (NavMenuItem)parentContainer.ContainerFromItem(item1)!;
            item1Container.Open();
            Dispatcher.UIThread.RunJobs();

            var option2Container = (NavMenuItem)item1Container.ContainerFromItem(option2)!;
            menu.InteractionHandler.ShouldNotBeNull();
            menu.InteractionHandler.Select(option2Container);
            Dispatcher.UIThread.RunJobs();

            var item1Header = GetItemHeader(item1Container);
            item1Header.IsInSelectedPath.ShouldBeTrue();

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();
            PressKey(window, Key.Up, PhysicalKey.ArrowUp);
            PressKey(window, Key.Up, PhysicalKey.ArrowUp);
            Dispatcher.UIThread.RunJobs();

            item1Container.IsKeyboardActive.ShouldBeTrue();
            item1Header.IsKeyboardActive.ShouldBeTrue();
            GetSolidBrushColor(item1Header.Background).ShouldNotBe(
                Colors.Transparent,
                "A selected-path submenu title must still show keyboard active background; selected path should keep text color, not suppress active feedback.");
        });
    }

    [Fact]
    public void Popup_Escape_Closes_Open_Submenu_Without_Clearing_Selected_Item()
    {
        var selected = new NavMenuNode
        {
            Header  = "Selected",
            ItemKey = "selected"
        };
        var child = new NavMenuNode
        {
            Header  = "Child",
            ItemKey = "child"
        };
        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(child);

        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Vertical,
            IsMotionEnabled = false
        };
        menu.Items.Add(selected);
        menu.Items.Add(parent);

        ShowInWindow(CreatePopupOverlayHost(menu), window =>
        {
            var selectedContainer = (NavMenuItem)menu.ContainerFromItem(selected)!;
            var parentContainer   = (NavMenuItem)menu.ContainerFromItem(parent)!;

            menu.InteractionHandler.ShouldNotBeNull();
            menu.InteractionHandler.Select(selectedContainer);
            Dispatcher.UIThread.RunJobs();

            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();
            parentContainer.IsSubMenuOpen.ShouldBeTrue();
            menu.SelectedItem.ShouldBeSameAs(selected);

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue(
                "NavMenu itself must be focusable so Escape can close an open keyboard popup branch.");
            PressKey(window, Key.Escape, PhysicalKey.Escape);
            RunDispatcherJobsUntil(() => !parentContainer.IsSubMenuOpen);

            parentContainer.IsSubMenuOpen.ShouldBeFalse();
            menu.SelectedItem.ShouldBeSameAs(selected);
            selectedContainer.IsSelected.ShouldBeTrue();
        });
    }

    [Fact]
    public void Horizontal_Down_Key_Enters_Submenu_And_Enter_Commits_Leaf()
    {
        var child = new NavMenuNode
        {
            Header  = "Child",
            ItemKey = "child"
        };
        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(child);

        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Horizontal,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);

        var selectedNodes = new List<INavMenuNode>();
        menu.NavMenuNodeSelected += (_, args) => selectedNodes.Add(args.NavMenuNode);

        ShowInWindow(CreatePopupOverlayHost(menu), window =>
        {
            var parentContainer = (NavMenuItem)menu.ContainerFromItem(parent)!;

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();

            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(parentContainer).ShouldBeTrue();
            parentContainer.IsSubMenuOpen.ShouldBeFalse();
            menu.SelectedItem.ShouldBeNull();

            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            parentContainer.IsSubMenuOpen.ShouldBeTrue();
            var childContainer = (NavMenuItem)parentContainer.ContainerFromItem(child)!;
            IsKeyboardActive(parentContainer).ShouldBeFalse();
            IsKeyboardActive(childContainer).ShouldBeTrue();
            menu.SelectedItem.ShouldBeNull(
                "Entering a submenu with the keyboard must only move keyboard active state; it must not commit selection.");

            PressKey(window, Key.Enter, PhysicalKey.Enter);
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem.ShouldBeSameAs(child);
            selectedNodes.ShouldBe([child]);
        });
    }

    [Fact]
    public void Keyboard_Active_State_Is_Propagated_To_Header()
    {
        var item = new NavMenuNode
        {
            Header  = "Item",
            ItemKey = "item"
        };
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(item);

        ShowInWindow(menu, window =>
        {
            var container = (NavMenuItem)menu.ContainerFromItem(item)!;
            var header    = GetItemHeader(container);

            menu.Focus(NavigationMethod.Tab).ShouldBeTrue();
            PressKey(window, Key.Down, PhysicalKey.ArrowDown);
            Dispatcher.UIThread.RunJobs();

            IsKeyboardActive(container).ShouldBeTrue();
            header.IsKeyboardActive.ShouldBeTrue(
                "NavMenuItem keyboard active state must reach the header so the AXAML active visual is driven by the same state.");
        });
    }

    private static bool IsKeyboardActive(NavMenuItem container)
    {
        return container.IsKeyboardActive;
    }

    private static BaseNavMenuItemHeader GetItemHeader(NavMenuItem container)
    {
        container.ItemHeader.ShouldBeAssignableTo<BaseNavMenuItemHeader>();
        return (BaseNavMenuItemHeader)container.ItemHeader!;
    }

    private static Color? GetSolidBrushColor(IBrush? brush)
    {
        return brush is ISolidColorBrush solidColorBrush
            ? solidColorBrush.Color
            : null;
    }

    private static void PressKey(AvaloniaWindow window, Key key, PhysicalKey physicalKey)
    {
        window.KeyPress(key, RawInputModifiers.None, physicalKey, null);
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 260,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            content.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }

    private static void RunDispatcherJobsUntil(Func<bool> condition, int maxPasses = 128)
    {
        for (var i = 0; i < maxPasses && !condition(); i++)
        {
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static VisualLayerManager CreatePopupOverlayHost(Control content)
    {
        var visualLayerManager = new VisualLayerManager
        {
            Child = content
        };

        EnablePopupOverlayLayer(visualLayerManager);
        return visualLayerManager;
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
