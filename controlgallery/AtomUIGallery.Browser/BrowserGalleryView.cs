using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Fonts.AlibabaPuHuiTi;
using AtomUI.Icons.AntDesign;
using AtomUIGallery.Localization;
using AtomUIGallery.ShowCases.AboutUs;
using AtomUIGallery.ShowCases.Breadcrumb;
using AtomUIGallery.ShowCases.Button;
using AtomUIGallery.ShowCases.ButtonSpinner;
using AtomUIGallery.ShowCases.ComboBox;
using AtomUIGallery.ShowCases.CustomizeTheme;
using AtomUIGallery.ShowCases.DropdownButton;
using AtomUIGallery.ShowCases.FlexPanel;
using AtomUIGallery.ShowCases.FloatButton;
using AtomUIGallery.ShowCases.Grid;
using AtomUIGallery.ShowCases.Icon;
using AtomUIGallery.ShowCases.Menu;
using AtomUIGallery.ShowCases.Palette;
using AtomUIGallery.ShowCases.Separator;
using AtomUIGallery.ShowCases.Space;
using AtomUIGallery.ShowCases.SplitButton;
using AtomUIGallery.ShowCases.Splitter;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using ReactiveUI;
using AtomScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;
using NavMenu = AtomUI.Desktop.Controls.NavMenu;
using NavMenuItemClickEventArgs = AtomUI.Desktop.Controls.NavMenuItemClickEventArgs;
using NavMenuMode = AtomUI.Desktop.Controls.NavMenuMode;
using NavMenuNode = AtomUI.Desktop.Controls.NavMenuNode;

namespace AtomUIGallery.Browser;

internal sealed class BrowserGalleryView : UserControl, IScreen
{
    private static readonly TimeSpan s_pagePreloadInitialDelay = TimeSpan.FromSeconds(6);
    private static readonly TimeSpan s_pagePreloadUserIdleDelay = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan s_pagePreloadStepDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan s_pagePreloadTimerInterval = TimeSpan.FromSeconds(1);
    private static readonly DispatcherPriority s_pagePreloadPriority = DispatcherPriority.SystemIdle;
    private static readonly FontFamily s_appFontFamily =
        FontFamily.Parse($"fonts:AlibabaSans#Alibaba Sans, {AlibabaPuHuiTiFontConstants.FontFamily}, $Default");
    private static readonly BrowserGalleryPageKind[] s_pagesToPreload =
    [
        BrowserGalleryPageKind.Button,
        BrowserGalleryPageKind.FloatButton,
        BrowserGalleryPageKind.SplitButton,
        BrowserGalleryPageKind.Separator,
        BrowserGalleryPageKind.CustomizeTheme,
        BrowserGalleryPageKind.Menu,
        BrowserGalleryPageKind.FlexPanel,
        BrowserGalleryPageKind.Grid,
        BrowserGalleryPageKind.Space,
        BrowserGalleryPageKind.Splitter,
        BrowserGalleryPageKind.Breadcrumb,
        BrowserGalleryPageKind.ButtonSpinner,
        BrowserGalleryPageKind.ComboBox,
        BrowserGalleryPageKind.DropdownButton,
        BrowserGalleryPageKind.Palette,
        BrowserGalleryPageKind.Icons
    ];

    private readonly NavMenu _showCaseNavMenu;
    private readonly Dictionary<EntityKey, BrowserGalleryPageKind> _pageKindsByItemKey = new();
    private readonly Dictionary<BrowserGalleryPageKind, NavMenuNode> _navigationNodes = new();
    private readonly Grid _contentHost;
    private readonly Dictionary<BrowserGalleryPageKind, Control> _pageCache = new();
    private readonly HashSet<BrowserGalleryPageKind> _warmedPages = new();
    private BrowserGalleryPageKind? _activePageKind;
    private BrowserGalleryPageKind? _warmingPageKind;
    private Control? _warmingPage;
    private DispatcherTimer? _pagePreloadTimer;
    private DateTime _nextAllowedPagePreloadUtc;
    private int _nextPreloadPageIndex;

    public RoutingState Router { get; } = new();

