using System;
using System.IO;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Shouldly;
using Xunit;

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
        hostSource.ShouldContain("[Content]");
        hostSource.ShouldContain("RegisterTokenResourceScope(GalleryStickyTabsHostToken.ScopeProvider)");
        hostSource.ShouldContain("OverlayLayer.GetOverlayLayer(this)");
        hostSource.ShouldContain("VisualBrush");
        hostSource.ShouldContain("IsHitTestVisible = false");
        hostSource.ShouldContain("RemoveStickyMirror()");
        hostSource.ShouldContain("!IsStickyMirrorEnabled");
        hostSource.ShouldNotContain("Popup");

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
