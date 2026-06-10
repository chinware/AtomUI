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
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.TreeView;

public partial class TreeViewShowCase : GalleryReactiveUserControl<TreeViewViewModel>
{
    public const string LanguageId = nameof(TreeViewShowCase);

    private const string BasicScenario    = "Basic";
    private const string AdvancedScenario = "Advanced";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

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
        EnsureSelectedScenarioContent();
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
            BasicScenario    => new TreeViewBasicShowCase(),
            AdvancedScenario => new TreeViewAdvancedShowCase(),
            _                => throw new InvalidOperationException($"Unknown TreeView scenario: {scenario}")
        };
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
        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
