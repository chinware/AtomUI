using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class GalleryReleaseWorkflowTests
{
    [Fact]
    public void MacOS_Dmg_Jobs_Remove_Unused_Homebrew_Taps_Before_Installing_Packages()
    {
        var workflow = File.ReadAllText(GetRepoFile(".github/workflows/release-gallery.yml"));

        AssertHomebrewTapCleanupBeforeInstall(workflow, "Build macOS x86_64 DMG Installer", "/dmg/x64");
        AssertHomebrewTapCleanupBeforeInstall(workflow, "Build macOS arm64 DMG Installer", "/dmg/arm64");
    }

    private static void AssertHomebrewTapCleanupBeforeInstall(string workflow, string jobName, string artifactPath)
    {
        var jobIndex = workflow.IndexOf($"name: {jobName}", StringComparison.Ordinal);
        jobIndex.ShouldBeGreaterThanOrEqualTo(0, $"Could not find workflow job: {jobName}");

        var prepareStepIndex = workflow.IndexOf($"GalleryArtifactsOutputDir }}}}{artifactPath}", jobIndex, StringComparison.Ordinal);
        prepareStepIndex.ShouldBeGreaterThanOrEqualTo(0, $"Could not find prepare step for workflow job: {jobName}");

        var tapCleanupIndex = workflow.IndexOf("brew untap --force aws/tap azure/bicep", prepareStepIndex, StringComparison.Ordinal);
        tapCleanupIndex.ShouldBeGreaterThanOrEqualTo(0, $"Missing Homebrew tap cleanup in workflow job: {jobName}");

        var brewInstallIndex = workflow.IndexOf("brew install create-dmg openssl@3", prepareStepIndex, StringComparison.Ordinal);
        brewInstallIndex.ShouldBeGreaterThanOrEqualTo(0, $"Missing Homebrew package install in workflow job: {jobName}");
        tapCleanupIndex.ShouldBeLessThan(brewInstallIndex, $"Homebrew tap cleanup must run before package install in workflow job: {jobName}");
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
