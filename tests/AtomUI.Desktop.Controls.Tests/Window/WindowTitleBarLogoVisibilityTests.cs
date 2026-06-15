using System;
using System.IO;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowTitleBarLogoVisibilityTests
{
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
    public void Title_Bar_Logo_Auto_Mode_Hides_Titleless_Logo_Only_In_FullScreen()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBar.cs"));

        source.ShouldContain("UpdateEffectiveLogoVisible()");
        source.ShouldContain("WindowTitleBarLogoVisibility.Always => hasLogo");
        source.ShouldContain("WindowTitleBarLogoVisibility.Never => false");
        source.ShouldContain("_ => hasLogo && ShouldShowLogoInAutoMode()");
        source.ShouldContain("_isWindowFullScreen = x == WindowState.FullScreen");
        source.ShouldContain("return !_isWindowFullScreen;");
        source.ShouldContain("string text => !string.IsNullOrWhiteSpace(text)");
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
}
