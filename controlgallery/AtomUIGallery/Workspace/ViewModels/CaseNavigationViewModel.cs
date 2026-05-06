using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
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
            DoNavigateTo(AboutUsViewModel.ID);
        });
    }

    private void RegisterShowCaseViewModels()
    {
        _showCaseViewModelFactories.Add(AboutUsViewModel.ID, () => new AboutUsViewModel(HostScreen));
        _showCaseViewModelFactories.Add(PaletteViewModel.ID, () => new PaletteViewModel(HostScreen));
        _showCaseViewModelFactories.Add(IconViewModel.ID, () => new IconViewModel(HostScreen));
        _showCaseViewModelFactories.Add(ButtonViewModel.ID, () => new ButtonViewModel(HostScreen));
        _showCaseViewModelFactories.Add(ButtonSpinnerViewModel.ID, () => new ButtonSpinnerViewModel(HostScreen));
        _showCaseViewModelFactories.Add(SplitButtonViewModel.ID, () => new SplitButtonViewModel(HostScreen));
        _showCaseViewModelFactories.Add(SeparatorViewModel.ID, () => new SeparatorViewModel(HostScreen));
        _showCaseViewModelFactories.Add(SplitterViewModel.ID, () => new SplitterViewModel(HostScreen));
        _showCaseViewModelFactories.Add(BreadcrumbViewModel.ID, () => new BreadcrumbViewModel(HostScreen));
        _showCaseViewModelFactories.Add(ComboBoxViewModel.ID, () => new ComboBoxViewModel(HostScreen));
        _showCaseViewModelFactories.Add(DropdownButtonViewModel.ID, () => new DropdownButtonViewModel(HostScreen));
        _showCaseViewModelFactories.Add(MessageViewModel.ID, () => new MessageViewModel(HostScreen));
        _showCaseViewModelFactories.Add(NotificationViewModel.ID, () => new NotificationViewModel(HostScreen));
        _showCaseViewModelFactories.Add(PopupConfirmViewModel.ID, () => new PopupConfirmViewModel(HostScreen));
        _showCaseViewModelFactories.Add(AlertViewModel.ID, () => new AlertViewModel(HostScreen));
        _showCaseViewModelFactories.Add(SpinViewModel.ID, () => new SpinViewModel(HostScreen));
        _showCaseViewModelFactories.Add(FlexPanelViewModel.ID, () => new FlexPanelViewModel(HostScreen));
        _showCaseViewModelFactories.Add(CarouselViewModel.ID, () => new CarouselViewModel(HostScreen));
        _showCaseViewModelFactories.Add(AvatarViewModel.ID, () => new AvatarViewModel(HostScreen));
        _showCaseViewModelFactories.Add(CalendarViewModel.ID, () => new CalendarViewModel(HostScreen));
        _showCaseViewModelFactories.Add(SkeletonViewModel.ID, () => new SkeletonViewModel(HostScreen));
        _showCaseViewModelFactories.Add(CardViewModel.ID, () => new CardViewModel(HostScreen));
        _showCaseViewModelFactories.Add(CollapseViewModel.ID, () => new CollapseViewModel(HostScreen));
        _showCaseViewModelFactories.Add(EmptyViewModel.ID, () => new EmptyViewModel(HostScreen));
        _showCaseViewModelFactories.Add(ExpanderViewModel.ID, () => new ExpanderViewModel(HostScreen));
        _showCaseViewModelFactories.Add(CheckBoxViewModel.ID, () => new CheckBoxViewModel(HostScreen));
        _showCaseViewModelFactories.Add(ToggleSwitchViewModel.ID, () => new ToggleSwitchViewModel(HostScreen));
        _showCaseViewModelFactories.Add(RadioButtonViewModel.ID, () => new RadioButtonViewModel(HostScreen));
        _showCaseViewModelFactories.Add(RateViewModel.ID, () => new RateViewModel(HostScreen));
        _showCaseViewModelFactories.Add(SliderViewModel.ID, () => new SliderViewModel(HostScreen));
        _showCaseViewModelFactories.Add(TagViewModel.ID, () => new TagViewModel(HostScreen));
        _showCaseViewModelFactories.Add(SegmentedViewModel.ID, () => new SegmentedViewModel(HostScreen));
        _showCaseViewModelFactories.Add(GroupBoxViewModel.ID, () => new GroupBoxViewModel(HostScreen));
        _showCaseViewModelFactories.Add(ResultViewModel.ID, () => new ResultViewModel(HostScreen));
        _showCaseViewModelFactories.Add(BadgeViewModel.ID, () => new BadgeViewModel(HostScreen));
        _showCaseViewModelFactories.Add(TimelineViewModel.ID, () => new TimelineViewModel(HostScreen));
        _showCaseViewModelFactories.Add(QRCodeViewModel.ID, () => new QRCodeViewModel(HostScreen));
        _showCaseViewModelFactories.Add(LineEditViewModel.ID, () => new LineEditViewModel(HostScreen));
        _showCaseViewModelFactories.Add(TabControlViewModel.ID, () => new TabControlViewModel(HostScreen));
        _showCaseViewModelFactories.Add(InfoFlyoutViewModel.ID, () => new InfoFlyoutViewModel(HostScreen));
        _showCaseViewModelFactories.Add(WatermarkViewModel.ID, () => new WatermarkViewModel(HostScreen));
        _showCaseViewModelFactories.Add(ProgressBarViewModel.ID, () => new ProgressBarViewModel(HostScreen));
        _showCaseViewModelFactories.Add(PaginationViewModel.ID, () => new PaginationViewModel(HostScreen));
        _showCaseViewModelFactories.Add(MenuViewModel.ID, () => new MenuViewModel(HostScreen));
        _showCaseViewModelFactories.Add(TimePickerViewModel.ID, () => new TimePickerViewModel(HostScreen));
        _showCaseViewModelFactories.Add(DatePickerViewModel.ID, () => new DatePickerViewModel(HostScreen));
        _showCaseViewModelFactories.Add(NumberUpDownViewModel.ID, () => new NumberUpDownViewModel(HostScreen));
        _showCaseViewModelFactories.Add(ListViewModel.ID, () => new ListViewModel(HostScreen));
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
