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

        string[] expectedPackageIds =
        [
            "AtomUI.Native",
            "AtomUI.Localization",
            "AtomUI.Core",
            "AtomUI.Fonts.AlibabaSans",
            "AtomUI.Controls.Shared",
            "AtomUI.Controls",
            "AtomUI.Desktop.Controls",
            "AtomUI.Toolkits.GalleryBase",
            "AtomUI.Generator",
            "AtomUI.Icons.Shared",
            "AtomUI.Icons.AntDesign",
            "AtomUI.Desktop.Controls.DataGrid",
            "AtomUI.Desktop.Controls.ColorPicker",
            "AtomUI.LanguagePack.Template",
            "AtomUI.Controls.I18n.PtBR",
            "AtomUI.Desktop.Controls.I18n.PtBR",
            "AtomUI.Desktop.Controls.DataGrid.I18n.PtBR",
            "AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR",
            "AtomUI.I18n.PtBR"
        ];
        foreach (var packageId in expectedPackageIds)
        {
            verificationStep.ShouldContain($"\"{packageId}\"");
        }

        verificationStep.ShouldContain("Missing NuGet packages");
        verificationStep.ShouldContain("Unexpected NuGet packages");
    }

    [Fact]
    public void Local_NuGet_Publish_Script_Builds_And_Packs_GalleryBase()
    {
        var script = ReadRepoFile("scripts/PublishToLocalSources.ps1");
        var scriptProject = $"../{GalleryBaseProject}";

        script.ShouldContain($"\"{scriptProject}\"");
        script.ShouldContain("dotnet build -v minimal --configuration $buildType $project");
        script.ShouldContain("dotnet pack --no-build --configuration $buildType $project");
    }

    [Fact]
    public void Packaging_Docs_List_GalleryBase_As_Main_Package()
    {
        var packagingDoc = ReadRepoFile("docs/architecture/foundations/build-and-packaging.md");

        packagingDoc.ShouldContain("- `AtomUI.Toolkits.GalleryBase`");
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
