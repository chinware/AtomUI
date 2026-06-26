using AtomUI.Docs.LLMsGenerator.Catalog;
using AtomUI.Docs.LLMsGenerator.Config;
using AtomUI.Docs.LLMsGenerator.Reader;
using AtomUI.Docs.LLMsGenerator.Writers;
using Shouldly;
using Xunit;

namespace AtomUI.Docs.LLMsGenerator.Tests;

public class LLMsWriterTests
{
    private static readonly string[] IndexHeadings =
    [
        "概述",
        "包与命名空间",
        "何时使用",
        "公共 API",
        "事件与命令",
        "使用示例",
        "状态模型",
        "主题与 Design Token",
        "AOT 与裁剪注意事项",
        "源码索引",
        "相关文档"
    ];

    private static readonly string[] SemanticHeadings =
    [
        "Semantic Parts",
        "Abstract AXAML Structure",
        "Composition Model",
        "Template Parts",
        "Pseudo Classes",
        "State Flow",
        "Theme and Token Boundaries",
        "Customization Boundaries"
    ];

    [Fact]
    public void PerControlWritersProduceButtonContractHeadings()
    {
        var model = ReadButtonModel();

        var index = LLMsControlWriter.Write(model);
        var semantic = LLMsSemanticWriter.Write(model);

        model.OutputIndexPath.ShouldBe("docs/AI/llms/controls/button/index-cn.md");
        model.OutputSemanticPath.ShouldBe("docs/AI/llms/controls/button/semantic-cn.md");
        ExtractSecondLevelHeadings(index).ShouldBe(IndexHeadings);
        ExtractSecondLevelHeadings(semantic).ShouldBe(SemanticHeadings);
        index.ShouldContain(LLMsWriterConstants.GeneratedMarker);
        index.ShouldContain("### 按钮类型");
        index.ShouldContain("SourceKey：`button-type`");
        index.ShouldContain("```axaml");
        index.ShouldContain("<atom:Button ButtonType=\"Primary\" Content=\"主要按钮\" />");
        index.ShouldNotContain("{gallery:ButtonShowCaseLangResource P2ContentPrimaryButton}");
        semantic.ShouldContain(LLMsWriterConstants.GeneratedMarker);
        semantic.ShouldNotContain("<SemanticPart");
        semantic.ShouldContain("<Panel>");
        semantic.ShouldContain("<WaveSpiritDecorator Name=\"PART_WaveSpirit\"");
        semantic.ShouldContain("<ContentPresenter Name=\"PART_ContentPresenter\"");
        semantic.ShouldContain("## Composition Model");

        var composition = ExtractSecondLevelSection(semantic, "Composition Model");
        composition.ShouldNotContain("DropdownButton");
        composition.ShouldNotContain("SplitButton");
        composition.ShouldNotContain("HyperLinkButton");
    }

    [Fact]
    public void SemanticWriterDoesNotInventAbstractStructureWhenControlThemeIsMissing()
    {
        var model = new ControlDocModel
        {
            DisplayName = "NoTemplate",
            SemanticPartsMarkdown = "| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |\n| --- | --- | --- | --- | --- | --- |\n| `root` | `NoTemplate` | root | API | Token | stable |",
            AbstractAxamlStructureMarkdown = string.Empty,
            TemplatePartsMarkdown = "无",
            PseudoClassesMarkdown = "无",
            StateSection = "state",
            ThemeSection = "theme",
            TokenSourceDescription = "token",
            CompatibilitySection = "compatibility",
            ImplementationInvariantsSection = "invariants"
        };

        var semantic = LLMsSemanticWriter.Write(model);

        semantic.ShouldNotContain("<SemanticPart");
        semantic.ShouldNotContain("<NoTemplate>");
        semantic.ShouldContain("未定位到可生成抽象 AXAML 结构的 ControlTheme 模板");
        semantic.ShouldContain("该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层");
    }

