using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Toolkits;

public class GalleryBasePackagingTests
{
    private const string GalleryBaseProject = "src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj";

    [Fact]
    public void Main_Release_Workflow_Builds_And_Packs_GalleryBase()
    {
        var workflow = ReadRepoFile(".github/workflows/release-nuget-packages.yml");
        var workflowProject = $"./{GalleryBaseProject}";

        workflow.ShouldContain($"dotnet build --configuration ${{{{ inputs.BuildType }}}} {workflowProject}");
        workflow.ShouldContain($"\"{workflowProject}\"");
    }

    [Fact]
    public void Local_NuGet_Publish_Script_Builds_And_Packs_GalleryBase()
    {
        var script = ReadRepoFile("scripts/PublishToLocalSources.ps1");
        var scriptProject = $"../{GalleryBaseProject}";

        script.ShouldContain($"dotnet build -v diag --configuration $buildType {scriptProject}");
        script.ShouldContain($"dotnet pack --no-build --configuration $buildType {scriptProject}");
    }

    [Fact]
    public void Packaging_Docs_List_GalleryBase_As_Main_Package()
    {
        var packagingDoc = ReadRepoFile("docs/architecture/build-and-packaging.md");
        var readme = ReadRepoFile("README.nuget.md");

        packagingDoc.ShouldContain("- `AtomUI.Toolkits.GalleryBase`");
        readme.ShouldContain("AtomUI.Toolkits.GalleryBase");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = Path.Combine(GetRepoRoot(), relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "AtomUI.slnx");
            if (File.Exists(candidate))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return AppContext.BaseDirectory;
    }
}
