using System.Collections.Generic;
using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Controls.Data;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Transfer;

public class TransferViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Transfer";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<TransferApiRow>? _apiRows;

    public ObservableCollection<TransferApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    private ObservableCollection<TransferDesignTokenRow>? _designTokenRows;

    public ObservableCollection<TransferDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private List<IListItemData>? _basicTransferItems;

    public List<IListItemData>? BasicTransferItems
    {
        get => _basicTransferItems;
        set => this.RaiseAndSetIfChanged(ref _basicTransferItems, value);
    }

    private List<IListItemData>? _oneWayTransferItems;

    public List<IListItemData>? OneWayTransferItems
    {
        get => _oneWayTransferItems;
        set => this.RaiseAndSetIfChanged(ref _oneWayTransferItems, value);
    }

    private bool _oneWayTransferEnabled = true;

    public bool OneWayTransferEnabled
    {
        get => _oneWayTransferEnabled;
        set => this.RaiseAndSetIfChanged(ref _oneWayTransferEnabled, value);
    }

    private List<IListItemData>? _searchTransferItems;

    public List<IListItemData>? SearchTransferItems
    {
        get => _searchTransferItems;
        set => this.RaiseAndSetIfChanged(ref _searchTransferItems, value);
    }

    private DefaultFilterValueSelector? _transferFilterValueSelector;

    public DefaultFilterValueSelector? TransferFilterValueSelector
    {
        get => _transferFilterValueSelector;
        set => this.RaiseAndSetIfChanged(ref _transferFilterValueSelector, value);
    }

    private List<IListItemData>? _controlledTransferItems;

    public List<IListItemData>? ControlledTransferItems
    {
        get => _controlledTransferItems;
        set => this.RaiseAndSetIfChanged(ref _controlledTransferItems, value);
    }

    private ObservableCollection<EntityKey>? _controlledTransferTargetKeys;

    public ObservableCollection<EntityKey>? ControlledTransferTargetKeys
    {
        get => _controlledTransferTargetKeys;
        set => this.RaiseAndSetIfChanged(ref _controlledTransferTargetKeys, value);
    }

    private ObservableCollection<EntityKey>? _controlledTransferSelectedKeys;

    public ObservableCollection<EntityKey>? ControlledTransferSelectedKeys
    {
        get => _controlledTransferSelectedKeys;
        set => this.RaiseAndSetIfChanged(ref _controlledTransferSelectedKeys, value);
    }

    private List<IListItemData>? _advanceTransferItems;

    public List<IListItemData>? AdvanceTransferItems
    {
        get => _advanceTransferItems;
        set => this.RaiseAndSetIfChanged(ref _advanceTransferItems, value);
    }

    private List<EntityKey>? _advanceTransferDefaultTargetKeys;

    public List<EntityKey>? AdvanceTransferDefaultTargetKeys
    {
        get => _advanceTransferDefaultTargetKeys;
        set => this.RaiseAndSetIfChanged(ref _advanceTransferDefaultTargetKeys, value);
    }

    private bool _paginationIsOneWay = false;

    public bool PaginationIsOneWay
    {
        get => _paginationIsOneWay;
        set => this.RaiseAndSetIfChanged(ref _paginationIsOneWay, value);
    }

    private List<IListItemData>? _paginationTransferItems;

    public List<IListItemData>? PaginationTransferItems
    {
        get => _paginationTransferItems;
        set => this.RaiseAndSetIfChanged(ref _paginationTransferItems, value);
    }

    private List<EntityKey>? _paginationTransferDefaultTargetKeys;

    public List<EntityKey>? PaginationTransferDefaultTargetKeys
    {
        get => _paginationTransferDefaultTargetKeys;
        set => this.RaiseAndSetIfChanged(ref _paginationTransferDefaultTargetKeys, value);
    }

    private List<DataGridTransferData>? _gridDataTransformItems;

    public List<DataGridTransferData>? GridDataTransformItems
    {
        get => _gridDataTransformItems;
        set => this.RaiseAndSetIfChanged(ref _gridDataTransformItems, value);
    }

    private List<ITreeItemNode>? _transferTreeNodes = [];

    public List<ITreeItemNode>? TransferTreeNodes
    {
        get => _transferTreeNodes;
        set => this.RaiseAndSetIfChanged(ref _transferTreeNodes, value);
    }

    private bool _treeTransferIsOneWay = false;

    public bool TreeTransferIsOneWay
    {
        get => _treeTransferIsOneWay;
        set => this.RaiseAndSetIfChanged(ref _treeTransferIsOneWay, value);
    }

    public TransferViewModel(IScreen screen)
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
            new TransferApiRow("ItemsSource", Lang(TransferShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable<IListItemData>?", "cyan", "null"),
            new TransferApiRow("TargetKeys", Lang(TransferShowCaseLangResourceKind.ApiPropertyTargetKeys), "IList<EntityKey>?", "cyan", "null"),
            new TransferApiRow("SelectedKeys", Lang(TransferShowCaseLangResourceKind.ApiPropertySelectedKeys), "IList<EntityKey>?", "cyan", "null"),
            new TransferApiRow("SourceTitle", Lang(TransferShowCaseLangResourceKind.ApiPropertySourceTitle), "string?", "cyan", "null"),
            new TransferApiRow("TargetTitle", Lang(TransferShowCaseLangResourceKind.ApiPropertyTargetTitle), "string?", "cyan", "null"),
            new TransferApiRow("IsOneWay", Lang(TransferShowCaseLangResourceKind.ApiPropertyIsOneWay), "bool", "green", "false"),
            new TransferApiRow("IsFilterEnabled", Lang(TransferShowCaseLangResourceKind.ApiPropertyIsFilterEnabled), "bool", "green", "false"),
            new TransferApiRow("FilterPlaceholderText", Lang(TransferShowCaseLangResourceKind.ApiPropertyFilterPlaceholderText), "string?", "cyan", "null"),
            new TransferApiRow("FilterValueSelector", Lang(TransferShowCaseLangResourceKind.ApiPropertyFilterValueSelector), "DefaultFilterValueSelector?", "cyan", "null"),
            new TransferApiRow("ListWidth", Lang(TransferShowCaseLangResourceKind.ApiPropertyListWidth), "double", "green", "TransferToken.ListWidth"),
            new TransferApiRow("ListHeight", Lang(TransferShowCaseLangResourceKind.ApiPropertyListHeight), "double", "green", "TransferToken.ListHeight"),
            new TransferApiRow("PageSize", Lang(TransferShowCaseLangResourceKind.ApiPropertyPageSize), "int", "green", "0"),
            new TransferApiRow("Status", Lang(TransferShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "purple", "Default")
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
            new TransferDesignTokenRow("ListWidth", Lang(TransferShowCaseLangResourceKind.TokenNameListWidth), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TransferDesignTokenRow("ListWidthLG", Lang(TransferShowCaseLangResourceKind.TokenNameListWidthLG), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TransferDesignTokenRow("ListHeight", Lang(TransferShowCaseLangResourceKind.TokenNameListHeight), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TransferDesignTokenRow("ItemHeight", Lang(TransferShowCaseLangResourceKind.TokenNameItemHeight), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TransferDesignTokenRow("ItemPadding", Lang(TransferShowCaseLangResourceKind.TokenNameItemPadding), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TransferDesignTokenRow("HeaderHeight", Lang(TransferShowCaseLangResourceKind.TokenNameHeaderHeight), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TransferDesignTokenRow("HeaderPadding", Lang(TransferShowCaseLangResourceKind.TokenNameHeaderPadding), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TransferDesignTokenRow("PaginationMargin", Lang(TransferShowCaseLangResourceKind.TokenNamePaginationMargin), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TransferDesignTokenRow("DataGridSelectionHeaderMargin", Lang(TransferShowCaseLangResourceKind.TokenNameDataGridSelectionHeaderMargin), Lang(TransferShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TransferShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TransferShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TransferShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TransferShowCaseLangResourceKind.ApiPropertyItemsSource                   => en_US.ApiPropertyItemsSource,
            TransferShowCaseLangResourceKind.ApiPropertyTargetKeys                    => en_US.ApiPropertyTargetKeys,
            TransferShowCaseLangResourceKind.ApiPropertySelectedKeys                  => en_US.ApiPropertySelectedKeys,
            TransferShowCaseLangResourceKind.ApiPropertySourceTitle                   => en_US.ApiPropertySourceTitle,
            TransferShowCaseLangResourceKind.ApiPropertyTargetTitle                   => en_US.ApiPropertyTargetTitle,
            TransferShowCaseLangResourceKind.ApiPropertyIsOneWay                      => en_US.ApiPropertyIsOneWay,
            TransferShowCaseLangResourceKind.ApiPropertyIsFilterEnabled               => en_US.ApiPropertyIsFilterEnabled,
            TransferShowCaseLangResourceKind.ApiPropertyFilterPlaceholderText         => en_US.ApiPropertyFilterPlaceholderText,
            TransferShowCaseLangResourceKind.ApiPropertyFilterValueSelector           => en_US.ApiPropertyFilterValueSelector,
            TransferShowCaseLangResourceKind.ApiPropertyListWidth                     => en_US.ApiPropertyListWidth,
            TransferShowCaseLangResourceKind.ApiPropertyListHeight                    => en_US.ApiPropertyListHeight,
            TransferShowCaseLangResourceKind.ApiPropertyPageSize                      => en_US.ApiPropertyPageSize,
            TransferShowCaseLangResourceKind.ApiPropertyStatus                        => en_US.ApiPropertyStatus,
            TransferShowCaseLangResourceKind.TokenNameListWidth                       => en_US.TokenNameListWidth,
            TransferShowCaseLangResourceKind.TokenNameListWidthLG                     => en_US.TokenNameListWidthLG,
            TransferShowCaseLangResourceKind.TokenNameListHeight                      => en_US.TokenNameListHeight,
            TransferShowCaseLangResourceKind.TokenNameItemHeight                      => en_US.TokenNameItemHeight,
            TransferShowCaseLangResourceKind.TokenNameItemPadding                     => en_US.TokenNameItemPadding,
            TransferShowCaseLangResourceKind.TokenNameHeaderHeight                    => en_US.TokenNameHeaderHeight,
            TransferShowCaseLangResourceKind.TokenNameHeaderPadding                   => en_US.TokenNameHeaderPadding,
            TransferShowCaseLangResourceKind.TokenNamePaginationMargin                => en_US.TokenNamePaginationMargin,
            TransferShowCaseLangResourceKind.TokenNameDataGridSelectionHeaderMargin   => en_US.TokenNameDataGridSelectionHeaderMargin,
            TransferShowCaseLangResourceKind.TokenScopeComponent                      => en_US.TokenScopeComponent,
            TransferShowCaseLangResourceKind.TokenStatusStable                        => en_US.TokenStatusStable,
            _                                                                         => kind.ToString()
        };
    }
}

public sealed record TransferApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TransferDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);

public record SearchCaseItemData : ListItemData
{
    public string? Description { get; init; }
}

public record DataGridTransferData : IItemKey
{
    public EntityKey? ItemKey { get; init; }
    public string? Title { get; init; }
    public string? Description  { get; init; }
    public string? Tag  { get; init; }
}
