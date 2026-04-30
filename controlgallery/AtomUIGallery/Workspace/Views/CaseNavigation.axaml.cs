using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Workspace.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Window = Avalonia.Controls.Window;

namespace AtomUIGallery.Workspace.Views;

public partial class CaseNavigation : ReactiveUserControl<CaseNavigationViewModel>
{
    public const string LanguageId = nameof(CaseNavigation);

    public CaseNavigation()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            void NavMenuItemClickHandler(object? sender, NavMenuItemClickEventArgs args)
            {
                var key = args.NavMenuItem.ItemKey;
                if (key.HasValue && ViewModel is not null)
                {
                    ViewModel.NavigateToCommand.Execute(key.Value)
                             .Subscribe()
                             .DisposeWith(disposables);
                }
            }

            ShowCaseNavMenu.NavMenuItemClick += NavMenuItemClickHandler;
            Disposable.Create(() => ShowCaseNavMenu.NavMenuItemClick -= NavMenuItemClickHandler)
                      .DisposeWith(disposables);
        });
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window)
        {
            window.AddHandler(InputElement.KeyDownEvent, OnGlobalKeyDown, RoutingStrategies.Tunnel);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window)
        {
            window.RemoveHandler(InputElement.KeyDownEvent, OnGlobalKeyDown);
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void OnGlobalKeyDown(object? sender, KeyEventArgs e)
    {
        if (ViewModel is not null)
        {
            if (e.Key == Key.F5)
            {
                ViewModel.TestNavigatePagesCommand.Execute(TimeSpan.FromMilliseconds(300))
                         .Subscribe();
                e.Handled = true;
            }
            else if (e.Key == Key.F6)
            {
                ViewModel.StopTestNavigatePagesCommand.Execute()
                         .Subscribe();
            }
        }
    }
}
