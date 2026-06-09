using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Fonts.AlibabaPuHuiTi;
using AtomUI.Icons.AntDesign;
using AtomUIGallery.Localization;
using AtomUIGallery.ShowCases.AboutUs;
using AtomUIGallery.ShowCases.AutoComplete;
using AtomUIGallery.ShowCases.Avatar;
using AtomUIGallery.ShowCases.Badge;
using AtomUIGallery.ShowCases.Breadcrumb;
using AtomUIGallery.ShowCases.Button;
using AtomUIGallery.ShowCases.ButtonSpinner;
using AtomUIGallery.ShowCases.Calendar;
using AtomUIGallery.ShowCases.Card;
using AtomUIGallery.ShowCases.Carousel;
using AtomUIGallery.ShowCases.Cascader;
using AtomUIGallery.ShowCases.CheckBox;
using AtomUIGallery.ShowCases.Collapse;
using AtomUIGallery.ShowCases.ColorPicker;
using AtomUIGallery.ShowCases.ComboBox;
using AtomUIGallery.ShowCases.CustomizeTheme;
using AtomUIGallery.ShowCases.DataGrid;
using AtomUIGallery.ShowCases.DatePicker;
using AtomUIGallery.ShowCases.Descriptions;
using AtomUIGallery.ShowCases.DropdownButton;
using AtomUIGallery.ShowCases.Empty;
using AtomUIGallery.ShowCases.Expander;
using AtomUIGallery.ShowCases.FlexPanel;
using AtomUIGallery.ShowCases.FloatButton;
using AtomUIGallery.ShowCases.Form;
using AtomUIGallery.ShowCases.Grid;
using AtomUIGallery.ShowCases.GroupBox;
using AtomUIGallery.ShowCases.Icon;
using AtomUIGallery.ShowCases.ImagePreviewer;
using AtomUIGallery.ShowCases.InfoFlyout;
using AtomUIGallery.ShowCases.LineEdit;
using AtomUIGallery.ShowCases.List;
using AtomUIGallery.ShowCases.Mentions;
using AtomUIGallery.ShowCases.Menu;
using AtomUIGallery.ShowCases.NumberUpDown;
using AtomUIGallery.ShowCases.Palette;
using AtomUIGallery.ShowCases.Pagination;
using AtomUIGallery.ShowCases.QRCode;
using AtomUIGallery.ShowCases.RadioButton;
using AtomUIGallery.ShowCases.Rate;
using AtomUIGallery.ShowCases.Segmented;
using AtomUIGallery.ShowCases.Select;
using AtomUIGallery.ShowCases.Separator;
using AtomUIGallery.ShowCases.Slider;
using AtomUIGallery.ShowCases.Space;
using AtomUIGallery.ShowCases.SplitButton;
using AtomUIGallery.ShowCases.Splitter;
using AtomUIGallery.ShowCases.Steps;
using AtomUIGallery.ShowCases.Statistic;
using AtomUIGallery.ShowCases.TabControl;
using AtomUIGallery.ShowCases.TabStrip;
using AtomUIGallery.ShowCases.Tag;
using AtomUIGallery.ShowCases.TimePicker;
using AtomUIGallery.ShowCases.Timeline;
using AtomUIGallery.ShowCases.ToggleSwitch;
using AtomUIGallery.ShowCases.Tooltip;
using AtomUIGallery.ShowCases.Tour;
using AtomUIGallery.ShowCases.Transfer;
using AtomUIGallery.ShowCases.TreeView;
using AtomUIGallery.ShowCases.TreeSelect;
using AtomUIGallery.ShowCases.Upload;
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
        BrowserGalleryPageKind.Pagination,
        BrowserGalleryPageKind.Steps,
        BrowserGalleryPageKind.TabControl,
        BrowserGalleryPageKind.TabStrip,
        BrowserGalleryPageKind.AutoComplete,
        BrowserGalleryPageKind.Cascader,
        BrowserGalleryPageKind.CheckBox,
        BrowserGalleryPageKind.ColorPicker,
        BrowserGalleryPageKind.DatePicker,
        BrowserGalleryPageKind.TimePicker,
        BrowserGalleryPageKind.Form,
        BrowserGalleryPageKind.LineEdit,
        BrowserGalleryPageKind.Mentions,
        BrowserGalleryPageKind.NumberUpDown,
        BrowserGalleryPageKind.RadioButton,
        BrowserGalleryPageKind.Rate,
        BrowserGalleryPageKind.Select,
        BrowserGalleryPageKind.Slider,
        BrowserGalleryPageKind.ToggleSwitch,
        BrowserGalleryPageKind.TreeSelect,
        BrowserGalleryPageKind.Transfer,
        BrowserGalleryPageKind.Upload,
        BrowserGalleryPageKind.Palette,
        BrowserGalleryPageKind.Icons,
        BrowserGalleryPageKind.Avatar,
        BrowserGalleryPageKind.Badge,
        BrowserGalleryPageKind.Calendar,
        BrowserGalleryPageKind.Card,
        BrowserGalleryPageKind.Carousel,
        BrowserGalleryPageKind.Collapse,
        BrowserGalleryPageKind.Descriptions,
        BrowserGalleryPageKind.DataGrid,
        BrowserGalleryPageKind.Expander,
        BrowserGalleryPageKind.Empty,
        BrowserGalleryPageKind.ImagePreviewer,
        BrowserGalleryPageKind.GroupBox,
        BrowserGalleryPageKind.InfoFlyout,
        BrowserGalleryPageKind.List,
        BrowserGalleryPageKind.QRCode,
        BrowserGalleryPageKind.Segmented,
        BrowserGalleryPageKind.Statistic,
        BrowserGalleryPageKind.Tag,
        BrowserGalleryPageKind.Timeline,
        BrowserGalleryPageKind.TreeView,
        BrowserGalleryPageKind.Tooltip,
        BrowserGalleryPageKind.Tour
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
                                     BrowserGalleryPageKind.Menu),
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_Pagination,
                                     PaginationViewModel.ID,
                                     BrowserGalleryPageKind.Pagination),
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_Steps,
                                     StepsViewModel.ID,
                                     BrowserGalleryPageKind.Steps),
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_TabControl,
                                     TabControlViewModel.ID,
                                     BrowserGalleryPageKind.TabControl),
                CreateNavigationNode(CaseNavigationLangResourceKind.Navigation_TabStrip,
                                     TabStripViewModel.ID,
                                     BrowserGalleryPageKind.TabStrip)));

        navMenu.Items.Add(
            CreateNavigationGroup(
                CaseNavigationLangResourceKind.DataEntry,
                AntDesignIconKind.FormOutlined,
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_AutoComplete,
                                     AutoCompleteViewModel.ID,
                                     BrowserGalleryPageKind.AutoComplete),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_Cascader,
                                     CascaderViewModel.ID,
                                     BrowserGalleryPageKind.Cascader),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_CheckBox,
                                     CheckBoxViewModel.ID,
                                     BrowserGalleryPageKind.CheckBox),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_ColorPicker,
                                     ColorPickerViewModel.ID,
                                     BrowserGalleryPageKind.ColorPicker),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_DatePicker,
                                     DatePickerViewModel.ID,
                                     BrowserGalleryPageKind.DatePicker),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_TimePicker,
                                     TimePickerViewModel.ID,
                                     BrowserGalleryPageKind.TimePicker),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_Form,
                                     FormViewModel.ID,
                                     BrowserGalleryPageKind.Form),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_LineEdit,
                                     LineEditViewModel.ID,
                                     BrowserGalleryPageKind.LineEdit),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_Mentions,
                                     MentionsViewModel.ID,
                                     BrowserGalleryPageKind.Mentions),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_NumberUpDown,
                                     NumberUpDownViewModel.ID,
                                     BrowserGalleryPageKind.NumberUpDown),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_RadioButton,
                                     RadioButtonViewModel.ID,
                                     BrowserGalleryPageKind.RadioButton),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_Rate,
                                     RateViewModel.ID,
                                     BrowserGalleryPageKind.Rate),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_Select,
                                     SelectViewModel.ID,
                                     BrowserGalleryPageKind.Select),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_Slider,
                                     SliderViewModel.ID,
                                     BrowserGalleryPageKind.Slider),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_ToggleSwitch,
                                     ToggleSwitchViewModel.ID,
                                     BrowserGalleryPageKind.ToggleSwitch),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_TreeSelect,
                                     TreeSelectViewModel.ID,
                                     BrowserGalleryPageKind.TreeSelect),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_Transfer,
                                     TransferViewModel.ID,
                                     BrowserGalleryPageKind.Transfer),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataEntry_Upload,
                                     UploadViewModel.ID,
                                     BrowserGalleryPageKind.Upload)));

        navMenu.Items.Add(
            CreateNavigationGroup(
                CaseNavigationLangResourceKind.DataDisplay,
                AntDesignIconKind.AreaChartOutlined,
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Avatar,
                                     AvatarViewModel.ID,
                                     BrowserGalleryPageKind.Avatar),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Badge,
                                     BadgeViewModel.ID,
                                     BrowserGalleryPageKind.Badge),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Calendar,
                                     CalendarViewModel.ID,
                                     BrowserGalleryPageKind.Calendar),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Card,
                                     CardViewModel.ID,
                                     BrowserGalleryPageKind.Card),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Carousel,
                                     CarouselViewModel.ID,
                                     BrowserGalleryPageKind.Carousel),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Collapse,
                                     CollapseViewModel.ID,
                                     BrowserGalleryPageKind.Collapse),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Descriptions,
                                     DescriptionsViewModel.ID,
                                     BrowserGalleryPageKind.Descriptions),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_DataGrid,
                                     DataGridViewModel.ID,
                                     BrowserGalleryPageKind.DataGrid),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Expander,
                                     ExpanderViewModel.ID,
                                     BrowserGalleryPageKind.Expander),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Empty,
                                     EmptyViewModel.ID,
                                     BrowserGalleryPageKind.Empty),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_ImagePreviewer,
                                     ImagePreviewerViewModel.ID,
                                     BrowserGalleryPageKind.ImagePreviewer),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_GroupBox,
                                     GroupBoxViewModel.ID,
                                     BrowserGalleryPageKind.GroupBox),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_InfoFlyout,
                                     InfoFlyoutViewModel.ID,
                                     BrowserGalleryPageKind.InfoFlyout),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_List,
                                     ListViewModel.ID,
                                     BrowserGalleryPageKind.List),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_QRCode,
                                     QRCodeViewModel.ID,
                                     BrowserGalleryPageKind.QRCode),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Segmented,
                                     SegmentedViewModel.ID,
                                     BrowserGalleryPageKind.Segmented),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Statistic,
                                     StatisticViewModel.ID,
                                     BrowserGalleryPageKind.Statistic),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Tag,
                                     TagViewModel.ID,
                                     BrowserGalleryPageKind.Tag),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Timeline,
                                     TimelineViewModel.ID,
                                     BrowserGalleryPageKind.Timeline),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_TreeView,
                                     TreeViewViewModel.ID,
                                     BrowserGalleryPageKind.TreeView),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Tooltip,
                                     TooltipViewModel.ID,
                                     BrowserGalleryPageKind.Tooltip),
                CreateNavigationNode(CaseNavigationLangResourceKind.DataDisplay_Tour,
                                     TourViewModel.ID,
                                     BrowserGalleryPageKind.Tour)));

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
        var key = args.NavMenuItem.ItemKey;
        NavigateToPage(key);
    }

    private void NavigateToPage(EntityKey? key)
    {
        DelayPagePreload(s_pagePreloadUserIdleDelay);
        CancelPageWarmup();
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
            BrowserGalleryPageKind.Pagination => new PaginationShowCase
            {
                DataContext = new PaginationViewModel(this)
            },
            BrowserGalleryPageKind.Steps => new StepsShowCase
            {
                DataContext = new StepsViewModel(this)
            },
            BrowserGalleryPageKind.TabControl => new TabControlShowCase
            {
                DataContext = new TabControlViewModel(this)
            },
            BrowserGalleryPageKind.TabStrip => new TabStripShowCase
            {
                DataContext = new TabStripViewModel(this)
            },
            BrowserGalleryPageKind.AutoComplete => new AutoCompleteShowCase
            {
                DataContext = new AutoCompleteViewModel(this)
            },
            BrowserGalleryPageKind.Cascader => new CascaderShowCase
            {
                DataContext = new CascaderViewModel(this)
            },
            BrowserGalleryPageKind.CheckBox => new CheckBoxShowCase
            {
                DataContext = new CheckBoxViewModel(this)
            },
            BrowserGalleryPageKind.ColorPicker => new ColorPickerShowCase
            {
                DataContext = new ColorPickerViewModel(this)
            },
            BrowserGalleryPageKind.DatePicker => new DatePickerShowCase
            {
                DataContext = new DatePickerViewModel(this)
            },
            BrowserGalleryPageKind.TimePicker => new TimePickerShowCase
            {
                DataContext = new TimePickerViewModel(this)
            },
            BrowserGalleryPageKind.Form => new FormShowCase
            {
                DataContext = new FormViewModel(this)
            },
            BrowserGalleryPageKind.LineEdit => new LineEditShowCase
            {
                DataContext = new LineEditViewModel(this)
            },
            BrowserGalleryPageKind.Mentions => new MentionsShowCase
            {
                DataContext = new MentionsViewModel(this)
            },
            BrowserGalleryPageKind.NumberUpDown => new NumberUpDownShowCase
            {
                DataContext = new NumberUpDownViewModel(this)
            },
            BrowserGalleryPageKind.RadioButton => new RadioButtonShowCase
            {
                DataContext = new RadioButtonViewModel(this)
            },
            BrowserGalleryPageKind.Rate => new RateShowCase
            {
                DataContext = new RateViewModel(this)
            },
            BrowserGalleryPageKind.Select => new SelectShowCase
            {
                DataContext = new SelectViewModel(this)
            },
            BrowserGalleryPageKind.Slider => new SliderShowCase
            {
                DataContext = new SliderViewModel(this)
            },
            BrowserGalleryPageKind.ToggleSwitch => new ToggleSwitchShowCase
            {
                DataContext = new ToggleSwitchViewModel(this)
            },
            BrowserGalleryPageKind.TreeSelect => new TreeSelectShowCase
            {
                DataContext = new TreeSelectViewModel(this)
            },
            BrowserGalleryPageKind.Transfer => new TransferShowCase
            {
                DataContext = new TransferViewModel(this)
            },
            BrowserGalleryPageKind.Upload => new UploadShowCase
            {
                DataContext = new UploadViewModel(this)
            },
            BrowserGalleryPageKind.Palette => new PaletteShowCase
            {
                DataContext = new PaletteViewModel(this)
            },
            BrowserGalleryPageKind.Icons => new IconShowCase
            {
                DataContext = new IconViewModel(this)
            },
            BrowserGalleryPageKind.Avatar => new AvatarShowCase
            {
                DataContext = new AvatarViewModel(this)
            },
            BrowserGalleryPageKind.Badge => new BadgeShowCase
            {
                DataContext = new BadgeViewModel(this)
            },
            BrowserGalleryPageKind.Calendar => new CalendarShowCase
            {
                DataContext = new CalendarViewModel(this)
            },
            BrowserGalleryPageKind.Card => new CardShowCase
            {
                DataContext = new CardViewModel(this)
            },
            BrowserGalleryPageKind.Carousel => new CarouselShowCase
            {
                DataContext = new CarouselViewModel(this)
            },
            BrowserGalleryPageKind.Collapse => new CollapseShowCase
            {
                DataContext = new CollapseViewModel(this)
            },
            BrowserGalleryPageKind.Descriptions => new DescriptionsShowCase
            {
                DataContext = new DescriptionsViewModel(this)
            },
            BrowserGalleryPageKind.DataGrid => new DataGridShowCase
            {
                DataContext = new DataGridViewModel(this)
            },
            BrowserGalleryPageKind.Expander => new ExpanderShowCase
            {
                DataContext = new ExpanderViewModel(this)
            },
            BrowserGalleryPageKind.Empty => new EmptyShowCase
            {
                DataContext = new EmptyViewModel(this)
            },
            BrowserGalleryPageKind.ImagePreviewer => new ImagePreviewerShowCase
            {
                DataContext = new ImagePreviewerViewModel(this)
            },
            BrowserGalleryPageKind.GroupBox => new GroupBoxShowCase
            {
                DataContext = new GroupBoxViewModel(this)
            },
            BrowserGalleryPageKind.InfoFlyout => new InfoFlyoutShowCase
            {
                DataContext = new InfoFlyoutViewModel(this)
            },
            BrowserGalleryPageKind.List => new ListShowCase
            {
                DataContext = new ListViewModel(this)
            },
            BrowserGalleryPageKind.QRCode => new QRCodeShowCase
            {
                DataContext = new QRCodeViewModel(this)
            },
            BrowserGalleryPageKind.Segmented => new SegmentedShowCase
            {
                DataContext = new SegmentedViewModel(this)
            },
            BrowserGalleryPageKind.Statistic => new StatisticShowCase
            {
                DataContext = new StatisticViewModel(this)
            },
            BrowserGalleryPageKind.Tag => new TagShowCase
            {
                DataContext = new TagViewModel(this)
            },
            BrowserGalleryPageKind.Timeline => new TimelineShowCase
            {
                DataContext = new TimelineViewModel(this)
            },
            BrowserGalleryPageKind.TreeView => new TreeViewShowCase
            {
                DataContext = new TreeViewViewModel(this)
            },
            BrowserGalleryPageKind.Tooltip => new TooltipShowCase
            {
                DataContext = new TooltipViewModel(this)
            },
            BrowserGalleryPageKind.Tour => new TourShowCase
            {
                DataContext = new TourViewModel(this)
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
        Pagination,
        Steps,
        TabControl,
        TabStrip,
        AutoComplete,
        Cascader,
        CheckBox,
        ColorPicker,
        DatePicker,
        TimePicker,
        Form,
        LineEdit,
        Mentions,
        NumberUpDown,
        RadioButton,
        Rate,
        Select,
        Slider,
        ToggleSwitch,
        TreeSelect,
        Transfer,
        Upload,
        Palette,
        Icons,
        Avatar,
        Badge,
        Calendar,
        Card,
        Carousel,
        Collapse,
        Descriptions,
        DataGrid,
        Expander,
        Empty,
        ImagePreviewer,
        GroupBox,
        InfoFlyout,
        List,
        QRCode,
        Segmented,
        Statistic,
        Tag,
        Timeline,
        TreeView,
        Tooltip,
        Tour
    }
}
