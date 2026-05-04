using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Input;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class MenuShowCase : ReactiveUserControl<MenuViewModel>
{
    private NavMenuNode? _navMenuDefaultSelectedItem;

    public MenuShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (DataContext is MenuViewModel viewModel)
            {
                ChangeModeSwitch.IsCheckedChanged  += viewModel.HandleChangeModeCheckChanged;
                ChangeStyleSwitch.IsCheckedChanged += viewModel.HandleChangeStyleCheckChanged;

                viewModel.DefaultOpenPaths = new List<TreeNodePath>
                {
                    new("/3/SubGroup2")
                };
                viewModel.DefaultSelectedPath = new TreeNodePath("/3/SubGroup1/Option1");

                InitInlineNavMenuNodes(viewModel);
                InitMenuTreeNodes(viewModel);
                InitContextMenuItems(viewModel);
                InitMenuFlyoutMenuItems(viewModel);

                this.OneWayBind(ViewModel, vm => vm.MenuItems, v => v.BasicItemsSourceMenu.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.InlineNavMenuNodes, v => v.InlineModeMenu.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ContextMenuItems, v => v.BasicContextMenu.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ItemsSourceDemoNavMenuNodes, v => v.ItemsSourceDemoNavMenu.ItemsSource)
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    ChangeModeSwitch.IsCheckedChanged     -= viewModel.HandleChangeModeCheckChanged;
                    ChangeStyleSwitch.IsCheckedChanged    -= viewModel.HandleChangeStyleCheckChanged;
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

    private void InitContextMenuItems(MenuViewModel viewModel)
    {
        viewModel.ContextMenuItems = new List<IMenuItemData>
        {
            new MenuItemData
            {
                Header       = "Cut",
                Icon         = new ScissorOutlined(),
                InputGesture = KeyGesture.Parse("Ctrl+X"),
            },
            new MenuItemData
            {
                Header       = "Copy",
                Icon         = new CopyOutlined(),
                InputGesture = KeyGesture.Parse("Ctrl+C"),
            },
            new MenuItemData
            {
                Header       = "Delete",
                Icon         = new DeleteOutlined(),
                InputGesture = KeyGesture.Parse("Ctrl+D"),
            },
            new MenuItemData
            {
                Header = "Paste",
                Children =
                [
                    new MenuItemData
                    {
                        Header       = "Paste",
                        Icon         = new FileDoneOutlined(),
                        InputGesture = KeyGesture.Parse("Ctrl+P")
                    },
                    new MenuItemData
                    {
                        Header       = "Paste from History",
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
                Header = "File",
                Children =
                [
                    new MenuItemData
                    {
                        Header       = "New Text File",
                        InputGesture = KeyGesture.Parse("Ctrl+N")
                    },
                    new MenuItemData
                    {
                        Header       = "New File",
                        InputGesture = KeyGesture.Parse("Ctrl+Alt+N")
                    },
                    new MenuItemData
                    {
                        Header       = "New Window",
                        InputGesture = KeyGesture.Parse("Ctrl+Shift+N")
                    }
                ]
            },
            new MenuItemData
            {
                Header = "Edit",
                Children =
                [
                    new MenuItemData
                    {
                        Header       = "Undo",
                        InputGesture = KeyGesture.Parse("Ctrl+Shift+Z")
                    },
                    new MenuSeparatorData(),
                    new MenuItemData
                    {
                        Header       = "Cut",
                        InputGesture = KeyGesture.Parse("Ctrl+X")
                    }
                ]
            },
            new MenuItemData
            {
                Header    = "Disabled Item",
                IsEnabled = false
            }
        };
    }

    private void InitInlineNavMenuNodes(MenuViewModel viewModel)
    {
        viewModel.InlineNavMenuNodes          = BuildNavMenuNodes(out _);
        viewModel.ItemsSourceDemoNavMenuNodes = BuildNavMenuNodes(out _navMenuDefaultSelectedItem);
        viewModel.DefaultSelectedNode         = _navMenuDefaultSelectedItem;
        ItemsSourceDemoNavMenu.SelectedItem   = _navMenuDefaultSelectedItem;
    }

    private static List<INavMenuNode> BuildNavMenuNodes(out NavMenuNode defaultSelected)
    {
        defaultSelected = new NavMenuNode
        {
            Header  = "Option 4",
            ItemKey = "Option4",
            Icon    = new TwitterOutlined()
        };
        return new List<INavMenuNode>
        {
            new NavMenuNode
            {
                Header  = "Navigation One",
                Icon    = new MailOutlined(),
                ItemKey = "1"
            },
            new NavMenuNode
            {
                Header  = "Navigation Two",
                Icon    = new AppstoreOutlined(),
                ItemKey = "2"
            },
            new NavMenuNode
            {
                Header  = "Navigation Three - Submenu",
                Icon    = new SettingOutlined(),
                ItemKey = "3",
                Children =
                [
                    new NavMenuNode
                    {
                        Header  = "Item 1",
                        ItemKey = "SubGroup1",
                        Children =
                        [
                            new NavMenuNode
                            {
                                Header  = "Option 1",
                                ItemKey = "Option1"
                            },
                            new NavMenuNode
                            {
                                Header  = "Option 2",
                                ItemKey = "Option2"
                            }
                        ]
                    },
                    new NavMenuNode
                    {
                        Header  = "Item 2",
                        ItemKey = "SubGroup2",
                        Children =
                        [
                            new NavMenuNode
                            {
                                Header  = "Option 3",
                                ItemKey = "Option3"
                            },
                            defaultSelected
                        ]
                    }
                ]
            },
            new NavMenuNode
            {
                Header  = "Navigation Four",
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
                Header       = "Cut",
                InputGesture = KeyGesture.Parse("Ctrl+X"),
                Icon         = new ScissorOutlined(),
            },
            new MenuItemData
            {
                Header       = "Copy",
                InputGesture = KeyGesture.Parse("Ctrl+C"),
                Icon         = new CopyOutlined(),
            },
            new MenuItemData
            {
                Header       = "Delete",
                InputGesture = KeyGesture.Parse("Ctrl+D"),
                Icon         = new DeleteOutlined(),
            },
            new MenuItemData
            {
                Header = "Paste",
                Children =
                [
                    new MenuItemData
                    {
                        Header       = "Paste",
                        InputGesture = KeyGesture.Parse("Ctrl+P"),
                        Icon         = new FileDoneOutlined(),
                    },
                    new MenuSeparatorData(),
                    new MenuItemData
                    {
                        Header       = "Paste from History",
                        InputGesture = KeyGesture.Parse("Ctrl+Shift+V"),
                    }
                ]
            }
        };
    }
}
