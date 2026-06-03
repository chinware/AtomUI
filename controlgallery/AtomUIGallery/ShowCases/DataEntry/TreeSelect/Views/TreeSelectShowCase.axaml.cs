using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Language;
using Avalonia;
using AtomUIGallery.Localization;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.TreeSelect;

public partial class TreeSelectShowCase : ReactiveUserControl<TreeSelectViewModel>
{
    public const string LanguageId = nameof(TreeSelectShowCase);

    public TreeSelectShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is TreeSelectViewModel viewModel)
            {
                RefreshLocalizedTreeNodes(viewModel);
                viewModel.AsyncLoadTreeNodeLoader = new LocalizedTreeSelectItemDataLoader();

                this.OneWayBind(ViewModel, vm => vm.BasicTreeNodes, v => v.BasicTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.MultiSelectionTreeNodes, v => v.MultiSelectionTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ItemsSourceTreeNodes, v => v.ItemsSourceTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.CheckableTreeNodes, v => v.CheckableTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.AsyncLoadTreeNodes, v => v.AsyncLoadTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.AsyncLoadTreeNodeLoader, v => v.AsyncLoadTreeSelect.DataLoader)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.PlacementTreeNodes, v => v.PlacementTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Placement, v => v.PlacementTreeSelect.Placement)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ShowTreeLineTreeNodes, v => v.ShowTreeLineTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.LeftAddTreeNodes, v => v.LeftAddTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.ContentLeftAddTreeNodes, v => v.ContentLeftAddTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.MaxSelectedTreeNodes, v => v.MaxSelectedTreeSelect.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.MaxCheckedTreeNodes, v => v.MaxCheckedTreeSelect.ItemsSource)
                    .DisposeWith(disposables);

                PlacementOptionGroup.OptionCheckedChanged += HandlePlacementOptionCheckedChanged;
                viewModel.Placement = SelectPopupPlacement.TopEdgeAlignedLeft;

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
                    PlacementOptionGroup.OptionCheckedChanged -= HandlePlacementOptionCheckedChanged;
                    viewModel.AsyncLoadTreeNodeLoader = null;
                    viewModel.BasicTreeNodes          = null;
                    viewModel.MultiSelectionTreeNodes = null;
                    viewModel.ItemsSourceTreeNodes    = null;
                    viewModel.CheckableTreeNodes      = null;
                    viewModel.AsyncLoadTreeNodes      = null;
                    viewModel.ShowTreeLineTreeNodes   = null;
                    viewModel.LeftAddTreeNodes        = null;
                    viewModel.ContentLeftAddTreeNodes = null;
                    viewModel.PlacementTreeNodes      = null;
                    viewModel.MaxSelectedTreeNodes    = null;
                    viewModel.MaxCheckedTreeNodes     = null;
                }).DisposeWith(disposables);
            }
        });

        InitializeComponent();
    }

    public void HandlePlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (DataContext is TreeSelectViewModel vm)
        {
            if (args.Index == 0)
            {
                vm.Placement = SelectPopupPlacement.TopEdgeAlignedLeft;
            }
            else if (args.Index == 1)
            {
                vm.Placement = SelectPopupPlacement.TopEdgeAlignedRight;
            }
            else if (args.Index == 2)
            {
                vm.Placement = SelectPopupPlacement.BottomEdgeAlignedLeft;
            }
            else
            {
                vm.Placement = SelectPopupPlacement.BottomEdgeAlignedRight;
            }
        }
    }

    private void RefreshLocalizedTreeNodes(TreeSelectViewModel viewModel)
    {
        InitBasicTreeNodes(viewModel);
        InitMultiTreeNodes(viewModel);
        InitItemsSourceTreeNodes(viewModel);
        InitCheckableTreeNodes(viewModel);
        InitAsyncLoadTreeNodes(viewModel);
        InitShowLineTreeNodes(viewModel);
        InitLeftAddOnTreeNodes(viewModel);
        InitContentLeftAddOnTreeNodes(viewModel);
        InitPlacementTreeNodes(viewModel);
        InitMaxSelectedTreeNodes(viewModel);
        InitMaxCheckedTreeNodes(viewModel);
    }

    private static string Lang(TreeSelectShowCaseLangResourceKind resourceKind, string fallback)
    {
        return TreeSelectShowCaseLanguage.Get(resourceKind, fallback);
    }

    private void InitBasicTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.BasicTreeNodes = CreateBasicTreeNodes();
    }

    private void InitMultiTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.MultiSelectionTreeNodes = CreatePersonalLeafTreeNodes();
    }

    private void InitLeftAddOnTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.LeftAddTreeNodes = CreatePersonalLeafTreeNodes();
    }

    private void InitContentLeftAddOnTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.ContentLeftAddTreeNodes = CreatePersonalLeafTreeNodes();
    }

    private void InitItemsSourceTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.ItemsSourceTreeNodes =
        [
            new TreeItemNode()
            {
                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderNode1, "Node 1"),
                Value  = "0-0",
                Children = [
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode1, "Child Node 1"),
                        Value  = "0-0-1",
                    },
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode2, "Child Node 2"),
                        Value  = "0-0-2",
                    }
                ]
            },
            new TreeItemNode()
            {
                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderNode2, "Node 2"),
                Value  = "0-1",
            }
        ];
    }

    private void InitCheckableTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.CheckableTreeNodes =
        [
            new TreeItemNode()
            {
                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderNode1, "Node 1"),
                ItemKey = "0-0",
                Children = [
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode1, "Child Node 1"),
                        Value  = "0-0-0",
                    }
                ]
            },
            new TreeItemNode()
            {
                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderNode2, "Node 2"),
                ItemKey = "0-1",
                Children = [
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode3, "Child Node 3"),
                        Value  = "0-1-0",
                    },
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode4, "Child Node 4"),
                        Value  = "0-1-1",
                        Children = [
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode6, "Child Node 6"),
                                Value  = "0-1-1-0",
                            },
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode7, "Child Node 7"),
                                Value  = "0-1-1-1",
                            },
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode5, "Child Node 5"),
                        Value  = "0-1-2",
                    }
                ]
            }
        ];
    }

    private void InitAsyncLoadTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.AsyncLoadTreeNodes =
        [
            new TreeItemNode()
            {
                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderExpandToLoad, "Expand to load"),
                ItemKey = "0",
            },
            new TreeItemNode()
            {
                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderExpandToLoad, "Expand to load"),
                ItemKey = "1",
            },
            new TreeItemNode()
            {
                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderTreeNode, "Tree Node"),
                ItemKey = "2",
                IsLeaf  = true
            }
        ];
    }

    private void InitShowLineTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.ShowTreeLineTreeNodes = CreateLineTreeNodes(true);
    }

    private void InitPlacementTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.PlacementTreeNodes = CreateLineTreeNodes(false);
    }

    private void InitMaxSelectedTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.MaxSelectedTreeNodes = CreateLineTreeNodes(false);
    }

    private void InitMaxCheckedTreeNodes(TreeSelectViewModel viewModel)
    {
        viewModel.MaxCheckedTreeNodes = CreateLineTreeNodes(false);
    }

    private static List<ITreeItemNode> CreateBasicTreeNodes()
    {
        return
        [
            new TreeItemNode()
            {
                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent1, "Parent 1"),
                Value  = "parent 1",
                Children = [
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent10, "Parent 1-0"),
                        Value  = "parent 1-0",
                        Children = [
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf1, "Leaf 1"),
                                Value  = "leaf1",
                            },
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf2, "Leaf 2"),
                                Value  = "leaf2",
                            },
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf3, "Leaf 3"),
                                Value  = "leaf3",
                            },
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf4, "Leaf 4"),
                                Value  = "leaf4",
                            },
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf5, "Leaf 5"),
                                Value  = "leaf5",
                            },
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf6, "Leaf 6"),
                                Value  = "leaf6",
                            },
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent11, "Parent 1-1"),
                        Value  = "parent 1-1",
                        Children = [
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf11, "Leaf 11"),
                                Value  = "leaf11",
                            }
                        ]
                    }
                ]
            }
        ];
    }

    private static List<ITreeItemNode> CreatePersonalLeafTreeNodes()
    {
        return
        [
            new TreeItemNode()
            {
                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent1, "Parent 1"),
                Value  = "parent 1",
                Children = [
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent10, "Parent 1-0"),
                        Value  = "parent 1-0",
                        Children = [
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderMyLeaf, "My leaf"),
                                Value  = "leaf1",
                            },
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderYourLeaf, "Your leaf"),
                                Value  = "leaf2",
                            }
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent11, "Parent 1-1"),
                        Value  = "parent 1-1",
                        Children = [
                            new TreeItemNode()
                            {
                                Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderSss, "Node SSS"),
                                Value  = "sss",
                            }
                        ]
                    }
                ]
            },
        ];
    }

    private static List<ITreeItemNode> CreateLineTreeNodes(bool includeIcons)
    {
        return
        [
            new TreeItemNode()
            {
                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent1, "Parent 1"),
                ItemKey = "parent 1",
                Children = [
                    new TreeItemNode()
                    {
                        Header = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent10, "Parent 1-0"),
                        Value  = "parent 1-0",
                        Icon   = CreateCarryIcon(includeIcons),
                        Children = [
                            new TreeItemNode()
                            {
                                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf1, "Leaf 1"),
                                ItemKey = "Leaf1",
                                IsLeaf  = true,
                                Icon    = CreateCarryIcon(includeIcons),
                            },
                            new TreeItemNode()
                            {
                                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderLeaf2, "Leaf 2"),
                                ItemKey = "Leaf2",
                                IsLeaf  = true,
                                Icon    = CreateCarryIcon(includeIcons),
                            }
                        ]
                    },
                    new TreeItemNode()
                    {
                        Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderParent11, "Parent 1-1"),
                        ItemKey = "parent 1-1",
                        Icon    = CreateCarryIcon(includeIcons),
                        Children = [
                            new TreeItemNode()
                            {
                                Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderSss, "Node SSS"),
                                ItemKey = "sss",
                                IsLeaf  = true,
                                Icon    = CreateCarryIcon(includeIcons),
                            }
                        ]
                    },
                ]
            }
        ];
    }

    private static AtomUI.Controls.Icon? CreateCarryIcon(bool includeIcon)
    {
        return includeIcon ? new CarryOutOutlined() : null;
    }

    private sealed class LocalizedTreeSelectItemDataLoader : ITreeItemNodeLoader
    {
        public async Task<TreeItemLoadResult> LoadAsync(ITreeItemNode targetTreeItemData, CancellationToken token)
        {
            var level = 0;
            ITreeNode<ITreeItemNode>? current = targetTreeItemData;
            while (current != null)
            {
                level++;
                current = current.ParentNode;
            }
            await Task.Delay(TimeSpan.FromMilliseconds(600), token);
            var children = new List<TreeItemNode>();
            if (level < 3)
            {
                children.AddRange([
                    new TreeItemNode()
                    {
                        ItemKey = $"{targetTreeItemData.ItemKey}-0",
                        Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode, "Child Node")
                    },
                    new TreeItemNode()
                    {
                        ItemKey = $"{targetTreeItemData.ItemKey}-1",
                        Header  = Lang(TreeSelectShowCaseLangResourceKind.P2HeaderChildNode, "Child Node")
                    }
                ]);
            }
            return new TreeItemLoadResult()
            {
                IsSuccess = true,
                Data      = children
            };
        }
    }
}

internal static class TreeSelectShowCaseLanguage
{
    public static string Get(TreeSelectShowCaseLangResourceKind resourceKind, string fallback)
    {
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
