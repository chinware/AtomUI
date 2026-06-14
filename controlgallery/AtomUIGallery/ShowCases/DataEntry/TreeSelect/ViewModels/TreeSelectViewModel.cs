using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.TreeSelect;

public class TreeSelectViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "TreeSelect";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<TreeSelectApiRow>? _apiRows;

    public ObservableCollection<TreeSelectApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    private ObservableCollection<TreeSelectDesignTokenRow>? _designTokenRows;

    public ObservableCollection<TreeSelectDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private List<ITreeItemNode>? _basicTreeNodes = [];

    public List<ITreeItemNode>? BasicTreeNodes
    {
        get => _basicTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _basicTreeNodes, value);
    }

    private IList<ITreeItemNode>? _selectedItems;

    public IList<ITreeItemNode>? SelectedItems
    {
        get => _selectedItems;
        set => this.RaiseAndSetIfChanged(ref _selectedItems, value);
    }

    private List<ITreeItemNode>? _multiSelectionTreeNodes = [];

    public List<ITreeItemNode>? MultiSelectionTreeNodes
    {
        get => _multiSelectionTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _multiSelectionTreeNodes, value);
    }

    private List<ITreeItemNode>? _itemsSourceTreeNodes = [];

    public List<ITreeItemNode>? ItemsSourceTreeNodes
    {
        get => _itemsSourceTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _itemsSourceTreeNodes, value);
    }

    private List<ITreeItemNode>? _checkableTreeNodes = [];

    public List<ITreeItemNode>? CheckableTreeNodes
    {
        get => _checkableTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _checkableTreeNodes, value);
    }

    private List<ITreeItemNode>? _asyncLoadTreeNodes = [];

    public List<ITreeItemNode>? AsyncLoadTreeNodes
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

    private List<ITreeItemNode>? _showTreeLineTreeNodes = [];

    public List<ITreeItemNode>? ShowTreeLineTreeNodes
    {
        get => _showTreeLineTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _showTreeLineTreeNodes, value);
    }

    private List<ITreeItemNode>? _leftAddTreeNodes = [];

    public List<ITreeItemNode>? LeftAddTreeNodes
    {
        get => _leftAddTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _leftAddTreeNodes, value);
    }

    private List<ITreeItemNode>? _contentLeftAddTreeNodes = [];

    public List<ITreeItemNode>? ContentLeftAddTreeNodes
    {
        get => _contentLeftAddTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _contentLeftAddTreeNodes, value);
    }

    private List<ITreeItemNode>? _placementTreeNodes = [];

    public List<ITreeItemNode>? PlacementTreeNodes
    {
        get => _placementTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _placementTreeNodes, value);
    }

    private SelectPopupPlacement _placement;

    public SelectPopupPlacement Placement
    {
        get => _placement;
        set => this.RaiseAndSetIfChanged(ref _placement, value);
    }

    private List<ITreeItemNode>? _maxSelectedTreeNodes = [];

    public List<ITreeItemNode>? MaxSelectedTreeNodes
    {
        get => _maxSelectedTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _maxSelectedTreeNodes, value);
    }

    private List<ITreeItemNode>? _maxCheckedTreeNodes = [];

    public List<ITreeItemNode>? MaxCheckedTreeNodes
    {
        get => _maxCheckedTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _maxCheckedTreeNodes, value);
    }

    public TreeSelectViewModel(IScreen screen)
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
            new TreeSelectApiRow("ItemsSource", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable<ITreeItemNode>?", "cyan", "null"),
            new TreeSelectApiRow("SelectedItems", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertySelectedItems), "IList<ITreeItemNode>?", "cyan", "null"),
            new TreeSelectApiRow("IsMultiple", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyIsMultiple), "bool", "green", "false"),
            new TreeSelectApiRow("IsTreeCheckable", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyIsTreeCheckable), "bool", "green", "false"),
            new TreeSelectApiRow("IsDefaultExpandAll", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyIsDefaultExpandAll), "bool", "green", "false"),
            new TreeSelectApiRow("IsAllowClear", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "green", "false"),
            new TreeSelectApiRow("IsFilterEnabled", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyIsFilterEnabled), "bool", "green", "false"),
            new TreeSelectApiRow("DataLoader", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyDataLoader), "ITreeItemNodeLoader?", "cyan", "null"),
            new TreeSelectApiRow("Placement", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyPlacement), "SelectPopupPlacement", "purple", "BottomEdgeAlignedLeft"),
            new TreeSelectApiRow("MaxCount", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyMaxCount), "int", "green", "0"),
            new TreeSelectApiRow("StyleVariant", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "purple", "Outlined"),
            new TreeSelectApiRow("Status", Lang(TreeSelectShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "purple", "Default")
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
            new TreeSelectDesignTokenRow("MinPopupWidth", Lang(TreeSelectShowCaseLangResourceKind.TokenNameMinPopupWidth), Lang(TreeSelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TreeSelectShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TreeSelectShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TreeSelectShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TreeSelectShowCaseLangResourceKind.ApiPropertyItemsSource          => en_US.ApiPropertyItemsSource,
            TreeSelectShowCaseLangResourceKind.ApiPropertySelectedItems        => en_US.ApiPropertySelectedItems,
            TreeSelectShowCaseLangResourceKind.ApiPropertyIsMultiple           => en_US.ApiPropertyIsMultiple,
            TreeSelectShowCaseLangResourceKind.ApiPropertyIsTreeCheckable      => en_US.ApiPropertyIsTreeCheckable,
            TreeSelectShowCaseLangResourceKind.ApiPropertyIsDefaultExpandAll   => en_US.ApiPropertyIsDefaultExpandAll,
            TreeSelectShowCaseLangResourceKind.ApiPropertyIsAllowClear         => en_US.ApiPropertyIsAllowClear,
            TreeSelectShowCaseLangResourceKind.ApiPropertyIsFilterEnabled      => en_US.ApiPropertyIsFilterEnabled,
            TreeSelectShowCaseLangResourceKind.ApiPropertyDataLoader           => en_US.ApiPropertyDataLoader,
            TreeSelectShowCaseLangResourceKind.ApiPropertyPlacement            => en_US.ApiPropertyPlacement,
            TreeSelectShowCaseLangResourceKind.ApiPropertyMaxCount             => en_US.ApiPropertyMaxCount,
            TreeSelectShowCaseLangResourceKind.ApiPropertyStyleVariant         => en_US.ApiPropertyStyleVariant,
            TreeSelectShowCaseLangResourceKind.ApiPropertyStatus               => en_US.ApiPropertyStatus,
            TreeSelectShowCaseLangResourceKind.TokenNameMinPopupWidth          => en_US.TokenNameMinPopupWidth,
            TreeSelectShowCaseLangResourceKind.TokenScopeComponent             => en_US.TokenScopeComponent,
            TreeSelectShowCaseLangResourceKind.TokenStatusStable               => en_US.TokenStatusStable,
            _                                                                  => kind.ToString()
        };
    }
}

public sealed record TreeSelectApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TreeSelectDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
