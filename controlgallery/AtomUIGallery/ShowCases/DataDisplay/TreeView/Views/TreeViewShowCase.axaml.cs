using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using Avalonia.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.TreeView;

public partial class TreeViewShowCase : GalleryReactiveUserControl<TreeViewViewModel>
{
    public const string LanguageId = nameof(TreeViewShowCase);

    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);
    private AtomUI.Desktop.Controls.TreeViewItem? _contextMenuTargetItem;

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
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
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
        _contextMenuTargetItem = null;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ExamplesContent.DataContext = DataContext;
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            content.DataContext = DataContext;
        }
        EnsureSelectedScenarioContent();
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
            ApiScenario         => new TreeViewApiDataGrid(),
            DesignTokenScenario => new TreeViewDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown TreeView scenario: {scenario}")
        };
    }

    private void HandleHoverModeChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUIRadioButton radioButton &&
            radioButton.IsChecked == true &&
            radioButton.Tag is TreeItemHoverMode hoverMode &&
            DataContext is TreeViewViewModel viewModel)
        {
            viewModel.TreeViewNodeHoverMode = hoverMode;
        }
    }

    private void HandleFilterItemsSourceTreeClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is SearchEdit searchEdit &&
            TryFindTemplateTreeView(searchEdit, "SearchTreeViewByItemsSource", out var treeView))
        {
            treeView.FilterValue = searchEdit.Text?.Trim();
        }
    }

    private void HandleFilterTreeClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is SearchEdit searchEdit &&
            TryFindTemplateTreeView(searchEdit, "SearchTreeView", out var treeView))
        {
            treeView.FilterValue = searchEdit.Text?.Trim();
        }
    }

    private static bool TryFindTemplateTreeView(Control source, string treeViewName, out AtomUITreeView treeView)
    {
        var parent = source.Parent as Control;
        while (parent is not null)
        {
            if (parent is AtomUITreeView directTreeView &&
                directTreeView.Name == treeViewName)
            {
                treeView = directTreeView;
                return true;
            }

            var descendantTreeView = parent.GetVisualDescendants()
                                           .OfType<AtomUITreeView>()
                                           .FirstOrDefault(candidate => candidate.Name == treeViewName);
            if (descendantTreeView is not null)
            {
                treeView = descendantTreeView;
                return true;
            }

            parent = parent.Parent as Control;
        }

        treeView = null!;
        return false;
    }

    private void HandleContextMenuTreeItemContextMenuRequest(object? sender, TreeItemContextMenuEventArgs e)
    {
        _contextMenuTargetItem = e.ViewItem;
        if (sender is AtomUITreeView treeView &&
            treeView.Resources.TryGetValue("TreeItemContextMenu", out var resource) &&
            resource is AtomUI.Desktop.Controls.MenuFlyout flyout)
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
        var newItem = new AtomUI.Desktop.Controls.TreeViewItem
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

        if (_contextMenuTargetItem.Parent is AtomUI.Desktop.Controls.TreeViewItem parentItem)
        {
            parentItem.Items.Remove(_contextMenuTargetItem);
        }
        else if (_contextMenuTargetItem.Parent is AtomUITreeView parentTree)
        {
            parentTree.Items.Remove(_contextMenuTargetItem);
        }
        _contextMenuTargetItem = null;
    }

    private void RefreshLocalizedTreeNodes(TreeViewViewModel viewModel)
    {
        InitBasicTreeNodes(viewModel);
        InitAsyncLoadTreeNodes(viewModel);
    }

    internal static string Lang(TreeViewShowCaseLangResourceKind resourceKind, string fallback)
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

}

internal static class TreeViewShowCaseLanguage
{
    public static string Get(TreeViewShowCaseLangResourceKind resourceKind, string fallback)
    {
        if (Application.Current is null)
        {
            return fallback;
        }

        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
