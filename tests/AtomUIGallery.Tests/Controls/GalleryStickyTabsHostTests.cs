using System;
using System.IO;
using System.Linq;
using AtomUI.Desktop.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaScrollViewer = Avalonia.Controls.ScrollViewer;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUIGallery.Tests.Controls;

public class GalleryStickyTabsHostTests
{
    public GalleryStickyTabsHostTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Sticky_Panel_Keeps_Sticky_Item_In_Normal_Flow_Before_Threshold()
    {
        var panel = CreatePanel();

        panel.StickyOffsetY = 20;
        MeasureAndArrange(panel, 200);

        panel.Children[0].Bounds.Y.ShouldBe(0);
        panel.Children[1].Bounds.Y.ShouldBe(40);
        panel.Children[2].Bounds.Y.ShouldBe(70);
        panel.Children[2].Clip.ShouldBeNull();
        panel.Bounds.Height.ShouldBe(170);
    }

    [Fact]
    public void Sticky_Panel_Pins_Sticky_Item_To_Scroll_Offset_After_Threshold()
    {
        var panel = CreatePanel();

        panel.StickyOffsetY = 56;
        MeasureAndArrange(panel, 200);

        panel.Children[0].Bounds.Y.ShouldBe(0);
        panel.Children[1].Bounds.Y.ShouldBe(56);
        panel.Children[2].Bounds.Y.ShouldBe(70);
        panel.IsStickyPinned.ShouldBeTrue();
        panel.Bounds.Height.ShouldBe(170);
    }

    [Fact]
    public void Sticky_Panel_Reports_Unpinned_State_Before_Threshold()
    {
        var panel = CreatePanel();

        panel.StickyOffsetY = 20;
        MeasureAndArrange(panel, 200);

        panel.IsStickyPinned.ShouldBeFalse();
    }

    [Fact]
    public void Sticky_Panel_Does_Not_Clip_Content_When_Pinned()
    {
        var panel = CreatePanel();

        panel.StickyOffsetY = 56;
        MeasureAndArrange(panel, 200);

        panel.Children[1].Bounds.Y.ShouldBe(56);
        panel.Children[2].Bounds.Y.ShouldBe(70);
        panel.Children[2].Clip.ShouldBeNull();
    }

    [Fact]
    public void Sticky_Host_Uses_Gallery_Control_Token_And_Template_Conventions()
    {
        var hostSource    = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryStickyTabsHost.cs");
        var panelSource   = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryStickyTabsPanel.cs");
        var themeSource   = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryStickyTabsHostTheme.axaml");
        var tokenSource   = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryStickyTabsHostToken.cs");
        var provider      = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryControlThemesProvider.axaml");

        hostSource.ShouldContain("HeaderProperty");
        hostSource.ShouldContain("StickyContentProperty");
        hostSource.ShouldContain("ContentProperty");
        hostSource.ShouldContain("IsStickyMirrorEnabledProperty");
        hostSource.ShouldContain("HasStickyContentProperty");
        hostSource.ShouldContain("[Content]");
        hostSource.ShouldContain("RegisterTokenResourceScope(GalleryStickyTabsHostToken.ScopeProvider)");
        hostSource.ShouldContain("ScopeAwareAdornerLayer.GetLayer(this)");
        hostSource.ShouldContain("StickyMirrorZIndex       = -1");
        hostSource.ShouldContain("VisualBrush");
        hostSource.ShouldContain("IsHitTestVisible = false");
        hostSource.ShouldContain("ZIndex           = StickyMirrorZIndex");
        hostSource.ShouldContain("RemoveStickyMirror()");
        hostSource.ShouldContain("!IsStickyMirrorEnabled");
        hostSource.ShouldNotContain("Popup");
        hostSource.ShouldNotContain("OverlayLayer.GetOverlayLayer(this)");

        panelSource.ShouldContain("ScrollChanged += HandleScrollChanged");
        panelSource.ShouldContain("ScrollChanged -= HandleScrollChanged");
        panelSource.ShouldContain("StickyOffsetYProperty");
        panelSource.ShouldContain("IsStickyPinnedProperty");
        panelSource.ShouldContain("Math.Max(naturalY, StickyOffsetY)");
        panelSource.ShouldNotContain("ApplyStickyClip");
        panelSource.ShouldNotContain("StickyClipGeometry");

        tokenSource.ShouldContain("[ControlDesignToken]");
        tokenSource.ShouldContain("StickyContentPadding");
        tokenSource.ShouldContain("StickyBackground");
        tokenSource.ShouldContain("StickyBorderBrush");

        themeSource.ShouldContain("PART_ScrollViewer");
        themeSource.ShouldContain("PART_StickyPanel");
        themeSource.ShouldContain("PART_StickyContentHost");
        themeSource.ShouldContain("gallery:GalleryStickyTabsPanel");
        themeSource.ShouldContain("GalleryStickyTabsHostTokenResource");
        themeSource.ShouldContain("StickyContentPadding");
        themeSource.ShouldContain("StickyBackground");
        themeSource.ShouldContain("StickyBorderBrush");
        themeSource.ShouldContain("IsVisible=\"{TemplateBinding HasStickyContent}\"");
        themeSource.ShouldContain("ZIndex=\"1\"");
        themeSource.ShouldNotContain("<VisualLayerManager>");
        themeSource.ShouldContain("<ContentPresenter Content=\"{TemplateBinding Content}\" />");
        provider.ShouldContain("<ResourceInclude Source=\"GalleryStickyTabsHostTheme.axaml\" />");
    }

