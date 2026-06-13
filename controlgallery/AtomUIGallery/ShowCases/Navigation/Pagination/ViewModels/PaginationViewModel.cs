using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Pagination;

public class PaginationViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Pagination";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<PaginationApiRow>? _apiRows;
    private ObservableCollection<PaginationDesignTokenRow>? _designTokenRows;

    public ObservableCollection<PaginationApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<PaginationDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public PaginationViewModel(IScreen screen)
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
            new PaginationApiRow("Total", Lang(PaginationShowCaseLangResourceKind.ApiPropertyTotal), "int", "cyan", "0"),
            new PaginationApiRow("CurrentPage", Lang(PaginationShowCaseLangResourceKind.ApiPropertyCurrentPage), "int", "cyan", "1"),
            new PaginationApiRow("PageSize", Lang(PaginationShowCaseLangResourceKind.ApiPropertyPageSize), "int", "cyan", "10"),
            new PaginationApiRow("PageCount", Lang(PaginationShowCaseLangResourceKind.ApiPropertyPageCount), "int", "cyan", "computed"),
            new PaginationApiRow("IsHideOnSinglePage", Lang(PaginationShowCaseLangResourceKind.ApiPropertyIsHideOnSinglePage), "bool", "purple", "false"),
            new PaginationApiRow("Align", Lang(PaginationShowCaseLangResourceKind.ApiPropertyAlign), "PaginationAlign", "blue", "Start"),
            new PaginationApiRow("SizeType", Lang(PaginationShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new PaginationApiRow("IsMotionEnabled", Lang(PaginationShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token"),
            new PaginationApiRow("CurrentPageChanged", Lang(PaginationShowCaseLangResourceKind.ApiPropertyCurrentPageChanged), "event", "default", "null"),
            new PaginationApiRow("Pagination.IsShowSizeChanger", Lang(PaginationShowCaseLangResourceKind.ApiPropertyIsShowSizeChanger), "bool", "purple", "false"),
            new PaginationApiRow("Pagination.IsShowQuickJumper", Lang(PaginationShowCaseLangResourceKind.ApiPropertyIsShowQuickJumper), "bool", "purple", "false"),
            new PaginationApiRow("Pagination.IsShowTotalInfo", Lang(PaginationShowCaseLangResourceKind.ApiPropertyIsShowTotalInfo), "bool", "purple", "false"),
            new PaginationApiRow("Pagination.TotalInfoTemplate", Lang(PaginationShowCaseLangResourceKind.ApiPropertyTotalInfoTemplate), "string?", "cyan", "null"),
            new PaginationApiRow("SimplePagination.IsReadOnly", Lang(PaginationShowCaseLangResourceKind.ApiPropertyIsReadOnly), "bool", "purple", "true")
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
            new PaginationDesignTokenRow("ItemBg", Lang(PaginationShowCaseLangResourceKind.TokenNameItemBg), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("ItemSize", Lang(PaginationShowCaseLangResourceKind.TokenNameItemSize), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("ItemActiveBg", Lang(PaginationShowCaseLangResourceKind.TokenNameItemActiveBg), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("ItemSizeSM", Lang(PaginationShowCaseLangResourceKind.TokenNameItemSizeSM), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("ItemLinkBg", Lang(PaginationShowCaseLangResourceKind.TokenNameItemLinkBg), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("ItemActiveBgDisabled", Lang(PaginationShowCaseLangResourceKind.TokenNameItemActiveBgDisabled), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("ItemActiveColorDisabled", Lang(PaginationShowCaseLangResourceKind.TokenNameItemActiveColorDisabled), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("ItemInputBg", Lang(PaginationShowCaseLangResourceKind.TokenNameItemInputBg), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("InputOutlineOffset", Lang(PaginationShowCaseLangResourceKind.TokenNameInputOutlineOffset), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("PaginationLayoutSpacing", Lang(PaginationShowCaseLangResourceKind.TokenNamePaginationLayoutSpacing), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("PaginationLayoutMiniSpacing", Lang(PaginationShowCaseLangResourceKind.TokenNamePaginationLayoutMiniSpacing), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("PaginationQuickJumperInputWidth", Lang(PaginationShowCaseLangResourceKind.TokenNamePaginationQuickJumperInputWidth), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("PaginationMiniQuickJumperInputWidth", Lang(PaginationShowCaseLangResourceKind.TokenNamePaginationMiniQuickJumperInputWidth), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success"),
            new PaginationDesignTokenRow("PaginationItemPaddingInline", Lang(PaginationShowCaseLangResourceKind.TokenNamePaginationItemPaddingInline), Lang(PaginationShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(PaginationShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(PaginationShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(PaginationShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            PaginationShowCaseLangResourceKind.ApiPropertyTotal                           => en_US.ApiPropertyTotal,
            PaginationShowCaseLangResourceKind.ApiPropertyCurrentPage                     => en_US.ApiPropertyCurrentPage,
            PaginationShowCaseLangResourceKind.ApiPropertyPageSize                        => en_US.ApiPropertyPageSize,
            PaginationShowCaseLangResourceKind.ApiPropertyPageCount                       => en_US.ApiPropertyPageCount,
            PaginationShowCaseLangResourceKind.ApiPropertyIsHideOnSinglePage              => en_US.ApiPropertyIsHideOnSinglePage,
            PaginationShowCaseLangResourceKind.ApiPropertyAlign                           => en_US.ApiPropertyAlign,
            PaginationShowCaseLangResourceKind.ApiPropertySizeType                        => en_US.ApiPropertySizeType,
            PaginationShowCaseLangResourceKind.ApiPropertyIsMotionEnabled                 => en_US.ApiPropertyIsMotionEnabled,
            PaginationShowCaseLangResourceKind.ApiPropertyCurrentPageChanged              => en_US.ApiPropertyCurrentPageChanged,
            PaginationShowCaseLangResourceKind.ApiPropertyIsShowSizeChanger               => en_US.ApiPropertyIsShowSizeChanger,
            PaginationShowCaseLangResourceKind.ApiPropertyIsShowQuickJumper               => en_US.ApiPropertyIsShowQuickJumper,
            PaginationShowCaseLangResourceKind.ApiPropertyIsShowTotalInfo                 => en_US.ApiPropertyIsShowTotalInfo,
            PaginationShowCaseLangResourceKind.ApiPropertyTotalInfoTemplate               => en_US.ApiPropertyTotalInfoTemplate,
            PaginationShowCaseLangResourceKind.ApiPropertyIsReadOnly                      => en_US.ApiPropertyIsReadOnly,
            PaginationShowCaseLangResourceKind.TokenNameItemBg                            => en_US.TokenNameItemBg,
            PaginationShowCaseLangResourceKind.TokenNameItemSize                          => en_US.TokenNameItemSize,
            PaginationShowCaseLangResourceKind.TokenNameItemActiveBg                      => en_US.TokenNameItemActiveBg,
            PaginationShowCaseLangResourceKind.TokenNameItemSizeSM                        => en_US.TokenNameItemSizeSM,
            PaginationShowCaseLangResourceKind.TokenNameItemLinkBg                        => en_US.TokenNameItemLinkBg,
            PaginationShowCaseLangResourceKind.TokenNameItemActiveBgDisabled              => en_US.TokenNameItemActiveBgDisabled,
            PaginationShowCaseLangResourceKind.TokenNameItemActiveColorDisabled           => en_US.TokenNameItemActiveColorDisabled,
            PaginationShowCaseLangResourceKind.TokenNameItemInputBg                       => en_US.TokenNameItemInputBg,
            PaginationShowCaseLangResourceKind.TokenNameInputOutlineOffset                => en_US.TokenNameInputOutlineOffset,
            PaginationShowCaseLangResourceKind.TokenNamePaginationLayoutSpacing           => en_US.TokenNamePaginationLayoutSpacing,
            PaginationShowCaseLangResourceKind.TokenNamePaginationLayoutMiniSpacing       => en_US.TokenNamePaginationLayoutMiniSpacing,
            PaginationShowCaseLangResourceKind.TokenNamePaginationQuickJumperInputWidth   => en_US.TokenNamePaginationQuickJumperInputWidth,
            PaginationShowCaseLangResourceKind.TokenNamePaginationMiniQuickJumperInputWidth => en_US.TokenNamePaginationMiniQuickJumperInputWidth,
            PaginationShowCaseLangResourceKind.TokenNamePaginationItemPaddingInline       => en_US.TokenNamePaginationItemPaddingInline,
            PaginationShowCaseLangResourceKind.TokenScopeComponent                        => en_US.TokenScopeComponent,
            PaginationShowCaseLangResourceKind.TokenStatusStable                          => en_US.TokenStatusStable,
            _                                                                            => kind.ToString()
        };
    }
}

public sealed record PaginationApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record PaginationDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
