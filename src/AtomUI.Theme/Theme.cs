using System.Diagnostics;
using System.Reflection;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme;

/// <summary>
/// 主要是生成主题资源，绘制相关的管理不在这里，因为是公用的所以放在 ThemeManager 里面
/// </summary>
internal class Theme : AvaloniaObject, ITheme
{
    protected bool Loaded;
    protected bool LoadedStatus = true;
    protected bool Activated;

    protected readonly ResourceDictionary ResourceDictionary;
    protected readonly Dictionary<string, IControlDesignToken> ControlTokens;
    internal ThemeDefinition ThemeDefinition;

    public string DefinitionFilePath { get; }

    public string Id => _id;
    public string DisplayName => ThemeDefinition.DisplayName;
    public bool LoadStatus => LoadedStatus;
    public string? LoadErrorMsg => _loadErrorMsg;
    public bool IsLoaded => Loaded;
    public ThemeVariant ThemeVariant => _themeVariant;
    public ResourceDictionary ThemeResource => ResourceDictionary;
    public bool IsDarkMode { get; protected set; }
    public bool IsActivated => Activated;
    public bool IsBuiltIn => _isBuiltIn;

    // 当 request algorithms 跟定义文件加载的一样的时候就是 primary theme
    public bool IsPrimary => _isPrimary;

    public DesignToken SharedToken => _sharedToken;
    public IList<ThemeAlgorithm> Algorithms => _algorithms;
    
    private string _id;
    private string? _loadErrorMsg;
    private ThemeVariant _themeVariant;
    private DesignToken _sharedToken;
    private bool _isBuiltIn;
    private bool _isPrimary;
    private IList<ThemeAlgorithm> _algorithms;
    private ThemeManager _themeManager;

    public Theme(ThemeManager themeManager, string id, string defFilePath, ISet<ThemeAlgorithm> requestAlgorithms, bool isBuiltIn = false)
    {
        _id                = id;
        _isBuiltIn         = isBuiltIn;
        _sharedToken       = new DesignToken();
        _themeVariant      = new ThemeVariant(id, null);
        _algorithms        = new List<ThemeAlgorithm>();
        DefinitionFilePath = defFilePath;
        ResourceDictionary = new ResourceDictionary();
        ControlTokens      = new Dictionary<string, IControlDesignToken>();
        ThemeDefinition    = new ThemeDefinition(_id);
        _algorithms.Add(ThemeAlgorithm.Default);
        if (requestAlgorithms.Contains(ThemeAlgorithm.Dark))
        {
            _algorithms.Add(ThemeAlgorithm.Dark);
        }

        if (requestAlgorithms.Contains(ThemeAlgorithm.Compact))
        {
            _algorithms.Add(ThemeAlgorithm.Compact);
        }

        _themeVariant = BuildThemeVariant(id, _algorithms);
        _themeManager = themeManager;
    }

    public List<string> ThemeResourceKeys => ResourceDictionary.Keys.Select(s => s.ToString()!).ToList();

    internal void Load()
    {
        try
        {
            if (Loaded)
            {
                throw new InvalidOperationException($"Theme: {_id} already loaded");
            }
            NotifyLoadThemeDef();

            if (_algorithms.Count != ThemeDefinition.Algorithms.Count)
            {
                _isPrimary = false;
            }
            else
            {
                _isPrimary = _algorithms.ToHashSet().SetEquals(ThemeDefinition.Algorithms);
            }
            
            BuildThemeResource(_algorithms);

            LoadedStatus = true;
            Loaded       = true;
        }
        catch (Exception exception)
        {
            _loadErrorMsg = exception.Message;
            LoadedStatus  = false;
            throw;
        }
    }

    private void BuildThemeResource(IList<ThemeAlgorithm> algorithms)
    {
        if (algorithms.Contains(DarkThemeVariantCalculator.Algorithm))
        {
            IsDarkMode    = true;
        }
        else
        {
            IsDarkMode    = false;
        }
    
        IThemeVariantCalculator? baseCalculator = null;
        IThemeVariantCalculator? calculator     = null;
        foreach (var algorithmId in algorithms)
        {
            calculator     = _themeManager.CreateThemeVariantCalculator(algorithmId, baseCalculator);
            baseCalculator = calculator;
        }
    
        Debug.Assert(calculator != null);
        _sharedToken.LoadConfig(ThemeDefinition.SharedTokens);

        calculator.Calculate(_sharedToken);

        // 交付最终的基础色
        _sharedToken.ColorBgBase   = calculator.ColorBgBase;
        _sharedToken.ColorTextBase = calculator.ColorTextBase;

        _sharedToken.CalculateAliasTokenValues();
        _sharedToken.BuildResourceDictionary(ResourceDictionary);

        CollectControlTokens();
        foreach (var entry in ControlTokens)
        {
            // 如果没有修改就使用全局的
            entry.Value.AssignSharedToken(_sharedToken);
        }

        foreach (var entry in ThemeDefinition.ControlTokens)
        {
            var controlTokenInfo  = entry.Value;
            if (!ControlTokens.ContainsKey(entry.Key))
            {
                continue;
            }

            var copiedSharedToken = (DesignToken)_sharedToken.Clone();
            copiedSharedToken.LoadConfig(ExtraSharedTokenInfos(controlTokenInfo));

            if (controlTokenInfo.EnableAlgorithm)
            {
                calculator.Calculate(copiedSharedToken);
                copiedSharedToken.CalculateAliasTokenValues();
            }

            var controlToken = (ControlTokens[entry.Key] as AbstractControlDesignToken)!;
            controlToken.AssignSharedToken(copiedSharedToken);
            controlToken.SetHasCustomTokenConfig(true);
            controlToken.SetCustomTokens(controlTokenInfo.Tokens.Keys.ToList());
        }

        foreach (var token in ControlTokens.Values)
        {
            var controlToken = (token as AbstractControlDesignToken)!;
            controlToken.CalculateFromAlias();
            var controlTokenType  = controlToken.GetType();
            var tokenAttr         = controlTokenType.GetCustomAttribute<ControlDesignTokenAttribute>();
            var qualifiedTokenKey = GenerateTokenQualifiedKey(controlToken.GetId(), tokenAttr?.ResourceCatalog);
                
            if (ThemeDefinition.ControlTokens.TryGetValue(qualifiedTokenKey, out var tokenInfo))
            {
                controlToken.LoadConfig(tokenInfo.Tokens);
            }

            controlToken.BuildResourceDictionary(ResourceDictionary);
            if (controlToken.HasCustomTokenConfig())
            {
                controlToken.BuildSharedResourceDeltaDictionary(_sharedToken);
            }
        }
    }

