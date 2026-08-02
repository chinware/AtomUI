using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlsDataDisplayThemeScopeMigrationTests
{
    private static readonly string[] ControlDirectories =
    [
        "src/AtomUI.Desktop.Controls/Avatar",
        "src/AtomUI.Desktop.Controls/Badge",
        "src/AtomUI.Desktop.Controls/Calendar",
        "src/AtomUI.Desktop.Controls/Card",
        "src/AtomUI.Desktop.Controls/Carousel",
        "src/AtomUI.Desktop.Controls/Descriptions",
        "src/AtomUI.Desktop.Controls/Empty",
        "src/AtomUI.Desktop.Controls/Expander",
        "src/AtomUI.Desktop.Controls/ImagePreviewer",
        "src/AtomUI.Desktop.Controls/ListBox",
        "src/AtomUI.Desktop.Controls/ListView",
        "src/AtomUI.Desktop.Controls/QRCode",
        "src/AtomUI.Desktop.Controls/Segmented",
        "src/AtomUI.Desktop.Controls/Statistic",
        "src/AtomUI.Desktop.Controls/Tag",
        "src/AtomUI.Desktop.Controls/Timeline",
        "src/AtomUI.Desktop.Controls/TreeView"
    ];

    private static readonly string[] ThemeDirectories =
        ControlDirectories.Select(static directory => $"{directory}/Themes").ToArray();

    [Fact]
    public void DataDisplay_Controls_Remove_Legacy_Token_Scope_Registration()
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
    public void DataDisplay_Control_Themes_Use_Explicit_Token_Resources()
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
