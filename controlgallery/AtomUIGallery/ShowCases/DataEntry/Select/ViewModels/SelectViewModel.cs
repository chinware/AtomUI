using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Select;

public class SelectViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Select";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

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
}

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
