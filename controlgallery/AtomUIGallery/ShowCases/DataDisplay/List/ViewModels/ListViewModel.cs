using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUIGallery.Localization;
using Avalonia.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.List;

public class ListViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "List";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private List<IListItemData>? _listItems;

    public List<IListItemData>? ListItems
    {
        get => _listItems;
        set => this.RaiseAndSetIfChanged(ref _listItems, value);
    }

    private List<IListItemData>? _selectionListItems;

    public List<IListItemData>? SelectionListItems
    {
        get => _selectionListItems;
        set => this.RaiseAndSetIfChanged(ref _selectionListItems, value);
    }

    private ObservableCollection<IListItemData> _boundSelectedItems = [];

    public ObservableCollection<IListItemData> BoundSelectedItems
    {
        get => _boundSelectedItems;
        set
        {
            var nextSelectedItems = value ?? [];

            if (ReferenceEquals(_boundSelectedItems, nextSelectedItems))
            {
                return;
            }

            _boundSelectedItems.CollectionChanged -= HandleBoundSelectedItemsCollectionChanged;
            this.RaiseAndSetIfChanged(ref _boundSelectedItems, nextSelectedItems);
            _boundSelectedItems.CollectionChanged += HandleBoundSelectedItemsCollectionChanged;
            this.RaisePropertyChanged(nameof(BoundSelectedItemsText));
        }
    }

    public string BoundSelectedItemsText
    {
        get
        {
            if (BoundSelectedItems.Count == 0)
            {
                return Lang(ListShowCaseLangResourceKind.P2TextNoSelection);
            }

            return string.Join(
                ", ",
                BoundSelectedItems
                    .Select(item => item.Content?.ToString())
                    .Where(text => !string.IsNullOrWhiteSpace(text)));
        }
    }

    private List<IListItemData>? _listItemsWidthDisabled = [];

    public List<IListItemData>? ListItemsWidthDisabled
    {
        get => _listItemsWidthDisabled;
        set => this.RaiseAndSetIfChanged(ref _listItemsWidthDisabled, value);
    }

    private List<IListItemData>? _filteredGroupListItems;

    public List<IListItemData>? FilteredGroupListItems
    {
        get => _filteredGroupListItems;
        set => this.RaiseAndSetIfChanged(ref _filteredGroupListItems, value);
    }

    private List<IListItemData>? _orderedGroupListItems;

    public List<IListItemData>? OrderedGroupListItems
    {
        get => _orderedGroupListItems;
        set => this.RaiseAndSetIfChanged(ref _orderedGroupListItems, value);
    }

    private List<IListItemData>? _groupListItems;

    public List<IListItemData>? GroupListItems
    {
        get => _groupListItems;
        set => this.RaiseAndSetIfChanged(ref _groupListItems, value);
    }

    private List<IListItemData>? _emptyDemoItems;

    public List<IListItemData>? EmptyDemoItems
    {
        get => _emptyDemoItems;
        set => this.RaiseAndSetIfChanged(ref _emptyDemoItems, value);
    }

    private SelectionMode _selectionMode;
    private string? _searchFilterValue;
    private IList<IListSortDescription>? _orderedSortDescriptions;

    public SelectionMode SelectionMode
    {
        get => _selectionMode;
        set => this.RaiseAndSetIfChanged(ref _selectionMode, value);
    }

    public string? SearchFilterValue
    {
        get => _searchFilterValue;
        set => this.RaiseAndSetIfChanged(ref _searchFilterValue, value);
    }

    public IList<IListSortDescription>? OrderedSortDescriptions
    {
        get => _orderedSortDescriptions;
        set => this.RaiseAndSetIfChanged(ref _orderedSortDescriptions, value);
    }

    private List<IListItemData>? _basicListBoxItems;

    public List<IListItemData>? BasicListBoxItems
    {
        get => _basicListBoxItems;
        set => this.RaiseAndSetIfChanged(ref _basicListBoxItems, value);
    }

    private List<IListItemData>? _paginationListItems;

    public List<IListItemData>? PaginationListItems
    {
        get => _paginationListItems;
        set => this.RaiseAndSetIfChanged(ref _paginationListItems, value);
    }

    private ObservableCollection<ListApiRow>? _apiRows;
    private ObservableCollection<ListDesignTokenRow>? _designTokenRows;

    public ObservableCollection<ListApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ListDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ReactiveCommand<Unit, Unit> SelectBoundSelectedItemsCommand { get; }

    public ReactiveCommand<Unit, Unit> ClearBoundSelectedItemsCommand { get; }

    public ListViewModel(IScreen screen)
    {
        HostScreen                         = screen;
        SelectBoundSelectedItemsCommand    = ReactiveCommand.Create(SelectBoundSelectedItems);
        ClearBoundSelectedItemsCommand     = ReactiveCommand.Create(ClearBoundSelectedItems);
        _boundSelectedItems.CollectionChanged += HandleBoundSelectedItemsCollectionChanged;
    }

    public void ResetBoundSelectedItems()
    {
        if (SelectionListItems is { Count: > 1 })
        {
            BoundSelectedItems = new ObservableCollection<IListItemData>
            {
                SelectionListItems[1]
            };
            return;
        }

        BoundSelectedItems = [];
    }

    public void ClearBoundSelectedItems()
    {
        BoundSelectedItems.Clear();
    }

    private void SelectBoundSelectedItems()
    {
        BoundSelectedItems.Clear();

        if (SelectionListItems is not { Count: > 3 })
        {
            return;
        }

        BoundSelectedItems.Add(SelectionListItems[1]);
        BoundSelectedItems.Add(SelectionListItems[3]);
    }

    private void HandleBoundSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(BoundSelectedItemsText));
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new ListApiRow("IsSelectable", Lang(ListShowCaseLangResourceKind.ApiPropertyIsSelectable), "bool", "purple", "true"),
            new ListApiRow("SizeType", Lang(ListShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
            new ListApiRow("IsBorderless", Lang(ListShowCaseLangResourceKind.ApiPropertyIsBorderless), "bool", "purple", "false"),
            new ListApiRow("SelectionMode", Lang(ListShowCaseLangResourceKind.ApiPropertySelectionMode), "SelectionMode", "blue", "Single"),
            new ListApiRow("SelectedItems", Lang(ListShowCaseLangResourceKind.ApiPropertySelectedItems), "IList?", "cyan", "null"),
            new ListApiRow("IsShowSelectedIndicator", Lang(ListShowCaseLangResourceKind.ApiPropertyIsShowSelectedIndicator), "bool", "purple", "false"),
            new ListApiRow("IsShowEmptyIndicator", Lang(ListShowCaseLangResourceKind.ApiPropertyIsShowEmptyIndicator), "bool", "purple", "true"),
            new ListApiRow("IsGroupEnabled", Lang(ListShowCaseLangResourceKind.ApiPropertyIsGroupEnabled), "bool", "purple", "false"),
            new ListApiRow("GroupPropertySelector", Lang(ListShowCaseLangResourceKind.ApiPropertyGroupPropertySelector), "DefaultFilterValueSelector?", "cyan", "null"),
            new ListApiRow("SortDescriptions", Lang(ListShowCaseLangResourceKind.ApiPropertySortDescriptions), "IList<IListSortDescription>?", "cyan", "null"),
            new ListApiRow("Filter", Lang(ListShowCaseLangResourceKind.ApiPropertyFilter), "IValueFilter?", "cyan", "null"),
            new ListApiRow("FilterValue", Lang(ListShowCaseLangResourceKind.ApiPropertyFilterValue), "object?", "cyan", "null"),
            new ListApiRow("FilterValueSelector", Lang(ListShowCaseLangResourceKind.ApiPropertyFilterValueSelector), "DefaultFilterValueSelector?", "cyan", "null"),
            new ListApiRow("PaginationVisibility", Lang(ListShowCaseLangResourceKind.ApiPropertyPaginationVisibility), "ListPaginationVisibility", "blue", "Bottom"),
            new ListApiRow("PageSize", Lang(ListShowCaseLangResourceKind.ApiPropertyPageSize), "int", "blue", "0"),
            new ListApiRow("BottomPagination", Lang(ListShowCaseLangResourceKind.ApiPropertyBottomPagination), "AbstractPagination?", "cyan", "null"),
            new ListApiRow("ListBox.FilterHighlightStrategy", Lang(ListShowCaseLangResourceKind.ApiPropertyFilterHighlightStrategy), "TextBlockHighlightStrategy", "blue", "All")
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
            new ListDesignTokenRow("ContentPadding", Lang(ListShowCaseLangResourceKind.TokenNameContentPadding), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemColor", Lang(ListShowCaseLangResourceKind.TokenNameItemColor), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemHoverColor", Lang(ListShowCaseLangResourceKind.TokenNameItemHoverColor), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemSelectedColor", Lang(ListShowCaseLangResourceKind.TokenNameItemSelectedColor), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemDisabledColor", Lang(ListShowCaseLangResourceKind.TokenNameItemDisabledColor), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemBgColor", Lang(ListShowCaseLangResourceKind.TokenNameItemBgColor), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemHoverBgColor", Lang(ListShowCaseLangResourceKind.TokenNameItemHoverBgColor), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemSelectedBgColor", Lang(ListShowCaseLangResourceKind.TokenNameItemSelectedBgColor), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemPadding", Lang(ListShowCaseLangResourceKind.TokenNameItemPadding), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemPaddingSM", Lang(ListShowCaseLangResourceKind.TokenNameItemPaddingSM), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemPaddingLG", Lang(ListShowCaseLangResourceKind.TokenNameItemPaddingLG), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("ItemMargin", Lang(ListShowCaseLangResourceKind.TokenNameItemMargin), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("PaginationMargin", Lang(ListShowCaseLangResourceKind.TokenNamePaginationMargin), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("GroupHeaderColor", Lang(ListShowCaseLangResourceKind.TokenNameGroupHeaderColor), "ListView", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("SelectedIndicatorMargin", Lang(ListShowCaseLangResourceKind.TokenNameSelectedIndicatorMargin), "ListView/ListBox", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ListDesignTokenRow("FilterHighlightColor", Lang(ListShowCaseLangResourceKind.TokenNameFilterHighlightColor), "ListBox", "cyan", Lang(ListShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(ListShowCaseLangResourceKind kind)
    {
        return ListShowCase.Lang(kind, FallbackLang(kind));
    }

    private static string FallbackLang(ListShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ListShowCaseLangResourceKind.ApiPropertyIsSelectable             => en_US.ApiPropertyIsSelectable,
            ListShowCaseLangResourceKind.ApiPropertySizeType                 => en_US.ApiPropertySizeType,
            ListShowCaseLangResourceKind.ApiPropertyIsBorderless             => en_US.ApiPropertyIsBorderless,
            ListShowCaseLangResourceKind.ApiPropertySelectionMode            => en_US.ApiPropertySelectionMode,
            ListShowCaseLangResourceKind.ApiPropertySelectedItems             => en_US.ApiPropertySelectedItems,
            ListShowCaseLangResourceKind.ApiPropertyIsShowSelectedIndicator  => en_US.ApiPropertyIsShowSelectedIndicator,
            ListShowCaseLangResourceKind.ApiPropertyIsShowEmptyIndicator     => en_US.ApiPropertyIsShowEmptyIndicator,
            ListShowCaseLangResourceKind.ApiPropertyIsGroupEnabled           => en_US.ApiPropertyIsGroupEnabled,
            ListShowCaseLangResourceKind.ApiPropertyGroupPropertySelector    => en_US.ApiPropertyGroupPropertySelector,
            ListShowCaseLangResourceKind.ApiPropertySortDescriptions         => en_US.ApiPropertySortDescriptions,
            ListShowCaseLangResourceKind.ApiPropertyFilter                   => en_US.ApiPropertyFilter,
            ListShowCaseLangResourceKind.ApiPropertyFilterValue              => en_US.ApiPropertyFilterValue,
            ListShowCaseLangResourceKind.ApiPropertyFilterValueSelector      => en_US.ApiPropertyFilterValueSelector,
            ListShowCaseLangResourceKind.ApiPropertyPaginationVisibility     => en_US.ApiPropertyPaginationVisibility,
            ListShowCaseLangResourceKind.ApiPropertyPageSize                 => en_US.ApiPropertyPageSize,
            ListShowCaseLangResourceKind.ApiPropertyBottomPagination         => en_US.ApiPropertyBottomPagination,
            ListShowCaseLangResourceKind.ApiPropertyFilterHighlightStrategy  => en_US.ApiPropertyFilterHighlightStrategy,
            ListShowCaseLangResourceKind.TokenNameContentPadding             => en_US.TokenNameContentPadding,
            ListShowCaseLangResourceKind.TokenNameItemColor                  => en_US.TokenNameItemColor,
            ListShowCaseLangResourceKind.TokenNameItemHoverColor             => en_US.TokenNameItemHoverColor,
            ListShowCaseLangResourceKind.TokenNameItemSelectedColor          => en_US.TokenNameItemSelectedColor,
            ListShowCaseLangResourceKind.TokenNameItemDisabledColor          => en_US.TokenNameItemDisabledColor,
            ListShowCaseLangResourceKind.TokenNameItemBgColor                => en_US.TokenNameItemBgColor,
            ListShowCaseLangResourceKind.TokenNameItemHoverBgColor           => en_US.TokenNameItemHoverBgColor,
            ListShowCaseLangResourceKind.TokenNameItemSelectedBgColor        => en_US.TokenNameItemSelectedBgColor,
            ListShowCaseLangResourceKind.TokenNameItemPadding                => en_US.TokenNameItemPadding,
            ListShowCaseLangResourceKind.TokenNameItemPaddingSM              => en_US.TokenNameItemPaddingSM,
            ListShowCaseLangResourceKind.TokenNameItemPaddingLG              => en_US.TokenNameItemPaddingLG,
            ListShowCaseLangResourceKind.TokenNameItemMargin                 => en_US.TokenNameItemMargin,
            ListShowCaseLangResourceKind.TokenNamePaginationMargin           => en_US.TokenNamePaginationMargin,
            ListShowCaseLangResourceKind.TokenNameGroupHeaderColor           => en_US.TokenNameGroupHeaderColor,
            ListShowCaseLangResourceKind.TokenNameSelectedIndicatorMargin    => en_US.TokenNameSelectedIndicatorMargin,
            ListShowCaseLangResourceKind.TokenNameFilterHighlightColor       => en_US.TokenNameFilterHighlightColor,
            ListShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            ListShowCaseLangResourceKind.SelectedItemsBindingTitle           => en_US.SelectedItemsBindingTitle,
            ListShowCaseLangResourceKind.SelectedItemsBindingDescription     => en_US.SelectedItemsBindingDescription,
            ListShowCaseLangResourceKind.P2TextSelectedItems                 => en_US.P2TextSelectedItems,
            ListShowCaseLangResourceKind.P2TextNoSelection                   => en_US.P2TextNoSelection,
            ListShowCaseLangResourceKind.P2ContentSelectColors               => en_US.P2ContentSelectColors,
            ListShowCaseLangResourceKind.P2ContentClearSelection             => en_US.P2ContentClearSelection,
            _                                                                => kind.ToString()
        };
    }
}

public sealed record ListApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ListDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
