using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AtomUI.Docs.LLMsGenerator.Catalog;

namespace AtomUI.Docs.LLMsGenerator.Reader;

public static partial class ControlThemeStructureReader
{
    private const int MaxTemplateDepth = 7;
    private const int MaxTemplateNodes = 36;

    public static string Read(
        string repositoryRoot,
        ControlDocumentInfo control,
        string displayName,
        string sourceIndex)
    {
        var candidates = FindThemeCandidates(repositoryRoot, control, displayName, sourceIndex);
        foreach (var candidate in candidates)
        {
            var structure = TryReadControlTemplate(repositoryRoot, candidate, control, displayName);
            if (!string.IsNullOrWhiteSpace(structure))
            {
                return structure;
            }
        }

        return string.Empty;
    }

    private static IReadOnlyList<string> FindThemeCandidates(
        string repositoryRoot,
        ControlDocumentInfo control,
        string displayName,
        string sourceIndex)
    {
        var candidates = new List<string>();

        foreach (Match match in SourceThemePathRegex().Matches(sourceIndex))
        {
            var path = Path.Combine(repositoryRoot, match.Value);
            if (File.Exists(path))
            {
                candidates.Add(path);
            }
        }

        var expectedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            $"{displayName}Theme.axaml",
            $"{ToPascalName(control.Name)}Theme.axaml",
            $"{control.Name.Replace("-", string.Empty, StringComparison.Ordinal)}Theme.axaml"
        };

        foreach (var sourceRoot in control.SourceRoots.Where(Directory.Exists))
        {
            foreach (var file in Directory.GetFiles(sourceRoot, "*Theme.axaml", SearchOption.AllDirectories))
            {
                if (expectedNames.Contains(Path.GetFileName(file)))
                {
                    candidates.Add(file);
                }
            }
        }

        return candidates.Distinct(StringComparer.Ordinal).ToArray();
    }

    private static string TryReadControlTemplate(
        string repositoryRoot,
        string themePath,
        ControlDocumentInfo control,
        string displayName)
    {
        var text = File.ReadAllText(themePath);
        if (!text.Contains("ControlTheme", StringComparison.Ordinal) ||
            !text.Contains("ControlTemplate", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        if (!TargetsControl(text, control, displayName))
        {
            return string.Empty;
        }

        XDocument document;
        try
        {
            document = XDocument.Parse(text, LoadOptions.PreserveWhitespace);
        }
        catch
        {
            return string.Empty;
        }

        var template = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "ControlTemplate");
        if (template is null)
        {
            return string.Empty;
        }

        var visualRoots = template.Elements().Where(IsVisualNode).ToArray();
        if (visualRoots.Length == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        builder.AppendLine($"来源：`{Path.GetRelativePath(repositoryRoot, themePath).Replace(Path.DirectorySeparatorChar, '/')}`");
        builder.AppendLine();
        builder.AppendLine("```xml");

        var remainingNodes = MaxTemplateNodes;
        foreach (var root in visualRoots)
        {
            AppendElement(builder, root, 0, ref remainingNodes);
            if (remainingNodes <= 0)
            {
                builder.AppendLine("    <!-- structure truncated -->");
                break;
            }
        }

        builder.AppendLine("```");
        return builder.ToString().TrimEnd();
    }

    private static bool TargetsControl(string text, ControlDocumentInfo control, string displayName)
    {
        var names = new[]
        {
            displayName,
            ToPascalName(control.Name),
            control.Name.Replace("-", string.Empty, StringComparison.Ordinal)
        };

        return names.Any(name =>
            text.Contains($"TargetType=\"atom:{name}\"", StringComparison.Ordinal) ||
            text.Contains($"TargetType=\"atomc:{name}\"", StringComparison.Ordinal) ||
            text.Contains($"TargetType=\"{name}\"", StringComparison.Ordinal) ||
            text.Contains($":{name}\"", StringComparison.Ordinal));
    }

    private static void AppendElement(StringBuilder builder, XElement element, int depth, ref int remainingNodes)
    {
        if (remainingNodes <= 0 || depth >= MaxTemplateDepth)
        {
            return;
        }

        remainingNodes--;
        var indent = new string(' ', depth * 4);
        var name = element.Name.LocalName;
        var nameAttribute = ReadNameAttribute(element);
        var children = element.Elements().Where(IsVisualNode).ToArray();

        if (children.Length == 0)
        {
            builder.AppendLine(string.IsNullOrWhiteSpace(nameAttribute)
                ? $"{indent}<{name} />"
                : $"{indent}<{name} Name=\"{EscapeAttribute(nameAttribute)}\" />");
            return;
        }

        builder.AppendLine(string.IsNullOrWhiteSpace(nameAttribute)
            ? $"{indent}<{name}>"
            : $"{indent}<{name} Name=\"{EscapeAttribute(nameAttribute)}\">");

        foreach (var child in children)
        {
            AppendElement(builder, child, depth + 1, ref remainingNodes);
            if (remainingNodes <= 0)
            {
                break;
            }
        }

        builder.AppendLine($"{indent}</{name}>");
    }

    private static string ReadNameAttribute(XElement element)
    {
        return element.Attributes().FirstOrDefault(attribute =>
                   attribute.Name.LocalName is "Name" or "x:Name")?.Value ??
               string.Empty;
    }

    private static bool IsVisualNode(XElement element)
    {
        var localName = element.Name.LocalName;
        if (localName.Contains('.', StringComparison.Ordinal))
        {
            return false;
        }

        return localName is not "Binding" and not "MultiBinding" and not "Setter" and not "Style"
            and not "ControlTemplate" and not "ControlTheme" and not "Transitions";
    }

    private static string EscapeAttribute(string value)
    {
        return value.Replace("\"", "&quot;", StringComparison.Ordinal);
    }

    private static string ToPascalName(string value)
    {
        return string.Concat(value.Split('-', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
    }

    [GeneratedRegex(@"src/[^\s`]+Theme\.axaml")]
    private static partial Regex SourceThemePathRegex();
}
