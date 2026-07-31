using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomCardTabControl = AtomUI.Desktop.Controls.CardTabControl;
using AtomTabItem = AtomUI.Desktop.Controls.TabItem;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.TabControl;

public class TabCardLineMaskTests
{
    static TabCardLineMaskTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CardTabControl_LineMask_Is_Visible_Only_For_Selected_Item()
    {
        var tabControl = new AtomCardTabControl
        {
            Width  = 360,
            Height = 220
        };

        tabControl.Items.Add(new AtomTabItem { Header = "Tab 1", Content = "Content 1" });
        tabControl.Items.Add(new AtomTabItem { Header = "Tab 2", Content = "Content 2", IsEnabled = false });
        tabControl.Items.Add(new AtomTabItem { Header = "Tab 3", Content = "Content 3" });
        tabControl.SelectedIndex = 2;

        ShowInWindow(tabControl, _ =>
        {
            GetLineMask(tabControl, 0).IsVisible.ShouldBeFalse();
            GetLineMask(tabControl, 1).IsVisible.ShouldBeFalse();
            GetLineMask(tabControl, 2).IsVisible.ShouldBeTrue();
        });
    }

    private static Rectangle GetLineMask(ItemsControl owner, int index)
    {
        var container = GetContainer<Control>(owner, index);
        var lineMask = container.GetVisualDescendants()
                                .OfType<Rectangle>()
                                .SingleOrDefault(control => control.Name == "LineMask");
        lineMask.ShouldNotBeNull();
        return lineMask!;
    }

    private static T GetContainer<T>(ItemsControl owner, int index)
        where T : Control
    {
        RunJobsUntil(() => owner.ContainerFromIndex(index) is T);
        var container = owner.ContainerFromIndex(index);
        container.ShouldNotBeNull();
        container.ShouldBeAssignableTo<T>();
        return (T)container!;
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 520,
            Height  = 360,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }

    private static void RunJobsUntil(Func<bool> condition, int maxPasses = 128)
    {
        for (var i = 0; i < maxPasses; i++)
        {
            Dispatcher.UIThread.RunJobs();
            if (condition())
            {
                return;
            }
        }
    }
}
