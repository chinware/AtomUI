using System.Reflection;
using System.Threading;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Icons.AntDesign;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuInlineCollapsedTests
{
    static NavMenuInlineCollapsedTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void InlineCollapsedWidth_Defaults_To_Token_Value_And_Can_Be_Overridden_Locally()
    {
        var defaultMenu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode              = NavMenuMode.Inline,
            IsMotionEnabled   = false,
            IsInlineCollapsed = true,
            Width             = 240
        };
        defaultMenu.Items.Add(new NavMenuNode
        {
            Header  = "Default",
            ItemKey = "default"
        });

        var customMenu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode                 = NavMenuMode.Inline,
            IsMotionEnabled      = false,
            IsInlineCollapsed    = true,
            InlineCollapsedWidth = 72
        };
        customMenu.Items.Add(new NavMenuNode
        {
            Header  = "Custom",
            ItemKey = "custom"
        });

        var stack = new StackPanel
        {
            Children =
            {
                defaultMenu,
                customMenu
            }
        };
        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = stack
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            defaultMenu.InlineCollapsedWidth.ShouldBe(48);
            defaultMenu.Bounds.Width.ShouldBe(48, 0.5);
            customMenu.Bounds.Width.ShouldBe(72, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InlineCollapsed_Toggle_Restores_Explicit_Expanded_Width()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false,
            MinWidth        = 160,
            Width           = 240
        };
        menu.Items.Add(new NavMenuNode
        {
            Header  = "Default",
            ItemKey = "default"
        });

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

            menu.Bounds.Width.ShouldBe(240, 0.5);

            menu.IsInlineCollapsed = true;
            Dispatcher.UIThread.RunJobs();

            menu.Bounds.Width.ShouldBe(48, 0.5);

            menu.IsInlineCollapsed = false;
            Dispatcher.UIThread.RunJobs();

            menu.Width.ShouldBe(240);
            menu.Bounds.Width.ShouldBe(240, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InlineCollapsed_Toggle_With_Motion_Animates_Width_Instead_Of_Jumping_To_Target()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode              = NavMenuMode.Inline,
            IsMotionEnabled   = true,
            IsInlineCollapsed = false,
            Width             = 300
        };
        menu.Items.Add(new NavMenuNode
        {
            Header  = "Default",
            ItemKey = "default"
        });

        var window = new Avalonia.Controls.Window
        {
            Width   = 420,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            menu.Bounds.Width.ShouldBe(300, 0.5);

            menu.IsInlineCollapsed = true;
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(20);
            Dispatcher.UIThread.RunJobs();

            menu.Bounds.Width.ShouldBeGreaterThan(menu.InlineCollapsedWidth,
                "motion enabled inline collapsed should expose an intermediate layout width instead of jumping from expanded width to the collapsed target in one frame.");
            WaitForWidthBetween(menu, menu.InlineCollapsedWidth, 300);

            WaitForWidth(menu, menu.InlineCollapsedWidth);
            WaitForInlineCollapsedLayoutWidthCleared(menu);

            menu.Bounds.Width.ShouldBe(menu.InlineCollapsedWidth, 0.5);
            menu.GetBaseValue(Layoutable.WidthProperty).Value.ShouldBe(300);
            double.IsNaN(menu.InlineCollapsedLayoutWidth).ShouldBeTrue(
                "the internal animated layout width must be cleared after collapse motion completes so the collapsed steady state is controlled by InlineCollapsedWidth.");

            menu.IsInlineCollapsed = false;
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(20);
            Dispatcher.UIThread.RunJobs();

            menu.Bounds.Width.ShouldBeLessThan(300);
            WaitForWidthBetween(menu, menu.InlineCollapsedWidth, 300);

            WaitForWidth(menu, 300);
            WaitForInlineCollapsedLayoutWidthCleared(menu);

            menu.Bounds.Width.ShouldBe(300, 0.5);
            menu.GetBaseValue(Layoutable.WidthProperty).Value.ShouldBe(300);
            double.IsNaN(menu.InlineCollapsedLayoutWidth).ShouldBeTrue(
                "the internal animated layout width must be cleared after expand motion completes so future Width changes are not constrained by stale animation state.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InlineCollapsed_Coerces_Effective_Width_And_Preserves_Base_Width_For_Expand()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false,
            MinWidth        = 160,
            Width           = 240
        };
        menu.Items.Add(new NavMenuNode
        {
            Header  = "Default",
            ItemKey = "default"
        });

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

            menu.GetBaseValue(Layoutable.WidthProperty).Value.ShouldBe(240);
            menu.MinWidth.ShouldBe(160);

            menu.IsInlineCollapsed = true;
            Dispatcher.UIThread.RunJobs();

            menu.Width.ShouldBe(48);
            menu.MinWidth.ShouldBe(48);
            menu.GetBaseValue(Layoutable.WidthProperty).Value.ShouldBe(240);
            menu.GetBaseValue(Layoutable.MinWidthProperty).Value.ShouldBe(160);

            menu.IsInlineCollapsed = false;
            Dispatcher.UIThread.RunJobs();

            menu.Width.ShouldBe(240);
            menu.MinWidth.ShouldBe(160);
            menu.GetBaseValue(Layoutable.WidthProperty).Value.ShouldBe(240);
            menu.GetBaseValue(Layoutable.MinWidthProperty).Value.ShouldBe(160);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InlineCollapsed_Uses_Effective_Vertical_Mode_Without_Changing_Public_Mode()
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
            Mode              = NavMenuMode.Inline,
            IsInlineCollapsed = true,
            IsMotionEnabled   = false
        };
        menu.Items.Add(parent);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = CreatePopupOverlayHost(menu)
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            menu.Mode.ShouldBe(NavMenuMode.Inline);
            menu.EffectiveMode.ShouldBe(NavMenuMode.Vertical);
            menu.InteractionHandler.ShouldBeOfType<DefaultNavMenuInteractionHandler>();

            var parentContainer = (NavMenuItem)menu.ContainerFromItem(parent)!;
            parentContainer.Mode.ShouldBe(NavMenuMode.Vertical);

            ((INavMenuItem)parentContainer).Open();
            Dispatcher.UIThread.RunJobs();

            parentContainer.Popup.ShouldNotBeNull();
            parentContainer.Popup.IsOpen.ShouldBeTrue(
                "inline collapsed reuses the popup branch instead of realizing inline child visuals in the root tree.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InlineCollapsed_Toggle_Preserves_SelectedItem_And_Restores_Open_Inline_Path()
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
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = CreatePopupOverlayHost(menu)
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = (NavMenuItem)menu.ContainerFromItem(parent)!;
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();

            var childContainer = (NavMenuItem)parentContainer.ContainerFromItem(child)!;
            menu.InteractionHandler.ShouldNotBeNull();
            menu.InteractionHandler.Select(childContainer);
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem.ShouldBeSameAs(child);
            parentContainer.IsSubMenuOpen.ShouldBeTrue();
            parentContainer.IsInSelectedPath.ShouldBeTrue();

            menu.IsInlineCollapsed = true;
            Dispatcher.UIThread.RunJobs();

            menu.Mode.ShouldBe(NavMenuMode.Inline);
            menu.EffectiveMode.ShouldBe(NavMenuMode.Vertical);
            menu.SelectedItem.ShouldBeSameAs(child);
            parentContainer.IsSubMenuOpen.ShouldBeFalse(
                "entering collapsed state closes inline child visuals in the root tree.");
            parentContainer.IsInSelectedPath.ShouldBeTrue(
                "selected path is a selection state and must survive inline collapsed toggles.");

            menu.IsInlineCollapsed = false;
            Dispatcher.UIThread.RunJobs();

            menu.EffectiveMode.ShouldBe(NavMenuMode.Inline);
            menu.SelectedItem.ShouldBeSameAs(child);
            parentContainer.IsSubMenuOpen.ShouldBeTrue(
                "leaving collapsed state restores the cached inline open path.");
            parentContainer.IsInSelectedPath.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InlineCollapsed_Handler_Recreation_Does_Not_Leave_Previous_Item_Selected()
    {
        var option1 = new NavMenuNode
        {
            Header  = "Option 1",
            ItemKey = "Option1",
            Icon    = new PieChartOutlined()
        };
        var option2 = new NavMenuNode
        {
            Header  = "Option 2",
            ItemKey = "Option2",
            Icon    = new PieChartOutlined()
        };

        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode                = NavMenuMode.Inline,
            IsMotionEnabled     = false,
            DefaultSelectedPath = new TreeNodePath("/Option1")
        };
        menu.Items.Add(option1);
        menu.Items.Add(option2);

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

            var option1Container = (NavMenuItem)menu.ContainerFromItem(option1)!;
            var option2Container = (NavMenuItem)menu.ContainerFromItem(option2)!;

            option1Container.IsSelected.ShouldBeTrue();
            option2Container.IsSelected.ShouldBeFalse();

            menu.IsInlineCollapsed = true;
            Dispatcher.UIThread.RunJobs();

            menu.InteractionHandler.ShouldNotBeNull();
            menu.InteractionHandler.Select(option2Container);
            Dispatcher.UIThread.RunJobs();

            option1Container.IsSelected.ShouldBeFalse(
                "recreating the interaction handler for inline collapsed mode must not lose the previous selected item, otherwise the next selection cannot clear it.");
            option2Container.IsSelected.ShouldBeTrue();
            menu.SelectedItem.ShouldBeSameAs(option2);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InlineCollapsed_Does_Not_Affect_Vertical_Or_Horizontal_Effective_Mode()
    {
        foreach (var mode in new[] { NavMenuMode.Vertical, NavMenuMode.Horizontal })
        {
            var menu = new AtomUI.Desktop.Controls.NavMenu
            {
                Mode              = mode,
                IsInlineCollapsed = true
            };

            menu.EffectiveMode.ShouldBe(mode);
        }
    }

    [Fact]
    public void InlineCollapsed_Icon_Is_Centered_In_Item_Header()
    {
        var item = new NavMenuNode
        {
            Header  = "Dashboard",
            ItemKey = "dashboard",
            Icon    = new PieChartOutlined()
        };

        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode              = NavMenuMode.Inline,
            IsMotionEnabled   = false,
            IsInlineCollapsed = true,
            IsDarkStyle       = true
        };
        menu.Items.Add(item);
        menu.SelectedItem = item;

        var window = new Avalonia.Controls.Window
        {
            Width   = 240,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var itemContainer = (Control)menu.ContainerFromItem(item)!;
            var itemHeader    = GetItemHeader(itemContainer);
            var frame = itemHeader.GetVisualDescendants()
                                  .OfType<Border>()
                                  .Single(border => border.Name == "Frame");
            var iconPresenter = itemHeader.GetVisualDescendants()
                                          .OfType<IconPresenter>()
                                          .Single(presenter => presenter.Name == "ItemIconPresenter");

            var iconPosition = iconPresenter.TranslatePoint(default, frame);
            iconPosition.ShouldNotBeNull();

            var frameCenter = frame.Bounds.Width / 2;
            var iconCenter  = iconPosition.Value.X + iconPresenter.Bounds.Width / 2;

            iconCenter.ShouldBe(frameCenter, 0.5,
                "the collapsed inline header hides text and indicator visuals, so the icon should be centered in the visible item frame rather than in the original text-layout column.");
        }
        finally
        {
            window.Close();
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
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }

    private static BaseNavMenuItemHeader GetItemHeader(Control container)
    {
        var field = container.GetType().GetField(
            "_itemHeader",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return (BaseNavMenuItemHeader)field.GetValue(container)!;
    }

    private static void WaitForWidth(Control control, double expectedWidth)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(2);
        while (DateTime.UtcNow < deadline)
        {
            Dispatcher.UIThread.RunJobs(DispatcherPriority.SystemIdle);
            if (Math.Abs(control.Bounds.Width - expectedWidth) <= 0.5)
            {
                return;
            }

            Thread.Sleep(20);
        }

        Dispatcher.UIThread.RunJobs(DispatcherPriority.SystemIdle);
    }

    private static void WaitForWidthBetween(Control control, double lowerBound, double upperBound)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(1);
        while (DateTime.UtcNow < deadline)
        {
            Dispatcher.UIThread.RunJobs(DispatcherPriority.SystemIdle);
            if (control.Bounds.Width > lowerBound + 0.5 &&
                control.Bounds.Width < upperBound - 0.5)
            {
                return;
            }

            Thread.Sleep(10);
        }

        control.Bounds.Width.ShouldBeGreaterThan(lowerBound);
        control.Bounds.Width.ShouldBeLessThan(upperBound);
    }

    private static void WaitForInlineCollapsedLayoutWidthCleared(AtomUI.Desktop.Controls.NavMenu menu)
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(1);
        while (DateTime.UtcNow < deadline)
        {
            Dispatcher.UIThread.RunJobs(DispatcherPriority.SystemIdle);
            if (double.IsNaN(menu.InlineCollapsedLayoutWidth))
            {
                return;
            }

            Thread.Sleep(10);
        }

        double.IsNaN(menu.InlineCollapsedLayoutWidth).ShouldBeTrue();
    }
}
