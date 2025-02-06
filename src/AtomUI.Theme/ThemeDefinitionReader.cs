using System.Xml;
using AtomUI.Theme.TokenSystem;
using Avalonia.Platform;

namespace AtomUI.Theme;

internal class ThemeDefinitionReader
{
    private readonly StaticTheme _theme;
    private ThemeDefinition? _currentDef;
    private bool _parseFinished;

    // 上下文信息
    private readonly Stack<string> _currentElementNames;
    private ControlTokenConfigInfo? _currentControlToken;
    private bool _inGlobalTokenCtx;
    private bool _inControlTokenCtx;

    private const string ThemeElementName = "Theme";
    private const string AlgorithmsElementName = "Algorithms";
    private const string GlobalTokensElementName = "GlobalTokens";
    private const string ControlTokensElementName = "ControlTokens";
    private const string ControlTokenElementName = "ControlToken";
    private const string TokenElementName = "Token";

    private const string NameAttrName = "Name";
    private const string IdAttrName = "Id";
    private const string AlgorithmAttrName = "Algorithm";
    private const string ValueAttrName = "Value";

    public ThemeDefinitionReader(StaticTheme theme)
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
            _inGlobalTokenCtx  = false;
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
        else if (name == GlobalTokensElementName)
        {
            _currentDef?.GlobalTokens.Clear();
            _inGlobalTokenCtx  = true;
            _inControlTokenCtx = false;
        }
        else if (name == ControlTokensElementName)
        {
            _currentDef?.ControlTokens.Clear();
        }
        else if (name == ControlTokenElementName)
        {
            _inGlobalTokenCtx  = false;
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
            EmitErrorMsg(reader, $"Element tag: {string.Join('.', _currentElementNames)} not supported.");
        }
    }

    private void HandleEndElement(string name)
    {
        _currentElementNames.Pop();
        if (name == ControlTokenElementName)
        {
            _currentDef?.ControlTokens.Add(_currentControlToken!.TokenId, _currentControlToken);
            _currentControlToken = null;
            _inControlTokenCtx   = false;
        }
        else if (name == GlobalTokensElementName)
        {
            _inGlobalTokenCtx = false;
        }

        _parseFinished = name == ThemeElementName;
    }

    private void HandleStartThemeElement(XmlReader reader)
    {
        var displayName = reader.GetAttribute(NameAttrName);
        if (displayName is null)
        {
            EmitRequiredAttrError(reader, NameAttrName);
        }
        else
        {
            _currentDef!.DisplayName = displayName;
        }
    }

    private void HandleStartAlgorithmsElement(XmlReader reader)
    {
        // 这样处理方便一点
        var algorithmsStr = reader.ReadElementContentAsString();
        var algorithms    = algorithmsStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
        _currentDef!.Algorithms = algorithms.Select(s => s.Trim()).Distinct().ToList();
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
            algorithm = algorithm.Trim().ToLower();
            if (algorithm == "true")
            {
                useAlgorithm = true;
            }
        }

        _currentControlToken.UseAlgorithm = useAlgorithm;
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

        if (_inGlobalTokenCtx)
        {
            _currentDef!.GlobalTokens.Add(tokenName!, tokenValue);
        }
        else if (_inControlTokenCtx)
        {
            _currentControlToken!.ControlTokens.Add(tokenName!, tokenValue);
        }
        else
        {
            EmitErrorMsg(reader, "The Token element must appear under WidgetToken or GlobalTokens.");
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
}