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

    private static readonly (string Directory, string Extension)[] ThemeDirectories =
    [
        ("src/AtomUI.Desktop.Controls/Alert/Themes", "AlertTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Collapse/Themes", "CollapseTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Message/Themes", "MessageTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/MessageBox/Themes", "MessageBoxTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Notifications/Themes", "NotificationTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/PopupConfirm/Themes", "PopupConfirmTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/ProgressBar/Themes", "ProgressBarTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Result/Themes", "ResultTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Skeleton/Themes", "SkeletonTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Spin/Themes", "SpinTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Tooltip/Themes", "ToolTipTokenSharedTokenResource"),
        ("src/AtomUI.Desktop.Controls/Tour/Themes", "TourTokenSharedTokenResource")
    ];

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
    public void Feedback_Component_Themes_Use_Component_Shared_Token_Resources()
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
