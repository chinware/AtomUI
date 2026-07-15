using System.Diagnostics;
using System.Xml;
using Avalonia.Platform;

namespace AtomUI.Theme;

internal class ThemeDefinitionReader
{
    private readonly Theme _theme;
    private ThemeDefinitionBuilder? _currentDefinitionBuilder;
    private bool _parseFinished;

    // 上下文信息
    private readonly Stack<string> _currentElementNames;
    private ControlTokenDefinitionBuilder? _currentControlTokenBuilder;
    private bool _inSharedTokenCtx;
    private bool _inControlTokenCtx;

    private const string ThemeElementName = "Theme";
    private const string IsDefaultAttrName = "IsDefault";
    private const string AlgorithmsElementName = "Algorithms";
    private const string SharedTokensElementName = "SharedTokens";
    private const string ControlTokensElementName = "ControlTokens";

    private const string TokenElementName = "Token";
    private const string NameAttrName = "Name";
    private const string ValueAttrName = "Value";
    private const string IsShardAttrName = "IsShared";
    
    private const string ControlTokenElementName = "ControlToken";
    private const string IdAttrName = "Id";
    private const string AlgorithmAttrName = "EnableAlgorithm";

    public ThemeDefinitionReader(Theme theme)
    {
        _theme               = theme;
        _currentElementNames = new Stack<string>();
    }

