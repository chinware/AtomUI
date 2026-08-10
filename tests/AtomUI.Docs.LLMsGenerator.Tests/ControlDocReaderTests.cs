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
        var model = ReadModel("steps");

        model.ImplementationLifecycleSection.ShouldContain("容器准备");
        model.ImplementationAotSection.ShouldContain("AOT 边界");
        model.ImplementationInvariantsSection.ShouldContain("Current 是唯一当前步骤输入");
        model.ImplementationTestsSection.ShouldContain("交互与 Wave");
    }

    [Fact]
    public void StepsGeneratedDocumentsMatchCurrentSources()
    {
        var model = ReadModel("steps");
        var index = LLMsControlWriter.Write(model);
        var semantic = LLMsSemanticWriter.Write(model);

        File.ReadAllText(Path.Combine(TestRepository.RootPath, model.OutputIndexPath))
            .ShouldBe(index);
        File.ReadAllText(Path.Combine(TestRepository.RootPath, model.OutputSemanticPath))
            .ShouldBe(semantic);

        File.ReadAllText(Path.Combine(TestRepository.RootPath, "docs/AI/generated/llms/llms-full-cn.txt"))
            .ShouldContain($"Source: ./controls/steps/index-cn.md\n\n{index.TrimEnd()}");
        File.ReadAllText(Path.Combine(TestRepository.RootPath, "docs/AI/generated/llms/llms-semantic-cn.md"))
            .ShouldContain($"Source: ./controls/steps/semantic-cn.md\n\n{semantic.TrimEnd()}");
    }

    [Fact]
    public void UploadGeneratedDocumentsMatchCurrentSources()
    {
        var model = ReadModel("upload");

        File.ReadAllText(Path.Combine(TestRepository.RootPath, model.OutputIndexPath))
            .ShouldBe(LLMsControlWriter.Write(model));
        File.ReadAllText(Path.Combine(TestRepository.RootPath, model.OutputSemanticPath))
            .ShouldBe(LLMsSemanticWriter.Write(model));
    }

    private static ControlDocModel ReadModel(string controlName)
    {
        var config = LLMsGeneratorConfigReader.Read(
            Path.Combine(TestRepository.RootPath, "docs/AI/generated/llms.config.json"));
        var control = config.ControlSets
                            .SelectMany(controlSet => ControlInventory.Discover(
                                TestRepository.RootPath,
                                controlSet,
                                config.OutputRoot,
                                config.DefaultLanguage))
                            .Single(candidate => candidate.Name == controlName);

        return ControlDocReader.Read(TestRepository.RootPath, control);
    }
}
