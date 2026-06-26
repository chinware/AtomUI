using AtomUI.Docs.LLMsGenerator.Catalog;
using AtomUI.Docs.LLMsGenerator.Config;
using AtomUI.Docs.LLMsGenerator.Reader;
using AtomUI.Docs.LLMsGenerator.Verification;
using AtomUI.Docs.LLMsGenerator.Writers;
using Shouldly;
using Xunit;

namespace AtomUI.Docs.LLMsGenerator.Tests;

public class LLMsVerifierTests
{
    [Fact]
    public void SourceVerifierAcceptsNormalizedButtonDocs()
    {
        var model = ReadButtonModel();

        var diagnostics = LLMsVerifier.ValidateSource(model);

        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void GeneratedContentVerifierRejectsStalePathsPlaceholdersAndForbiddenNames()
    {
        var content = $"""
        # Broken

        Generated output contains components/button/index-cn.md.
        TODO: fill this in.
        {ForbiddenExternalName()}
        """;

        var diagnostics = LLMsVerifier.ValidateGeneratedContent("docs/AI/llms/controls/button/index-cn.md", content);

        diagnostics.ShouldContain(diagnostic => diagnostic.Contains("generated marker", StringComparison.OrdinalIgnoreCase));
        diagnostics.ShouldContain(diagnostic => diagnostic.Contains("stale component path", StringComparison.OrdinalIgnoreCase));
        diagnostics.ShouldContain(diagnostic => diagnostic.Contains("placeholder", StringComparison.OrdinalIgnoreCase));
        diagnostics.ShouldContain(diagnostic => diagnostic.Contains("forbidden external project name", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void GeneratedOutputVerifierRejectsStaleFilesOnDisk()
    {
        var directory = Directory.CreateTempSubdirectory("atomui-llms-verify-");
        var expected = new GeneratedLLMsFile("docs/AI/llms/llms.txt", "# Index\n\n" + LLMsWriterConstants.GeneratedMarker + "\n");
        var stalePath = Path.Combine(directory.FullName, "docs/AI/llms/controls/stale/index-cn.md");
        Directory.CreateDirectory(Path.GetDirectoryName(stalePath)!);
        File.WriteAllText(stalePath, "stale");

        var diagnostics = LLMsVerifier.VerifyGeneratedFiles(directory.FullName, [expected]);

        diagnostics.ShouldContain(diagnostic => diagnostic.Contains("stale generated file", StringComparison.OrdinalIgnoreCase));
    }

    private static ControlDocModel ReadButtonModel()
    {
        var config = LLMsGeneratorConfigReader.Read(Path.Combine(TestRepository.RootPath, "docs/AI/llms.config.json"));
        var controls = config.ControlSets
                             .SelectMany(controlSet => ControlInventory.Discover(
                                 TestRepository.RootPath,
                                 controlSet,
                                 config.OutputRoot,
                                 config.DefaultLanguage))
                             .ToArray();

        var button = controls.Single(control => control.Name == "button");
        return ControlDocReader.Read(TestRepository.RootPath, button);
    }

    private static string ForbiddenExternalName()
    {
        return "Ant" + " " + "Design";
    }
}
