using System.Diagnostics;
using System.Reflection;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace AtomUI.Theme;

using AtomUITheme = Theme;

public class ThemeConfigProvider : Control, IThemeConfigProvider
{
    #region 公共属性定义

    public static readonly StyledProperty<Control?> ContentProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, Control?>(nameof(Content));
    
    public static readonly StyledProperty<List<string>> AlgorithmsProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, List<string>>(nameof(Algorithms));
    
    public static readonly StyledProperty<List<TokenSetter>> SharedTokenSettersProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, List<TokenSetter>>(nameof(SharedTokenSetters));
    
    public static readonly StyledProperty<List<ControlTokenInfoSetter>> ControlTokenInfoSettersProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, List<ControlTokenInfoSetter>>(nameof(ControlTokenInfoSetters));
    
    [Content]
    public Control? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public List<string> Algorithms
    {
        get => GetValue(AlgorithmsProperty);
        set => SetValue(AlgorithmsProperty, value);
    }
    
    public List<TokenSetter> SharedTokenSetters
    {
        get => GetValue(SharedTokenSettersProperty);
        set => SetValue(SharedTokenSettersProperty, value);
    }
    
    public List<ControlTokenInfoSetter> ControlTokenInfoSetters
    {
        get => GetValue(ControlTokenInfoSettersProperty);
        set => SetValue(ControlTokenInfoSettersProperty, value);
    }

    public DesignToken SharedToken => _sharedToken;
    public bool IsDarkMode { get; protected set; } = false;
    
    public Dictionary<string, IControlDesignToken> ControlTokens => _controlTokens;
    public ThemeVariant ThemeVariant => _themeVariant;

    #endregion

    #region 内部属性定义
    
    private List<string> _algorithms;
    private ThemeVariant _themeVariant;
    private DesignToken _sharedToken;
    private Dictionary<string, IControlDesignToken> _controlTokens;
    
    #endregion

    private bool _needCalculateTokenResources = true;

    static ThemeConfigProvider()
    {
        AffectsMeasure<ThemeConfigProvider>(ContentProperty);
        ContentProperty.Changed.AddClassHandler<ThemeConfigProvider>((x, e) => x.ContentChanged(e));
    }

    public ThemeConfigProvider()
    {
        _algorithms              = new List<string>();
        _themeVariant            = ThemeVariant.Default;
        _controlTokens           = new Dictionary<string, IControlDesignToken>();
        _sharedToken             = new DesignToken();
    }

    private void ContentChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldChild = (Control?)e.OldValue;
        var newChild = (Control?)e.NewValue;

        if (oldChild != null)
        {
            ((ISetLogicalParent)oldChild).SetParent(null);
            LogicalChildren.Clear();
            VisualChildren.Remove(oldChild);
        }

        if (newChild != null)
        {
            ((ISetLogicalParent)newChild).SetParent(this);
            VisualChildren.Add(newChild);
            LogicalChildren.Add(newChild);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (_needCalculateTokenResources)
        {
            CalculateTokenResources();
            _needCalculateTokenResources = false;
        }
    }

