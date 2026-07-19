using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlsMiscThemeScopeMigrationTests
{
    private static readonly string[] ControlDirectories =
    [
        "src/AtomUI.Desktop.Controls/BorderBeam",
        "src/AtomUI.Desktop.Controls/GroupBox",
        "src/AtomUI.Desktop.Controls/MarqueeLabel",
        "src/AtomUI.Desktop.Controls/ScrollViewer",
        "src/AtomUI.Desktop.Controls/Primitives/ArrowDecoratedBox",
        "src/AtomUI.Desktop.Controls/Primitives/IndicatorScrollViewer"
    ];

    private static readonly string[] ThemeDirectories =
    [
        "src/AtomUI.Desktop.Controls/BorderBeam/Themes",
        "src/AtomUI.Desktop.Controls/GroupBox/Themes",
        "src/AtomUI.Desktop.Controls/ScrollViewer/Themes",
        "src/AtomUI.Desktop.Controls/Primitives/ArrowDecoratedBox/Themes",
        "src/AtomUI.Desktop.Controls/Primitives/IndicatorScrollViewer/Themes"
    ];

    [Fact]
    public void Misc_Controls_Remove_Legacy_Token_Scope_Registration()
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
    public void Misc_Control_Themes_Use_Ambient_Shared_Token_Scopes()
    {
        foreach (var relativeDirectory in ThemeDirectories)
        {
            ThemeAssetScopeAssertions.AssertDirectoryUsesSharedTokenScope(relativeDirectory);
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
