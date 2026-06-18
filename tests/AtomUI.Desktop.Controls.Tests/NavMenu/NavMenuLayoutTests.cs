using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuLayoutTests
{
    static NavMenuLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(NavMenuMode.Inline)]
    [InlineData(NavMenuMode.Vertical)]
    public void Vertical_Item_Header_Gap_Collapses_AntDesign_Block_Margins(NavMenuMode mode)
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = mode,
            IsMotionEnabled = false,
            Width           = 240
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

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();

            var firstContainer  = (Control)menu.ContainerFromItem(first)!;
            var secondContainer = (Control)menu.ContainerFromItem(second)!;
            var firstHeader     = GetItemHeader(firstContainer);
            var secondHeader    = GetItemHeader(secondContainer);

            var gap = GetTop(secondHeader, menu) - GetBottom(firstHeader, menu);

            gap.ShouldBe(4, 0.5,
                "Ant Design uses marginBlock: marginXXS on adjacent block menu items; CSS collapses those adjacent vertical margins to 4px.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Inline_Submenu_First_Child_Header_Gap_Collapses_AntDesign_Block_Margins()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false,
            Width           = 240
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
            Height  = 320,
            Content = menu
        };

        try
        {
            window.Show();

            var parentContainer = (Control)menu.ContainerFromItem(parent)!;
            var parentMenuItem  = (INavMenuItem)parentContainer;
            parentMenuItem.Open();
            Dispatcher.UIThread.RunJobs();

            var parentHeader   = GetItemHeader(parentContainer);
            var childContainer = (Control)((ItemsControl)parentContainer).ContainerFromItem(child)!;
            var childHeader    = GetItemHeader(childContainer);

            var gap = GetTop(childHeader, menu) - GetBottom(parentHeader, menu);

            gap.ShouldBe(4, 0.5,
                "Ant Design does not add an extra top margin to the inline child items container.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Inline_Submenu_Child_Header_Gap_Collapses_AntDesign_Block_Margins()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false,
            Width           = 240
        };
        var firstChild = new NavMenuNode
        {
            Header  = "First child",
            ItemKey = "first-child"
        };
        var secondChild = new NavMenuNode
        {
            Header  = "Second child",
            ItemKey = "second-child"
        };
        var parent = new NavMenuNode
        {
            Header  = "Parent",
            ItemKey = "parent"
        };
        parent.Children.Add(firstChild);
        parent.Children.Add(secondChild);
        menu.Items.Add(parent);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 320,
            Content = menu
        };

        try
        {
            window.Show();

            var parentContainer = (Control)menu.ContainerFromItem(parent)!;
            var parentMenuItem  = (INavMenuItem)parentContainer;
            parentMenuItem.Open();
            Dispatcher.UIThread.RunJobs();

            var firstChildContainer  = (Control)((ItemsControl)parentContainer).ContainerFromItem(firstChild)!;
            var secondChildContainer = (Control)((ItemsControl)parentContainer).ContainerFromItem(secondChild)!;
            var firstChildHeader     = GetItemHeader(firstChildContainer);
            var secondChildHeader    = GetItemHeader(secondChildContainer);

            var gap = GetTop(secondChildHeader, menu) - GetBottom(firstChildHeader, menu);

            gap.ShouldBe(4, 0.5,
                "Inline submenu children share Ant Design's vertical menu item margin semantics.");
        }
        finally
        {
            window.Close();
        }
    }

    private static BaseNavMenuItemHeader GetItemHeader(Control container)
    {
        var field = container.GetType().GetField(
            "_itemHeader",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return (BaseNavMenuItemHeader)field.GetValue(container)!;
    }

    private static double GetTop(Control control, Visual relativeTo)
    {
        var point = control.TranslatePoint(default, relativeTo);
        point.ShouldNotBeNull();
        return point.Value.Y;
    }

    private static double GetBottom(Control control, Visual relativeTo)
    {
        return GetTop(control, relativeTo) + control.Bounds.Height;
    }
}
