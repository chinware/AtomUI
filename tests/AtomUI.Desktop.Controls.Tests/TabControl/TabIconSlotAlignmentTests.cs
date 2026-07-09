using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomCardTabControl = AtomUI.Desktop.Controls.CardTabControl;
using AtomCardTabStrip = AtomUI.Desktop.Controls.CardTabStrip;
using AtomTabControl = AtomUI.Desktop.Controls.TabControl;
using AtomTabItem = AtomUI.Desktop.Controls.TabItem;
using AtomTabStrip = AtomUI.Desktop.Controls.TabStrip;
using AtomTabStripItem = AtomUI.Desktop.Controls.TabStripItem;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.TabControl;

public class TabIconSlotAlignmentTests
{
    static TabIconSlotAlignmentTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void TabControl_Left_Placement_Aligns_Text_When_Only_Some_Tabs_Have_Icons()
    {
        var tabControl = CreateTabControl(Dock.Left);

        ShowInWindow(tabControl, _ =>
        {
            var iconTextX = GetContentPresenterOffsetX(tabControl, 0);
            var textOnlyX = GetContentPresenterOffsetX(tabControl, 1);

            textOnlyX.ShouldBe(iconTextX, 0.5);
        });
    }

    [Fact]
    public void CardTabControl_Left_Placement_Aligns_Text_When_Only_Some_Tabs_Have_Icons()
    {
        var tabControl = CreateCardTabControl(Dock.Left);

        ShowInWindow(tabControl, _ =>
        {
            var iconTextX = GetContentPresenterOffsetX(tabControl, 0);
            var textOnlyX = GetContentPresenterOffsetX(tabControl, 1);

            textOnlyX.ShouldBe(iconTextX, 0.5);
        });
    }

    [Fact]
    public void TabStrip_Left_Placement_Aligns_Text_When_Only_Some_Tabs_Have_Icons()
    {
        var tabStrip = CreateTabStrip(Dock.Left);

        ShowInWindow(tabStrip, _ =>
        {
            var iconTextX = GetContentPresenterOffsetX(tabStrip, 0);
            var textOnlyX = GetContentPresenterOffsetX(tabStrip, 1);

            textOnlyX.ShouldBe(iconTextX, 0.5);
        });
    }

    [Fact]
    public void CardTabStrip_Left_Placement_Aligns_Text_When_Only_Some_Tabs_Have_Icons()
    {
        var tabStrip = CreateCardTabStrip(Dock.Left);

        ShowInWindow(tabStrip, _ =>
        {
            var iconTextX = GetContentPresenterOffsetX(tabStrip, 0);
            var textOnlyX = GetContentPresenterOffsetX(tabStrip, 1);

            textOnlyX.ShouldBe(iconTextX, 0.5);
        });
    }

    [Fact]
    public void TabControl_Top_Placement_Remains_Compact_When_Only_Some_Tabs_Have_Icons()
    {
        var tabControl = CreateTabControl(Dock.Top);

        ShowInWindow(tabControl, _ =>
        {
            var iconTextX = GetContentPresenterOffsetX(tabControl, 0);
            var textOnlyX = GetContentPresenterOffsetX(tabControl, 1);

            textOnlyX.ShouldBeLessThan(iconTextX);
        });
    }

    [Fact]
    public void TabStrip_Top_Placement_Remains_Compact_When_Only_Some_Tabs_Have_Icons()
    {
        var tabStrip = CreateTabStrip(Dock.Top);

        ShowInWindow(tabStrip, _ =>
        {
            var iconTextX = GetContentPresenterOffsetX(tabStrip, 0);
            var textOnlyX = GetContentPresenterOffsetX(tabStrip, 1);

            textOnlyX.ShouldBeLessThan(iconTextX);
        });
    }

    [Fact]
    public void TabControl_Changing_From_Left_To_Top_Releases_Text_Only_Icon_Slot()
    {
        var tabControl = CreateTabControl(Dock.Left);

        ShowInWindow(tabControl, _ =>
        {
            var reservedOffset = GetContentPresenterOffsetX(tabControl, 1);

            tabControl.TabStripPlacement = Dock.Top;
            Dispatcher.UIThread.RunJobs();

            GetContentPresenterOffsetX(tabControl, 1).ShouldBeLessThan(reservedOffset);
        });
    }

    [Fact]
    public void TabStrip_Changing_From_Left_To_Top_Releases_Text_Only_Icon_Slot()
    {
        var tabStrip = CreateTabStrip(Dock.Left);

        ShowInWindow(tabStrip, _ =>
        {
            var reservedOffset = GetContentPresenterOffsetX(tabStrip, 1);

            tabStrip.TabStripPlacement = Dock.Top;
            Dispatcher.UIThread.RunJobs();

            GetContentPresenterOffsetX(tabStrip, 1).ShouldBeLessThan(reservedOffset);
        });
    }

