using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Toolkits;

public class GalleryBasePackagingTests
{
    private const string PackageManifest = "scripts/NuGetPackageProjects.ps1";
    private const string GalleryBaseProject = "src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj";
    private const string ExtrasProject = "src/AtomUI.Desktop.Controls.Extras/AtomUI.Desktop.Controls.Extras.csproj";

    [Theory]
    [InlineData("AtomUI.Toolkits.GalleryBase", GalleryBaseProject)]
    [InlineData("AtomUI.Desktop.Controls.Extras", ExtrasProject)]
    public void Release_Package_Manifest_Includes_Product_Package(string packageId, string projectPath)
    {
        var manifest = ReadRepoFile(PackageManifest);

        manifest.ShouldContain($"PackageId = \"{packageId}\"");
        manifest.ShouldContain($"ProjectPath = \"{projectPath}\"");
    }

    [Fact]
    public void Main_Release_Workflow_Builds_And_Packs_All_Manifest_Groups()
    {
        var workflow = ReadRepoFile(".github/workflows/release-nuget-packages.yml");

        workflow.ShouldContain(". ./scripts/NuGetPackageProjects.ps1");
        workflow.ShouldContain("foreach ($project in $AtomUIBasePackageProjects)");
        workflow.ShouldContain("foreach ($project in $AtomUIExtensionPackageProjects)");
        workflow.ShouldContain("foreach ($project in $AtomUILanguagePackageProjects)");
        workflow.ShouldContain("dotnet build --configuration ${{ inputs.BuildType }} $project");
        workflow.ShouldContain("dotnet pack --no-build --output $env:BASE_OUTPUT_DIR --configuration ${{ inputs.BuildType }} $project");
    }

    [Fact]
    public void Main_Release_Workflow_Verifies_All_Expected_NuGet_Packages()
    {
        var workflow = ReadRepoFile(".github/workflows/release-nuget-packages.yml");
        const string verificationStepName = "-  name: Verify AtomUI NuGet package artifacts";
        var verificationStepStart = workflow.IndexOf(verificationStepName, StringComparison.Ordinal);
        verificationStepStart.ShouldBeGreaterThanOrEqualTo(0);
        var uploadStepStart = workflow.IndexOf(
            "-  name: Upload NuGet artifacts",
            verificationStepStart,
            StringComparison.Ordinal);
        uploadStepStart.ShouldBeGreaterThan(verificationStepStart);
        var verificationStep = workflow[verificationStepStart..uploadStepStart];

        verificationStep.ShouldContain(". ./scripts/NuGetPackageProjects.ps1");
        verificationStep.ShouldContain("./build/Versions.props");
        verificationStep.ShouldContain("$AtomUIExpectedPackageIds");
        verificationStep.ShouldContain("Missing NuGet packages");
        verificationStep.ShouldContain("Unexpected NuGet packages");
    }

    [Fact]
    public void Local_NuGet_Publish_Script_Builds_And_Packs_Manifest_Groups()
    {
        var script = ReadRepoFile("scripts/PublishToLocalSources.ps1");

        script.ShouldContain(". \"$PSScriptRoot/NuGetPackageProjects.ps1\"");
        script.ShouldContain("foreach ($project in $AtomUIBasePackageProjects)");
        script.ShouldContain("foreach ($project in $AtomUIExtensionPackageProjects)");
        script.ShouldContain("foreach ($project in $AtomUILanguagePackageProjects)");
        script.ShouldContain("dotnet build -v minimal --configuration $buildType $project");
        script.ShouldContain("dotnet pack --no-build --configuration $buildType $project");
    }

    [Fact]
    public void Packaging_Docs_List_GalleryBase_As_Main_Package()
    {
        var packagingDoc = ReadRepoFile("docs/architecture/foundations/build-and-packaging.md");

        packagingDoc.ShouldContain("- `AtomUI.Toolkits.GalleryBase`");
        packagingDoc.ShouldContain("- `AtomUI.Desktop.Controls.Extras`");
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
