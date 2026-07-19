using System.Diagnostics;
using AtomUI.Generated.AtomUI_Core;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Language;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;

namespace AtomUI.Theme;

internal class ThemeManager : Styles, IThemeManager, ILanguageManager
{
    private static readonly LanguageVariant s_defaultLanguage = LanguageVariant.zh_CN;

    public static readonly StyledProperty<LanguageVariant> LanguageVariantProperty = 
        LanguageVariant.LanguageVariantProperty.AddOwner<ThemeManager>();
    
    public LanguageVariant LanguageVariant
    {
        get => GetValue(LanguageVariantProperty);
        set => SetValue(LanguageVariantProperty, value);
    }
    
    public FontFamily? FontFamily { get; internal set; }
    internal ThemeSnapshot? CurrentSnapshot => Volatile.Read(ref _currentSnapshot);
    public ThemeState? CurrentTheme => Volatile.Read(ref _currentTheme);
    public IReadOnlyList<ThemeInfo> AvailableThemes =>
        _compiledThemeCatalog?.AvailableThemes ?? Array.Empty<ThemeInfo>();

    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
    public event EventHandler<ThemeChangeFailedEventArgs>? ThemeChangeFailed;
    public event EventHandler<LanguageVariantChangedEventArgs>? LanguageVariantChanged;

    private readonly List<ControlTokenDescriptor> _controlTokenDescriptors;
    private readonly List<IControlThemesProvider> _controlThemesProviders;
    private ThemeCompiler? _themeCompiler;
    private ThemeSnapshotCache? _themeSnapshotCache;
    private readonly object _transactionGate;
    private readonly SemaphoreSlim _transactionExecutionGate;
    private readonly Func<bool> _checkTransitionAccess;
    private readonly ThemePrepareDelegate _prepareTheme;
    private ThemeTransaction? _activeTransaction;
    private ThemeTransaction? _queuedTransaction;
    private ThemeRequestCacheKey? _lastCommittedRequestKey;
    private ThemeSnapshot? _currentSnapshot;
    private ThemeSnapshotCacheKey? _currentSnapshotKey;
    private ThemeTokenResourceProvider? _rootTokenResourceProvider;
    private ThemeContext? _rootContext;
    private readonly ThemeScopeGraph _scopeGraph;
    private ThemeState? _currentTheme;
    private Application? _application;
    private ThemeSchemaRegistry? _startupRegistry;
    private CompiledThemeCatalog? _compiledThemeCatalog;
    private ThemeRequest _initialRequest = new(
        IThemeManager.DEFAULT_THEME_ID,
        null,
        ThemeTransitionReason.Startup);
    private ThemeRequest? _followSystemLightRequest;
    private ThemeRequest? _followSystemDarkRequest;
    private bool _applicationInitialized;
    private long _generation;
    private long _nextTransitionId;
    
    private readonly Dictionary<LanguageVariant, ResourceDictionary> _languages;
    private List<ILanguageProvider>? _languageProviders;
    
    internal ThemeManager(
        Func<bool>? themeTransitionAccessCheck = null,
        ThemePrepareDelegate? prepareTheme = null)
    {
        _controlTokenDescriptors = new List<ControlTokenDescriptor>();
        _controlThemesProviders  = new List<IControlThemesProvider>();
        _languageProviders       = new List<ILanguageProvider>();
        _languages               = new Dictionary<LanguageVariant, ResourceDictionary>();
        _transactionGate         = new object();
        _transactionExecutionGate = new SemaphoreSlim(1, 1);
        _scopeGraph              = new ThemeScopeGraph(this);
        _checkTransitionAccess   = themeTransitionAccessCheck ?? (static () => Dispatcher.UIThread.CheckAccess());
        _prepareTheme            = prepareTheme ?? PrepareThemeAsync;
    }

    internal ThemeContext RootContext => _rootContext ??
        throw new InvalidOperationException("The root ThemeContext has not been initialized.");

    internal ThemeScopeGraph ScopeGraph => _scopeGraph;
    internal void ConfigureStartup(
        ThemeRequest initialRequest,
        ThemeRequest? followSystemLightRequest,
        ThemeRequest? followSystemDarkRequest)
    {
        ArgumentNullException.ThrowIfNull(initialRequest);
        if ((followSystemLightRequest is null) != (followSystemDarkRequest is null))
        {
            throw new ArgumentException(
                "FollowSystem requires both Light and Dark requests.");
        }

        _initialRequest            = initialRequest;
        _followSystemLightRequest  = followSystemLightRequest;
        _followSystemDarkRequest   = followSystemDarkRequest;
    }

