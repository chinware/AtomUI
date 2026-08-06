using AtomUIGallery.Localization;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Localization;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Menu;

public partial class MenuShowCase : GalleryReactiveUserControl<MenuViewModel>
{
    public const string LanguageId = nameof(MenuShowCase);

    private NavMenuNode? _navMenuDefaultSelectedItem;

    public MenuShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            RefreshCurrentViewModelData();

            var languageManager = GalleryLocalization.GetLanguageManager();
            if (languageManager != null)
            {
                EventHandler<LanguageChangedEventArgs> handler = (_, _) => RefreshCurrentViewModelData();
                languageManager.LanguageChanged += handler;
                Disposable.Create(() => languageManager.LanguageChanged -= handler)
                          .DisposeWith(disposables);
            }

            Disposable.Create(ClearCurrentViewModelData).DisposeWith(disposables);
        });
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        RefreshCurrentViewModelData();
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

    public void HandleToggleInlineCollapsedClick(object? sender, RoutedEventArgs? args)
    {
        if (DataContext is MenuViewModel viewModel)
        {
            viewModel.HandleToggleInlineCollapsedClick(sender, args);
        }
    }

    public void HandleToggleStructuredNavMenuCollapsedClick(object? sender, RoutedEventArgs? args)
    {
        if (DataContext is MenuViewModel viewModel)
        {
            viewModel.HandleToggleStructuredNavMenuCollapsedClick(sender, args);
        }
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
            viewModel.IsInlineCollapsed             = false;
            viewModel.IsStructuredNavMenuCollapsed  = false;
            viewModel.InlineCollapsedOpenPaths =
            [
                new TreeNodePath("/NavigationOne")
            ];
            viewModel.InlineCollapsedSelectedPath = new TreeNodePath("/Option1");
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
        viewModel.IsInlineCollapsed           = false;
        viewModel.IsStructuredNavMenuCollapsed = false;
        viewModel.InlineCollapsedOpenPaths    = null;
        viewModel.InlineCollapsedSelectedPath = null;
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
        return GalleryLocalization.Get(resourceKind, fallback);
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
