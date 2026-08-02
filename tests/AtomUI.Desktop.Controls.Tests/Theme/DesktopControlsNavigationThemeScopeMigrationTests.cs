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
    public void Navigation_Control_Themes_Use_Explicit_Token_Resources()
    {
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Breadcrumb/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Menu/Themes");
        ThemeAssetScopeAssertions.AssertFileUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Flyouts/Themes/MenuFlyoutPresenterTheme.axaml");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/NavMenu/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Pagination/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Steps/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/TabControl/Themes");
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