    internal void InitializeApplication(
        Application application,
        ThemeAppearance? initialSystemAppearance = null)
    {
        ArgumentNullException.ThrowIfNull(application);
        if (_applicationInitialized)
        {
            throw new InvalidOperationException("ThemeManager is already initialized.");
        }

        _application = application;
        _startupRegistry = CreateStartupRegistry();
        _compiledThemeCatalog = CompiledThemeCatalog.LoadBuiltIn(_startupRegistry);
        MountStaticResources();

        var request = _followSystemLightRequest is null
            ? _initialRequest
            : (initialSystemAppearance ?? ResolveSystemAppearance(application)) == ThemeAppearance.Dark
                ? _followSystemDarkRequest!
                : _followSystemLightRequest;
        var result = ApplyThemeAsync(request).GetAwaiter().GetResult();
        if (result.Status is not ThemeTransitionStatus.Committed and not ThemeTransitionStatus.NoOp)
        {
            var exceptionMessage = result.Exception is null
                ? string.Empty
                : $" {result.Exception.GetBaseException().Message}";
            throw new ThemeLoadException(
                $"Initial theme '{request.ThemeId}' failed with status '{result.Status}': " +
                string.Join(" ", result.Diagnostics.Select(static diagnostic => diagnostic.Message)) +
                exceptionMessage,
                result.Exception);
        }

        var rootContextStyle = new Style(selector => selector.OfType<TopLevel>());
        rootContextStyle.Setters.Add(new Setter(ThemeScope.ContextProperty, RootContext));
        Add(rootContextStyle);
        application.Styles.Add(this);
        _applicationInitialized = true;
        SubscribeSystemAppearance(application);
    }

    internal Task<ThemeTransitionResult> ApplySystemAppearanceAsync(
        ThemeAppearance appearance,
        CancellationToken cancellationToken = default)
    {
        var request = appearance == ThemeAppearance.Dark
            ? _followSystemDarkRequest
            : _followSystemLightRequest;
        if (request is null)
        {
            throw new InvalidOperationException("FollowSystem is not configured.");
        }
        return ApplyThemeAsync(request, cancellationToken);
    }

    public Task<ThemeTransitionResult> ApplyThemeAsync(
        ThemeRequest request,
        CancellationToken cancellationToken = default)
    {
        VerifyTransitionAccess();
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ThemeId);

        var requestKey = ThemeRequestCacheKey.Create(request);
        ThemeTransaction? transaction;
        ThemeTransitionResult? immediateResult = null;
        var startProcessor = false;
        lock (_transactionGate)
        {
            if (_queuedTransaction is not null &&
                _queuedTransaction.RequestKey.Equals(requestKey))
            {
                transaction = _queuedTransaction;
            }
            else if (_queuedTransaction is null &&
                     _activeTransaction is not null &&
                     _activeTransaction.RequestKey.Equals(requestKey))
            {
                transaction = _activeTransaction;
            }
            else if (_activeTransaction is null &&
                     _lastCommittedRequestKey is not null &&
                     _lastCommittedRequestKey.Equals(requestKey))
            {
                transaction = null;
                immediateResult = new ThemeTransitionResult(
                    NextTransitionId(),
                    ThemeTransitionStatus.NoOp,
                    _currentTheme,
                    Array.Empty<ThemeDiagnostic>(),
                    Array.Empty<ThemeDiagnostic>(),
                    null);
            }
            else
            {
                var generation = ++_generation;
                transaction = new ThemeTransaction(
                    NextTransitionId(),
                    generation,
                    request,
                    requestKey)
                {
                    ScopeCapture = _scopeGraph.CaptureAll()
                };
                if (_activeTransaction is null)
                {
                    _activeTransaction = transaction;
                    startProcessor = true;
                }
                else
                {
                    if (_queuedTransaction is not null)
                    {
                        _queuedTransaction.Complete(CreateSupersededResult(_queuedTransaction));
                    }
                    _queuedTransaction = transaction;
                }
            }
        }

        if (immediateResult is not null)
        {
            return WaitForCallerAsync(Task.FromResult(immediateResult), cancellationToken);
        }

        if (startProcessor)
        {
            _ = ProcessTransactionQueueAsync(transaction!);
        }

