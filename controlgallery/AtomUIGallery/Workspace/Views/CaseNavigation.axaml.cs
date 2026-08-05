using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Language;
using AtomUI.Toolkits.GalleryBase.Navigation;
using AtomUI.Toolkits.GalleryBase.Shell;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.ViewModels;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using DesktopButton = AtomUI.Desktop.Controls.Button;
using Window = Avalonia.Controls.Window;

namespace AtomUIGallery.Workspace.Views;

public partial class CaseNavigation : GalleryReactiveUserControl<CaseNavigationViewModel>,
                                      IGallerySidebarNavMenuHost
{
    public const string LanguageId = nameof(CaseNavigation);
    private EventHandler<LanguageVariantChangedEventArgs>? _languageVariantChangedHandler;
    private readonly PathIcon _collapseNavigationIcon;
    private readonly PathIcon _expandNavigationIcon;
    private readonly DesktopButton _navigationCollapseButton;

    NavMenu IGallerySidebarNavMenuHost.SidebarNavMenu => ShowCaseNavMenu;
    Control? IGallerySidebarNavMenuHost.SidebarHeaderAction => _navigationCollapseButton;

    public CaseNavigation()
    {
        InitializeComponent();
        _collapseNavigationIcon = CreateNavigationIcon(AntDesignIconKind.MenuFoldOutlined);
        _expandNavigationIcon   = CreateNavigationIcon(AntDesignIconKind.MenuUnfoldOutlined);
        _navigationCollapseButton = new DesktopButton
        {
            Name                = "NavigationCollapseButton",
            Width               = 40,
            Height              = 40,
            ButtonType          = ButtonType.Text,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        _navigationCollapseButton.Click += HandleToggleNavigationCollapsedClick;
        ConfigureNavigationMenu();
        UpdateNavigationCollapseButtonPresentation();

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
            ShowCaseNavMenu.GetObservable(NavMenu.IsInlineCollapsedProperty)
                           .Subscribe(_ => UpdateNavigationCollapseButtonPresentation())
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

        _languageVariantChangedHandler = (_, _) =>
        {
            ConfigureNavigationMenu();
            UpdateNavigationCollapseButtonPresentation();
        };
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

    private void HandleToggleNavigationCollapsedClick(object? sender, RoutedEventArgs e)
    {
        ShowCaseNavMenu.IsInlineCollapsed = !ShowCaseNavMenu.IsInlineCollapsed;
        UpdateNavigationCollapseButtonPresentation();
    }

    private void UpdateNavigationCollapseButtonPresentation()
    {
        var isCollapsed = ShowCaseNavMenu.IsInlineCollapsed;
        var resourceKind = isCollapsed
            ? CaseNavigationLangResourceKind.ExpandNavigation
            : CaseNavigationLangResourceKind.CollapseNavigation;
        var fallback = isCollapsed ? "Expand navigation" : "Collapse navigation";
        var accessibleText = LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;

        _navigationCollapseButton.Icon = isCollapsed
            ? _expandNavigationIcon
            : _collapseNavigationIcon;
        AutomationProperties.SetName(_navigationCollapseButton, accessibleText);
    }

    private static PathIcon CreateNavigationIcon(AntDesignIconKind iconKind)
    {
        return (PathIcon)new AntDesignIconProvider(iconKind).ProvideValue(null!);
    }
}
