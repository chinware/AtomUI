using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlsFeedbackThemeScopeMigrationTests
{
    private static readonly string[] ControlDirectories =
    [
        "src/AtomUI.Desktop.Controls/Alert",
        "src/AtomUI.Desktop.Controls/Collapse",
        "src/AtomUI.Desktop.Controls/Message",
        "src/AtomUI.Desktop.Controls/MessageBox",
        "src/AtomUI.Desktop.Controls/Notifications",
        "src/AtomUI.Desktop.Controls/PopupConfirm",
        "src/AtomUI.Desktop.Controls/ProgressBar",
        "src/AtomUI.Desktop.Controls/Result",
        "src/AtomUI.Desktop.Controls/Skeleton",
        "src/AtomUI.Desktop.Controls/Spin",
        "src/AtomUI.Desktop.Controls/Tooltip",
        "src/AtomUI.Desktop.Controls/Tour"
    ];

    private static readonly string[] ThemeDirectories =
        ControlDirectories.Select(static directory => $"{directory}/Themes").ToArray();

    [Fact]
    public void Feedback_Controls_Remove_Legacy_Token_Scope_Registration()
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
    public void Feedback_Control_Themes_Use_Ambient_Shared_Token_Scopes()
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
