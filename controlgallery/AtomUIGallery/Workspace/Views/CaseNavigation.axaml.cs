using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUI.Toolkits.GalleryBase.Navigation;
using AtomUIGallery.Workspace.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Window = Avalonia.Controls.Window;

namespace AtomUIGallery.Workspace.Views;

public partial class CaseNavigation : GalleryReactiveUserControl<CaseNavigationViewModel>
{
    public const string LanguageId = nameof(CaseNavigation);
    private EventHandler<LanguageVariantChangedEventArgs>? _languageVariantChangedHandler;

    public CaseNavigation()
    {
        InitializeComponent();
        ConfigureNavigationMenu();

        this.WhenActivated(disposables =>
        {
            void NavMenuItemClickHandler(object? sender, NavMenuItemClickEventArgs args)
            {
                var key = args.NavMenuItem.ItemKey;
                if (key.HasValue &&
                    ViewModel is not null &&
                    ViewModel.CanNavigateTo(key.Value))
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

    private void ConfigureNavigationMenu()
    {
        var configuration = AtomUIGalleryModule.GetConfiguration();
        var adapter       = new GalleryNavigationMenuAdapter();

        ShowCaseNavMenu.Items.Clear();
        foreach (var node in adapter.BuildNodes(configuration.NavigationNodes))
        {
            ShowCaseNavMenu.Items.Add(node);
        }

        ShowCaseNavMenu.DefaultOpenPaths = adapter.BuildDefaultOpenPaths(configuration.DefaultOpenKeys);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SubscribeLanguageChanged();
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window)
        {
            window.AddHandler(InputElement.KeyDownEvent, OnGlobalKeyDown, RoutingStrategies.Tunnel);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        UnsubscribeLanguageChanged();
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window window)
        {
            window.RemoveHandler(InputElement.KeyDownEvent, OnGlobalKeyDown);
        }

        base.OnDetachedFromVisualTree(e);
    }

    private void SubscribeLanguageChanged()
    {
        if (_languageVariantChangedHandler is not null)
        {
            return;
        }

        var languageManager = Application.Current?.GetLanguageManager();
        if (languageManager is null)
        {
            return;
        }

        _languageVariantChangedHandler = (_, _) => ConfigureNavigationMenu();
        languageManager.LanguageVariantChanged += _languageVariantChangedHandler;
    }

    private void UnsubscribeLanguageChanged()
    {
        if (_languageVariantChangedHandler is null)
        {
            return;
        }

        var languageManager = Application.Current?.GetLanguageManager();
        if (languageManager is not null)
        {
            languageManager.LanguageVariantChanged -= _languageVariantChangedHandler;
        }

        _languageVariantChangedHandler = null;
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
