using System.Text;
using AtomUI.Docs.LLMsGenerator.Reader;

namespace AtomUI.Docs.LLMsGenerator.Writers;

public static class LLMsControlWriter
{
    public static string Write(ControlDocModel model)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {model.DisplayName}");
        builder.AppendLine();
        builder.AppendLine(LLMsWriterConstants.GeneratedMarker);
        builder.AppendLine();
        AppendSection(builder, "概述", StripMetadataTable(model.OverviewSection));
        AppendPackageSection(builder, model);
        AppendSection(builder, "何时使用", BuildUsageSection(model));
        AppendSection(builder, "公共 API", model.ApiSection);
        AppendSection(builder, "事件与命令", BuildEventsSection(model));
        AppendSection(builder, "使用示例", BuildExamplesSection(model));
        AppendSection(builder, "状态模型", model.StateSection);
        AppendSection(builder, "主题与 Design Token", BuildTokenSection(model));
        AppendSection(builder, "AOT 与裁剪注意事项", model.ImplementationAotSection);
        AppendSection(builder, "源码索引", model.SourceIndex);
        AppendSection(builder, "相关文档", BuildRelatedDocsSection(model));
        return Normalize(builder.ToString());
    }

    private static void AppendPackageSection(StringBuilder builder, ControlDocModel model)
    {
        builder.AppendLine("## 包与命名空间");
        builder.AppendLine();
        builder.AppendLine("| 项 | 值 |");
        builder.AppendLine("| --- | --- |");
        builder.AppendLine($"| NuGet 包 | `{model.PackageName}` |");
        builder.AppendLine($"| .NET 命名空间 | `{model.DotNetNamespace}` |");
        builder.AppendLine($"| AXAML 命名空间 | `{model.AxamlNamespace}` |");
        builder.AppendLine($"| Gallery 页面 | `{model.GalleryPath}` |");
        builder.AppendLine($"| 状态 | {model.Status} |");
        builder.AppendLine();
    }

    private static string BuildUsageSection(ControlDocModel model)
    {
        var scenarioIndex = model.OverviewSection.IndexOf("典型使用场景", StringComparison.Ordinal);
        if (scenarioIndex >= 0)
        {
            return model.OverviewSection[scenarioIndex..].Trim();
        }

        return model.DesignLanguageSection;
    }

    private static string BuildEventsSection(ControlDocModel model)
    {
        var lines = model.ApiSection.Split('\n')
                         .Where(line => line.Contains("事件", StringComparison.Ordinal) ||
                                        line.Contains("命令", StringComparison.Ordinal) ||
                                        line.Contains("event", StringComparison.OrdinalIgnoreCase))
                         .ToArray();
        if (lines.Length == 0)
        {
            return $"{model.DisplayName} 的事件与命令以控件文档、源码 public surface 和 Avalonia 基类契约为准；生成器不从源码发明额外事件。";
        }

        return string.Join('\n', lines);
    }

    private static string BuildExamplesSection(ControlDocModel model)
    {
        return $"""
        稳定示例来源于 Gallery ShowCase 和源码查看片段。生成器只输出可从 `ShowCaseItem` 追溯的示例，不维护第二套手写示例。

        {model.GalleryExamplesMarkdown}
        """.Trim();
    }

    private static string BuildTokenSection(ControlDocModel model)
    {
        return $"""
        {model.ThemeSection}

        Token 来源：

        {model.TokenSourceDescription}
        """.Trim();
    }

    private static string BuildRelatedDocsSection(ControlDocModel model)
    {
        var lines = new List<string>
        {
            $"- 源设计文档：`{model.SourceOverviewRelativePath}`",
            $"- 实现文档：`{model.SourceImplementationRelativePath}`",
            $"- 变更记录：`{model.SourceChangelogRelativePath}`"
        };

        if (model.SourceTokenRelativePath is not null)
        {
            lines.Insert(2, $"- Token 文档：`{model.SourceTokenRelativePath}`");
        }

        if (model.SourceSemanticPartRelativePath is not null)
        {
            lines.Insert(2, $"- Semantic Part 文档：`{model.SourceSemanticPartRelativePath}`");
        }

        lines.Add($"- 语义结构：`./semantic-cn.md`");
        return string.Join('\n', lines);
    }

    private static void AppendSection(StringBuilder builder, string heading, string content)
    {
        builder.AppendLine($"## {heading}");
        builder.AppendLine();
        builder.AppendLine(string.IsNullOrWhiteSpace(content) ? "源文档未提供该章节的可抽取内容。" : content.Trim());
        builder.AppendLine();
    }

    private static string StripMetadataTable(string content)
    {
        var lines = content.ReplaceLineEndings("\n").Split('\n');
        var result = new List<string>();
        var skippingTable = false;
        var skippedAny = false;

        foreach (var line in lines)
        {
            if (!skippedAny && line.TrimStart().StartsWith('|'))
            {
                skippingTable = true;
                skippedAny = true;
                continue;
            }

            if (skippingTable && line.TrimStart().StartsWith('|'))
            {
                continue;
            }

            skippingTable = false;
            result.Add(line);
        }

        return string.Join('\n', result).Trim();
    }

    private static string Normalize(string content)
    {
        return content.ReplaceLineEndings("\n").TrimEnd() + "\n";
    }
}
