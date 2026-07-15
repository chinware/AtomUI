using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowResizeArtifactTests
{
    [Fact]
    public void Visual_Layers_Use_One_Window_Frame_Clip_Without_Changing_Their_Coordinate_System()
    {
        var clientSize = new Size(900, 650);
        var shadow     = new Thickness(36, 27, 36, 45);
        var document   = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        XNamespace av   = "https://github.com/avaloniaui";
        XNamespace atom = "https://atomui.net";

        WindowVisualLayerClip.CalculateClipBounds(clientSize, shadow)
              .ShouldBe(new Rect(36, 27, 828, 578));
        WindowVisualLayerClip.CalculateClipBounds(clientSize, default)
                             .ShouldBe(new Rect(clientSize));

        var layerManagers = document.Descendants(av + "VisualLayerManager")
                                    .Where(element =>
                                        (string?)element.Attribute("Name") == "PART_VisualLayerManager")
                                    .ToList();
        layerManagers.Count.ShouldBe(3);
        layerManagers.Count(manager =>
            manager.Parent?.Name == atom + "WindowVisualLayerClip").ShouldBe(2);
    }

    [Fact]
    public void Linux_Window_Preserves_Shadow_And_Scales_Only_The_Managed_Resize_Grip()
    {
        var tokenSource  = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/WindowToken.cs"));
        var chromeSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs"));
        var reflectionSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/WindowDrawnDecorationsReflectionExtensions.cs"));
        var waylandSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/WaylandWindowChromeManager.cs"));

        tokenSource.ShouldContain("FrameShadows             = SharedToken.BoxShadowsSecondary;");
        tokenSource.ShouldNotContain("ScaleFrameShadows");
        waylandSource.ShouldContain("private const double ManagedResizeGripScale = 1.0 / 3.0;");
        waylandSource.ShouldContain("Window.TryTakeOverManagedResizeGrip(");
        waylandSource.ShouldContain("ManagedResizeGripScale,");
        reflectionSource.ShouldContain("decorations.FrameThickness");
        reflectionSource.ShouldContain("decorations.ShadowThickness");
        reflectionSource.ShouldContain("(frame.Left + shadow.Left) * scale");
        reflectionSource.ShouldContain("(frame.Top + shadow.Top) * scale");
        reflectionSource.ShouldContain("(frame.Right + shadow.Right) * scale");
        reflectionSource.ShouldContain("(frame.Bottom + shadow.Bottom) * scale");
        reflectionSource.ShouldContain("gripThicknessProperty.SetValue(resizeGrips, default(Thickness));");
    }

    [Fact]
    public void Wayland_Resize_Uses_The_Platform_Reported_Size_Unmodified()
    {
        var waylandSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/WaylandWindowChromeManager.cs"));
        var windowSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Window.cs"));
        var resizerSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/WindowResizer.cs"));

        waylandSource.ShouldNotContain("platformImpl.Resized =");
        waylandSource.ShouldNotContain("CorrectPlatformResize");
        windowSource.ShouldNotContain("NotifyResizeStarted");
        windowSource.ShouldNotContain("NotifyResizeFinished");
        resizerSource.ShouldNotContain("NotifyResizeStarted");
        resizerSource.ShouldContain("TargetWindow.BeginResizeDrag(windowEdge, e);");
    }

    [Theory]
    [SupportedOSPlatform("linux")]
    [InlineData(36, 26.666666666666668, 36, 45.333333333333336, 36, 27, 36, 45)]
    [InlineData(35.5, 26.4, 36.49, 45.5, 36, 26, 36, 46)]
    public void Wayland_Shadow_Extents_Are_Integral_At_The_Protocol_Boundary(
        double left,
        double top,
        double right,
        double bottom,
        double expectedLeft,
        double expectedTop,
        double expectedRight,
        double expectedBottom)
    {
        var normalized = WaylandWindowChromeManager.NormalizeWaylandShadowExtents(
            new Thickness(left, top, right, bottom));
        normalized.ShouldBe(new Thickness(expectedLeft, expectedTop, expectedRight, expectedBottom));

        const int surfaceWidth  = 1000;
        const int surfaceHeight = 900;
        var geometryWidth = (int)Math.Ceiling(surfaceWidth - normalized.Right) - (int)normalized.Left;
        var geometryHeight = (int)Math.Ceiling(surfaceHeight - normalized.Bottom) - (int)normalized.Top;

        (geometryWidth + normalized.Left + normalized.Right).ShouldBe(surfaceWidth);
        (geometryHeight + normalized.Top + normalized.Bottom).ShouldBe(surfaceHeight);
    }

    [Fact]
    [SupportedOSPlatform("linux")]
    public void Wayland_Shadow_Input_Region_Preserves_Only_The_Managed_Resize_Band()
    {
        var region = WaylandWindowChromeManager.CalculateInputRegion(
            new Avalonia.Size(900, 600),
            new Avalonia.Thickness(24),
            new Avalonia.Thickness(8),
            canResize: true,
            isCsdEnabled: true,
            Avalonia.Controls.WindowState.Normal);

        region.ShouldBe(new Avalonia.PixelRect(16, 16, 868, 568));

        WaylandWindowChromeManager.CalculateInputRegion(
                new Avalonia.Size(900, 600),
                new Avalonia.Thickness(24),
                new Avalonia.Thickness(8),
                canResize: false,
                isCsdEnabled: true,
                Avalonia.Controls.WindowState.Normal)
            .ShouldBe(new Avalonia.PixelRect(24, 24, 852, 552));

        WaylandWindowChromeManager.CalculateInputRegion(
                new Avalonia.Size(900, 600),
                new Avalonia.Thickness(24),
                new Avalonia.Thickness(8),
                canResize: true,
                isCsdEnabled: true,
                Avalonia.Controls.WindowState.Maximized)
            .ShouldBe(new Avalonia.PixelRect(0, 0, 900, 600));

        WaylandWindowChromeManager.CalculateInputRegion(
                new Avalonia.Size(900, 600),
                new Avalonia.Thickness(24),
                new Avalonia.Thickness(8),
                canResize: true,
                isCsdEnabled: false,
                Avalonia.Controls.WindowState.Normal)
            .ShouldBe(new Avalonia.PixelRect(0, 0, 900, 600));
    }

    [Fact]
    public void Wayland_Shadow_Input_Region_Uses_WlSurface_Protocol_And_Tracks_Size()
    {
        var chromeSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/WaylandWindowChromeManager.cs"));
        var reflectionSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/WaylandWindowReflectionExtensions.cs"));
        var nativeSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Native/Linux/WaylandWindowUtils.cs"));

        chromeSource.ShouldContain("e.Property == Visual.BoundsProperty");
        chromeSource.ShouldContain("RequestFrameGeometryUpdate();");
        chromeSource.ShouldContain("Window.TrySetWaylandInputRectangle(");
        chromeSource.ShouldContain("shadowThickness.Left - effectiveGrip.Left");
        chromeSource.ShouldContain("Math.Ceiling(insetLeft)");
        chromeSource.ShouldContain("Math.Floor(surfaceWidth - insetRight)");
        chromeSource.ShouldNotContain("RenderScaling");

        reflectionSource.ShouldContain("WaylandWindowUtils.SetInputRectangle");
        nativeSource.ShouldContain("wlCompositor.CreateRegion()");
        nativeSource.ShouldContain("region.Add(x, y, width, height);");
        nativeSource.ShouldContain("wlSurface.SetInputRegion(region);");
        nativeSource.ShouldContain("region.Destroy();");
        nativeSource.IndexOf("SetInputRegion", StringComparison.Ordinal)
                    .ShouldBeLessThan(nativeSource.IndexOf("region.Destroy", StringComparison.Ordinal));
    }

    [Fact]
    public void Browser_Wasm_Excludes_Wayland_Input_Region_Protocol_Assemblies()
    {
        var browserProject = File.ReadAllText(GetRepoFile(
            "controlgallery/AtomUIGallery.Browser/AtomUIGallery.Browser.csproj"));

        browserProject.ShouldContain("ExcludeDesktopPlatformAssembliesFromBrowserWasm");
        browserProject.ShouldContain("'%(FileName)' == 'Avalonia.Wayland'");
        browserProject.ShouldContain("'%(FileName)' == 'NWayland'");
    }

    [Fact]
    public void Window_Resizer_Hover_Cursors_Preserve_Directional_Semantics()
    {
        var source = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Themes/WindowResizerTheme.axaml"));
        var builderSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/DesktopAppBuilderExtensions.cs"));

        source.ShouldContain("Cursor=\"TopSide\"");
        source.ShouldContain("Cursor=\"BottomSide\"");
        source.ShouldContain("Cursor=\"LeftSide\"");
        source.ShouldContain("Cursor=\"RightSide\"");
        source.ShouldContain("Cursor=\"TopLeftCorner\"");
        source.ShouldContain("Cursor=\"TopRightCorner\"");
        source.ShouldContain("Cursor=\"BottomLeftCorner\"");
        source.ShouldContain("Cursor=\"BottomRightCorner\"");
        builderSource.ShouldNotContain("PreferSymmetricResizeCursorNames");
    }

    [Fact]
    public void Window_Resizer_Compiled_Theme_Uses_The_Expected_Hover_Cursors()
    {
        AvaloniaTestApp.EnsureInitialized();
        var resizer = new WindowResizer();
        Application.Current!.TryFindResource(typeof(WindowResizer), out var resource).ShouldBeTrue();
        resizer.Theme = resource.ShouldBeAssignableTo<ControlTheme>();
        var host = new Avalonia.Controls.Window
        {
            Width = 100,
            Height = 100,
            Content = resizer
        };
        host.Show();
        resizer.ApplyTemplate();

        var grips = resizer.GetVisualDescendants()
                           .OfType<Border>()
                           .Where(border => border.Tag is ResizeHandleLocation)
                           .ToDictionary(
                               border => (ResizeHandleLocation)border.Tag!,
                               border => border.Cursor?.ToString());

        grips[ResizeHandleLocation.North].ShouldBe("TopSide");
        grips[ResizeHandleLocation.South].ShouldBe("BottomSide");
        grips[ResizeHandleLocation.West].ShouldBe("LeftSide");
        grips[ResizeHandleLocation.East].ShouldBe("RightSide");
        grips[ResizeHandleLocation.NorthWest].ShouldBe("TopLeftCorner");
        grips[ResizeHandleLocation.NorthEast].ShouldBe("TopRightCorner");
        grips[ResizeHandleLocation.SouthWest].ShouldBe("BottomLeftCorner");
        grips[ResizeHandleLocation.SouthEast].ShouldBe("BottomRightCorner");
        host.Close();
    }

    [Fact]
    public void Window_Prepares_Linux_Initial_Client_Size_Before_Show()
    {
        var windowSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs"));
        var chromeSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs"));
        var x11Source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/X11WindowChromeManager.cs"));

        windowSource.ShouldContain("public override void Show()");
        windowSource.ShouldContain("_platformChromeManager?.PrepareInitialShowState();");
        windowSource.ShouldContain("base.Show();");
        windowSource.IndexOf("_platformChromeManager?.PrepareInitialShowState();", StringComparison.Ordinal)
                    .ShouldBeLessThan(windowSource.IndexOf("base.Show();", StringComparison.Ordinal));

        chromeSource.ShouldContain("PrepareInitialShowState()");
        x11Source.ShouldContain("SizeToContent != SizeToContent.Manual");
        x11Source.ShouldContain("WindowState is WindowState.Minimized or WindowState.Maximized or WindowState.FullScreen");
        x11Source.ShouldContain("SetPlatformChromeClientSize(clientSize);");
        x11Source.ShouldContain("Window.Width = clientSize.Width;");
        x11Source.ShouldContain("Window.Height = clientSize.Height;");
        x11Source.ShouldContain("TryGetInitialStartupPosition");
        x11Source.ShouldContain("WindowStartupLocation.Manual");
        x11Source.ShouldContain("ConfigureLinuxInitialWindowGeometry");
        chromeSource.ShouldContain("_initialShowStatePrepared");
    }

    [Fact]
    public void Linux_Initial_Geometry_Updates_X11_Size_Hints_Before_Map()
    {
        var extensionSource = File.ReadAllText(GetRepoFile("src/AtomUI.Native/WindowExtensions.cs"));
        var linuxSource     = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Linux.cs"));
        var interopSource   = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Interop.cs"));

        extensionSource.ShouldContain("ConfigureLinuxInitialWindowGeometry");
        extensionSource.ShouldContain("WindowUtilsLinux.ConfigureInitialWindowGeometry");

        linuxSource.ShouldContain("ConfigureInitialWindowGeometry");
        linuxSource.ShouldContain("XSetWMNormalHints");
        linuxSource.ShouldContain("XMoveResizeWindow");
        linuxSource.IndexOf("XSetWMNormalHints", StringComparison.Ordinal)
                   .ShouldBeLessThan(linuxSource.IndexOf("XMoveResizeWindow", StringComparison.Ordinal));

        interopSource.ShouldContain("USPosition");
        interopSource.ShouldContain("USSize");
        interopSource.ShouldContain("PSize");
    }

    [Fact]
    public void Linux_Client_Drawn_Shadow_Publishes_X11_Csd_Frame_Extents()
    {
        var windowSource    = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs"));
        var chromeSource    = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/X11WindowChromeManager.cs"));
        var linuxChromeSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs"));
        var extensionSource = File.ReadAllText(GetRepoFile("src/AtomUI.Native/WindowExtensions.cs"));
        var linuxSource     = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Linux.cs"));
        var interopSource   = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Interop.cs"));

        windowSource.ShouldContain("_platformChromeManager = WindowChromeManager.Attach(this);");
        linuxChromeSource.ShouldContain("_window.ScalingChanged += HandleScalingChanged;");
        chromeSource.ShouldContain("ApplyX11CsdFrameExtents();");
        chromeSource.ShouldContain("Window.WindowState is WindowState.Normal ? Window.FrameShadowThickness : default");
        chromeSource.ShouldContain("Window.SetLinuxX11CsdFrameExtents(frameExtents);");
        windowSource.ShouldNotContain("EnsureMinSizeForDecorations");
        windowSource.IndexOf("_platformChromeManager?.PrepareInitialShowState();", StringComparison.Ordinal)
                    .ShouldBeLessThan(windowSource.IndexOf("base.Show();", StringComparison.Ordinal));

        extensionSource.ShouldContain("SetLinuxX11CsdFrameExtents");
        extensionSource.ShouldContain("handle.HandleDescriptor != \"XID\"");
        extensionSource.ShouldContain("ToPixelMargin(frameExtents.Left, scaling)");
        extensionSource.ShouldContain("ToPixelMargin(frameExtents.Top, scaling)");
        extensionSource.ShouldContain("ToPixelMargin(frameExtents.Right, scaling)");
        extensionSource.ShouldContain("ToPixelMargin(frameExtents.Bottom, scaling)");

        linuxSource.ShouldContain("X11CsdFrameExtentsPropertyName = \"_GTK_FRAME_EXTENTS\"");
        linuxSource.ShouldContain("SetX11CsdFrameExtents");
        linuxSource.ShouldContain("Mutter and KWin");
        linuxSource.ShouldContain("left, right, top, bottom");
        linuxSource.ShouldContain("XInternAtom");
        linuxSource.ShouldContain("XChangeProperty");
        linuxSource.IndexOf("ToCardinal(left)", StringComparison.Ordinal)
                   .ShouldBeLessThan(linuxSource.IndexOf("ToCardinal(right)", StringComparison.Ordinal));
        linuxSource.IndexOf("ToCardinal(right)", StringComparison.Ordinal)
                   .ShouldBeLessThan(linuxSource.IndexOf("ToCardinal(top)", StringComparison.Ordinal));
        linuxSource.IndexOf("ToCardinal(top)", StringComparison.Ordinal)
                   .ShouldBeLessThan(linuxSource.IndexOf("ToCardinal(bottom)", StringComparison.Ordinal));

        interopSource.ShouldContain("PropModeReplace");
        interopSource.ShouldContain("XInternAtom");
        interopSource.ShouldContain("XChangeProperty");
        interopSource.ShouldContain("IntPtr[] data");

        windowSource.ShouldNotContain("GtkFrameExtents");
        chromeSource.ShouldNotContain("GtkFrameExtents");
        extensionSource.ShouldNotContain("GtkFrameExtents");
        linuxSource.ShouldNotContain("GtkFrameExtents");
    }

    [Fact]
    public void Linux_Window_Frame_Geometry_Updates_Are_Coalesced_Before_Render()
    {
        var windowSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs"));
        var chromeSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs"));

        chromeSource.ShouldContain("HandleFrameShadowChanged");
        chromeSource.ShouldContain("ConfigureTitleBarHeightHint");
        chromeSource.ShouldContain("_hasPendingFrameShadowThickness");
        chromeSource.ShouldContain("_hasPendingTitleBarHeightHint");
        chromeSource.ShouldContain("RequestFrameGeometryUpdate");
        chromeSource.ShouldContain("ApplyPendingFrameGeometryUpdate");
        chromeSource.ShouldContain("ApplyPendingFrameGeometryInputs");
        chromeSource.ShouldContain("UpdateFrameGeometry");
        chromeSource.ShouldContain("Avalonia.Threading.DispatcherPriority.Render");

        windowSource.ShouldContain("FrameShadowProperty.Changed.AddClassHandler<Window>((window, args) =>");
        windowSource.ShouldContain("window.HandleFrameShadowPropertyChanged(args.GetNewValue<BoxShadows>())");
        windowSource.ShouldContain("_platformChromeManager.HandleFrameShadowChanged(frameShadow);");
        windowSource.ShouldContain("_platformChromeManager.ConfigureTitleBarHeightHint(e.NewSize.Height);");
        windowSource.ShouldContain("_platformChromeManager?.HandlePropertyChanged(change.Property);");
        chromeSource.ShouldContain("RequestFrameGeometryUpdate();");

        chromeSource.IndexOf("protected void RequestFrameGeometryUpdate()", StringComparison.Ordinal)
                    .ShouldBeLessThan(chromeSource.IndexOf("private void ApplyPendingFrameGeometryUpdate()", StringComparison.Ordinal));
        chromeSource.IndexOf("private void ApplyPendingFrameGeometryUpdate()", StringComparison.Ordinal)
                    .ShouldBeLessThan(chromeSource.IndexOf("private void ApplyPendingFrameGeometryInputs()", StringComparison.Ordinal));
        chromeSource.ShouldContain("ApplyPendingFrameGeometryInputs();\n        UpdateFrameGeometry();");
        chromeSource.IndexOf("Dispatcher.Post(ApplyPendingFrameGeometryUpdate", StringComparison.Ordinal)
                    .ShouldBeLessThan(chromeSource.IndexOf("private void ApplyPendingFrameGeometryUpdate()", StringComparison.Ordinal));

        windowSource.ShouldNotContain("RequestLinuxFrameGeometryUpdate");
        windowSource.ShouldNotContain("ApplyPendingLinuxFrameGeometryUpdate");
        windowSource.ShouldNotContain("UpdateLinuxFrameGeometry");
    }

    [Fact]
    public void Linux_Shadow_Input_Region_Keeps_Only_Ten_Dip_Resize_Band()
    {
        var chromeSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/X11WindowChromeManager.cs"));
        var inputSource  = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/ClickThroughShadowExtensions.cs"));

        chromeSource.ShouldContain("Window.AttachClickThroughShadow(");
        chromeSource.ShouldContain("s_shadowInputRegionAffectsProperties");
        chromeSource.ShouldContain("() => Window.FrameShadowThickness");
        chromeSource.ShouldContain("ClickThroughShadowExtensions.DefaultResizeBand");
        chromeSource.ShouldContain("CanResizeProperty");
        chromeSource.ShouldContain("FrameShadowThicknessProperty");
        chromeSource.ShouldContain("IsCsdEnabledProperty");

        inputSource.ShouldContain("public const double DefaultResizeBand = 10.0;");
        inputSource.ShouldContain("var effectiveResizeBand = window.CanResize ? resizeBand : 0;");
        inputSource.ShouldContain("shadowThickness.Left - effectiveResizeBand");
        inputSource.ShouldContain("shadowThickness.Top - effectiveResizeBand");
        inputSource.ShouldContain("window.SetWindowInputRectangle(x, y, w, h);");
        inputSource.ShouldContain("window.ResetWindowInputRegion(fullW, fullH);");
    }

    [Fact]
    public void Native_Linux_Window_Utilities_Do_Not_Keep_Legacy_Ignore_Mouse_Shape_Query_Code()
    {
        var extensionSource = File.ReadAllText(GetRepoFile("src/AtomUI.Native/WindowExtensions.cs"));
        var linuxSource     = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Linux.cs"));
        var interopSource   = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Interop.cs"));

        extensionSource.ShouldNotContain("SetWindowIgnoreMouseEventsLinux");
        extensionSource.ShouldNotContain("IsWindowIgnoreMouseEventsLinux");
        linuxSource.ShouldNotContain("SetWindowIgnoreMouseEventsLinux");
        linuxSource.ShouldNotContain("IsWindowIgnoreMouseEventsLinux");
        linuxSource.ShouldNotContain("GetWindowGeometry");

        interopSource.ShouldNotContain("xcb_get_geometry");
        interopSource.ShouldNotContain("xcb_get_geometry_reply");
        interopSource.ShouldNotContain("xcb_shape_get_rectangles");
        interopSource.ShouldNotContain("xcb_shape_get_rectangles_reply");
        interopSource.ShouldNotContain("xcb_shape_get_rectangles_rectangles_length");
        interopSource.ShouldNotContain("xcb_get_geometry_reply_t");
        interopSource.ShouldNotContain("xcb_shape_get_rectangles_reply_t");
        interopSource.ShouldNotContain("xcb_shape_query_version_reply_t");
    }

    [Fact]
    public void Linux_NonCsd_Window_Template_Clips_Content_To_Window_CornerRadius()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";
        XNamespace atom = "https://atomui.net";

        var linuxTemplateStyle = document.Descendants(av + "Style")
                                         .Single(element =>
                                             (string?)element.Attribute("Selector") ==
                                             "^[OsType=Linux][IsCsdEnabled=False]" &&
                                             element.Descendants(av + "ControlTemplate").Any());

        var contentClip = linuxTemplateStyle.Descendants(av + "Border")
                                            .Single(element =>
                                                (string?)element.Attribute("Name") == "WindowContentClip");

        contentClip.Attribute("CornerRadius").ShouldNotBeNull().Value.ShouldBe("{TemplateBinding CornerRadius}");
        contentClip.Attribute("ClipToBounds").ShouldNotBeNull().Value.ShouldBe("True");
        contentClip.Attribute("Margin").ShouldNotBeNull().Value.ShouldBe("{TemplateBinding FrameShadowThickness}");
        contentClip.Ancestors(atom + "WindowVisualLayerClip").Count().ShouldBe(1);

        var dockPanel = contentClip.Elements(av + "DockPanel").Single();
        dockPanel.Attribute("LastChildFill").ShouldNotBeNull().Value.ShouldBe("True");
        dockPanel.Attribute("Margin").ShouldBeNull();
    }

    [Fact]
    public void Linux_Csd_Window_Template_Clips_Content_Surface_To_Bottom_Window_CornerRadius()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";
        XNamespace atom = "https://atomui.net";

        document.Descendants()
                .ShouldContain(element =>
                    element.Name.LocalName == "CornerRadiusFilterConverter" &&
                    element.Attributes().Any(attribute =>
                        attribute.Name.LocalName == "Key" &&
                        (string?)attribute == "WindowContentBottomCornerRadiusFilter") &&
                    (string?)element.Attribute("Filter") == "BottomLeft, BottomRight");

        var csdTemplateStyle = document.Descendants(av + "Style")
                                       .Single(element =>
                                           (string?)element.Attribute("Selector") == "^[IsCsdEnabled=True]" &&
                                           element.Descendants(av + "ControlTemplate").Any());

        csdTemplateStyle.Descendants(av + "Border")
                        .ShouldNotContain(element =>
                            (string?)element.Attribute("Name") == "WindowContentClip");

        var layerClip = csdTemplateStyle.Descendants(atom + "WindowVisualLayerClip").Single();
        layerClip.Attribute("ShadowThickness").ShouldNotBeNull().Value.ShouldBe(
            "{TemplateBinding FrameShadowThickness}");
        layerClip.Attribute("CornerRadius").ShouldNotBeNull().Value.ShouldBe(
            "{TemplateBinding CornerRadius}");

        var visualLayerManager = layerClip.Elements(av + "VisualLayerManager")
                                          .Single(element =>
                                              (string?)element.Attribute("Name") == "PART_VisualLayerManager");
        var contentPanel = visualLayerManager.Elements(av + "Panel").Single();

        contentPanel.Attribute("Margin").ShouldNotBeNull().Value.ShouldBe(
            "{Binding $parent[Window].WindowDecorationMargin}");
        contentPanel.Attribute("ClipToBounds").ShouldBeNull();

        var contentFrameLayer = contentPanel.Elements(av + "ContentPresenter")
                                            .Single(element =>
                                                (string?)element.Attribute("Name") == "ContentFrameLayer");

        contentFrameLayer.Attribute("CornerRadius").ShouldNotBeNull().Value.ShouldBe(
            "{TemplateBinding CornerRadius, Converter={StaticResource WindowContentBottomCornerRadiusFilter}}");
        contentFrameLayer.Attribute("ClipToBounds").ShouldNotBeNull().Value.ShouldBe("True");

        var contentFrame = contentPanel.Elements(av + "Border")
                                       .Single(element =>
                                           (string?)element.Attribute("Name") == "ContentFrame");

        contentFrame.Attribute("CornerRadius").ShouldNotBeNull().Value.ShouldBe(
            "{TemplateBinding CornerRadius, Converter={StaticResource WindowContentBottomCornerRadiusFilter}}");
        contentFrame.Attribute("ClipToBounds").ShouldNotBeNull().Value.ShouldBe("True");
    }

    [Fact]
    public void Windows_Window_Has_No_NonCsd_Template_And_Uses_Opaque_Csd_Background()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        var decorationsDocument = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Themes/WindowDrawnDecorationsTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

        document.Descendants(av + "Style")
                .ShouldNotContain(element =>
                    (string?)element.Attribute("Selector") == "^[OsType=Windows][IsCsdEnabled=False]");

        var windowsStyle = document.Descendants(av + "Style")
                                   .Single(element =>
                                       (string?)element.Attribute("Selector") == "^[OsType=Windows]");

        windowsStyle.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "TransparencyLevelHint" &&
            (string?)setter.Attribute("Value") == "None");

        decorationsDocument.Descendants(av + "Style")
                           .Single(style =>
                               (string?)style.Attribute("Selector") == "^/template/ Border#PART_WindowFrame")
                           .Elements(av + "Setter")
                           .ShouldContain(setter =>
                               (string?)setter.Attribute("Property") == "Background" &&
                               (string?)setter.Attribute("Value") == "{atom:WindowTokenResource DefaultBackground}");
    }

    [Fact]
    public void Windows_Window_Uses_Avalonia_Csd_Without_The_Legacy_Chrome_Hook()
    {
        var managerSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/WindowChromeManager.cs"));
        var reflectionSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/WindowDrawnDecorationsReflectionExtensions.cs"));
        var windowSource  = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs"));
        var nativeSource  = File.ReadAllText(GetRepoFile("src/AtomUI.Native/WindowExtensions.cs"));
        var interopSource = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Windows/WindowUtils.Interop.cs"));
        var captionSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/CaptionButtonGroup.cs"));
        var document = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        var decorationsDocument = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Themes/WindowDrawnDecorationsTheme.axaml"));
        var captionDocument = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonGroupTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";
        XNamespace atom = "https://atomui.net";

        string? GetCaptionButtonRole(string name)
        {
            return (string?)captionDocument.Descendants(atom + "WindowsCaptionButton")
                                           .Single(element => (string?)element.Attribute("Name") == name)
                                           .Attribute("WindowDecorationProperties.ElementRole");
        }

        managerSource.ShouldNotContain("WindowsWindowChromeManager.Attach(window)");
        File.Exists(Path.Combine(
            Path.GetDirectoryName(GetRepoFile(
                "src/AtomUI.Desktop.Controls/Window/WindowChromeManager.cs"))!,
            "WindowsWindowChromeManager.cs")).ShouldBeFalse();
        File.Exists(Path.Combine(
            Path.GetDirectoryName(GetRepoFile(
                "src/AtomUI.Desktop.Controls/Window/WindowChromeManager.cs"))!,
            "WindowsInactiveFramePolicy.cs")).ShouldBeFalse();
        windowSource.ShouldContain("else if (OperatingSystem.IsWindows())");
        windowSource.ShouldContain("IsCsdEnabled = true;");
        windowSource.ShouldContain("CalculateWindowsCsdMinimumHeight");
        windowSource.ShouldContain("SetCurrentValue(MinHeightProperty, minimumHeight);");
        windowSource.ShouldContain("PointerCaptureLost");
        windowSource.ShouldContain("ResetTitleBarMoveDragState();");
        windowSource.ShouldNotContain("_isDragging");
        windowSource.ShouldNotContain("IsWindowsDrawnDecorationsEnabledProperty");
        windowSource.ShouldNotContain("WindowsInactiveFramePolicy.Apply(this)");
        windowSource.ShouldNotContain("AddWndProcHookCallback");
        reflectionSource.ShouldNotContain("TryUpdateDrawnDecorations");
        reflectionSource.ShouldNotContain("\"UpdateDrawnDecorations\"");
        reflectionSource.ShouldNotContain("\"UpdateDrawnDecorationMargins\"");
        nativeSource.ShouldNotContain("WinWndProcHook");
        nativeSource.ShouldNotContain("ForceWinNonClientFrameChanged");
        interopSource.ShouldNotContain("WM_NCCALCSIZE");
        interopSource.ShouldNotContain("WM_NCHITTEST");
        interopSource.ShouldNotContain("HTMAXBUTTON");
        captionSource.ShouldNotContain("Win32Properties.AddWndProcHookCallback");
        captionSource.ShouldNotContain("EnableWindowsSnapLayout");
        GetCaptionButtonRole("PART_FullScreenButton").ShouldBe("FullScreenButton");
        GetCaptionButtonRole("PART_PinButton").ShouldBe("DecorationsElement");
        GetCaptionButtonRole("PART_MinimizeButton").ShouldBe("MinimizeButton");
        GetCaptionButtonRole("PART_MaximizeButton").ShouldBe("MaximizeButton");
        GetCaptionButtonRole("PART_CloseButton").ShouldBe("CloseButton");
        document.Descendants(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "ExtendClientAreaToDecorationsHint" &&
            (string?)setter.Attribute("Value") == "True");
        document.Descendants(av + "Setter").ShouldNotContain(setter =>
            (string?)setter.Attribute("Property") == "ExtendClientAreaToDecorationsHint" &&
            (string?)setter.Attribute("Value") == "False");
        var hasWindowsManagedFrameStyle = document.Descendants(av + "Style").Any(style =>
            ((string?)style.Attribute("Selector"))?.Contains("IsWindowsDrawnDecorationsEnabled=True") == true &&
            style.Elements(av + "Setter").Any(setter =>
                (string?)setter.Attribute("Property") is "TransparencyLevelHint" or "FrameShadow"));
        hasWindowsManagedFrameStyle.ShouldBeFalse();
        decorationsDocument.Descendants(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "DefaultFrameThickness" &&
            (string?)setter.Attribute("Value") == "1");
        decorationsDocument.Descendants(av + "Style").Single(style =>
                (string?)style.Attribute("Selector") == "^/template/ Border#PART_WindowFrame")
            .Elements(av + "Setter")
            .ShouldNotContain(setter => (string?)setter.Attribute("Property") == "BoxShadow");
        decorationsDocument.Descendants(av + "Style").Single(style =>
                (string?)style.Attribute("Selector") == "^:has-shadow /template/ Border#PART_WindowFrame")
            .Elements(av + "Setter")
            .ShouldContain(setter =>
                (string?)setter.Attribute("Property") == "BoxShadow" &&
                (string?)setter.Attribute("Value") == "{atom:WindowTokenResource FrameShadows}");
    }

    [Theory]
    [InlineData(40, 80)]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    public void Windows_Csd_Minimum_Height_Always_Leaves_A_Nonzero_Content_Surface(
        double titleBarHeight,
        double expectedMinimumHeight)
    {
        AtomUI.Desktop.Controls.Window.CalculateWindowsCsdMinimumHeight(titleBarHeight)
              .ShouldBe(expectedMinimumHeight);
    }

    [Fact]
    [SupportedOSPlatform("windows")]
    public void Windows_Csd_Window_Rejects_A_Minimum_Height_Below_Its_Safe_Content_Surface()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        AvaloniaTestApp.EnsureInitialized();
        var window = new AtomUI.Desktop.Controls.Window
        {
            TitleBarHeight = 40
        };

        window.MinHeight.ShouldBe(80);

        window.MinHeight = 20;
        window.MinHeight.ShouldBe(80);

        window.MinHeight = 160;
        window.MinHeight.ShouldBe(160);
    }

    [Fact]
    public void Windows_Caption_Buttons_Suppress_Stale_Hover_After_Window_State_Transitions()
    {
        var captionSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/CaptionButtonGroup.cs"));
        var themeSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowsCaptionButtonTheme.axaml"));

        captionSource.ShouldContain("if (stateChanged)");
        captionSource.ShouldContain("InvalidateWindowsCaptionButtonPointerOverVisualStates();");
        captionSource.ShouldContain("InvalidateWindowsCaptionButtonPointerOverVisualState(_fullScreenButton);");
        captionSource.ShouldContain("InvalidateWindowsCaptionButtonPointerOverVisualState(_maximizeButton);");
        captionSource.ShouldContain("button is WindowsCaptionButton windowsCaptionButton");
        themeSource.ShouldContain("^[IsPointerOverSuppressed=False]:pointerover");
    }

    [Fact]
    public void AtomUI_Defaults_Use_Redirection_Surface_For_Windows_Live_Resize()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Core/AppBuilderExtensions.cs"));

        source.ShouldContain(".With(new Win32PlatformOptions");
        source.ShouldContain("Win32RenderingMode.AngleEgl");
        source.ShouldContain("Win32RenderingMode.Software");
        source.ShouldContain("CompositionMode = [Win32CompositionMode.RedirectionSurface]");
        source.ShouldNotContain("WindowsAppBuilderDefaults");
        source.ShouldNotContain("Win32CompositionMode.WinUIComposition");
        source.ShouldNotContain("Win32CompositionMode.DirectComposition");
        source.ShouldNotContain("Win32CompositionMode.LowLatencyDxgiSwapChain");
        source.ShouldNotContain("ShouldRenderOnUIThread = true");
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

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
