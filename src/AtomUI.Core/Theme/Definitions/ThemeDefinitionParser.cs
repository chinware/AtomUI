using System.Xml;
using System.Xml.Linq;

namespace AtomUI.Theme.Definitions;

internal sealed record ThemeDefinitionParseRequest(
    Stream Stream,
    string FilePath,
    IReadOnlySet<string> SharedTokenNames,
    IReadOnlyDictionary<string, IReadOnlySet<string>> ComponentOwnTokenNames);

internal static class ThemeDefinitionParser
{
    private const string InvalidXmlCode = "ATMTHM001";
    private const string InvalidStructureCode = "ATMTHM002";
    private const string RequiredAttributeCode = "ATMTHM003";
    private const string InvalidBooleanCode = "ATMTHM004";
    private const string UnknownAlgorithmCode = "ATMTHM005";
    private const string InvalidTokenValueCode = "ATMTHM006";
    private const string DuplicateKeyCode = "ATMTHM007";
    private const string UnknownSharedTokenCode = "ATMTHM008";
    private const string UnknownComponentTokenCode = "ATMTHM009";
    private const string UnregisteredComponentCode = "ATMTHM010";

    private const string ThemeElementName = "Theme";
    private const string AlgorithmsElementName = "Algorithms";
    private const string SharedTokensElementName = "SharedTokens";
    private const string ControlTokensElementName = "ControlTokens";
    private const string ControlTokenElementName = "ControlToken";
    private const string TokenElementName = "Token";

    private const string NameAttributeName = "Name";
    private const string IsDefaultAttributeName = "IsDefault";
    private const string IdAttributeName = "Id";
    private const string EnableAlgorithmAttributeName = "EnableAlgorithm";
    private const string ValueAttributeName = "Value";
    private const string IsSharedAttributeName = "IsShared";

    private const string RootPath = "/Theme";

    internal static ThemeDefinitionParseResult Parse(ThemeDefinitionParseRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Stream);
        ArgumentNullException.ThrowIfNull(request.FilePath);
        ArgumentNullException.ThrowIfNull(request.SharedTokenNames);
        ArgumentNullException.ThrowIfNull(request.ComponentOwnTokenNames);

        var diagnostics = new List<ThemeDefinitionDiagnostic>();
        XDocument document;
        try
        {
            var settings = new XmlReaderSettings
            {
                CloseInput   = false,
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver  = null
            };

            using var reader = XmlReader.Create(request.Stream, settings, request.FilePath);
            document = XDocument.Load(reader, LoadOptions.SetLineInfo);
        }
        catch (XmlException exception)
        {
            diagnostics.Add(new ThemeDefinitionDiagnostic(
                                InvalidXmlCode,
                                ThemeDiagnosticSeverity.Error,
                                request.FilePath,
                                exception.LineNumber,
                                exception.LinePosition,
                                "/",
                                "The theme definition is not well-formed XML."));
            return new ThemeDefinitionParseResult(null, diagnostics);
        }

        var root = document.Root;
        if (root is null)
        {
            AddDiagnostic(
                diagnostics,
                InvalidXmlCode,
                request.FilePath,
                null,
                "/",
                "The theme definition is not well-formed XML.");
            return new ThemeDefinitionParseResult(null, diagnostics);
        }

        if (root.Name != ThemeElementName)
        {
            AddDiagnostic(
                diagnostics,
                InvalidStructureCode,
                request.FilePath,
                root,
                $"/{root.Name.LocalName}",
                $"Expected root element '{ThemeElementName}', but found '{root.Name.LocalName}'.");
            return new ThemeDefinitionParseResult(null, diagnostics);
        }

        ValidateAttributes(
            root,
            RootPath,
            request.FilePath,
            diagnostics,
            NameAttributeName,
            IsDefaultAttributeName);
        ValidateContainerText(root, RootPath, request.FilePath, diagnostics);

        var displayName = ReadRequiredAttribute(
            root,
            NameAttributeName,
            RootPath,
            request.FilePath,
            diagnostics);
        var isDefaultText = ReadRequiredAttribute(
            root,
            IsDefaultAttributeName,
            RootPath,
            request.FilePath,
            diagnostics);
        var isDefault = false;
        if (isDefaultText is not null)
        {
            TryParseBoolean(
                root.Attribute(IsDefaultAttributeName)!,
                RootPath,
                request.FilePath,
                diagnostics,
                out isDefault);
        }

