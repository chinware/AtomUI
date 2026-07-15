using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class GalleryBaseThemeScopeMigrationTests
{
    private static readonly (string Path, string Extension)[] ComponentThemeFiles =
    [
        ("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseHeaderTheme.axaml",
            "GalleryShowCaseHeaderTokenSharedTokenResource"),
        ("src/AtomUI.Toolkits.GalleryBase/Controls/ShowCaseItemTheme.axaml",
            "ShowCaseItemTokenSharedTokenResource")
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
    public void GalleryBase_Component_Themes_Use_Component_Shared_Token_Resources()
    {
        foreach (var (relativePath, componentResourceExtension) in ComponentThemeFiles)
        {
            var text = File.ReadAllText(GetRepoFile(relativePath));

            text.ShouldContain(componentResourceExtension);
            text.ShouldNotContain("{atom:SharedTokenResource ");
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
