using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.CustomizeTheme;

public class CustomizeThemeViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "CustomizeTheme";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<CustomizeThemeApiRow>? _apiRows;
    private ObservableCollection<CustomizeThemeDesignTokenRow>? _designTokenRows;

    public ObservableCollection<CustomizeThemeApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<CustomizeThemeDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public CustomizeThemeViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
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
            new CustomizeThemeApiRow("ThemeConfigProvider.Algorithms", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigProviderAlgorithms), "List<string>", "blue", "[]"),
            new CustomizeThemeApiRow("ThemeConfigProvider.SharedTokenSetters", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigProviderSharedTokenSetters), "List<TokenSetter>", "cyan", "[]"),
            new CustomizeThemeApiRow("ThemeConfigProvider.ControlTokenInfoSetters", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigProviderControlTokenInfoSetters), "List<ControlTokenInfoSetter>", "cyan", "[]"),
            new CustomizeThemeApiRow("TokenSetter.Key", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberTokenSetterKey), "string", "purple", "string.Empty"),
            new CustomizeThemeApiRow("TokenSetter.Value", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberTokenSetterValue), "string", "purple", "string.Empty"),
            new CustomizeThemeApiRow("TokenSetter.Catalog", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberTokenSetterCatalog), "string?", "purple", "null"),
            new CustomizeThemeApiRow("ControlTokenInfoSetter.TokenId", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberControlTokenInfoSetter), "string", "purple", "string.Empty"),
            new CustomizeThemeApiRow("ControlTokenInfoSetter.EnableAlgorithm", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberControlTokenInfoSetterEnableAlgorithm), "bool", "purple", "false"),
            new CustomizeThemeApiRow("ControlTokenInfoSetter.Setters", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberControlTokenInfoSetterSetters), "List<TokenSetter>", "cyan", "[]")
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
            new CustomizeThemeDesignTokenRow("ColorPrimary", Lang(CustomizeThemeShowCaseLangResourceKind.TokenNameColorPrimary), Lang(CustomizeThemeShowCaseLangResourceKind.TokenScopeShared), "cyan", Lang(CustomizeThemeShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CustomizeThemeDesignTokenRow("BorderRadius", Lang(CustomizeThemeShowCaseLangResourceKind.TokenNameBorderRadius), Lang(CustomizeThemeShowCaseLangResourceKind.TokenScopeShared), "cyan", Lang(CustomizeThemeShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CustomizeThemeDesignTokenRow("ColorBgContainer", Lang(CustomizeThemeShowCaseLangResourceKind.TokenNameColorBgContainer), Lang(CustomizeThemeShowCaseLangResourceKind.TokenScopeShared), "cyan", Lang(CustomizeThemeShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CustomizeThemeDesignTokenRow("Button.ColorPrimary", Lang(CustomizeThemeShowCaseLangResourceKind.TokenNameButtonColorPrimary), Lang(CustomizeThemeShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(CustomizeThemeShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CustomizeThemeDesignTokenRow("AddOnDecoratedBox.ColorPrimary", Lang(CustomizeThemeShowCaseLangResourceKind.TokenNameAddOnDecoratedBoxColorPrimary), Lang(CustomizeThemeShowCaseLangResourceKind.TokenScopeComponent), "blue", Lang(CustomizeThemeShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(CustomizeThemeShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(CustomizeThemeShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigProviderAlgorithms              => en_US.ApiMemberThemeConfigProviderAlgorithms,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigProviderSharedTokenSetters      => en_US.ApiMemberThemeConfigProviderSharedTokenSetters,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigProviderControlTokenInfoSetters => en_US.ApiMemberThemeConfigProviderControlTokenInfoSetters,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberTokenSetterKey                             => en_US.ApiMemberTokenSetterKey,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberTokenSetterValue                           => en_US.ApiMemberTokenSetterValue,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberTokenSetterCatalog                         => en_US.ApiMemberTokenSetterCatalog,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberControlTokenInfoSetter                     => en_US.ApiMemberControlTokenInfoSetter,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberControlTokenInfoSetterEnableAlgorithm      => en_US.ApiMemberControlTokenInfoSetterEnableAlgorithm,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberControlTokenInfoSetterSetters              => en_US.ApiMemberControlTokenInfoSetterSetters,
            CustomizeThemeShowCaseLangResourceKind.TokenNameColorPrimary                               => en_US.TokenNameColorPrimary,
            CustomizeThemeShowCaseLangResourceKind.TokenNameBorderRadius                               => en_US.TokenNameBorderRadius,
            CustomizeThemeShowCaseLangResourceKind.TokenNameColorBgContainer                           => en_US.TokenNameColorBgContainer,
            CustomizeThemeShowCaseLangResourceKind.TokenNameButtonColorPrimary                         => en_US.TokenNameButtonColorPrimary,
            CustomizeThemeShowCaseLangResourceKind.TokenNameAddOnDecoratedBoxColorPrimary              => en_US.TokenNameAddOnDecoratedBoxColorPrimary,
            CustomizeThemeShowCaseLangResourceKind.TokenScopeShared                                    => en_US.TokenScopeShared,
            CustomizeThemeShowCaseLangResourceKind.TokenScopeComponent                                 => en_US.TokenScopeComponent,
            CustomizeThemeShowCaseLangResourceKind.TokenStatusStable                                   => en_US.TokenStatusStable,
            _                                                                                          => kind.ToString()
        };
    }
}

public sealed record CustomizeThemeApiRow(
    string Member,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record CustomizeThemeDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