    private void CalculateTokenResources()
    {
        AtomUITheme.CheckAlgorithmNames(_algorithms);
        if (!_algorithms.Contains(DefaultThemeVariantCalculator.ID))
        {
            _algorithms.Insert(0, DefaultThemeVariantCalculator.ID);
        }
        else if (_algorithms.Contains(DefaultThemeVariantCalculator.ID) &&
                 _algorithms[0] != DefaultThemeVariantCalculator.ID)
        {
            _algorithms.Remove(DefaultThemeVariantCalculator.ID);
            _algorithms.Insert(0, DefaultThemeVariantCalculator.ID);
        }
        
        if (_algorithms.Contains(DarkThemeVariantCalculator.ID))
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
        foreach (var algorithmId in _algorithms)
        {
            calculator     = AtomUITheme.CreateThemeVariantCalculator(algorithmId, baseCalculator);
            baseCalculator = calculator;
        }
        Debug.Assert(calculator != null);

        var sharedTokenConfig = new Dictionary<string, string>();
        foreach (var tokenSetter in SharedTokenSetters)
        {
            sharedTokenConfig.Add(tokenSetter.Key, tokenSetter.Value);
        }
        _sharedToken.LoadConfig(sharedTokenConfig);
        calculator.Calculate(_sharedToken);
        
        // 交付最终的基础色
        _sharedToken.ColorBgBase   = calculator.ColorBgBase;
        _sharedToken.ColorTextBase = calculator.ColorTextBase;
        _sharedToken.CalculateAliasTokenValues();
        
        var resourceDictionary = new ResourceDictionary();
        _sharedToken.BuildResourceDictionary(resourceDictionary);
        
        CollectControlTokens();
        foreach (var entry in ControlTokens)
        {
            // 如果没有修改就使用全局的
            entry.Value.AssignSharedToken(_sharedToken);
        }

        var controlTokenConfig = new Dictionary<string, ControlTokenConfigInfo>();
        foreach (var controlTokenInfoSetter in ControlTokenInfoSetters)
        {
            var key = AtomUITheme.GenerateTokenQualifiedKey(controlTokenInfoSetter.TokenId,
                controlTokenInfoSetter.Catalog);
            var configInfo = new ControlTokenConfigInfo();
            configInfo.Catalog = controlTokenInfoSetter.Catalog;
            configInfo.TokenId = controlTokenInfoSetter.TokenId;
            configInfo.EnableAlgorithm = controlTokenInfoSetter.EnableAlgorithm;
            foreach (var setter in controlTokenInfoSetter.Setters)
            {
                configInfo.Tokens.Add(setter.Key, setter.Value);
            }
            controlTokenConfig.Add(key, configInfo);
        }

        foreach (var entry in controlTokenConfig)
        {
            var tokenId           = entry.Key;
            var catalog           = entry.Value.Catalog;
            var qualifiedTokenKey = AtomUITheme.GenerateTokenQualifiedKey(tokenId, catalog);
            var controlTokenInfo  = entry.Value;
            if (!ControlTokens.ContainsKey(qualifiedTokenKey))
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
        
            var controlToken = (ControlTokens[qualifiedTokenKey] as AbstractControlDesignToken)!;
            controlToken.AssignSharedToken(copiedSharedToken);
            controlToken.SetHasCustomTokenConfig(true);
            controlToken.SetCustomTokens(controlTokenInfo.Tokens.Keys.ToList());
        }

        foreach (var token in ControlTokens.Values)
        {
            var controlToken = (token as AbstractControlDesignToken)!;
            controlToken.CalculateFromAlias();
            var controlTokenType = controlToken.GetType();
            var tokenAttr        = controlTokenType.GetCustomAttribute<ControlDesignTokenAttribute>();
            var qualifiedTokenKey =
                AtomUITheme.GenerateTokenQualifiedKey(controlToken.GetId(), tokenAttr?.ResourceCatalog);
            if (controlTokenConfig.ContainsKey(qualifiedTokenKey))
            {
                controlToken.LoadConfig(controlTokenConfig[qualifiedTokenKey].Tokens);
            }
        
            controlToken.BuildResourceDictionary(resourceDictionary);
            if (controlToken.HasCustomTokenConfig())
            {
                controlToken.BuildSharedResourceDeltaDictionary(_sharedToken);
            }
        }
        
        Resources.ThemeDictionaries.Add(_themeVariant, resourceDictionary);
    }

    private IDictionary<string, string> ExtraSharedTokenInfos(ControlTokenConfigInfo controlTokenConfigInfo)
    {
        var qualifiedKey =
            AtomUITheme.GenerateTokenQualifiedKey(controlTokenConfigInfo.TokenId, controlTokenConfigInfo.Catalog);
        var tokenType  = ControlTokens[qualifiedKey].GetType();
        var tokenInfos = new Dictionary<string, string>();
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
    
    protected void CollectControlTokens()
    {
        _controlTokens.Clear();
        var controlTokenTypes = ThemeManager.Current.ControlTokenTypes;
        foreach (var tokenType in controlTokenTypes)
        {
            var obj = Activator.CreateInstance(tokenType);
            if (obj is AbstractControlDesignToken controlToken)
            {
                var attr = tokenType.GetCustomAttribute<ControlDesignTokenAttribute>();
                Debug.Assert(attr != null);
                var qualifiedKey = AtomUITheme.GenerateTokenQualifiedKey(controlToken.GetId(), attr.ResourceCatalog);
                _controlTokens.Add(qualifiedKey, controlToken);
            }
        }
    }

    public IControlDesignToken? GetControlToken(string tokenId, string? catalog = null)
    {
        var qualifiedKey = AtomUITheme.GenerateTokenQualifiedKey(tokenId, catalog);
        return ControlTokens.GetValueOrDefault(qualifiedKey);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot != null)
        {
            if (change.Property == ControlTokenInfoSettersProperty ||
                change.Property == SharedTokenSettersProperty ||
                change.Property == AlgorithmsProperty)
            {
                CalculateTokenResources();
            }
        }
    }
}