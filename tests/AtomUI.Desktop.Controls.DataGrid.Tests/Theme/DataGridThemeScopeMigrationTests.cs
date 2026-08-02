using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Theme;

public class DataGridThemeScopeMigrationTests
{
    [Fact]
    public void DataGrid_Removes_Legacy_Token_Scope_Registration()
    {
        ReadRepoFile("src/AtomUI.Desktop.Controls.DataGrid/DataGrid.cs")
            .ShouldNotContain("RegisterTokenResourceScope");
        ReadRepoFile("src/AtomUI.Desktop.Controls.DataGrid/DataGridToken.cs")
            .ShouldNotContain("ScopeProvider");
    }

    [Fact]
    public void DataGrid_Control_Themes_Use_Explicit_Token_Resources()
    {
        var themeFiles = Directory.GetFiles(
            GetRepoFile("src/AtomUI.Desktop.Controls.DataGrid/Themes"),
            "*.axaml",
            SearchOption.AllDirectories);

        themeFiles.ShouldNotBeEmpty();
        themeFiles.ShouldContain(file =>
            File.ReadAllText(file).Contains("{atom:SharedTokenResource ", StringComparison.Ordinal));

        foreach (var themeFile in themeFiles)
        {
            var text = File.ReadAllText(themeFile);
            text.ShouldNotContain("TokenSharedTokenResource");
            text.ShouldNotContain("ControlTokenScope.Identity");
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
