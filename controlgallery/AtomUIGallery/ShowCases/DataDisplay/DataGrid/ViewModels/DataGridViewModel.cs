using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using DynamicData;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.DataGrid;

public partial class DataGridViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "DataGrid";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public ObservableCollection<DataGridBaseInfo>? BasicCaseDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? FilterAndSorterDataSource { get; set; }
    public ObservableCollection<MultiSorterDataType>? MultiSorterDataSource { get; set; }
    public ObservableCollection<ExpandableRowDataType>? ExpandableRowDataSource { get; set; }
    public ObservableCollection<GroupHeaderDataType>? GroupHeaderDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? FixedHeaderDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? FixedColumnsDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? FixedColumnsAndHeadersDataSource { get; set; }
    public ObservableCollection<DragColumnDataType>? DragColumnDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? DragRowDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? DragRowManyDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? CustomEmptyDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? EditableCellsDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? EditableRowsDataSource { get; set; }
    public ObservableCollection<DataGridBaseInfo>? PagingGridDataSource { get; set; }

    private ObservableCollection<DataGridApiRow>? _apiRows;
    private ObservableCollection<DataGridDesignTokenRow>? _designTokenRows;

    public ObservableCollection<DataGridApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<DataGridDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public DataGridViewModel(IScreen screen)
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
            new DataGridApiRow("ItemsSource", Lang(DataGridShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "blue", "null"),
            new DataGridApiRow("AutoGenerateColumns", Lang(DataGridShowCaseLangResourceKind.ApiPropertyAutoGenerateColumns), "bool", "purple", "false"),
            new DataGridApiRow("Columns", Lang(DataGridShowCaseLangResourceKind.ApiPropertyColumns), "DataGridColumnCollection", "cyan", "[]"),
            new DataGridApiRow("ColumnGroups", Lang(DataGridShowCaseLangResourceKind.ApiPropertyColumnGroups), "DataGridColumnGroupCollection", "cyan", "[]"),
            new DataGridApiRow("SelectionMode", Lang(DataGridShowCaseLangResourceKind.ApiPropertySelectionMode), "DataGridSelectionMode", "blue", "Single"),
            new DataGridApiRow("SelectTriggerType", Lang(DataGridShowCaseLangResourceKind.ApiPropertySelectTriggerType), "DataGridSelectTriggerType", "blue", "Row"),
            new DataGridApiRow("CanUserSortColumns", Lang(DataGridShowCaseLangResourceKind.ApiPropertyCanUserSortColumns), "bool", "purple", "false"),
            new DataGridApiRow("CanUserFilterColumns", Lang(DataGridShowCaseLangResourceKind.ApiPropertyCanUserFilterColumns), "bool", "purple", "false"),
            new DataGridApiRow("CanUserResizeColumns", Lang(DataGridShowCaseLangResourceKind.ApiPropertyCanUserResizeColumns), "bool", "purple", "false"),
            new DataGridApiRow("CanUserReorderColumns", Lang(DataGridShowCaseLangResourceKind.ApiPropertyCanUserReorderColumns), "bool", "purple", "false"),
            new DataGridApiRow("CanUserReorderRows", Lang(DataGridShowCaseLangResourceKind.ApiPropertyCanUserReorderRows), "bool", "purple", "false"),
            new DataGridApiRow("LeftFrozenColumnCount", Lang(DataGridShowCaseLangResourceKind.ApiPropertyLeftFrozenColumnCount), "int", "cyan", "0"),
            new DataGridApiRow("RightFrozenColumnCount", Lang(DataGridShowCaseLangResourceKind.ApiPropertyRightFrozenColumnCount), "int", "cyan", "0"),
            new DataGridApiRow("RowDetailsTemplate", Lang(DataGridShowCaseLangResourceKind.ApiPropertyRowDetailsTemplate), "IDataTemplate?", "blue", "null"),
            new DataGridApiRow("PaginationVisibility", Lang(DataGridShowCaseLangResourceKind.ApiPropertyPaginationVisibility), "DataGridPaginationVisibility", "blue", "Bottom"),
            new DataGridApiRow("PageSize", Lang(DataGridShowCaseLangResourceKind.ApiPropertyPageSize), "int", "cyan", "10"),
            new DataGridApiRow("IsOperating", Lang(DataGridShowCaseLangResourceKind.ApiPropertyIsOperating), "bool", "purple", "false"),
            new DataGridApiRow("GridLinesVisibility", Lang(DataGridShowCaseLangResourceKind.ApiPropertyGridLinesVisibility), "DataGridGridLinesVisibility", "blue", "None"),
            new DataGridApiRow("IsReadOnly", Lang(DataGridShowCaseLangResourceKind.ApiPropertyIsReadOnly), "bool", "purple", "false")
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
            new DataGridDesignTokenRow("HeaderBg", Lang(DataGridShowCaseLangResourceKind.TokenNameHeaderBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("HeaderColor", Lang(DataGridShowCaseLangResourceKind.TokenNameHeaderColor), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("HeaderSortActiveBg", Lang(DataGridShowCaseLangResourceKind.TokenNameHeaderSortActiveBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("BodySortBg", Lang(DataGridShowCaseLangResourceKind.TokenNameBodySortBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("RowHoverBg", Lang(DataGridShowCaseLangResourceKind.TokenNameRowHoverBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("RowSelectedBg", Lang(DataGridShowCaseLangResourceKind.TokenNameRowSelectedBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("RowExpandedBg", Lang(DataGridShowCaseLangResourceKind.TokenNameRowExpandedBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("CellPadding", Lang(DataGridShowCaseLangResourceKind.TokenNameCellPadding), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("CellPaddingMD", Lang(DataGridShowCaseLangResourceKind.TokenNameCellPaddingMD), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("CellPaddingSM", Lang(DataGridShowCaseLangResourceKind.TokenNameCellPaddingSM), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("BorderColor", Lang(DataGridShowCaseLangResourceKind.TokenNameBorderColor), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("HeaderBorderRadius", Lang(DataGridShowCaseLangResourceKind.TokenNameHeaderBorderRadius), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("FooterBg", Lang(DataGridShowCaseLangResourceKind.TokenNameFooterBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("CellFontSize", Lang(DataGridShowCaseLangResourceKind.TokenNameCellFontSize), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("HeaderSplitColor", Lang(DataGridShowCaseLangResourceKind.TokenNameHeaderSplitColor), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("FilterDropdownBg", Lang(DataGridShowCaseLangResourceKind.TokenNameFilterDropdownBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("SelectionColumnWidth", Lang(DataGridShowCaseLangResourceKind.TokenNameSelectionColumnWidth), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("LeftFrozenShadows", Lang(DataGridShowCaseLangResourceKind.TokenNameLeftFrozenShadows), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("RightFrozenShadows", Lang(DataGridShowCaseLangResourceKind.TokenNameRightFrozenShadows), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("PaginationMargin", Lang(DataGridShowCaseLangResourceKind.TokenNamePaginationMargin), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("ColumnReorderActiveBg", Lang(DataGridShowCaseLangResourceKind.TokenNameColumnReorderActiveBg), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DataGridDesignTokenRow("RowReorderIndicatorSize", Lang(DataGridShowCaseLangResourceKind.TokenNameRowReorderIndicatorSize), Lang(DataGridShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DataGridShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(DataGridShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(DataGridShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            DataGridShowCaseLangResourceKind.ApiPropertyItemsSource             => en_US.ApiPropertyItemsSource,
            DataGridShowCaseLangResourceKind.ApiPropertyAutoGenerateColumns     => en_US.ApiPropertyAutoGenerateColumns,
            DataGridShowCaseLangResourceKind.ApiPropertyColumns                 => en_US.ApiPropertyColumns,
            DataGridShowCaseLangResourceKind.ApiPropertyColumnGroups            => en_US.ApiPropertyColumnGroups,
            DataGridShowCaseLangResourceKind.ApiPropertySelectionMode           => en_US.ApiPropertySelectionMode,
            DataGridShowCaseLangResourceKind.ApiPropertySelectTriggerType       => en_US.ApiPropertySelectTriggerType,
            DataGridShowCaseLangResourceKind.ApiPropertyCanUserSortColumns      => en_US.ApiPropertyCanUserSortColumns,
            DataGridShowCaseLangResourceKind.ApiPropertyCanUserFilterColumns    => en_US.ApiPropertyCanUserFilterColumns,
            DataGridShowCaseLangResourceKind.ApiPropertyCanUserResizeColumns    => en_US.ApiPropertyCanUserResizeColumns,
            DataGridShowCaseLangResourceKind.ApiPropertyCanUserReorderColumns   => en_US.ApiPropertyCanUserReorderColumns,
            DataGridShowCaseLangResourceKind.ApiPropertyCanUserReorderRows      => en_US.ApiPropertyCanUserReorderRows,
            DataGridShowCaseLangResourceKind.ApiPropertyLeftFrozenColumnCount   => en_US.ApiPropertyLeftFrozenColumnCount,
            DataGridShowCaseLangResourceKind.ApiPropertyRightFrozenColumnCount  => en_US.ApiPropertyRightFrozenColumnCount,
            DataGridShowCaseLangResourceKind.ApiPropertyRowDetailsTemplate      => en_US.ApiPropertyRowDetailsTemplate,
            DataGridShowCaseLangResourceKind.ApiPropertyPaginationVisibility    => en_US.ApiPropertyPaginationVisibility,
            DataGridShowCaseLangResourceKind.ApiPropertyPageSize                => en_US.ApiPropertyPageSize,
            DataGridShowCaseLangResourceKind.ApiPropertyIsOperating             => en_US.ApiPropertyIsOperating,
            DataGridShowCaseLangResourceKind.ApiPropertyGridLinesVisibility     => en_US.ApiPropertyGridLinesVisibility,
            DataGridShowCaseLangResourceKind.ApiPropertyIsReadOnly              => en_US.ApiPropertyIsReadOnly,
            DataGridShowCaseLangResourceKind.TokenNameHeaderBg                  => en_US.TokenNameHeaderBg,
            DataGridShowCaseLangResourceKind.TokenNameHeaderColor               => en_US.TokenNameHeaderColor,
            DataGridShowCaseLangResourceKind.TokenNameHeaderSortActiveBg        => en_US.TokenNameHeaderSortActiveBg,
            DataGridShowCaseLangResourceKind.TokenNameBodySortBg                => en_US.TokenNameBodySortBg,
            DataGridShowCaseLangResourceKind.TokenNameRowHoverBg                => en_US.TokenNameRowHoverBg,
            DataGridShowCaseLangResourceKind.TokenNameRowSelectedBg             => en_US.TokenNameRowSelectedBg,
            DataGridShowCaseLangResourceKind.TokenNameRowExpandedBg             => en_US.TokenNameRowExpandedBg,
            DataGridShowCaseLangResourceKind.TokenNameCellPadding               => en_US.TokenNameCellPadding,
            DataGridShowCaseLangResourceKind.TokenNameCellPaddingMD             => en_US.TokenNameCellPaddingMD,
            DataGridShowCaseLangResourceKind.TokenNameCellPaddingSM             => en_US.TokenNameCellPaddingSM,
            DataGridShowCaseLangResourceKind.TokenNameBorderColor               => en_US.TokenNameBorderColor,
            DataGridShowCaseLangResourceKind.TokenNameHeaderBorderRadius        => en_US.TokenNameHeaderBorderRadius,
            DataGridShowCaseLangResourceKind.TokenNameFooterBg                  => en_US.TokenNameFooterBg,
            DataGridShowCaseLangResourceKind.TokenNameCellFontSize              => en_US.TokenNameCellFontSize,
            DataGridShowCaseLangResourceKind.TokenNameHeaderSplitColor          => en_US.TokenNameHeaderSplitColor,
            DataGridShowCaseLangResourceKind.TokenNameFilterDropdownBg          => en_US.TokenNameFilterDropdownBg,
            DataGridShowCaseLangResourceKind.TokenNameSelectionColumnWidth      => en_US.TokenNameSelectionColumnWidth,
            DataGridShowCaseLangResourceKind.TokenNameLeftFrozenShadows         => en_US.TokenNameLeftFrozenShadows,
            DataGridShowCaseLangResourceKind.TokenNameRightFrozenShadows        => en_US.TokenNameRightFrozenShadows,
            DataGridShowCaseLangResourceKind.TokenNamePaginationMargin          => en_US.TokenNamePaginationMargin,
            DataGridShowCaseLangResourceKind.TokenNameColumnReorderActiveBg     => en_US.TokenNameColumnReorderActiveBg,
            DataGridShowCaseLangResourceKind.TokenNameRowReorderIndicatorSize   => en_US.TokenNameRowReorderIndicatorSize,
            DataGridShowCaseLangResourceKind.TokenScopeComponent                => en_US.TokenScopeComponent,
            DataGridShowCaseLangResourceKind.TokenStatusStable                  => en_US.TokenStatusStable,
            _                                                                   => kind.ToString()
        };
    }
}

public sealed record DataGridApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record DataGridDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);

public class DataGridBaseInfo
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Money { get; set; } = string.Empty;
    public List<TagInfo> Tags { get; set; } = new();
}

public class TagInfo
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class MultiSorterDataType
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Chinese { get; set; }
    public int Math { get; set; }
    public int English { get; set; }
}

public class ExpandableRowDataType : DataGridBaseInfo
{
    public string Description { get; set; } = string.Empty;
}

public class GroupHeaderDataType
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Street { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Number { get; set; }
}

public class DragColumnDataType
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
