using AtomUI.Theme.Schema;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlRegistrationSelectorTests
{
    [Theory]
    [InlineData("AdornerLayer", false)]
    [InlineData("OtpLineEdit", false)]
    [InlineData("OtpLineEditCell", false)]
    [InlineData("SplitView", false)]
    [InlineData("TreeViewFlyoutPresenter", false)]
    [InlineData("Window", false)]
    [InlineData("WindowTitleBar", false)]
    [InlineData("Button", true)]
    [InlineData("DropdownButton", true)]
    [InlineData("IconButton", true)]
    [InlineData("TextBox", true)]
    [InlineData("IndicatorScrollViewer", true)]
    public void Browser_Control_Support_Uses_Exact_Registered_Identity(string controlId, bool expected)
    {
        DesktopControlRegistrationSelector.IsBrowserControlSupported(
            new ControlTokenIdentity("AtomUI", controlId)).ShouldBe(expected);
    }

    [Fact]
    public void Desktop_Controls_Do_Not_Define_Browser_Specific_Theme_Assets()
    {
        var repositoryRoot = GetRepositoryRoot();
        var controlsRoot = Path.Combine(repositoryRoot, "src/AtomUI.Desktop.Controls");
        var browserThemeAssets = Directory.EnumerateFiles(
                                             controlsRoot,
                                             "*.axaml",
                                             SearchOption.AllDirectories)
                                         .Where(path => path.Contains(
                                             $"{Path.DirectorySeparatorChar}Themes{Path.DirectorySeparatorChar}Browser{Path.DirectorySeparatorChar}",
                                             StringComparison.Ordinal))
                                         .Select(path => Path.GetRelativePath(repositoryRoot, path))
                                         .ToArray();

        browserThemeAssets.ShouldBeEmpty();
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root.");
    }
}