    [Fact]
    public void TabControl_Removing_Last_Icon_Releases_Text_Only_Icon_Slot()
    {
        var tabControl = CreateTabControl(Dock.Left);

        ShowInWindow(tabControl, _ =>
        {
            var reservedOffset = GetContentPresenterOffsetX(tabControl, 1);

            GetContainer<AtomTabItem>(tabControl, 0).Icon = null;
            Dispatcher.UIThread.RunJobs();

            GetContentPresenterOffsetX(tabControl, 1).ShouldBeLessThan(reservedOffset);
        });
    }

    [Fact]
    public void TabStrip_Adding_First_Icon_Reserves_Text_Only_Icon_Slot()
    {
        var tabStrip = CreateTextOnlyTabStrip(Dock.Left);

        ShowInWindow(tabStrip, _ =>
        {
            var compactOffset = GetContentPresenterOffsetX(tabStrip, 1);

            GetContainer<AtomTabStripItem>(tabStrip, 0).Icon = CreateIcon();
            Dispatcher.UIThread.RunJobs();

            var iconTextX = GetContentPresenterOffsetX(tabStrip, 0);
            var textOnlyX = GetContentPresenterOffsetX(tabStrip, 1);

            textOnlyX.ShouldBeGreaterThan(compactOffset);
            textOnlyX.ShouldBe(iconTextX, 0.5);
        });
    }

    private static AtomTabControl CreateTabControl(Dock placement)
    {
        var tabControl = new AtomTabControl
        {
            Width             = 320,
            Height            = 180,
            TabStripPlacement = placement,
            SelectedIndex     = 0
        };
        tabControl.Items.Add(new AtomTabItem { Header = "With icon", Content = "Content 1", Icon = CreateIcon() });
        tabControl.Items.Add(new AtomTabItem { Header = "Text only", Content = "Content 2" });
        return tabControl;
    }

    private static AtomCardTabControl CreateCardTabControl(Dock placement)
    {
        var tabControl = new AtomCardTabControl
        {
            Width             = 320,
            Height            = 180,
            TabStripPlacement = placement,
            SelectedIndex     = 0
        };
        tabControl.Items.Add(new AtomTabItem { Header = "With icon", Content = "Content 1", Icon = CreateIcon() });
        tabControl.Items.Add(new AtomTabItem { Header = "Text only", Content = "Content 2" });
        return tabControl;
    }

    private static AtomTabStrip CreateTabStrip(Dock placement)
    {
        var tabStrip = new AtomTabStrip
        {
            Width             = 320,
            Height            = 180,
            TabStripPlacement = placement,
            SelectedIndex     = 0
        };
        tabStrip.Items.Add(new AtomTabStripItem { Content = "With icon", Icon = CreateIcon() });
        tabStrip.Items.Add(new AtomTabStripItem { Content = "Text only" });
        return tabStrip;
    }

    private static AtomTabStrip CreateTextOnlyTabStrip(Dock placement)
    {
        var tabStrip = new AtomTabStrip
        {
            Width             = 320,
            Height            = 180,
            TabStripPlacement = placement,
            SelectedIndex     = 0
        };
        tabStrip.Items.Add(new AtomTabStripItem { Content = "First" });
        tabStrip.Items.Add(new AtomTabStripItem { Content = "Second" });
        return tabStrip;
    }

    private static AtomCardTabStrip CreateCardTabStrip(Dock placement)
    {
        var tabStrip = new AtomCardTabStrip
        {
            Width             = 320,
            Height            = 180,
            TabStripPlacement = placement,
            SelectedIndex     = 0
        };
        tabStrip.Items.Add(new AtomTabStripItem { Content = "With icon", Icon = CreateIcon() });
        tabStrip.Items.Add(new AtomTabStripItem { Content = "Text only" });
        return tabStrip;
    }

    private static PathIcon CreateIcon()
    {
        return new PathIcon { Data = Geometry.Parse("M0,0 L10,0 L10,10 Z") };
    }

    private static double GetContentPresenterOffsetX(ItemsControl owner, int index)
    {
        var container = GetContainer<Control>(owner, index);
        var presenter = GetVisualDescendant<ContentPresenter>(container, "ContentPresenter");
        var point = presenter.TranslatePoint(default, container);
        point.ShouldNotBeNull();
        return point.Value.X;
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

    private static T GetVisualDescendant<T>(Visual root, string name)
        where T : Control
    {
        var match = root.GetVisualDescendants()
                        .OfType<T>()
                        .FirstOrDefault(control => control.Name == name);
        match.ShouldNotBeNull();
        return match;
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
