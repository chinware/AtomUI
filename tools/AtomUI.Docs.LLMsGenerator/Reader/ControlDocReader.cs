using System.Text;
using System.Text.RegularExpressions;
using AtomUI.Docs.LLMsGenerator.Catalog;
using AtomUI.Docs.LLMsGenerator.Markdown;

namespace AtomUI.Docs.LLMsGenerator.Reader;

public static partial class ControlDocReader
{
    private static readonly IReadOnlyDictionary<string, string> CategoryGalleryNames =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["data-display"] = "DataDisplay",
            ["data-entry"] = "DataEntry",
            ["feedback"] = "Feedback",
            ["general"] = "General",
            ["layout"] = "Layout",
            ["navigation"] = "Navigation",
            ["other"] = "Other",
            ["window"] = "Window"
        };

    public static ControlDocModel Read(string repositoryRoot, ControlDocumentInfo control)
    {
        var overviewText = File.ReadAllText(control.OverviewPath);
        var implementationText = File.ReadAllText(control.ImplementationPath);
        var semanticPartText = control.SemanticPartPath is null
            ? overviewText
            : File.ReadAllText(control.SemanticPartPath);
        var overview = MarkdownSectionParser.Parse(overviewText, control.OverviewPath);
        var implementation = MarkdownSectionParser.Parse(implementationText, control.ImplementationPath);

        var metadata = ReadMetadata(overview.GetRequiredSection("1. 控件定位").Content);
        var displayName = ReadDisplayName(overview.Title, control.Name);
        var galleryPath = ReadMetadataValue(metadata, "Gallery 页面");
        if (string.IsNullOrWhiteSpace(galleryPath))
        {
            galleryPath = FindGalleryPath(repositoryRoot, control, displayName);
        }

        var semanticPartsMarkdown = ExtractSemanticPartsMarkdown(semanticPartText);
        var semanticParts = ParseSemanticParts(semanticPartsMarkdown);
        var templatePartsMarkdown = ExtractTableByLeadText(overviewText, "template part");
        var pseudoClassesMarkdown = ExtractPseudoClassesMarkdown(overviewText);
        var sourceIndex = implementation.GetRequiredSection("2. 源码文件结构").Content.Trim();
        var hasTokenDoc = control.TokenPath is not null;
        var tokenSourceDescription = ReadTokenDescription(control.TokenPath, overviewText, displayName, hasTokenDoc);

        return new ControlDocModel
        {
            ControlName = control.Name,
            DisplayName = displayName,
            Category = control.Category,
            SourceOverviewPath = control.OverviewPath,
            SourceImplementationPath = control.ImplementationPath,
            SourceSemanticPartPath = control.SemanticPartPath,
            SourceTokenPath = control.TokenPath,
            SourceChangelogPath = control.ChangelogPath,
            OutputIndexPath = control.OutputIndexPath,
            OutputSemanticPath = control.OutputSemanticPath,
            PackageName = ReadMetadataValue(metadata, "NuGet 包", InferPackageName(control.Name)),
            DotNetNamespace = ReadMetadataValue(metadata, ".NET 命名空间", InferNamespace(control.Name)),
            AxamlNamespace = ReadMetadataValue(metadata, "AXAML 命名空间", "https://atomui.net"),
            GalleryPath = galleryPath,
            Status = ReadMetadataValue(metadata, "控件状态", "Stable"),
            SemanticParts = semanticParts.Count > 0 ? semanticParts : BuildFallbackSemanticParts(displayName, templatePartsMarkdown, tokenSourceDescription),
            SemanticPartsMarkdown = !string.IsNullOrWhiteSpace(semanticPartsMarkdown)
                ? semanticPartsMarkdown
                : FormatSemanticParts(BuildFallbackSemanticParts(displayName, templatePartsMarkdown, tokenSourceDescription)),
            TemplatePartsMarkdown = !string.IsNullOrWhiteSpace(templatePartsMarkdown)
                ? templatePartsMarkdown
                : "源文档未声明稳定 Template Part。维护模板时应以源码和主题文件中的实际声明为准。",
            AbstractAxamlStructureMarkdown = ControlThemeStructureReader.Read(
                repositoryRoot,
                control,
                displayName,
                sourceIndex),
            CompositionModelMarkdown = CompositionModelReader.Read(
                repositoryRoot,
                control,
                displayName,
                sourceIndex),
            PseudoClassesMarkdown = !string.IsNullOrWhiteSpace(pseudoClassesMarkdown)
                ? pseudoClassesMarkdown
                : "源文档未声明控件专属伪类。控件仍可能消费 Avalonia 标准状态，例如 `:pointerover`、`:pressed`、`:disabled` 和 focus 相关状态。",
            SourceIndex = sourceIndex,
            HasTokenDoc = hasTokenDoc,
            TokenSourceDescription = tokenSourceDescription,
            OverviewSection = overview.GetRequiredSection("1. 控件定位").Content.Trim(),
            DesignLanguageSection = overview.GetRequiredSection("2. 设计语言").Content.Trim(),
            ApiSection = overview.GetRequiredSection("3. API 与契约模型").Content.Trim(),
            StateSection = overview.GetRequiredSection("4. 行为与状态模型").Content.Trim(),
            ThemeSection = overview.GetRequiredSection("5. 视觉与主题模型").Content.Trim(),
            CompatibilitySection = overview.GetRequiredSection("7. 兼容性不变量").Content.Trim(),
            VerificationSection = GetRequiredSection(
                overview,
                "9. 文档导航、LLMS 导出与验证策略",
                "9. 文档导航与验证策略").Content.Trim(),
            ImplementationLifecycleSection = GetRequiredSection(
                implementation,
                "6. 生命周期与模板接入",
                "5. 生命周期与模板接入").Content.Trim(),
            ImplementationAotSection = GetRequiredSection(
                implementation,
                "9. 资源、性能与 AOT 边界",
                "8. 资源、性能与 AOT 边界").Content.Trim(),
            ImplementationInvariantsSection = GetRequiredSection(
                implementation,
                "10. 维护不变量",
                "9. 维护不变量").Content.Trim(),
            ImplementationTestsSection = GetRequiredSection(
                implementation,
                "11. 测试与验证",
                "10. 测试与验证").Content.Trim(),
            GalleryExamplesMarkdown = GalleryExampleReader.ReadMarkdown(repositoryRoot, galleryPath, displayName),
            SourceOverviewRelativePath = ToRelativePath(repositoryRoot, control.OverviewPath),
            SourceImplementationRelativePath = ToRelativePath(repositoryRoot, control.ImplementationPath),
            SourceSemanticPartRelativePath = control.SemanticPartPath is null
                ? null
                : ToRelativePath(repositoryRoot, control.SemanticPartPath),
            SourceTokenRelativePath = control.TokenPath is null ? null : ToRelativePath(repositoryRoot, control.TokenPath),
            SourceChangelogRelativePath = ToRelativePath(repositoryRoot, control.ChangelogPath),
            HasExplicitMetadata = metadata.ContainsKey("NuGet 包") &&
                                  metadata.ContainsKey(".NET 命名空间") &&
                                  metadata.ContainsKey("AXAML 命名空间") &&
                                  metadata.ContainsKey("Gallery 页面") &&
                                  metadata.ContainsKey("控件状态"),
            HasExplicitSemanticParts = !string.IsNullOrWhiteSpace(semanticPartsMarkdown),
            HasExplicitExportSourceTable = overviewText.Contains("LLMS 导出来源", StringComparison.Ordinal) &&
                                           overviewText.Contains($"controls/{control.Name}/index-cn.md", StringComparison.Ordinal) &&
                                           overviewText.Contains($"controls/{control.Name}/semantic-cn.md", StringComparison.Ordinal),
            HasExplicitTokenSourceDescription = hasTokenDoc ||
                                                overviewText.Contains("Token 说明", StringComparison.Ordinal) ||
                                                overviewText.Contains("没有专属 Token", StringComparison.Ordinal) ||
                                                overviewText.Contains("复用", StringComparison.Ordinal)
        };
    }

    public static IReadOnlyList<ControlDocModel> ReadAll(string repositoryRoot, IReadOnlyList<ControlDocumentInfo> controls)
    {
        return controls.Select(control => Read(repositoryRoot, control)).ToArray();
    }

    private static IReadOnlyDictionary<string, string> ReadMetadata(string section)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var row in ParseMarkdownTable(section))
        {
            if (row.Cells.Count >= 2)
            {
                result[row.Cells[0]] = row.Cells[1].Trim('`');
            }
        }

        return result;
    }

    private static MarkdownSection GetRequiredSection(MarkdownDocument document, params string[] headings)
    {
        foreach (var heading in headings)
        {
            var match = document.Sections.FirstOrDefault(section => section.Heading == heading);
            if (match is not null)
            {
                return match;
            }
        }

        return document.GetRequiredSection(headings[0]);
    }

    private static string ReadMetadataValue(
        IReadOnlyDictionary<string, string> metadata,
        string key,
        string fallback = "")
    {
        return metadata.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : fallback;
    }

    private static string ReadDisplayName(string title, string controlName)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            var normalized = title.Replace(" 桌面版架构设计", string.Empty, StringComparison.Ordinal)
                                  .Replace("架构设计", string.Empty, StringComparison.Ordinal)
                                  .Trim();
            if (!string.IsNullOrWhiteSpace(normalized))
            {
                return normalized;
            }
        }

        return ToPascalName(controlName);
    }

    private static string InferPackageName(string controlName)
    {
        return controlName switch
        {
            "data-grid" => "AtomUI.Desktop.Controls.DataGrid",
            "color-picker" => "AtomUI.Desktop.Controls.ColorPicker",
            _ => "AtomUI.Desktop.Controls"
        };
    }

    private static string InferNamespace(string controlName)
    {
        return controlName switch
        {
            "data-grid" => "AtomUI.Desktop.Controls.DataGrid",
            "color-picker" => "AtomUI.Desktop.Controls.ColorPicker",
            _ => "AtomUI.Desktop.Controls"
        };
    }

    private static string FindGalleryPath(string repositoryRoot, ControlDocumentInfo control, string displayName)
    {
        if (string.IsNullOrWhiteSpace(control.GalleryRoot) || !Directory.Exists(control.GalleryRoot))
        {
            return string.Empty;
        }

        var categoryName = CategoryGalleryNames.GetValueOrDefault(control.Category, ToPascalName(control.Category));
        var categoryRoot = Path.Combine(control.GalleryRoot, categoryName);
        if (!Directory.Exists(categoryRoot))
        {
            return string.Empty;
        }

        var expectedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ToPascalName(control.Name),
            displayName,
            control.Name.Replace("-", string.Empty, StringComparison.Ordinal)
        };

        var match = Directory.GetDirectories(categoryRoot)
                             .FirstOrDefault(directory => expectedNames.Contains(Path.GetFileName(directory)));

        return match is null ? string.Empty : ToRelativePath(repositoryRoot, match);
    }

    private static string ReadTokenDescription(string? tokenPath, string overviewText, string displayName, bool hasTokenDoc)
    {
        if (tokenPath is not null)
        {
            var tokenText = File.ReadAllText(tokenPath);
            var tokenDocument = MarkdownSectionParser.Parse(tokenText, tokenPath);
            return tokenDocument.Sections.FirstOrDefault()?.Content.Trim()
                   ?? $"{displayName} 使用专属 token.md 说明组件 Token 语义。";
        }

        var tokenNote = ExtractLeadParagraphAfter(overviewText, "Token 说明");
        if (!string.IsNullOrWhiteSpace(tokenNote))
        {
            return tokenNote;
        }

        return $"{displayName} 当前没有专属 token.md；LLMS 输出按源文档中的主题模型、家族 Token、SharedToken 和主题资源说明 Token 边界。";
    }

    private static string ExtractSemanticPartsMarkdown(string markdown)
    {
        var lines = markdown.ReplaceLineEndings("\n").Split('\n');
        for (var index = 0; index < lines.Length; index++)
        {
            var trimmed = lines[index].TrimStart();
            if (trimmed.StartsWith("## ", StringComparison.Ordinal) &&
                trimmed.Contains("Semantic Parts", StringComparison.OrdinalIgnoreCase))
            {
                return ExtractSectionContent(lines, index);
            }
        }

        var markerIndex = markdown.IndexOf("LLMS 语义区域", StringComparison.Ordinal);
        if (markerIndex < 0)
        {
            return string.Empty;
        }

        return ExtractNextTable(markdown[markerIndex..]);
    }

    private static string ExtractSectionContent(IReadOnlyList<string> lines, int headingIndex)
    {
        var content = new List<string>();
        for (var index = headingIndex + 1; index < lines.Count; index++)
        {
            if (lines[index].TrimStart().StartsWith("## ", StringComparison.Ordinal))
            {
                break;
            }

            content.Add(lines[index]);
        }

        return string.Join('\n', content).Trim();
    }

    private static string ExtractTableByLeadText(string markdown, string leadText)
    {
        var lines = markdown.ReplaceLineEndings("\n").Split('\n');
        for (var index = 0; index < lines.Length; index++)
        {
            if (!lines[index].Contains(leadText, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var table = ExtractNextTable(string.Join('\n', lines.Skip(index)));
            if (!string.IsNullOrWhiteSpace(table))
            {
                return table;
            }
        }

        return string.Empty;
    }

    private static string ExtractPseudoClassesMarkdown(string markdown)
    {
        var lines = markdown.ReplaceLineEndings("\n").Split('\n');
        for (var index = 0; index < lines.Length; index++)
        {
            if (!lines[index].Contains("伪类", StringComparison.Ordinal))
            {
                continue;
            }

            var builder = new StringBuilder();
            for (var current = index; current < Math.Min(lines.Length, index + 12); current++)
            {
                if (current > index && lines[current].StartsWith("## ", StringComparison.Ordinal))
                {
                    break;
                }

                builder.AppendLine(lines[current]);
            }

            return builder.ToString().Trim();
        }

        return string.Empty;
    }

    private static string ExtractNextTable(string markdown)
    {
        var lines = markdown.ReplaceLineEndings("\n").Split('\n');
        var tableLines = new List<string>();
        var inTable = false;

        foreach (var line in lines)
        {
            if (line.TrimStart().StartsWith('|'))
            {
                inTable = true;
                tableLines.Add(line);
                continue;
            }

            if (inTable)
            {
                break;
            }
        }

        return string.Join('\n', tableLines).Trim();
    }

    private static IReadOnlyList<MarkdownTableRow> ParseMarkdownTable(string markdown)
    {
        var rows = new List<MarkdownTableRow>();
        foreach (var line in markdown.ReplaceLineEndings("\n").Split('\n'))
        {
            var trimmed = line.Trim();
            if (!trimmed.StartsWith('|') || !trimmed.EndsWith('|'))
            {
                continue;
            }

            if (SeparatorRowRegex().IsMatch(trimmed))
            {
                continue;
            }

            var cells = trimmed.Trim('|')
                               .Split('|')
                               .Select(cell => cell.Trim())
                               .ToArray();
            rows.Add(new MarkdownTableRow(cells));
        }

        return rows;
    }

    private static IReadOnlyList<MarkdownTableRow> ParseSemanticParts(string markdown)
    {
        var descriptorRows = new List<MarkdownTableRow>();
        foreach (var table in ExtractTables(markdown))
        {
            var rows = ParseMarkdownTable(table);
            if (rows.Count == 0)
            {
                continue;
            }

            var header = rows[0].Cells;
            if (header.Contains("Owner", StringComparer.Ordinal) &&
                header.Contains("Part", StringComparer.Ordinal))
            {
                descriptorRows.AddRange(rows.Skip(1));
                continue;
            }

            var fields = rows.Where(row => row.Cells.Count >= 2)
                             .ToDictionary(
                                 row => row.Cells[0],
                                 row => row.Cells[1],
                                 StringComparer.Ordinal);
            if (!fields.ContainsKey("Owner") || !fields.ContainsKey("Part"))
            {
                continue;
            }

            descriptorRows.Add(new MarkdownTableRow(
            [
                fields.GetValueOrDefault("Owner", string.Empty),
                fields.GetValueOrDefault("Part", string.Empty),
                fields.GetValueOrDefault("Selector", string.Empty),
                fields.GetValueOrDefault("ContractType", string.Empty),
                fields.GetValueOrDefault("Cardinality", string.Empty),
                fields.GetValueOrDefault("Customization", string.Empty),
                fields.GetValueOrDefault("CrossVisualRoot", string.Empty),
                fields.GetValueOrDefault("RuntimeCreated", string.Empty),
                fields.GetValueOrDefault("AtomUI 节点", string.Empty),
                fields.GetValueOrDefault("职责", string.Empty),
                fields.GetValueOrDefault("相关 API", string.Empty),
                fields.GetValueOrDefault("相关 Token", string.Empty),
                fields.GetValueOrDefault("稳定性", string.Empty)
            ]));
        }

        if (descriptorRows.Count > 0)
        {
            return descriptorRows;
        }

        return ParseMarkdownTable(markdown);
    }

    private static IReadOnlyList<string> ExtractTables(string markdown)
    {
        var tables = new List<string>();
        var tableLines = new List<string>();
        var inTable = false;

        foreach (var line in markdown.ReplaceLineEndings("\n").Split('\n'))
        {
            if (line.TrimStart().StartsWith('|'))
            {
                inTable = true;
                tableLines.Add(line);
                continue;
            }

            if (inTable)
            {
                tables.Add(string.Join('\n', tableLines).Trim());
                tableLines.Clear();
                inTable = false;
            }
        }

        if (inTable)
        {
            tables.Add(string.Join('\n', tableLines).Trim());
        }

        return tables;
    }

    private static IReadOnlyList<MarkdownTableRow> BuildFallbackSemanticParts(
        string displayName,
        string templatePartsMarkdown,
        string tokenSourceDescription)
    {
        var rows = new List<MarkdownTableRow>
        {
            new([
                "`root`",
                $"`{displayName}`",
                "控件根语义区域，承载 public API、状态归一、主题入口和 Gallery 可观察行为。",
                "见公共 API",
                "见主题与 Token 边界",
                "stable"
            ])
        };

        var templateRows = ParseMarkdownTable(templatePartsMarkdown)
            .Where(row => row.Cells.Count >= 3 && row.Cells[0] != "Template Part")
            .Take(8)
            .ToArray();

        foreach (var row in templateRows)
        {
            rows.Add(new MarkdownTableRow([
                NormalizeSemanticName(row.Cells[0]),
                row.Cells[0],
                row.Cells[2],
                "见公共 API",
                tokenSourceDescription.Contains("没有专属 token.md", StringComparison.Ordinal)
                    ? "SharedToken / 主题资源"
                    : "组件 Token / SharedToken",
                "stable"
            ]));
        }

        if (rows.Count == 1)
        {
            rows.Add(new MarkdownTableRow([
                "`content`",
                "抽象内容区域",
                "承载用户内容、集合项、输入值或可视反馈。",
                "见公共 API",
                "见主题与 Token 边界",
                "stable"
            ]));
        }

        return rows;
    }

    private static string FormatSemanticParts(IReadOnlyList<MarkdownTableRow> rows)
    {
        var builder = new StringBuilder();
        builder.AppendLine("| Part | AtomUI 节点 | 职责 | 相关 API | 相关 Token | 稳定性 |");
        builder.AppendLine("| --- | --- | --- | --- | --- | --- |");
        foreach (var row in rows.Where(row => row.Cells.Count >= 6))
        {
            builder.AppendLine($"| {string.Join(" | ", row.Cells.Take(6))} |");
        }

        return builder.ToString().TrimEnd();
    }

    private static string NormalizeSemanticName(string templatePart)
    {
        var value = templatePart.Trim('`');
        value = value.StartsWith("PART_", StringComparison.Ordinal) ? value[5..] : value;
        value = value.Replace("_", string.Empty, StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(value))
        {
            return "`part`";
        }

        return $"`{char.ToLowerInvariant(value[0])}{value[1..]}`";
    }

    private static string ExtractLeadParagraphAfter(string markdown, string marker)
    {
        var index = markdown.IndexOf(marker, StringComparison.Ordinal);
        if (index < 0)
        {
            return string.Empty;
        }

        var lines = markdown[index..].ReplaceLineEndings("\n").Split('\n').Skip(1);
        var paragraph = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (paragraph.Count > 0)
                {
                    break;
                }

                continue;
            }

            if (line.StartsWith("## ", StringComparison.Ordinal))
            {
                break;
            }

            paragraph.Add(line);
        }

        return string.Join('\n', paragraph).Trim();
    }

    private static string ToPascalName(string value)
    {
        return string.Concat(value.Split('-', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
    }

    private static string ToRelativePath(string repositoryRoot, string path)
    {
        return Path.GetRelativePath(repositoryRoot, path).Replace(Path.DirectorySeparatorChar, '/');
    }

    [GeneratedRegex(@"^\|\s*:?-{3,}:?\s*(\|\s*:?-{3,}:?\s*)+\|$")]
    private static partial Regex SeparatorRowRegex();
}
