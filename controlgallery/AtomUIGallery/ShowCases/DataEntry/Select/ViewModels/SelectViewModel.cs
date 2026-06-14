using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Select;

public class SelectViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Select";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<SelectApiRow>? _apiRows;

    public ObservableCollection<SelectApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    private ObservableCollection<SelectDesignTokenRow>? _designTokenRows;

    public ObservableCollection<SelectDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private List<SelectOption>? _randomOptions;

    public List<SelectOption>? RandomOptions
    {
        get => _randomOptions;
        set => this.RaiseAndSetIfChanged(ref _randomOptions, value);
    }

    private List<ISelectOption>? _basicSelectedOptions = [];

    public List<ISelectOption>? BasicSelectedOptions
    {
        get => _basicSelectedOptions;
        set => this.RaiseAndSetIfChanged(ref _basicSelectedOptions, value);
    }

    private List<ISelectOption>? _defaultSelectedOptions;

    public List<ISelectOption>? DefaultSelectedOptions
    {
        get => _defaultSelectedOptions;
        set => this.RaiseAndSetIfChanged(ref _defaultSelectedOptions, value);
    }

    private List<ISelectOption>? _singleLucyOptions;

    public List<ISelectOption>? SingleLucyOptions
    {
        get => _singleLucyOptions;
        set => this.RaiseAndSetIfChanged(ref _singleLucyOptions, value);
    }

    private List<ISelectOption>? _searchOptions;

    public List<ISelectOption>? SearchOptions
    {
        get => _searchOptions;
        set => this.RaiseAndSetIfChanged(ref _searchOptions, value);
    }

    private List<CustomOption>? _customCountryOptions;

    public List<CustomOption>? CustomCountryOptions
    {
        get => _customCountryOptions;
        set => this.RaiseAndSetIfChanged(ref _customCountryOptions, value);
    }

    private List<ISelectOption>? _groupedPersonOptions;

    public List<ISelectOption>? GroupedPersonOptions
    {
        get => _groupedPersonOptions;
        set => this.RaiseAndSetIfChanged(ref _groupedPersonOptions, value);
    }

    private List<ISelectOption>? _variantOptions;

    public List<ISelectOption>? VariantOptions
    {
        get => _variantOptions;
        set => this.RaiseAndSetIfChanged(ref _variantOptions, value);
    }

    private List<ISelectOption>? _hideSelectedOptions;

    public List<ISelectOption>? HideSelectedOptions
    {
        get => _hideSelectedOptions;
        set => this.RaiseAndSetIfChanged(ref _hideSelectedOptions, value);
    }

    private List<ISelectOption>? _maxCountLimitedOptions;

    public List<ISelectOption>? MaxCountLimitedOptions
    {
        get => _maxCountLimitedOptions;
        set => this.RaiseAndSetIfChanged(ref _maxCountLimitedOptions, value);
    }

    private List<ISelectOption>? _prefixSuffixOptions;

    public List<ISelectOption>? PrefixSuffixOptions
    {
        get => _prefixSuffixOptions;
        set => this.RaiseAndSetIfChanged(ref _prefixSuffixOptions, value);
    }

    private List<SelectOption>? _maxTagCountOptions;

    public List<SelectOption>? MaxTagCountOptions
    {
        get => _maxTagCountOptions;
        set => this.RaiseAndSetIfChanged(ref _maxTagCountOptions, value);
    }

    private SizeType _selectSizeType;

    public SizeType SelectSizeType
    {
        get => _selectSizeType;
        set => this.RaiseAndSetIfChanged(ref _selectSizeType, value);
    }

    private ISelectOptionsAsyncLoader? _selectOptionsAsyncLoader;

    public ISelectOptionsAsyncLoader? SelectOptionsAsyncLoader
    {
        get => _selectOptionsAsyncLoader;
        set => this.RaiseAndSetIfChanged(ref _selectOptionsAsyncLoader, value);
    }

    public SelectViewModel(IScreen screen)
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
            new SelectApiRow("Mode", Lang(SelectShowCaseLangResourceKind.ApiPropertyMode), "SelectMode", "purple", "Single"),
            new SelectApiRow("OptionsSource", Lang(SelectShowCaseLangResourceKind.ApiPropertyOptionsSource), "IEnumerable<ISelectOption>?", "cyan", "null"),
            new SelectApiRow("SelectedOptions", Lang(SelectShowCaseLangResourceKind.ApiPropertySelectedOptions), "IList<ISelectOption>?", "cyan", "null"),
            new SelectApiRow("DefaultValues", Lang(SelectShowCaseLangResourceKind.ApiPropertyDefaultValues), "string?", "cyan", "null"),
            new SelectApiRow("PlaceholderText", Lang(SelectShowCaseLangResourceKind.ApiPropertyPlaceholderText), "string?", "cyan", "null"),
            new SelectApiRow("IsAllowClear", Lang(SelectShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "green", "false"),
            new SelectApiRow("IsFilterEnabled", Lang(SelectShowCaseLangResourceKind.ApiPropertyIsFilterEnabled), "bool", "green", "false"),
            new SelectApiRow("Filter", Lang(SelectShowCaseLangResourceKind.ApiPropertyFilter), "IValueFilter?", "cyan", "null"),
            new SelectApiRow("IsGroupEnabled", Lang(SelectShowCaseLangResourceKind.ApiPropertyIsGroupEnabled), "bool", "green", "false"),
            new SelectApiRow("IsHideSelectedOptions", Lang(SelectShowCaseLangResourceKind.ApiPropertyIsHideSelectedOptions), "bool", "green", "false"),
            new SelectApiRow("MaxCount", Lang(SelectShowCaseLangResourceKind.ApiPropertyMaxCount), "int", "green", "0"),
            new SelectApiRow("IsResponsiveTagMode", Lang(SelectShowCaseLangResourceKind.ApiPropertyIsResponsiveTagMode), "bool", "green", "false"),
            new SelectApiRow("MaxTagCount", Lang(SelectShowCaseLangResourceKind.ApiPropertyMaxTagCount), "int", "green", "0"),
            new SelectApiRow("OptionsLoader", Lang(SelectShowCaseLangResourceKind.ApiPropertyOptionsLoader), "ISelectOptionsAsyncLoader?", "cyan", "null"),
            new SelectApiRow("StyleVariant", Lang(SelectShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "purple", "Outlined"),
            new SelectApiRow("Status", Lang(SelectShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "purple", "Default"),
            new SelectApiRow("SizeType", Lang(SelectShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "purple", "Middle")
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
            new SelectDesignTokenRow("MultipleItemBg", Lang(SelectShowCaseLangResourceKind.TokenNameMultipleItemBg), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("MultipleItemHeight", Lang(SelectShowCaseLangResourceKind.TokenNameMultipleItemHeight), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("MultipleItemHeightSM", Lang(SelectShowCaseLangResourceKind.TokenNameMultipleItemHeightSM), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("MultipleItemHeightLG", Lang(SelectShowCaseLangResourceKind.TokenNameMultipleItemHeightLG), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("MultipleSelectorBgDisabled", Lang(SelectShowCaseLangResourceKind.TokenNameMultipleSelectorBgDisabled), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("OptionSelectedColor", Lang(SelectShowCaseLangResourceKind.TokenNameOptionSelectedColor), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("OptionSelectedBg", Lang(SelectShowCaseLangResourceKind.TokenNameOptionSelectedBg), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("OptionActiveBg", Lang(SelectShowCaseLangResourceKind.TokenNameOptionActiveBg), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("OptionPadding", Lang(SelectShowCaseLangResourceKind.TokenNameOptionPadding), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("OptionHeight", Lang(SelectShowCaseLangResourceKind.TokenNameOptionHeight), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SelectDesignTokenRow("PopupContentPadding", Lang(SelectShowCaseLangResourceKind.TokenNamePopupContentPadding), Lang(SelectShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SelectShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(SelectShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SelectShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SelectShowCaseLangResourceKind.ApiPropertyMode                       => en_US.ApiPropertyMode,
            SelectShowCaseLangResourceKind.ApiPropertyOptionsSource              => en_US.ApiPropertyOptionsSource,
            SelectShowCaseLangResourceKind.ApiPropertySelectedOptions            => en_US.ApiPropertySelectedOptions,
            SelectShowCaseLangResourceKind.ApiPropertyDefaultValues              => en_US.ApiPropertyDefaultValues,
            SelectShowCaseLangResourceKind.ApiPropertyPlaceholderText            => en_US.ApiPropertyPlaceholderText,
            SelectShowCaseLangResourceKind.ApiPropertyIsAllowClear               => en_US.ApiPropertyIsAllowClear,
            SelectShowCaseLangResourceKind.ApiPropertyIsFilterEnabled            => en_US.ApiPropertyIsFilterEnabled,
            SelectShowCaseLangResourceKind.ApiPropertyFilter                     => en_US.ApiPropertyFilter,
            SelectShowCaseLangResourceKind.ApiPropertyIsGroupEnabled             => en_US.ApiPropertyIsGroupEnabled,
            SelectShowCaseLangResourceKind.ApiPropertyIsHideSelectedOptions      => en_US.ApiPropertyIsHideSelectedOptions,
            SelectShowCaseLangResourceKind.ApiPropertyMaxCount                   => en_US.ApiPropertyMaxCount,
            SelectShowCaseLangResourceKind.ApiPropertyIsResponsiveTagMode        => en_US.ApiPropertyIsResponsiveTagMode,
            SelectShowCaseLangResourceKind.ApiPropertyMaxTagCount                => en_US.ApiPropertyMaxTagCount,
            SelectShowCaseLangResourceKind.ApiPropertyOptionsLoader              => en_US.ApiPropertyOptionsLoader,
            SelectShowCaseLangResourceKind.ApiPropertyStyleVariant               => en_US.ApiPropertyStyleVariant,
            SelectShowCaseLangResourceKind.ApiPropertyStatus                     => en_US.ApiPropertyStatus,
            SelectShowCaseLangResourceKind.ApiPropertySizeType                   => en_US.ApiPropertySizeType,
            SelectShowCaseLangResourceKind.TokenNameMultipleItemBg               => en_US.TokenNameMultipleItemBg,
            SelectShowCaseLangResourceKind.TokenNameMultipleItemHeight           => en_US.TokenNameMultipleItemHeight,
            SelectShowCaseLangResourceKind.TokenNameMultipleItemHeightSM         => en_US.TokenNameMultipleItemHeightSM,
            SelectShowCaseLangResourceKind.TokenNameMultipleItemHeightLG         => en_US.TokenNameMultipleItemHeightLG,
            SelectShowCaseLangResourceKind.TokenNameMultipleSelectorBgDisabled   => en_US.TokenNameMultipleSelectorBgDisabled,
            SelectShowCaseLangResourceKind.TokenNameOptionSelectedColor          => en_US.TokenNameOptionSelectedColor,
            SelectShowCaseLangResourceKind.TokenNameOptionSelectedBg             => en_US.TokenNameOptionSelectedBg,
            SelectShowCaseLangResourceKind.TokenNameOptionActiveBg               => en_US.TokenNameOptionActiveBg,
            SelectShowCaseLangResourceKind.TokenNameOptionPadding                => en_US.TokenNameOptionPadding,
            SelectShowCaseLangResourceKind.TokenNameOptionHeight                 => en_US.TokenNameOptionHeight,
            SelectShowCaseLangResourceKind.TokenNamePopupContentPadding          => en_US.TokenNamePopupContentPadding,
            SelectShowCaseLangResourceKind.TokenScopeComponent                   => en_US.TokenScopeComponent,
            SelectShowCaseLangResourceKind.TokenStatusStable                     => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }
}

public sealed record SelectApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SelectDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);

public class SelectOptionsAsyncLoader : ISelectOptionsAsyncLoader
{
    public async Task<SelectOptionsLoadResult> LoadAsync(object? context, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(600), token);
        var options = new List<ISelectOption>();
        options.Add(new SelectOption()
        {
            Header  = SelectShowCaseLanguage.Get(SelectShowCaseLangResourceKind.P2HeaderJack, "Jack"),
            Content = "jack"
        });
        options.Add(new SelectOption()
        {
            Header  = SelectShowCaseLanguage.Get(SelectShowCaseLangResourceKind.P2HeaderLucy, "Lucy"),
            Content = "lucy"
        });
        options.Add(new SelectOption()
        {
            Header  = SelectShowCaseLanguage.Get(SelectShowCaseLangResourceKind.P2HeaderYiminghe, "Yiminghe"),
            Content = "yiminghe"
        });
        options.Add(new SelectOption()
        {
            Header    = SelectShowCaseLanguage.Get(SelectShowCaseLangResourceKind.P2HeaderDisabled, "Disabled"),
            Content   = "disabled",
            IsEnabled = false
        });
        return new SelectOptionsLoadResult()
        {
            Data       = options,
            StatusCode = RpcStatusCode.Success
        };
    }
}
