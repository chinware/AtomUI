using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Toolkits;

public class GalleryBaseExtractionTests
{
    [Fact]
    public void GalleryBase_Project_Is_Solution_Module_And_Owns_Gallery_Controls()
    {
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj")).ShouldBeTrue();
        ReadRepoFile("AtomUI.slnx").ShouldContain("src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj");

        var project = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj");
        project.ShouldContain("AtomUI.Desktop.Controls.csproj");
        project.ShouldContain("AtomUI.Generator.csproj");
        project.ShouldNotContain("controlgallery/AtomUIGallery");

        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/ShowCaseItem.axaml.cs")).ShouldBeTrue();
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/ShowCasePanel.axaml.cs")).ShouldBeTrue();
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryStickyTabsHost.cs")).ShouldBeTrue();
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseScenarioController.cs")).ShouldBeTrue();
        File.Exists(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseRuntimeOptions.cs")).ShouldBeTrue();
    }

    [Fact]
    public void AtomUIGallery_Consumes_GalleryBase_Instead_Of_Owning_ShowCase_Infrastructure()
    {
        var project = ReadRepoFile("controlgallery/AtomUIGallery/AtomUIGallery.csproj");
        project.ShouldContain("../../src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj");

        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItem.axaml.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/Controls/ShowCasePanel.axaml.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/Controls/GalleryStickyTabsHost.cs")).ShouldBeFalse();

        var aliases = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/ShowCaseControlAliases.cs");
        aliases.ShouldContain("global using AtomUI.Toolkits.GalleryBase.Controls;");
        aliases.ShouldNotContain("global using AtomUIGallery.Controls;");
    }

    [Fact]
    public void GalleryBase_Exposes_Neutral_And_Legacy_Xaml_Namespaces_Without_Product_ShowCases()
    {
        var assemblyInfo = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Properties/AssemblyInfo.cs");

        assemblyInfo.ShouldContain("https://atomui.net/toolkits/gallery-base");
        assemblyInfo.ShouldContain("https://atomui.net/oss-controls/gallery");
        assemblyInfo.ShouldContain("AtomUI.Toolkits.GalleryBase.Controls");
        assemblyInfo.ShouldContain("AtomUI.Toolkits.GalleryBase.Models");
        assemblyInfo.ShouldNotContain("AtomUIGallery.ShowCases");
        assemblyInfo.ShouldNotContain("AtomUIGallery.Localization");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate) || Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
