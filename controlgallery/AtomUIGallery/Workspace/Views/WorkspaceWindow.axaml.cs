using System.Reactive;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Shell;
using AtomUIGallery.Workspace.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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
    LanguageZhTW,
    LanguageEnUS,
}

public partial class WorkspaceWindow : ReactiveWindow<WorkspaceWindowViewModel>
{
    public const string LanguageId = nameof(WorkspaceWindow);
    private const string TitleBarMenuResourceKey = "WorkspaceTitleBarMenu";
    private GalleryShellView? _shellView;

    public WorkspaceWindow()
    {
        ViewModel = new WorkspaceWindowViewModel();
        InitializeComponent();

        if (ViewModel is not null)
        {
            var showCaseNavigation = new CaseNavigation
            {
                Name      = "ShowCaseNavigation",
                ViewModel = ViewModel.CaseNavigation
            };
            _shellView = new GalleryShellView(
                AtomUIGalleryModule.GetConfiguration(),
                showCaseNavigation,
                ViewModel.Router);
            ShellHost.Children.Add(_shellView);
        }
    }

    protected override WindowTitleBar? NotifyCreateTitleBar(WindowTitleBar? oldTitleBar)
    {
        return new GalleryWindowTitleBar
        {
            Name = "PART_TitleBar"
        };
    }

    protected override void NotifyConfigureTitleBar(WindowTitleBar titleBar)
    {
        base.NotifyConfigureTitleBar(titleBar);
        if (titleBar is GalleryWindowTitleBar galleryTitleBar &&
            Resources.TryGetValue(TitleBarMenuResourceKey, out var titleBarMenu))
        {
            galleryTitleBar.Menu = titleBarMenu as Control;
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        AddHandler(MenuItem.ClickEvent, HandleMenuItemClick);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        RemoveHandler(MenuItem.ClickEvent, HandleMenuItemClick);
        _shellView?.Dispose();
        _shellView = null;
        ViewModel?.Dispose();
        base.OnDetachedFromVisualTree(e);
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
                case WindowMenuItemKind.LanguageZhTW:
                    ViewModel.SwitchToZhTWCommand.Execute(Unit.Default)
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
