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

    private static readonly (string Directory, string[] Extensions)[] ThemeDirectories =
    [
        ("src/AtomUI.Desktop.Controls/AdornerLayer/Themes", ["AdornerLayerTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Dialog/Themes", ["DialogTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Drawer/Themes", ["DrawerTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Flyouts/Themes", [
            "FlyoutHostTokenSharedTokenResource",
            "TreeFlyoutTokenSharedTokenResource"
        ]),
        ("src/AtomUI.Desktop.Controls/Popup/Themes", ["PopupHostTokenSharedTokenResource"]),
        ("src/AtomUI.Desktop.Controls/Window/Themes", [
            "WindowTokenSharedTokenResource",
            "WindowTitleBarTokenSharedTokenResource"
        ]),
        ("src/AtomUI.Desktop.Controls/WindowTitleBar/Themes", ["WindowTitleBarTokenSharedTokenResource"])
    ];

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
    public void WindowOverlay_Component_Themes_Use_Component_Shared_Token_Resources()
    {
        foreach (var (relativeDirectory, componentResourceExtensions) in ThemeDirectories)
        {
            var themeFiles = Directory.GetFiles(
                GetRepoFile(relativeDirectory),
                "*.axaml",
                SearchOption.AllDirectories);

            themeFiles.ShouldNotBeEmpty();

            foreach (var componentResourceExtension in componentResourceExtensions)
            {
                themeFiles.ShouldContain(file =>
                    File.ReadAllText(file).Contains(componentResourceExtension, StringComparison.Ordinal));
            }

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
