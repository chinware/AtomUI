using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class BrowserButtonThemeAssetSplitTests
{
    private static readonly (string Path, string ThemeName, string Owner)[] BrowserThemeAssets =
    [
        ("src/AtomUI.Desktop.Controls/Buttons/Themes/Browser/ButtonTheme.axaml", "ControlTheme", "atom:Button"),
        ("src/AtomUI.Desktop.Controls/Buttons/Themes/Browser/DropdownButtonTheme.axaml", "ControlTheme", "atom:DropdownButton"),
        ("src/AtomUI.Desktop.Controls/Buttons/Themes/Browser/IconButtonTheme.axaml", "IconButtonTheme", "atom:IconButton")
    ];

    [Fact]
    public void Browser_Button_Themes_Are_Independent_Owner_Assets()
    {
        var repositoryRoot = GetRepositoryRoot();

        File.Exists(Path.Combine(
            repositoryRoot,
            "src/AtomUI.Desktop.Controls/Buttons/Themes/BrowserButtonThemes.axaml")).ShouldBeFalse();

        foreach (var (relativePath, themeName, owner) in BrowserThemeAssets)
        {
            var document = XDocument.Load(Path.Combine(repositoryRoot, relativePath));
            document.Root.ShouldNotBeNull();
            document.Root.Name.LocalName.ShouldBe("ResourceDictionary");
            document.Descendants().Any(element => element.Name.LocalName == "ResourceInclude").ShouldBeFalse();
            var theme = document.Root.Elements().ShouldHaveSingleItem();
            theme.Name.LocalName.ShouldBe(themeName);
            theme.Attributes().Single(attribute => attribute.Name.LocalName == "TargetType").Value
                    .ShouldContain(owner);
            theme.Attributes().Any(attribute => attribute.Name.LocalName == "Class").ShouldBeFalse();
        }
    }

    [Fact]
    public void Generated_Manifest_Includes_Browser_Button_Theme_Leaves_Directly()
    {
        var manifest = File.ReadAllText(Path.Combine(
            GetRepositoryRoot(),
            "src/AtomUI.Desktop.Controls/GeneratedFiles/AtomUI.Generator/AtomUI.Generator.ThemeAssetManifestGenerator/GeneratedControlThemeAssetManifest.g.cs"));

        manifest.ShouldContain("Buttons/Themes/Browser/ButtonTheme.axaml");
        manifest.ShouldContain("Buttons/Themes/Browser/DropdownButtonTheme.axaml");
        manifest.ShouldContain("Buttons/Themes/Browser/IconButtonTheme.axaml");
        manifest.ShouldContain("Buttons/Themes/SplitButtonTheme.axaml");
        manifest.ShouldContain("Buttons/Themes/HyperLinkButtonTheme.axaml");
        manifest.ShouldNotContain("Buttons/Themes/BrowserButtonThemes.axaml");
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
