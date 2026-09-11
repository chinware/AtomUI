using System.Reflection;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.TabControl;

public class TabOverflowMenuTemplateTests
{
    static TabOverflowMenuTemplateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CardTabStrip_Overflow_Items_Preserve_Header_ContentTemplate()
    {
        var template = CreateHeaderTemplate();
        var tabStrip = new CardTabStrip
        {
            Width           = 220,
            ItemsSource     = CreateTabs(),
            ItemTemplate    = template,
            SelectedIndex   = 0,
            IsTabClosable   = true,
            IsMotionEnabled = false
        };

        ShowInWindow(tabStrip, window =>
        {
            var presenter = OpenOverflowMenu(tabStrip, window);
            var overflowItems = presenter.MenuFlyout!.Items
                                         .OfType<TabStripOverflowMenuItem>()
                                         .ToList();

            overflowItems.ShouldNotBeEmpty();
            foreach (var overflowItem in overflowItems)
            {
                overflowItem.TabStripItem.ShouldNotBeNull();
                overflowItem.Header.ShouldBeSameAs(overflowItem.TabStripItem.Content);
                overflowItem.HeaderTemplate.ShouldBeSameAs(overflowItem.TabStripItem.ContentTemplate);
                overflowItem.HeaderTemplate.ShouldBeSameAs(template);
            }
        });
    }

    [Fact]
    public void CardTabControl_Overflow_Items_Preserve_HeaderTemplate()
    {
        var template = CreateHeaderTemplate();
        var tabControl = new CardTabControl
        {
            Width           = 220,
            Height          = 180,
            SelectedIndex   = 0,
            IsTabClosable   = true,
            IsMotionEnabled = false
        };
        foreach (var tab in CreateTabs())
        {
            tabControl.Items.Add(new TabItem
            {
                Header         = tab,
                HeaderTemplate = template,
                Content        = tab.Title
            });
        }

        ShowInWindow(tabControl, window =>
        {
            var presenter = OpenOverflowMenu(tabControl, window);
            var overflowItems = presenter.MenuFlyout!.Items
                                         .OfType<TabControlOverflowMenuItem>()
                                         .ToList();

            overflowItems.ShouldNotBeEmpty();
            foreach (var overflowItem in overflowItems)
            {
                overflowItem.TabItem.ShouldNotBeNull();
                overflowItem.Header.ShouldBeSameAs(overflowItem.TabItem.Header);
                overflowItem.HeaderTemplate.ShouldBeSameAs(overflowItem.TabItem.HeaderTemplate);
                overflowItem.HeaderTemplate.ShouldBeSameAs(template);
            }
        });
    }

    [Fact]
    public void CardTabControl_Overflow_Item_Hides_Close_Button_When_Tab_Is_Not_Closable()
    {
        var tabControl = new CardTabControl
        {
            Width           = 220,
            Height          = 180,
            SelectedIndex   = 0,
            IsTabClosable   = false,
            IsMotionEnabled = false
        };
        foreach (var tab in CreateTabItems())
        {
            tabControl.Items.Add(tab);
        }

        ShowInWindow(tabControl, window =>
        {
            var presenter   = OpenOverflowMenu(tabControl, window);
            var overflowItem = presenter.MenuFlyout!.Items
                                         .OfType<TabControlOverflowMenuItem>()
                                         .First();

            overflowItem.IsClosable.ShouldBeFalse();
            GetOverflowCloseButton(overflowItem).IsVisible.ShouldBeFalse();
        });
    }

    [Fact]
    public void CardTabStrip_Overflow_Item_Hides_Close_Button_When_Tab_Is_Not_Closable()
    {
        var tabStrip = new CardTabStrip
        {
            Width           = 220,
            SelectedIndex   = 0,
            IsTabClosable   = false,
            IsMotionEnabled = false
        };
        foreach (var tab in CreateTabStripItems())
        {
            tabStrip.Items.Add(tab);
        }

        ShowInWindow(tabStrip, window =>
        {
            var presenter   = OpenOverflowMenu(tabStrip, window);
            var overflowItem = presenter.MenuFlyout!.Items
                                         .OfType<TabStripOverflowMenuItem>()
                                         .First();

            overflowItem.IsClosable.ShouldBeFalse();
            GetOverflowCloseButton(overflowItem).IsVisible.ShouldBeFalse();
        });
    }