        return WaitForCallerAsync(transaction!.Completion, cancellationToken);
    }

    private async Task ProcessTransactionQueueAsync(ThemeTransaction transaction)
    {
        var current = transaction;
        while (true)
        {
            var result = await ExecuteTransactionAsync(current).ConfigureAwait(true);
            ThemeTransaction? next;
            lock (_transactionGate)
            {
                Debug.Assert(ReferenceEquals(_activeTransaction, current));
                next = _queuedTransaction;
                _queuedTransaction = null;
                _activeTransaction = next;
            }

            current.Complete(result);
            if (next is null)
            {
                return;
            }
            current = next;
        }
    }

    private async ValueTask<ThemeTransitionResult> ExecuteTransactionAsync(
        ThemeTransaction transaction)
    {
        await _transactionExecutionGate.WaitAsync().ConfigureAwait(true);
        try
        {
            return await ExecuteTransactionCoreAsync(transaction).ConfigureAwait(true);
        }
        finally
        {
            _transactionExecutionGate.Release();
        }
    }

    private async ValueTask<ThemeTransitionResult> ExecuteTransactionCoreAsync(
        ThemeTransaction transaction)
    {
        var committed = false;
        try
        {
            transaction.Phase = ThemeTransactionPhase.Prepare;
            var preparation = await _prepareTheme(
                transaction.Request,
                CurrentSnapshot,
                CancellationToken.None).ConfigureAwait(true);
            transaction.Preparation = preparation;
            lock (_transactionGate)
            {
                if (!IsTransactionCurrent(transaction))
                {
                    return CreateSupersededResult(transaction);
                }
            }

            if (!preparation.Success)
            {
                return CreateFailedResult(transaction, preparation.Diagnostics, preparation.Exception);
            }

            var scopePreparation = PrepareScopeCapture(
                preparation.Snapshot!,
                transaction.ScopeCapture!,
                useLastValidConfig: true);
            if (!scopePreparation.Success)
            {
                lock (_transactionGate)
                {
                    if (!IsTransactionCurrent(transaction))
                    {
                        return CreateSupersededResult(transaction);
                    }
                }

                return CreateFailedResult(
                    transaction,
                    scopePreparation.Diagnostics,
                    scopePreparation.Exception);
            }
            transaction.PreparedScopes = scopePreparation.Snapshots;

            lock (_transactionGate)
            {
                if (!IsTransactionCurrent(transaction))
                {
                    return CreateSupersededResult(transaction);
                }

                if (_currentSnapshotKey is ThemeSnapshotCacheKey currentKey &&
                    currentKey.Equals(preparation.SnapshotKey))
                {
                    return new ThemeTransitionResult(
                        transaction.TransitionId,
                        ThemeTransitionStatus.NoOp,
                        _currentTheme,
                        preparation.Diagnostics,
                        Array.Empty<ThemeDiagnostic>(),
                        null);
                }
            }

            PrepareCommitPayload(transaction);
            lock (_transactionGate)
            {
                if (!IsTransactionCurrent(transaction))
                {
                    return CreateSupersededResult(transaction);
                }

                transaction.Phase = ThemeTransactionPhase.CommitCore;
                CommitCore(transaction);
                committed = true;
            }

            transaction.Phase = ThemeTransactionPhase.Publish;
            var publishDiagnostics = Publish(transaction);
            return new ThemeTransitionResult(
                transaction.TransitionId,
                ThemeTransitionStatus.Committed,
                transaction.PreparedState,
                preparation.Diagnostics,
                publishDiagnostics,
                null);
        }
        catch (Exception exception)
        {
            if (committed)
            {
                return new ThemeTransitionResult(
                    transaction.TransitionId,
                    ThemeTransitionStatus.Committed,
                    transaction.PreparedState,
                    transaction.Preparation?.Diagnostics ?? Array.Empty<ThemeDiagnostic>(),
                    [new ThemeDiagnostic(
                        "ATMTHM7006",
                        ThemeDiagnosticSeverity.Warning,
                        nameof(ThemeManager),
                        "$",
                        $"Theme publish failed after commit: {exception.GetBaseException().Message}")],
                    null);
            }

            return CreateFailedResult(
                transaction,
                [new ThemeDiagnostic(
                    "ATMTHM7002",
                    ThemeDiagnosticSeverity.Error,
                    nameof(ThemeManager),
                    "$",
                    $"Theme transition failed before commit: {exception.GetBaseException().Message}")],
                exception);
        }
    }

    private void PrepareCommitPayload(ThemeTransaction transaction)
    {
        var snapshot = transaction.Preparation!.Snapshot!;
        transaction.PreparedState = new ThemeState(
            transaction.Request.ThemeId,
            snapshot.EffectiveConfig.Algorithms.Select(static algorithm => algorithm.Id).ToArray(),
            snapshot.Appearance,
            snapshot.ContentFingerprint.Value,
            transaction.TransitionId);
        transaction.AddsResourceProvider = _rootContext is null;
        transaction.PreparedRootContext = _rootContext ??
                                          new ThemeContext(this, snapshot, 0);
        transaction.PreparedResourceProvider = transaction.PreparedRootContext.ResourceProvider;
    }

    private void CommitCore(ThemeTransaction transaction)
    {
        var preparation = transaction.Preparation!;
        var rootContext = transaction.PreparedRootContext!;
        if (!transaction.AddsResourceProvider)
        {
            rootContext.Commit(preparation.Snapshot!);
        }

        foreach (var scope in transaction.PreparedScopes)
        {
            scope.Context.Commit(scope.Snapshot);
        }

        _rootContext               = rootContext;
        _rootTokenResourceProvider = rootContext.ResourceProvider;
        _currentSnapshot           = preparation.Snapshot;
        _currentSnapshotKey        = preparation.SnapshotKey;
        _currentTheme              = transaction.PreparedState;
        _lastCommittedRequestKey   = transaction.RequestKey;
    }

    private IReadOnlyList<ThemeDiagnostic> Publish(ThemeTransaction transaction)
    {
        var diagnostics = new List<ThemeDiagnostic>();
        var snapshot = transaction.Preparation!.Snapshot!;
        var avaloniaVariant = snapshot.Appearance == ThemeAppearance.Dark
            ? Avalonia.Styling.ThemeVariant.Dark
            : Avalonia.Styling.ThemeVariant.Light;

        PublishBoundary(
            () =>
            {
                if (_application is not null)
                {
                    _application.RequestedThemeVariant = avaloniaVariant;
                }
            },
            diagnostics,
            "ApplicationThemeVariant");

        PublishBoundary(
            () =>
            {
                if (transaction.AddsResourceProvider)
                {
                    Resources.MergedDictionaries.Add(transaction.PreparedResourceProvider!);
                    transaction.PreparedRootContext!.Publish(notifyResources: false);
                }
                else
                {
                    transaction.PreparedRootContext!.Publish();
                }
            },
            diagnostics,
            "ThemeResources");

        foreach (var scope in transaction.PreparedScopes)
        {
            if (!_scopeGraph.TryGetNode(scope.Stamp.RegistrationId, out var node) ||
                node!.Stamp != scope.Stamp)
            {
                continue;
            }

            PublishBoundary(
                () => node.Provider.PublishCommittedContext(scope.Context, notifyResources: true),
                diagnostics,
                $"ThemeScope[{scope.Stamp.RegistrationId}]");
        }

        ThemeEventDispatcher.Dispatch(
            ThemeChanged,
            this,
            new ThemeChangedEventArgs(
                transaction.Request,
                transaction.PreparedState!,
                diagnostics),
            diagnostics,
            nameof(ThemeChanged));
        return diagnostics.AsReadOnly();
    }

    private bool IsTransactionCurrent(ThemeTransaction transaction)
    {
        return transaction.Generation == _generation &&
               transaction.ScopeCapture is not null &&
               _scopeGraph.IsCurrent(transaction.ScopeCapture);
    }

    private ThemeTransitionResult CreateFailedResult(
        ThemeTransaction transaction,
        IReadOnlyList<ThemeDiagnostic> diagnostics,
        Exception? exception)
    {
        var publishDiagnostics = new List<ThemeDiagnostic>();
        ThemeEventDispatcher.Dispatch(
            ThemeChangeFailed,
            this,
            new ThemeChangeFailedEventArgs(transaction.Request, diagnostics, exception),
            publishDiagnostics,
            nameof(ThemeChangeFailed));
        return new ThemeTransitionResult(
            transaction.TransitionId,
            ThemeTransitionStatus.Failed,
            _currentTheme,
            diagnostics,
            publishDiagnostics,
            exception);
    }

    private ThemeTransitionResult CreateSupersededResult(ThemeTransaction transaction)
    {
        return new ThemeTransitionResult(
            transaction.TransitionId,
            ThemeTransitionStatus.Superseded,
            _currentTheme,
            transaction.Preparation?.Diagnostics ?? Array.Empty<ThemeDiagnostic>(),
            Array.Empty<ThemeDiagnostic>(),
            null);
    }

    private static Task<ThemeTransitionResult> WaitForCallerAsync(
        Task<ThemeTransitionResult> transaction,
        CancellationToken cancellationToken)
    {
        return cancellationToken.CanBeCanceled
            ? transaction.WaitAsync(cancellationToken)
            : transaction;
    }

    private static void PublishBoundary(
        Action action,
        List<ThemeDiagnostic> diagnostics,
        string source)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            diagnostics.Add(new ThemeDiagnostic(
                "ATMTHM7003",
                ThemeDiagnosticSeverity.Warning,
                source,
                "$",
                $"Theme publish boundary failed: {exception.GetBaseException().Message}"));
        }
    }

    private long NextTransitionId()
    {
        return ++_nextTransitionId;
    }

    private void VerifyTransitionAccess()
    {
        if (!_checkTransitionAccess())
        {
            throw new InvalidOperationException("Theme transitions must be captured on the UI thread.");
        }
    }

    internal void EnsureRegistrationCapacity(
        int controlTokenCount,
        int controlThemesProviderCount,
        int languageProviderCount)
    {
        EnsureListCapacity(_controlTokenDescriptors, controlTokenCount);
        EnsureListCapacity(_controlThemesProviders, controlThemesProviderCount);

        if (_languageProviders is not null)
        {
            EnsureListCapacity(_languageProviders, languageProviderCount);
        }
    }

    private static void EnsureListCapacity<T>(List<T> list, int capacity)
    {
        if (list.Capacity < capacity)
        {
            list.Capacity = capacity;
        }
    }

    internal void RegisterControlThemesProvider(IControlThemesProvider controlThemesProvider)
    {
        _controlThemesProviders.Add(controlThemesProvider);
    }

    internal void RegisterLanguageProvider(ILanguageProvider languageProvider)
    {
        _languageProviders?.Add(languageProvider);
    }

    internal void RegisterControlTokenDescriptor(ControlTokenDescriptor descriptor)
    {
        _controlTokenDescriptors.Add(descriptor);
    }

    internal ThemeScopeRegistration RegisterScope(
        ThemeConfigProvider provider,
        ThemeContext parentContext,
        ThemeConfig config,
        out ThemeScopeUpdateResult result)
    {
        VerifyTransitionAccess();
        var registration = _scopeGraph.Register(provider, parentContext, config);
        var request = CreateScopeRequest(config);
        var capture = _scopeGraph.CaptureSubtree(registration.RegistrationId);
        var preparation = PrepareScopeCapture(
            parentContext.Snapshot,
            capture,
            useLastValidConfig: false);
        if (!preparation.Success)
        {
            result = ThemeScopeUpdateResult.Failed(
                request,
                CurrentTheme ?? throw new InvalidOperationException("Root theme is not initialized."),
                preparation.Diagnostics,
                preparation.Exception);
            return registration;
        }

        if (!_scopeGraph.IsCurrent(capture))
        {
            result = ThemeScopeUpdateResult.Failed(
                request,
                CurrentTheme ?? throw new InvalidOperationException("Root theme is not initialized."),
                [new ThemeDiagnostic(
                    "ATMTHM8001",
                    ThemeDiagnosticSeverity.Error,
                    nameof(ThemeScopeGraph),
                    "$",
                    "Theme scope topology changed before the initial snapshot could be committed.")],
                null);
            return registration;
        }

        foreach (var staged in preparation.Snapshots)
        {
            staged.Context.Commit(staged.Snapshot);
            _scopeGraph.AcceptConfig(staged.Stamp.RegistrationId);
        }
        result = ThemeScopeUpdateResult.Succeeded(
            request,
            CurrentTheme ?? throw new InvalidOperationException("Root theme is not initialized."));
        return registration;
    }

    internal void ReplaceScopeConfig(
        ThemeConfigProvider provider,
        ThemeConfig config)
    {
        VerifyTransitionAccess();
        if (!_scopeGraph.TryGetNode(provider, out var node))
        {
            throw new InvalidOperationException("The ThemeConfigProvider is not registered.");
        }

        _scopeGraph.ReplaceConfig(node!.RegistrationId, config);
        var registrationId = node.RegistrationId;
        var configRevision = node.ConfigRevision;
        long generation;
        lock (_transactionGate)
        {
            generation = _generation;
        }

        _ = ProcessScopeUpdateAsync(provider, registrationId, configRevision, generation);
    }

    private async Task ProcessScopeUpdateAsync(
        ThemeConfigProvider provider,
        long registrationId,
        long configRevision,
        long generation)
    {
        await _transactionExecutionGate.WaitAsync().ConfigureAwait(true);
        try
        {
            lock (_transactionGate)
            {
                if (generation != _generation ||
                    !_scopeGraph.TryGetNode(registrationId, out var current) ||
                    current!.ConfigRevision != configRevision ||
                    !ReferenceEquals(current.Provider, provider))
                {
                    return;
                }
            }

            var result = ApplyScopeConfig(provider, registrationId, generation);
            provider.DispatchResult(result);
        }
        finally
        {
            _transactionExecutionGate.Release();
        }
    }

    private ThemeScopeUpdateResult ApplyScopeConfig(
        ThemeConfigProvider provider,
        long registrationId,
        long generation)
    {
        if (!_scopeGraph.TryGetNode(registrationId, out var node) ||
            !ReferenceEquals(node!.Provider, provider))
        {
            throw new InvalidOperationException("The ThemeConfigProvider is not registered.");
        }

        var config = node.Config;
        var request = CreateScopeRequest(config);
        var capture = _scopeGraph.CaptureSubtree(node.RegistrationId);
        var parentSnapshot = ResolveParentSnapshot(capture.Nodes[0].Stamp.ParentRegistrationId);
        var preparation = PrepareScopeCapture(
            parentSnapshot,
            capture,
            useLastValidConfig: false);
        if (!preparation.Success)
        {
            return ThemeScopeUpdateResult.Failed(
                request,
                CurrentTheme ?? throw new InvalidOperationException("Root theme is not initialized."),
                preparation.Diagnostics,
                preparation.Exception);
        }
        lock (_transactionGate)
        {
            if (generation != _generation || !_scopeGraph.IsCurrent(capture))
            {
                return ThemeScopeUpdateResult.Failed(
                    request,
                    CurrentTheme ?? throw new InvalidOperationException("Root theme is not initialized."),
                    [new ThemeDiagnostic(
                        "ATMTHM8002",
                        ThemeDiagnosticSeverity.Error,
                        nameof(ThemeScopeGraph),
                        "$",
                        "Theme scope changed while its configuration was being prepared.")],
                    null);
            }

            foreach (var staged in preparation.Snapshots)
            {
                staged.Context.Commit(staged.Snapshot);
                _scopeGraph.AcceptConfig(staged.Stamp.RegistrationId);
            }
        }

        var publishDiagnostics = new List<ThemeDiagnostic>();
        foreach (var staged in preparation.Snapshots)
        {
            if (!_scopeGraph.TryGetNode(staged.Stamp.RegistrationId, out var stagedNode))
            {
                continue;
            }
            PublishBoundary(
                () => stagedNode!.Provider.PublishCommittedContext(
                    staged.Context,
                    notifyResources: true),
                publishDiagnostics,
                $"ThemeScope[{staged.Stamp.RegistrationId}]");
        }

        return ThemeScopeUpdateResult.Succeeded(
            request,
            CurrentTheme ?? throw new InvalidOperationException("Root theme is not initialized."),
            publishDiagnostics.AsReadOnly());
    }

    private ScopePreparation PrepareScopeCapture(
        ThemeSnapshot rootParentSnapshot,
        ThemeScopeCapture capture,
        bool useLastValidConfig)
    {
        if (capture.Nodes.Count == 0)
        {
            return ScopePreparation.Succeeded(Array.Empty<ThemeScopeStagedSnapshot>());
        }

        var staged = new ThemeScopeStagedSnapshot[capture.Nodes.Count];
        var snapshots = new Dictionary<long, ThemeSnapshot>(capture.Nodes.Count);
        for (var index = 0; index < capture.Nodes.Count; index++)
        {
            var node = capture.Nodes[index];
            ThemeSnapshot parentSnapshot;
            if (node.Stamp.ParentRegistrationId == 0)
            {
                parentSnapshot = rootParentSnapshot;
            }
            else if (!snapshots.TryGetValue(node.Stamp.ParentRegistrationId, out parentSnapshot!))
            {
                parentSnapshot = ResolveParentSnapshot(node.Stamp.ParentRegistrationId);
            }

            var config = useLastValidConfig ? node.LastValidConfig : node.Config;
            var compileResult = CompileScopeSnapshot(parentSnapshot, config);
            if (!compileResult.Success)
            {
                return ScopePreparation.Failed(
                    ConvertDiagnostics(compileResult.Diagnostics),
                    compileResult.Exception);
            }

            var snapshot = compileResult.Snapshot!;
            staged[index] = new ThemeScopeStagedSnapshot(node.Stamp, node.Context, snapshot);
            snapshots.Add(node.Stamp.RegistrationId, snapshot);
        }
        return ScopePreparation.Succeeded(Array.AsReadOnly(staged));
    }

    private ThemeCompileResult CompileScopeSnapshot(
        ThemeSnapshot parentSnapshot,
        ThemeConfig config)
    {
        var normalized = ThemeConfigNormalizer.Normalize(config, parentSnapshot.Registry);
        if (!normalized.Success)
        {
            return new ThemeCompileResult(null, normalized.Diagnostics, null);
        }

        var defaults = ThemeCompiler.CreateDefinitionDefaults(
            parentSnapshot.Definition,
            parentSnapshot.Registry);
        var effective = ThemeConfigMerger.Merge(
            defaults,
            parentSnapshot.EffectiveConfig,
            normalized.Config!).EffectiveConfig;
        var input = new ThemeCompileInput(
            parentSnapshot.Definition,
            parentSnapshot.DefinitionRevision,
            effective,
            parentSnapshot.Registry,
            parentSnapshot);
        return GetThemeSnapshotCache().GetOrCompile(input, GetThemeCompiler());
    }

    private ThemeSnapshot ResolveParentSnapshot(long parentRegistrationId)
    {
        if (parentRegistrationId == 0)
        {
            return RootContext.Snapshot;
        }
        if (_scopeGraph.TryGetNode(parentRegistrationId, out var parent))
        {
            return parent!.Context.Snapshot;
        }
        throw new InvalidOperationException(
            $"Parent theme scope '{parentRegistrationId}' is not active.");
    }

    private ThemeRequest CreateScopeRequest(ThemeConfig config)
    {
        return new ThemeRequest(
            CurrentTheme?.ThemeId ?? IThemeManager.DEFAULT_THEME_ID,
            config,
            ThemeTransitionReason.LocalConfigChanged);
    }

    private ValueTask<ThemeTransactionPreparation> PrepareThemeAsync(
        ThemeRequest request,
        ThemeSnapshot? currentSnapshot,
        CancellationToken cancellationToken)
    {
        Debug.Assert(cancellationToken == CancellationToken.None);
        if (_compiledThemeCatalog is null || _startupRegistry is null)
        {
            return ValueTask.FromResult(ThemeTransactionPreparation.Failed(
                [new ThemeDiagnostic(
                    "ATMTHM7006",
                    ThemeDiagnosticSeverity.Error,
                    nameof(ThemeManager),
                    "$",
                    "ThemeManager has not initialized its schema registry and compiled catalog.")]));
        }

        return ValueTask.FromResult(PrepareCompiledTheme(request, currentSnapshot));
    }

    private ThemeTransactionPreparation PrepareCompiledTheme(
        ThemeRequest request,
        ThemeSnapshot? currentSnapshot)
    {
        try
        {
            var entry = _compiledThemeCatalog!.Get(request.ThemeId);
            var config = AddDefaultFont(request.Config) ?? new ThemeConfigBuilder().Build();
            var normalized = ThemeConfigNormalizer.Normalize(config, _startupRegistry!);
            if (!normalized.Success)
            {
                return ThemeTransactionPreparation.Failed(
                    ConvertDiagnostics(normalized.Diagnostics));
            }

            var defaults = ThemeCompiler.CreateDefinitionDefaults(
                entry.Definition,
                _startupRegistry!);
            var effective = ThemeConfigMerger.Merge(
                defaults,
                null,
                normalized.Config!).EffectiveConfig;
            var input = new ThemeCompileInput(
                entry.Definition,
                entry.Revision,
                effective,
                _startupRegistry!,
                currentSnapshot);
            var key = ThemeSnapshotCacheKey.Create(input);
            if (_currentSnapshotKey is ThemeSnapshotCacheKey currentKey &&
                currentSnapshot is not null &&
                currentKey.Equals(key))
            {
                return ThemeTransactionPreparation.Succeeded(currentSnapshot, key);
            }

            var result = GetThemeSnapshotCache().GetOrCompile(input, GetThemeCompiler());
            return result.Success
                ? ThemeTransactionPreparation.Succeeded(
                    result.Snapshot!,
                    key,
                    ConvertDiagnostics(result.Diagnostics))
                : ThemeTransactionPreparation.Failed(
                    ConvertDiagnostics(result.Diagnostics),
                    result.Exception);
        }
        catch (Exception exception)
        {
            return ThemeTransactionPreparation.Failed(
                [new ThemeDiagnostic(
                    "ATMTHM7005",
                    ThemeDiagnosticSeverity.Error,
                    nameof(ThemeManager),
                    "$",
                    $"Theme preparation failed: {exception.GetBaseException().Message}")],
                exception);
        }
    }

    private ThemeConfig? AddDefaultFont(ThemeConfig? config)
    {
        const string fontFamilyToken = "FontFamily";
        if (FontFamily is null ||
            config?.Tokens.ContainsKey(fontFamilyToken) == true)
        {
            return config;
        }

        var tokens = config is null
            ? new Dictionary<string, string>(StringComparer.Ordinal)
            : new Dictionary<string, string>(config.Tokens, StringComparer.Ordinal);
        tokens[fontFamilyToken] = FontFamily.ToString();
        return new ThemeConfig(
            config?.Inherit ?? true,
            config?.Algorithms,
            tokens,
            config?.Controls ??
            new Dictionary<Schema.ControlTokenIdentity, ControlThemeConfig>());
    }

    private static IReadOnlyList<ThemeDiagnostic> ConvertDiagnostics(
        IReadOnlyList<Definitions.ThemeDefinitionDiagnostic> diagnostics)
    {
        var converted = new ThemeDiagnostic[diagnostics.Count];
        for (var index = 0; index < converted.Length; index++)
        {
            var diagnostic = diagnostics[index];
            converted[index] = new ThemeDiagnostic(
                diagnostic.Code,
                diagnostic.Severity == Definitions.ThemeDefinitionDiagnosticSeverity.Error
                    ? ThemeDiagnosticSeverity.Error
                    : ThemeDiagnosticSeverity.Warning,
                diagnostic.FilePath,
                diagnostic.Path,
                diagnostic.Message);
        }
        return Array.AsReadOnly(converted);
    }

    private ThemeCompiler GetThemeCompiler()
    {
        return _themeCompiler ??= new ThemeCompiler();
    }

    private ThemeSnapshotCache GetThemeSnapshotCache()
    {
        return _themeSnapshotCache ??= new ThemeSnapshotCache();
    }

    private ResourceDictionary? TryGetLanguageResource(LanguageVariant languageVariant)
    {
        if (_languages.TryGetValue(languageVariant, out var resource))
        {
            return resource;
        }
        return null;
    }

    private void MountStaticResources()
    {
        foreach (var provider in _controlThemesProviders)
        {
            foreach (var resourceProvider in provider.ControlThemes)
            {
                Resources.MergedDictionaries.Add(resourceProvider);
            }
        }
        _controlThemesProviders.Clear();
        BuildLanguageResources();
        SwitchLanguageResource(null, LanguageVariant);
    }

    private ThemeSchemaRegistry CreateStartupRegistry()
    {
        return new ThemeSchemaRegistry(
            GeneratedThemeSchema.GetGlobalTokens(),
            _controlTokenDescriptors,
            GeneratedThemeSchema.GetAlgorithms());
    }

    private static ThemeAppearance ResolveSystemAppearance(Application application)
    {
        if (application.PlatformSettings is { } settings)
        {
            return settings.GetColorValues().ThemeVariant == PlatformThemeVariant.Dark
                ? ThemeAppearance.Dark
                : ThemeAppearance.Light;
        }
        return application.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark
            ? ThemeAppearance.Dark
            : ThemeAppearance.Light;
    }

    private void SubscribeSystemAppearance(Application application)
    {
        if (_followSystemLightRequest is not null && application.PlatformSettings is { } settings)
        {
            settings.ColorValuesChanged += HandleSystemColorValuesChanged;
        }
    }

    private async void HandleSystemColorValuesChanged(object? sender, PlatformColorValues values)
    {
        try
        {
            await ApplySystemAppearanceAsync(
                values.ThemeVariant == PlatformThemeVariant.Dark
                    ? ThemeAppearance.Dark
                    : ThemeAppearance.Light).ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
        }
    }

    private void SwitchLanguageResource(LanguageVariant? oldVariant, LanguageVariant? newVariant)
    {
        if (oldVariant != null)
        {
            var oldResource = TryGetLanguageResource(oldVariant);
            if (oldResource != null)
            {
                Resources.MergedDictionaries.Remove(oldResource);
            }
        }

        newVariant ??= s_defaultLanguage;
        var languageResource = TryGetLanguageResource(newVariant);
        if (_languages.TryGetValue(s_defaultLanguage, out var defaultLang))
        {
            languageResource ??= defaultLang;
        }

        if (languageResource != null && !Resources.MergedDictionaries.Contains(languageResource))
        {
            Resources.MergedDictionaries.Add(languageResource);
        }
    }

    private void BuildLanguageResources()
    {
        if (_languageProviders is not null)
        {
            foreach (var languageProvider in _languageProviders)
            {
                var languageVariant = LanguageVariant.FromCode(languageProvider.LangCode);
                if (!_languages.TryGetValue(languageVariant, out var resourceDictionary))
                {
                    resourceDictionary           = new ResourceDictionary();
                    _languages[languageVariant] = resourceDictionary;
                }

                languageProvider.BuildResourceDictionary(resourceDictionary);
            }

            _languageProviders = null;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == LanguageVariantProperty)
        {
            SwitchLanguageResource(change.OldValue as LanguageVariant, change.NewValue as LanguageVariant);
            LanguageVariantChanged?.Invoke(this, new LanguageVariantChangedEventArgs(LanguageVariant, change.GetOldValue<LanguageVariant>()));
        }
    }

    private sealed class ScopePreparation
    {
        private ScopePreparation(
            IReadOnlyList<ThemeScopeStagedSnapshot> snapshots,
            IReadOnlyList<ThemeDiagnostic> diagnostics,
            Exception? exception)
        {
            Snapshots   = snapshots;
            Diagnostics = diagnostics;
            Exception   = exception;
        }

        internal IReadOnlyList<ThemeScopeStagedSnapshot> Snapshots { get; }
        internal IReadOnlyList<ThemeDiagnostic> Diagnostics { get; }
        internal Exception? Exception { get; }
        internal bool Success => Exception is null &&
                                 Diagnostics.All(static diagnostic =>
                                     diagnostic.Severity != ThemeDiagnosticSeverity.Error);

        internal static ScopePreparation Succeeded(
            IReadOnlyList<ThemeScopeStagedSnapshot> snapshots)
        {
            return new ScopePreparation(
                snapshots,
                Array.Empty<ThemeDiagnostic>(),
                null);
        }

        internal static ScopePreparation Failed(
            IReadOnlyList<ThemeDiagnostic> diagnostics,
            Exception? exception)
        {
            return new ScopePreparation(
                Array.Empty<ThemeScopeStagedSnapshot>(),
                diagnostics,
                exception);
        }
    }
}
