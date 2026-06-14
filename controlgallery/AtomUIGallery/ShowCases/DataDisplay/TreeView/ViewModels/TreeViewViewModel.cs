using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.TreeView;

public class TreeViewViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "TreeView";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private IList<TreeNodePath>? _basicTreeViewDefaultExpandedPaths;
    public IList<TreeNodePath>? BasicTreeViewDefaultExpandedPaths
    {
        get => _basicTreeViewDefaultExpandedPaths;
        set => this.RaiseAndSetIfChanged(ref _basicTreeViewDefaultExpandedPaths, value);
    }

    private IList<TreeNodePath>? _basicTreeViewDefaultSelectedPaths;
    public IList<TreeNodePath>? BasicTreeViewDefaultSelectedPaths
    {
        get => _basicTreeViewDefaultSelectedPaths;
        set => this.RaiseAndSetIfChanged(ref _basicTreeViewDefaultSelectedPaths, value);
    }

    private IList<TreeNodePath>? _basicTreeViewDefaultCheckedPaths;
    public IList<TreeNodePath>? BasicTreeViewDefaultCheckedPaths
    {
        get => _basicTreeViewDefaultCheckedPaths;
        set => this.RaiseAndSetIfChanged(ref _basicTreeViewDefaultCheckedPaths, value);
    }

    private IList<ITreeItemNode>? _basicTreeNodes;
    public IList<ITreeItemNode>? BasicTreeNodes
    {
        get => _basicTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _basicTreeNodes, value);
    }

    private IList<TreeNodePath>? _customizeCollapseExpandTreeDefaultExpandedPaths;
    public IList<TreeNodePath>? CustomizeCollapseExpandTreeDefaultExpandedPaths
    {
        get => _customizeCollapseExpandTreeDefaultExpandedPaths;
        set => this.RaiseAndSetIfChanged(ref _customizeCollapseExpandTreeDefaultExpandedPaths, value);
    }

    private IList<ITreeItemNode>? _asyncLoadTreeNodes;
    public IList<ITreeItemNode>? AsyncLoadTreeNodes
    {
        get => _asyncLoadTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _asyncLoadTreeNodes, value);
    }

    private ITreeItemNodeLoader? _asyncLoadTreeNodeLoader;
    public ITreeItemNodeLoader? AsyncLoadTreeNodeLoader
    {
        get => _asyncLoadTreeNodeLoader;
        set => this.RaiseAndSetIfChanged(ref _asyncLoadTreeNodeLoader, value);
    }

    private IList<ITreeItemNode>? _filterTreeNodes;
    public IList<ITreeItemNode>? FilterTreeNodes
    {
        get => _filterTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _filterTreeNodes, value);
    }

    private ObservableCollection<TreeViewApiRow>? _apiRows;
    private ObservableCollection<TreeViewDesignTokenRow>? _designTokenRows;

    public ObservableCollection<TreeViewApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<TreeViewDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private bool _showLineSwitchChecked = true;
    public bool ShowLineSwitchChecked
    {
        get => _showLineSwitchChecked;
        set => this.RaiseAndSetIfChanged(ref _showLineSwitchChecked, value);
    }

    private bool _showIconSwitchChecked;
    public bool ShowIconSwitchChecked
    {
        get => _showIconSwitchChecked;
        set => this.RaiseAndSetIfChanged(ref _showIconSwitchChecked, value);
    }

    private bool _showLeafIconSwitchChecked;
    public bool ShowLeafIconSwitchChecked
    {
        get => _showLeafIconSwitchChecked;
        set => this.RaiseAndSetIfChanged(ref _showLeafIconSwitchChecked, value);
    }

    private TreeItemHoverMode _treeViewNodeHoverMode = TreeItemHoverMode.Default;
    public TreeItemHoverMode TreeViewNodeHoverMode
    {
        get => _treeViewNodeHoverMode;
        set => this.RaiseAndSetIfChanged(ref _treeViewNodeHoverMode, value);
    }

    public TreeViewViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new TreeViewApiRow("ItemsSource", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "blue", "null"),
            new TreeViewApiRow("ItemTemplate", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyItemTemplate), "TreeDataTemplate?", "cyan", "null"),
            new TreeViewApiRow("ToggleType", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyToggleType), "ItemToggleType", "blue", "None"),
            new TreeViewApiRow("DefaultExpandedPaths", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyDefaultExpandedPaths), "IList<TreeNodePath>?", "cyan", "null"),
            new TreeViewApiRow("DefaultSelectedPaths", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyDefaultSelectedPaths), "IList<TreeNodePath>?", "cyan", "null"),
            new TreeViewApiRow("DefaultCheckedPaths", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyDefaultCheckedPaths), "IList<TreeNodePath>?", "cyan", "null"),
            new TreeViewApiRow("IsShowLine", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyIsShowLine), "bool", "purple", "false"),
            new TreeViewApiRow("IsShowIcon", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyIsShowIcon), "bool", "purple", "false"),
            new TreeViewApiRow("IsShowLeafIcon", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyIsShowLeafIcon), "bool", "purple", "false"),
            new TreeViewApiRow("NodeHoverMode", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyNodeHoverMode), "TreeItemHoverMode", "blue", "Default"),
            new TreeViewApiRow("IsDraggable", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyIsDraggable), "bool", "purple", "false"),
            new TreeViewApiRow("DataLoader", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyDataLoader), "ITreeItemNodeLoader?", "cyan", "null"),
            new TreeViewApiRow("FilterValue", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyFilterValue), "object?", "cyan", "null"),
            new TreeViewApiRow("FilterStrategy", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyFilterStrategy), "TreeFilterStrategy", "blue", "All"),
            new TreeViewApiRow("IsSelectOnRightClick", Lang(TreeViewShowCaseLangResourceKind.ApiPropertyIsSelectOnRightClick), "bool", "purple", "true")
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
            new TreeViewDesignTokenRow("HeaderHeight", Lang(TreeViewShowCaseLangResourceKind.TokenNameHeaderHeight), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("NodeHoverBg", Lang(TreeViewShowCaseLangResourceKind.TokenNameNodeHoverBg), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("NodeSelectedBg", Lang(TreeViewShowCaseLangResourceKind.TokenNameNodeSelectedBg), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("DirectoryNodeSelectedColor", Lang(TreeViewShowCaseLangResourceKind.TokenNameDirectoryNodeSelectedColor), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("DirectoryNodeSelectedBg", Lang(TreeViewShowCaseLangResourceKind.TokenNameDirectoryNodeSelectedBg), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("TreeItemMargin", Lang(TreeViewShowCaseLangResourceKind.TokenNameTreeItemMargin), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("TreeItemHeaderPadding", Lang(TreeViewShowCaseLangResourceKind.TokenNameTreeItemHeaderPadding), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("TreeItemHeaderMargin", Lang(TreeViewShowCaseLangResourceKind.TokenNameTreeItemHeaderMargin), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("TreeNodeSwitcherMargin", Lang(TreeViewShowCaseLangResourceKind.TokenNameTreeNodeSwitcherMargin), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("TreeNodeIconMargin", Lang(TreeViewShowCaseLangResourceKind.TokenNameTreeNodeIconMargin), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("DragIndicatorLineWidth", Lang(TreeViewShowCaseLangResourceKind.TokenNameDragIndicatorLineWidth), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TreeViewDesignTokenRow("FilterHighlightColor", Lang(TreeViewShowCaseLangResourceKind.TokenNameFilterHighlightColor), Lang(TreeViewShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeViewShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TreeViewShowCaseLangResourceKind kind)
    {
        return TreeViewShowCase.Lang(kind, FallbackLang(kind));
    }

    private static string FallbackLang(TreeViewShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TreeViewShowCaseLangResourceKind.ApiPropertyItemsSource              => en_US.ApiPropertyItemsSource,
            TreeViewShowCaseLangResourceKind.ApiPropertyItemTemplate             => en_US.ApiPropertyItemTemplate,
            TreeViewShowCaseLangResourceKind.ApiPropertyToggleType               => en_US.ApiPropertyToggleType,
            TreeViewShowCaseLangResourceKind.ApiPropertyDefaultExpandedPaths     => en_US.ApiPropertyDefaultExpandedPaths,
            TreeViewShowCaseLangResourceKind.ApiPropertyDefaultSelectedPaths     => en_US.ApiPropertyDefaultSelectedPaths,
            TreeViewShowCaseLangResourceKind.ApiPropertyDefaultCheckedPaths      => en_US.ApiPropertyDefaultCheckedPaths,
            TreeViewShowCaseLangResourceKind.ApiPropertyIsShowLine               => en_US.ApiPropertyIsShowLine,
            TreeViewShowCaseLangResourceKind.ApiPropertyIsShowIcon               => en_US.ApiPropertyIsShowIcon,
            TreeViewShowCaseLangResourceKind.ApiPropertyIsShowLeafIcon           => en_US.ApiPropertyIsShowLeafIcon,
            TreeViewShowCaseLangResourceKind.ApiPropertyNodeHoverMode            => en_US.ApiPropertyNodeHoverMode,
            TreeViewShowCaseLangResourceKind.ApiPropertyIsDraggable              => en_US.ApiPropertyIsDraggable,
            TreeViewShowCaseLangResourceKind.ApiPropertyDataLoader               => en_US.ApiPropertyDataLoader,
            TreeViewShowCaseLangResourceKind.ApiPropertyFilterValue              => en_US.ApiPropertyFilterValue,
            TreeViewShowCaseLangResourceKind.ApiPropertyFilterStrategy           => en_US.ApiPropertyFilterStrategy,
            TreeViewShowCaseLangResourceKind.ApiPropertyIsSelectOnRightClick     => en_US.ApiPropertyIsSelectOnRightClick,
            TreeViewShowCaseLangResourceKind.TokenNameHeaderHeight               => en_US.TokenNameHeaderHeight,
            TreeViewShowCaseLangResourceKind.TokenNameNodeHoverBg                => en_US.TokenNameNodeHoverBg,
            TreeViewShowCaseLangResourceKind.TokenNameNodeSelectedBg             => en_US.TokenNameNodeSelectedBg,
            TreeViewShowCaseLangResourceKind.TokenNameDirectoryNodeSelectedColor => en_US.TokenNameDirectoryNodeSelectedColor,
            TreeViewShowCaseLangResourceKind.TokenNameDirectoryNodeSelectedBg    => en_US.TokenNameDirectoryNodeSelectedBg,
            TreeViewShowCaseLangResourceKind.TokenNameTreeItemMargin             => en_US.TokenNameTreeItemMargin,
            TreeViewShowCaseLangResourceKind.TokenNameTreeItemHeaderPadding      => en_US.TokenNameTreeItemHeaderPadding,
            TreeViewShowCaseLangResourceKind.TokenNameTreeItemHeaderMargin       => en_US.TokenNameTreeItemHeaderMargin,
            TreeViewShowCaseLangResourceKind.TokenNameTreeNodeSwitcherMargin     => en_US.TokenNameTreeNodeSwitcherMargin,
            TreeViewShowCaseLangResourceKind.TokenNameTreeNodeIconMargin         => en_US.TokenNameTreeNodeIconMargin,
            TreeViewShowCaseLangResourceKind.TokenNameDragIndicatorLineWidth     => en_US.TokenNameDragIndicatorLineWidth,
            TreeViewShowCaseLangResourceKind.TokenNameFilterHighlightColor       => en_US.TokenNameFilterHighlightColor,
            TreeViewShowCaseLangResourceKind.TokenScopeComponent                 => en_US.TokenScopeComponent,
            TreeViewShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }
}

public sealed record TreeViewApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TreeViewDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);

public class TreeItemDataLoader : ITreeItemNodeLoader
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
                    Header  = TreeViewShowCaseLanguage.Get(TreeViewShowCaseLangResourceKind.P2HeaderChildNode,
                        "Child Node")
                },
                new TreeItemNode()
                {
                    ItemKey = $"{targetTreeItemData.ItemKey}-1",
                    Header  = TreeViewShowCaseLanguage.Get(TreeViewShowCaseLangResourceKind.P2HeaderChildNode,
                        "Child Node")
                }
            ]);
        }
        return new TreeItemLoadResult()
        {
            IsSuccess = true,
            Data = children
        };
    }
}