    [Fact]
    public void CardTabControl_Overflow_Close_Uses_Owner_Closing_Contract()
    {
        var tabControl = new CardTabControl
        {
            Width           = 220,
            Height          = 180,
            SelectedIndex   = 0,
            IsTabClosable   = true,
            IsMotionEnabled = false
        };
        foreach (var tab in CreateTabItems())
        {
            tabControl.Items.Add(tab);
        }

        var closingCount = 0;
        var closedCount  = 0;
        tabControl.Closing += (_, args) =>
        {
            closingCount++;
            args.Cancel = true;
        };
        tabControl.Closed += (_, _) => closedCount++;

        ShowInWindow(tabControl, window =>
        {
            var presenter    = OpenOverflowMenu(tabControl, window);
            var overflowItem = presenter.MenuFlyout!.Items
                                          .OfType<TabControlOverflowMenuItem>()
                                          .First();
            var itemCount    = tabControl.Items.Count;
            var menuCount    = presenter.MenuFlyout.Items.Count;

            ClickOverflowCloseButton(overflowItem);

            closingCount.ShouldBe(1);
            closedCount.ShouldBe(0);
            tabControl.Items.Count.ShouldBe(itemCount);
            presenter.MenuFlyout.Items.Count.ShouldBe(menuCount);
            presenter.MenuFlyout.Items.ShouldContain(overflowItem);
        });
    }

    [Fact]
    public void CardTabStrip_Overflow_Close_Uses_Owner_Closing_Contract()
    {
        var tabStrip = new CardTabStrip
        {
            Width           = 220,
            SelectedIndex   = 0,
            IsTabClosable   = true,
            IsMotionEnabled = false
        };
        foreach (var tab in CreateTabStripItems())
        {
            tabStrip.Items.Add(tab);
        }

        var closingCount = 0;
        var closedCount  = 0;
        tabStrip.Closing += (_, args) =>
        {
            closingCount++;
            args.Cancel = true;
        };
        tabStrip.Closed += (_, _) => closedCount++;

        ShowInWindow(tabStrip, window =>
        {
            var presenter    = OpenOverflowMenu(tabStrip, window);
            var overflowItem = presenter.MenuFlyout!.Items
                                          .OfType<TabStripOverflowMenuItem>()
                                          .First();
            var itemCount    = tabStrip.Items.Count;
            var menuCount    = presenter.MenuFlyout.Items.Count;

            ClickOverflowCloseButton(overflowItem);

            closingCount.ShouldBe(1);
            closedCount.ShouldBe(0);
            tabStrip.Items.Count.ShouldBe(itemCount);
            presenter.MenuFlyout.Items.Count.ShouldBe(menuCount);
            presenter.MenuFlyout.Items.ShouldContain(overflowItem);
        });
    }

    [Fact]
    public void CardTabControl_Overflow_Close_Removes_Menu_Item_After_Owner_Succeeds()
    {
        var tabControl = new CardTabControl
        {
            Width           = 220,
            Height          = 180,
            SelectedIndex   = 0,
            IsTabClosable   = true,
            IsMotionEnabled = false
        };
        foreach (var tab in CreateTabItems())
        {
            tabControl.Items.Add(tab);
        }

        var eventOrder = new List<string>();
        tabControl.Closing += (_, _) => eventOrder.Add("closing");
        tabControl.Closed += (_, _) => eventOrder.Add("closed");

        ShowInWindow(tabControl, window =>
        {
            var presenter    = OpenOverflowMenu(tabControl, window);
            var overflowItem = presenter.MenuFlyout!.Items
                                          .OfType<TabControlOverflowMenuItem>()
                                          .First();
            var tabItem = overflowItem.TabItem.ShouldNotBeNull();
            var itemCount = tabControl.Items.Count;

            ClickOverflowCloseButton(overflowItem);

            eventOrder.ShouldBe(["closing", "closed"]);
            tabControl.Items.Count.ShouldBe(itemCount - 1);
            tabControl.Items.ShouldNotContain(tabItem);
            presenter.MenuFlyout.Items.ShouldNotContain(overflowItem);
        });
    }

    [Fact]
    public void CardTabStrip_Overflow_Close_Removes_Menu_Item_After_Owner_Succeeds()
    {
        var tabStrip = new CardTabStrip
        {
            Width           = 220,
            SelectedIndex   = 0,
            IsTabClosable   = true,
            IsMotionEnabled = false
        };
        foreach (var tab in CreateTabStripItems())
        {
            tabStrip.Items.Add(tab);
        }

        var eventOrder = new List<string>();
        tabStrip.Closing += (_, _) => eventOrder.Add("closing");
        tabStrip.Closed += (_, _) => eventOrder.Add("closed");

        ShowInWindow(tabStrip, window =>
        {
            var presenter    = OpenOverflowMenu(tabStrip, window);
            var overflowItem = presenter.MenuFlyout!.Items
                                          .OfType<TabStripOverflowMenuItem>()
                                          .First();
            var tabItem = overflowItem.TabStripItem.ShouldNotBeNull();
            var itemCount = tabStrip.Items.Count;

            ClickOverflowCloseButton(overflowItem);

            eventOrder.ShouldBe(["closing", "closed"]);
            tabStrip.Items.Count.ShouldBe(itemCount - 1);
            tabStrip.Items.ShouldNotContain(tabItem);
            presenter.MenuFlyout.Items.ShouldNotContain(overflowItem);
        });
    }

