using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Workspace.ViewModels;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Threading;

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
#if DEBUG
        this.AttachDevTools();
#endif
        DataContext = new WorkspaceWindowViewModel();
        InitializeComponent();
        AddHandler(MenuItem.IsCheckStateChangedEvent, HandleMenuItemCheckChanged);
    }

    public override void Show()
    {
        base.Show();
        Height = double.NaN;
        Width  = double.NaN;
    }

    private void HandleMenuItemCheckChanged(object? sender, RoutedEventArgs e)
    {
        if (e.Source is MenuItem menuItem && menuItem.Tag is WindowMenuItemKind kind)
        {
            var application = Application.Current;
            Debug.Assert(application != null);
            if (kind == WindowMenuItemKind.FullScreen)
            {
                IsFullScreenCaptionButtonEnabled = menuItem.IsChecked;
            }
            else if (kind == WindowMenuItemKind.Pin)
            {
                IsPinCaptionButtonEnabled = menuItem.IsChecked;
            }
            else if (kind == WindowMenuItemKind.Minimize)
            {
                CanMinimize = menuItem.IsChecked;
            }
            else if (kind == WindowMenuItemKind.Maximize)
            {
                CanMaximize = menuItem.IsChecked;
            }
            else if (kind == WindowMenuItemKind.Move)
            {
                IsMoveEnabled = menuItem.IsChecked;
            }
            else if (kind == WindowMenuItemKind.Resize)
            {
                CanResize = menuItem.IsChecked;
            }
            else if (kind == WindowMenuItemKind.DarkMode)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    application.SetDarkThemeMode(menuItem.IsChecked);
                });
            }
            else if (kind == WindowMenuItemKind.Compact)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    application.SetCompactThemeMode(menuItem.IsChecked);
                });
            }
            else if (kind == WindowMenuItemKind.Motion)
            {
                if (menuItem.Parent is MenuItem themeMenuItem)
                {
                    foreach (var item in themeMenuItem.Items)
                    {
                        if (item is MenuItem themeMenuChildItem && themeMenuChildItem.Tag is WindowMenuItemKind themeMenuChildItemKind)
                        {
                            if (themeMenuChildItemKind == WindowMenuItemKind.WaveSpirit)
                            {
                                if (!menuItem.IsChecked)
                                {
                                    themeMenuChildItem.IsChecked = false;
                                }
                            }
                        }
                    }
                }
                application.SetMotionEnabled(menuItem.IsChecked);
            }
            else if (kind == WindowMenuItemKind.WaveSpirit)
            {
                application.SetWaveSpiritEnabled(menuItem.IsChecked);
            }
            else if (kind == WindowMenuItemKind.LanguageZhCN)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    application.SetLanguageVariant(LanguageVariant.zh_CN);
                });
            }
            else if (kind == WindowMenuItemKind.LanguageEnUS)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    application.SetLanguageVariant(LanguageVariant.en_US);
                });
            }
        }
    }
}