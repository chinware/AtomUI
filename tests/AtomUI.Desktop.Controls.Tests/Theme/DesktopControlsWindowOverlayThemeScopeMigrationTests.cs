using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlsWindowOverlayThemeScopeMigrationTests
{
    private static readonly string[] ControlDirectories =
    [
        "src/AtomUI.Desktop.Controls/AdornerLayer",
        "src/AtomUI.Desktop.Controls/Dialog",
        "src/AtomUI.Desktop.Controls/Drawer",
        "src/AtomUI.Desktop.Controls/Flyouts",
        "src/AtomUI.Desktop.Controls/Popup",
        "src/AtomUI.Desktop.Controls/Window",
        "src/AtomUI.Desktop.Controls/WindowTitleBar"
    ];

    private static readonly string[] ThemeDirectories =
        ControlDirectories.Select(static directory => $"{directory}/Themes").ToArray();

    [Fact]
    public void WindowOverlay_Controls_Remove_Legacy_Token_Scope_Registration()
    {
        foreach (var relativeDirectory in ControlDirectories)
        {
            var codeFiles = Directory.GetFiles(
                GetRepoFile(relativeDirectory),
                "*.cs",
                SearchOption.AllDirectories);

            codeFiles.ShouldNotBeEmpty();

            foreach (var codeFile in codeFiles)
            {
                var text = File.ReadAllText(codeFile);
                text.ShouldNotContain("RegisterTokenResourceScope");
                text.ShouldNotContain("ScopeProvider");
            }
        }
    }

    [Fact]
    public void WindowOverlay_Control_Themes_Use_Explicit_Token_Resources()
    {
        foreach (var relativeDirectory in ThemeDirectories)
        {
            ThemeAssetScopeAssertions.AssertDirectoryUsesExplicitTokenResources(relativeDirectory);
        }
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
