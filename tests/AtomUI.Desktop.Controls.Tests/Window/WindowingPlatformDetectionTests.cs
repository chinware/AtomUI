using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Xml.Linq;
using AtomUI;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowingPlatformDetectionTests
{
    [Theory]
    [InlineData(AtomUIWindowingPlatform.Wayland, null, null, AtomUIWindowingPlatform.Wayland)]
    [InlineData(AtomUIWindowingPlatform.X11, "wayland", "wayland-0", AtomUIWindowingPlatform.X11)]
    [InlineData(AtomUIWindowingPlatform.Auto, "wayland", null, AtomUIWindowingPlatform.Wayland)]
    [InlineData(AtomUIWindowingPlatform.Auto, " X11 ", "wayland-0", AtomUIWindowingPlatform.X11)]
    [InlineData(AtomUIWindowingPlatform.Auto, null, "wayland-0", AtomUIWindowingPlatform.Wayland)]
    [InlineData(AtomUIWindowingPlatform.Auto, null, null, AtomUIWindowingPlatform.X11)]
    [InlineData(AtomUIWindowingPlatform.Auto, "", "", AtomUIWindowingPlatform.X11)]
    public void Linux_Windowing_Platform_Resolution_Uses_Approved_Precedence(
        AtomUIWindowingPlatform requested,
        string? environmentOverride,
        string? waylandDisplay,
        AtomUIWindowingPlatform expected)
    {
        DesktopAppBuilderExtensions.ResolveLinuxWindowingPlatform(
                requested,
                environmentOverride,
                waylandDisplay)
            .ShouldBe(expected);
    }

    [Fact]
    public void Invalid_Environment_Override_Fails_Clearly()
    {
        var exception = Should.Throw<InvalidOperationException>(() =>
            DesktopAppBuilderExtensions.ResolveLinuxWindowingPlatform(
                AtomUIWindowingPlatform.Auto,
                "mir",
                "wayland-0"));

        exception.Message.ShouldContain(DesktopAppBuilderExtensions.WindowingPlatformEnvironmentVariable);
        exception.Message.ShouldContain("mir");
    }

    [Fact]
    public void Desktop_Platform_Detection_Does_Not_Expand_AtomUI_Core_Backend_Dependencies()
    {
        var coreProject = File.ReadAllText(GetRepoFile("src/AtomUI.Core/AtomUI.Core.csproj"));
        var desktopProject = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj"));

        coreProject.ShouldNotContain("Avalonia.Wayland");
        coreProject.ShouldNotContain("Avalonia.Desktop");
        desktopProject.ShouldContain("Avalonia.Desktop");
        desktopProject.ShouldContain("Avalonia.Wayland");
    }

    [Fact]
    public void Wayland_Chrome_Does_Not_Call_X11_Handle_Or_Absolute_Geometry_Hacks()
    {
        var commonSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Chrome/AbstractLinuxWindowChromeManager.cs"));
        var waylandSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Chrome/WaylandWindowChromeManager.cs"));
        var x11Source = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Chrome/X11WindowChromeManager.cs"));

        commonSource.ShouldContain("new X11WindowChromeManager(window)");
        commonSource.ShouldContain("new WaylandWindowChromeManager(window)");
        commonSource.ShouldContain("new GenericLinuxWindowChromeManager(window)");
        commonSource.ShouldContain("HandleDescriptor, \"XID\"");
        waylandSource.ShouldNotContain("ConfigureLinuxInitialWindowGeometry");
        waylandSource.ShouldNotContain("SetLinuxX11CsdFrameExtents");
        waylandSource.ShouldNotContain("AttachClickThroughShadow");
        waylandSource.ShouldNotContain("Position");
        waylandSource.ShouldNotContain("PlatformImpl?.Handle");
        x11Source.ShouldContain("ConfigureLinuxInitialWindowGeometry");
        x11Source.ShouldContain("SetLinuxX11CsdFrameExtents");
        x11Source.ShouldContain("AttachClickThroughShadow");
    }

    [Fact]
    public void Window_Consumes_Custom_Resizer_As_A_Chrome_Manager_Capability()
    {
        var windowSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Window.cs"));
        var contractSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Chrome/WindowChromeManager.cs"));
        var linuxSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Chrome/AbstractLinuxWindowChromeManager.cs"));
        var waylandSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Chrome/WaylandWindowChromeManager.cs"));

        contractSource.ShouldContain("bool UsesCustomResizer { get; }");
        linuxSource.ShouldContain("public virtual bool UsesCustomResizer => !Window.IsCsdEnabled;");
        waylandSource.ShouldContain("public override bool UsesCustomResizer => true;");
        windowSource.ShouldContain("_platformChromeManager?.UsesCustomResizer == true");
        windowSource.ShouldNotContain("_platformChromeManager is not WaylandWindowChromeManager");
    }

    [Theory]
    [SupportedOSPlatform("linux")]
    [InlineData(AtomUIWindowingPlatform.X11, null, null, 1)]
    [InlineData(AtomUIWindowingPlatform.Wayland, "XID", "Avalonia.X11", 2)]
    [InlineData(null, "XID", "Avalonia.X11", 1)]
    [InlineData(null, null, "Avalonia.Wayland", 2)]
    [InlineData(null, null, "Avalonia.Headless", 0)]
    [InlineData(null, "FB", "Avalonia.LinuxFramebuffer", 0)]
    public void Linux_Chrome_Backend_Requires_Positive_Platform_Evidence(
        AtomUIWindowingPlatform? configuredPlatform,
        string? handleDescriptor,
        string? platformAssemblyName,
        int expected)
    {
        AbstractLinuxWindowChromeManager.ResolveBackend(
                configuredPlatform,
                handleDescriptor,
                platformAssemblyName)
            .ShouldBe((LinuxWindowingBackend)expected);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    public void Caption_Pin_Support_Follows_Linux_Backend_Capability(
        int backend,
        bool expected)
    {
        CaptionButtonGroup.IsPinSupportedForBackend((LinuxWindowingBackend)backend)
                          .ShouldBe(expected);
    }

    [Theory]
    [SupportedOSPlatform("windows")]
    [InlineData(true, true, WindowState.Normal, true)]
    [InlineData(false, true, WindowState.Normal, false)]
    [InlineData(true, false, WindowState.Normal, false)]
    [InlineData(true, true, WindowState.Maximized, false)]
    [InlineData(true, true, WindowState.FullScreen, false)]
    public void Windows_Visible_Frame_Border_Is_Only_Used_For_Extended_Normal_Csd_Windows(
        bool isCsdEnabled,
        bool isExtendedIntoWindowDecorations,
        WindowState windowState,
        bool expected)
    {
        WindowsWindowChromeManager.ShouldUseVisibleFrameBorder(
                isCsdEnabled,
                isExtendedIntoWindowDecorations,
                windowState)
            .ShouldBe(expected);
    }

    [Fact]
    public void Wayland_Disables_Caption_Pin_Through_Effective_State()
    {
        var captionSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/CaptionButtonGroup.cs"));
        var captionDocument = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/CaptionButtonGroupTheme.axaml"));

        var pinButtons = captionDocument.Descendants()
                                        .Where(element =>
                                            element.Name.LocalName is "CaptionButton" or "WindowsCaptionButton" &&
                                            (string?)element.Attribute("Name") == "PART_PinButton")
                                        .ToList();

        captionSource.ShouldContain("IsPinButtonEffectivelyVisibleProperty");
        captionSource.ShouldContain("IsPinSupportedForBackend");
        captionSource.ShouldContain("LinuxWindowingBackend.Wayland");
        captionSource.ShouldContain("UpdatePinButtonVisibility();");
        captionSource.ShouldContain("hostWindow.Opened += HandleHostWindowOpened");
        captionSource.ShouldContain("hostWindow.Opened -= HandleHostWindowOpened");
        captionSource.ShouldContain("!IsPinButtonEffectivelyVisible");

        pinButtons.Count.ShouldBe(3);
        pinButtons.ShouldAllBe(button =>
            (string?)button.Attribute("IsVisible") == "{TemplateBinding IsPinButtonEffectivelyVisible}");
        pinButtons.ShouldAllBe(button =>
            (string?)button.Attribute("IsEnabled") == "{TemplateBinding IsPinButtonEffectivelyVisible}");
    }

    [Fact]
    public void Linux_Csd_Tracks_Platform_Decoration_Requests_And_Uses_AtomUI_Theme()
    {
        var commonSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Chrome/AbstractLinuxWindowChromeManager.cs"));
        var windowSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Window.cs"));
        var document = XDocument.Load(GetRepoFile(
            "src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

        commonSource.ShouldContain("platformImpl.DrawnDecorationsRequestChanged +=");
        commonSource.ShouldContain("_window.RefreshPlatformCsdStatus();");
        windowSource.ShouldContain("PlatformImpl?.NeedsManagedDecorations == true");
        windowSource.ShouldContain("change.Property == IsExtendedIntoWindowDecorationsProperty");

        document.Descendants(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "WindowDecorationsTheme" &&
            (string?)setter.Attribute("Value") ==
            "{DynamicResource {x:Type WindowDrawnDecorations}}");
    }

    [Fact]
    public void Platform_Backend_Helpers_Configure_Renderer_And_Text_Shaping_Without_Reflection()
    {
        var source = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/DesktopAppBuilderExtensions.cs"));

        source.ShouldContain("MethodImplOptions.NoInlining");
        source.ShouldContain("builder.UseWayland()");
        source.ShouldContain("builder.UseX11()");
        source.ShouldContain(".UseSkia()");
        source.ShouldContain(".UseHarfBuzz()");
        source.ShouldNotContain("System.Reflection");
        source.ShouldNotContain("Assembly.Load");
        source.ShouldNotContain("Activator.CreateInstance");
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
