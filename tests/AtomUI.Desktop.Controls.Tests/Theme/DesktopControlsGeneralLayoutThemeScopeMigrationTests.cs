using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlsGeneralLayoutThemeScopeMigrationTests
{
    [Fact]
    public void General_And_Layout_Controls_Remove_Legacy_Token_Scope_Registration()
    {
        AssertNoLegacyScope(
            "src/AtomUI.Desktop.Controls/Button/ButtonToken.cs",
            "src/AtomUI.Desktop.Controls/Button/IconButton.cs",
            "src/AtomUI.Desktop.Controls/Button/HyperLinkButton.cs",
            "src/AtomUI.Desktop.Controls/SplitButton/SplitButton.cs",
            "src/AtomUI.Desktop.Controls/Button/ToggleIconButton.cs");

        AssertNoLegacyScope(
            "src/AtomUI.Desktop.Controls/FloatButton/FloatButtonToken.cs",
            "src/AtomUI.Desktop.Controls/FloatButton/FloatButton.cs",
            "src/AtomUI.Desktop.Controls/FloatButton/FloatButtonHost.cs",
            "src/AtomUI.Desktop.Controls/FloatButton/FloatButtonGroup.cs",
            "src/AtomUI.Desktop.Controls/FloatButton/FloatButtonGroupHost.cs");

        AssertNoLegacyScope(
            "src/AtomUI.Desktop.Controls/Space/SpaceToken.cs",
            "src/AtomUI.Desktop.Controls/Space/Space.cs",
            "src/AtomUI.Desktop.Controls/Space/CompactSpace.cs",
            "src/AtomUI.Desktop.Controls/Space/CompactSpaceAddOn.cs");

        AssertNoLegacyScope(
            "src/AtomUI.Desktop.Controls/Splitter/SplitterToken.cs",
            "src/AtomUI.Desktop.Controls/Splitter/Splitter.cs",
            "src/AtomUI.Desktop.Controls/SplitView/SplitViewToken.cs",
            "src/AtomUI.Desktop.Controls/SplitView/SplitView.cs",
            "src/AtomUI.Desktop.Controls/Separator/SeparatorToken.cs",
            "src/AtomUI.Desktop.Controls/Separator/Separator.cs");
    }

    [Fact]
    public void General_And_Layout_Control_Themes_Use_Explicit_Token_Resources()
    {
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Buttons/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/FloatButton/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Space/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Splitter/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/SplitView/Themes");
        ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(
            "src/AtomUI.Desktop.Controls/Separator/Themes");
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
