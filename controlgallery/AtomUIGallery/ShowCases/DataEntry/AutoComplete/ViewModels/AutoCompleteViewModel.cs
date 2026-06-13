using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.AutoComplete;

public class AutoCompleteViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "AutoComplete";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<AutoCompleteApiRow>? _apiRows;
    private ObservableCollection<AutoCompleteDesignTokenRow>? _designTokenRows;

    public ObservableCollection<AutoCompleteApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<AutoCompleteDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private ICompleteOptionsAsyncLoader? _basicOptionsAsyncLoader;

    public ICompleteOptionsAsyncLoader? BasicOptionsAsyncLoader
    {
        get => _basicOptionsAsyncLoader;
        set => this.RaiseAndSetIfChanged(ref _basicOptionsAsyncLoader, value);
    }

    private ICompleteOptionsAsyncLoader? _customLabelOptionsAsyncLoader;

    public ICompleteOptionsAsyncLoader? CustomLabelOptionsAsyncLoader
    {
        get => _customLabelOptionsAsyncLoader;
        set => this.RaiseAndSetIfChanged(ref _customLabelOptionsAsyncLoader, value);
    }

    private ICompleteOptionsAsyncLoader? _searchEditOptionsAsyncLoader;

    public ICompleteOptionsAsyncLoader? SearchEditOptionsAsyncLoader
    {
        get => _searchEditOptionsAsyncLoader;
        set => this.RaiseAndSetIfChanged(ref _searchEditOptionsAsyncLoader, value);
    }

    private List<IAutoCompleteOption>? _filterCaseOptions;

    public List<IAutoCompleteOption>? FilterCaseOptions
    {
        get => _filterCaseOptions;
        set => this.RaiseAndSetIfChanged(ref _filterCaseOptions, value);
    }

    private List<IAutoCompleteOption>? _cityOptions;

    public List<IAutoCompleteOption>? CityOptions
    {
        get => _cityOptions;
        set => this.RaiseAndSetIfChanged(ref _cityOptions, value);
    }

    public AutoCompleteViewModel(IScreen screen)
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
            new AutoCompleteApiRow("Value", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyValue), "string?", "cyan", "null"),
            new AutoCompleteApiRow("OptionsSource", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyOptionsSource), "IEnumerable<IAutoCompleteOption>?", "cyan", "null"),
            new AutoCompleteApiRow("OptionsAsyncLoader", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyOptionsAsyncLoader), "ICompleteOptionsAsyncLoader?", "cyan", "null"),
            new AutoCompleteApiRow("OptionTemplate", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyOptionTemplate), "IDataTemplate?", "cyan", "null"),
            new AutoCompleteApiRow("Filter", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyFilter), "IValueFilter?", "cyan", "StartsWith"),
            new AutoCompleteApiRow("FilterValueSelector", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyFilterValueSelector), "DefaultFilterValueSelector?", "cyan", "null"),
            new AutoCompleteApiRow("IsAllowClear", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "green", "false"),
            new AutoCompleteApiRow("ClearIcon", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyClearIcon), "PathIcon?", "cyan", "null"),
            new AutoCompleteApiRow("StyleVariant", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "purple", "Outlined"),
            new AutoCompleteApiRow("Status", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "purple", "Default"),
            new AutoCompleteApiRow("Placement", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyPlacement), "AutoCompletePlacementMode", "purple", "Bottom"),
            new AutoCompleteApiRow("MinimumPrefixLength", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyMinimumPrefixLength), "int", "green", "1"),
            new AutoCompleteApiRow("DisplayCandidateCount", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyDisplayCandidateCount), "int", "green", "10"),
            new AutoCompleteApiRow("MaxDropDownHeight", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyMaxDropDownHeight), "double", "green", "Infinity"),
            new AutoCompleteApiRow("IsPopupMatchSelectWidth", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyIsPopupMatchSelectWidth), "bool", "green", "true"),
            new AutoCompleteApiRow("ShouldUseOverlayPopup", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup), "bool", "green", "true"),
            new AutoCompleteApiRow("AutoCompleteSearchEdit.SearchButtonStyle", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyAutoCompleteSearchButtonStyle), "SearchEditButtonStyle", "purple", "Default"),
            new AutoCompleteApiRow("AutoCompleteTextArea.Lines", Lang(AutoCompleteShowCaseLangResourceKind.ApiPropertyAutoCompleteTextAreaLines), "int", "green", "1")
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
            new AutoCompleteDesignTokenRow("PopupContentPadding", Lang(AutoCompleteShowCaseLangResourceKind.TokenNamePopupContentPadding), Lang(AutoCompleteShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AutoCompleteShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AutoCompleteDesignTokenRow("OptionHeight", Lang(AutoCompleteShowCaseLangResourceKind.TokenNameOptionHeight), Lang(AutoCompleteShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AutoCompleteShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AutoCompleteDesignTokenRow("MinPopupWidth", Lang(AutoCompleteShowCaseLangResourceKind.TokenNameMinPopupWidth), Lang(AutoCompleteShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AutoCompleteShowCaseLangResourceKind.TokenStatusStable), "success"),
            new AutoCompleteDesignTokenRow("MaxPopupWidth", Lang(AutoCompleteShowCaseLangResourceKind.TokenNameMaxPopupWidth), Lang(AutoCompleteShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(AutoCompleteShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(AutoCompleteShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(AutoCompleteShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            AutoCompleteShowCaseLangResourceKind.ApiPropertyValue                         => en_US.ApiPropertyValue,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyOptionsSource                 => en_US.ApiPropertyOptionsSource,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyOptionsAsyncLoader            => en_US.ApiPropertyOptionsAsyncLoader,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyOptionTemplate                => en_US.ApiPropertyOptionTemplate,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyFilter                        => en_US.ApiPropertyFilter,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyFilterValueSelector           => en_US.ApiPropertyFilterValueSelector,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyIsAllowClear                  => en_US.ApiPropertyIsAllowClear,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyClearIcon                     => en_US.ApiPropertyClearIcon,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyStyleVariant                  => en_US.ApiPropertyStyleVariant,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyStatus                        => en_US.ApiPropertyStatus,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyPlacement                     => en_US.ApiPropertyPlacement,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyMinimumPrefixLength           => en_US.ApiPropertyMinimumPrefixLength,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyDisplayCandidateCount         => en_US.ApiPropertyDisplayCandidateCount,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyMaxDropDownHeight             => en_US.ApiPropertyMaxDropDownHeight,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyIsPopupMatchSelectWidth       => en_US.ApiPropertyIsPopupMatchSelectWidth,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup         => en_US.ApiPropertyShouldUseOverlayPopup,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyAutoCompleteSearchButtonStyle => en_US.ApiPropertyAutoCompleteSearchButtonStyle,
            AutoCompleteShowCaseLangResourceKind.ApiPropertyAutoCompleteTextAreaLines     => en_US.ApiPropertyAutoCompleteTextAreaLines,
            AutoCompleteShowCaseLangResourceKind.TokenNamePopupContentPadding             => en_US.TokenNamePopupContentPadding,
            AutoCompleteShowCaseLangResourceKind.TokenNameOptionHeight                    => en_US.TokenNameOptionHeight,
            AutoCompleteShowCaseLangResourceKind.TokenNameMinPopupWidth                   => en_US.TokenNameMinPopupWidth,
            AutoCompleteShowCaseLangResourceKind.TokenNameMaxPopupWidth                   => en_US.TokenNameMaxPopupWidth,
            AutoCompleteShowCaseLangResourceKind.TokenScopeComponent                      => en_US.TokenScopeComponent,
            AutoCompleteShowCaseLangResourceKind.TokenStatusStable                        => en_US.TokenStatusStable,
            _                                                                             => kind.ToString()
        };
    }
}

public sealed record AutoCompleteApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record AutoCompleteDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);

public class BasicOptionsAsyncLoader : ICompleteOptionsAsyncLoader
{
    public async Task<CompleteOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        List<IAutoCompleteOption> data = [];
        if (!string.IsNullOrWhiteSpace(context))
        {
            {
                var value = context;
                data.Add(new AutoCompleteOption()
                {
                    Header = value.Replace("\r\n", " ")
                                  .Replace("\n", " "),
                    Content = value
                });
            }
            {
                var value = string.Concat(Enumerable.Repeat(context, 2));
                data.Add(new AutoCompleteOption()
                {
                    Header = value.Replace("\r\n", " ")
                                  .Replace("\n", " "),
                    Content = value
                });
            }
            {
                var value = string.Concat(Enumerable.Repeat(context, 3));
                data.Add(new AutoCompleteOption()
                {
                    Header = value.Replace("\r\n", " ")
                                  .Replace("\n", " "),
                    Content = value
                });
            }
        }
        return new CompleteOptionsLoadResult()
        {
            StatusCode = RpcStatusCode.Success,
            Data       = data
        };
    }
}

public class CustomLabelOptionsAsyncLoader : ICompleteOptionsAsyncLoader
{
    protected List<string> Suffixes = new();

    public CustomLabelOptionsAsyncLoader()
    {
        Suffixes.Add("gmail.com");
        Suffixes.Add("163.com");
        Suffixes.Add("qq.com");
    }

    public async Task<CompleteOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        List<IAutoCompleteOption> data = [];

        if (context != null && !string.IsNullOrWhiteSpace(context) && !context.Contains('@'))
        {
            foreach (var suffix in Suffixes)
            {
                var value = $"{context}@{suffix}";
                data.Add(new AutoCompleteOption()
                {
                    Header  = value,
                    Content = value
                });
            }
        }

        return new CompleteOptionsLoadResult()
        {
            Data = data
        };
    }
}

public class SearchEditOptionsAsyncLoader : ICompleteOptionsAsyncLoader
{
    public async Task<CompleteOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        List<IAutoCompleteOption> data   = [];
        var                       random = new Random();

        if (!string.IsNullOrWhiteSpace(context))
        {
            var count = random.Next(5);
            for (var i = 0; i < count; i++)
            {
                var newValue = $"{context}{i}";
                var value    = $"Found {context} on {newValue}";
                data.Add(new CustomAutoCompleteOption()
                {
                    Header      = value,
                    Content     = newValue,
                    ResultCount = random.Next(100, 200)
                });
            }
        }

        return new CompleteOptionsLoadResult()
        {
            Data = data
        };
    }
}

public record CustomAutoCompleteOption : AutoCompleteOption
{
    public int ResultCount { get; set; }
}

public record CityAutoCompleteOption : AutoCompleteOption
{
    public string? Country { get; set; }
    public int Population { get; set; }
}
