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

    [Theory]
    [InlineData("button", "Button root 是动作、状态与根视觉样式的统一 owner。")]
    [InlineData("badge", "CountBadge indicator 表示完整数量徽标视觉。")]
    public void ReaderPrefersDedicatedSemanticPartDocument(string controlName, string expectedPartDescription)
    {
        var model = ReadModel(controlName);

        model.SourceSemanticPartPath.ShouldNotBeNull();
        model.SourceSemanticPartRelativePath.ShouldNotBeNull();
        model.SourceSemanticPartRelativePath.ShouldEndWith("/semantic-part.md");
        model.SemanticPartsMarkdown.ShouldContain(expectedPartDescription);
    }

    [Fact]
    public void ReaderFallsBackToOverviewWhenDedicatedSemanticPartDocumentIsAbsent()
    {
        var model = ReadModel("avatar");

        model.SourceSemanticPartPath.ShouldBeNull();
        model.SourceSemanticPartRelativePath.ShouldBeNull();
        model.HasExplicitSemanticParts.ShouldBeTrue();
    }

    [Theory]
    [InlineData("button", 3)]
    [InlineData("badge", 7)]
    [InlineData("card", 12)]
    public void ReaderParsesEveryPartFromGroupedSemanticTables(string controlName, int expectedPartCount)
    {
        var model = ReadModel(controlName);

        model.SemanticParts.Count.ShouldBe(expectedPartCount);
        model.SemanticParts.ShouldAllBe(row => row.Cells.Count == 13);
        model.SemanticParts.ShouldNotContain(row => row.Cells[0] == "Owner" || row.Cells[0] == "字段");
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
