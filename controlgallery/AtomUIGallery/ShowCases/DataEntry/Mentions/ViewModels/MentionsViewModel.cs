using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.DataLoad;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Mentions;

public class MentionsViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Mentions";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<MentionsApiRow>? _apiRows;
    private ObservableCollection<MentionsDesignTokenRow>? _designTokenRows;

    public ObservableCollection<MentionsApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<MentionsDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private List<IMentionOption>? _basicMentionOptions = [];

    public List<IMentionOption>? BasicMentionOptions
    {
        get => _basicMentionOptions;
        set => this.RaiseAndSetIfChanged(ref _basicMentionOptions, value);
    }

    private IMentionOptionsAsyncLoader? _mentionOptionAsyncLoader;

    public IMentionOptionsAsyncLoader? MentionOptionAsyncLoader
    {
        get => _mentionOptionAsyncLoader;
        set => this.RaiseAndSetIfChanged(ref _mentionOptionAsyncLoader, value);
    }

    private List<string>? _mentionTriggers = [];

    public List<string>? MentionTriggers
    {
        get => _mentionTriggers;
        set => this.RaiseAndSetIfChanged(ref _mentionTriggers, value);
    }

    public MentionsViewModel(IScreen screen)
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
            new MentionsApiRow("Value", Lang(MentionsShowCaseLangResourceKind.ApiPropertyValue), "string?", "cyan", "null"),
            new MentionsApiRow("DefaultValue", Lang(MentionsShowCaseLangResourceKind.ApiPropertyDefaultValue), "string?", "cyan", "null"),
            new MentionsApiRow("OptionsSource", Lang(MentionsShowCaseLangResourceKind.ApiPropertyOptionsSource), "IEnumerable<IMentionOption>?", "cyan", "null"),
            new MentionsApiRow("OptionsAsyncLoader", Lang(MentionsShowCaseLangResourceKind.ApiPropertyOptionsAsyncLoader), "IMentionOptionsAsyncLoader?", "cyan", "null"),
            new MentionsApiRow("OptionTemplate", Lang(MentionsShowCaseLangResourceKind.ApiPropertyOptionTemplate), "IDataTemplate?", "cyan", "null"),
            new MentionsApiRow("TriggerPrefix", Lang(MentionsShowCaseLangResourceKind.ApiPropertyTriggerPrefix), "IList<string>?", "cyan", "null"),
            new MentionsApiRow("Split", Lang(MentionsShowCaseLangResourceKind.ApiPropertySplit), "string?", "cyan", "null"),
            new MentionsApiRow("IsAllowClear", Lang(MentionsShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "green", "false"),
            new MentionsApiRow("ClearIcon", Lang(MentionsShowCaseLangResourceKind.ApiPropertyClearIcon), "PathIcon?", "cyan", "null"),
            new MentionsApiRow("StyleVariant", Lang(MentionsShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "purple", "Outlined"),
            new MentionsApiRow("Status", Lang(MentionsShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "purple", "Default"),
            new MentionsApiRow("Placement", Lang(MentionsShowCaseLangResourceKind.ApiPropertyPlacement), "MentionsPlacementMode", "purple", "Bottom"),
            new MentionsApiRow("IsAutoSize", Lang(MentionsShowCaseLangResourceKind.ApiPropertyIsAutoSize), "bool", "green", "false"),
            new MentionsApiRow("Lines", Lang(MentionsShowCaseLangResourceKind.ApiPropertyLines), "int", "green", "1"),
            new MentionsApiRow("MinLines", Lang(MentionsShowCaseLangResourceKind.ApiPropertyMinLines), "int", "green", "1"),
            new MentionsApiRow("MaxLines", Lang(MentionsShowCaseLangResourceKind.ApiPropertyMaxLines), "int", "green", "int.MaxValue"),
            new MentionsApiRow("DisplayCandidateCount", Lang(MentionsShowCaseLangResourceKind.ApiPropertyDisplayCandidateCount), "int", "green", "10"),
            new MentionsApiRow("IsReadOnly", Lang(MentionsShowCaseLangResourceKind.ApiPropertyIsReadOnly), "bool", "green", "false"),
            new MentionsApiRow("AsyncLoadDebounce", Lang(MentionsShowCaseLangResourceKind.ApiPropertyAsyncLoadDebounce), "TimeSpan", "cyan", "0ms"),
            new MentionsApiRow("AsyncLoadTimeout", Lang(MentionsShowCaseLangResourceKind.ApiPropertyAsyncLoadTimeout), "TimeSpan", "cyan", "30s"),
            new MentionsApiRow("ShouldUseOverlayPopup", Lang(MentionsShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup), "bool", "green", "true")
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
            new MentionsDesignTokenRow("PopupContentPadding", Lang(MentionsShowCaseLangResourceKind.TokenNamePopupContentPadding), Lang(MentionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MentionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MentionsDesignTokenRow("OptionHeight", Lang(MentionsShowCaseLangResourceKind.TokenNameOptionHeight), Lang(MentionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MentionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new MentionsDesignTokenRow("MinPopupWidth", Lang(MentionsShowCaseLangResourceKind.TokenNameMinPopupWidth), Lang(MentionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(MentionsShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(MentionsShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(MentionsShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            MentionsShowCaseLangResourceKind.ApiPropertyValue                 => en_US.ApiPropertyValue,
            MentionsShowCaseLangResourceKind.ApiPropertyDefaultValue          => en_US.ApiPropertyDefaultValue,
            MentionsShowCaseLangResourceKind.ApiPropertyOptionsSource         => en_US.ApiPropertyOptionsSource,
            MentionsShowCaseLangResourceKind.ApiPropertyOptionsAsyncLoader    => en_US.ApiPropertyOptionsAsyncLoader,
            MentionsShowCaseLangResourceKind.ApiPropertyOptionTemplate        => en_US.ApiPropertyOptionTemplate,
            MentionsShowCaseLangResourceKind.ApiPropertyTriggerPrefix         => en_US.ApiPropertyTriggerPrefix,
            MentionsShowCaseLangResourceKind.ApiPropertySplit                 => en_US.ApiPropertySplit,
            MentionsShowCaseLangResourceKind.ApiPropertyIsAllowClear          => en_US.ApiPropertyIsAllowClear,
            MentionsShowCaseLangResourceKind.ApiPropertyClearIcon             => en_US.ApiPropertyClearIcon,
            MentionsShowCaseLangResourceKind.ApiPropertyStyleVariant          => en_US.ApiPropertyStyleVariant,
            MentionsShowCaseLangResourceKind.ApiPropertyStatus                => en_US.ApiPropertyStatus,
            MentionsShowCaseLangResourceKind.ApiPropertyPlacement             => en_US.ApiPropertyPlacement,
            MentionsShowCaseLangResourceKind.ApiPropertyIsAutoSize            => en_US.ApiPropertyIsAutoSize,
            MentionsShowCaseLangResourceKind.ApiPropertyLines                 => en_US.ApiPropertyLines,
            MentionsShowCaseLangResourceKind.ApiPropertyMinLines              => en_US.ApiPropertyMinLines,
            MentionsShowCaseLangResourceKind.ApiPropertyMaxLines              => en_US.ApiPropertyMaxLines,
            MentionsShowCaseLangResourceKind.ApiPropertyDisplayCandidateCount => en_US.ApiPropertyDisplayCandidateCount,
            MentionsShowCaseLangResourceKind.ApiPropertyIsReadOnly            => en_US.ApiPropertyIsReadOnly,
            MentionsShowCaseLangResourceKind.ApiPropertyAsyncLoadDebounce     => en_US.ApiPropertyAsyncLoadDebounce,
            MentionsShowCaseLangResourceKind.ApiPropertyAsyncLoadTimeout      => en_US.ApiPropertyAsyncLoadTimeout,
            MentionsShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup => en_US.ApiPropertyShouldUseOverlayPopup,
            MentionsShowCaseLangResourceKind.TokenNamePopupContentPadding     => en_US.TokenNamePopupContentPadding,
            MentionsShowCaseLangResourceKind.TokenNameOptionHeight            => en_US.TokenNameOptionHeight,
            MentionsShowCaseLangResourceKind.TokenNameMinPopupWidth           => en_US.TokenNameMinPopupWidth,
            MentionsShowCaseLangResourceKind.TokenScopeComponent              => en_US.TokenScopeComponent,
            MentionsShowCaseLangResourceKind.TokenStatusStable                => en_US.TokenStatusStable,
            _                                                                 => kind.ToString()
        };
    }
}

public sealed record MentionsApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record MentionsDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);

public class MentionOptionsAsyncLoader : IMentionOptionsAsyncLoader
{
    public async Task<MentionOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(600), token);
        List<IMentionOption>? options = null;
        if (!token.IsCancellationRequested)
        {
            var count = Random.Shared.Next(3, 8);
            var names = RandomUsernameGenerator.GenerateBatch(count);
            options = new List<IMentionOption>();
            foreach (var name in names)
            {
                options.Add(new MentionOption()
                {
                    Header = name,
                    Value  = name,
                });
            }
        }

        return new MentionOptionsLoadResult()
        {
            StatusCode = RpcStatusCode.Success,
            Data       = options
        };
    }
}

public class RandomUsernameGenerator
{
    private static readonly List<string> FirstNames =
    [
        "Alex", "Jordan", "Casey", "Riley", "Taylor", "Morgan", "Cameron",
        "Skye", "Dakota", "Phoenix", "Rowan", "Peyton", "Quinn", "Blake"
    ];

    private static readonly List<string> LastNames =
    [
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller",
        "Davis", "Rodriguez", "Martinez", "Lee", "Walker", "Hall", "Allen"
    ];

    private static readonly List<string> Adjectives =
    [
        "Swift", "Clever", "Brave", "Silent", "Lucky", "Fierce", "Gentle",
        "Witty", "Noble", "Calm", "Radiant", "Vivid", "Solar", "Lunar"
    ];

    private static readonly List<string> Nouns =
    [
        "Wolf", "Eagle", "Fox", "Lion", "Tiger", "Bear", "Hawk", "Falcon",
        "Storm", "River", "Stone", "Mountain", "Shadow", "Flame"
    ];

    private static readonly Random _random = new();

    public enum GenerationStyle
    {
        Simple,
        WithNumber,
        Creative,
        FullName,
        DotSeparated
    }

    public static string Generate(GenerationStyle style = GenerationStyle.WithNumber)
    {
        string firstName = FirstNames[_random.Next(FirstNames.Count)];
        string lastName = LastNames[_random.Next(LastNames.Count)];
        string adjective = Adjectives[_random.Next(Adjectives.Count)];
        string noun = Nouns[_random.Next(Nouns.Count)];
        int randomNumber = _random.Next(10, 9999);

        return style switch
        {
            GenerationStyle.Simple       => $"{firstName}{lastName}",
            GenerationStyle.WithNumber   => $"{firstName}{lastName}{randomNumber}",
            GenerationStyle.Creative     => $"{adjective}{noun}{randomNumber}",
            GenerationStyle.FullName     => $"{firstName}.{lastName}{randomNumber}",
            GenerationStyle.DotSeparated => $"{firstName}.{lastName}",
            _                            => $"{firstName}{lastName}{randomNumber}"
        };
    }

    public static List<string> GenerateBatch(int count, GenerationStyle style = GenerationStyle.WithNumber)
    {
        var usernames = new HashSet<string>();
        while (usernames.Count < count)
        {
            usernames.Add(Generate(style));
        }

        return usernames.ToList();
    }
}