    public BrowserGalleryView()
    {
        Loaded += HandleLoaded;
        AddHandler(PointerPressedEvent, HandleUserInput, RoutingStrategies.Tunnel);
        AddHandler(PointerWheelChangedEvent, HandleUserInput, RoutingStrategies.Tunnel);
        AddHandler(KeyDownEvent, HandleUserInput, RoutingStrategies.Tunnel);
        FontFamily = s_appFontFamily;

        var header = new Border
        {
            BorderBrush     = new SolidColorBrush(Color.Parse("#E5E7EB")),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding         = new Thickness(24, 0),
            Child           = new StackPanel
            {
                Orientation       = Orientation.Horizontal,
                Spacing           = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new Image
                    {
                        Source = LoadGalleryLogo(),
                        Width  = 28,
                        Height = 28
                    },
                    new TextBlock
                    {
                        Text              = "AtomUI Browser Gallery",
                        FontSize          = 20,
                        FontWeight        = FontWeight.SemiBold,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground        = new SolidColorBrush(Color.Parse("#111827"))
                    }
                }
            }
        };
        Grid.SetColumnSpan(header, 2);

        _showCaseNavMenu = CreateNavigationMenu();

        var navigation = new Border
        {
            BorderBrush     = new SolidColorBrush(Color.Parse("#E5E7EB")),
            BorderThickness = new Thickness(0, 0, 1, 0),
            Background      = Brushes.White,
            Child           = _showCaseNavMenu
        };
        Grid.SetRow(navigation, 1);

        _contentHost = new Grid();
        Grid.SetColumn(_contentHost, 1);
        Grid.SetRow(_contentHost, 1);

        var rootLayout = new Grid
        {
            RowDefinitions    = new RowDefinitions("64,*"),
            ColumnDefinitions = new ColumnDefinitions("260,*"),
            Background        = Brushes.White,
            Children =
            {
                header,
                navigation,
                _contentHost
            }
        };
        var visualLayerManager = new VisualLayerManager
        {
            Child = rootLayout
        };
        ConfigureOverlayLayers(visualLayerManager);
        Content = visualLayerManager;