        var algorithms = new List<ThemeAlgorithm>();
        var sharedTokens = new Dictionary<string, string>(StringComparer.Ordinal);
        var controlTokens = new Dictionary<string, ThemeControlTokenDefinition>(StringComparer.Ordinal);
        var hasAlgorithms = false;
        var hasSharedTokens = false;
        var hasControlTokens = false;

        foreach (var element in root.Elements())
        {
            if (element.Name == AlgorithmsElementName)
            {
                if (hasAlgorithms)
                {
                    AddDuplicateElementDiagnostic(element, RootPath, request.FilePath, diagnostics);
                    continue;
                }

                hasAlgorithms = true;
                ParseAlgorithms(element, request.FilePath, algorithms, diagnostics);
            }
            else if (element.Name == SharedTokensElementName)
            {
                if (hasSharedTokens)
                {
                    AddDuplicateElementDiagnostic(element, RootPath, request.FilePath, diagnostics);
                    continue;
                }

                hasSharedTokens = true;
                ParseSharedTokens(element, request, sharedTokens, diagnostics);
            }
            else if (element.Name == ControlTokensElementName)
            {
                if (hasControlTokens)
                {
                    AddDuplicateElementDiagnostic(element, RootPath, request.FilePath, diagnostics);
                    continue;
                }

                hasControlTokens = true;
                ParseControlTokens(element, request, controlTokens, diagnostics);
            }
            else
            {
                var path = $"{RootPath}/{element.Name.LocalName}";
                AddDiagnostic(
                    diagnostics,
                    InvalidStructureCode,
                    request.FilePath,
                    element,
                    path,
                    $"Element '{element.Name.LocalName}' is not valid under '{ThemeElementName}'.");
            }
        }

        if (displayName is null || HasErrors(diagnostics))
        {
            return new ThemeDefinitionParseResult(null, diagnostics);
        }

        var id = Path.GetFileNameWithoutExtension(request.FilePath);
        if (string.IsNullOrWhiteSpace(id))
        {
            id = displayName;
        }

