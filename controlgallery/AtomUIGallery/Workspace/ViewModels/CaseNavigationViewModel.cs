using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.Workspace.ViewModels;

public class CaseNavigationViewModel : ReactiveObject, IActivatableViewModel
{
    private readonly Dictionary<EntityKey, Func<IRoutableViewModel>> _showCaseViewModelFactories;
    private EntityKey?      _currentShowCase;
    private DispatcherTimer _dispatcherTimer;

    private INavMenuNode? _selectedItem;

    public INavMenuNode? SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public IScreen           HostScreen { get; }
    public ViewModelActivator Activator  { get; }

    public ReactiveCommand<EntityKey, Unit> NavigateToCommand            { get; }
    public ReactiveCommand<TimeSpan, Unit>  TestNavigatePagesCommand     { get; }
    public ReactiveCommand<Unit, Unit>      StopTestNavigatePagesCommand { get; }

    public CaseNavigationViewModel(IScreen hostScreen)
    {
        _showCaseViewModelFactories = new Dictionary<EntityKey, Func<IRoutableViewModel>>();
        HostScreen                  = hostScreen;
        Activator                   = new ViewModelActivator();

        RegisterShowCaseViewModels();

        _dispatcherTimer      = new DispatcherTimer();
        _dispatcherTimer.Tick += RandomNavigateToTimerHandler;

        NavigateToCommand            = ReactiveCommand.Create<EntityKey>(DoNavigateTo);
        TestNavigatePagesCommand     = ReactiveCommand.Create<TimeSpan>(DoTestNavigatePages);
        StopTestNavigatePagesCommand = ReactiveCommand.Create(DoStopTestNavigatePages);

        this.WhenActivated((CompositeDisposable disposables) =>
        {
        });
    }

    private void RegisterShowCaseViewModels()
    {
        // ShowCase ViewModels will be registered here as they are added.
        // Example:
        // _showCaseViewModelFactories.Add(SomeViewModel.ID, () => new SomeViewModel(HostScreen));
    }

    private void DoNavigateTo(EntityKey showCaseId)
    {
        if (_currentShowCase is not null && _currentShowCase == showCaseId)
        {
            return;
        }

        _currentShowCase = showCaseId;

        if (!_showCaseViewModelFactories.TryGetValue(showCaseId, out var viewModelFactory))
        {
            throw new NotSupportedException($"unknown showcase id {showCaseId}");
        }

        var viewModel = viewModelFactory();
        HostScreen.Router.NavigateAndReset.Execute(viewModel);
    }

    private static int _currentShowCaseIdx = 0;

    private void RandomNavigateToTimerHandler(object? sender, EventArgs e)
    {
        var caseIds = _showCaseViewModelFactories.Keys.ToList();
        if (caseIds.Count == 0) return;
        var id = caseIds[_currentShowCaseIdx++ % caseIds.Count];
        DoNavigateTo(id);
    }

    private void DoTestNavigatePages(TimeSpan interval)
    {
        _dispatcherTimer.Stop();
        _dispatcherTimer.Interval = interval;
        _dispatcherTimer.Start();
    }

    private void DoStopTestNavigatePages()
    {
        _dispatcherTimer.Stop();
    }
}