    [Fact]
    public void SemanticWriterIncludesThemeCompositionModelForComplexControls()
    {
        var models = ReadAllModels();

        var drawer = LLMsSemanticWriter.Write(models.Single(model => model.ControlName == "drawer"));
        drawer.ShouldContain("## Composition Model");
        drawer.ShouldContain("DrawerContainer");
        drawer.ShouldContain("DrawerInfoContainer");
        drawer.ShouldContain("PART_Mask");
        drawer.ShouldContain("PART_CloseButton");
        drawer.ShouldContain("template-stable");
        drawer.ShouldContain("internal-observable");

        var badge = LLMsSemanticWriter.Write(models.Single(model => model.ControlName == "badge"));
        badge.ShouldContain("CountBadgeAdorner");
        badge.ShouldContain("DotBadgeAdorner");
        badge.ShouldContain("RibbonBadgeAdorner");
        badge.ShouldContain("PART_MotionActor");
        badge.ShouldContain("DotBadgeIndicator");
        badge.ShouldContain("internal-observable");

        var badgeComposition = ExtractSecondLevelSection(badge, "Composition Model");
        badgeComposition.ShouldContain("""
          -> CountBadgeAdorner (internal adorner control theme, CountBadgeAdornerTheme.axaml)
             -> MotionActor#PART_MotionActor (template-stable)
                -> Panel#RootLayout (template-stable)
                   -> Border#BadgeIndicator (template-stable)
                   -> TextBlock#BadgeText (template-stable)
        """);
        badgeComposition.ShouldContain("""
          -> DotBadgeAdorner (internal adorner control theme, DotBadgeAdornerTheme.axaml)
             -> DockPanel#RootLayout (template-stable)
                -> MotionActor#PART_MotionActor (template-stable)
                   -> DotBadgeIndicator#Indicator (internal-observable)
                -> Label#Label (template-stable)
        """);
        badgeComposition.ShouldNotContain("     -> MotionActor (motion actor)");
        badgeComposition.ShouldNotContain("     -> Panel (template node)");

        var breadcrumb = LLMsSemanticWriter.Write(models.Single(model => model.ControlName == "breadcrumb"));
        breadcrumb.ShouldContain("BreadcrumbItem");
        breadcrumb.ShouldContain("ItemsPresenter");
        breadcrumb.ShouldContain("BreadcrumbItemTheme.axaml");
        breadcrumb.ShouldContain("template-stable");
    }

    [Fact]
    public void RootAndAggregateWritersCoverEveryDesktopControl()
    {
        var models = ReadAllModels();
        var files = LLMsDocumentationWriter.WriteAll(models);

        files.Count(file => file.Path.EndsWith("/index-cn.md", StringComparison.Ordinal)).ShouldBe(76);
        files.Count(file => file.Path.EndsWith("/semantic-cn.md", StringComparison.Ordinal)).ShouldBe(76);
        files.ShouldContain(file => file.Path == "docs/AI/llms/llms.txt");
        files.ShouldContain(file => file.Path == "docs/AI/llms/llms-full-cn.txt");
        files.ShouldContain(file => file.Path == "docs/AI/llms/llms-semantic-cn.md");

        var index = files.Single(file => file.Path == "docs/AI/llms/llms.txt").Content;
        index.ShouldContain("./controls/button/index-cn.md");
        index.ShouldContain("./controls/button/semantic-cn.md");
        index.ShouldNotContain("components/");

        var full = files.Single(file => file.Path == "docs/AI/llms/llms-full-cn.txt").Content;
        CountSourceMarkers(full, "index-cn.md").ShouldBe(76);

        var semantic = files.Single(file => file.Path == "docs/AI/llms/llms-semantic-cn.md").Content;
        CountSourceMarkers(semantic, "semantic-cn.md").ShouldBe(76);
    }

    private static ControlDocModel ReadButtonModel()
    {
        return ReadAllModels().Single(model => model.ControlName == "button");
    }

    private static IReadOnlyList<ControlDocModel> ReadAllModels()
    {
        var config = LLMsGeneratorConfigReader.Read(Path.Combine(TestRepository.RootPath, "docs/AI/llms.config.json"));
        var controls = config.ControlSets
                             .SelectMany(controlSet => ControlInventory.Discover(
                                 TestRepository.RootPath,
                                 controlSet,
                                 config.OutputRoot,
                                 config.DefaultLanguage))
                             .ToArray();

        return controls.Select(control => ControlDocReader.Read(TestRepository.RootPath, control)).ToArray();
    }

    private static string[] ExtractSecondLevelHeadings(string markdown)
    {
        return markdown.Split('\n')
                       .Where(line => line.StartsWith("## ", StringComparison.Ordinal))
                       .Select(line => line[3..].Trim())
                       .ToArray();
    }

    private static string ExtractSecondLevelSection(string markdown, string heading)
    {
        var lines = markdown.ReplaceLineEndings("\n").Split('\n');
        var start = Array.FindIndex(lines, line => line == $"## {heading}");
        if (start < 0)
        {
            return string.Empty;
        }

        var end = Array.FindIndex(lines, start + 1, line => line.StartsWith("## ", StringComparison.Ordinal));
        if (end < 0)
        {
            end = lines.Length;
        }

        return string.Join('\n', lines[start..end]);
    }

    private static int CountSourceMarkers(string content, string fileName)
    {
        return content.Split('\n').Count(line =>
            line.StartsWith("Source: ./controls/", StringComparison.Ordinal) &&
            line.EndsWith(fileName, StringComparison.Ordinal));
    }
}
