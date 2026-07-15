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

    private static readonly (string Directory, string Extension)[] ThemeDirectories =
    [
        ("src/AtomUI.Desktop.Controls/BorderBeam/Themes", "BorderBeamTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/GroupBox/Themes", "GroupBoxTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/ScrollViewer/Themes", "ScrollViewerTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Primitives/ArrowDecoratedBox/Themes",
            "ArrowDecoratedBoxTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Primitives/IndicatorScrollViewer/Themes",
            "IndicatorScrollViewerTokenSharedTokenResource")
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
    public void Misc_Component_Themes_Use_Component_Shared_Token_Resources()
    {
        foreach (var (relativeDirectory, componentResourceExtension) in ThemeDirectories)
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
