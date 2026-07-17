using AtomUI.Docs.LLMsGenerator.Catalog;
using AtomUI.Docs.LLMsGenerator.Config;
using AtomUI.Docs.LLMsGenerator.Reader;
using AtomUI.Docs.LLMsGenerator.Writers;
using Shouldly;
using Xunit;

namespace AtomUI.Docs.LLMsGenerator.Tests;

public class ControlDocReaderTests
{
    [Fact]
    public void ReaderUsesCurrentImplementationSectionNumbers()
    {
        var model = ReadStepsModel();

        model.ImplementationLifecycleSection.ShouldContain("容器准备");
        model.ImplementationAotSection.ShouldContain("AOT 边界");
        model.ImplementationInvariantsSection.ShouldContain("Current 是唯一当前步骤输入");
        model.ImplementationTestsSection.ShouldContain("交互与 Wave");
    }

    [Fact]
    public void StepsGeneratedDocumentsMatchCurrentSources()
    {
        var model = ReadStepsModel();
        var index = LLMsControlWriter.Write(model);
        var semantic = LLMsSemanticWriter.Write(model);

        File.ReadAllText(Path.Combine(TestRepository.RootPath, model.OutputIndexPath))
            .ShouldBe(index);
        File.ReadAllText(Path.Combine(TestRepository.RootPath, model.OutputSemanticPath))
            .ShouldBe(semantic);

        File.ReadAllText(Path.Combine(TestRepository.RootPath, "docs/AI/llms/llms-full-cn.txt"))
            .ShouldContain($"Source: ./controls/steps/index-cn.md\n\n{index.TrimEnd()}");
        File.ReadAllText(Path.Combine(TestRepository.RootPath, "docs/AI/llms/llms-semantic-cn.md"))
            .ShouldContain($"Source: ./controls/steps/semantic-cn.md\n\n{semantic.TrimEnd()}");
    }

    private static ControlDocModel ReadStepsModel()
    {
        var config = LLMsGeneratorConfigReader.Read(
            Path.Combine(TestRepository.RootPath, "docs/AI/llms.config.json"));
        var steps = config.ControlSets
                          .SelectMany(controlSet => ControlInventory.Discover(
                              TestRepository.RootPath,
                              controlSet,
                              config.OutputRoot,
                              config.DefaultLanguage))
                          .Single(control => control.Name == "steps");

        return ControlDocReader.Read(TestRepository.RootPath, steps);
    }
}
