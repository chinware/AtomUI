using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Empty;

public class EmptyViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Empty";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<EmptyApiRow>? _apiRows;
    private ObservableCollection<EmptyDesignTokenRow>? _designTokenRows;

    public ObservableCollection<EmptyApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<EmptyDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public EmptyViewModel(IScreen screen)
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
            new EmptyApiRow("PresetImage", Lang(EmptyShowCaseLangResourceKind.ApiPropertyPresetImage), "PresetEmptyImage?", "blue", "null"),
            new EmptyApiRow("ImagePath", Lang(EmptyShowCaseLangResourceKind.ApiPropertyImagePath), "string?", "cyan", "null"),
            new EmptyApiRow("ImageSource", Lang(EmptyShowCaseLangResourceKind.ApiPropertyImageSource), "string?", "cyan", "null"),
            new EmptyApiRow("Description", Lang(EmptyShowCaseLangResourceKind.ApiPropertyDescription), "string?", "cyan", "NoData"),
            new EmptyApiRow("SizeType", Lang(EmptyShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new EmptyApiRow("IsDescriptionVisible", Lang(EmptyShowCaseLangResourceKind.ApiPropertyIsDescriptionVisible), "bool", "green", "true")
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
            new EmptyDesignTokenRow("EmptyImgHeight", Lang(EmptyShowCaseLangResourceKind.TokenNameEmptyImgHeight), Lang(EmptyShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(EmptyShowCaseLangResourceKind.TokenStatusStable), "success"),
            new EmptyDesignTokenRow("EmptyImgHeightMD", Lang(EmptyShowCaseLangResourceKind.TokenNameEmptyImgHeightMD), Lang(EmptyShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(EmptyShowCaseLangResourceKind.TokenStatusStable), "success"),
            new EmptyDesignTokenRow("EmptyImgHeightSM", Lang(EmptyShowCaseLangResourceKind.TokenNameEmptyImgHeightSM), Lang(EmptyShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(EmptyShowCaseLangResourceKind.TokenStatusStable), "success"),
            new EmptyDesignTokenRow("DescriptionMargin", Lang(EmptyShowCaseLangResourceKind.TokenNameDescriptionMargin), Lang(EmptyShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(EmptyShowCaseLangResourceKind.TokenStatusStable), "success"),
            new EmptyDesignTokenRow("DescriptionMarginSM", Lang(EmptyShowCaseLangResourceKind.TokenNameDescriptionMarginSM), Lang(EmptyShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(EmptyShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(EmptyShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(EmptyShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            EmptyShowCaseLangResourceKind.ApiPropertyPresetImage          => en_US.ApiPropertyPresetImage,
            EmptyShowCaseLangResourceKind.ApiPropertyImagePath            => en_US.ApiPropertyImagePath,
            EmptyShowCaseLangResourceKind.ApiPropertyImageSource          => en_US.ApiPropertyImageSource,
            EmptyShowCaseLangResourceKind.ApiPropertyDescription          => en_US.ApiPropertyDescription,
            EmptyShowCaseLangResourceKind.ApiPropertySizeType             => en_US.ApiPropertySizeType,
            EmptyShowCaseLangResourceKind.ApiPropertyIsDescriptionVisible => en_US.ApiPropertyIsDescriptionVisible,
            EmptyShowCaseLangResourceKind.TokenNameEmptyImgHeight         => en_US.TokenNameEmptyImgHeight,
            EmptyShowCaseLangResourceKind.TokenNameEmptyImgHeightMD       => en_US.TokenNameEmptyImgHeightMD,
            EmptyShowCaseLangResourceKind.TokenNameEmptyImgHeightSM       => en_US.TokenNameEmptyImgHeightSM,
            EmptyShowCaseLangResourceKind.TokenNameDescriptionMargin      => en_US.TokenNameDescriptionMargin,
            EmptyShowCaseLangResourceKind.TokenNameDescriptionMarginSM    => en_US.TokenNameDescriptionMarginSM,
            EmptyShowCaseLangResourceKind.TokenScopeComponent             => en_US.TokenScopeComponent,
            EmptyShowCaseLangResourceKind.TokenStatusStable               => en_US.TokenStatusStable,
            _                                                             => kind.ToString()
        };
    }
}

public sealed record EmptyApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record EmptyDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