    [Fact]
    public void CardTabStrip_Overflow_Close_Removes_Item_From_Writable_ItemsSource()
    {
        var source = CreateTabs().ToList();
        var tabStrip = new CardTabStrip
        {
            Width           = 220,
            ItemsSource     = source,
            ItemTemplate    = CreateHeaderTemplate(),
            SelectedIndex   = 0,
            IsTabClosable   = true,
            IsMotionEnabled = false
        };

        var closedCount = 0;
        tabStrip.Closed += (_, _) => closedCount++;

        ShowInWindow(tabStrip, window =>
        {
            var presenter    = OpenOverflowMenu(tabStrip, window);
            var overflowItem = presenter.MenuFlyout!.Items
                                          .OfType<TabStripOverflowMenuItem>()
                                          .First();
            var tabStripItem = overflowItem.TabStripItem.ShouldNotBeNull();
            var sourceItem   = tabStripItem.Content.ShouldBeOfType<DemoTab>();

            source.ShouldContain(sourceItem);
            ClickOverflowCloseButton(overflowItem);

            closedCount.ShouldBe(1);
            source.ShouldNotContain(sourceItem);
            tabStrip.Items.Count.ShouldBe(source.Count);
            presenter.MenuFlyout.Items.ShouldNotContain(overflowItem);
        });
    }

    [Fact]
    public void CardTabControl_Pinned_Overflow_Menu_Reopens_After_Detach()
    {
        var tabControl = new CardTabControl
        {
            Width                 = 220,
            Height                = 180,
            SelectedIndex         = 0,
            IsTabClosable         = true,
            IsMotionEnabled       = false,
            IsPopupPinnedOpen     = true
        };
        foreach (var tab in CreateTabs())
        {
            tabControl.Items.Add(new TabItem
            {
                Header  = tab.Title,
                Content = tab.Title
            });
        }

        ShowInWindowWithHost(tabControl, (_, host) =>
        {
            var scrollViewer = GetOverflowScrollViewer(tabControl);
            var flyout       = GetMenuFlyout(scrollViewer);
            flyout.IsOpen.ShouldBeTrue();
            flyout.Popup.IsOpen.ShouldBeTrue();

            flyout.Hide();
            Dispatcher.UIThread.RunJobs();
            flyout.IsOpen.ShouldBeTrue();
            flyout.Popup.IsOpen.ShouldBeTrue();

            host.Children.Remove(tabControl);
            Dispatcher.UIThread.RunJobs();
            flyout.IsOpen.ShouldBeFalse();
            flyout.Popup.IsOpen.ShouldBeFalse();
            flyout.IsPopupPinnedOpen.ShouldBeFalse();

            host.Children.Add(tabControl);
            Dispatcher.UIThread.RunJobs();
            var reopenedFlyout = GetMenuFlyout(GetOverflowScrollViewer(tabControl));
            reopenedFlyout.IsOpen.ShouldBeTrue();
            reopenedFlyout.Popup.IsOpen.ShouldBeTrue();

            tabControl.IsPopupPinnedOpen = false;
            Dispatcher.UIThread.RunJobs();
            reopenedFlyout.IsOpen.ShouldBeTrue();
        });
    }

    [Fact]
    public void CardTabStrip_Pinned_Overflow_Menu_Reopens_After_Detach()
    {
        var tabStrip = new CardTabStrip
        {
            Width                 = 220,
            ItemsSource           = CreateTabs(),
            ItemTemplate          = CreateHeaderTemplate(),
            SelectedIndex         = 0,
            IsTabClosable         = true,
            IsMotionEnabled       = false,
            IsPopupPinnedOpen     = true
        };

        ShowInWindowWithHost(tabStrip, (_, host) =>
        {
            var scrollViewer = GetOverflowScrollViewer(tabStrip);
            var flyout       = GetMenuFlyout(scrollViewer);
            flyout.IsOpen.ShouldBeTrue();
            flyout.Popup.IsOpen.ShouldBeTrue();

            flyout.Hide();
            Dispatcher.UIThread.RunJobs();
            flyout.IsOpen.ShouldBeTrue();
            flyout.Popup.IsOpen.ShouldBeTrue();

            host.Children.Remove(tabStrip);
            Dispatcher.UIThread.RunJobs();
            flyout.IsOpen.ShouldBeFalse();
            flyout.Popup.IsOpen.ShouldBeFalse();
            flyout.IsPopupPinnedOpen.ShouldBeFalse();

            host.Children.Add(tabStrip);
            Dispatcher.UIThread.RunJobs();
            var reopenedFlyout = GetMenuFlyout(GetOverflowScrollViewer(tabStrip));
            reopenedFlyout.IsOpen.ShouldBeTrue();
            reopenedFlyout.Popup.IsOpen.ShouldBeTrue();

            tabStrip.IsPopupPinnedOpen = false;
            Dispatcher.UIThread.RunJobs();
            reopenedFlyout.IsOpen.ShouldBeTrue();
        });
    }

