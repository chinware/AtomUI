using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowTitleBarLogoVisibilityTests
{
    static WindowTitleBarLogoVisibilityTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Title_Bar_Logo_Visibility_Mode_Defaults_To_Auto()
    {
        var enumSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBarLogoVisibility.cs"));
        var titleBarSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBar.cs"));

        enumSource.ShouldContain("Auto");
        enumSource.ShouldContain("Always");
        enumSource.ShouldContain("Never");
        titleBarSource.ShouldContain("WindowTitleBarLogoVisibility.Auto");
    }

    [Fact]
    public void Title_Bar_Logo_Auto_Mode_Uses_Title_Content_Platform_And_Fullscreen_State()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBar.cs"));

        source.ShouldContain("UpdateEffectiveLogoVisible()");
        source.ShouldContain("WindowTitleBarLogoVisibility.Always => hasLogo");
        source.ShouldContain("WindowTitleBarLogoVisibility.Never => false");
        source.ShouldContain("_ => hasLogo && ShouldShowLogoInAutoMode()");
        source.ShouldContain("_isWindowFullScreen = x == WindowState.FullScreen");
        source.ShouldContain("OsType == OsType.macOS");
        source.ShouldContain("return !_isWindowFullScreen;");
        source.ShouldContain("string text => !string.IsNullOrWhiteSpace(text)");
    }

    [Fact]
    public void Title_Bar_Logo_Auto_Mode_Hides_Titleless_Logo_On_MacOS()
    {
        var titleBar = new AtomUI.Desktop.Controls.WindowTitleBar();

        titleBar.SetValue(AtomUI.Desktop.Controls.WindowTitleBar.OsTypeProperty, OsType.macOS);
        titleBar.Logo = new object();

        GetIsEffectiveLogoVisible(titleBar).ShouldBeFalse();
    }

    [Fact]
    public void Title_Bar_Logo_Auto_Mode_Keeps_Titleless_Logo_On_Non_MacOS_When_Not_Fullscreen()
    {
        var titleBar = new AtomUI.Desktop.Controls.WindowTitleBar();

        titleBar.SetValue(AtomUI.Desktop.Controls.WindowTitleBar.OsTypeProperty, OsType.Windows);
        titleBar.Logo = new object();

        GetIsEffectiveLogoVisible(titleBar).ShouldBeTrue();
    }

    [Fact]
    public void Title_Bar_Logo_Always_Mode_Keeps_Titleless_Logo_On_MacOS()
    {
        var titleBar = new AtomUI.Desktop.Controls.WindowTitleBar();

        titleBar.SetValue(AtomUI.Desktop.Controls.WindowTitleBar.OsTypeProperty, OsType.macOS);
        titleBar.LogoVisibility = AtomUI.Desktop.Controls.WindowTitleBarLogoVisibility.Always;
        titleBar.Logo           = new object();

        GetIsEffectiveLogoVisible(titleBar).ShouldBeTrue();
    }

    [Fact]
    public void Title_Bar_Logo_Auto_Mode_Recomputes_When_OsType_Changes()
    {
        var titleBar = new AtomUI.Desktop.Controls.WindowTitleBar();

        titleBar.SetValue(AtomUI.Desktop.Controls.WindowTitleBar.OsTypeProperty, OsType.Windows);
        titleBar.Logo = new object();

        GetIsEffectiveLogoVisible(titleBar).ShouldBeTrue();

        titleBar.SetValue(AtomUI.Desktop.Controls.WindowTitleBar.OsTypeProperty, OsType.macOS);

        GetIsEffectiveLogoVisible(titleBar).ShouldBeFalse();
    }

    [Fact]
    public void Window_Propagates_Logo_Visibility_To_Title_Bar_Without_Clearing_Window_Icon()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs"));

        source.ShouldContain("WindowTitleBar.LogoVisibilityProperty.AddOwner<Window>()");
        source.ShouldContain("public WindowTitleBarLogoVisibility LogoVisibility");
        source.ShouldContain("titleBar[!WindowTitleBar.LogoVisibilityProperty] = this[!LogoVisibilityProperty]");
        source.ShouldContain("IsEffectiveFullscreenLogoVisibleProperty");
        source.ShouldContain("UpdateEffectiveFullscreenLogoVisible()");
        source.ShouldContain("_ => hasLogo && HasTitleContent(Title)");
        source.ShouldContain("TryApplyWindowIconLogo(Icon)");
        source.ShouldNotContain("Icon = null");
        source.ShouldNotContain("SetCurrentValue(IconProperty");
    }

    [Fact]
    public void Title_Bar_Templates_Bind_Logo_Visibility_To_Effective_State()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/Themes/WindowTitleBarTheme.axaml"));

        Regex.Matches(source, "IsVisible=\"\\{TemplateBinding IsEffectiveLogoVisible\\}\"")
             .Count
             .ShouldBe(3);
    }

    [Fact]
    public void Fullscreen_Title_Bar_Templates_Hide_Logo_With_Effective_Fullscreen_State()
    {
        var fullscreenPopoverSource =
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Themes/FullscreenPopoverLayerTheme.axaml"));
        var drawnDecorationsSource =
            File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Themes/WindowDrawnDecorationsTheme.axaml"));

        fullscreenPopoverSource.ShouldContain(
            "IsVisible=\"{Binding $parent[atom:Window].IsEffectiveFullscreenLogoVisible}\"");
        drawnDecorationsSource.ShouldContain(
            "IsVisible=\"{Binding $parent[atom:Window].IsEffectiveFullscreenLogoVisible}\"");
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

    private static bool GetIsEffectiveLogoVisible(AtomUI.Desktop.Controls.WindowTitleBar titleBar)
    {
        var property = typeof(AtomUI.Desktop.Controls.WindowTitleBar)
                       .GetProperty("IsEffectiveLogoVisible", BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        return (bool)property.GetValue(titleBar)!;
    }
}
