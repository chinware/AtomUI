using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace AtomUI.Docs.LLMsGenerator.Reader;

public static partial class GalleryExampleReader
{
    private const int MaxExamplesPerControl = 4;
    private const int MaxSnippetLineCount = 80;
    private const int MaxSnippetLength = 6_000;

    public static string ReadMarkdown(string repositoryRoot, string galleryPath)
    {
        if (string.IsNullOrWhiteSpace(galleryPath))
        {
            return "未找到对应 Gallery 目录；生成器只链接源文档，不发明示例。";
        }

        var absoluteGalleryPath = ResolvePath(repositoryRoot, galleryPath);
        if (!Directory.Exists(absoluteGalleryPath))
        {
            return $"Gallery 目录 `{galleryPath}` 当前不存在；请检查控件文档中的 Gallery 页面元数据。";
        }

        var examples = Directory.GetFiles(absoluteGalleryPath, "*ShowCase.axaml", SearchOption.AllDirectories)
                                .Where(path => path.Contains($"{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                                .Order(StringComparer.Ordinal)
                                .SelectMany(path => ReadExamplesFromShowCase(path, repositoryRoot, absoluteGalleryPath))
                                .Where(IsPublicDocumentationExample)
                                .Take(MaxExamplesPerControl)
                                .ToArray();

        if (examples.Length > 0)
        {
            return FormatExamples(examples);
        }

        var axamlFiles = Directory.GetFiles(absoluteGalleryPath, "*.axaml", SearchOption.AllDirectories)
                                  .Where(path => path.Contains($"{Path.DirectorySeparatorChar}Views{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                                  .Select(path => $"- `{ToRelativePath(repositoryRoot, path)}`")
                                  .Order(StringComparer.Ordinal)
                                  .Take(12)
                                  .ToArray();

        return axamlFiles.Length == 0
            ? $"Gallery 目录 `{galleryPath}` 未声明稳定 AXAML 示例。"
            : string.Join('\n', axamlFiles);
    }

    private static IReadOnlyList<GalleryExample> ReadExamplesFromShowCase(
        string showCasePath,
        string repositoryRoot,
        string galleryControlPath)
    {
        var sourceText = File.ReadAllText(showCasePath);
        XDocument document;
        try
        {
            document = XDocument.Parse(sourceText, LoadOptions.SetLineInfo);
        }
        catch (XmlException)
        {
            return [];
        }

        var localization = ReadLocalization(galleryControlPath);
        var lines = sourceText.ReplaceLineEndings("\n").Split('\n');
        var examples = new List<GalleryExample>();
        foreach (var panel in document.Descendants().Where(static element => element.Name.LocalName == "ShowCasePanel"))
        {
            var panelKey = ReadAttribute(panel, "Name");
            if (string.IsNullOrWhiteSpace(panelKey))
            {
                continue;
            }

            var itemIndex = 0;
            foreach (var item in panel.Elements().Where(static element => element.Name.LocalName == "ShowCaseItem"))
            {
                if (TryCreateExample(repositoryRoot, showCasePath, lines, localization, panelKey, itemIndex, item, out var example))
                {
                    examples.Add(example);
                }

                itemIndex++;
            }
        }

        return examples;
    }

    private static bool TryCreateExample(
        string repositoryRoot,
        string showCasePath,
        IReadOnlyList<string> sourceLines,
        IReadOnlyDictionary<string, string> localization,
        string panelKey,
        int itemIndex,
        XElement item,
        out GalleryExample example)
    {
        example = default!;
        var contentElements = GetSnippetContentElements(item).ToArray();
        if (contentElements.Length == 0)
        {
            return false;
        }

        var snippetText = ExtractSnippetText(sourceLines, contentElements);
        if (string.IsNullOrWhiteSpace(snippetText))
        {
            return false;
        }

        snippetText = ReplaceGalleryResources(snippetText, localization);
        var sourceKey = ReadAttribute(item, "SourceKey");
        var title = ResolveGalleryResource(ReadAttribute(item, "Title"), localization);
        if (string.IsNullOrWhiteSpace(title))
        {
            title = !string.IsNullOrWhiteSpace(sourceKey) ? sourceKey : $"示例 {itemIndex + 1}";
        }

        var startLine = GetLineNumber(contentElements[0]);
        var endLine = FindElementEndLine(sourceLines, contentElements[^1]);
        example = new GalleryExample(
            Title: title,
            SourceKey: sourceKey,
            PanelKey: panelKey,
            ItemIndex: itemIndex,
            SourceFilePath: ToRelativePath(repositoryRoot, showCasePath),
            StartLine: startLine,
            EndLine: endLine,
            Axaml: snippetText);
        return true;
    }

    private static bool IsPublicDocumentationExample(GalleryExample example)
    {
        if (string.IsNullOrWhiteSpace(example.Axaml))
        {
            return false;
        }

        if (example.Axaml.Length > MaxSnippetLength ||
            example.Axaml.Count(static character => character == '\n') + 1 > MaxSnippetLineCount)
        {
            return false;
        }

        if (ContainsBlockedTerm(example.Axaml) || ContainsBlockedTerm(example.Title))
        {
            return false;
        }

        return !example.Axaml.Contains("ShowCase", StringComparison.Ordinal) &&
               !example.Axaml.Contains("gallery:", StringComparison.Ordinal);
    }

    private static string FormatExamples(IReadOnlyList<GalleryExample> examples)
    {
        var builder = new StringBuilder();
        builder.AppendLine("以下示例来自 Gallery 源码查看使用的 `ShowCaseItem` 片段，并已按中文资源规范化。");
        for (var index = 0; index < examples.Count; index++)
        {
            var example = examples[index];
            builder.AppendLine();
            builder.AppendLine($"### {example.Title}");
            builder.AppendLine();
            builder.AppendLine($"来源：`{example.SourceFilePath}:{example.StartLine}`");
            if (!string.IsNullOrWhiteSpace(example.SourceKey))
            {
                builder.AppendLine();
                builder.AppendLine($"SourceKey：`{example.SourceKey}`");
            }
            else
            {
                builder.AppendLine();
                builder.AppendLine($"Gallery key：`{example.PanelKey}` / item `{example.ItemIndex}`");
            }

            builder.AppendLine();
            builder.AppendLine("```axaml");
            builder.AppendLine(example.Axaml.Trim());
            builder.AppendLine("```");
        }

        return builder.ToString().TrimEnd();
    }

    private static IEnumerable<XElement> GetSnippetContentElements(XElement item)
    {
        var deferredTemplate = item.Elements()
                                   .FirstOrDefault(static element =>
                                       element.Name.LocalName == "ShowCaseItem.DeferredContentTemplate");
        var dataTemplate = deferredTemplate?.Elements()
                                           .FirstOrDefault(static element => element.Name.LocalName == "DataTemplate");
        if (dataTemplate is not null)
        {
            return dataTemplate.Elements();
        }

        return item.Elements()
                   .Where(static element => !element.Name.LocalName.StartsWith("ShowCaseItem.", StringComparison.Ordinal));
    }

    private static string ExtractSnippetText(IReadOnlyList<string> sourceLines, IReadOnlyList<XElement> elements)
    {
        var firstLine = GetLineNumber(elements[0]);
        var lastLine = FindElementEndLine(sourceLines, elements[^1]);
        if (firstLine <= 0 || lastLine < firstLine)
        {
            return string.Empty;
        }

        var lines = new List<string>();
        for (var index = firstLine - 1; index <= lastLine - 1 && index < sourceLines.Count; index++)
        {
            lines.Add(sourceLines[index]);
        }

        return TrimCommonIndent(lines);
    }

    private static int FindElementEndLine(IReadOnlyList<string> sourceLines, XElement element)
    {
        var startLine = GetLineNumber(element);
        if (startLine <= 0 || startLine > sourceLines.Count)
        {
            return startLine;
        }

        var localName = element.Name.LocalName;
        var depth = 0;
        for (var index = startLine - 1; index < sourceLines.Count; index++)
        {
            var lineText = sourceLines[index];
            depth += CountElementOpenings(lineText, localName);
            depth -= CountSelfClosings(lineText, localName);
            depth -= CountElementClosings(lineText, localName);
            if (depth <= 0)
            {
                return index + 1;
            }
        }

        return startLine;
    }

    private static int CountElementOpenings(string lineText, string localName)
    {
        return CountTagOccurrences(lineText, localName, requireSlashBefore: false);
    }

    private static int CountElementClosings(string lineText, string localName)
    {
        return CountTagOccurrences(lineText, localName, requireSlashBefore: true);
    }

    private static int CountSelfClosings(string lineText, string localName)
    {
        var count = 0;
        var searchStart = 0;
        while (true)
        {
            var openIndex = FindTagStart(lineText, localName, searchStart, requireSlashBefore: false);
            if (openIndex < 0)
            {
                break;
            }

            var tagEnd = lineText.IndexOf('>', openIndex);
            if (tagEnd < 0)
            {
                break;
            }

            if (tagEnd > 0 && lineText[tagEnd - 1] == '/')
            {
                count++;
            }

            searchStart = tagEnd + 1;
        }

        return count;
    }

    private static int CountTagOccurrences(string lineText, string localName, bool requireSlashBefore)
    {
        var count = 0;
        var searchStart = 0;
        while (true)
        {
            var index = FindTagStart(lineText, localName, searchStart, requireSlashBefore);
            if (index < 0)
            {
                break;
            }

            count++;
            searchStart = index + 1;
        }

        return count;
    }

    private static int FindTagStart(string lineText, string localName, int searchStart, bool requireSlashBefore)
    {
        for (var index = lineText.IndexOf('<', searchStart); index >= 0; index = lineText.IndexOf('<', index + 1))
        {
            var cursor = index + 1;
            var isClosing = cursor < lineText.Length && lineText[cursor] == '/';
            if (isClosing)
            {
                cursor++;
            }

            if (requireSlashBefore != isClosing)
            {
                continue;
            }

            if (MatchesElementName(lineText, cursor, localName))
            {
                return index;
            }
        }

        return -1;
    }

    private static bool MatchesElementName(string lineText, int cursor, string localName)
    {
        var nameStart = cursor;
        var scan = cursor;
        while (scan < lineText.Length && (char.IsLetterOrDigit(lineText[scan]) || lineText[scan] == '_' || lineText[scan] == '.'))
        {
            scan++;
        }

        if (scan < lineText.Length && lineText[scan] == ':')
        {
            nameStart = scan + 1;
            scan = nameStart;
            while (scan < lineText.Length && (char.IsLetterOrDigit(lineText[scan]) || lineText[scan] == '_' || lineText[scan] == '.'))
            {
                scan++;
            }
        }

        if (string.CompareOrdinal(lineText, nameStart, localName, 0, localName.Length) != 0)
        {
            return false;
        }

        var afterName = nameStart + localName.Length;
        if (afterName != scan)
        {
            return false;
        }

        if (afterName >= lineText.Length)
        {
            return true;
        }

        var next = lineText[afterName];
        return char.IsWhiteSpace(next) || next == '>' || next == '/';
    }

    private static string TrimCommonIndent(IReadOnlyList<string> rawLines)
    {
        var start = 0;
        var end = rawLines.Count - 1;
        while (start <= end && string.IsNullOrWhiteSpace(rawLines[start]))
        {
            start++;
        }

        while (end >= start && string.IsNullOrWhiteSpace(rawLines[end]))
        {
            end--;
        }

        if (start > end)
        {
            return string.Empty;
        }

        var minIndent = rawLines.Skip(start)
                                .Take(end - start + 1)
                                .Where(static line => !string.IsNullOrWhiteSpace(line))
                                .Select(CountIndent)
                                .DefaultIfEmpty(0)
                                .Min();

        var builder = new StringBuilder();
        for (var index = start; index <= end; index++)
        {
            var line = rawLines[index];
            builder.Append(line.Length >= minIndent ? line[minIndent..] : line.TrimStart());
            if (index < end)
            {
                builder.AppendLine();
            }
        }

        return builder.ToString();
    }

    private static IReadOnlyDictionary<string, string> ReadLocalization(string galleryControlPath)
    {
        var localizationPath = Path.Combine(galleryControlPath, "Localization", "zh_CN.cs");
        if (!File.Exists(localizationPath))
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var source = File.ReadAllText(localizationPath);
        foreach (Match match in ConstStringRegex().Matches(source))
        {
            result[match.Groups["key"].Value] = DecodeCSharpStringLiteral(match.Groups["value"].Value);
        }

        return result;
    }

    private static string ReplaceGalleryResources(string text, IReadOnlyDictionary<string, string> localization)
    {
        return GalleryResourceRegex().Replace(text, match =>
        {
            var key = match.Groups["key"].Value;
            return localization.TryGetValue(key, out var value)
                ? EscapeXmlAttribute(SanitizeValue(value))
                : match.Value;
        });
    }

    private static string ResolveGalleryResource(string? text, IReadOnlyDictionary<string, string> localization)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var match = GalleryResourceRegex().Match(text);
        if (match.Success && localization.TryGetValue(match.Groups["key"].Value, out var value))
        {
            return SanitizeValue(value);
        }

        return text;
    }

    private static string DecodeCSharpStringLiteral(string value)
    {
        return Regex.Unescape(value);
    }

    private static string SanitizeValue(string value)
    {
        return BlockedTermRegex().Replace(value, "AtomUI");
    }

    private static bool ContainsBlockedTerm(string value)
    {
        return BlockedTermRegex().IsMatch(value);
    }

    private static string EscapeXmlAttribute(string value)
    {
        return value.Replace("&", "&amp;", StringComparison.Ordinal)
                    .Replace("\"", "&quot;", StringComparison.Ordinal)
                    .Replace("<", "&lt;", StringComparison.Ordinal)
                    .Replace(">", "&gt;", StringComparison.Ordinal);
    }

    private static string? ReadAttribute(XElement element, string localName)
    {
        return element.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == localName)?.Value;
    }

    private static int GetLineNumber(XObject node)
    {
        return node is IXmlLineInfo lineInfo && lineInfo.HasLineInfo()
            ? lineInfo.LineNumber
            : 0;
    }

    private static int CountIndent(string line)
    {
        var count = 0;
        while (count < line.Length && char.IsWhiteSpace(line[count]))
        {
            count++;
        }

        return count;
    }

    private static string ResolvePath(string repositoryRoot, string path)
    {
        return Path.IsPathFullyQualified(path)
            ? Path.GetFullPath(path)
            : Path.GetFullPath(Path.Combine(repositoryRoot, path));
    }

    private static string ToRelativePath(string repositoryRoot, string path)
    {
        return Path.GetRelativePath(repositoryRoot, path).Replace(Path.DirectorySeparatorChar, '/');
    }

    [GeneratedRegex(@"public\s+const\s+string\s+(?<key>[A-Za-z0-9_]+)\s*=\s*""(?<value>(?:\\.|[^""\\])*)""\s*;")]
    private static partial Regex ConstStringRegex();

    [GeneratedRegex(@"\{gallery:[A-Za-z0-9_]+LangResource\s+(?<key>[A-Za-z0-9_]+)\}")]
    private static partial Regex GalleryResourceRegex();

    [GeneratedRegex("Ant Design|ant design|ant\\.design|antd", RegexOptions.IgnoreCase)]
    private static partial Regex BlockedTermRegex();

    private sealed record GalleryExample(
        string Title,
        string? SourceKey,
        string PanelKey,
        int ItemIndex,
        string SourceFilePath,
        int StartLine,
        int EndLine,
        string Axaml);
}
