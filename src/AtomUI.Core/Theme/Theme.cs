using System.Diagnostics;
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
        _algorithms        = new List<ThemeAlgorithm>(3);
        DefinitionFilePath = defFilePath;
        ResourceDictionary = new ResourceDictionary();
        ControlTokens      = new Dictionary<string, IControlDesignToken>(themeManager.ControlTokenTypes.Count);
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

    public List<string> ThemeResourceKeys
    {
        get
        {
            var keys = new List<string>(ResourceDictionary.Keys.Count);
            foreach (var key in ResourceDictionary.Keys)
            {
                keys.Add(key.ToString()!);
            }

            return keys;
        }
    }

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
                _isPrimary = true;
                foreach (var algorithm in _algorithms)
                {
                    if (!ThemeDefinition.Algorithms.Contains(algorithm))
                    {
                        _isPrimary = false;
                        break;
                    }
                }
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
        
        var seedTokenKeys  = DesignToken.GetTokenPropertyNames(DesignTokenKind.Seed);
        var mapTokenKeys   = DesignToken.GetTokenPropertyNames(DesignTokenKind.Map);
        var aliasTokenKeys = DesignToken.GetTokenPropertyNames(DesignTokenKind.Alias);
        
        var sharedTokenConfig = new TokenConfigBuckets();
        foreach (var tokenSetter in ThemeDefinition.SharedTokens)
        {
            sharedTokenConfig.AddByTokenName(tokenSetter.Key,
                                             tokenSetter.Value,
                                             seedTokenKeys,
                                             mapTokenKeys,
                                             aliasTokenKeys);
        }
        
        _sharedToken.LoadConfig(sharedTokenConfig.Seed);
        // 计算得到 Map Tokens
        calculator.Calculate(_sharedToken);
        // 覆盖 Map Token
        _sharedToken.LoadConfig(sharedTokenConfig.Map);

        // 交付最终的基础色
        _sharedToken.ColorBgBase   = calculator.ColorBgBase;
        _sharedToken.ColorTextBase = calculator.ColorTextBase;

        _sharedToken.CalculateAliasTokenValues();
        
        // 覆盖 Alias Token
        _sharedToken.LoadConfig(sharedTokenConfig.Alias);
        
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
            if (!ControlTokens.TryGetValue(entry.Key, out var token))
            {
                continue;
            }

            var copiedSharedToken = (DesignToken)_sharedToken.Clone();
            
            var controlTokenConfig = new TokenConfigBuckets();
            foreach (var tokenSetter in controlTokenInfo.SharedTokens)
            {
                controlTokenConfig.AddByTokenName(tokenSetter.Key,
                                                  tokenSetter.Value,
                                                  seedTokenKeys,
                                                  mapTokenKeys,
                                                  aliasTokenKeys);
            }
            
            if (controlTokenInfo.EnableAlgorithm)
            {
                copiedSharedToken.LoadConfig(controlTokenConfig.Seed);
                calculator.Calculate(copiedSharedToken);
                copiedSharedToken.LoadConfig(controlTokenConfig.Map);
                copiedSharedToken.CalculateAliasTokenValues();
                copiedSharedToken.LoadConfig(controlTokenConfig.Alias);
            }
            else
            {
                copiedSharedToken.LoadConfig(controlTokenConfig.Seed);
                copiedSharedToken.LoadConfig(controlTokenConfig.Map);
                copiedSharedToken.LoadConfig(controlTokenConfig.Alias);
            }

            var controlToken = (token as AbstractControlDesignToken)!;
            controlToken.AssignSharedToken(copiedSharedToken);
            controlToken.SetHasCustomTokenConfig(true);
            controlToken.SetCustomTokens(new List<string>(controlTokenInfo.Tokens.Keys));
        }

        foreach (var token in ControlTokens.Values)
        {
            var controlToken = (token as AbstractControlDesignToken)!;
            controlToken.CalculateTokenValues(IsDarkMode);
            if (ThemeDefinition.ControlTokens.TryGetValue(controlToken.Id, out var tokenInfo))
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
        var hasDark    = algorithms.Contains(DarkThemeVariantCalculator.Algorithm);
        var hasCompact = algorithms.Contains(CompactThemeVariantCalculator.Algorithm);
        return BuildThemeVariant(id, hasDark, hasCompact);
    }

    internal static ThemeVariant BuildThemeVariant(string id, bool hasDark, bool hasCompact)
    {
        var variantName = id;
        if (hasDark)
        {
            variantName += $"-{nameof(ThemeAlgorithm.Dark)}";
        }

        if (hasCompact)
        {
            variantName += $"-{nameof(ThemeAlgorithm.Compact)}";
        }

        return new ThemeVariant(variantName, null);
    }

    internal static ISet<ThemeAlgorithm> CheckAlgorithmNames(IList<string> algorithmNames)
    {
        var algorithms = new HashSet<ThemeAlgorithm>(algorithmNames.Count);
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
        var controlTokenTypes = ThemeManager.Current?.ControlTokenTypes;
        if (controlTokenTypes is null)
        {
            return;
        }

        ControlTokens.EnsureCapacity(controlTokenTypes.Count);
        foreach (var tokenType in controlTokenTypes)
        {
            var obj = Activator.CreateInstance(tokenType);
            if (obj is AbstractControlDesignToken controlToken)
            {
                ControlTokens.Add(controlToken.Id, controlToken);
            }
        }
    }

    public IControlDesignToken? GetControlToken(string tokenId)
    {
        return ControlTokens.GetValueOrDefault(tokenId);
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
        Activated = false;
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