        var definition = new ThemeDefinition(
            id,
            displayName,
            isDefault,
            algorithms,
            sharedTokens,
            controlTokens);
        return new ThemeDefinitionParseResult(definition, diagnostics);
    }

    private static void ParseAlgorithms(
        XElement element,
        string filePath,
        List<ThemeAlgorithm> algorithms,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var path = $"{RootPath}/{AlgorithmsElementName}";
        ValidateAttributes(element, path, filePath, diagnostics);
        if (element.HasElements)
        {
            AddDiagnostic(
                diagnostics,
                InvalidStructureCode,
                filePath,
                element.Elements().First(),
                $"{path}/{element.Elements().First().Name.LocalName}",
                $"Element '{AlgorithmsElementName}' cannot contain child elements.");
            return;
        }

        var names = element.Value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (var name in names)
        {
            if (!TryParseAlgorithmName(name, out var algorithm))
            {
                AddDiagnostic(
                    diagnostics,
                    UnknownAlgorithmCode,
                    filePath,
                    element,
                    path,
                    $"Algorithm '{name}' is not supported.");
                continue;
            }

            if (!algorithms.Contains(algorithm))
            {
                algorithms.Add(algorithm);
            }
        }
    }

    private static void ParseSharedTokens(
        XElement container,
        ThemeDefinitionParseRequest request,
        Dictionary<string, string> sharedTokens,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var containerPath = $"{RootPath}/{SharedTokensElementName}";
        ValidateAttributes(container, containerPath, request.FilePath, diagnostics);
        ValidateContainerText(container, containerPath, request.FilePath, diagnostics);
        var seenTokenNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var element in container.Elements())
        {
            if (element.Name != TokenElementName)
            {
                AddInvalidChildDiagnostic(
                    element,
                    SharedTokensElementName,
                    containerPath,
                    request.FilePath,
                    diagnostics);
                continue;
            }

            var name = ReadRequiredAttribute(
                element,
                NameAttributeName,
                $"{containerPath}/{TokenElementName}",
                request.FilePath,
                diagnostics);
            var tokenPath = BuildTokenPath(containerPath, name);
            ValidateAttributes(
                element,
                tokenPath,
                request.FilePath,
                diagnostics,
                NameAttributeName,
                ValueAttributeName);
            if (name is null)
            {
                continue;
            }

            if (!seenTokenNames.Add(name))
            {
                AddDuplicateTokenDiagnostic(element, name, tokenPath, request.FilePath, diagnostics);
                continue;
            }

            var value = ReadTokenValue(element, name, tokenPath, request.FilePath, diagnostics);
            if (!request.SharedTokenNames.Contains(name))
            {
                AddDiagnostic(
                    diagnostics,
                    UnknownSharedTokenCode,
                    request.FilePath,
                    element,
                    tokenPath,
                    $"Shared token '{name}' is not registered.");
                continue;
            }

            if (value is not null)
            {
                sharedTokens.Add(name, value);
            }
        }
    }

    private static void ParseControlTokens(
        XElement container,
        ThemeDefinitionParseRequest request,
        Dictionary<string, ThemeControlTokenDefinition> controlTokens,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var containerPath = $"{RootPath}/{ControlTokensElementName}";
        ValidateAttributes(container, containerPath, request.FilePath, diagnostics);
        ValidateContainerText(container, containerPath, request.FilePath, diagnostics);

        foreach (var element in container.Elements())
        {
            if (element.Name != ControlTokenElementName)
            {
                AddInvalidChildDiagnostic(
                    element,
                    ControlTokensElementName,
                    containerPath,
                    request.FilePath,
                    diagnostics);
                continue;
            }

            var id = ReadRequiredAttribute(
                element,
                IdAttributeName,
                $"{containerPath}/{ControlTokenElementName}",
                request.FilePath,
                diagnostics);
            var controlPath = BuildControlTokenPath(containerPath, id);
            ValidateAttributes(
                element,
                controlPath,
                request.FilePath,
                diagnostics,
                IdAttributeName,
                EnableAlgorithmAttributeName);
            ValidateContainerText(element, controlPath, request.FilePath, diagnostics);
            if (id is null)
            {
                continue;
            }

            if (controlTokens.ContainsKey(id))
            {
                AddDiagnostic(
                    diagnostics,
                    DuplicateKeyCode,
                    request.FilePath,
                    element,
                    controlPath,
                    $"Component '{id}' is defined more than once.");
                continue;
            }

            var enableAlgorithm = false;
            var enableAlgorithmAttribute = element.Attribute(EnableAlgorithmAttributeName);
            if (enableAlgorithmAttribute is not null &&
                !TryParseBoolean(
                    enableAlgorithmAttribute,
                    controlPath,
                    request.FilePath,
                    diagnostics,
                    out enableAlgorithm))
            {
                enableAlgorithm = false;
            }

            var isRegistered = request.ComponentOwnTokenNames.TryGetValue(id, out var ownTokenNames);
            if (!isRegistered)
            {
                AddDiagnostic(
                    diagnostics,
                    UnregisteredComponentCode,
                    request.FilePath,
                    element,
                    controlPath,
                    $"Component '{id}' is not registered; its own tokens were preserved without schema validation.",
                    ThemeDiagnosticSeverity.Warning);
            }

            var tokens = new Dictionary<string, string>(StringComparer.Ordinal);
            var sharedTokens = new Dictionary<string, string>(StringComparer.Ordinal);
            ParseComponentTokens(
                element,
                id,
                controlPath,
                isRegistered ? ownTokenNames : null,
                request,
                tokens,
                sharedTokens,
                diagnostics);
            controlTokens.Add(
                id,
                new ThemeControlTokenDefinition(
                    id,
                    enableAlgorithm,
                    tokens,
                    sharedTokens));
        }
    }

    private static void ParseComponentTokens(
        XElement controlElement,
        string componentId,
        string controlPath,
        IReadOnlySet<string>? ownTokenNames,
        ThemeDefinitionParseRequest request,
        Dictionary<string, string> tokens,
        Dictionary<string, string> sharedTokens,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var seenOwnTokenNames = new HashSet<string>(StringComparer.Ordinal);
        var seenSharedTokenNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var element in controlElement.Elements())
        {
            if (element.Name != TokenElementName)
            {
                AddInvalidChildDiagnostic(
                    element,
                    ControlTokenElementName,
                    controlPath,
                    request.FilePath,
                    diagnostics);
                continue;
            }

            var name = ReadRequiredAttribute(
                element,
                NameAttributeName,
                $"{controlPath}/{TokenElementName}",
                request.FilePath,
                diagnostics);
            var tokenPath = BuildTokenPath(controlPath, name);
            ValidateAttributes(
                element,
                tokenPath,
                request.FilePath,
                diagnostics,
                NameAttributeName,
                ValueAttributeName,
                IsSharedAttributeName);
            if (name is null)
            {
                continue;
            }

            var isShared = false;
            var isSharedAttribute = element.Attribute(IsSharedAttributeName);
            if (isSharedAttribute is not null &&
                !TryParseBoolean(
                    isSharedAttribute,
                    tokenPath,
                    request.FilePath,
                    diagnostics,
                    out isShared))
            {
                continue;
            }

            var seenTokenNames = isShared ? seenSharedTokenNames : seenOwnTokenNames;
            if (!seenTokenNames.Add(name))
            {
                AddDuplicateTokenDiagnostic(element, name, tokenPath, request.FilePath, diagnostics);
                continue;
            }

            var targetTokens = isShared ? sharedTokens : tokens;
            var value = ReadTokenValue(element, name, tokenPath, request.FilePath, diagnostics);

            if (isShared)
            {
                if (!request.SharedTokenNames.Contains(name))
                {
                    AddDiagnostic(
                        diagnostics,
                        UnknownSharedTokenCode,
                        request.FilePath,
                        element,
                        tokenPath,
                        $"Shared token '{name}' is not registered.");
                    continue;
                }
            }
            else if (ownTokenNames is not null && !ownTokenNames.Contains(name))
            {
                AddDiagnostic(
                    diagnostics,
                    UnknownComponentTokenCode,
                    request.FilePath,
                    element,
                    tokenPath,
                    $"Token '{name}' is not registered for component '{componentId}'.");
                continue;
            }

            if (value is not null)
            {
                targetTokens.Add(name, value);
            }
        }
    }

    private static string? ReadTokenValue(
        XElement element,
        string tokenName,
        string path,
        string filePath,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var valueAttribute = element.Attribute(ValueAttributeName);
        var attributeValue = valueAttribute?.Value.Trim();
        var contentValue = element.HasElements ? null : element.Value.Trim();
        var hasAttributeValue = !string.IsNullOrWhiteSpace(attributeValue);
        var hasContentValue = !string.IsNullOrWhiteSpace(contentValue);
        if (element.HasElements || hasAttributeValue == hasContentValue)
        {
            AddDiagnostic(
                diagnostics,
                InvalidTokenValueCode,
                filePath,
                element,
                path,
                $"Token '{tokenName}' must specify exactly one non-empty value using either the '{ValueAttributeName}' attribute or element content.");
            return null;
        }

        return hasAttributeValue ? attributeValue : contentValue;
    }

    private static string? ReadRequiredAttribute(
        XElement element,
        string attributeName,
        string elementPath,
        string filePath,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        var attribute = element.Attribute(attributeName);
        var value = attribute?.Value.Trim();
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        AddDiagnostic(
            diagnostics,
            RequiredAttributeCode,
            filePath,
            attribute is not null ? attribute : element,
            $"{elementPath}/@{attributeName}",
            $"Required attribute '{attributeName}' is missing or empty.");
        return null;
    }

    private static bool TryParseBoolean(
        XAttribute attribute,
        string elementPath,
        string filePath,
        List<ThemeDefinitionDiagnostic> diagnostics,
        out bool value)
    {
        if (bool.TryParse(attribute.Value.Trim(), out value))
        {
            return true;
        }

        AddDiagnostic(
            diagnostics,
            InvalidBooleanCode,
            filePath,
            attribute,
            $"{elementPath}/@{attribute.Name.LocalName}",
            $"Attribute '{attribute.Name.LocalName}' must be 'true' or 'false'.");
        return false;
    }

    private static void ValidateAttributes(
        XElement element,
        string elementPath,
        string filePath,
        List<ThemeDefinitionDiagnostic> diagnostics,
        params string[] allowedNames)
    {
        foreach (var attribute in element.Attributes())
        {
            if (attribute.IsNamespaceDeclaration)
            {
                continue;
            }

            if (attribute.Name.Namespace == XNamespace.None &&
                allowedNames.Contains(attribute.Name.LocalName, StringComparer.Ordinal))
            {
                continue;
            }

            var attributeName = FormatXmlName(attribute.Name);

            AddDiagnostic(
                diagnostics,
                InvalidStructureCode,
                filePath,
                attribute,
                $"{elementPath}/@{attributeName}",
                $"Attribute '{attributeName}' is not valid on element '{element.Name.LocalName}'.");
        }
    }

    private static bool TryParseAlgorithmName(string name, out ThemeAlgorithm algorithm)
    {
        switch (name)
        {
            case nameof(ThemeAlgorithm.Default):
                algorithm = ThemeAlgorithm.Default;
                return true;
            case nameof(ThemeAlgorithm.Dark):
                algorithm = ThemeAlgorithm.Dark;
                return true;
            case nameof(ThemeAlgorithm.Compact):
                algorithm = ThemeAlgorithm.Compact;
                return true;
            default:
                algorithm = default;
                return false;
        }
    }

    private static string FormatXmlName(XName name)
    {
        return name.Namespace == XNamespace.None ? name.LocalName : name.ToString();
    }

    private static void ValidateContainerText(
        XElement element,
        string elementPath,
        string filePath,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        foreach (var text in element.Nodes().OfType<XText>())
        {
            if (string.IsNullOrWhiteSpace(text.Value))
            {
                continue;
            }

            AddDiagnostic(
                diagnostics,
                InvalidStructureCode,
                filePath,
                text,
                elementPath,
                $"Element '{element.Name.LocalName}' cannot contain text content.");
        }
    }

    private static void AddDuplicateElementDiagnostic(
        XElement element,
        string parentPath,
        string filePath,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        AddDiagnostic(
            diagnostics,
            DuplicateKeyCode,
            filePath,
            element,
            $"{parentPath}/{element.Name.LocalName}",
            $"Element '{element.Name.LocalName}' may appear only once.");
    }

    private static void AddDuplicateTokenDiagnostic(
        XElement element,
        string name,
        string tokenPath,
        string filePath,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        AddDiagnostic(
            diagnostics,
            DuplicateKeyCode,
            filePath,
            element,
            tokenPath,
            $"Token '{name}' is defined more than once in the same scope.");
    }

    private static void AddInvalidChildDiagnostic(
        XElement element,
        string parentName,
        string parentPath,
        string filePath,
        List<ThemeDefinitionDiagnostic> diagnostics)
    {
        AddDiagnostic(
            diagnostics,
            InvalidStructureCode,
            filePath,
            element,
            $"{parentPath}/{element.Name.LocalName}",
            $"Element '{element.Name.LocalName}' is not valid under '{parentName}'.");
    }

    private static void AddDiagnostic(
        List<ThemeDefinitionDiagnostic> diagnostics,
        string code,
        string filePath,
        XObject? source,
        string path,
        string message,
        ThemeDiagnosticSeverity severity = ThemeDiagnosticSeverity.Error)
    {
        var line = 0;
        var column = 0;
        if (source is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            line   = lineInfo.LineNumber;
            column = lineInfo.LinePosition;
        }

        diagnostics.Add(new ThemeDefinitionDiagnostic(
                            code,
                            severity,
                            filePath,
                            line,
                            column,
                            path,
                            message));
    }

    private static bool HasErrors(IEnumerable<ThemeDefinitionDiagnostic> diagnostics)
    {
        return diagnostics.Any(static diagnostic => diagnostic.Severity == ThemeDiagnosticSeverity.Error);
    }

    private static string BuildControlTokenPath(string containerPath, string? id)
    {
        return id is null
            ? $"{containerPath}/{ControlTokenElementName}"
            : $"{containerPath}/{ControlTokenElementName}[@Id='{EscapePathValue(id)}']";
    }

    private static string BuildTokenPath(string parentPath, string? name)
    {
        return name is null
            ? $"{parentPath}/{TokenElementName}"
            : $"{parentPath}/{TokenElementName}[@Name='{EscapePathValue(name)}']";
    }

    private static string EscapePathValue(string value)
    {
        return value.Replace("'", "&apos;", StringComparison.Ordinal);
    }
}