    private static MenuFlyoutPresenter OpenOverflowMenu(Control owner, AvaloniaWindow window)
    {
        var scrollViewer = GetOverflowScrollViewer(owner);
        scrollViewer.Extent.Width.ShouldBeGreaterThan(scrollViewer.Viewport.Width);

        var menuIndicator = scrollViewer.GetVisualDescendants()
                                        .OfType<IconButton>()
                                        .Single(button => button.Name == "PART_ScrollMenuIndicator");
        menuIndicator.IsVisible.ShouldBeTrue();
        menuIndicator.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, menuIndicator));
        Dispatcher.UIThread.RunJobs();

        return window.GetVisualDescendants()
                     .OfType<MenuFlyoutPresenter>()
                     .ShouldHaveSingleItem();
    }

    private static BaseTabScrollViewer GetOverflowScrollViewer(Control owner)
    {
        var scrollViewer = owner.GetVisualDescendants()
                                .OfType<BaseTabScrollViewer>()
                                .ShouldHaveSingleItem();
        scrollViewer.Extent.Width.ShouldBeGreaterThan(scrollViewer.Viewport.Width);
        return scrollViewer;
    }

    private static MenuFlyout GetMenuFlyout(BaseTabScrollViewer scrollViewer)
    {
        var field = typeof(BaseTabScrollViewer).GetField(
            "MenuFlyout",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return field.GetValue(scrollViewer).ShouldBeOfType<MenuFlyout>();
    }

    private static IconButton GetOverflowCloseButton(BaseOverflowMenuItem overflowItem)
    {
        return overflowItem.GetVisualDescendants()
                           .OfType<IconButton>()
                           .Single(button => button.Name == "PART_ItemCloseButton");
    }

    private static void ClickOverflowCloseButton(BaseOverflowMenuItem overflowItem)
    {
        var closeButton = GetOverflowCloseButton(overflowItem);
        closeButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, closeButton));
        Dispatcher.UIThread.RunJobs();
    }

    private static FuncDataTemplate<DemoTab> CreateHeaderTemplate()
    {
        return new FuncDataTemplate<DemoTab>(
            (item, _) => new TextBlock { Text = $"Rendered:{item?.Title}" });
    }

    private static DemoTab[] CreateTabs()
    {
        return Enumerable.Range(1, 8)
                         .Select(index => new DemoTab($"Document {index:00}"))
                         .ToArray();
    }

    private static TabItem[] CreateTabItems()
    {
        return Enumerable.Range(1, 8)
                         .Select(index => new TabItem
                         {
                             Header  = $"Document {index:00}",
                             Content = $"Content {index:00}"
                         })
                         .ToArray();
    }

    private static TabStripItem[] CreateTabStripItems()
    {
        return Enumerable.Range(1, 8)
                         .Select(index => new TabStripItem
                         {
                             Content = $"Document {index:00}"
                         })
                         .ToArray();
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 260,
            Content = CreatePopupOverlayHost(content)
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

    private static void ShowInWindowWithHost(
        Control content,
        Action<AvaloniaWindow, ScopeAwareOverlayLayerPanel> assertion)
    {
        var overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 360,
            Height = 260
        };
        overlayPanel.Children.Add(content);

        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 260,
            Content = CreatePopupOverlayHost(overlayPanel)
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion(window, overlayPanel);
        }
        finally
        {
            window.Close();
        }
    }

    private static VisualLayerManager CreatePopupOverlayHost(Control content)
    {
        var overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 360,
            Height = 260
        };
        overlayPanel.Children.Add(content);

        var visualLayerManager = new VisualLayerManager
        {
            Child = overlayPanel
        };
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);
        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
        return visualLayerManager;
    }

    private sealed class DemoTab(string title)
    {
        public string Title { get; } = title;
    }
}
