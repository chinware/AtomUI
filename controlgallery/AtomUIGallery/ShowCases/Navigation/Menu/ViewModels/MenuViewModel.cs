using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Menu;

public class MenuViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Menu";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private List<IMenuItemData>? _menuItems;

    public List<IMenuItemData>? MenuItems
    {
        get => _menuItems;
        set => this.RaiseAndSetIfChanged(ref _menuItems, value);
    }

    private List<IMenuItemData>? _menuFlyoutItems;

    public List<IMenuItemData>? MenuFlyoutItems
    {
        get => _menuFlyoutItems;
        set => this.RaiseAndSetIfChanged(ref _menuFlyoutItems, value);
    }

    private List<IMenuItemData>? _contextMenuItems;

    public List<IMenuItemData>? ContextMenuItems
    {
        get => _contextMenuItems;
        set => this.RaiseAndSetIfChanged(ref _contextMenuItems, value);
    }

    private bool _isDark;

    public bool IsDark
    {
        get => _isDark;
        set => this.RaiseAndSetIfChanged(ref _isDark, value);
    }

    private NavMenuMode _mode = NavMenuMode.Inline;

    public NavMenuMode Mode
    {
        get => _mode;
        set => this.RaiseAndSetIfChanged(ref _mode, value);
    }

    private IList<TreeNodePath>? _defaultOpenPaths;

    public IList<TreeNodePath>? DefaultOpenPaths
    {
        get => _defaultOpenPaths;
        set => this.RaiseAndSetIfChanged(ref _defaultOpenPaths, value);
    }

    private TreeNodePath? _defaultSelectedPath;

    public TreeNodePath? DefaultSelectedPath
    {
        get => _defaultSelectedPath;
        set => this.RaiseAndSetIfChanged(ref _defaultSelectedPath, value);
    }

    private bool _isInlineCollapsed;

    public bool IsInlineCollapsed
    {
        get => _isInlineCollapsed;
        set => this.RaiseAndSetIfChanged(ref _isInlineCollapsed, value);
    }

    private IList<TreeNodePath>? _inlineCollapsedOpenPaths;

    public IList<TreeNodePath>? InlineCollapsedOpenPaths
    {
        get => _inlineCollapsedOpenPaths;
        set => this.RaiseAndSetIfChanged(ref _inlineCollapsedOpenPaths, value);
    }

    private TreeNodePath? _inlineCollapsedSelectedPath;

    public TreeNodePath? InlineCollapsedSelectedPath
    {
        get => _inlineCollapsedSelectedPath;
        set => this.RaiseAndSetIfChanged(ref _inlineCollapsedSelectedPath, value);
    }

    private INavMenuNode? _defaultSelectedNode;

    public INavMenuNode? DefaultSelectedNode
    {
        get => _defaultSelectedNode;
        set => this.RaiseAndSetIfChanged(ref _defaultSelectedNode, value);
    }

    private List<INavMenuNode>? _inlineNavMenuNodes;

    public List<INavMenuNode>? InlineNavMenuNodes
    {
        get => _inlineNavMenuNodes;
        set => this.RaiseAndSetIfChanged(ref _inlineNavMenuNodes, value);
    }

    private List<INavMenuNode>? _itemsSourceDemoNavMenuNodes;

    public List<INavMenuNode>? ItemsSourceDemoNavMenuNodes
    {
        get => _itemsSourceDemoNavMenuNodes;
        set => this.RaiseAndSetIfChanged(ref _itemsSourceDemoNavMenuNodes, value);
    }

    private ObservableCollection<MenuApiRow>? _apiRows;
    private ObservableCollection<MenuDesignTokenRow>? _designTokenRows;

    public ObservableCollection<MenuApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<MenuDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ReactiveCommand<string, Unit> NavigateCommand { get; }

    public MenuViewModel(IScreen screen)
    {
        HostScreen      = screen;
        Activator       = new ViewModelActivator();
        NavigateCommand = ReactiveCommand.CreateFromTask<string>(OnNavigate, Observable.Return(true));
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new MenuApiRow("Menu.SizeType", Lang(MenuShowCaseLangResourceKind.ApiPropertyMenuSizeType), "CustomizableSizeType", "blue", "Middle"),
            new MenuApiRow("Menu.IsMotionEnabled", Lang(MenuShowCaseLangResourceKind.ApiPropertyMenuIsMotionEnabled), "bool", "purple", "token"),
            new MenuApiRow("Menu.DisplayPageSize", Lang(MenuShowCaseLangResourceKind.ApiPropertyMenuDisplayPageSize), "int", "cyan", "10"),
            new MenuApiRow("Menu.ShouldUseOverlayPopup", Lang(MenuShowCaseLangResourceKind.ApiPropertyMenuShouldUseOverlayPopup), "bool", "purple", "false"),
            new MenuApiRow("MenuItem.Icon", Lang(MenuShowCaseLangResourceKind.ApiPropertyMenuItemIcon), "PathIcon?", "cyan", "null"),
            new MenuApiRow("MenuItem.ToggleType", Lang(MenuShowCaseLangResourceKind.ApiPropertyMenuItemToggleType), "MenuItemToggleType", "blue", "None"),
            new MenuApiRow("ContextMenu.SizeType", Lang(MenuShowCaseLangResourceKind.ApiPropertyContextMenuSizeType), "CustomizableSizeType", "blue", "Middle"),
            new MenuApiRow("ContextMenu.ItemsSource", Lang(MenuShowCaseLangResourceKind.ApiPropertyContextMenuItemsSource), "IEnumerable?", "cyan", "null"),
            new MenuApiRow("MenuFlyout.ItemsSource", Lang(MenuShowCaseLangResourceKind.ApiPropertyMenuFlyoutItemsSource), "IEnumerable?", "cyan", "null"),
            new MenuApiRow("NavMenu.Mode", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuMode), "NavMenuMode", "blue", "Inline"),
            new MenuApiRow("NavMenu.IsInlineCollapsed", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuIsInlineCollapsed), "bool", "purple", "false"),
            new MenuApiRow("NavMenu.InlineCollapsedWidth", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuInlineCollapsedWidth), "double", "cyan", "token"),
            new MenuApiRow("NavMenu.IsDarkStyle", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuIsDarkStyle), "bool", "purple", "false"),
            new MenuApiRow("NavMenu.DefaultOpenPaths", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuDefaultOpenPaths), "IList<TreeNodePath>?", "cyan", "null"),
            new MenuApiRow("NavMenu.DefaultSelectedPath", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuDefaultSelectedPath), "TreeNodePath?", "cyan", "null"),
            new MenuApiRow("NavMenu.SelectedItem", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuSelectedItem), "INavMenuNode?", "cyan", "null"),
            new MenuApiRow("NavMenu.IsAccordionMode", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuIsAccordionMode), "bool", "purple", "false"),
            new MenuApiRow("NavMenu.ShouldUseOverlayPopup", Lang(MenuShowCaseLangResourceKind.ApiPropertyNavMenuShouldUseOverlayPopup), "bool", "purple", "false")
        ];
    }

    public void EnsureDesignTokenRows()
    {
        if (DesignTokenRows is not null)
        {
            return;
        }

        DesignTokenRows =
        [
            new MenuDesignTokenRow("Menu.ItemHeight", Lang(MenuShowCaseLangResourceKind.TokenNameMenuItemHeight), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("Menu.ItemIconSize", Lang(MenuShowCaseLangResourceKind.TokenNameMenuItemIconSize), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("Menu.ItemHoverBg", Lang(MenuShowCaseLangResourceKind.TokenNameMenuItemHoverBg), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("Menu.MenuPopupContentPadding", Lang(MenuShowCaseLangResourceKind.TokenNameMenuPopupContentPadding), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("Menu.MenuPopupMinWidth", Lang(MenuShowCaseLangResourceKind.TokenNameMenuPopupMinWidth), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("Menu.SeparatorItemHeight", Lang(MenuShowCaseLangResourceKind.TokenNameMenuSeparatorItemHeight), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("Menu.ContextMenuOffsetX", Lang(MenuShowCaseLangResourceKind.TokenNameMenuContextMenuOffsetX), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("NavMenu.ItemHeight", Lang(MenuShowCaseLangResourceKind.TokenNameNavMenuItemHeight), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("NavMenu.ItemContentPadding", Lang(MenuShowCaseLangResourceKind.TokenNameNavMenuItemContentPadding), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("NavMenu.InlineItemIndentUnit", Lang(MenuShowCaseLangResourceKind.TokenNameNavMenuInlineItemIndentUnit), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("NavMenu.CollapsedWidth", Lang(MenuShowCaseLangResourceKind.TokenNameNavMenuCollapsedWidth), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("NavMenu.InlineCollapsedWidth", Lang(MenuShowCaseLangResourceKind.TokenNameNavMenuInlineCollapsedWidth), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("NavMenu.MenuHorizontalHeight", Lang(MenuShowCaseLangResourceKind.TokenNameNavMenuMenuHorizontalHeight), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("NavMenu.DarkMenuBg", Lang(MenuShowCaseLangResourceKind.TokenNameNavMenuDarkMenuBg), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MenuDesignTokenRow("NavMenu.MenuPopupMaxHeight", Lang(MenuShowCaseLangResourceKind.TokenNameNavMenuMenuPopupMaxHeight), Lang(MenuShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(MenuShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private async Task OnNavigate(string? itemKey)
    {
        await Task.Delay(100);
    }

    public void HandleChangeModeCheckChanged(object? sender, RoutedEventArgs? args)
    {
        if (sender is AtomUIToggleSwitch changeModeSwitch)
        {
            if (changeModeSwitch.IsChecked.HasValue)
            {
                Mode = changeModeSwitch.IsChecked.Value ? NavMenuMode.Vertical : NavMenuMode.Inline;
            }
            else
            {
                Mode = NavMenuMode.Inline;
            }
        }
    }

    public void HandleChangeStyleCheckChanged(object? sender, RoutedEventArgs? args)
    {
        if (sender is AtomUIToggleSwitch changeStyleSwitch)
        {
            IsDark = changeStyleSwitch.IsChecked.GetValueOrDefault();
        }
    }

    public void HandleToggleInlineCollapsedClick(object? sender, RoutedEventArgs? args)
    {
        IsInlineCollapsed = !IsInlineCollapsed;
    }

    private static string Lang(MenuShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(MenuShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            MenuShowCaseLangResourceKind.ApiPropertyMenuSizeType                    => en_US.ApiPropertyMenuSizeType,
            MenuShowCaseLangResourceKind.ApiPropertyMenuIsMotionEnabled             => en_US.ApiPropertyMenuIsMotionEnabled,
            MenuShowCaseLangResourceKind.ApiPropertyMenuDisplayPageSize             => en_US.ApiPropertyMenuDisplayPageSize,
            MenuShowCaseLangResourceKind.ApiPropertyMenuShouldUseOverlayPopup       => en_US.ApiPropertyMenuShouldUseOverlayPopup,
            MenuShowCaseLangResourceKind.ApiPropertyMenuItemIcon                    => en_US.ApiPropertyMenuItemIcon,
            MenuShowCaseLangResourceKind.ApiPropertyMenuItemToggleType              => en_US.ApiPropertyMenuItemToggleType,
            MenuShowCaseLangResourceKind.ApiPropertyContextMenuSizeType             => en_US.ApiPropertyContextMenuSizeType,
            MenuShowCaseLangResourceKind.ApiPropertyContextMenuItemsSource          => en_US.ApiPropertyContextMenuItemsSource,
            MenuShowCaseLangResourceKind.ApiPropertyMenuFlyoutItemsSource           => en_US.ApiPropertyMenuFlyoutItemsSource,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuMode                     => en_US.ApiPropertyNavMenuMode,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuIsInlineCollapsed        => en_US.ApiPropertyNavMenuIsInlineCollapsed,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuInlineCollapsedWidth     => en_US.ApiPropertyNavMenuInlineCollapsedWidth,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuIsDarkStyle              => en_US.ApiPropertyNavMenuIsDarkStyle,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuDefaultOpenPaths         => en_US.ApiPropertyNavMenuDefaultOpenPaths,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuDefaultSelectedPath      => en_US.ApiPropertyNavMenuDefaultSelectedPath,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuSelectedItem             => en_US.ApiPropertyNavMenuSelectedItem,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuIsAccordionMode          => en_US.ApiPropertyNavMenuIsAccordionMode,
            MenuShowCaseLangResourceKind.ApiPropertyNavMenuShouldUseOverlayPopup    => en_US.ApiPropertyNavMenuShouldUseOverlayPopup,
            MenuShowCaseLangResourceKind.TokenNameMenuItemHeight                    => en_US.TokenNameMenuItemHeight,
            MenuShowCaseLangResourceKind.TokenNameMenuItemIconSize                  => en_US.TokenNameMenuItemIconSize,
            MenuShowCaseLangResourceKind.TokenNameMenuItemHoverBg                   => en_US.TokenNameMenuItemHoverBg,
            MenuShowCaseLangResourceKind.TokenNameMenuPopupContentPadding           => en_US.TokenNameMenuPopupContentPadding,
            MenuShowCaseLangResourceKind.TokenNameMenuPopupMinWidth                 => en_US.TokenNameMenuPopupMinWidth,
            MenuShowCaseLangResourceKind.TokenNameMenuSeparatorItemHeight           => en_US.TokenNameMenuSeparatorItemHeight,
            MenuShowCaseLangResourceKind.TokenNameMenuContextMenuOffsetX            => en_US.TokenNameMenuContextMenuOffsetX,
            MenuShowCaseLangResourceKind.TokenNameNavMenuItemHeight                 => en_US.TokenNameNavMenuItemHeight,
            MenuShowCaseLangResourceKind.TokenNameNavMenuItemContentPadding         => en_US.TokenNameNavMenuItemContentPadding,
            MenuShowCaseLangResourceKind.TokenNameNavMenuInlineItemIndentUnit       => en_US.TokenNameNavMenuInlineItemIndentUnit,
            MenuShowCaseLangResourceKind.TokenNameNavMenuCollapsedWidth             => en_US.TokenNameNavMenuCollapsedWidth,
            MenuShowCaseLangResourceKind.TokenNameNavMenuInlineCollapsedWidth       => en_US.TokenNameNavMenuInlineCollapsedWidth,
            MenuShowCaseLangResourceKind.TokenNameNavMenuMenuHorizontalHeight       => en_US.TokenNameNavMenuMenuHorizontalHeight,
            MenuShowCaseLangResourceKind.TokenNameNavMenuDarkMenuBg                 => en_US.TokenNameNavMenuDarkMenuBg,
            MenuShowCaseLangResourceKind.TokenNameNavMenuMenuPopupMaxHeight         => en_US.TokenNameNavMenuMenuPopupMaxHeight,
            MenuShowCaseLangResourceKind.TokenScopeComponent                        => en_US.TokenScopeComponent,
            MenuShowCaseLangResourceKind.TokenStatusStable                          => en_US.TokenStatusStable,
            _                                                                       => kind.ToString()
        };
    }
}

public sealed record MenuApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record MenuDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
