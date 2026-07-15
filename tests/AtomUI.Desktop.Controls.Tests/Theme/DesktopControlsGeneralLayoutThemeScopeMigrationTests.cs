using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlsGeneralLayoutThemeScopeMigrationTests
{
    [Fact]
    public void General_And_Layout_Controls_Remove_Legacy_Token_Scope_Registration()
    {
        AssertNoLegacyScope(
            "src/AtomUI.Desktop.Controls/Buttons/ButtonToken.cs",
            "src/AtomUI.Desktop.Controls/Buttons/IconButton.cs",
            "src/AtomUI.Desktop.Controls/Buttons/HyperLinkButton.cs",
            "src/AtomUI.Desktop.Controls/Buttons/SplitButton.cs",
            "src/AtomUI.Desktop.Controls/Buttons/ToggleIconButton.cs");

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
    public void General_And_Layout_Component_Themes_Use_Component_Shared_Token_Resources()
    {
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/Buttons/Themes",
            "ButtonTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/FloatButton/Themes",
            "FloatButtonTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/Space/Themes",
            "SpaceTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/Splitter/Themes",
            "SplitterTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/SplitView/Themes",
            "SplitViewTokenSharedTokenResource");
        AssertComponentThemeResources(
            "src/AtomUI.Desktop.Controls/Separator/Themes",
            "SeparatorTokenSharedTokenResource");
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
