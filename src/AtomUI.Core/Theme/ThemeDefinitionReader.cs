using System.Diagnostics;
using System.Xml;
using AtomUI.Theme.TokenSystem;
using Avalonia.Platform;

namespace AtomUI.Theme;

internal class ThemeDefinitionReader
{
    private readonly Theme _theme;
    private ThemeDefinition? _currentDef;
    private bool _parseFinished;

    // 上下文信息
    private readonly Stack<string> _currentElementNames;
    private ControlTokenConfigInfo? _currentControlToken;
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

    public void Load(ThemeDefinition themeDefinition)
    {
        try
        {
            _currentDef = themeDefinition;
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
        }
        finally
        {
            _currentControlToken = null;
            _currentElementNames.Clear();
            _inControlTokenCtx = false;
            _inSharedTokenCtx  = false;
            _currentDef        = null;
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
            _currentDef?.SharedTokens.Clear();
            _inSharedTokenCtx  = true;
            _inControlTokenCtx = false;
        }
        else if (name == ControlTokensElementName)
        {
            _currentDef?.ControlTokens.Clear();
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
            var tokenId = _currentControlToken!.TokenId;
            _currentDef?.ControlTokens.Add(tokenId, _currentControlToken!);
            _currentControlToken = null;
            _inControlTokenCtx   = false;
        }
        else if (name == SharedTokensElementName)
        {
            _inSharedTokenCtx = false;
        }

        _parseFinished = name == ThemeElementName;
    }

    private void HandleStartThemeElement(XmlReader reader)
    {
        Debug.Assert(_currentDef != null);
        var displayName = reader.GetAttribute(NameAttrName);
        if (string.IsNullOrWhiteSpace(displayName))
        {
            EmitRequiredAttrError(reader, NameAttrName);
        }
        else
        {
            _currentDef.DisplayName = displayName;
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
                _currentDef.IsDefault = true;
            }
            else
            {
                _currentDef.IsDefault = false;
            }
        }
    }

    private void HandleStartAlgorithmsElement(XmlReader reader)
    {
        Debug.Assert(_currentDef != null);
        // 这样处理方便一点
        var algorithmsStr = reader.ReadElementContentAsString();
        _currentDef.Algorithms = Theme.CheckAlgorithmNames(SplitDistinctAlgorithmNames(algorithmsStr));
    }

    private void HandleStartControlTokenElement(XmlReader reader)
    {
        _currentControlToken = new ControlTokenConfigInfo();
        var tokenId = reader.GetAttribute(IdAttrName);
        if (string.IsNullOrWhiteSpace(tokenId))
        {
            EmitRequiredAttrError(reader, IdAttrName);
        }

        _currentControlToken.TokenId = tokenId!;
        var useAlgorithm = false;
        var algorithm    = reader.GetAttribute(AlgorithmAttrName);
        if (algorithm is not null)
        {
            if (IsTrueValue(algorithm))
            {
                useAlgorithm = true;
            }
        }
        
        _currentControlToken.EnableAlgorithm = useAlgorithm;
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
            _currentDef!.SharedTokens.Add(tokenName!, tokenValue);
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
                _currentControlToken!.SharedTokens.Add(tokenName!, tokenValue);
            }
            else
            {
                _currentControlToken!.Tokens.Add(tokenName!, tokenValue);
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
}
