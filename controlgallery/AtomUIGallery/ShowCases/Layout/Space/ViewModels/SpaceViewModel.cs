using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Space;

public class SpaceViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "SpaceShowCase";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private CustomizableSizeType _sizeType;

    public CustomizableSizeType SizeType
    {
        get => _sizeType;
        set => this.RaiseAndSetIfChanged(ref _sizeType, value);
    }

    private double _customSpacingValue;

    public double CustomSpacingValue
    {
        get => _customSpacingValue;
        set => this.RaiseAndSetIfChanged(ref _customSpacingValue, value);
    }

    private List<ISelectOption>? _provinceOptions;

    public List<ISelectOption>? ProvinceOptions
    {
        get => _provinceOptions;
        set => this.RaiseAndSetIfChanged(ref _provinceOptions, value);
    }

    private List<ISelectOption>? _basicOptions;

    public List<ISelectOption>? BasicOptions
    {
        get => _basicOptions;
        set => this.RaiseAndSetIfChanged(ref _basicOptions, value);
    }

    private List<ISelectOption>? _firstNestedOptions;

    public List<ISelectOption>? FirstNestedOptions
    {
        get => _firstNestedOptions;
        set => this.RaiseAndSetIfChanged(ref _firstNestedOptions, value);
    }

    private List<ISelectOption>? _secondNestedOptions;

    public List<ISelectOption>? SecondNestedOptions
    {
        get => _secondNestedOptions;
        set => this.RaiseAndSetIfChanged(ref _secondNestedOptions, value);
    }

    private List<ISelectOption>? _conditionOptions;

    public List<ISelectOption>? ConditionOptions
    {
        get => _conditionOptions;
        set => this.RaiseAndSetIfChanged(ref _conditionOptions, value);
    }

    private List<ISelectOption>? _authActionOptions;

    public List<ISelectOption>? AuthActionOptions
    {
        get => _authActionOptions;
        set => this.RaiseAndSetIfChanged(ref _authActionOptions, value);
    }

    private List<IAutoCompleteOption>? _autoCompleteTextOptions;

    public List<IAutoCompleteOption>? AutoCompleteTextOptions
    {
        get => _autoCompleteTextOptions;
        set => this.RaiseAndSetIfChanged(ref _autoCompleteTextOptions, value);
    }

    private List<ICascaderOption>? _addressCascaderOptions;

    public List<ICascaderOption>? AddressCascaderOptions
    {
        get => _addressCascaderOptions;
        set => this.RaiseAndSetIfChanged(ref _addressCascaderOptions, value);
    }

    private List<ITreeItemNode>? _treeSelectNodes;
    private ObservableCollection<SpaceApiRow>? _apiRows;
    private ObservableCollection<SpaceDesignTokenRow>? _designTokenRows;

    public List<ITreeItemNode>? TreeSelectNodes
    {
        get => _treeSelectNodes;
        set => this.RaiseAndSetIfChanged(ref _treeSelectNodes, value);
    }

    public ObservableCollection<SpaceApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SpaceDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public SpaceViewModel(IScreen screen)
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
            new SpaceApiRow("Space.ItemSpacing", Lang(SpaceShowCaseLangResourceKind.ApiPropertySpaceItemSpacing), "double", "cyan", "token"),
            new SpaceApiRow("Space.LineSpacing", Lang(SpaceShowCaseLangResourceKind.ApiPropertySpaceLineSpacing), "double", "cyan", "token"),
            new SpaceApiRow("Space.Orientation", Lang(SpaceShowCaseLangResourceKind.ApiPropertySpaceOrientation), "Orientation", "cyan", "Horizontal"),
            new SpaceApiRow("Space.ItemsAlignment", Lang(SpaceShowCaseLangResourceKind.ApiPropertySpaceItemsAlignment), "SpaceItemsAlignment", "cyan", "Start"),
            new SpaceApiRow("Space.SizeType", Lang(SpaceShowCaseLangResourceKind.ApiPropertySpaceSizeType), "CustomizableSizeType", "cyan", "Small"),
            new SpaceApiRow("Space.ItemWidth", Lang(SpaceShowCaseLangResourceKind.ApiPropertySpaceItemWidth), "double", "cyan", "NaN"),
            new SpaceApiRow("Space.ItemHeight", Lang(SpaceShowCaseLangResourceKind.ApiPropertySpaceItemHeight), "double", "cyan", "NaN"),
            new SpaceApiRow("Space.SplitTemplate", Lang(SpaceShowCaseLangResourceKind.ApiPropertySpaceSplitTemplate), "ITemplate<Control>?", "cyan", "null"),
            new SpaceApiRow("CompactSpace.Orientation", Lang(SpaceShowCaseLangResourceKind.ApiPropertyCompactSpaceOrientation), "Orientation", "cyan", "Horizontal"),
            new SpaceApiRow("CompactSpace.SizeType", Lang(SpaceShowCaseLangResourceKind.ApiPropertyCompactSpaceSizeType), "CustomizableSizeType", "cyan", "Middle"),
            new SpaceApiRow("CompactSpace.ItemSize", Lang(SpaceShowCaseLangResourceKind.ApiPropertyCompactSpaceItemSize), "CompactSpaceSize", "cyan", "Auto")
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
            new SpaceDesignTokenRow("GapSmallSize", Lang(SpaceShowCaseLangResourceKind.TokenNameGapSmallSize), Lang(SpaceShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(SpaceShowCaseLangResourceKind.TokenStatusDefault), "success"),
            new SpaceDesignTokenRow("GapMiddleSize", Lang(SpaceShowCaseLangResourceKind.TokenNameGapMiddleSize), Lang(SpaceShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(SpaceShowCaseLangResourceKind.TokenStatusDefault), "success"),
            new SpaceDesignTokenRow("GapLargeSize", Lang(SpaceShowCaseLangResourceKind.TokenNameGapLargeSize), Lang(SpaceShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(SpaceShowCaseLangResourceKind.TokenStatusDefault), "success"),
            new SpaceDesignTokenRow("AddonBg", Lang(SpaceShowCaseLangResourceKind.TokenNameAddonBg), Lang(SpaceShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(SpaceShowCaseLangResourceKind.TokenStatusDefault), "success"),
            new SpaceDesignTokenRow("AddOnPadding", Lang(SpaceShowCaseLangResourceKind.TokenNameAddOnPadding), Lang(SpaceShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(SpaceShowCaseLangResourceKind.TokenStatusDefault), "success"),
            new SpaceDesignTokenRow("AddOnPaddingSM", Lang(SpaceShowCaseLangResourceKind.TokenNameAddOnPaddingSM), Lang(SpaceShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(SpaceShowCaseLangResourceKind.TokenStatusDefault), "success"),
            new SpaceDesignTokenRow("AddOnPaddingLG", Lang(SpaceShowCaseLangResourceKind.TokenNameAddOnPaddingLG), Lang(SpaceShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(SpaceShowCaseLangResourceKind.TokenStatusDefault), "success")
        ];
    }

    private static string Lang(SpaceShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return SpaceShowCaseLanguage.Get(kind, FallbackLang(kind));
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SpaceShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SpaceShowCaseLangResourceKind.ApiPropertySpaceItemSpacing          => en_US.ApiPropertySpaceItemSpacing,
            SpaceShowCaseLangResourceKind.ApiPropertySpaceLineSpacing          => en_US.ApiPropertySpaceLineSpacing,
            SpaceShowCaseLangResourceKind.ApiPropertySpaceOrientation          => en_US.ApiPropertySpaceOrientation,
            SpaceShowCaseLangResourceKind.ApiPropertySpaceItemsAlignment       => en_US.ApiPropertySpaceItemsAlignment,
            SpaceShowCaseLangResourceKind.ApiPropertySpaceSizeType             => en_US.ApiPropertySpaceSizeType,
            SpaceShowCaseLangResourceKind.ApiPropertySpaceItemWidth            => en_US.ApiPropertySpaceItemWidth,
            SpaceShowCaseLangResourceKind.ApiPropertySpaceItemHeight           => en_US.ApiPropertySpaceItemHeight,
            SpaceShowCaseLangResourceKind.ApiPropertySpaceSplitTemplate        => en_US.ApiPropertySpaceSplitTemplate,
            SpaceShowCaseLangResourceKind.ApiPropertyCompactSpaceOrientation   => en_US.ApiPropertyCompactSpaceOrientation,
            SpaceShowCaseLangResourceKind.ApiPropertyCompactSpaceSizeType      => en_US.ApiPropertyCompactSpaceSizeType,
            SpaceShowCaseLangResourceKind.ApiPropertyCompactSpaceItemSize      => en_US.ApiPropertyCompactSpaceItemSize,
            SpaceShowCaseLangResourceKind.TokenNameGapSmallSize                => en_US.TokenNameGapSmallSize,
            SpaceShowCaseLangResourceKind.TokenNameGapMiddleSize               => en_US.TokenNameGapMiddleSize,
            SpaceShowCaseLangResourceKind.TokenNameGapLargeSize                => en_US.TokenNameGapLargeSize,
            SpaceShowCaseLangResourceKind.TokenNameAddonBg                     => en_US.TokenNameAddonBg,
            SpaceShowCaseLangResourceKind.TokenNameAddOnPadding                => en_US.TokenNameAddOnPadding,
            SpaceShowCaseLangResourceKind.TokenNameAddOnPaddingSM              => en_US.TokenNameAddOnPaddingSM,
            SpaceShowCaseLangResourceKind.TokenNameAddOnPaddingLG              => en_US.TokenNameAddOnPaddingLG,
            SpaceShowCaseLangResourceKind.TokenScopeComponent                  => en_US.TokenScopeComponent,
            SpaceShowCaseLangResourceKind.TokenStatusDefault                   => en_US.TokenStatusDefault,
            _                                                                  => kind.ToString()
        };
    }
}

public sealed record SpaceApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SpaceDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
