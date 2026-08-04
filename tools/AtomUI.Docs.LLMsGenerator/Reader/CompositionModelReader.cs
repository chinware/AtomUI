using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using AtomUI.Docs.LLMsGenerator.Catalog;

namespace AtomUI.Docs.LLMsGenerator.Reader;

public static partial class CompositionModelReader
{
    private const int MaxThemeTemplateRoots = 4;
    private const int MaxTemplateTreeDepth = 8;
    private const int MaxTableRows = 36;

    private static readonly HashSet<string> StructuralElementNames = new(StringComparer.Ordinal)
    {
        "ItemsPresenter",
        "ContentPresenter",
        "SelectingItemsControl",
        "ItemsRepeater",
        "Popup",
        "PopupHost",
        "FlyoutPresenter"
    };

    private static readonly string[] NotableTypeSuffixes =
    [
        "Adorner",
        "Container",
        "Presenter",
        "Indicator",
        "MotionActor",
        "Panel",
        "Button",
        "Item",
        "Host",
        "Popup"
    ];

    public static string Read(
        string repositoryRoot,
        ControlDocumentInfo control,
        string displayName,
        string sourceIndex)
    {
        var themeFiles = FindThemeFiles(repositoryRoot, control, displayName, sourceIndex);
        if (themeFiles.Count == 0)
        {
            return string.Empty;
        }

        var sourceDirectories = FindSourceDirectories(control, displayName, sourceIndex);
        var typeAccessibilities = ReadTypeAccessibilities(sourceDirectories);
        var publicControlNames = BuildPublicControlNames(control, displayName, typeAccessibilities);

        var themes = themeFiles.SelectMany(path => ReadThemes(
                                    repositoryRoot,
                                    control,
                                    displayName,
                                    path,
                                    typeAccessibilities,
                                    publicControlNames))
                               .ToArray();
        if (themes.Length == 0)
        {
            return string.Empty;
        }

        var nodes = BuildNodes(displayName, themes);
        var hasCompositionLayer = nodes.Any(node =>
            node.Stability is "internal-observable" ||
            node.Kind.Contains("adorner", StringComparison.OrdinalIgnoreCase) ||
            node.Kind.Contains("container", StringComparison.OrdinalIgnoreCase) ||
            node.Kind.Contains("motion", StringComparison.OrdinalIgnoreCase) ||
            node.Name.Contains("Presenter", StringComparison.Ordinal) ||
            node.Name.Contains("Indicator", StringComparison.Ordinal) ||
            node.Name.StartsWith("PART_", StringComparison.Ordinal));

        if (!hasCompositionLayer)
        {
            return string.Empty;
        }

        return FormatCompositionModel(displayName, themes, nodes);
    }

    private static IReadOnlyList<string> FindThemeFiles(
        string repositoryRoot,
        ControlDocumentInfo control,
        string displayName,
        string sourceIndex)
    {
        var files = new List<string>();
        var themeDirectories = new List<string>();

        foreach (Match match in SourceThemePathRegex().Matches(sourceIndex))
        {
            var path = ResolvePath(repositoryRoot, match.Value);
            if (!File.Exists(path))
            {
                continue;
            }

            files.Add(path);
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                themeDirectories.Add(directory);
            }
        }

        foreach (var sourceDirectory in FindSourceDirectories(control, displayName, sourceIndex))
        {
            var themesDirectory = Path.Combine(sourceDirectory, "Themes");
            if (Directory.Exists(themesDirectory))
            {
                themeDirectories.Add(themesDirectory);
            }
        }

        foreach (var directory in themeDirectories.Distinct(StringComparer.Ordinal))
        {
            files.AddRange(Directory.GetFiles(directory, "*.axaml", SearchOption.TopDirectoryOnly));
        }

