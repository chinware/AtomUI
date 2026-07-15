using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Scope;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AtomUI.Theme;

using AtomUITheme = Theme;

public class ThemeConfigProvider : Control, IThemeConfigProvider
{
    #region 公共属性定义

    public static readonly StyledProperty<Control?> ContentProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, Control?>(nameof(Content));

    public static readonly StyledProperty<AvaloniaList<string>> AlgorithmsProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, AvaloniaList<string>>(nameof(Algorithms));

    public static readonly StyledProperty<AvaloniaList<TokenSetter>> SharedTokenSettersProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, AvaloniaList<TokenSetter>>(nameof(SharedTokenSetters));

    public static readonly StyledProperty<AvaloniaList<ControlTokenInfoSetter>> ControlTokenInfoSettersProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, AvaloniaList<ControlTokenInfoSetter>>(nameof(ControlTokenInfoSetters));

    public static readonly StyledProperty<bool> InheritProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, bool>(nameof(Inherit), true);

    [Content]
    public Control? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public AvaloniaList<string> Algorithms
    {
        get => GetValue(AlgorithmsProperty);
        set => SetValue(AlgorithmsProperty, value);
    }

    public AvaloniaList<TokenSetter> SharedTokenSetters
    {
        get => GetValue(SharedTokenSettersProperty);
        set => SetValue(SharedTokenSettersProperty, value);
    }

    public AvaloniaList<ControlTokenInfoSetter> ControlTokenInfoSetters
    {
        get => GetValue(ControlTokenInfoSettersProperty);
        set => SetValue(ControlTokenInfoSettersProperty, value);
    }

    public bool Inherit
    {
        get => GetValue(InheritProperty);
        set => SetValue(InheritProperty, value);
    }

    public DesignToken SharedToken => _sharedToken;
    public bool IsDarkMode { get; protected set; }
    
    public ThemeVariant ThemeVariant { get; }

    public Dictionary<string, IControlDesignToken> ControlTokens => _controlTokens;

    #endregion

    private DesignToken _sharedToken;
    private Dictionary<string, IControlDesignToken> _controlTokens;
    private ThemeSnapshot? _snapshot;
    private ThemeTokenResourceProvider? _tokenResourceProvider;
    private IDisposable? _parentSnapshotSubscription;
    private readonly CompositeDisposable _configurationSubscriptions;
    private bool _isInitializing;
    private bool _isLogicalAttached;
    private bool _recompileQueued;
    private int _recompileGeneration;
    private int _lifecycleGeneration;
    private static int _idSeed = 1;

    internal event EventHandler<ThemeScopeCompileFailedEventArgs>? ThemeScopeCompileFailed;

    static ThemeConfigProvider()
    {
        AffectsMeasure<ThemeConfigProvider>(ContentProperty);
        ContentProperty.Changed.AddClassHandler<ThemeConfigProvider>((x, e) => x.ContentChanged(e));
    }

    public ThemeConfigProvider()
    {
        _controlTokens                = new Dictionary<string, IControlDesignToken>();
        _sharedToken                  = new DesignToken();
        _configurationSubscriptions   = new CompositeDisposable();
        _isInitializing               = true;
        Algorithms                    = new AvaloniaList<string>();
        SharedTokenSetters            = new AvaloniaList<TokenSetter>();
        ControlTokenInfoSetters       = new AvaloniaList<ControlTokenInfoSetter>();
        ThemeVariant                  = new ThemeVariant($"ThemeConfigProvider-{_idSeed++}", null);
        _isInitializing               = false;
    }

    private void ContentChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var oldChild = (Control?)change.OldValue;
        var newChild = (Control?)change.NewValue;

        if (oldChild != null)
        {
            oldChild.ClearValue(ThemeScope.SnapshotProperty);
            ((ISetLogicalParent)oldChild).SetParent(null);
            LogicalChildren.Clear();
            VisualChildren.Remove(oldChild);
        }

        if (newChild != null)
        {
            ((ISetLogicalParent)newChild).SetParent(this);
            VisualChildren.Add(newChild);
            LogicalChildren.Add(newChild);
            if (_isLogicalAttached && _snapshot is not null)
            {
                newChild.SetValue(ThemeScope.SnapshotProperty, _snapshot);
            }
        }

        ScheduleRecompile();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _isLogicalAttached = true;
        ResetConfigurationSubscriptions();
        _parentSnapshotSubscription ??=
            this.GetObservable(ThemeScope.SnapshotProperty).Subscribe(_ => ScheduleRecompile());
        CompileAndPublishImmediately();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _isLogicalAttached = false;
        CancelScheduledRecompile();
        _parentSnapshotSubscription?.Dispose();
        _parentSnapshotSubscription = null;
        _configurationSubscriptions.Clear();
        ClearContentSnapshot();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (_isInitializing)
        {
            return;
        }