    internal static ThemeVariant BuildThemeVariant(string id, IList<ThemeAlgorithm> algorithms)
    {
        var parts = new List<string>()
        {
            id
        };
        if (algorithms.Contains(DarkThemeVariantCalculator.Algorithm))
        {
            parts.Add(nameof(ThemeAlgorithm.Dark));
        }

        if (algorithms.Contains(CompactThemeVariantCalculator.Algorithm))
        {
            parts.Add(nameof(ThemeAlgorithm.Compact));
        }

        return new ThemeVariant(string.Join("-", parts), null);
    }

    private IDictionary<string, string> ExtraSharedTokenInfos(ControlTokenConfigInfo controlTokenConfigInfo)
    {
        var qualifiedKey = GenerateTokenQualifiedKey(controlTokenConfigInfo.TokenId, controlTokenConfigInfo.Catalog);
        var tokenType    = ControlTokens[qualifiedKey].GetType();
        var tokenInfos   = new Dictionary<string, string>();
        var tokenProperties = tokenType.GetProperties(BindingFlags.Public |
                                                      BindingFlags.NonPublic |
                                                      BindingFlags.Instance |
                                                      BindingFlags.FlattenHierarchy)
                                       .Select(p => p.Name).ToHashSet();
        foreach (var entry in controlTokenConfigInfo.Tokens)
        {
            if (!tokenProperties.Contains(entry.Key))
            {
                tokenInfos.Add(entry.Key, entry.Value);
            }
        }

        return tokenInfos;
    }

    internal static ISet<ThemeAlgorithm> CheckAlgorithmNames(IList<string> algorithmNames)
    {
        var algorithms = new HashSet<ThemeAlgorithm>();
        foreach (var algorithmName in algorithmNames)
        {
            if (!Enum.TryParse<ThemeAlgorithm>(algorithmName, out var algorithm))
            {
                throw new ThemeLoadException(
                    $"Algorithm: {algorithm} is not supported. Supported algorithms are: {ThemeAlgorithm.Default}, {ThemeAlgorithm.Dark}, {ThemeAlgorithm.Compact}.");
            }
            algorithms.Add(algorithm);
        }
        return algorithms;
    }

    protected void CollectControlTokens()
    {
        ControlTokens.Clear();
        var controlTokenTypes = ThemeManager.Current.ControlTokenTypes;
        foreach (var tokenType in controlTokenTypes)
        {
            var obj = Activator.CreateInstance(tokenType);
            if (obj is AbstractControlDesignToken controlToken)
            {
                var attr = tokenType.GetCustomAttribute<ControlDesignTokenAttribute>();
                Debug.Assert(attr != null);
                var qualifiedKey = GenerateTokenQualifiedKey(controlToken.GetId(), attr.ResourceCatalog);
                ControlTokens.Add(qualifiedKey, controlToken);
            }
        }
    }

    internal static string GenerateTokenQualifiedKey(string tokenId, string? catalog)
    {
        var qualifiedPrefix = "";
        if (!string.IsNullOrEmpty(catalog))
        {
            qualifiedPrefix += $"{catalog}{TokenResourceKey.CatalogSeparator}";
        }

        return $"{qualifiedPrefix}{tokenId}";
    }

    public IControlDesignToken? GetControlToken(string tokenId, string? catalog = null)
    {
        var qualifiedKey = GenerateTokenQualifiedKey(tokenId, catalog);
        return ControlTokens.GetValueOrDefault(qualifiedKey);
    }

    internal virtual void NotifyAboutToActive()
    {
    }

    internal virtual void NotifyActivated()
    {
        Activated = true;
    }

    internal virtual void NotifyAboutToDeActive()
    {
    }

    internal virtual void NotifyDeActivated()
    {
        Activated         = false;
    }

    internal virtual void NotifyAboutToLoad()
    {
    }

    internal virtual void NotifyLoaded()
    {
    }

    internal virtual void NotifyAboutToUnload()
    {
    }

    internal virtual void NotifyUnloaded()
    {
    }

    internal virtual void NotifyLoadThemeDef()
    {
        var reader = new ThemeDefinitionReader(this);
        reader.Load(ThemeDefinition);
    }

    internal virtual void NotifyRegistered()
    {
    }
}