        return files.Where(path => path.EndsWith("Theme.axaml", StringComparison.Ordinal) ||
                                   path.EndsWith("Themes.axaml", StringComparison.Ordinal))
                    .Distinct(StringComparer.Ordinal)
                    .Order(StringComparer.Ordinal)
                    .ToArray();
    }

    private static IReadOnlyList<string> FindSourceDirectories(
        ControlDocumentInfo control,
        string displayName,
        string sourceIndex)
    {
        var directories = new List<string>();
        foreach (Match match in SourceCodePathRegex().Matches(sourceIndex))
        {
            var path = match.Value;
            var directory = Path.GetDirectoryName(path);
            if (string.IsNullOrWhiteSpace(directory))
            {
                continue;
            }

            foreach (var sourceRoot in control.SourceRoots)
            {
                var sourceRootName = Path.GetFileName(sourceRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                var sourceRootIndex = path.IndexOf(sourceRootName, StringComparison.Ordinal);
                if (sourceRootIndex < 0)
                {
                    continue;
                }

                var relativeFromRoot = path[(sourceRootIndex + sourceRootName.Length)..].TrimStart('/', '\\');
                var candidate = Path.Combine(sourceRoot, Path.GetDirectoryName(relativeFromRoot) ?? string.Empty);
                if (Directory.Exists(candidate))
                {
                    directories.Add(candidate);
                }
            }
        }

        var expectedNames = BuildExpectedSourceDirectoryNames(control, displayName);
        foreach (var sourceRoot in control.SourceRoots.Where(Directory.Exists))
        {
            foreach (var directory in Directory.GetDirectories(sourceRoot, "*", SearchOption.AllDirectories))
            {
                if (expectedNames.Contains(Path.GetFileName(directory)) &&
                    Directory.Exists(Path.Combine(directory, "Themes")))
                {
                    directories.Add(directory);
                }
            }
        }

        return directories.Distinct(StringComparer.Ordinal)
                          .Order(StringComparer.Ordinal)
                          .ToArray();
    }

    private static IReadOnlySet<string> BuildExpectedSourceDirectoryNames(
        ControlDocumentInfo control,
        string displayName)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ToPascalName(control.Name),
            control.Name.Replace("-", string.Empty, StringComparison.Ordinal),
            displayName.Replace(" ", string.Empty, StringComparison.Ordinal)
        };

        if (displayName.Contains(' ', StringComparison.Ordinal))
        {
            names.Add(ToPascalName(displayName.Replace(" ", "-", StringComparison.Ordinal)));
        }

        return names;
    }

    private static IReadOnlyDictionary<string, string> ReadTypeAccessibilities(IReadOnlyList<string> sourceDirectories)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var directory in sourceDirectories)
        {
            foreach (var file in Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories).Order(StringComparer.Ordinal))
            {
                var text = File.ReadAllText(file);
                foreach (Match match in TypeDeclarationRegex().Matches(text))
                {
                    var accessibility = match.Groups["access"].Success ? match.Groups["access"].Value : "internal";
                    var typeName = match.Groups["name"].Value;
                    if (!result.ContainsKey(typeName))
                    {
                        result[typeName] = accessibility;
                    }
                }
            }
        }

        return result;
    }

    private static IReadOnlySet<string> BuildPublicControlNames(
        ControlDocumentInfo control,
        string displayName,
        IReadOnlyDictionary<string, string> typeAccessibilities)
    {
        var names = new HashSet<string>(StringComparer.Ordinal)
        {
            displayName,
            displayName.Replace(" ", string.Empty, StringComparison.Ordinal),
            ToPascalName(control.Name),
            control.Name.Replace("-", string.Empty, StringComparison.Ordinal)
        };

        foreach (var pair in typeAccessibilities)
        {
            if (pair.Value == "public" && !IsInternalCompositionName(pair.Key))
            {
                names.Add(pair.Key);
            }
        }

        return names;
    }

    private static IReadOnlyList<ThemeModel> ReadThemes(
        string repositoryRoot,
        ControlDocumentInfo control,
        string displayName,
        string themePath,
        IReadOnlyDictionary<string, string> typeAccessibilities,
        IReadOnlySet<string> publicControlNames)
    {
        var text = File.ReadAllText(themePath);
        XDocument document;
        try
        {
            document = XDocument.Parse(text, LoadOptions.PreserveWhitespace);
        }
        catch (XmlException)
        {
            return [];
        }

        var result = new List<ThemeModel>();
        foreach (var theme in document.Descendants().Where(element => element.Name.LocalName == "ControlTheme"))
        {
            var targetType = ReadTargetType(theme);
            if (string.IsNullOrWhiteSpace(targetType))
            {
                continue;
            }

            if (!ShouldIncludeThemeTarget(targetType, control, displayName, typeAccessibilities))
            {
                continue;
            }

            var model = new ThemeModel(
                TargetType: targetType,
                SourceFileName: Path.GetFileName(themePath),
                SourcePath: ToRelativePath(repositoryRoot, themePath),
                Kind: GetTargetKind(targetType),
                Stability: GetTargetStability(targetType, typeAccessibilities, publicControlNames),
                PublicApis: ReadTemplateBindings(theme));

            var templateRoots = theme.Descendants()
                                     .Where(element => element.Name.LocalName == "ControlTemplate")
                                     .SelectMany(element => element.Elements())
                                     .Where(IsVisualNode)
                                     .ToArray();

            var templateTrees = new List<TemplateTreeNode>();
            foreach (var root in templateRoots)
            {
                templateTrees.Add(BuildTemplateTree(root, model, typeAccessibilities, publicControlNames, depth: 0));
            }

            model.TemplateRoots.AddRange(templateTrees.Take(MaxThemeTemplateRoots));
            result.Add(model);
        }

        return result;
    }

    private static bool ShouldIncludeThemeTarget(
        string targetType,
        ControlDocumentInfo control,
        string displayName,
        IReadOnlyDictionary<string, string> typeAccessibilities)
    {
        if (IsCurrentControlTarget(targetType, control, displayName))
        {
            return true;
        }

        if (typeAccessibilities.TryGetValue(targetType, out var access) && access == "internal")
        {
            return true;
        }

        if (IsInternalCompositionName(targetType))
        {
            return true;
        }

        if (targetType.EndsWith("Item", StringComparison.Ordinal))
        {
            return true;
        }

        return targetType.StartsWith(displayName.Replace(" ", string.Empty, StringComparison.Ordinal), StringComparison.Ordinal);
    }

    private static bool IsCurrentControlTarget(string targetType, ControlDocumentInfo control, string displayName)
    {
        var expectedNames = new HashSet<string>(StringComparer.Ordinal)
        {
            displayName,
            displayName.Replace(" ", string.Empty, StringComparison.Ordinal),
            ToPascalName(control.Name),
            control.Name.Replace("-", string.Empty, StringComparison.Ordinal)
        };

        return expectedNames.Contains(targetType);
    }

    private static IReadOnlyList<CompositionNode> BuildNodes(string displayName, IReadOnlyList<ThemeModel> themes)
    {
        var nodes = new List<CompositionNode>
        {
            new(
                Name: displayName,
                Kind: "public control",
                Source: "源文档 + public API",
                LifecycleOwner: "用户代码 / 控件宿主",
                PublicApis: "public API",
                Stability: "public",
                AgentBoundary: "用户可直接使用 public 控件；可作为示例和 API 入口。")
        };

        foreach (var theme in themes)
        {
            nodes.Add(new CompositionNode(
                Name: theme.TargetType,
                Kind: theme.Kind,
                Source: theme.SourceFileName,
                LifecycleOwner: theme.Stability == "public" ? "用户代码 / 控件宿主" : displayName,
                PublicApis: FormatPublicApis(theme.PublicApis),
                Stability: theme.Stability,
                AgentBoundary: GetAgentBoundary(theme.Stability)));

            nodes.AddRange(FlattenTemplateTree(theme.TemplateRoots));
        }

        return DeduplicateNodes(nodes).Take(MaxTableRows).ToArray();
    }

    private static TemplateTreeNode BuildTemplateTree(
        XElement element,
        ThemeModel theme,
        IReadOnlyDictionary<string, string> typeAccessibilities,
        IReadOnlySet<string> publicControlNames,
        int depth)
    {
        var elementType = element.Name.LocalName;
        var name = ReadNameAttribute(element);
        var publicApis = FormatPublicApis(ReadTemplateBindings(element));
        var stability = GetTemplateNodeStability(elementType, name, typeAccessibilities, publicControlNames);
        var node = new TemplateTreeNode(
            ElementType: elementType,
            Name: name,
            Kind: $"template node ({elementType})",
            Source: theme.SourceFileName,
            LifecycleOwner: theme.TargetType,
            PublicApis: publicApis,
            Stability: stability,
            AgentBoundary: GetAgentBoundary(stability));

        if (depth >= MaxTemplateTreeDepth)
        {
            return node;
        }

        foreach (var child in element.Elements().Where(IsVisualNode))
        {
            node.Children.Add(BuildTemplateTree(child, theme, typeAccessibilities, publicControlNames, depth + 1));
        }

        return node;
    }

    private static IReadOnlyList<CompositionNode> FlattenTemplateTree(IEnumerable<TemplateTreeNode> roots)
    {
        var nodes = new List<CompositionNode>();
        foreach (var root in roots)
        {
            AddFlattenedTemplateNode(nodes, root);
        }

        return nodes;
    }

    private static void AddFlattenedTemplateNode(List<CompositionNode> nodes, TemplateTreeNode node)
    {
        if (ShouldAddTemplateNodeToTable(node))
        {
            nodes.Add(new CompositionNode(
                Name: string.IsNullOrWhiteSpace(node.Name) ? node.ElementType : node.Name,
                Kind: node.Kind,
                Source: node.Source,
                LifecycleOwner: node.LifecycleOwner,
                PublicApis: node.PublicApis,
                Stability: node.Stability,
                AgentBoundary: node.AgentBoundary));
        }

        foreach (var child in node.Children)
        {
            AddFlattenedTemplateNode(nodes, child);
        }
    }

    private static IReadOnlyList<CompositionNode> DeduplicateNodes(IEnumerable<CompositionNode> nodes)
    {
        var result = new List<CompositionNode>();
        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var node in nodes)
        {
            var key = $"{node.Name}|{node.Kind}|{node.Source}|{node.LifecycleOwner}";
            if (keys.Add(key))
            {
                result.Add(node);
            }
        }

        return result;
    }

    private static string FormatCompositionModel(
        string displayName,
        IReadOnlyList<ThemeModel> themes,
        IReadOnlyList<CompositionNode> nodes)
    {
        var builder = new StringBuilder();
        builder.AppendLine("该章节由控件 `Themes/` 文件夹中的真实主题文件生成，用于说明 public 控件与内部协作对象之间的运行时结构。内部节点只用于理解和维护，不应指导用户代码直接依赖。");
        builder.AppendLine();
        builder.AppendLine("### 控件角色图");
        builder.AppendLine();
        builder.AppendLine("```text");
        builder.AppendLine(displayName);
        foreach (var theme in themes)
        {
            builder.AppendLine($"  -> {theme.TargetType} ({theme.Kind}, {theme.SourceFileName})");
            foreach (var child in theme.TemplateRoots)
            {
                AppendTemplateTree(builder, child, depth: 1);
            }
        }

        builder.AppendLine("```");
        builder.AppendLine();
        builder.AppendLine("### 协作节点");
        builder.AppendLine();
        builder.AppendLine("| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |");
        builder.AppendLine("| --- | --- | --- | --- | --- | --- | --- |");
        foreach (var node in nodes)
        {
            builder.AppendLine($"| `{EscapeTableCell(node.Name)}` | {EscapeTableCell(node.Kind)} | `{EscapeTableCell(node.Source)}` | {EscapeTableCell(node.LifecycleOwner)} | {EscapeTableCell(node.PublicApis)} | {EscapeTableCell(node.Stability)} | {EscapeTableCell(node.AgentBoundary)} |");
        }

        return builder.ToString().TrimEnd();
    }

    private static void AppendTemplateTree(StringBuilder builder, TemplateTreeNode node, int depth)
    {
        var indent = new string(' ', 2 + depth * 3);
        builder.AppendLine($"{indent}-> {FormatTemplateTreeNodeName(node)} ({node.Stability})");
        foreach (var child in node.Children)
        {
            AppendTemplateTree(builder, child, depth + 1);
        }
    }

    private static string FormatTemplateTreeNodeName(TemplateTreeNode node)
    {
        return string.IsNullOrWhiteSpace(node.Name)
            ? node.ElementType
            : $"{node.ElementType}#{node.Name}";
    }

    private static string ReadTargetType(XElement theme)
    {
        var target = theme.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "TargetType")?.Value;
        if (!string.IsNullOrWhiteSpace(target))
        {
            return ReadTypeName(target);
        }

        var key = theme.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "Key")?.Value;
        return string.IsNullOrWhiteSpace(key) ? string.Empty : ReadTypeName(key);
    }

    private static string ReadTypeName(string value)
    {
        var matches = TypeNameInMarkupRegex().Matches(value);
        return matches.Count == 0 ? value.Trim() : matches[^1].Groups["name"].Value;
    }

    private static string ReadNameAttribute(XElement element)
    {
        return element.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == "Name")?.Value ?? string.Empty;
    }

    private static IReadOnlyList<string> ReadTemplateBindings(XElement element)
    {
        return TemplateBindingRegex().Matches(element.ToString(SaveOptions.DisableFormatting))
                                     .Select(match => match.Groups["name"].Value)
                                     .Distinct(StringComparer.Ordinal)
                                     .Order(StringComparer.Ordinal)
                                     .Take(6)
                                     .ToArray();
    }

    private static string FormatPublicApis(IReadOnlyList<string> publicApis)
    {
        return publicApis.Count == 0
            ? "主题状态 / visual state"
            : string.Join(", ", publicApis.Select(api => $"`{api}`"));
    }

    private static string GetTargetKind(string targetType)
    {
        if (targetType.EndsWith("Adorner", StringComparison.Ordinal))
        {
            return "internal adorner control theme";
        }

        if (targetType.EndsWith("Container", StringComparison.Ordinal))
        {
            return "internal container control theme";
        }

        if (targetType.EndsWith("Presenter", StringComparison.Ordinal))
        {
            return "presenter control theme";
        }

        if (targetType.EndsWith("Item", StringComparison.Ordinal))
        {
            return "item container control theme";
        }

        return "control theme";
    }

    private static string GetTargetStability(
        string targetType,
        IReadOnlyDictionary<string, string> typeAccessibilities,
        IReadOnlySet<string> publicControlNames)
    {
        if (typeAccessibilities.TryGetValue(targetType, out var access) && access == "internal")
        {
            return "internal-observable";
        }

        if (publicControlNames.Contains(targetType) && !IsInternalCompositionName(targetType))
        {
            return "public";
        }

        return IsInternalCompositionName(targetType) ? "internal-observable" : "template-stable";
    }

    private static string GetElementStability(
        string elementType,
        IReadOnlyDictionary<string, string> typeAccessibilities,
        IReadOnlySet<string> publicControlNames)
    {
        if (typeAccessibilities.TryGetValue(elementType, out var access) && access == "internal")
        {
            return "internal-observable";
        }

        if (IsInternalCompositionName(elementType))
        {
            return "internal-observable";
        }

        return publicControlNames.Contains(elementType) ? "public" : "template-stable";
    }

    private static string GetTemplateNodeStability(
        string elementType,
        string name,
        IReadOnlyDictionary<string, string> typeAccessibilities,
        IReadOnlySet<string> publicControlNames)
    {
        if (name.StartsWith("PART_", StringComparison.Ordinal))
        {
            return "template-stable";
        }

        return GetElementStability(elementType, typeAccessibilities, publicControlNames);
    }

    private static string GetAgentBoundary(string stability)
    {
        return stability switch
        {
            "public" => "用户可直接使用 public 控件；可作为示例和 API 入口。",
            "template-stable" => "用于主题维护；变更需同步主题、实现和 LLMS。",
            "internal-observable" => "用于理解结构和状态流，不应指导用户代码直接依赖。",
            _ => "仅用于理解实现边界，不应作为用户代码依赖。"
        };
    }

    private static bool ShouldAddTemplateNodeToTable(TemplateTreeNode node)
    {
        if (!string.IsNullOrWhiteSpace(node.Name))
        {
            return true;
        }

        if (StructuralElementNames.Contains(node.ElementType))
        {
            return true;
        }

        if (NotableTypeSuffixes.Any(suffix => node.ElementType.EndsWith(suffix, StringComparison.Ordinal)))
        {
            return true;
        }

        return false;
    }

    private static bool IsInternalCompositionName(string typeName)
    {
        return typeName.EndsWith("Adorner", StringComparison.Ordinal) ||
               typeName.EndsWith("Container", StringComparison.Ordinal) ||
               typeName.EndsWith("Presenter", StringComparison.Ordinal) ||
               typeName.EndsWith("MotionActor", StringComparison.Ordinal) ||
               typeName.EndsWith("Indicator", StringComparison.Ordinal) ||
               typeName.EndsWith("Host", StringComparison.Ordinal) ||
               typeName.EndsWith("Popup", StringComparison.Ordinal);
    }

    private static bool IsVisualNode(XElement element)
    {
        var localName = element.Name.LocalName;
        if (localName.Contains('.', StringComparison.Ordinal))
        {
            return false;
        }

        return localName is not "Binding" and not "MultiBinding" and not "Setter" and not "Style"
            and not "ControlTemplate" and not "ControlTheme" and not "Transitions"
            and not "ResourceDictionary" and not "ResourceInclude";
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

    private static string ToPascalName(string value)
    {
        return string.Concat(value.Split('-', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
    }

    private static string EscapeTableCell(string value)
    {
        return value.ReplaceLineEndings(" ")
                    .Replace("|", "\\|", StringComparison.Ordinal)
                    .Trim();
    }

    private sealed record ThemeModel(
        string TargetType,
        string SourceFileName,
        string SourcePath,
        string Kind,
        string Stability,
        IReadOnlyList<string> PublicApis)
    {
        public List<TemplateTreeNode> TemplateRoots { get; } = [];
    }

    private sealed record TemplateTreeNode(
        string ElementType,
        string Name,
        string Kind,
        string Source,
        string LifecycleOwner,
        string PublicApis,
        string Stability,
        string AgentBoundary)
    {
        public List<TemplateTreeNode> Children { get; } = [];
    }

    private sealed record CompositionNode(
        string Name,
        string Kind,
        string Source,
        string LifecycleOwner,
        string PublicApis,
        string Stability,
        string AgentBoundary);

    [GeneratedRegex(@"src/[^\s`]+/Themes/[^\s`]+\.axaml")]
    private static partial Regex SourceThemePathRegex();

    [GeneratedRegex(@"src/[^\s`]+\.cs")]
    private static partial Regex SourceCodePathRegex();

    [GeneratedRegex(@"(?m)^\s*(?<access>public|internal|protected|private)?\s*(?:(?:abstract|sealed|static|partial|readonly|ref|file)\s+)*(?:class|interface|enum|struct|record(?:\s+(?:class|struct))?)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)")]
    private static partial Regex TypeDeclarationRegex();

    [GeneratedRegex(@"(?:^|[:\s{])(?<name>[A-Z][A-Za-z0-9_]+)(?=[}\s,]|$)")]
    private static partial Regex TypeNameInMarkupRegex();

    [GeneratedRegex(@"TemplateBinding\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)")]
    private static partial Regex TemplateBindingRegex();
}
