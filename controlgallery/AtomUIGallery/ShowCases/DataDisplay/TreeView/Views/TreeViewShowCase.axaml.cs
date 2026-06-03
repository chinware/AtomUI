using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Interactivity;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.TreeView;

public partial class TreeViewShowCase : ReactiveUserControl<TreeViewViewModel>
{
    public const string LanguageId = nameof(TreeViewShowCase);

    public TreeViewShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TreeViewViewModel viewModel)
            {
                InitBasicTreeViewData(viewModel);
                viewModel.TreeViewNodeHoverMode = TreeItemHoverMode.Default;
                RefreshLocalizedTreeNodes(viewModel);
                InitCustomizeCollapseExpandTreeDefaultExpandedPaths(viewModel);
                InitFilterTreeNodes(viewModel);
                viewModel.AsyncLoadTreeNodeLoader = new TreeItemDataLoader();

                this.OneWayBind(ViewModel, vm => vm.BasicTreeViewDefaultExpandedPaths, v => v.BasicTree.DefaultExpandedPaths)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.BasicTreeViewDefaultSelectedPaths, v => v.BasicTree.DefaultSelectedPaths)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.BasicTreeViewDefaultCheckedPaths, v => v.BasicTree.DefaultCheckedPaths)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.BasicTreeViewDefaultExpandedPaths, v => v.BasicTplTree.DefaultExpandedPaths)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.BasicTreeViewDefaultSelectedPaths, v => v.BasicTplTree.DefaultSelectedPaths)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.BasicTreeViewDefaultCheckedPaths, v => v.BasicTplTree.DefaultCheckedPaths)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.BasicTreeNodes, v => v.BasicTplTree.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.AsyncLoadTreeNodes, v => v.AsyncLoadTree.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.AsyncLoadTreeNodeLoader, v => v.AsyncLoadTree.DataLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.FilterTreeNodes, v => v.SearchTreeViewByItemsSource.ItemsSource)
                    .DisposeWith(disposables);

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshLocalizedTreeNodes(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                              .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    viewModel.BasicTreeViewDefaultExpandedPaths = null;
                    viewModel.BasicTreeViewDefaultSelectedPaths = null;
                    viewModel.BasicTreeViewDefaultCheckedPaths  = null;
                    viewModel.BasicTreeNodes                    = null;
                    viewModel.AsyncLoadTreeNodes                = null;
                    viewModel.AsyncLoadTreeNodeLoader           = null;
                    viewModel.FilterTreeNodes                   = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }

    private void RefreshLocalizedTreeNodes(TreeViewViewModel viewModel)
    {
        InitBasicTreeNodes(viewModel);
        InitAsyncLoadTreeNodes(viewModel);
    }

    private static string Lang(TreeViewShowCaseLangResourceKind resourceKind, string fallback)
    {
        return TreeViewShowCaseLanguage.Get(resourceKind, fallback);
    }

    private void InitBasicTreeViewData(TreeViewViewModel viewModel)
    {
        viewModel.BasicTreeViewDefaultExpandedPaths = [
            new TreeNodePath("0-0/0-0-0"),
            new TreeNodePath("0-0/0-0-1/0-0-1-1")
        ];
        
        viewModel.BasicTreeViewDefaultSelectedPaths =
        [
            new TreeNodePath("0-0/0-0-1")
        ];
        
        viewModel.BasicTreeViewDefaultCheckedPaths =
        [
            new TreeNodePath("0-0/0-0-1/0-0-1-1")
        ];
    }

    private void HandleHoverModeChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUI.Desktop.Controls.RadioButton radioButton)
        {
            if (radioButton.IsChecked == true)
            {
                if (radioButton.Tag is TreeItemHoverMode hoverMode)
                {
                    if (DataContext is TreeViewViewModel viewModel)
                    {
                        viewModel.TreeViewNodeHoverMode = hoverMode;
                    }
                }
            }
        }
    }
    
    private void InitBasicTreeNodes(TreeViewViewModel viewModel)
    {
        viewModel.BasicTreeNodes = [
            new TreeItemNode()
            {
                Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderParentN1, "parent 1"),
                ItemKey = "0-0",
                Children = [
                    new TreeItemNode()
                    {
                        Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderParentN1N0, "parent 1-0"),
                        ItemKey = "0-0-0",
                        Children = [
                            new TreeItemNode()
                            {
                                Header    = Lang(TreeViewShowCaseLangResourceKind.P2HeaderLeafN1, "leaf 1"),
                                ItemKey   = "0-0-0-0",
                                IsEnabled = false
                            },
                            new TreeItemNode()
                            {
                                Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderLeafN2, "leaf 2"),
                                ItemKey = "0-0-0-1"
                            }
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderParentN1N1, "parent 1-1"),
                        ItemKey = "0-0-1",
                        Children = [
                            new TreeItemNode()
                            {
                                Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderSss, "sss"),
                                ItemKey = "0-0-1-0",
                                Children = [
                                    new TreeItemNode()
                                    {
                                        Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderCcc, "ccc"),
                                        ItemKey = "0-0-1-0-0"
                                    }
                                ]
                            },
                            new TreeItemNode()
                            {
                                Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderXxx, "xxx"),
                                ItemKey = "0-0-1-1",
                                Children = [
                                    new TreeItemNode()
                                    {
                                        Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderAaaa, "aaaa"),
                                        ItemKey = "0-0-1-1-0"
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private void InitCustomizeCollapseExpandTreeDefaultExpandedPaths(TreeViewViewModel viewModel)
    {
        viewModel.CustomizeCollapseExpandTreeDefaultExpandedPaths = [
            new TreeNodePath("0-0/0-0-0")
        ];
    }

    private void InitAsyncLoadTreeNodes(TreeViewViewModel viewModel)
    {
        viewModel.AsyncLoadTreeNodes =
        [
            new TreeItemNode()
            {
                Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderExpandToLoad, "Expand to load"),
                ItemKey = "0",
            },
            new TreeItemNode()
            {
                Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderExpandToLoad, "Expand to load"),
                ItemKey = "1",
            },
            new TreeItemNode()
            {
                Header  = Lang(TreeViewShowCaseLangResourceKind.P2HeaderTreeNode, "Tree Node"),
                ItemKey = "2",
                IsLeaf  = true
            }
        ];
    }

    private void InitFilterTreeNodes(TreeViewViewModel viewModel)
    {
        viewModel.FilterTreeNodes =
        [
            new TreeItemNode()
            {
                Header  = "0-0",
                ItemKey = "0-0",
                Children = [
                    new TreeItemNode()
                    {
                        Header  = "0-0-0",
                        ItemKey = "0-0-0",
                        Children = [
                            new TreeItemNode()
                            {
                                Header  = "0-0-0-0",
                                ItemKey = "0-0-0-0",
                            },
                            new TreeItemNode()
                            {
                                Header  = "0-0-0-1",
                                ItemKey = "0-0-0-1",
                            },
                            new TreeItemNode()
                            {
                                Header  = "0-0-0-2",
                                ItemKey = "0-0-0-2",
                            }
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header  = "0-0-1",
                        ItemKey = "0-0-1",
                        Children = [
                            new TreeItemNode()
                            {
                                Header  = "0-0-1-0",
                                ItemKey = "0-0-1-0",
                            },
                            new TreeItemNode()
                            {
                                Header  = "0-0-1-1",
                                ItemKey = "0-0-1-1",
                            },
                            new TreeItemNode()
                            {
                                Header  = "0-0-1-2",
                                ItemKey = "0-0-1-2",
                            }
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header  ="0-0-2",
                        ItemKey ="0-0-2",
                    },
                ]
            },
            new TreeItemNode()
            {
                Header  = "0-1",
                ItemKey = "0-1",
                Children = [
                    new TreeItemNode()
                    {
                        Header  = "0-1-0",
                        ItemKey = "0-1-0",
                        Children = [
                            new TreeItemNode()
                            {
                                Header  = "0-1-0-0",
                                ItemKey = "0-1-0-0",
                            },
                            new TreeItemNode()
                            {
                                Header  = "0-1-0-1",
                                ItemKey = "0-1-0-1",
                            },
                            new TreeItemNode()
                            {
                                Header  = "0-1-0-2",
                                ItemKey = "0-1-0-2",
                            }
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header  = "0-1-1",
                        ItemKey = "0-1-1",
                        Children = [
                            new TreeItemNode()
                            {
                                Header  = "0-1-1-0",
                                ItemKey = "0-1-1-0",
                            },
                            new TreeItemNode()
                            {
                                Header  = "0-1-1-1",
                                ItemKey = "0-1-1-1",
                            },
                            new TreeItemNode()
                            {
                                Header  = "0-1-1-2",
                                ItemKey = "0-1-1-2",
                            }
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header  ="0-1-2",
                        ItemKey ="0-1-2",
                    },
                ]
            },
            new TreeItemNode()
            {
                Header  = "0-2",
                ItemKey = "0-2",
            }
        ];
    }

    private void HandleFilterItemsSourceTreeClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchTreeViewByItemsSource.FilterValue = searchEdit.Text?.Trim();
        }
    }
    
    private void HandleFilterTreeClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is SearchEdit searchEdit)
        {
            SearchTreeView.FilterValue = searchEdit.Text?.Trim();
        }
    }

    private TreeViewItem? _contextMenuTargetItem;

    private void HandleContextMenuTreeItemContextMenuRequest(object? sender, TreeItemContextMenuEventArgs e)
    {
        _contextMenuTargetItem = e.ViewItem;
        if (ContextMenuTree.Resources.TryGetValue("TreeItemContextMenu", out var resource) &&
            resource is MenuFlyout flyout)
        {
            flyout.ShowAt(e.ViewItem);
        }
    }

    private void HandleContextMenuNewNodeClick(object? sender, RoutedEventArgs e)
    {
        if (_contextMenuTargetItem is null)
        {
            return;
        }

        var header = _contextMenuTargetItem.Header?.ToString() ??
                     Lang(TreeViewShowCaseLangResourceKind.P2HeaderNodeFallback, "node");
        var newItem = new TreeViewItem
        {
            Header = string.Format(
                Lang(TreeViewShowCaseLangResourceKind.P2HeaderNewNodeFormat, "{0} / new ({1})"),
                header,
                _contextMenuTargetItem.Items.Count + 1)
        };
        _contextMenuTargetItem.Items.Add(newItem);
        _contextMenuTargetItem.IsExpanded = true;
    }

    private void HandleContextMenuRenameClick(object? sender, RoutedEventArgs e)
    {
        if (_contextMenuTargetItem is null)
        {
            return;
        }

        var header = _contextMenuTargetItem.Header?.ToString() ??
                     Lang(TreeViewShowCaseLangResourceKind.P2HeaderNodeFallback, "node");
        _contextMenuTargetItem.Header = string.Format(
            Lang(TreeViewShowCaseLangResourceKind.P2HeaderRenamedFormat, "{0} (renamed)"),
            header);
    }

    private void HandleContextMenuDeleteClick(object? sender, RoutedEventArgs e)
    {
        if (_contextMenuTargetItem is null)
        {
            return;
        }

        if (_contextMenuTargetItem.Parent is TreeViewItem parentItem)
        {
            parentItem.Items.Remove(_contextMenuTargetItem);
        }
        else if (_contextMenuTargetItem.Parent is AtomUITreeView parentTree)
        {
            parentTree.Items.Remove(_contextMenuTargetItem);
        }
        _contextMenuTargetItem = null;
    }
}

internal static class TreeViewShowCaseLanguage
{
    public static string Get(TreeViewShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
