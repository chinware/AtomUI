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
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs"));

        source.ShouldContain("public override void Show()");
        source.ShouldContain("PrepareLinuxInitialShowState();");
        source.ShouldContain("base.Show();");
        source.IndexOf("PrepareLinuxInitialShowState();", StringComparison.Ordinal)
              .ShouldBeLessThan(source.IndexOf("base.Show();", StringComparison.Ordinal));
        source.ShouldContain("OperatingSystem.IsLinux()");
        source.ShouldContain("SizeToContent != SizeToContent.Manual");
        source.ShouldContain("WindowState is WindowState.Minimized or WindowState.Maximized or WindowState.FullScreen");
        source.ShouldContain("ClientSize = clientSize;");
        source.ShouldContain("Width = clientSize.Width;");
        source.ShouldContain("Height = clientSize.Height;");
        source.ShouldContain("TryGetLinuxInitialStartupPosition");
        source.ShouldContain("WindowStartupLocation.Manual");
        source.ShouldContain("ConfigureLinuxInitialWindowGeometry");
        source.ShouldContain("_isLinuxInitialShowStatePrepared");
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
        var extensionSource = File.ReadAllText(GetRepoFile("src/AtomUI.Native/WindowExtensions.cs"));
        var linuxSource     = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Linux.cs"));
        var interopSource   = File.ReadAllText(GetRepoFile("src/AtomUI.Native/Linux/WindowUtils.Interop.cs"));

        windowSource.ShouldContain("ScalingChanged += HandleLinuxScalingChanged;");
        windowSource.ShouldContain("ApplyLinuxX11CsdFrameExtents();");
        windowSource.ShouldContain("WindowState is WindowState.Normal ? FrameShadowThickness : default");
        windowSource.ShouldContain("this.SetLinuxX11CsdFrameExtents(frameExtents);");
        windowSource.ShouldNotContain("EnsureMinSizeForDecorations");
        windowSource.IndexOf("PrepareLinuxInitialShowState();", StringComparison.Ordinal)
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
        extensionSource.ShouldNotContain("GtkFrameExtents");
        linuxSource.ShouldNotContain("GtkFrameExtents");
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