    public ThemeDefinition Load()
    {
        try
        {
            _currentDefinitionBuilder = new ThemeDefinitionBuilder(_theme.Id);
            var settings = new XmlReaderSettings
            {
                CloseInput = true
            };
            var    filePath = _theme.DefinitionFilePath;
            Stream stream;
            if (filePath.StartsWith("avares://"))
            {
                stream = AssetLoader.Open(new Uri(filePath));
            }
            else
            {
                stream = File.OpenRead(filePath);
            }

            using (var xmlReader = XmlReader.Create(stream, settings))
            {
                while (!_parseFinished && xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            HandleStartElement(xmlReader);
                            break;
                        case XmlNodeType.EndElement:
                            HandleEndElement(xmlReader.Name);
                            break;
                    }
                }
            }

            return _currentDefinitionBuilder.Build();
        }
        finally
        {
            _currentControlTokenBuilder = null;
            _currentElementNames.Clear();
            _inControlTokenCtx         = false;
            _inSharedTokenCtx          = false;
            _currentDefinitionBuilder  = null;
        }
    }

    private void HandleStartElement(XmlReader reader)
    {
        var name = reader.Name;
        _currentElementNames.Push(name);
        if (name == ThemeElementName)
        {
            HandleStartThemeElement(reader);
        }
        else if (name == AlgorithmsElementName)
        {
            HandleStartAlgorithmsElement(reader);
        }
        else if (name == SharedTokensElementName)
        {
            _currentDefinitionBuilder?.SharedTokens.Clear();
            _inSharedTokenCtx  = true;
            _inControlTokenCtx = false;
        }
        else if (name == ControlTokensElementName)
        {
            _currentDefinitionBuilder?.ControlTokens.Clear();
        }
        else if (name == ControlTokenElementName)
        {
            _inSharedTokenCtx  = false;
            _inControlTokenCtx = true;
            HandleStartControlTokenElement(reader);
        }
        else if (name == TokenElementName)
        {
            HandleStartTokenElement(reader);
            if (reader.IsEmptyElement)
            {
                HandleEndElement(TokenElementName);
            }
        }
        else
        {
            EmitErrorMsg(reader, $"Element tag: {string.Join('.', _currentElementNames.Reverse())} not supported.");
        }
    }

    private void HandleEndElement(string name)
    {
        _currentElementNames.Pop();
        if (name == ControlTokenElementName)
        {
            var tokenId = _currentControlTokenBuilder!.TokenId;
            _currentDefinitionBuilder?.ControlTokens.Add(tokenId, _currentControlTokenBuilder.Build());
            _currentControlTokenBuilder = null;
            _inControlTokenCtx          = false;
        }
        else if (name == SharedTokensElementName)
        {
            _inSharedTokenCtx = false;
        }

        _parseFinished = name == ThemeElementName;
    }

    private void HandleStartThemeElement(XmlReader reader)
    {
        Debug.Assert(_currentDefinitionBuilder != null);
        var displayName = reader.GetAttribute(NameAttrName);
        if (string.IsNullOrWhiteSpace(displayName))
        {
            EmitRequiredAttrError(reader, NameAttrName);
        }
        else
        {
            _currentDefinitionBuilder.DisplayName = displayName;
        }
        var isDefaultStr = reader.GetAttribute(IsDefaultAttrName);
        if (string.IsNullOrWhiteSpace(isDefaultStr))
        {
            EmitRequiredAttrError(reader, IsDefaultAttrName);
        }
        else
        {
            if (IsTrueValue(isDefaultStr))
            {
                _currentDefinitionBuilder.IsDefault = true;
            }
            else
            {
                _currentDefinitionBuilder.IsDefault = false;
            }
        }
    }

    private void HandleStartAlgorithmsElement(XmlReader reader)
    {
        Debug.Assert(_currentDefinitionBuilder != null);
        // 这样处理方便一点
        var algorithmsStr = reader.ReadElementContentAsString();
        _currentDefinitionBuilder.Algorithms.Clear();
        _currentDefinitionBuilder.Algorithms.AddRange(
            Theme.CheckAlgorithmNames(SplitDistinctAlgorithmNames(algorithmsStr)));
    }

    private void HandleStartControlTokenElement(XmlReader reader)
    {
        _currentControlTokenBuilder = new ControlTokenDefinitionBuilder();
        var tokenId = reader.GetAttribute(IdAttrName);
        if (string.IsNullOrWhiteSpace(tokenId))
        {
            EmitRequiredAttrError(reader, IdAttrName);
        }

        _currentControlTokenBuilder.TokenId = tokenId!;
        var useAlgorithm = false;
        var algorithm    = reader.GetAttribute(AlgorithmAttrName);
        if (algorithm is not null)
        {
            if (IsTrueValue(algorithm))
            {
                useAlgorithm = true;
            }
        }
        
        _currentControlTokenBuilder.EnableAlgorithm = useAlgorithm;
    }

    private void HandleStartTokenElement(XmlReader reader)
    {
        var tokenName = reader.GetAttribute(NameAttrName);
        if (string.IsNullOrWhiteSpace(tokenName))
        {
            EmitRequiredAttrError(reader, NameAttrName);
        }

        var tokenValue = reader.GetAttribute(ValueAttrName);
        if (tokenValue is null)
        {
            tokenValue = reader.ReadElementContentAsString();
        }

        if (string.IsNullOrWhiteSpace(tokenValue))
        {
            EmitRequiredAttrError(reader, NameAttrName);
        }

        if (_inSharedTokenCtx)
        {
            _currentDefinitionBuilder!.SharedTokens.Add(tokenName!, tokenValue);
        }
        else if (_inControlTokenCtx)
        {
            var isShared         = false;
            var isSharedValueStr = reader.GetAttribute(IsShardAttrName);
            if (isSharedValueStr is not null)
            {
                if (IsTrueValue(isSharedValueStr))
                {
                    isShared = true;
                }
            }
            if (isShared)
            {
                _currentControlTokenBuilder!.SharedTokens.Add(tokenName!, tokenValue);
            }
            else
            {
                _currentControlTokenBuilder!.Tokens.Add(tokenName!, tokenValue);
            }
        }
        else
        {
            EmitErrorMsg(reader, "The Token element must appear under WidgetToken or SharedTokens.");
        }
    }

    private void EmitErrorMsg(XmlReader reader, string? userErrorMsg)
    {
        var errorMsg = $"Error reading {_theme.DefinitionFilePath}";
        if (reader is IXmlLineInfo lineInfo && lineInfo.HasLineInfo())
        {
            errorMsg = $"{errorMsg}: {lineInfo.LineNumber}";
        }

        if (userErrorMsg is not null)
        {
            errorMsg = $"{errorMsg}: {userErrorMsg}";
        }

        throw new ThemeDefinitionParserException(errorMsg);
    }

    private void EmitRequiredAttrError(XmlReader reader, string attrName)
    {
        EmitErrorMsg(reader, $"Attribute: {reader.Name} of element: {attrName} is required");
    }

    private static bool IsTrueValue(string value)
    {
        return value.AsSpan().Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    private static List<string> SplitDistinctAlgorithmNames(string algorithms)
    {
        var algorithmNames = new List<string>(CountAlgorithmSegments(algorithms));
        var startIndex     = 0;
        for (int i = 0; i <= algorithms.Length; i++)
        {
            if (i != algorithms.Length && algorithms[i] != ',')
            {
                continue;
            }

            if (i > startIndex)
            {
                var algorithmName = algorithms.AsSpan(startIndex, i - startIndex).Trim();
                if (!ContainsAlgorithmName(algorithmNames, algorithmName))
                {
                    algorithmNames.Add(algorithmName.ToString());
                }
            }

            startIndex = i + 1;
        }

        return algorithmNames;
    }

    private static bool ContainsAlgorithmName(List<string> algorithmNames, ReadOnlySpan<char> algorithmName)
    {
        for (int i = 0; i < algorithmNames.Count; i++)
        {
            if (algorithmName.Equals(algorithmNames[i].AsSpan(), StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static int CountAlgorithmSegments(string algorithms)
    {
        var count      = 0;
        var startIndex = 0;
        for (int i = 0; i <= algorithms.Length; i++)
        {
            if (i != algorithms.Length && algorithms[i] != ',')
            {
                continue;
            }

            if (i > startIndex)
            {
                count++;
            }

            startIndex = i + 1;
        }

        return count;
    }

    private sealed class ThemeDefinitionBuilder
    {
        public string Id { get; }
        public string DisplayName { get; set; }
        public bool IsDefault { get; set; }
        public List<ThemeAlgorithm> Algorithms { get; }
        public Dictionary<string, ThemeControlTokenDefinition> ControlTokens { get; }
        public Dictionary<string, string> SharedTokens { get; }

        public ThemeDefinitionBuilder(string id)
        {
            Id            = id;
            DisplayName   = id;
            Algorithms    = new List<ThemeAlgorithm>();
            ControlTokens = new Dictionary<string, ThemeControlTokenDefinition>(StringComparer.Ordinal);
            SharedTokens  = new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public ThemeDefinition Build()
        {
            return new ThemeDefinition(
                Id,
                DisplayName,
                IsDefault,
                Algorithms,
                SharedTokens,
                ControlTokens);
        }
    }

    private sealed class ControlTokenDefinitionBuilder
    {
        public string TokenId { get; set; } = string.Empty;
        public bool EnableAlgorithm { get; set; }
        public Dictionary<string, string> Tokens { get; } = new(StringComparer.Ordinal);
        public Dictionary<string, string> SharedTokens { get; } = new(StringComparer.Ordinal);

        public ThemeControlTokenDefinition Build()
        {
            return new ThemeControlTokenDefinition(
                TokenId,
                EnableAlgorithm,
                Tokens,
                SharedTokens);
        }
    }
}