        if (change.Property == AlgorithmsProperty ||
            change.Property == SharedTokenSettersProperty ||
            change.Property == ControlTokenInfoSettersProperty)
        {
            if (_isLogicalAttached)
            {
                ResetConfigurationSubscriptions();
            }
            ScheduleRecompile();
        }
        else if (change.Property == InheritProperty)
        {
            ScheduleRecompile();
        }
    }

    private void ScheduleRecompile()
    {
        if (!_isLogicalAttached)
        {
            return;
        }

        if (_recompileQueued)
        {
            return;
        }

        _recompileQueued = true;
        var recompileGeneration = ++_recompileGeneration;
        var lifecycleGeneration = _lifecycleGeneration;
        Dispatcher.UIThread.Post(() =>
        {
            if (!_recompileQueued ||
                recompileGeneration != _recompileGeneration ||
                lifecycleGeneration != _lifecycleGeneration)
            {
                return;
            }

            _recompileQueued = false;
            CompileAndPublish();
        });
    }

    private void CompileAndPublishImmediately()
    {
        CancelScheduledRecompile();
        CompileAndPublish();
    }

    private void CancelScheduledRecompile()
    {
        _recompileQueued = false;
        _recompileGeneration++;
        _lifecycleGeneration++;
    }

    private void CompileAndPublish()
    {
        var result = CreateCompileResult();
        if (!result.Success)
        {
            ThemeScopeCompileFailed?.Invoke(this, new ThemeScopeCompileFailedEventArgs(result));
            return;
        }

        PublishSnapshot(result.Snapshot!);
    }

    private ThemeCompileResult CreateCompileResult()
    {
        try
        {
            return CompileSnapshot();
        }
        catch (Exception exception)
        {
            return new ThemeCompileResult(
                null,
                Array.Empty<ThemeDefinitionDiagnostic>(),
                exception);
        }
    }

    private ThemeCompileResult CompileSnapshot()
    {
        var algorithms = GetRequestedAlgorithms();
        var definition = new ThemeDefinition(
            ThemeVariant.Key?.ToString() ?? ThemeVariant.ToString(),
            ThemeVariant.Key?.ToString() ?? ThemeVariant.ToString(),
            false,
            algorithms,
            new Dictionary<string, string>(),
            new Dictionary<string, ThemeControlTokenDefinition>());
        var request = new ThemeCompileRequest(
            ThemeVariant.Key?.ToString() ?? ThemeVariant.ToString(),
            definition,
            Inherit ? GetValue(ThemeScope.SnapshotProperty) : null,
            algorithms,
            ReadSharedOverrides(),
            ReadComponentOverrides(),
            ThemeManager.Current?.ControlTokenTypes ?? new List<ControlTokenRegistration>(),
            new Dictionary<string, string>());

        return new ThemeCompiler(ThemeManager.Current?.ThemeVariantCalculatorFactory).Compile(request);
    }

    private IReadOnlyList<ThemeAlgorithm> GetRequestedAlgorithms()
    {
        if (Algorithms.Count == 0)
        {
            return Array.Empty<ThemeAlgorithm>();
        }

        var requested = AtomUITheme.CheckAlgorithmNames(Algorithms);
        var algorithms = new List<ThemeAlgorithm>
        {
            ThemeAlgorithm.Default
        };

        if (requested.Contains(ThemeAlgorithm.Dark))
        {
            algorithms.Add(ThemeAlgorithm.Dark);
        }

        if (requested.Contains(ThemeAlgorithm.Compact))
        {
            algorithms.Add(ThemeAlgorithm.Compact);
        }

        return algorithms;
    }

    private IReadOnlyDictionary<string, string> ReadSharedOverrides()
    {
        var overrides = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var setter in SharedTokenSetters)
        {
            overrides[setter.Key] = setter.Value;
        }

        return overrides;
    }

    private IReadOnlyDictionary<ComponentTokenIdentity, ControlTokenConfigInfo> ReadComponentOverrides()
    {
        var overrides = new Dictionary<ComponentTokenIdentity, ControlTokenConfigInfo>();
        foreach (var infoSetter in ControlTokenInfoSetters)
        {
            if (string.IsNullOrWhiteSpace(infoSetter.TokenId))
            {
                continue;
            }

            var config = new ControlTokenConfigInfo
            {
                TokenId = infoSetter.TokenId,
                EnableAlgorithm = infoSetter.EnableAlgorithm
            };
            foreach (var setter in infoSetter.Setters)
            {
                if (setter is ControlTokenSetter)
                {
                    config.Tokens[setter.Key] = setter.Value;
                }
                else
                {
                    config.SharedTokens[setter.Key] = setter.Value;
                }
            }

            overrides[new ComponentTokenIdentity(null, infoSetter.TokenId)] = config;
        }

        return overrides;
    }

    private void PublishSnapshot(ThemeSnapshot snapshot)
    {
        var sharedToken = DesignTokenClone.DeepClone(snapshot.SharedTokenCore);
        var controlTokens = CreateCompatibilityControlTokenMap(snapshot, sharedToken);

        _snapshot      = snapshot;
        _sharedToken   = sharedToken;
        _controlTokens = controlTokens;
        IsDarkMode     = snapshot.IsDark;

        if (Content is not null)
        {
            Content.SetValue(ThemeScope.SnapshotProperty, snapshot);
        }

        if (_tokenResourceProvider is null)
        {
            _tokenResourceProvider = new ThemeTokenResourceProvider(snapshot);
            Resources.MergedDictionaries.Add(_tokenResourceProvider);
            NotifyContentResourcesChanged();
            return;
        }

        _tokenResourceProvider.PrepareSnapshot(snapshot);
        _tokenResourceProvider.PublishSnapshotChanged();
        NotifyContentResourcesChanged();
    }

    private void NotifyContentResourcesChanged()
    {
        if (Content is IResourceHost resourceHost)
        {
            resourceHost.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Create());
        }
    }

    private static Dictionary<string, IControlDesignToken> CreateCompatibilityControlTokenMap(
        ThemeSnapshot snapshot,
        DesignToken sharedToken)
    {
        var result = new Dictionary<string, IControlDesignToken>(StringComparer.Ordinal);
        foreach (var (identity, component) in snapshot.Components)
        {
            var source = component.ControlTokenCore;
            var token = CloneCompatibilityControlToken(source);
            token.AssignSharedToken(DesignTokenClone.DeepClone(component.EffectiveSharedTokenCore));
            token.SetHasCustomTokenConfig(source.HasCustomTokenConfig());
            token.SetCustomTokens(source.GetCustomTokens().ToList());
            ((AbstractControlDesignToken)token).BuildSharedResourceDeltaDictionary(sharedToken);
            result.TryAdd(identity.TokenId, token);
        }

        return result;
    }

    private static IControlDesignToken CloneCompatibilityControlToken(IControlDesignToken source)
    {
        return (IControlDesignToken)source.Clone();
    }

    private void ResetConfigurationSubscriptions()
    {
        _configurationSubscriptions.Clear();
        Subscribe(Algorithms, HandleCollectionChanged);
        Subscribe(SharedTokenSetters, HandleConfigurationCollectionChanged);
        Subscribe(ControlTokenInfoSetters, HandleConfigurationCollectionChanged);

        foreach (var setter in SharedTokenSetters)
        {
            Subscribe(setter);
        }

        foreach (var infoSetter in ControlTokenInfoSetters)
        {
            Subscribe(infoSetter);
            Subscribe(infoSetter.Setters, HandleConfigurationCollectionChanged);
            foreach (var setter in infoSetter.Setters)
            {
                Subscribe(setter);
            }
        }
    }

    private void Subscribe<T>(AvaloniaList<T> collection, NotifyCollectionChangedEventHandler handler)
    {
        collection.CollectionChanged += handler;
        _configurationSubscriptions.Add(Disposable.Create((collection, handler), static state =>
        {
            state.collection.CollectionChanged -= state.handler;
        }));
    }

    private void Subscribe(TokenSetter setter)
    {
        setter.PropertyChanged += HandleConfigurationObjectChanged;
        _configurationSubscriptions.Add(Disposable.Create(() =>
        {
            setter.PropertyChanged -= HandleConfigurationObjectChanged;
        }));
    }

    private void Subscribe(ControlTokenInfoSetter setter)
    {
        setter.PropertyChanged += HandleConfigurationObjectChanged;
        _configurationSubscriptions.Add(Disposable.Create(() =>
        {
            setter.PropertyChanged -= HandleConfigurationObjectChanged;
        }));
    }

    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ScheduleRecompile();
    }

    private void HandleConfigurationCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ResetConfigurationSubscriptions();
        ScheduleRecompile();
    }

    private void HandleConfigurationObjectChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        ResetConfigurationSubscriptions();
        ScheduleRecompile();
    }

    private void ClearContentSnapshot()
    {
        Content?.ClearValue(ThemeScope.SnapshotProperty);
    }

    public IControlDesignToken? GetControlToken(string tokenId)
    {
        return ControlTokens.GetValueOrDefault(tokenId);
    }
}

internal sealed class ThemeScopeCompileFailedEventArgs : EventArgs
{
    internal ThemeScopeCompileFailedEventArgs(ThemeCompileResult result)
    {
        Diagnostics = result.Diagnostics;
        Exception   = result.Exception;
    }

    public IReadOnlyList<ThemeDefinitionDiagnostic> Diagnostics { get; }
    public Exception? Exception { get; }
}
