using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Toolkits.GalleryBase.Generator.ShowCaseSource;

[Generator]
public sealed class ShowCaseCodeSnippetCatalogGenerator : IIncrementalGenerator
{
    private const string CatalogFileName = "ShowCaseCodeSnippetCatalog.g.cs";
    private const string DefaultNamespace = "Gallery.Generated";

#pragma warning disable RS2008
    private static readonly DiagnosticDescriptor PanelMissingNameDescriptor = new(
        "ATOMUIGEN101",
        "ShowCasePanel source key is missing",
        "ShowCasePanel in '{0}' must set Name to generate default source snippets",
        "Generator",
        DiagnosticSeverity.Warning,
        true);
#pragma warning restore RS2008

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var sourceFiles = context.AdditionalTextsProvider
                                 .Where(static file => file.Path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase))
                                 .Select(static (file, cancellationToken) =>
                                 {
                                     var text = file.GetText(cancellationToken);
                                     return new AdditionalSourceFile(file.Path, text?.ToString() ?? string.Empty);
                                 })
                                 .Collect();

        var optionsProvider = context.AnalyzerConfigOptionsProvider.Select(static (options, _) =>
            new GeneratorOptions(
                GetGlobalOption(options, "build_property.RootNamespace"),
                GetGlobalOption(options, "build_property.ProjectDir")));

        context.RegisterSourceOutput(sourceFiles.Combine(optionsProvider), static (context, combined) =>
        {
            var groups = new List<GeneratedSnippetGroup>();
            foreach (var file in combined.Left)
            {
                if (string.IsNullOrWhiteSpace(file.Text))
                {
                    continue;
                }

                ExtractFile(file, combined.Right, context, groups);
            }

            var rootNamespace = combined.Right.RootNamespace;
            var outputNamespace = string.IsNullOrWhiteSpace(rootNamespace)
                ? DefaultNamespace
                : $"{rootNamespace}.Generated";

            context.AddSource(CatalogFileName, SourceText.From(WriteCatalog(outputNamespace, groups), Encoding.UTF8));
        });
    }

    private static void ExtractFile(AdditionalSourceFile file,
                                    GeneratorOptions options,
                                    SourceProductionContext context,
                                    List<GeneratedSnippetGroup> groups)
    {
        var sourceText = SourceText.From(file.Text, Encoding.UTF8);
        XDocument document;
        try
        {
            document = XDocument.Parse(
                file.Text,
                LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
        }
        catch (XmlException)
        {
            return;
        }

        var root = document.Root;
        if (root is null)
        {
            return;
        }

        XNamespace xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
        var viewTypeName = root.Attribute(xamlNamespace + "Class")?.Value;
        if (string.IsNullOrWhiteSpace(viewTypeName))
        {
            return;
        }

        foreach (var panel in root.Descendants().Where(static element => element.Name.LocalName == "ShowCasePanel"))
        {
            var panelKey = panel.Attribute("Name")?.Value
                           ?? panel.Attribute(xamlNamespace + "Name")?.Value;
            if (string.IsNullOrWhiteSpace(panelKey))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    PanelMissingNameDescriptor,
                    CreateLocation(file.Path, sourceText, panel),
                    MakeRelativePath(file.Path, options.ProjectDir)));
                continue;
            }

            var itemIndex = 0;
            foreach (var item in panel.Elements().Where(static element => element.Name.LocalName == "ShowCaseItem"))
            {
                if (TryCreateGroup(file.Path, sourceText, viewTypeName!, panelKey!, itemIndex, item, options, out var group))
                {
                    groups.Add(group);
                }

                itemIndex++;
            }
        }
    }

    private static bool TryCreateGroup(string path,
                                       SourceText sourceText,
                                       string viewTypeName,
                                       string panelKey,
                                       int itemIndex,
                                       XElement item,
                                       GeneratorOptions options,
                                       out GeneratedSnippetGroup group)
    {
        group = default;
        var sourceKey = item.Attribute("SourceKey")?.Value;
        var title = item.Attribute("Title")?.Value;
        var contentElements = GetSnippetContentElements(item).ToArray();
        if (contentElements.Length == 0)
        {
            return false;
        }

        var snippetText = ExtractSnippetText(sourceText, contentElements);
        if (string.IsNullOrWhiteSpace(snippetText))
        {
            return false;
        }

        var startLine = GetLineNumber(contentElements[0]);
        var endLine = startLine + snippetText.Count(static ch => ch == '\n');
        var relativePath = MakeRelativePath(path, options.ProjectDir);
        group = new GeneratedSnippetGroup(
            viewTypeName,
            panelKey,
            itemIndex,
            string.IsNullOrWhiteSpace(sourceKey) ? null : sourceKey,
            string.IsNullOrWhiteSpace(title) ? "Source" : title!,
            new GeneratedSnippet("AXAML", "axaml", snippetText, relativePath, startLine, endLine));
        return true;
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

    private static string ExtractSnippetText(SourceText sourceText, IReadOnlyList<XElement> elements)
    {
        var firstLine = GetLineNumber(elements[0]);
        var lastLine = FindElementEndLine(sourceText, elements[elements.Count - 1]);
        if (firstLine <= 0 || lastLine < firstLine)
        {
            return string.Empty;
        }

        var lines = new List<string>();
        for (var index = firstLine - 1; index <= lastLine - 1 && index < sourceText.Lines.Count; index++)
        {
            lines.Add(sourceText.Lines[index].ToString());
        }

        return TrimCommonIndent(lines);
    }

    private static int FindElementEndLine(SourceText sourceText, XElement element)
    {
        var startLine = GetLineNumber(element);
        if (startLine <= 0 || startLine > sourceText.Lines.Count)
        {
            return startLine;
        }

        var startText = sourceText.Lines[startLine - 1].ToString();
        if (startText.Contains("/>"))
        {
            return startLine;
        }

        var closingTag = "</" + element.Name.LocalName;
        for (var index = startLine - 1; index < sourceText.Lines.Count; index++)
        {
            if (sourceText.Lines[index].ToString().Contains(closingTag))
            {
                return index + 1;
            }
        }

        return startLine;
    }

    private static int GetLineNumber(XObject node)
    {
        return node is IXmlLineInfo lineInfo && lineInfo.HasLineInfo()
            ? lineInfo.LineNumber
            : 0;
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
            builder.Append(line.Length >= minIndent ? line.Substring(minIndent) : line.TrimStart());
            if (index < end)
            {
                builder.AppendLine();
            }
        }

        return builder.ToString();
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

    private static Location CreateLocation(string path, SourceText sourceText, XElement element)
    {
        var lineNumber = GetLineNumber(element);
        if (lineNumber <= 0 || lineNumber > sourceText.Lines.Count)
        {
            return Location.None;
        }

        var line = sourceText.Lines[lineNumber - 1];
        return Location.Create(path, line.Span, sourceText.Lines.GetLinePositionSpan(line.Span));
    }

    private static string GetGlobalOption(AnalyzerConfigOptionsProvider options, string key)
    {
        return options.GlobalOptions.TryGetValue(key, out var value)
            ? value
            : string.Empty;
    }

    private static string MakeRelativePath(string path, string projectDir)
    {
        var normalizedPath = path.Replace('\\', '/');
        var normalizedProjectDir = projectDir.Replace('\\', '/');
        if (!string.IsNullOrWhiteSpace(normalizedProjectDir))
        {
            if (!normalizedProjectDir.EndsWith("/", StringComparison.Ordinal))
            {
                normalizedProjectDir += "/";
            }

            if (normalizedPath.StartsWith(normalizedProjectDir, StringComparison.OrdinalIgnoreCase))
            {
                return normalizedPath.Substring(normalizedProjectDir.Length);
            }
        }

        return normalizedPath;
    }

    private static string WriteCatalog(string outputNamespace, IReadOnlyList<GeneratedSnippetGroup> groups)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine();
        builder.Append("namespace ").Append(outputNamespace).AppendLine(";");
        builder.AppendLine();
        builder.AppendLine("internal static partial class ShowCaseCodeSnippetCatalog");
        builder.AppendLine("{");
        builder.AppendLine("    public static bool TryGetSnippetGroup(");
        builder.AppendLine("        string viewTypeName,");
        builder.AppendLine("        string panelKey,");
        builder.AppendLine("        int itemIndex,");
        builder.AppendLine("        string? sourceKey,");
        builder.AppendLine("        out global::AtomUI.Toolkits.GalleryBase.SourceCode.ShowCaseCodeSnippetGroup group)");
        builder.AppendLine("    {");

        foreach (var group in groups)
        {
            builder.Append("        if (viewTypeName == ")
                   .Append(CSharpSyntaxTreeString(group.ViewTypeName))
                   .Append(" && panelKey == ")
                   .Append(CSharpSyntaxTreeString(group.PanelKey))
                   .Append(" && itemIndex == ")
                   .Append(group.ItemIndex)
                   .Append(" && ");

            if (group.SourceKey is null)
            {
                builder.Append("string.IsNullOrEmpty(sourceKey)");
            }
            else
            {
                builder.Append("sourceKey == ").Append(CSharpSyntaxTreeString(group.SourceKey));
            }

            builder.AppendLine(")");
            builder.AppendLine("        {");
            builder.Append("            group = new global::AtomUI.Toolkits.GalleryBase.SourceCode.ShowCaseCodeSnippetGroup(")
                   .AppendLine();
            builder.Append("                Title: ").Append(CSharpSyntaxTreeString(group.Title)).AppendLine(",");
            builder.AppendLine("                Snippets: new global::AtomUI.Toolkits.GalleryBase.SourceCode.ShowCaseCodeSnippet[]");
            builder.AppendLine("                {");
            builder.Append("                    new global::AtomUI.Toolkits.GalleryBase.SourceCode.ShowCaseCodeSnippet(")
                   .Append("TabTitle: ")
                   .Append(CSharpSyntaxTreeString(group.Snippet.TabTitle))
                   .Append(", Language: ")
                   .Append(CSharpSyntaxTreeString(group.Snippet.Language))
                   .Append(", Text: ")
                   .Append(CSharpSyntaxTreeString(group.Snippet.Text))
                   .Append(", SourceFilePath: ")
                   .Append(CSharpSyntaxTreeString(group.Snippet.SourceFilePath))
                   .Append(", StartLine: ")
                   .Append(group.Snippet.StartLine)
                   .Append(", EndLine: ")
                   .Append(group.Snippet.EndLine)
                   .AppendLine(")");
            builder.AppendLine("                });");
            builder.AppendLine("            return true;");
            builder.AppendLine("        }");
        }

        builder.AppendLine();
        builder.AppendLine("        group = default!;");
        builder.AppendLine("        return false;");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string CSharpSyntaxTreeString(string? value)
    {
        return SymbolDisplay.FormatLiteral(value ?? string.Empty, true);
    }

    private readonly struct AdditionalSourceFile
    {
        public AdditionalSourceFile(string path, string text)
        {
            Path = path;
            Text = text;
        }

        public string Path { get; }
        public string Text { get; }
    }

    private readonly struct GeneratorOptions
    {
        public GeneratorOptions(string rootNamespace, string projectDir)
        {
            RootNamespace = rootNamespace;
            ProjectDir    = projectDir;
        }

        public string RootNamespace { get; }
        public string ProjectDir { get; }
    }

    private readonly struct GeneratedSnippetGroup
    {
        public GeneratedSnippetGroup(string viewTypeName,
                                     string panelKey,
                                     int itemIndex,
                                     string? sourceKey,
                                     string title,
                                     GeneratedSnippet snippet)
        {
            ViewTypeName = viewTypeName;
            PanelKey     = panelKey;
            ItemIndex    = itemIndex;
            SourceKey    = sourceKey;
            Title        = title;
            Snippet      = snippet;
        }

        public string ViewTypeName { get; }
        public string PanelKey { get; }
        public int ItemIndex { get; }
        public string? SourceKey { get; }
        public string Title { get; }
        public GeneratedSnippet Snippet { get; }
    }

    private readonly struct GeneratedSnippet
    {
        public GeneratedSnippet(string tabTitle,
                                string language,
                                string text,
                                string sourceFilePath,
                                int startLine,
                                int endLine)
        {
            TabTitle       = tabTitle;
            Language       = language;
            Text           = text;
            SourceFilePath = sourceFilePath;
            StartLine      = startLine;
            EndLine        = endLine;
        }

        public string TabTitle { get; }
        public string Language { get; }
        public string Text { get; }
        public string SourceFilePath { get; }
        public int StartLine { get; }
        public int EndLine { get; }
    }
}
