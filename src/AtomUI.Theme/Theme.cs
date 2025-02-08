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
    public static readonly IList<string> SUPPORTED_ALGORITHMS;

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
        SUPPORTED_ALGORITHMS = new List<string>
        {
            DefaultThemeVariantCalculator.ID,
            DarkThemeVariantCalculator.ID,
            CompactThemeVariantCalculator.ID
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

    public List<string> ThemeResourceKeys
    {
        get { return ResourceDictionary.Keys.Select(s => s.ToString()!).ToList(); }
    }

    internal void Load()
    {
        try
        {
            ThemeDefinition = new ThemeDefinition(_id);
            NotifyLoadThemeDef();
            var themeDef           = ThemeDefinition;
            var sharedTokenConfig  = themeDef.SharedTokens;
            var controlTokenConfig = themeDef.ControlTokens;
            CheckAlgorithmNames(themeDef.Algorithms);

            if (!themeDef.Algorithms.Contains(DefaultThemeVariantCalculator.ID))
            {
                themeDef.Algorithms.Insert(0, DefaultThemeVariantCalculator.ID);
            }
            else if (themeDef.Algorithms.Contains(DefaultThemeVariantCalculator.ID) &&
                     themeDef.Algorithms[0] != DefaultThemeVariantCalculator.ID)
            {
                themeDef.Algorithms.Remove(DefaultThemeVariantCalculator.ID);
                themeDef.Algorithms.Insert(0, DefaultThemeVariantCalculator.ID);
            }

            if (themeDef.Algorithms.Contains(DarkThemeVariantCalculator.ID))
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

            // TODO 先用算法，然后再设置配置文件中的值，不知道合理不
            _sharedToken.LoadConfig(sharedTokenConfig);
            _sharedToken.BuildResourceDictionary(ResourceDictionary);

            CollectControlTokens();
            foreach (var entry in ControlTokens)
            {
                // 如果没有修改就使用全局的
                entry.Value.AssignSharedToken(_sharedToken);
            }

            foreach (var entry in controlTokenConfig)
            {
                var tokenId           = entry.Key;
                var catalog           = entry.Value.Catalog;
                var qualifiedTokenKey = GenerateTokenQualifiedKey(tokenId, catalog);
                var controlTokenInfo  = entry.Value;
                if (!ControlTokens.ContainsKey(qualifiedTokenKey))
                {
                    continue;
                }

                // 需要 Review
                var controlAliasToken = (DesignToken)_sharedToken.Clone();
                controlAliasToken.LoadConfig(controlTokenInfo.Tokens);

                if (controlTokenInfo.EnableAlgorithm)
                {
                    ThemeVariantCalculator?.Calculate(controlAliasToken);
                    controlAliasToken.CalculateAliasTokenValues();
                }

                var controlToken = ControlTokens[qualifiedTokenKey];
                controlToken.AssignSharedToken(controlAliasToken);
                (controlToken as AbstractControlDesignToken)!.IsCustomTokenConfig = true;
                (controlToken as AbstractControlDesignToken)!.CustomTokens = controlTokenInfo.Tokens.Keys.ToList();
            }
            
            foreach (var controlToken in ControlTokens.Values)
            {
                (controlToken as AbstractControlDesignToken)!.CalculateFromAlias();
                var controlTokenType  = controlToken.GetType();
                var tokenAttr         = controlTokenType.GetCustomAttribute<ControlDesignTokenAttribute>();
                var qualifiedTokenKey = GenerateTokenQualifiedKey(controlToken.Id, tokenAttr?.ResourceCatalog);
                if (controlTokenConfig.ContainsKey(qualifiedTokenKey))
                {
                    (controlToken as AbstractControlDesignToken)!.LoadConfig(controlTokenConfig[qualifiedTokenKey]
                        .Tokens);
                }

                controlToken.BuildResourceDictionary(ResourceDictionary);
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

    protected void CheckAlgorithmNames(IList<string> algorithms)
    {
        foreach (var algorithm in algorithms)
        {
            if (!SUPPORTED_ALGORITHMS.Contains(algorithm))
            {
                throw new ThemeLoadException(
                    $"Algorithm: {algorithm} is not supported. Supported algorithms are: {string.Join(',', SUPPORTED_ALGORITHMS)}.");
            }
        }
    }

    protected IThemeVariantCalculator CreateThemeVariantCalculator(string algorithmId,
        IThemeVariantCalculator? baseAlgorithm)
    {
        IThemeVariantCalculator calculator;
        if (algorithmId == DefaultThemeVariantCalculator.ID)
        {
            calculator = new DefaultThemeVariantCalculator();
        }
        else if (algorithmId == DarkThemeVariantCalculator.ID)
        {
            calculator = new DarkThemeVariantCalculator(baseAlgorithm!);
        }
        else if (algorithmId == CompactThemeVariantCalculator.ID)
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
                var qualifiedKey = GenerateTokenQualifiedKey(controlToken.Id, attr.ResourceCatalog);
                ControlTokens.Add(qualifiedKey, controlToken);
            }
        }
    }

    private string GenerateTokenQualifiedKey(string tokenId, string? catalog)
    {
        var qualifiedPrefix = "";
        if (!string.IsNullOrEmpty(catalog))
        {
            qualifiedPrefix += $"{catalog}{TokenResourceKey.CatalogSeparator}";
        }

        return $"{qualifiedPrefix}{tokenId}";
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