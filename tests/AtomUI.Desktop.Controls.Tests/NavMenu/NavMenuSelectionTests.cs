using System.Reflection;
using System.Threading;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuSelectionTests
{
    static NavMenuSelectionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void NavMenu_Ignores_Stale_Async_SelectedItem_Replay()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
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
        menu.Items.Add(first);
        menu.Items.Add(second);

        var selectedNodes = new List<INavMenuNode>();
        menu.NavMenuNodeSelected += (_, args) => selectedNodes.Add(args.NavMenuNode);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem = first;
            menu.SelectedItem = second;
            Dispatcher.UIThread.RunJobs();

            selectedNodes.ShouldBe([second]);
            menu.SelectedItem.ShouldBeSameAs(second);
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData(NavMenuMode.Vertical)]
    [InlineData(NavMenuMode.Horizontal)]
    [InlineData(NavMenuMode.Inline)]
    public void NavMenu_Clears_Selected_Container_When_SelectedItem_Is_Set_To_Null(NavMenuMode mode)
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = mode,
            IsMotionEnabled = false
        };
        var first = new NavMenuNode
        {
            Header  = "First",
            ItemKey = "first"
        };
        menu.Items.Add(first);

        var selectedNodes = new List<INavMenuNode>();
        menu.NavMenuNodeSelected += (_, args) => selectedNodes.Add(args.NavMenuNode);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem = first;
            Dispatcher.UIThread.RunJobs();

            var firstContainer = menu.ContainerFromItem(first) as Control;
            firstContainer.ShouldNotBeNull();
            IsSelected(firstContainer).ShouldBeTrue();

            menu.SelectedItem = null;
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem.ShouldBeNull();
            IsSelected(firstContainer).ShouldBeFalse();

            selectedNodes.Clear();
            menu.SelectedItem = first;
            Dispatcher.UIThread.RunJobs();

            IsSelected(firstContainer).ShouldBeTrue();
            selectedNodes.ShouldBe([first]);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Inline_Submenu_Child_Press_Does_Not_Apply_Active_Background_To_Parent_Header()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
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
        menu.Items.Add(parent);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = (Control)menu.ContainerFromItem(parent)!;
            var parentMenuItem  = (INavMenuItem)parentContainer;
            parentMenuItem.Open();
            Dispatcher.UIThread.RunJobs();

            var childContainer = (Control)((ItemsControl)parentContainer).ContainerFromItem(child)!;
            var parentHeader   = GetItemHeader(parentContainer);
            var childHeader    = GetItemHeader(childContainer);

            MouseDown(childHeader, window);
            Dispatcher.UIThread.RunJobs();

            GetSolidBrushColor(parentHeader.Background).ShouldBe(
                Colors.Transparent,
                "pressing a child menu item must not make its parent header look active");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Inline_Item_Background_Control_Propagates_To_Header_And_Submenu_Frame()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode                    = NavMenuMode.Inline,
            IsMotionEnabled         = false,
            IsItemBackgroundEnabled = false
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
        menu.Items.Add(parent);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = (Control)menu.ContainerFromItem(parent)!;
            var parentHeader    = GetItemHeader(parentContainer);
            var childFrame      = GetChildItemsFrame(parentContainer);

            parentHeader.IsItemBackgroundEnabled.ShouldBeFalse();
            GetSolidBrushColor(parentHeader.Background).ShouldBe(Colors.Transparent);
            GetSolidBrushColor(childFrame.Background).ShouldBe(Colors.Transparent);

            menu.IsItemBackgroundEnabled = true;
            Dispatcher.UIThread.RunJobs();

            parentHeader.IsItemBackgroundEnabled.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Inline_Submenu_Open_With_Motion_Disabled_Restores_Actor_Opacity_After_Animated_Close()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = true
        };

        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(new NavMenuNode
        {
            Header  = "Child",
            ItemKey = "child"
        });
        menu.Items.Add(parent);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = (Control)menu.ContainerFromItem(parent)!;
            SetTimeSpanProperty(parentContainer, "OpenCloseMotionDuration", TimeSpan.FromMilliseconds(1));

            var parentMenuItem = (INavMenuItem)parentContainer;
            parentMenuItem.Open();
            DrainDispatcher();

            parentMenuItem.Close();
            DrainDispatcher();

            var actor = GetChildItemsMotionActor(parentContainer);
            actor.IsVisible.ShouldBeFalse();
            actor.Opacity.ShouldBe(0.0);

            menu.IsMotionEnabled = false;
            DrainDispatcher();

            parentMenuItem.Open();
            DrainDispatcher();

            actor.IsVisible.ShouldBeTrue();
            actor.Opacity.ShouldBe(1.0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Inline_Submenu_Open_With_Motion_Disabled_Clears_Child_Header_Transitions_After_Animated_Close()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = true
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
        menu.Items.Add(parent);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = (Control)menu.ContainerFromItem(parent)!;
            SetTimeSpanProperty(parentContainer, "OpenCloseMotionDuration", TimeSpan.FromMilliseconds(1));

            var parentMenuItem = (INavMenuItem)parentContainer;
            parentMenuItem.Open();
            DrainDispatcher();

            var childContainer = (Control)((ItemsControl)parentContainer).ContainerFromItem(child)!;
            var childHeader    = GetItemHeader(childContainer);

            childHeader.IsMotionEnabled.ShouldBeTrue();
            childHeader.Transitions.ShouldNotBeNull();

            parentMenuItem.Close();
            DrainDispatcher();

            menu.IsMotionEnabled = false;
            DrainDispatcher();

            parentMenuItem.Open();
            DrainDispatcher();

            childHeader.IsMotionEnabled.ShouldBeFalse();
            childHeader.Transitions.ShouldBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Inline_Submenu_Open_With_Motion_Disabled_Overrides_Pending_Animated_Close()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = true
        };

        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(new NavMenuNode
        {
            Header  = "Child",
            ItemKey = "child"
        });
        menu.Items.Add(parent);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = (Control)menu.ContainerFromItem(parent)!;
            var parentMenuItem  = (INavMenuItem)parentContainer;

            SetTimeSpanProperty(parentContainer, "OpenCloseMotionDuration", TimeSpan.FromMilliseconds(1));
            parentMenuItem.Open();
            DrainDispatcher();

            SetTimeSpanProperty(parentContainer, "OpenCloseMotionDuration", TimeSpan.FromMilliseconds(500));
            parentMenuItem.Close();
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.RunJobs();

            var actor = GetChildItemsMotionActor(parentContainer);
            actor.IsVisible.ShouldBeTrue();
            actor.Transitions.ShouldNotBeNull();

            menu.IsMotionEnabled = false;
            Dispatcher.UIThread.RunJobs();

            parentMenuItem.Open();
            Dispatcher.UIThread.RunJobs();

            parentMenuItem.IsSubMenuOpen.ShouldBeTrue();

            Thread.Sleep(600);
            Dispatcher.UIThread.RunJobs();

            actor.IsVisible.ShouldBeTrue();
            actor.Opacity.ShouldBe(1.0);
        }
        finally
        {
            window.Close();
        }
    }

    private static bool IsSelected(Control container)
    {
        return GetBoolProperty(container, "IsSelected");
    }

    private static bool GetBoolProperty(Control container, string propertyName)
    {
        var property = container.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property.ShouldNotBeNull();
        return (bool)property.GetValue(container)!;
    }

    private static BaseMotionActor GetChildItemsMotionActor(Control container)
    {
        var field = container.GetType().GetField(
            "_childItemsLayoutTransform",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return (BaseMotionActor)field.GetValue(container)!;
    }

    private static Border GetChildItemsFrame(Control container)
    {
        var actor = GetChildItemsMotionActor(container);
        actor.Content.ShouldBeOfType<Border>();
        return (Border)actor.Content;
    }

    private static BaseNavMenuItemHeader GetItemHeader(Control container)
    {
        var field = container.GetType().GetField(
            "_itemHeader",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return (BaseNavMenuItemHeader)field.GetValue(container)!;
    }

    private static void SetTimeSpanProperty(Control container, string propertyName, TimeSpan value)
    {
        var property = container.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        property.ShouldNotBeNull();
        property.SetValue(container, value);
    }

    private static void MouseDown(Control control, Avalonia.Controls.Window window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
    }

    private static Color? GetSolidBrushColor(IBrush? brush)
    {
        return brush is ISolidColorBrush solidColorBrush
            ? solidColorBrush.Color
            : null;
    }

    private static void DrainDispatcher()
    {
        Dispatcher.UIThread.RunJobs();
        Thread.Sleep(20);
        Dispatcher.UIThread.RunJobs();
    }
}