    [Fact]
    public void Sticky_Host_Keeps_Sticky_Content_In_Inline_Presenter_When_Pinned()
    {
        var hostSource = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryStickyTabsHost.cs");

        hostSource.ShouldNotContain("MoveStickyContentToOverlay");
        hostSource.ShouldNotContain("MoveStickyContentInline");
        hostSource.ShouldNotContain("_inlineStickyContentPresenter.Content");
    }

    [Fact]
    public void Sticky_Host_Collapses_Sticky_Content_Row_When_No_StickyContent_Is_Set()
    {
        var host = new GalleryStickyTabsHost
        {
            Header  = new FixedSizeControl(320, 80),
            Content = new FixedSizeControl(320, 240)
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 220,
            Content = host
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var stickyContentHost = host.GetVisualDescendants()
                                        .OfType<Control>()
                                        .Single(control => control.Name == "PART_StickyContentHost");
            stickyContentHost.IsVisible.ShouldBeFalse();

            host.StickyContent = new FixedSizeControl(320, 40);
            Dispatcher.UIThread.RunJobs();
            stickyContentHost.IsVisible.ShouldBeTrue();

            host.StickyContent = null;
            Dispatcher.UIThread.RunJobs();
            stickyContentHost.IsVisible.ShouldBeFalse();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Sticky_Host_Mirror_Layer_Is_Above_Native_Adorner_Layer()
    {
        var host = CreateStickyHost(new FixedSizeControl(320, 800));
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child              = host
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 180,
            Content = visualLayerManager
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var nativeAdornerLayer = AdornerLayer.GetAdornerLayer(host);
            nativeAdornerLayer.ShouldNotBeNull();
            var nativeAdorner = new Border
            {
                Width      = 20,
                Height     = 20,
                Background = Brushes.Red
            };
            AdornerLayer.SetAdornedElement(nativeAdorner, host);
            nativeAdornerLayer.Children.Add(nativeAdorner);

            PinStickyContent(host);

            var stickyMirrorLayer = visualLayerManager.GetVisualDescendants()
                                                      .OfType<ScopeAwareAdornerLayer>()
                                                      .Single(layer => ReferenceEquals(layer.GetVisualParent(),
                                                          visualLayerManager));

            nativeAdornerLayer.Children.Count.ShouldBe(1);
            stickyMirrorLayer.Children
                             .OfType<Border>()
                             .Count(border => border.Background is VisualBrush)
                             .ShouldBe(1);
            stickyMirrorLayer.ZIndex.ShouldBeGreaterThan(nativeAdornerLayer.ZIndex);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Window_Message_Layer_Is_Above_Sticky_Mirror_Layer()
    {
        var host = CreateStickyHost(new FixedSizeControl(320, 800));
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child              = host
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 180,
            Content = visualLayerManager
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            PinStickyContent(host);
            var stickyMirrorLayer = GetStickyMirrorLayer(visualLayerManager);

            using var messageManager = new WindowMessageManager(window);
            messageManager.Show(new Message("Action in progress...", MessageType.Loading, expiration: TimeSpan.Zero));
            Dispatcher.UIThread.RunJobs();

            var messageLayer = messageManager.GetVisualParent<Control>();
            messageLayer.ShouldNotBeNull();
            messageLayer.ZIndex.ShouldBeGreaterThan(stickyMirrorLayer.ZIndex);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Window_Notification_Layer_Is_Above_Sticky_Mirror_Layer()
    {
        var host = CreateStickyHost(new FixedSizeControl(320, 800));
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child              = host
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 180,
            Content = visualLayerManager
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            PinStickyContent(host);
            var stickyMirrorLayer = GetStickyMirrorLayer(visualLayerManager);

            using var notificationManager = new WindowNotificationManager(window);
            notificationManager.Show(new Notification("Notice", "Action in progress...",
                NotificationType.Information,
                expiration: TimeSpan.Zero));
            Dispatcher.UIThread.RunJobs();

            var notificationLayer = notificationManager.GetVisualParent<Control>();
            notificationLayer.ShouldNotBeNull();
            notificationLayer.ZIndex.ShouldBeGreaterThan(stickyMirrorLayer.ZIndex);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static GalleryStickyTabsHost CreateStickyHost(Control content)
    {
        return new GalleryStickyTabsHost
        {
            Header        = new FixedSizeControl(320, 80),
            StickyContent = new FixedSizeControl(320, 40),
            Content       = content
        };
    }

    private static void PinStickyContent(GalleryStickyTabsHost host)
    {
        var panel = host.GetVisualDescendants()
                        .OfType<GalleryStickyTabsPanel>()
                        .Single();
        var scrollViewer = panel.GetVisualAncestors()
                                .OfType<AvaloniaScrollViewer>()
                                .First();
        scrollViewer.Offset = new Vector(0, 96);
        Dispatcher.UIThread.RunJobs();

        panel.IsStickyPinned.ShouldBeTrue();
    }

    private static ScopeAwareAdornerLayer GetStickyMirrorLayer(VisualLayerManager visualLayerManager)
    {
        return visualLayerManager.GetVisualDescendants()
                                 .OfType<ScopeAwareAdornerLayer>()
                                 .Single(layer => ReferenceEquals(layer.GetVisualParent(),
                                     visualLayerManager));
    }

    private static GalleryStickyTabsPanel CreatePanel()
    {
        var panel = new GalleryStickyTabsPanel
        {
            StickyIndex = 1
        };

        panel.Children.Add(new FixedSizeControl(100, 40));
        panel.Children.Add(new FixedSizeControl(100, 30));
        panel.Children.Add(new FixedSizeControl(100, 100));
        return panel;
    }

    private static void MeasureAndArrange(Control control, double width)
    {
        control.Measure(new Size(width, double.PositiveInfinity));
        control.Arrange(new Rect(0, 0, width, control.DesiredSize.Height));
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }

    private sealed class FixedSizeControl : Control
    {
        private readonly Size _size;

        public FixedSizeControl(double width, double height)
        {
            _size = new Size(width, height);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(Math.Min(_size.Width, availableSize.Width), _size.Height);
        }
    }
}
