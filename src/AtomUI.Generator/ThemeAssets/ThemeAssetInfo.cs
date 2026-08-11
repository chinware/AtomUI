using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator;

internal sealed class ThemeAssetInfo
{
    private ThemeAssetInfo(
        string path,
        string assetPath,
        SourceText source,
        string fileName,
        string? controlCandidate,
        string? explicitUnit,
        IReadOnlyList<string> directoryCandidates,
        IReadOnlyList<ThemeAssetTargetTypeReference> targetTypes,
        IReadOnlyList<ThemeAssetElementTypeReference> elementTypes,
        IReadOnlyList<ThemeAssetSemanticThemeInfo> semanticThemes,
        IReadOnlyList<string> controlTokenFamilies,
        bool isResourceDictionary,
        string? controlThemeClassName,
        string? controlThemeTargetTypeName)
    {
        Path = path;
        AssetPath = assetPath;
        Source = source;
        FileName = fileName;
        ControlCandidate = controlCandidate;
        ExplicitUnit = explicitUnit;
        DirectoryCandidates = directoryCandidates;
        TargetTypes = targetTypes;
        ElementTypes = elementTypes;
        SemanticThemes = semanticThemes;
        ControlTokenFamilies = controlTokenFamilies;
        IsResourceDictionary = isResourceDictionary;
        ControlThemeClassName = controlThemeClassName;
        ControlThemeTargetTypeName = controlThemeTargetTypeName;
    }

    internal string Path { get; }
    internal string AssetPath { get; }
    internal SourceText Source { get; }
    internal string FileName { get; }
    internal string? ControlCandidate { get; }
    internal string? ExplicitUnit { get; }
    internal IReadOnlyList<string> DirectoryCandidates { get; }
    internal IReadOnlyList<ThemeAssetTargetTypeReference> TargetTypes { get; }
    internal IReadOnlyList<ThemeAssetElementTypeReference> ElementTypes { get; }
    internal IReadOnlyList<ThemeAssetSemanticThemeInfo> SemanticThemes { get; }
    internal IReadOnlyList<string> ControlTokenFamilies { get; }
    internal bool IsResourceDictionary { get; }
    internal string? ControlThemeClassName { get; }
    internal string? ControlThemeTargetTypeName { get; }
    internal bool IsDefaultTypedControlTheme =>
        ControlThemeClassName is not null &&
        ControlThemeTargetTypeName is not null &&
        !ControlThemeTargetTypeName.StartsWith("Abstract", StringComparison.Ordinal) &&
        !ControlThemeTargetTypeName.StartsWith("Base", StringComparison.Ordinal) &&
        string.Equals(FileName, ControlThemeTargetTypeName + "Theme", StringComparison.Ordinal);
    internal bool HasGeneratedResourceWrapper => IsResourceDictionary || IsDefaultTypedControlTheme;

