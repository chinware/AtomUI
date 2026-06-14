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
using Avalonia.Interactivity;
using AtomUIGallery.Localization;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Menu;

public partial class MenuShowCase : GalleryReactiveUserControl<MenuViewModel>
{
    public const string LanguageId = nameof(MenuShowCase);

    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);
    private NavMenuNode? _navMenuDefaultSelectedItem;

    public MenuShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;

        this.WhenActivated(disposables =>
        {
            RefreshCurrentViewModelData();

            var themeManager = Application.Current?.GetThemeManager();
            if (themeManager != null)
            {
                EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshCurrentViewModelData();
                themeManager.LanguageVariantChanged += handler;
                Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                          .DisposeWith(disposables);
            }

            Disposable.Create(ClearCurrentViewModelData).DisposeWith(disposables);
        });
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsureSelectedScenarioContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearLazyScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ExamplesContent.DataContext = DataContext;
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            content.DataContext = DataContext;
        }

        RefreshCurrentViewModelData();
        EnsureSelectedScenarioContent();
    }

    public void HandleChangeModeCheckChanged(object? sender, RoutedEventArgs? args)
    {
        if (DataContext is MenuViewModel viewModel)
        {
            viewModel.HandleChangeModeCheckChanged(sender, args);
        }
    }

    public void HandleChangeStyleCheckChanged(object? sender, RoutedEventArgs? args)
    {
        if (DataContext is MenuViewModel viewModel)
        {
            viewModel.HandleChangeStyleCheckChanged(sender, args);
        }
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not ScenarioTabStripItem tabStripItem ||
            tabStripItem.Tag is not string scenario)
        {
            return;
        }

        var content = ResolveScenarioContent(scenario);
        if (!ReferenceEquals(ScenarioContentHost.Content, content))
        {
            ScenarioContentHost.Content = content;
        }
    }

    private void ClearLazyScenarioContent()
    {
        if (ScenarioContentHost.Content is not null &&
            !ReferenceEquals(ScenarioContentHost.Content, ExamplesContent))
        {
            ScenarioContentHost.Content = null;
        }

        _lazyScenarioContentCache.Clear();
    }

    private Control ResolveScenarioContent(string scenario)
    {
        if (scenario == ExamplesScenario)
        {
            ExamplesContent.DataContext = DataContext;
            return ExamplesContent;
        }

        if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _lazyScenarioContentCache.Add(scenario, content);
        }

        return content;
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new MenuApiDataGrid(),
            DesignTokenScenario => new MenuDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Menu scenario: {scenario}")
        };
    }

    private void RefreshCurrentViewModelData()
    {
        if (DataContext is MenuViewModel viewModel)
        {
            viewModel.DefaultOpenPaths =
            [
                new TreeNodePath("/3/SubGroup2")
            ];
            viewModel.DefaultSelectedPath = new TreeNodePath("/3/SubGroup1/Option1");
            RefreshMenuSources(viewModel);
        }
    }

    private void ClearCurrentViewModelData()
    {
        if (DataContext is not MenuViewModel viewModel)
        {
            return;
        }

        viewModel.MenuItems                   = null;
        viewModel.InlineNavMenuNodes          = null;
        viewModel.ItemsSourceDemoNavMenuNodes = null;
        viewModel.MenuFlyoutItems             = null;
        viewModel.ContextMenuItems            = null;
        viewModel.DefaultOpenPaths            = null;
        viewModel.DefaultSelectedPath         = null;
        viewModel.DefaultSelectedNode         = null;
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
