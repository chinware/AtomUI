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
    public void Small_Package_Component_Themes_Use_Component_Shared_Token_Resources()
    {
        AssertComponentThemeResources(
            "src/AtomUI.Controls/Icon/Themes",
            "IconTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls.ColorPicker/Themes",
            "ColorPickerTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls.Extras/Splash/Themes",
            "SplashTokenSharedTokenResource");

        ReadRepoFile("src/AtomUI.Controls/Embedding/Themes/EmbeddableControlRootTheme.axaml")
            .ShouldContain("{atom:SharedTokenResource ColorBgContainer}");
    }

    private static void AssertComponentThemeResources(
        string relativeDirectory,
        string componentResourceExtension)
    {
        var themeFiles = Directory.GetFiles(
            GetRepoFile(relativeDirectory),
            "*.axaml",
            SearchOption.AllDirectories);

        themeFiles.ShouldNotBeEmpty();
        themeFiles.ShouldContain(file =>
            File.ReadAllText(file).Contains(componentResourceExtension, StringComparison.Ordinal));

        foreach (var themeFile in themeFiles)
        {
            File.ReadAllText(themeFile)
                .ShouldNotContain("{atom:SharedTokenResource ");
        }
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