    internal static ThemeAssetInfo Create(
        AdditionalText text,
        string? projectDirectory,
        string? link,
        string? explicitUnit,
        CancellationToken cancellationToken)
    {
        var source = text.GetText(cancellationToken) ?? SourceText.From(string.Empty);
        var assetPath = NormalizeAssetPath(text.Path, projectDirectory, link);
        var fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
        var targetTypes = new List<ThemeAssetTargetTypeReference>();
        var elementTypes = new HashSet<ThemeAssetElementTypeReference>();
        var semanticThemes = new List<ThemeAssetSemanticThemeInfo>();
        var nextSemanticTemplateIndex = 0;
        var controlTokenFamilies = new HashSet<string>(StringComparer.Ordinal);
        var isResourceDictionary = false;
        string? controlThemeClassName = null;
        string? controlThemeTargetTypeName = null;

        try
        {
            var document = XDocument.Parse(source.ToString(), LoadOptions.PreserveWhitespace);
            var root = document.Root;
            isResourceDictionary = string.Equals(
                root?.Name.LocalName,
                "ResourceDictionary",
                StringComparison.Ordinal);
            if (root is not null &&
                string.Equals(root.Name.LocalName, "ControlTheme", StringComparison.Ordinal))
            {
                XNamespace xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
                controlThemeClassName = root.Attribute(xamlNamespace + "Class")?.Value;
                controlThemeTargetTypeName = GetTypeName(
                    root.Attributes()
                        .FirstOrDefault(static attribute =>
                            string.Equals(attribute.Name.LocalName, "TargetType", StringComparison.Ordinal))
                        ?.Value);
            }
            if (root is not null)
            {
                foreach (var controlTheme in root.DescendantsAndSelf().Where(static element =>
                             string.Equals(element.Name.LocalName, "ControlTheme", StringComparison.Ordinal)))
                {
                    var targetTypeAttribute = controlTheme.Attributes().FirstOrDefault(static attribute =>
                        string.Equals(attribute.Name.LocalName, "TargetType", StringComparison.Ordinal));
                    if (targetTypeAttribute is null)
                    {
                        continue;
                    }

                    var targetType = ThemeAssetTargetTypeReference.Create(
                        controlTheme,
                        targetTypeAttribute.Value);
                    var themeTemplates = new List<ThemeAssetSemanticTemplateInfo>();
                    var templateSetters = controlTheme.Descendants()
                                                      .Where(IsTemplateSetter)
                                                      .Where(setter => ReferenceEquals(
                                                          setter.Ancestors().FirstOrDefault(static ancestor =>
                                                              string.Equals(
                                                                  ancestor.Name.LocalName,
                                                                  "ControlTheme",
                                                                  StringComparison.Ordinal)),
                                                          controlTheme))
                                                      .ToArray();
                    var overridesBaseTemplate = false;
                    foreach (var templateSetter in templateSetters)
                    {
                        var isConditional = templateSetter.Ancestors()
                                                          .TakeWhile(ancestor => !ReferenceEquals(
                                                              ancestor,
                                                              controlTheme))
                                                          .Any(static ancestor => string.Equals(
                                                              ancestor.Name.LocalName,
                                                              "Style",
                                                              StringComparison.Ordinal));
                        overridesBaseTemplate |= !isConditional;
                        foreach (var template in templateSetter.Descendants().Where(static element =>
                                     string.Equals(
                                         element.Name.LocalName,
                                         "ControlTemplate",
                                         StringComparison.Ordinal)))
                        {
                            var nearestSetter = template.Ancestors().FirstOrDefault(IsTemplateSetter);
                            if (!ReferenceEquals(nearestSetter, templateSetter))
                            {
                                continue;
                            }

                            var markers = new List<ThemeAssetSemanticMarkerInfo>();
                            var nodeId = 0;
                            foreach (var element in template.Descendants())
                            {
                                var currentNodeId = nodeId++;
                                var classes = element.Attributes().FirstOrDefault(static attribute =>
                                    string.Equals(attribute.Name.LocalName, "Classes", StringComparison.Ordinal));
                                if (classes is null)
                                {
                                    continue;
                                }

                                foreach (var selectorClass in classes.Value.Split(
                                             (char[]?)null,
                                             StringSplitOptions.RemoveEmptyEntries)
                                         .Distinct(StringComparer.Ordinal))
                                {
                                    if (selectorClass.StartsWith("semantic-", StringComparison.Ordinal))
                                    {
                                        markers.Add(new ThemeAssetSemanticMarkerInfo(
                                            selectorClass,
                                            element.Name.NamespaceName,
                                            element.Name.LocalName,
                                            currentNodeId));
                                    }
                                }
                            }
                            var templateInfo = new ThemeAssetSemanticTemplateInfo(
                                ++nextSemanticTemplateIndex,
                                markers);
                            themeTemplates.Add(templateInfo);
                        }
                    }
                    semanticThemes.Add(new ThemeAssetSemanticThemeInfo(
                        targetType,
                        CreateBasedOnReference(controlTheme),
                        themeTemplates,
                        overridesBaseTemplate));
                }
            }
            foreach (var element in document.Descendants())
            {
                elementTypes.Add(new ThemeAssetElementTypeReference(
                    element.Name.NamespaceName,
                    element.Name.LocalName));
                if (!string.Equals(element.Name.LocalName, "ControlTheme", StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (var targetType in element.Attributes().Where(static attribute =>
                             string.Equals(attribute.Name.LocalName, "TargetType", StringComparison.Ordinal)))
                {
                    targetTypes.Add(ThemeAssetTargetTypeReference.Create(element, targetType.Value));
                }
            }
            foreach (var attribute in document.Descendants().Attributes())
            {
                AddControlTokenFamilies(attribute.Value, controlTokenFamilies);
            }
            foreach (var textNode in document.DescendantNodes().OfType<XText>())
            {
                AddControlTokenFamilies(textNode.Value, controlTokenFamilies);
            }
        }
        catch
        {
            // Avalonia reports malformed AXAML. This generator consumes only successfully parsed structure.
        }

        return new ThemeAssetInfo(
            text.Path,
            assetPath,
            source,
            fileName,
            GetControlCandidate(fileName),
            explicitUnit,
            GetDirectoryCandidates(assetPath),
            targetTypes,
            elementTypes.OrderBy(static reference => reference.NamespaceUri, StringComparer.Ordinal)
                        .ThenBy(static reference => reference.LocalName, StringComparer.Ordinal)
                        .ToArray(),
            semanticThemes,
            controlTokenFamilies.OrderBy(static family => family, StringComparer.Ordinal).ToArray(),
            isResourceDictionary,
            controlThemeClassName,
            controlThemeTargetTypeName);
    }


    private static ThemeAssetTargetTypeReference? CreateBasedOnReference(XElement controlTheme)
    {
        var basedOn = controlTheme.Attributes().FirstOrDefault(static attribute =>
            string.Equals(attribute.Name.LocalName, "BasedOn", StringComparison.Ordinal));
        if (basedOn is null)
        {
            return null;
        }

        const string prefix = "{StaticResource ";
        var value = basedOn.Value.Trim();
        if (!value.StartsWith(prefix, StringComparison.Ordinal) ||
            !value.EndsWith("}", StringComparison.Ordinal))
        {
            return null;
        }

        var resourceKey = value.Substring(prefix.Length, value.Length - prefix.Length - 1).Trim();
        if (!resourceKey.StartsWith("{x:Type", StringComparison.Ordinal) ||
            !resourceKey.EndsWith("}", StringComparison.Ordinal))
        {
            return null;
        }
        return ThemeAssetTargetTypeReference.Create(controlTheme, resourceKey);
    }

    private static bool IsTemplateSetter(XElement element)
    {
        if (!string.Equals(element.Name.LocalName, "Setter", StringComparison.Ordinal))
        {
            return false;
        }

        var property = element.Attributes().FirstOrDefault(static attribute =>
            string.Equals(attribute.Name.LocalName, "Property", StringComparison.Ordinal));
        return string.Equals(property?.Value, "Template", StringComparison.Ordinal);
    }

    internal static bool IsAggregatePath(string path)
    {
        return System.IO.Path.GetFileNameWithoutExtension(path)
                             .EndsWith("Themes", StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsThemeAssetPath(string path)
    {
        if (!path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase) || IsAggregatePath(path))
        {
            return false;
        }

        var normalized = path.Replace('\\', '/').TrimStart('/');
        return normalized.StartsWith("Themes/", StringComparison.OrdinalIgnoreCase) ||
               normalized.IndexOf("/Themes/", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    internal static string GetGeneratedResourceClassName(string assetPath)
    {
        var hash = 14695981039346656037UL;
        foreach (var character in assetPath.Replace('\\', '/'))
        {
            hash ^= character;
            hash *= 1099511628211UL;
        }
        return $"GeneratedThemeAssetResource_{hash:X16}";
    }

    private static string? GetTypeName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }
        var typeName = value!.Trim();
        if (typeName.StartsWith("{x:Type", StringComparison.Ordinal) &&
            typeName.EndsWith("}", StringComparison.Ordinal))
        {
            typeName = typeName.Substring("{x:Type".Length, typeName.Length - "{x:Type".Length - 1).Trim();
        }
        var separator = typeName.LastIndexOf(':');
        return separator >= 0 ? typeName.Substring(separator + 1) : typeName;
    }

    private static void AddControlTokenFamilies(string value, ISet<string> families)
    {
        const string suffix = "TokenResource";
        var searchIndex = 0;
        while (searchIndex < value.Length)
        {
            var openingBrace = value.IndexOf('{', searchIndex);
            if (openingBrace < 0)
            {
                return;
            }

            var nameStart = openingBrace + 1;
            while (nameStart < value.Length && char.IsWhiteSpace(value[nameStart]))
            {
                nameStart++;
            }

            var nameEnd = nameStart;
            while (nameEnd < value.Length &&
                   !char.IsWhiteSpace(value[nameEnd]) &&
                   value[nameEnd] != ',' &&
                   value[nameEnd] != '}')
            {
                nameEnd++;
            }

            if (nameEnd > nameStart)
            {
                var extensionName = value.Substring(nameStart, nameEnd - nameStart);
                var namespaceSeparator = extensionName.LastIndexOf(':');
                var localName = namespaceSeparator >= 0
                    ? extensionName.Substring(namespaceSeparator + 1)
                    : extensionName;
                if (localName.EndsWith(suffix, StringComparison.Ordinal) &&
                    localName.Length > suffix.Length)
                {
                    var family = localName.Substring(0, localName.Length - suffix.Length);
                    if (!string.Equals(family, "Shared", StringComparison.Ordinal))
                    {
                        families.Add(family);
                    }
                }
            }

            searchIndex = openingBrace + 1;
        }
    }

    internal Location CreateLocation()
    {
        var span = new TextSpan(0, Source.Length);
        return Location.Create(Path, span, Source.Lines.GetLinePositionSpan(span));
    }

    private static string? GetControlCandidate(string fileName)
    {
        const string suffix = "Theme";
        if (!fileName.EndsWith(suffix, StringComparison.Ordinal) ||
            fileName.EndsWith("Themes", StringComparison.Ordinal) ||
            fileName.Length == suffix.Length)
        {
            return null;
        }

        return fileName.Substring(0, fileName.Length - suffix.Length);
    }

    private static IReadOnlyList<string> GetDirectoryCandidates(string assetPath)
    {
        var segments = assetPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        var themesIndex = Array.FindIndex(
            segments,
            static segment => string.Equals(segment, "Themes", StringComparison.OrdinalIgnoreCase));
        if (themesIndex <= 0)
        {
            return Array.Empty<string>();
        }

        return segments.Take(themesIndex)
                       .Reverse()
                       .Where(static segment => IsIdentifier(segment))
                       .Distinct(StringComparer.Ordinal)
                       .ToArray();
    }

    private static bool IsIdentifier(string value)
    {
        if (value.Length == 0 || !(char.IsLetter(value[0]) || value[0] == '_'))
        {
            return false;
        }

        return value.Skip(1).All(static character => char.IsLetterOrDigit(character) || character == '_');
    }

    internal static string NormalizeAssetPath(
        string path,
        string? projectDirectory,
        string? link)
    {
        if (!string.IsNullOrWhiteSpace(link))
        {
            return link!.Replace('\\', '/').TrimStart('/');
        }

        var normalized = path.Replace('\\', '/');
        if (!System.IO.Path.IsPathRooted(path))
        {
            return normalized.TrimStart('/');
        }

        if (!string.IsNullOrWhiteSpace(projectDirectory))
        {
            var normalizedProjectDirectory = projectDirectory!
                                             .Replace('\\', '/')
                                             .TrimEnd('/') + "/";
            if (normalized.StartsWith(
                    normalizedProjectDirectory,
                    StringComparison.OrdinalIgnoreCase))
            {
                return normalized.Substring(normalizedProjectDirectory.Length);
            }
        }

        var sourceMarker = normalized.IndexOf("/src/", StringComparison.OrdinalIgnoreCase);
        if (sourceMarker >= 0)
        {
            var projectStart = sourceMarker + 5;
            var relativeStart = normalized.IndexOf('/', projectStart);
            if (relativeStart >= 0 && relativeStart + 1 < normalized.Length)
            {
                return normalized.Substring(relativeStart + 1);
            }
        }

        return System.IO.Path.GetFileName(path);
    }
}

internal sealed class ThemeAssetSemanticTemplateInfo
{
    internal ThemeAssetSemanticTemplateInfo(
        int index,
        IReadOnlyList<ThemeAssetSemanticMarkerInfo> markers)
    {
        Index = index;
        Markers = markers;
    }

    internal int Index { get; }
    internal IReadOnlyList<ThemeAssetSemanticMarkerInfo> Markers { get; }
}

internal sealed class ThemeAssetSemanticThemeInfo
{
    internal ThemeAssetSemanticThemeInfo(
        ThemeAssetTargetTypeReference targetType,
        ThemeAssetTargetTypeReference? basedOn,
        IReadOnlyList<ThemeAssetSemanticTemplateInfo> templates,
        bool overridesBaseTemplate)
    {
        TargetType = targetType;
        BasedOn = basedOn;
        Templates = templates;
        OverridesBaseTemplate = overridesBaseTemplate;
    }

    internal ThemeAssetTargetTypeReference TargetType { get; }
    internal ThemeAssetTargetTypeReference? BasedOn { get; }
    internal IReadOnlyList<ThemeAssetSemanticTemplateInfo> Templates { get; }
    internal bool OverridesBaseTemplate { get; }
}

internal sealed class ThemeAssetSemanticMarkerInfo
{
    internal ThemeAssetSemanticMarkerInfo(
        string selectorClass,
        string xmlNamespace,
        string typeName,
        int nodeId)
    {
        SelectorClass = selectorClass;
        XmlNamespace = xmlNamespace;
        TypeName = typeName;
        NodeId = nodeId;
    }

    internal string SelectorClass { get; }
    internal string XmlNamespace { get; }
    internal string TypeName { get; }
    internal int NodeId { get; }
}


internal sealed class ThemeAssetElementTypeReference : IEquatable<ThemeAssetElementTypeReference>
{
    internal ThemeAssetElementTypeReference(string namespaceUri, string localName)
    {
        NamespaceUri = namespaceUri;
        LocalName = localName;
    }

    internal string NamespaceUri { get; }
    internal string LocalName { get; }

    public bool Equals(ThemeAssetElementTypeReference? other)
    {
        return other is not null &&
               string.Equals(NamespaceUri, other.NamespaceUri, StringComparison.Ordinal) &&
               string.Equals(LocalName, other.LocalName, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as ThemeAssetElementTypeReference);

    public override int GetHashCode()
    {
        unchecked
        {
            return (StringComparer.Ordinal.GetHashCode(NamespaceUri) * 397) ^
                   StringComparer.Ordinal.GetHashCode(LocalName);
        }
    }
}

internal sealed class ThemeAssetTargetTypeReference
{
    private ThemeAssetTargetTypeReference(
        string value,
        IReadOnlyDictionary<string, string> namespaces)
    {
        Value = value;
        Namespaces = namespaces;
    }

    internal string Value { get; }
    internal IReadOnlyDictionary<string, string> Namespaces { get; }

    internal static ThemeAssetTargetTypeReference Create(XElement element, string value)
    {
        var namespaces = element.AncestorsAndSelf()
                                .Reverse()
                                .SelectMany(static current => current.Attributes().Where(static attribute =>
                                    attribute.IsNamespaceDeclaration))
                                .ToDictionary(
                                    static attribute => attribute.Name.LocalName == "xmlns"
                                        ? string.Empty
                                        : attribute.Name.LocalName,
                                    static attribute => attribute.Value,
                                    StringComparer.Ordinal);
        return new ThemeAssetTargetTypeReference(value, namespaces);
    }
}

internal sealed class ThemeAssetSemanticPartInfo
{
    internal ThemeAssetSemanticPartInfo(string propertyName, string targetTypeName)
    {
        PropertyName = propertyName;
        TargetTypeName = targetTypeName;
    }

    internal string PropertyName { get; }
    internal string TargetTypeName { get; }
}

internal sealed class ResolvedThemeAssetInfo
{
    internal ResolvedThemeAssetInfo(
        ThemeAssetInfo asset,
        ThemeAssetControlIdentityInfo ownerIdentity,
        string ownerUnitId,
        IReadOnlyList<ThemeAssetControlIdentityInfo> referencedControlIdentities,
        IReadOnlyList<string> referencedUnitIds,
        ThemeAssetSemanticPartInfo? semanticPart)
    {
        Asset = asset;
        OwnerIdentity = ownerIdentity;
        OwnerUnitId = ownerUnitId;
        ReferencedControlIdentities = referencedControlIdentities;
        ReferencedUnitIds = referencedUnitIds;
        SemanticPart = semanticPart;
    }

    internal ThemeAssetInfo Asset { get; }
    internal ThemeAssetControlIdentityInfo OwnerIdentity { get; }
    internal string OwnerUnitId { get; }
    internal IReadOnlyList<ThemeAssetControlIdentityInfo> ReferencedControlIdentities { get; }
    internal IReadOnlyList<string> ReferencedUnitIds { get; }
    internal ThemeAssetSemanticPartInfo? SemanticPart { get; }
}

internal sealed class UnitOwnedThemeAssetInfo
{
    internal UnitOwnedThemeAssetInfo(ThemeAssetInfo asset, string unitId)
    {
        Asset = asset;
        UnitId = unitId;
    }

    internal ThemeAssetInfo Asset { get; }
    internal string UnitId { get; }
}

internal sealed class ThemeAssetControlIdentityInfo : IEquatable<ThemeAssetControlIdentityInfo>
{
    internal ThemeAssetControlIdentityInfo(string catalog, string id)
    {
        Catalog = catalog;
        Id = id;
    }

    internal string Catalog { get; }
    internal string Id { get; }

    public bool Equals(ThemeAssetControlIdentityInfo? other)
    {
        return other is not null && Catalog == other.Catalog && Id == other.Id;
    }

    public override bool Equals(object? obj) => Equals(obj as ThemeAssetControlIdentityInfo);

    public override int GetHashCode()
    {
        unchecked
        {
            return (StringComparer.Ordinal.GetHashCode(Catalog) * 397) ^
                   StringComparer.Ordinal.GetHashCode(Id);
        }
    }
}
