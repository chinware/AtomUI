using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using AtomUIGallery.Localization;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Menu;

public partial class MenuShowCase : GalleryReactiveUserControl<MenuViewModel>
{
    public const string LanguageId = nameof(MenuShowCase);

    private const string BasicScenario       = "Basic";
    private const string FeaturesScenario    = "Features";
    private const string ItemsSourceScenario = "ItemsSource";
    private const string ContextScenario     = "Context";
    private const string NavMenuScenario     = "NavMenu";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);
    private NavMenuNode? _navMenuDefaultSelectedItem;

    public MenuShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
        EnsureSelectedScenarioContent();

        this.WhenActivated(disposables =>
        {
            if (DataContext is MenuViewModel viewModel)
            {
                viewModel.DefaultOpenPaths = new List<TreeNodePath>
                {
                    new("/3/SubGroup2")
                };
                viewModel.DefaultSelectedPath = new TreeNodePath("/3/SubGroup1/Option1");

                RefreshMenuSources(viewModel);

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshMenuSources(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                        .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    viewModel.MenuItems                   = null;
                    viewModel.InlineNavMenuNodes          = null;
                    viewModel.ItemsSourceDemoNavMenuNodes = null;
                    viewModel.MenuFlyoutItems             = null;
                    viewModel.ContextMenuItems            = null;
                    viewModel.DefaultOpenPaths            = null;
                    viewModel.DefaultSelectedPath         = null;
                    viewModel.DefaultSelectedNode         = null;
                }).DisposeWith(disposables);
            }
        });
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        foreach (var content in _scenarioCache.Values)
        {
            content.DataContext = DataContext;
        }
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not AtomUI.Desktop.Controls.TabItem tabItem ||
            tabItem.Tag is not string scenario)
        {
            return;
        }

        if (!_scenarioCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _scenarioCache.Add(scenario, content);
        }

        if (tabItem.Content != content)
        {
            tabItem.Content = content;
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            BasicScenario       => new MenuBasicShowCase(),
            FeaturesScenario    => new MenuFeaturesShowCase(),
            ItemsSourceScenario => new MenuItemsSourceShowCase(),
            ContextScenario     => new MenuContextShowCase(),
            NavMenuScenario     => new MenuNavigationShowCase(),
            _                   => throw new InvalidOperationException($"Unknown Menu scenario: {scenario}")
        };
    }

    private void RefreshMenuSources(MenuViewModel viewModel)
    {
        InitInlineNavMenuNodes(viewModel);
        InitMenuTreeNodes(viewModel);
        InitContextMenuItems(viewModel);
        InitMenuFlyoutMenuItems(viewModel);
    }

    private static string Lang(MenuShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }

    private static string DisplayLang(MenuShowCaseLangResourceKind resourceKind, string fallback)
    {
        return Lang(resourceKind, fallback).Replace("_", string.Empty, StringComparison.Ordinal);
    }

    private void InitContextMenuItems(MenuViewModel viewModel)
    {
        viewModel.ContextMenuItems = new List<IMenuItemData>
        {
            new MenuItemData
            {
                Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderCut, "Cut"),
                Icon         = new ScissorOutlined(),
                InputGesture = KeyGesture.Parse("Ctrl+X"),
            },
            new MenuItemData
            {
                Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderCopy, "Copy"),
                Icon         = new CopyOutlined(),
                InputGesture = KeyGesture.Parse("Ctrl+C"),
            },
            new MenuItemData
            {
                Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderDelete, "Delete"),
                Icon         = new DeleteOutlined(),
                InputGesture = KeyGesture.Parse("Ctrl+D"),
            },
            new MenuItemData
            {
                Header = Lang(MenuShowCaseLangResourceKind.P2HeaderPaste, "Paste"),
                Children =
                [
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderPaste, "Paste"),
                        Icon         = new FileDoneOutlined(),
                        InputGesture = KeyGesture.Parse("Ctrl+P")
                    },
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderPasteFromHistory, "Paste from History"),
                        InputGesture = KeyGesture.Parse("Ctrl+Shift+V")
                    }
                ]
            }
        };
    }

    private void InitMenuTreeNodes(MenuViewModel viewModel)
    {
        viewModel.MenuItems = new List<IMenuItemData>
        {
            new MenuItemData
            {
                Header = DisplayLang(MenuShowCaseLangResourceKind.P2HeaderFile, "File"),
                Children =
                [
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderNewTextFile, "New Text File"),
                        InputGesture = KeyGesture.Parse("Ctrl+N")
                    },
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderNewFile, "New File"),
                        InputGesture = KeyGesture.Parse("Ctrl+Alt+N")
                    },
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderNewWindow, "New Window"),
                        InputGesture = KeyGesture.Parse("Ctrl+Shift+N")
                    }
                ]
            },
            new MenuItemData
            {
                Header = DisplayLang(MenuShowCaseLangResourceKind.P2HeaderEdit, "Edit"),
                Children =
                [
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderUndo, "Undo"),
                        InputGesture = KeyGesture.Parse("Ctrl+Shift+Z")
                    },
                    new MenuSeparatorData(),
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderCut, "Cut"),
                        InputGesture = KeyGesture.Parse("Ctrl+X")
                    }
                ]
            },
            new MenuItemData
            {
                Header    = Lang(MenuShowCaseLangResourceKind.P2HeaderDisabledItem, "Disabled Item"),
                IsEnabled = false
            }
        };
    }

    private void InitInlineNavMenuNodes(MenuViewModel viewModel)
    {
        viewModel.InlineNavMenuNodes          = BuildNavMenuNodes(out _);
        viewModel.ItemsSourceDemoNavMenuNodes = BuildNavMenuNodes(out _navMenuDefaultSelectedItem);
        viewModel.DefaultSelectedNode         = _navMenuDefaultSelectedItem;
    }

    private static List<INavMenuNode> BuildNavMenuNodes(out NavMenuNode defaultSelected)
    {
        defaultSelected = new NavMenuNode
        {
            Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderOptionN4, "Option 4"),
            ItemKey = "Option4",
            Icon    = new TwitterOutlined()
        };
        return new List<INavMenuNode>
        {
            new NavMenuNode
            {
                Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderNavigationOne, "Navigation One"),
                Icon    = new MailOutlined(),
                ItemKey = "1"
            },
            new NavMenuNode
            {
                Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderNavigationTwo, "Navigation Two"),
                Icon    = new AppstoreOutlined(),
                ItemKey = "2"
            },
            new NavMenuNode
            {
                Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderNavigationThreeSubmenu, "Navigation Three - Submenu"),
                Icon    = new SettingOutlined(),
                ItemKey = "3",
                Children =
                [
                    new NavMenuNode
                    {
                        Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderItemN1, "Item 1"),
                        ItemKey = "SubGroup1",
                        Children =
                        [
                            new NavMenuNode
                            {
                                Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderOptionN1, "Option 1"),
                                ItemKey = "Option1"
                            },
                            new NavMenuNode
                            {
                                Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderOptionN2, "Option 2"),
                                ItemKey = "Option2"
                            }
                        ]
                    },
                    new NavMenuNode
                    {
                        Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderItemN2, "Item 2"),
                        ItemKey = "SubGroup2",
                        Children =
                        [
                            new NavMenuNode
                            {
                                Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderOptionN3, "Option 3"),
                                ItemKey = "Option3"
                            },
                            defaultSelected
                        ]
                    }
                ]
            },
            new NavMenuNode
            {
                Header  = Lang(MenuShowCaseLangResourceKind.P2HeaderNavigationFour, "Navigation Four"),
                ItemKey = "4"
            }
        };
    }

    private void InitMenuFlyoutMenuItems(MenuViewModel viewModel)
    {
        viewModel.MenuFlyoutItems = new List<IMenuItemData>
        {
            new MenuItemData
            {
                Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderCut, "Cut"),
                InputGesture = KeyGesture.Parse("Ctrl+X"),
                Icon         = new ScissorOutlined(),
            },
            new MenuItemData
            {
                Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderCopy, "Copy"),
                InputGesture = KeyGesture.Parse("Ctrl+C"),
                Icon         = new CopyOutlined(),
            },
            new MenuItemData
            {
                Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderDelete, "Delete"),
                InputGesture = KeyGesture.Parse("Ctrl+D"),
                Icon         = new DeleteOutlined(),
            },
            new MenuItemData
            {
                Header = Lang(MenuShowCaseLangResourceKind.P2HeaderPaste, "Paste"),
                Children =
                [
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderPaste, "Paste"),
                        InputGesture = KeyGesture.Parse("Ctrl+P"),
                        Icon         = new FileDoneOutlined(),
                    },
                    new MenuSeparatorData(),
                    new MenuItemData
                    {
                        Header       = Lang(MenuShowCaseLangResourceKind.P2HeaderPasteFromHistory, "Paste from History"),
                        InputGesture = KeyGesture.Parse("Ctrl+Shift+V"),
                    }
                ]
            }
        };
    }
}
