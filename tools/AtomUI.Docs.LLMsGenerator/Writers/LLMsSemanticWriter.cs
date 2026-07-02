using System.Text;
using AtomUI.Docs.LLMsGenerator.Reader;

namespace AtomUI.Docs.LLMsGenerator.Writers;

public static class LLMsSemanticWriter
{
    public static string Write(ControlDocModel model)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {model.DisplayName} 语义结构");
        builder.AppendLine();
        builder.AppendLine(LLMsWriterConstants.GeneratedMarker);
        builder.AppendLine();
        AppendSection(builder, "Semantic Parts", model.SemanticPartsMarkdown);
        AppendSection(builder, "Abstract AXAML Structure", BuildAbstractStructure(model));
        AppendSection(builder, "Composition Model", BuildCompositionModel(model));
        AppendSection(builder, "Template Parts", model.TemplatePartsMarkdown);
        AppendSection(builder, "Pseudo Classes", model.PseudoClassesMarkdown);
        AppendSection(builder, "State Flow", model.StateSection);
        AppendSection(builder, "Theme and Token Boundaries", BuildThemeBoundarySection(model));
        AppendSection(builder, "Customization Boundaries", BuildCustomizationSection(model));
        return builder.ToString().ReplaceLineEndings("\n").TrimEnd() + "\n";
    }

    private static string BuildAbstractStructure(ControlDocModel model)
    {
        return string.IsNullOrWhiteSpace(model.AbstractAxamlStructureMarkdown)
            ? "未定位到可生成抽象 AXAML 结构的 ControlTheme 模板。生成器不会根据 semantic parts 发明 AXAML 节点；请以 Template Parts、主题文件和源码索引为准。"
            : model.AbstractAxamlStructureMarkdown;
    }

    private static string BuildCompositionModel(ControlDocModel model)
    {
        return string.IsNullOrWhiteSpace(model.CompositionModelMarkdown)
            ? "该控件主要由 public 控件和 ControlTheme 模板直接表达，没有额外运行时组合层。"
            : model.CompositionModelMarkdown;
    }

    private static string BuildThemeBoundarySection(ControlDocModel model)
    {
        return $"""
        {model.ThemeSection}

        Token 边界：

        {model.TokenSourceDescription}
        """.Trim();
    }

    private static string BuildCustomizationSection(ControlDocModel model)
    {
        return $"""
        {model.CompatibilitySection}

        维护不变量：

        {model.ImplementationInvariantsSection}
        """.Trim();
    }

    private static void AppendSection(StringBuilder builder, string heading, string content)
    {
        builder.AppendLine($"## {heading}");
        builder.AppendLine();
        builder.AppendLine(string.IsNullOrWhiteSpace(content) ? "源文档未提供该章节的可抽取内容。" : content.Trim());
        builder.AppendLine();
    }
}
