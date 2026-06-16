using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowResizeArtifactTests
{
    [Fact]
    public void Window_Prepares_Linux_Initial_Client_Size_Before_Show()
    {
        var windowSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs"));
        var chromeSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs"));

        windowSource.ShouldContain("public override void Show()");
        windowSource.ShouldContain("_platformChromeManager?.PrepareInitialShowState();");
        windowSource.ShouldContain("base.Show();");
        windowSource.IndexOf("_platformChromeManager?.PrepareInitialShowState();", StringComparison.Ordinal)
                    .ShouldBeLessThan(windowSource.IndexOf("base.Show();", StringComparison.Ordinal));

        chromeSource.ShouldContain("PrepareInitialShowState()");
        chromeSource.ShouldContain("SizeToContent != SizeToContent.Manual");
        chromeSource.ShouldContain("WindowState is WindowState.Minimized or WindowState.Maximized or WindowState.FullScreen");
        chromeSource.ShouldContain("SetPlatformChromeClientSize(clientSize);");
        chromeSource.ShouldContain("_window.Width = clientSize.Width;");
        chromeSource.ShouldContain("_window.Height = clientSize.Height;");
        chromeSource.ShouldContain("TryGetInitialStartupPosition");
        chromeSource.ShouldContain("WindowStartupLocation.Manual");
        chromeSource.ShouldContain("ConfigureLinuxInitialWindowGeometry");
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
        var chromeSource    = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs"));
        var extensionSource = File.ReadAllText(GetRepoFile("src/AtomUI.Native/WindowExtensions.cs"));
        var linuxSource     = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Linux.cs"));
        var interopSource   = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Interop.cs"));

        windowSource.ShouldContain("_platformChromeManager = WindowChromeManager.Attach(this);");
        chromeSource.ShouldContain("_window.ScalingChanged += HandleScalingChanged;");
        chromeSource.ShouldContain("ApplyX11CsdFrameExtents();");
        chromeSource.ShouldContain("WindowState is WindowState.Normal ? _window.FrameShadowThickness : default");
        chromeSource.ShouldContain("_window.SetLinuxX11CsdFrameExtents(frameExtents);");
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

        chromeSource.IndexOf("private void RequestFrameGeometryUpdate()", StringComparison.Ordinal)
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
        var chromeSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/LinuxWindowChromeManager.cs"));
        var inputSource  = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/ClickThroughShadowExtensions.cs"));

        chromeSource.ShouldContain("_window.AttachClickThroughShadow(");
        chromeSource.ShouldContain("s_shadowInputRegionAffectsProperties");
        chromeSource.ShouldContain("() => _window.FrameShadowThickness");
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

        var dockPanel = contentClip.Elements(av + "DockPanel").Single();
        dockPanel.Attribute("LastChildFill").ShouldNotBeNull().Value.ShouldBe("True");
        dockPanel.Attribute("Margin").ShouldBeNull();
    }

    [Fact]
    public void Linux_Csd_Window_Template_Clips_Content_Surface_To_Bottom_Window_CornerRadius()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

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

        var contentPanel = csdTemplateStyle.Descendants(av + "VisualLayerManager")
                                           .Single(element =>
                                               (string?)element.Attribute("Name") == "PART_VisualLayerManager")
                                           .Elements(av + "Panel")
                                           .Single();

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
    public void Windows_Window_Template_Paints_Root_Background_And_Uses_Opaque_Transparency()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

        var windowsTemplateStyle = document.Descendants(av + "Style")
                                           .Single(element =>
                                               (string?)element.Attribute("Selector") ==
                                               "^[OsType=Windows][IsCsdEnabled=False]");
        var templateRoot = windowsTemplateStyle.Descendants(av + "ControlTemplate")
                                               .Single()
                                               .Elements(av + "Panel")
                                               .Single();

        templateRoot.Attribute("Background").ShouldNotBeNull().Value.ShouldBe("{TemplateBinding Background}");

        var windowsStyle = document.Descendants(av + "Style")
                                   .Single(element =>
                                       (string?)element.Attribute("Selector") == "^[OsType=Windows]");

        windowsStyle.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "TransparencyLevelHint" &&
            (string?)setter.Attribute("Value") == "None");
    }

    [Fact]
    public void AtomUI_Defaults_Prefer_Windows_Composition_That_Supports_Transparent_Popup_Windows()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Core/AppBuilderExtensions.cs"));

        source.ShouldContain("WithWin32TransparentPopupCompositionOptions");
        source.ShouldContain("if (OperatingSystem.IsWindows())");
        source.ShouldContain("[SupportedOSPlatform(\"windows\")]");
        source.ShouldContain("Avalonia.Win32PlatformOptions, Avalonia.Win32");
        source.ShouldContain("Avalonia.Win32RenderingMode, Avalonia.Win32");
        source.ShouldContain("Avalonia.Win32CompositionMode, Avalonia.Win32");
        source.ShouldContain("AngleEgl");
        source.ShouldContain("Software");
        source.ShouldContain("WinUIComposition");
        source.ShouldContain("DirectComposition");
        source.ShouldContain("RedirectionSurface");
        source.ShouldNotContain("LowLatencyDxgiSwapChain");
        source.IndexOf("\"WinUIComposition\"", StringComparison.Ordinal)
              .ShouldBeLessThan(source.IndexOf("\"DirectComposition\"", StringComparison.Ordinal));
        source.IndexOf("\"DirectComposition\"", StringComparison.Ordinal)
              .ShouldBeLessThan(source.IndexOf("\"RedirectionSurface\"", StringComparison.Ordinal));
    }

    [Fact]
    public void AtomUI_Core_Does_Not_Directly_Reference_Avalonia_Win32()
    {
        var projectFile = File.ReadAllText(GetRepoFile("src/AtomUI.Core/AtomUI.Core.csproj"));

        projectFile.ShouldNotContain("Avalonia.Win32");
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
