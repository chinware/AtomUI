using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class SmallPackageThemeScopeMigrationTests
{
    [Fact]
    public void Small_Packages_Remove_Legacy_Token_Scope_Registration()
    {
        ReadRepoFile("src/AtomUI.Desktop.Controls.ColorPicker/AbstractColorPicker.cs")
            .ShouldNotContain("RegisterTokenResourceScope");
        ReadRepoFile("src/AtomUI.Desktop.Controls.ColorPicker/ColorView/AbstractColorPickerView.cs")
            .ShouldNotContain("RegisterTokenResourceScope");
        ReadRepoFile("src/AtomUI.Desktop.Controls.ColorPicker/ColorPickerToken.cs")
            .ShouldNotContain("ScopeProvider");

        ReadRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/Splash.cs")
            .ShouldNotContain("RegisterTokenResourceScope");
        ReadRepoFile("src/AtomUI.Desktop.Controls.Extras/Splash/SplashToken.cs")
            .ShouldNotContain("ScopeProvider");
    }

    [Fact]
    public void Small_Package_Control_Themes_Use_Explicit_Token_Resources()
    {
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Controls/Icon/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls.ColorPicker/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls.Extras/Splash/Themes");

        ReadRepoFile("src/AtomUI.Controls/Embedding/Themes/EmbeddableControlRootTheme.axaml")
            .ShouldContain("{atom:SharedTokenResource ColorBgContainer}");
    }

    private static string ReadRepoFile(string relativePath)
    {
        return File.ReadAllText(GetRepoFile(relativePath));
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = AppContext.BaseDirectory;
        while (directory is not null)
        {
            var candidate = Path.Combine(directory, relativePath);
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new FileNotFoundException(relativePath);
    }
}
