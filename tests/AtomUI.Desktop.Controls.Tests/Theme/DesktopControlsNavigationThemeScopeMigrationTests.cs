using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlsNavigationThemeScopeMigrationTests
{
    [Fact]
    public void Navigation_Controls_Remove_Legacy_Token_Scope_Registration()
    {
        AssertNoLegacyScope(
            "src/AtomUI.Desktop.Controls/Breadcrumb/BreadcrumbToken.cs",
            "src/AtomUI.Desktop.Controls/Breadcrumb/Breadcrumb.cs",
            "src/AtomUI.Desktop.Controls/Menu/MenuToken.cs",
            "src/AtomUI.Desktop.Controls/Menu/Menu.cs",
            "src/AtomUI.Desktop.Controls/Menu/ContextMenu.cs",
            "src/AtomUI.Desktop.Controls/Menu/MenuSeparator.cs",
            "src/AtomUI.Desktop.Controls/Flyouts/MenuFlyoutPresenter.cs",
            "src/AtomUI.Desktop.Controls/NavMenu/NavMenuToken.cs",
            "src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs",
            "src/AtomUI.Desktop.Controls/Pagination/PaginationToken.cs",
            "src/AtomUI.Desktop.Controls/Pagination/Pagination.cs",
            "src/AtomUI.Desktop.Controls/Pagination/SimplePagination.cs",
            "src/AtomUI.Desktop.Controls/Steps/StepsToken.cs",
            "src/AtomUI.Desktop.Controls/Steps/Steps.cs",
            "src/AtomUI.Desktop.Controls/TabControl/TabControlToken.cs",
            "src/AtomUI.Desktop.Controls/TabControl/BaseTabControl.cs",
            "src/AtomUI.Desktop.Controls/TabControl/TabStrip/BaseTabStrip.cs");
    }

    [Fact]
    public void Navigation_Component_Themes_Use_Component_Shared_Token_Resources()
    {
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/Breadcrumb/Themes",
            "BreadcrumbTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/Menu/Themes",
            "MenuTokenSharedTokenResource");
        AssertComponentThemeFile(
            "src/AtomUI.Desktop.Controls/Flyouts/Themes/MenuFlyoutPresenterTheme.axaml",
            "MenuTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/NavMenu/Themes",
            "NavMenuTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/Pagination/Themes",
            "PaginationTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/Steps/Themes",
            "StepsTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/TabControl/Themes",
            "TabControlTokenSharedTokenResource");
    }

    private static void AssertNoLegacyScope(params string[] relativePaths)
    {
        foreach (var relativePath in relativePaths)
        {
            var text = ReadRepoFile(relativePath);
            text.ShouldNotContain("RegisterTokenResourceScope");
            text.ShouldNotContain("ScopeProvider");
        }
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

    private static void AssertComponentThemeFile(
        string relativePath,
        string componentResourceExtension)
    {
        var text = ReadRepoFile(relativePath);

        text.ShouldContain(componentResourceExtension);
        text.ShouldNotContain("{atom:SharedTokenResource ");
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
