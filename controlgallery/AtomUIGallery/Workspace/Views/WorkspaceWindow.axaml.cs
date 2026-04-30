using System.Reactive;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Workspace.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI;
using MenuItem = AtomUI.Desktop.Controls.MenuItem;

namespace AtomUIGallery.Workspace.Views;

internal enum WindowMenuItemKind
{
    FullScreen,
    Pin,
    Minimize,
    Maximize,
    Move,
    Resize,
    DarkMode,
    Compact,
    Motion,
    WaveSpirit,
    LanguageZhCN,
    LanguageEnUS,
}

public partial class WorkspaceWindow : ReactiveWindow<WorkspaceWindowViewModel>
{
    public const string LanguageId = nameof(WorkspaceWindow);

    public WorkspaceWindow()
    {
        ViewModel = new WorkspaceWindowViewModel();
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router)
                .DisposeWith(disposables);

            ShowCaseNavigation.ViewModel = ViewModel!.CaseNavigation;
        });
    }

    public override void Show()
    {
        base.Show();
        Height = double.NaN;
        Width  = double.NaN;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        AddHandler(MenuItem.ClickEvent, HandleMenuItemClick);
    }

    private void HandleMenuItemClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is null) return;

        if (e.Source is MenuItem menuItem && menuItem.Tag is WindowMenuItemKind kind)
        {
            if (menuItem.ToggleType == MenuItemToggleType.None) return;

            switch (kind)
            {
                case WindowMenuItemKind.FullScreen:
                    IsFullScreenCaptionButtonVisible = menuItem.IsChecked;
                    break;
                case WindowMenuItemKind.Pin:
                    IsPinCaptionButtonVisible = menuItem.IsChecked;
                    break;
                case WindowMenuItemKind.Minimize:
                    CanMinimize = menuItem.IsChecked;
                    break;
                case WindowMenuItemKind.Maximize:
                    CanMaximize = menuItem.IsChecked;
                    break;
                case WindowMenuItemKind.Move:
                    IsMoveEnabled = menuItem.IsChecked;
                    break;
                case WindowMenuItemKind.Resize:
                    CanResize = menuItem.IsChecked;
                    break;
                case WindowMenuItemKind.DarkMode:
                    ViewModel.ToggleDarkModeCommand.Execute(menuItem.IsChecked)
                             .Subscribe();
                    break;
                case WindowMenuItemKind.Compact:
                    ViewModel.ToggleCompactModeCommand.Execute(menuItem.IsChecked)
                             .Subscribe();
                    break;
                case WindowMenuItemKind.Motion:
                    if (menuItem.Parent is MenuItem themeMenuItem)
                    {
                        foreach (var item in themeMenuItem.Items)
                        {
                            if (item is MenuItem themeMenuChildItem &&
                                themeMenuChildItem.Tag is WindowMenuItemKind childKind &&
                                childKind == WindowMenuItemKind.WaveSpirit)
                            {
                                if (!menuItem.IsChecked)
                                {
                                    themeMenuChildItem.IsChecked = false;
                                }
                            }
                        }
                    }
                    ViewModel.ToggleMotionCommand.Execute(menuItem.IsChecked)
                             .Subscribe();
                    break;
                case WindowMenuItemKind.WaveSpirit:
                    ViewModel.ToggleWaveSpiritCommand.Execute(menuItem.IsChecked)
                             .Subscribe();
                    break;
                case WindowMenuItemKind.LanguageZhCN:
                    ViewModel.SwitchToZhCNCommand.Execute(Unit.Default)
                             .Subscribe();
                    break;
                case WindowMenuItemKind.LanguageEnUS:
                    ViewModel.SwitchToEnUSCommand.Execute(Unit.Default)
                             .Subscribe();
                    break;
            }
        }
    }
}
