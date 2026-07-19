using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class GalleryBaseThemeScopeMigrationTests
{
    private static readonly string[] ControlThemeFiles =
    [
        "src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseHeaderTheme.axaml",
        "src/AtomUI.Toolkits.GalleryBase/Controls/ShowCaseItemTheme.axaml"
    ];

    [Fact]
    public void GalleryBase_Controls_Remove_Legacy_Token_Scope_Registration()
    {
        var controlFiles = Directory.GetFiles(
            GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls"),
            "*.cs",
            SearchOption.TopDirectoryOnly);

        controlFiles.ShouldNotBeEmpty();

        foreach (var controlFile in controlFiles)
        {
            var text = File.ReadAllText(controlFile);
            text.ShouldNotContain("RegisterTokenResourceScope");
            text.ShouldNotContain("ScopeProvider");
        }
    }

    [Fact]
    public void GalleryBase_Control_Themes_Use_Ambient_Shared_Token_Scopes()
    {
        foreach (var relativePath in ControlThemeFiles)
        {
            var text = File.ReadAllText(GetRepoFile(relativePath));

            text.ShouldContain("{atom:SharedTokenResource ");
            text.ShouldContain("themeResources:ControlTokenScope.Identity=");
            text.ShouldNotContain("TokenSharedTokenResource");
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
