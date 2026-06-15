using System.Reflection;
using Avalonia.Controls;
using Avalonia.Threading;
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
}
