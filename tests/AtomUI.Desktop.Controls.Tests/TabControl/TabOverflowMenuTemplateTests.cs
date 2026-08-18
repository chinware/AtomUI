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

    private static MenuFlyoutPresenter OpenOverflowMenu(Control owner, AvaloniaWindow window)
    {
        var scrollViewer = owner.GetVisualDescendants()
                                .OfType<BaseTabScrollViewer>()
                                .ShouldHaveSingleItem();
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
