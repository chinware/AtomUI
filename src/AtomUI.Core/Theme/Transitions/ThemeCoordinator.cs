using System.Diagnostics;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Threading;

namespace AtomUI.Theme.Transitions;

internal sealed class ThemeCoordinator
{
    private enum TransitionState
    {
        Idle,
        Preparing,
        Committing
    }

    private readonly ThemeManager _themeManager;
    private readonly Func<bool> _checkAccess;
    private readonly Queue<ThemeRequest> _queuedRequests;
    private TransitionState _state;
    private ThemeRequest? _lastCommittedRequest;
    private Application? _application;

    internal ThemeCoordinator(ThemeManager themeManager)
        : this(themeManager, static () => Dispatcher.UIThread.CheckAccess())
    {
    }

    internal ThemeCoordinator(
        ThemeManager themeManager,
        Func<bool> checkAccess)
    {
        ArgumentNullException.ThrowIfNull(themeManager);
        ArgumentNullException.ThrowIfNull(checkAccess);

        _themeManager   = themeManager;
        _checkAccess    = checkAccess;
        _queuedRequests = new Queue<ThemeRequest>();
        _state          = TransitionState.Idle;
    }

    internal event EventHandler<ThemeTransitionEventArgs>? ThemeChanged;

    internal bool IsCommitting => _state == TransitionState.Committing;

    internal void AttachApplication(Application application)
    {
        _application = application;
    }

    internal void Request(ThemeRequest request)
    {
        VerifyAccess();
        var normalizedRequest = NormalizeRequest(request);
        if (_state != TransitionState.Idle)
        {
            _queuedRequests.Enqueue(normalizedRequest);
            return;
        }

        if (IsNoOp(normalizedRequest))
        {
            return;
        }

        try
        {
            CommitAndDrain(normalizedRequest);
        }
        finally
        {
            _state = TransitionState.Idle;
        }
    }

    private void CommitAndDrain(ThemeRequest request)
    {
        var nextRequest = request;
        while (true)
        {
            if (!IsNoOp(nextRequest))
            {
                Commit(nextRequest);
            }

            if (_queuedRequests.Count == 0)
            {
                return;
            }

            nextRequest = _queuedRequests.Dequeue();
        }
    }

    private void Commit(ThemeRequest request)
    {
        _state = TransitionState.Preparing;
        var themeVariant = Theme.BuildThemeVariant(request.ThemeId, request.Algorithms);
        var theme = _themeManager.LoadTheme(themeVariant, request.RuntimeOverrides);
        var oldTheme = _themeManager.ActivatedTheme;

        _state = TransitionState.Committing;
        _themeManager.CommitActiveTheme(theme);
        if (_application is not null)
        {
            _application.RequestedThemeVariant = theme.ThemeVariant;
        }

        var args = new ThemeTransitionEventArgs(
            request,
            oldTheme,
            theme,
            theme.Snapshot);
        _lastCommittedRequest = request;
        _themeManager.NotifyThemeChanged(theme, oldTheme);
        RaiseThemeChanged(args);
    }

    private bool IsNoOp(ThemeRequest request)
    {
        if (_lastCommittedRequest is not null)
        {
            return RequestsEqual(_lastCommittedRequest, request);
        }

        if (_themeManager.ActivatedTheme is not Theme activeTheme)
        {
            return false;
        }

        return string.Equals(activeTheme.Id, request.ThemeId, StringComparison.Ordinal) &&
               AlgorithmsEqual(activeTheme.Algorithms, request.Algorithms);
    }

    private ThemeRequest NormalizeRequest(ThemeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.ThemeId);
        return request with
        {
            Algorithms       = NormalizeAlgorithms(request.Algorithms),
            RuntimeOverrides = BuildRuntimeOverrides(request.RuntimeOverrides)
        };
    }

    private IReadOnlyDictionary<string, string> BuildRuntimeOverrides(
        IReadOnlyDictionary<string, string>? requestOverrides)
    {
        var overrides = requestOverrides is null
            ? new Dictionary<string, string>(StringComparer.Ordinal)
            : new Dictionary<string, string>(requestOverrides, StringComparer.Ordinal);
        if (_themeManager.FontFamily is not null &&
            !overrides.ContainsKey(nameof(DesignToken.FontFamily)))
        {
            overrides[nameof(DesignToken.FontFamily)] = _themeManager.FontFamily.ToString();
        }

        return overrides;
    }

    private static IReadOnlyList<ThemeAlgorithm> NormalizeAlgorithms(
        IReadOnlyList<ThemeAlgorithm> algorithms)
    {
        ArgumentNullException.ThrowIfNull(algorithms);
        var hasDark    = false;
        var hasCompact = false;
        foreach (var algorithm in algorithms)
        {
            switch (algorithm)
            {
                case ThemeAlgorithm.Default:
                    break;
                case ThemeAlgorithm.Dark:
                    hasDark = true;
                    break;
                case ThemeAlgorithm.Compact:
                    hasCompact = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(algorithms),
                        algorithm,
                        $"Unsupported theme algorithm: {algorithm}.");
            }
        }

        var normalized = new List<ThemeAlgorithm>
        {
            ThemeAlgorithm.Default
        };
        if (hasDark)
        {
            normalized.Add(ThemeAlgorithm.Dark);
        }

        if (hasCompact)
        {
            normalized.Add(ThemeAlgorithm.Compact);
        }

        return normalized;
    }

    private static bool RequestsEqual(ThemeRequest left, ThemeRequest right)
    {
        return string.Equals(left.ThemeId, right.ThemeId, StringComparison.Ordinal) &&
               AlgorithmsEqual(left.Algorithms, right.Algorithms) &&
               DictionaryEqual(left.RuntimeOverrides, right.RuntimeOverrides);
    }

    private static bool AlgorithmsEqual(
        IEnumerable<ThemeAlgorithm> left,
        IReadOnlyList<ThemeAlgorithm> right)
    {
        var index = 0;
        foreach (var algorithm in left)
        {
            if (index >= right.Count || algorithm != right[index])
            {
                return false;
            }
            index++;
        }

        return index == right.Count;
    }

    private static bool DictionaryEqual(
        IReadOnlyDictionary<string, string>? left,
        IReadOnlyDictionary<string, string>? right)
    {
        if (left is null || left.Count == 0)
        {
            return right is null || right.Count == 0;
        }

        if (right is null || left.Count != right.Count)
        {
            return false;
        }

        foreach (var (key, value) in left)
        {
            if (!right.TryGetValue(key, out var rightValue) ||
                !string.Equals(value, rightValue, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private void RaiseThemeChanged(ThemeTransitionEventArgs args)
    {
        var handlers = ThemeChanged;
        if (handlers is null)
        {
            return;
        }

        foreach (EventHandler<ThemeTransitionEventArgs> handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, args);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }

    private void VerifyAccess()
    {
        if (!_checkAccess())
        {
            throw new InvalidOperationException("Theme transitions must be requested on the UI thread.");
        }
    }
}