        ShowPage(BrowserGalleryPageKind.AboutUs);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        CancelPageWarmup();
        StopPagePreloadTimer();
        base.OnDetachedFromVisualTree(e);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(VisualLayerManager))]
    private static void ConfigureOverlayLayers(VisualLayerManager visualLayerManager)
    {
        visualLayerManager.EnableOverlayLayer = true;
        SetVisualLayerManagerProperty(visualLayerManager, "EnablePopupOverlayLayer", true);

        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "OverlayLayer");
        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "PopupOverlayLayer");
        _ = GetVisualLayerManagerPropertyValue(visualLayerManager, "LightDismissOverlayLayer");
    }

    private static void SetVisualLayerManagerProperty(VisualLayerManager visualLayerManager,
                                                      string propertyName,
                                                      object? value)
    {
        var propertyInfo = typeof(VisualLayerManager)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (propertyInfo is null)
        {
            throw new InvalidOperationException($"Unable to find {propertyName} on {nameof(VisualLayerManager)}.");
        }

        propertyInfo.SetValue(visualLayerManager, value);
    }

    private static object? GetVisualLayerManagerPropertyValue(VisualLayerManager visualLayerManager,
                                                              string propertyName)
    {
        var propertyInfo = typeof(VisualLayerManager)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (propertyInfo is null)
        {
            throw new InvalidOperationException($"Unable to find {propertyName} on {nameof(VisualLayerManager)}.");
        }

        return propertyInfo.GetValue(visualLayerManager);
    }

    private static Bitmap LoadGalleryLogo()
    {
        using var stream = AssetLoader.Open(new Uri("avares://AtomUIGallery.Browser/Assets/gallery-logo.png"));
        return new Bitmap(stream);
    }

    private NavMenu CreateNavigationMenu()
    {
        var navMenu = new NavMenu
        {
            Mode                = NavMenuMode.Inline,
            IsMotionEnabled     = false,
            Width               = 260,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment   = VerticalAlignment.Stretch
        };
        AtomScrollViewer.SetIsLiteMode(navMenu, true);
        navMenu.NavMenuItemClick += HandleNavigationMenuItemClick;

        navMenu.Items.Add(
            CreateNavigationGroup(
                CaseNavigationLangResourceKind.General,
                AntDesignIconKind.WindowsOutlined,
                CreateNavigationNode(CaseNavigationLangResourceKind.General_AboutUs,
                                     AboutUsViewModel.ID,
                                     BrowserGalleryPageKind.AboutUs),
                CreateNavigationNode(CaseNavigationLangResourceKind.General_Palette,
                                     PaletteViewModel.ID,
                                     BrowserGalleryPageKind.Palette),
                CreateNavigationNode(CaseNavigationLangResourceKind.General_Icons,
                                     IconViewModel.ID,
                                     BrowserGalleryPageKind.Icons),
                CreateNavigationNode(CaseNavigationLangResourceKind.General_Button,
                                     ButtonViewModel.ID,
                                     BrowserGalleryPageKind.Button),
                CreateNavigationNode(CaseNavigationLangResourceKind.General_FloatButton,
                                     FloatButtonViewModel.ID,
                                     BrowserGalleryPageKind.FloatButton),
                CreateNavigationNode(CaseNavigationLangResourceKind.General_SplitButton,
                                     SplitButtonViewModel.ID,
                                     BrowserGalleryPageKind.SplitButton),
                CreateNavigationNode(CaseNavigationLangResourceKind.General_Separator,
                                     SeparatorViewModel.ID,
                                     BrowserGalleryPageKind.Separator),
                CreateNavigationNode(CaseNavigationLangResourceKind.General_CustomizeTheme,
                                     CustomizeThemeViewModel.ID,
                                     BrowserGalleryPageKind.CustomizeTheme)));

        navMenu.Items.Add(
            CreateNavigationGroup(
                CaseNavigationLangResourceKind.Layout,
                AntDesignIconKind.LayoutOutlined,
                CreateNavigationNode(CaseNavigationLangResourceKind.Layout_FlexPanel,
                                     FlexPanelViewModel.ID,
                                     BrowserGalleryPageKind.FlexPanel),
                CreateNavigationNode(CaseNavigationLangResourceKind.Layout_Grid,
                                     GridViewModel.ID,
                                     BrowserGalleryPageKind.Grid),
                CreateNavigationNode(CaseNavigationLangResourceKind.Layout_Space,
                                     SpaceViewModel.ID,
                                     BrowserGalleryPageKind.Space),
                CreateNavigationNode(CaseNavigationLangResourceKind.Layout_Splitter,
                                     SplitterViewModel.ID,
                                     BrowserGalleryPageKind.Splitter)));

        navMenu.Items.Add(
            CreateNavigationGroup(
                CaseNavigationLangResourceKind.Navigation,
                AntDesignIconKind.MenuOutlined,
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_Breadcrumb,
                                     BreadcrumbViewModel.ID,
                                     BrowserGalleryPageKind.Breadcrumb),
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_ButtonSpinner,
                                     ButtonSpinnerViewModel.ID,
                                     BrowserGalleryPageKind.ButtonSpinner),
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_ComboBox,
                                     ComboBoxViewModel.ID,
                                     BrowserGalleryPageKind.ComboBox),
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_DropdownButton,
                                     DropdownButtonViewModel.ID,
                                     BrowserGalleryPageKind.DropdownButton),
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_Menu,
                                     MenuViewModel.ID,
                                     BrowserGalleryPageKind.Menu)));

        return navMenu;
    }

    private NavMenuNode CreateNavigationGroup(CaseNavigationLangResourceKind headerKind,
                                              AntDesignIconKind iconKind,
                                              params NavMenuNode[] children)
    {
        var node = new NavMenuNode
        {
            Header = GetNavigationText(headerKind),
            Icon   = CreateNavigationIcon(iconKind)
        };
        foreach (var child in children)
        {
            node.Children.Add(child);
        }
        return node;
    }

    private NavMenuNode CreateNavigationNode(CaseNavigationLangResourceKind headerKind,
                                             EntityKey itemKey,
                                             BrowserGalleryPageKind pageKind)
    {
        var node = new NavMenuNode
        {
            Header  = GetNavigationText(headerKind),
            ItemKey = itemKey
        };
        _pageKindsByItemKey.Add(itemKey, pageKind);
        _navigationNodes.Add(pageKind, node);
        return node;
    }

    private static string GetNavigationText(CaseNavigationLangResourceKind resourceKind)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? resourceKind.ToString();
    }

    private static PathIcon CreateNavigationIcon(AntDesignIconKind kind)
    {
        return (PathIcon)new AntDesignIconProvider(kind).ProvideValue(null!);
    }

    private void HandleNavigationMenuItemClick(object? sender, NavMenuItemClickEventArgs args)
    {
        DelayPagePreload(s_pagePreloadUserIdleDelay);
        CancelPageWarmup();
        var key = args.NavMenuItem.ItemKey;
        if (key.HasValue && _pageKindsByItemKey.TryGetValue(key.Value, out var pageKind))
        {
            ShowPage(pageKind);
        }
    }

    private void HandleUserInput(object? sender, RoutedEventArgs args)
    {
        DelayPagePreload(s_pagePreloadUserIdleDelay);
        CancelPageWarmup();
    }

    private void HandleLoaded(object? sender, RoutedEventArgs e)
    {
        StartPagePreloadTimer(s_pagePreloadInitialDelay);
    }

    private void StartPagePreloadTimer(TimeSpan delay)
    {
        if (_pagePreloadTimer is not null || _nextPreloadPageIndex >= s_pagesToPreload.Length)
        {
            return;
        }

        DelayPagePreload(delay);
        _pagePreloadTimer = new DispatcherTimer(s_pagePreloadTimerInterval, s_pagePreloadPriority, Dispatcher);
        _pagePreloadTimer.Tick += HandlePagePreloadTimerTick;
        _pagePreloadTimer.Start();
    }

    private void ShowPage(BrowserGalleryPageKind pageKind)
    {
        if (_activePageKind == pageKind)
        {
            return;
        }

        CancelPageWarmup();
        var page = EnsurePage(pageKind);

        foreach (var cachedPage in _pageCache)
        {
            var isActive = cachedPage.Key == pageKind;
            cachedPage.Value.Opacity          = 1;
            cachedPage.Value.IsHitTestVisible = true;
            cachedPage.Value.IsVisible        = isActive;
        }

        _activePageKind = pageKind;
        _warmedPages.Add(pageKind);
        SkipWarmedPreloadPages();
        UpdateNavigationSelection(pageKind);
    }

    private Control EnsurePage(BrowserGalleryPageKind pageKind)
    {
        if (_pageCache.TryGetValue(pageKind, out var page))
        {
            return page;
        }

        page           = CreatePage(pageKind);
        page.IsVisible = false;
        _pageCache.Add(pageKind, page);
        _contentHost.Children.Add(page);
        return page;
    }

    private Control CreatePage(BrowserGalleryPageKind pageKind)
    {
        return pageKind switch
        {
            BrowserGalleryPageKind.AboutUs => new AboutUsPage
            {
                DataContext = new AboutUsViewModel(this)
            },
            BrowserGalleryPageKind.Button => new ButtonShowCase
            {
                DataContext = new ButtonViewModel(this)
            },
            BrowserGalleryPageKind.FloatButton => new FloatButtonShowCase
            {
                DataContext = new FloatButtonViewModel(this)
            },
            BrowserGalleryPageKind.SplitButton => new SplitButtonShowCase
            {
                DataContext = new SplitButtonViewModel(this)
            },
            BrowserGalleryPageKind.Separator => new SeparatorShowCase
            {
                DataContext = new SeparatorViewModel(this)
            },
            BrowserGalleryPageKind.CustomizeTheme => new CustomizeThemeShowCase
            {
                DataContext = new CustomizeThemeViewModel(this)
            },
            BrowserGalleryPageKind.Menu => new MenuShowCase
            {
                DataContext = new MenuViewModel(this)
            },
            BrowserGalleryPageKind.FlexPanel => new FlexPanelShowCase
            {
                DataContext = new FlexPanelViewModel(this)
            },
            BrowserGalleryPageKind.Grid => new GridShowCase
            {
                DataContext = new GridViewModel(this)
            },
            BrowserGalleryPageKind.Space => new SpaceShowCase
            {
                DataContext = new SpaceViewModel(this)
            },
            BrowserGalleryPageKind.Splitter => new SplitterShowCase
            {
                DataContext = new SplitterViewModel(this)
            },
            BrowserGalleryPageKind.Breadcrumb => new BreadcrumbShowCase
            {
                DataContext = new BreadcrumbViewModel(this)
            },
            BrowserGalleryPageKind.ButtonSpinner => new ButtonSpinnerShowCase
            {
                DataContext = new ButtonSpinnerViewModel(this)
            },
            BrowserGalleryPageKind.ComboBox => new ComboBoxShowCase
            {
                DataContext = new ComboBoxViewModel(this)
            },
            BrowserGalleryPageKind.DropdownButton => new DropdownButtonShowCase
            {
                DataContext = new DropdownButtonViewModel(this)
            },
            BrowserGalleryPageKind.Palette => new PaletteShowCase
            {
                DataContext = new PaletteViewModel(this)
            },
            BrowserGalleryPageKind.Icons => new IconShowCase
            {
                DataContext = new IconViewModel(this)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(pageKind), pageKind, null)
        };
    }

    private void HandlePagePreloadTimerTick(object? sender, EventArgs e)
    {
        if (DateTime.UtcNow < _nextAllowedPagePreloadUtc)
        {
            return;
        }

        SkipWarmedPreloadPages();
        if (_nextPreloadPageIndex >= s_pagesToPreload.Length)
        {
            StopPagePreloadTimer();
            return;
        }

        BeginPageWarmup(s_pagesToPreload[_nextPreloadPageIndex]);
    }

    private void BeginPageWarmup(BrowserGalleryPageKind pageKind)
    {
        var page = EnsurePage(pageKind);
        if (_activePageKind == pageKind)
        {
            _warmedPages.Add(pageKind);
            SkipWarmedPreloadPages();
            return;
        }

        _warmingPageKind       = pageKind;
        _warmingPage           = page;
        page.Opacity           = 0;
        page.IsHitTestVisible  = false;
        page.IsVisible         = true;

        Dispatcher.Post(() => CompletePageWarmup(pageKind, page), DispatcherPriority.Loaded);
    }

    private void CompletePageWarmup(BrowserGalleryPageKind pageKind, Control page)
    {
        if (!ReferenceEquals(_warmingPage, page))
        {
            return;
        }

        if (_activePageKind != pageKind)
        {
            page.IsVisible = false;
        }

        page.Opacity          = 1;
        page.IsHitTestVisible = true;
        _warmedPages.Add(pageKind);
        _warmingPageKind = null;
        _warmingPage     = null;

        SkipWarmedPreloadPages();
        DelayPagePreload(s_pagePreloadStepDelay);
    }

    private void CancelPageWarmup()
    {
        if (_warmingPage is null)
        {
            return;
        }

        if (_activePageKind != _warmingPageKind)
        {
            _warmingPage.IsVisible = false;
        }

        _warmingPage.Opacity          = 1;
        _warmingPage.IsHitTestVisible = true;
        _warmingPageKind = null;
        _warmingPage     = null;
    }

    private void SkipWarmedPreloadPages()
    {
        while (_nextPreloadPageIndex < s_pagesToPreload.Length &&
               _warmedPages.Contains(s_pagesToPreload[_nextPreloadPageIndex]))
        {
            _nextPreloadPageIndex++;
        }
    }

    private void DelayPagePreload(TimeSpan delay)
    {
        var nextAllowedPreloadUtc = DateTime.UtcNow + delay;
        if (nextAllowedPreloadUtc > _nextAllowedPagePreloadUtc)
        {
            _nextAllowedPagePreloadUtc = nextAllowedPreloadUtc;
        }
    }

    private void StopPagePreloadTimer()
    {
        if (_pagePreloadTimer is null)
        {
            return;
        }

        _pagePreloadTimer.Stop();
        _pagePreloadTimer.Tick -= HandlePagePreloadTimerTick;
        _pagePreloadTimer = null;
    }

    private void UpdateNavigationSelection(BrowserGalleryPageKind pageKind)
    {
        if (_navigationNodes.TryGetValue(pageKind, out var navigationNode) &&
            !ReferenceEquals(_showCaseNavMenu.SelectedItem, navigationNode))
        {
            _showCaseNavMenu.SelectedItem = navigationNode;
        }
    }

    private enum BrowserGalleryPageKind
    {
        AboutUs,
        Button,
        FloatButton,
        SplitButton,
        Separator,
        CustomizeTheme,
        Menu,
        FlexPanel,
        Grid,
        Space,
        Splitter,
        Breadcrumb,
        ButtonSpinner,
        ComboBox,
        DropdownButton,
        Palette,
        Icons
    }
}
