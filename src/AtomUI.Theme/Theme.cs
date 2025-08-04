using System.Diagnostics;
using System.Reflection;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme;

/// <summary>
/// 主要是生成主题资源，绘制相关的管理不在这里，因为是公用的所以放在 ThemeManager 里面
/// </summary>
public class Theme : ITheme
{
    public static readonly IList<ThemeAlgorithm> SUPPORTED_ALGORITHMS;

    private string _id;
    private string? _loadErrorMsg;
    private ThemeVariant _themeVariant;
    private DesignToken _sharedToken;

    protected bool Loaded;
    protected bool LoadedStatus = true;
    protected bool Activated;
    protected IThemeVariantCalculator? ThemeVariantCalculator;
    protected ThemeDefinition? ThemeDefinition;
    protected ResourceDictionary ResourceDictionary;
    protected Dictionary<string, IControlDesignToken> ControlTokens;

    public string DefinitionFilePath { get; }

    public string Id => _id;
    public string DisplayName => string.Empty;
    public bool LoadStatus => LoadedStatus;
    public string? LoadErrorMsg => _loadErrorMsg;
    public bool IsLoaded => Loaded;
    public ThemeVariant ThemeVariant => _themeVariant;
    internal ResourceDictionary ThemeResource => ResourceDictionary;
    public bool IsDarkMode { get; protected set; }
    public bool IsActivated => Activated;

    public DesignToken SharedToken => _sharedToken;

    static Theme()
    {
        SUPPORTED_ALGORITHMS = new List<ThemeAlgorithm>
        {
            DefaultThemeVariantCalculator.Algorithm,
            DarkThemeVariantCalculator.Algorithm,
            CompactThemeVariantCalculator.Algorithm
        };
    }

    public Theme(string id, string defFilePath)
    {
        _id                                               = id;
        DefinitionFilePath                                = defFilePath;
        _themeVariant                                     = ThemeVariant.Default;
        ResourceDictionary                                = new ResourceDictionary();
        (ResourceDictionary as IThemeVariantProvider).Key = _themeVariant;
        _sharedToken                                      = new DesignToken();
        ControlTokens                                     = new Dictionary<string, IControlDesignToken>();
    }

    public List<string> ThemeResourceKeys => ResourceDictionary.Keys.Select(s => s.ToString()!).ToList();

    internal void Load()
    {
        try
        {
            ThemeDefinition = new ThemeDefinition(_id);
            NotifyLoadThemeDef();
            var themeDef           = ThemeDefinition;
            var sharedTokenConfig  = themeDef.SharedTokens;
            var controlTokenConfig = themeDef.ControlTokens;

            if (!themeDef.Algorithms.Contains(DefaultThemeVariantCalculator.Algorithm))
            {
                themeDef.Algorithms.Insert(0, DefaultThemeVariantCalculator.Algorithm);
            }
            else if (themeDef.Algorithms.Contains(DefaultThemeVariantCalculator.Algorithm) &&
                     themeDef.Algorithms[0] != DefaultThemeVariantCalculator.Algorithm)
            {
                themeDef.Algorithms.Remove(DefaultThemeVariantCalculator.Algorithm);
                themeDef.Algorithms.Insert(0, DefaultThemeVariantCalculator.Algorithm);
            }

            if (themeDef.Algorithms.Contains(DarkThemeVariantCalculator.Algorithm))
            {
                IsDarkMode    = true;
                _themeVariant = ThemeVariant.Dark;
            }
            else
            {
                IsDarkMode    = false;
                _themeVariant = ThemeVariant.Light;
            }

            IThemeVariantCalculator? baseCalculator = null;
            IThemeVariantCalculator? calculator     = null;
            foreach (var algorithmId in themeDef.Algorithms)
            {
                calculator     = CreateThemeVariantCalculator(algorithmId, baseCalculator);
                baseCalculator = calculator;
            }

            Debug.Assert(calculator != null);
            ThemeVariantCalculator = calculator;
            _sharedToken.LoadConfig(sharedTokenConfig);

            ThemeVariantCalculator?.Calculate(_sharedToken);

            // 交付最终的基础色
            _sharedToken.ColorBgBase   = ThemeVariantCalculator?.ColorBgBase;
            _sharedToken.ColorTextBase = ThemeVariantCalculator?.ColorTextBase;

            _sharedToken.CalculateAliasTokenValues();
            _sharedToken.BuildResourceDictionary(ResourceDictionary);

            CollectControlTokens();
            foreach (var entry in ControlTokens)
            {
                // 如果没有修改就使用全局的
                entry.Value.AssignSharedToken(_sharedToken);
            }

            foreach (var entry in controlTokenConfig)
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
                    ThemeVariantCalculator?.Calculate(copiedSharedToken);
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
                
                if (controlTokenConfig.TryGetValue(qualifiedTokenKey, out var tokenInfo))
                {
                    controlToken.LoadConfig(tokenInfo.Tokens);
                }

                controlToken.BuildResourceDictionary(ResourceDictionary);
                if (controlToken.HasCustomTokenConfig())
                {
                    controlToken.BuildSharedResourceDeltaDictionary(_sharedToken);
                }
            }

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

    internal static List<ThemeAlgorithm> CheckAlgorithmNames(IList<string> algorithmNames)
    {
        var algorithms = new List<ThemeAlgorithm>();
        foreach (var algorithmName in algorithmNames)
        {
            if (Enum.TryParse<ThemeAlgorithm>(algorithmName, out var algorithm))
            {
                if (!SUPPORTED_ALGORITHMS.Contains(algorithm))
                {
                    throw new ThemeLoadException(
                        $"Algorithm: {algorithm} is not supported. Supported algorithms are: {string.Join(',', SUPPORTED_ALGORITHMS)}.");
                }
                algorithms.Add(algorithm);
            }
        }
        return algorithms;
    }

    internal static IThemeVariantCalculator CreateThemeVariantCalculator(ThemeAlgorithm algorithmId, IThemeVariantCalculator? baseAlgorithm)
    {
        IThemeVariantCalculator calculator;
        if (algorithmId == DefaultThemeVariantCalculator.Algorithm)
        {
            calculator = new DefaultThemeVariantCalculator();
        }
        else if (algorithmId == DarkThemeVariantCalculator.Algorithm)
        {
            calculator = new DarkThemeVariantCalculator(baseAlgorithm!);
        }
        else if (algorithmId == CompactThemeVariantCalculator.Algorithm)
        {
            calculator = new CompactThemeVariantCalculator(baseAlgorithm!);
        }
        else
        {
            throw new ThemeLoadException($"Algorithm: {algorithmId} is not supported.");
        }

        return calculator;
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

    internal virtual void NotifyResetLoadStatus()
    {
    }

    internal virtual void NotifyLoadThemeDef()
    {
        var reader = new ThemeDefinitionReader(this);
        reader.Load(ThemeDefinition!);
    }

    internal virtual void NotifyRegistered()
    {
    }
}