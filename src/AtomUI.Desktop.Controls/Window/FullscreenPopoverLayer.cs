using System.Reactive.Disposables;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUI.Desktop.Controls;

internal class FullscreenPopoverLayer : TemplatedControl
{
    private const double PopoverTriggerZoneHeight = 1;

    private Window? _hostWindow;
    private Border? _popoverBorder;
    private CaptionButton? _fullScreenButton;
    private CaptionButton? _closeButton;
    private CompositeDisposable? _disposables;
    private readonly List<Action> _disposeActions = new();
    private bool _popoverEnabled;
    private WindowState _previousWindowState = WindowState.Normal;

    public FullscreenPopoverLayer()
    {
        IsVisible = false;
        this.RegisterTokenResourceScope(WindowTitleBarToken.ScopeProvider);
    }

    public void Attach(Window hostWindow)
    {
        if (_disposables != null)
        {
            return;
        }

        _hostWindow = hostWindow;
        _previousWindowState = hostWindow.WindowState;
        _disposables = new CompositeDisposable(2);

        _disposables.Add(hostWindow.GetObservable(Window.WindowStateProperty)
            .Subscribe(OnWindowStateChanged));

        hostWindow.PointerMoved += OnHostWindowPointerMoved;
        _disposables.Add(Disposable.Create(() =>
            hostWindow.PointerMoved -= OnHostWindowPointerMoved));
    }

    public void Detach()
    {
        if (_disposables == null)
        {
            return;
        }

        _disposables.Dispose();
        _disposables = null;
        _popoverEnabled = false;
        _previousWindowState = WindowState.Normal;
        _hostWindow = null;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        foreach (var action in _disposeActions)
        {
            action.Invoke();
        }
        _disposeActions.Clear();

        _popoverBorder = e.NameScope.Find<Border>("PART_PopoverBorder");
        _fullScreenButton = e.NameScope.Find<CaptionButton>("PART_PopoverFullScreenButton");
        _closeButton = e.NameScope.Find<CaptionButton>("PART_PopoverCloseButton");

        if (_fullScreenButton != null)
        {
            _fullScreenButton.Click += HandleFullScreenButtonClicked;
            _disposeActions.Add(() => _fullScreenButton.Click -= HandleFullScreenButtonClicked);
        }

        if (_closeButton != null)
        {
            _closeButton.Click += HandleCloseButtonClicked;
            _disposeActions.Add(() => _closeButton.Click -= HandleCloseButtonClicked);
        }
    }

    private void OnWindowStateChanged(WindowState state)
    {
        if (state == WindowState.FullScreen)
        {
            IsVisible = true;
            SetPopoverVisible(false);
            _popoverEnabled = true;
        }
        else
        {
            IsVisible = false;
            SetPopoverVisible(false);
            _popoverEnabled = false;
            _previousWindowState = state;
        }
    }

    private void OnHostWindowPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_popoverEnabled || _hostWindow == null || _popoverBorder == null)
        {
            return;
        }

        var pos = e.GetPosition(_hostWindow);
        var titleBarHeight = _hostWindow.TitleBarHeight;

        if (!_popoverBorder.IsVisible && pos.Y <= PopoverTriggerZoneHeight)
        {
            SetPopoverVisible(true);
        }
        else if (_popoverBorder.IsVisible && pos.Y > titleBarHeight)
        {
            SetPopoverVisible(false);
        }
    }

    private void SetPopoverVisible(bool visible)
    {
        if (_popoverBorder != null)
        {
            _popoverBorder.IsVisible = visible;
        }
    }

    private void HandleFullScreenButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (_hostWindow == null)
        {
            return;
        }

        _hostWindow.WindowState = _previousWindowState;
    }

    private void HandleCloseButtonClicked(object? sender, RoutedEventArgs args)
    {
        if (_hostWindow == null)
        {
            return;
        }

        _hostWindow.NotifyCloseRequestByUser();
        _hostWindow.Close();
    }
}
