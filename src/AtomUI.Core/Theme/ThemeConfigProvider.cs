using System.Diagnostics;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
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
    public bool IsDarkMode { get; protected set; }
    
    public ThemeVariant ThemeVariant { get; }

    public Dictionary<string, IControlDesignToken> ControlTokens => _controlTokens;

    #endregion

    #region 内部属性定义
    
    private DesignToken _sharedToken;
    private Dictionary<string, IControlDesignToken> _controlTokens;
    private static int _idSeed = 1;

    #endregion

    private bool _needCalculateTokenResources = true;

    static ThemeConfigProvider()
    {
        AffectsMeasure<ThemeConfigProvider>(ContentProperty);
        ContentProperty.Changed.AddClassHandler<ThemeConfigProvider>((x, e) => x.ContentChanged(e));
    }

    public ThemeConfigProvider()
    {
        _controlTokens          = new Dictionary<string, IControlDesignToken>();
        _sharedToken            = new DesignToken();
        Algorithms              = new List<string>();
        SharedTokenSetters      = new List<TokenSetter>();
        ControlTokenInfoSetters = new List<ControlTokenInfoSetter>();
        ThemeVariant            = new ThemeVariant($"ThemeConfigProvider-{_idSeed++}", null);
    }

    private void ContentChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var oldChild = (Control?)change.OldValue;
        var newChild = (Control?)change.NewValue;

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

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_needCalculateTokenResources)
        {
            CalculateTokenResources();
            _needCalculateTokenResources = false;
        }
    }

    private void CalculateTokenResources()
    {
        var checkedAlgorithms = AtomUITheme.CheckAlgorithmNames(Algorithms);
        var hasDark    = checkedAlgorithms.Contains(ThemeAlgorithm.Dark);
        var hasCompact = checkedAlgorithms.Contains(ThemeAlgorithm.Compact);
        IsDarkMode = hasDark;

        IThemeVariantCalculator? calculator = ThemeManager.Current?.CreateThemeVariantCalculator(
            ThemeAlgorithm.Default,
            null);
        if (hasDark)
        {
            calculator = ThemeManager.Current?.CreateThemeVariantCalculator(ThemeAlgorithm.Dark, calculator);
        }

        if (hasCompact)
        {
            calculator = ThemeManager.Current?.CreateThemeVariantCalculator(ThemeAlgorithm.Compact, calculator);
        }

        Debug.Assert(calculator != null);
        
        var seedTokenKeys  = DesignToken.GetTokenPropertyNames(DesignTokenKind.Seed);
        var mapTokenKeys   = DesignToken.GetTokenPropertyNames(DesignTokenKind.Map);
        var aliasTokenKeys = DesignToken.GetTokenPropertyNames(DesignTokenKind.Alias);
        
        var sharedTokenConfig = new TokenConfigBuckets();
        foreach (var tokenSetter in SharedTokenSetters)
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

        var resourceDictionary = new ResourceDictionary();
        _sharedToken.BuildResourceDictionary(resourceDictionary);

        CollectControlTokens();
        foreach (var entry in ControlTokens)
        {
            // 如果没有修改就使用全局的
            entry.Value.AssignSharedToken(_sharedToken);
        }

        var controlTokenConfig = new Dictionary<string, ControlTokenConfigInfo>(ControlTokenInfoSetters.Count);
        foreach (var controlTokenInfoSetter in ControlTokenInfoSetters)
        {
            var key        = controlTokenInfoSetter.TokenId;
            var configInfo = new ControlTokenConfigInfo();
            configInfo.TokenId         = controlTokenInfoSetter.TokenId;
            configInfo.EnableAlgorithm = controlTokenInfoSetter.EnableAlgorithm;
            foreach (var setter in controlTokenInfoSetter.Setters)
            {
                if (setter is ControlTokenSetter)
                {
                    configInfo.Tokens.Add(setter.Key, setter.Value);
                }
                else
                {
                    configInfo.SharedTokens.Add(setter.Key, setter.Value);
                }
            }

            controlTokenConfig.Add(key, configInfo);
        }

        foreach (var entry in controlTokenConfig)
        {
            var tokenId          = entry.Key;
            var controlTokenInfo = entry.Value;
            if (!ControlTokens.TryGetValue(tokenId, out var token))
            {
                continue;
            }

            var copiedSharedToken = (DesignToken)_sharedToken.Clone();

            var tokenConfig = new TokenConfigBuckets();
            foreach (var tokenSetter in controlTokenInfo.SharedTokens)
            {
                tokenConfig.AddByTokenName(tokenSetter.Key,
                                           tokenSetter.Value,
                                           seedTokenKeys,
                                           mapTokenKeys,
                                           aliasTokenKeys);
            }

            if (controlTokenInfo.EnableAlgorithm)
            {
                copiedSharedToken.LoadConfig(tokenConfig.Seed);
                calculator.Calculate(copiedSharedToken);
                copiedSharedToken.LoadConfig(tokenConfig.Map);
                copiedSharedToken.CalculateAliasTokenValues();
                copiedSharedToken.LoadConfig(tokenConfig.Alias);
            }
            else
            {
                copiedSharedToken.LoadConfig(tokenConfig.Seed);
                copiedSharedToken.LoadConfig(tokenConfig.Map);
                copiedSharedToken.LoadConfig(tokenConfig.Alias);
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
            if (controlTokenConfig.TryGetValue(controlToken.Id, out var tokenConfigInfo))
            {
                controlToken.LoadConfig(tokenConfigInfo.Tokens);
            }

            controlToken.BuildResourceDictionary(resourceDictionary);
            
            if (controlToken.HasCustomTokenConfig())
            {
                controlToken.BuildSharedResourceDeltaDictionary(_sharedToken);
            }
        }
        
        Resources.MergedDictionaries.Add(resourceDictionary);
    }

    protected void CollectControlTokens()
    {
        _controlTokens.Clear();
        var controlTokenTypes = ThemeManager.Current?.ControlTokenTypes;
        if (controlTokenTypes is null)
        {
            return;
        }

        _controlTokens.EnsureCapacity(controlTokenTypes.Count);
        foreach (var tokenRegistration in controlTokenTypes)
        {
            var obj = Activator.CreateInstance(tokenRegistration.TokenType);
            if (obj is AbstractControlDesignToken controlToken)
            {
                _controlTokens.Add(controlToken.Id, controlToken);
            }
        }
    }

    public IControlDesignToken? GetControlToken(string tokenId)
    {
        return ControlTokens.GetValueOrDefault(tokenId);
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
