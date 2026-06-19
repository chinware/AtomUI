using System.Reactive;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUI.Toolkits.GalleryBase.Navigation;

public class GalleryNavigationViewModel : ReactiveObject, IActivatableViewModel, IDisposable
{
    private readonly GalleryBaseConfiguration _configuration;
    private readonly HashSet<EntityKey>       _routeNodeKeys;
    private readonly DispatcherTimer          _dispatcherTimer;
    private readonly IDisposable              _activationSubscription;
    private bool _isDisposed;
    private bool _isDiagnosticNavigationRunning;
    private bool _shouldResetDeferredLoadingDisabledOnDiagnosticStop;
    private int _diagnosticRouteIndex;

    private EntityKey? _currentRoute;
    private INavMenuNode? _selectedItem;

    public INavMenuNode? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public IScreen HostScreen { get; }

    public ViewModelActivator Activator { get; } = new();

    public ReactiveCommand<EntityKey, Unit> NavigateToCommand { get; }

    public ReactiveCommand<TimeSpan, Unit> TestNavigatePagesCommand { get; }

    public ReactiveCommand<Unit, Unit> StopTestNavigatePagesCommand { get; }

    public GalleryNavigationViewModel(IScreen hostScreen, GalleryBaseConfiguration configuration)
    {
        HostScreen      = hostScreen;
        _configuration  = configuration;
        _routeNodeKeys  = Walk(configuration.NavigationNodes)
            .Where(node => node.IsRoute)
            .Select(node => node.Key)
            .ToHashSet();

        _dispatcherTimer      = new DispatcherTimer();
        _dispatcherTimer.Tick += HandleDiagnosticTimerTick;

        NavigateToCommand           = ReactiveCommand.Create<EntityKey>(DoNavigateTo);
        TestNavigatePagesCommand    = ReactiveCommand.Create<TimeSpan>(DoTestNavigatePages);
        StopTestNavigatePagesCommand = ReactiveCommand.Create(DoStopTestNavigatePages);

        _activationSubscription = Activator.Activated.Subscribe(_ => DoNavigateTo(_configuration.DefaultRoute));
    }

    public bool CanNavigateTo(EntityKey routeKey)
    {
        return _routeNodeKeys.Contains(routeKey) && _configuration.Routes.ContainsRoute(routeKey);
    }

    public virtual void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        StopDiagnosticNavigation();
        _dispatcherTimer.Tick -= HandleDiagnosticTimerTick;
        _activationSubscription.Dispose();
    }

    private void DoNavigateTo(EntityKey routeKey)
    {
        if (_isDisposed)
        {
            return;
        }

        if (!CanNavigateTo(routeKey))
        {
            return;
        }

        if (_currentRoute is not null && _currentRoute.Value == routeKey)
        {
            return;
        }

        _currentRoute = routeKey;
        var viewModel = _configuration.Routes.CreateViewModel(routeKey, HostScreen);
        HostScreen.Router.NavigateAndReset.Execute(viewModel)
                  .Subscribe();
    }

    private void HandleDiagnosticTimerTick(object? sender, EventArgs e)
    {
        var routes = _configuration.Routes.Routes
            .Select(route => route.RouteKey)
            .Where(CanNavigateTo)
            .ToArray();
        if (routes.Length == 0)
        {
            return;
        }

        var routeKey = routes[_diagnosticRouteIndex++ % routes.Length];
        DoNavigateTo(routeKey);
    }

    private void DoTestNavigatePages(TimeSpan interval)
    {
        if (_isDisposed)
        {
            return;
        }

        _dispatcherTimer.Stop();
        if (!_isDiagnosticNavigationRunning)
        {
            _shouldResetDeferredLoadingDisabledOnDiagnosticStop =
                !GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled;
        }

        _isDiagnosticNavigationRunning = true;
        GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled = true;
        _dispatcherTimer.Interval = interval;
        _dispatcherTimer.Start();
    }

    private void DoStopTestNavigatePages()
    {
        if (_isDisposed)
        {
            return;
        }

        StopDiagnosticNavigation();
    }

    private void StopDiagnosticNavigation()
    {
        _dispatcherTimer.Stop();
        if (_isDiagnosticNavigationRunning && _shouldResetDeferredLoadingDisabledOnDiagnosticStop)
        {
            GalleryShowCaseRuntimeOptions.IsDeferredLoadingDisabled = false;
        }

        _isDiagnosticNavigationRunning = false;
        _shouldResetDeferredLoadingDisabledOnDiagnosticStop = false;
    }

    private static IEnumerable<GalleryNavigationNode> Walk(IEnumerable<GalleryNavigationNode> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node;
            foreach (var child in Walk(node.Children))
            {
                yield return child;
            }
        }
    }
}
