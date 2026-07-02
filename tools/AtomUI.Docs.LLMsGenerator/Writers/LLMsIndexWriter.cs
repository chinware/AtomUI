using System.Text;
using AtomUI.Docs.LLMsGenerator.Reader;

namespace AtomUI.Docs.LLMsGenerator.Writers;

public static class LLMsIndexWriter
{
    public static string Write(IReadOnlyList<ControlDocModel> models)
    {
        var builder = new StringBuilder();
        builder.AppendLine("# AtomUI Desktop LLMS");
        builder.AppendLine();
        builder.AppendLine(LLMsWriterConstants.GeneratedMarker);
        builder.AppendLine();
        builder.AppendLine("## Navigation links");
        builder.AppendLine();
        builder.AppendLine("- [Full CN](./llms-full-cn.txt)");
        builder.AppendLine("- [Semantic CN](./llms-semantic-cn.md)");
        builder.AppendLine("- [Control documentation guidelines](../engineering/control-documentation-guidelines.md)");
        builder.AppendLine();
        builder.AppendLine("## Control Docs (CN)");
        builder.AppendLine();
        AppendControlLinks(builder, models, "index-cn.md");
        builder.AppendLine();
        builder.AppendLine("## Semantic (CN)");
        builder.AppendLine();
        AppendControlLinks(builder, models, "semantic-cn.md");
        builder.AppendLine();
        builder.AppendLine("## Source Docs");
        builder.AppendLine();
        foreach (var model in models)
        {
            builder.AppendLine($"- {model.DisplayName}: `{model.SourceOverviewRelativePath}`");
        }

        return builder.ToString().ReplaceLineEndings("\n").TrimEnd() + "\n";
    }

    private static void AppendControlLinks(StringBuilder builder, IReadOnlyList<ControlDocModel> models, string fileName)
    {
        foreach (var group in models.GroupBy(model => model.Category))
        {
            builder.AppendLine($"### {group.Key}");
            builder.AppendLine();
            foreach (var model in group)
            {
                builder.AppendLine($"- [{model.DisplayName}](./controls/{model.ControlName}/{fileName})");
            }

            builder.AppendLine();
        }
    }
}
