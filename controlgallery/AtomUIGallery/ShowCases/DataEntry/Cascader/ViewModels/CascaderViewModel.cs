using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.DataLoad;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Cascader;

public class CascaderViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Cascader";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<CascaderApiRow>? _apiRows;
    private ObservableCollection<CascaderDesignTokenRow>? _designTokenRows;

    public ObservableCollection<CascaderApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<CascaderDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private List<ICascaderOption>? _basicCascaderViewNodes = [];

    public List<ICascaderOption>? BasicCascaderViewNodes
    {
        get => _basicCascaderViewNodes;
        set => this.RaiseAndSetIfChanged(ref _basicCascaderViewNodes, value);
    }

    private TreeNodePath? _defaultSelectOptionPath;

    public TreeNodePath? DefaultSelectOptionPath
    {
        get => _defaultSelectOptionPath;
        set => this.RaiseAndSetIfChanged(ref _defaultSelectOptionPath, value);
    }

    private List<ICascaderOption>? _basicCheckableCascaderViewNodes = [];

    public List<ICascaderOption>? BasicCheckableCascaderViewNodes
    {
        get => _basicCheckableCascaderViewNodes;
        set => this.RaiseAndSetIfChanged(ref _basicCheckableCascaderViewNodes, value);
    }

    private List<ICascaderOption>? _hoverCascaderNodes = [];

    public List<ICascaderOption>? HoverCascaderNodes
    {
        get => _hoverCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _hoverCascaderNodes, value);
    }

    private List<ICascaderOption>? _disabledCascaderNodes = [];

    public List<ICascaderOption>? DisabledCascaderNodes
    {
        get => _disabledCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _disabledCascaderNodes, value);
    }

    private List<ICascaderOption>? _selectParentCascaderNodes = [];

    public List<ICascaderOption>? SelectParentCascaderNodes
    {
        get => _selectParentCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _selectParentCascaderNodes, value);
    }

    private List<ICascaderOption>? _multipleSelectCascaderNodes = [];

    public List<ICascaderOption>? MultipleSelectCascaderNodes
    {
        get => _multipleSelectCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _multipleSelectCascaderNodes, value);
    }

    private List<ICascaderOption>? _checkStrategyShowParentCascaderNodes = [];

    public List<ICascaderOption>? CheckStrategyShowParentCascaderNodes
    {
        get => _checkStrategyShowParentCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _checkStrategyShowParentCascaderNodes, value);
    }

    private List<ICascaderOption>? _checkStrategyShowAllCascaderNodes = [];

    public List<ICascaderOption>? CheckStrategyShowAllCascaderNodes
    {
        get => _checkStrategyShowAllCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _checkStrategyShowAllCascaderNodes, value);
    }

    private List<ICascaderOption>? _prefixAndSuffixCascaderNodes = [];

    public List<ICascaderOption>? PrefixAndSuffixCascaderNodes
    {
        get => _prefixAndSuffixCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _prefixAndSuffixCascaderNodes, value);
    }

    private SelectPopupPlacement _placement;

    public SelectPopupPlacement Placement
    {
        get => _placement;
        set => this.RaiseAndSetIfChanged(ref _placement, value);
    }

    private List<ICascaderOption>? _placementCascaderNodes = [];

    public List<ICascaderOption>? PlacementCascaderNodes
    {
        get => _placementCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _placementCascaderNodes, value);
    }

    private List<ICascaderOption>? _searchCascaderNodes = [];

    public List<ICascaderOption>? SearchCascaderNodes
    {
        get => _searchCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _searchCascaderNodes, value);
    }

    private List<ICascaderOption>? _sizeCascaderNodes = [];

    public List<ICascaderOption>? SizeCascaderNodes
    {
        get => _sizeCascaderNodes;
        set => this.RaiseAndSetIfChanged(ref _sizeCascaderNodes, value);
    }

    private List<ICascaderOption>? _asyncLoadCascaderViewNodes = [];

    public List<ICascaderOption>? AsyncLoadCascaderViewNodes
    {
        get => _asyncLoadCascaderViewNodes;
        set => this.RaiseAndSetIfChanged(ref _asyncLoadCascaderViewNodes, value);
    }

    private List<ICascaderOption>? _searchCascaderViewNodes = [];

    public List<ICascaderOption>? SearchCascaderViewNodes
    {
        get => _searchCascaderViewNodes;
        set => this.RaiseAndSetIfChanged(ref _searchCascaderViewNodes, value);
    }

    private CascaderItemDataLoader? _asyncCascaderNodeLoader;

    public CascaderItemDataLoader? AsyncCascaderNodeLoader
    {
        get => _asyncCascaderNodeLoader;
        set => this.RaiseAndSetIfChanged(ref _asyncCascaderNodeLoader, value);
    }

    private List<ICascaderOption>? _defaultExpandCascaderViewNodes = [];

    public List<ICascaderOption>? DefaultExpandCascaderViewNodes
    {
        get => _defaultExpandCascaderViewNodes;
        set => this.RaiseAndSetIfChanged(ref _defaultExpandCascaderViewNodes, value);
    }

    private TreeNodePath? _defaultExpandPath;

    public TreeNodePath? DefaultExpandPath
    {
        get => _defaultExpandPath;
        set => this.RaiseAndSetIfChanged(ref _defaultExpandPath, value);
    }

    public CascaderViewModel(IScreen screen)
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
            new CascaderApiRow("OptionsSource", Lang(CascaderShowCaseLangResourceKind.ApiPropertyOptionsSource), "IEnumerable<ICascaderOption>?", "cyan", "null"),
            new CascaderApiRow("OptionTemplate", Lang(CascaderShowCaseLangResourceKind.ApiPropertyOptionTemplate), "IDataTemplate?", "cyan", "null"),
            new CascaderApiRow("SelectedOption", Lang(CascaderShowCaseLangResourceKind.ApiPropertySelectedOption), "ICascaderOption?", "cyan", "null"),
            new CascaderApiRow("SelectedOptions", Lang(CascaderShowCaseLangResourceKind.ApiPropertySelectedOptions), "IList<ICascaderOption>?", "cyan", "null"),
            new CascaderApiRow("DefaultSelectOptionPath", Lang(CascaderShowCaseLangResourceKind.ApiPropertyDefaultSelectOptionPath), "TreeNodePath?", "cyan", "null"),
            new CascaderApiRow("IsAllowClear", Lang(CascaderShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "green", "false"),
            new CascaderApiRow("IsMultiple", Lang(CascaderShowCaseLangResourceKind.ApiPropertyIsMultiple), "bool", "green", "false"),
            new CascaderApiRow("ShowCheckedStrategy", Lang(CascaderShowCaseLangResourceKind.ApiPropertyShowCheckedStrategy), "TreeSelectCheckedStrategy", "purple", "All"),
            new CascaderApiRow("ExpandTrigger", Lang(CascaderShowCaseLangResourceKind.ApiPropertyExpandTrigger), "CascaderViewExpandTrigger", "purple", "Click"),
            new CascaderApiRow("IsAllowSelectParent", Lang(CascaderShowCaseLangResourceKind.ApiPropertyIsAllowSelectParent), "bool", "green", "false"),
            new CascaderApiRow("DataLoader", Lang(CascaderShowCaseLangResourceKind.ApiPropertyDataLoader), "ICascaderItemDataLoader?", "cyan", "null"),
            new CascaderApiRow("Filter", Lang(CascaderShowCaseLangResourceKind.ApiPropertyFilter), "IValueFilter?", "cyan", "Contains"),
            new CascaderApiRow("StyleVariant", Lang(CascaderShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "purple", "Outlined"),
            new CascaderApiRow("Status", Lang(CascaderShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "purple", "Default"),
            new CascaderApiRow("Placement", Lang(CascaderShowCaseLangResourceKind.ApiPropertyPlacement), "SelectPopupPlacement", "purple", "BottomEdgeAlignedLeft"),
            new CascaderApiRow("CascaderView.OptionsSource", Lang(CascaderShowCaseLangResourceKind.ApiPropertyCascaderViewOptionsSource), "IEnumerable<ICascaderOption>?", "cyan", "null"),
            new CascaderApiRow("CascaderView.IsCheckable", Lang(CascaderShowCaseLangResourceKind.ApiPropertyCascaderViewIsCheckable), "bool", "green", "false"),
            new CascaderApiRow("CascaderView.DefaultExpandedPath", Lang(CascaderShowCaseLangResourceKind.ApiPropertyCascaderViewDefaultExpandedPath), "TreeNodePath?", "cyan", "null"),
            new CascaderApiRow("CascaderView.FilterValue", Lang(CascaderShowCaseLangResourceKind.ApiPropertyCascaderViewFilterValue), "object?", "cyan", "null")
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
            new CascaderDesignTokenRow("HeaderHeight", Lang(CascaderShowCaseLangResourceKind.TokenNameHeaderHeight), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("ControlWidth", Lang(CascaderShowCaseLangResourceKind.TokenNameControlWidth), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("ControlItemWidth", Lang(CascaderShowCaseLangResourceKind.TokenNameControlItemWidth), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("DropdownHeight", Lang(CascaderShowCaseLangResourceKind.TokenNameDropdownHeight), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("OptionSelectedBg", Lang(CascaderShowCaseLangResourceKind.TokenNameOptionSelectedBg), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("OptionHoverBg", Lang(CascaderShowCaseLangResourceKind.TokenNameOptionHoverBg), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("OptionSelectedColor", Lang(CascaderShowCaseLangResourceKind.TokenNameOptionSelectedColor), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("OptionPadding", Lang(CascaderShowCaseLangResourceKind.TokenNameOptionPadding), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("MenuPadding", Lang(CascaderShowCaseLangResourceKind.TokenNameMenuPadding), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("FilterHighlightColor", Lang(CascaderShowCaseLangResourceKind.TokenNameFilterHighlightColor), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CascaderDesignTokenRow("ItemHeaderSpacing", Lang(CascaderShowCaseLangResourceKind.TokenNameItemHeaderSpacing), Lang(CascaderShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CascaderShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(CascaderShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(CascaderShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            CascaderShowCaseLangResourceKind.ApiPropertyOptionsSource                    => en_US.ApiPropertyOptionsSource,
            CascaderShowCaseLangResourceKind.ApiPropertyOptionTemplate                   => en_US.ApiPropertyOptionTemplate,
            CascaderShowCaseLangResourceKind.ApiPropertySelectedOption                   => en_US.ApiPropertySelectedOption,
            CascaderShowCaseLangResourceKind.ApiPropertySelectedOptions                  => en_US.ApiPropertySelectedOptions,
            CascaderShowCaseLangResourceKind.ApiPropertyDefaultSelectOptionPath          => en_US.ApiPropertyDefaultSelectOptionPath,
            CascaderShowCaseLangResourceKind.ApiPropertyIsAllowClear                     => en_US.ApiPropertyIsAllowClear,
            CascaderShowCaseLangResourceKind.ApiPropertyIsMultiple                       => en_US.ApiPropertyIsMultiple,
            CascaderShowCaseLangResourceKind.ApiPropertyShowCheckedStrategy              => en_US.ApiPropertyShowCheckedStrategy,
            CascaderShowCaseLangResourceKind.ApiPropertyExpandTrigger                    => en_US.ApiPropertyExpandTrigger,
            CascaderShowCaseLangResourceKind.ApiPropertyIsAllowSelectParent              => en_US.ApiPropertyIsAllowSelectParent,
            CascaderShowCaseLangResourceKind.ApiPropertyDataLoader                       => en_US.ApiPropertyDataLoader,
            CascaderShowCaseLangResourceKind.ApiPropertyFilter                           => en_US.ApiPropertyFilter,
            CascaderShowCaseLangResourceKind.ApiPropertyStyleVariant                     => en_US.ApiPropertyStyleVariant,
            CascaderShowCaseLangResourceKind.ApiPropertyStatus                           => en_US.ApiPropertyStatus,
            CascaderShowCaseLangResourceKind.ApiPropertyPlacement                        => en_US.ApiPropertyPlacement,
            CascaderShowCaseLangResourceKind.ApiPropertyCascaderViewOptionsSource        => en_US.ApiPropertyCascaderViewOptionsSource,
            CascaderShowCaseLangResourceKind.ApiPropertyCascaderViewIsCheckable          => en_US.ApiPropertyCascaderViewIsCheckable,
            CascaderShowCaseLangResourceKind.ApiPropertyCascaderViewDefaultExpandedPath  => en_US.ApiPropertyCascaderViewDefaultExpandedPath,
            CascaderShowCaseLangResourceKind.ApiPropertyCascaderViewFilterValue          => en_US.ApiPropertyCascaderViewFilterValue,
            CascaderShowCaseLangResourceKind.TokenNameHeaderHeight                       => en_US.TokenNameHeaderHeight,
            CascaderShowCaseLangResourceKind.TokenNameControlWidth                       => en_US.TokenNameControlWidth,
            CascaderShowCaseLangResourceKind.TokenNameControlItemWidth                   => en_US.TokenNameControlItemWidth,
            CascaderShowCaseLangResourceKind.TokenNameDropdownHeight                     => en_US.TokenNameDropdownHeight,
            CascaderShowCaseLangResourceKind.TokenNameOptionSelectedBg                   => en_US.TokenNameOptionSelectedBg,
            CascaderShowCaseLangResourceKind.TokenNameOptionHoverBg                      => en_US.TokenNameOptionHoverBg,
            CascaderShowCaseLangResourceKind.TokenNameOptionSelectedColor                => en_US.TokenNameOptionSelectedColor,
            CascaderShowCaseLangResourceKind.TokenNameOptionPadding                      => en_US.TokenNameOptionPadding,
            CascaderShowCaseLangResourceKind.TokenNameMenuPadding                        => en_US.TokenNameMenuPadding,
            CascaderShowCaseLangResourceKind.TokenNameFilterHighlightColor               => en_US.TokenNameFilterHighlightColor,
            CascaderShowCaseLangResourceKind.TokenNameItemHeaderSpacing                  => en_US.TokenNameItemHeaderSpacing,
            CascaderShowCaseLangResourceKind.TokenScopeComponent                         => en_US.TokenScopeComponent,
            CascaderShowCaseLangResourceKind.TokenStatusStable                           => en_US.TokenStatusStable,
            _                                                                            => kind.ToString()
        };
    }
}

public sealed record CascaderApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record CascaderDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);

public class CascaderItemDataLoader : ICascaderItemDataLoader
{
    public async Task<CascaderItemLoadResult> LoadAsync(ICascaderOption targetCascaderItem, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(600), token);
        var children = new List<CascaderOption>();
        children.AddRange(
        [
            new CascaderOption()
            {
                Header = CascaderShowCaseLanguage.FormatDynamicOption(targetCascaderItem, 1),
                IsLeaf = true
            },
            new CascaderOption()
            {
                Header = CascaderShowCaseLanguage.FormatDynamicOption(targetCascaderItem, 2),
                IsLeaf = true
            }
        ]);
        return new CascaderItemLoadResult()
        {
            IsSuccess = true,
            Data      = children
        };
    }
}
