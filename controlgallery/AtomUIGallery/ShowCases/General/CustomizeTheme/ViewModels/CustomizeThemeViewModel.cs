using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Schema;
using AtomUI.Theme.TokenSystem;
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
    private ThemeConfig _runtimeThemeConfig;

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

    public ThemeConfig GreenThemeConfig { get; }
    public ThemeConfig RedThemeConfig { get; }
    public ThemeConfig PurpleThemeConfig { get; }
    public ThemeConfig DarkThemeConfig { get; }
    public ThemeConfig ControlAlgorithmEnabledConfig { get; }
    public ThemeConfig ControlAlgorithmDisabledConfig { get; }
    public ThemeConfig NestedBlueThemeConfig { get; }
    public ThemeConfig NestedGreenThemeConfig { get; }
    public ThemeConfig NestedOrangeThemeConfig { get; }
    public ThemeConfig RuntimeThemeConfig
    {
        get => _runtimeThemeConfig;
        private set => this.RaiseAndSetIfChanged(ref _runtimeThemeConfig, value);
    }

    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> UseRuntimePrimaryBlue { get; }
    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> UseRuntimePrimaryGreen { get; }
    public ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit> UseRuntimePrimaryMagenta { get; }

    public CustomizeThemeViewModel(IScreen screen)
    {
        Activator                  = new ViewModelActivator();
        HostScreen                 = screen;
        GreenThemeConfig = BuildGlobalConfig(
            "#00b96b",
            (nameof(DesignToken.BorderRadius), "2"),
            (nameof(DesignToken.ColorBgContainer), "#f6ffed"));
        RedThemeConfig = BuildGlobalConfig(
            "#ff0000",
            (nameof(DesignToken.BorderRadius), "0"));
        PurpleThemeConfig = BuildGlobalConfig("#7D3C98");
        DarkThemeConfig = new ThemeConfigBuilder().WithAlgorithms("Dark").Build();
        ControlAlgorithmEnabledConfig = BuildControlConfig(ControlAlgorithmMode.Global);
        ControlAlgorithmDisabledConfig = BuildControlConfig(ControlAlgorithmMode.Disabled);
        NestedBlueThemeConfig = BuildGlobalConfig("#1677ff");
        NestedGreenThemeConfig = BuildGlobalConfig("#00b96b");
        NestedOrangeThemeConfig = new ThemeConfigBuilder()
                                 .WithInherit(false)
                                 .WithToken(nameof(DesignToken.ColorPrimary), "#faad14")
                                 .Build();
        _runtimeThemeConfig = BuildGlobalConfig("#1677ff");
        UseRuntimePrimaryBlue      = ReactiveCommand.Create(() => SetRuntimePrimaryColor("#1677ff"));
        UseRuntimePrimaryGreen     = ReactiveCommand.Create(() => SetRuntimePrimaryColor("#00b96b"));
        UseRuntimePrimaryMagenta   = ReactiveCommand.Create(() => SetRuntimePrimaryColor("#eb2f96"));
    }

    private void SetRuntimePrimaryColor(string color)
    {
        RuntimeThemeConfig = BuildGlobalConfig(color);
    }

    private static ThemeConfig BuildGlobalConfig(
        string primaryColor,
        params (string Name, string Value)[] additionalTokens)
    {
        var builder = new ThemeConfigBuilder()
                      .WithAlgorithms("Default")
                      .WithToken(nameof(DesignToken.ColorPrimary), primaryColor);
        foreach (var token in additionalTokens)
        {
            builder.WithToken(token.Name, token.Value);
        }
        return builder.Build();
    }

    private static ThemeConfig BuildControlConfig(ControlAlgorithmMode algorithm)
    {
        return new ThemeConfigBuilder()
               .WithControl(
                   new ControlTokenIdentity("AtomUI", "Button"),
                   new ControlThemeConfigBuilder()
                       .WithAlgorithm(algorithm)
                       .WithToken(nameof(DesignToken.ColorPrimary), "#00b96b")
                       .Build())
               .WithControl(
                   new ControlTokenIdentity("AtomUI", "AddOnDecoratedBox"),
                   new ControlThemeConfigBuilder()
                       .WithAlgorithm(algorithm)
                       .WithToken(nameof(DesignToken.ColorPrimary), "#eb2f96")
                       .Build())
               .Build();
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new CustomizeThemeApiRow("ThemeConfigProvider.Config", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigProviderConfig), "ThemeConfig?", "blue", "null"),
            new CustomizeThemeApiRow("ThemeConfig.Inherit", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigInherit), "bool", "purple", "true"),
            new CustomizeThemeApiRow("ThemeConfig.Algorithms", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigAlgorithms), "IReadOnlyList<string>?", "cyan", "null"),
            new CustomizeThemeApiRow("ThemeConfig.Tokens", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigTokens), "IReadOnlyDictionary<string, string>", "cyan", "{}"),
            new CustomizeThemeApiRow("ThemeConfig.Controls", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigControls), "IReadOnlyDictionary<ControlTokenIdentity, ControlThemeConfig>", "cyan", "{}"),
            new CustomizeThemeApiRow("ControlThemeConfig.Algorithm", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberControlThemeConfigAlgorithm), "ControlAlgorithmMode", "purple", "Unspecified"),
            new CustomizeThemeApiRow("ControlThemeConfig.Algorithms", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberControlThemeConfigAlgorithms), "IReadOnlyList<string>?", "cyan", "null"),
            new CustomizeThemeApiRow("ControlThemeConfig.Tokens", Lang(CustomizeThemeShowCaseLangResourceKind.ApiMemberControlThemeConfigTokens), "IReadOnlyDictionary<string, string>", "cyan", "{}")
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
            CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigProviderConfig          => en_US.ApiMemberThemeConfigProviderConfig,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigInherit                  => en_US.ApiMemberThemeConfigInherit,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigAlgorithms               => en_US.ApiMemberThemeConfigAlgorithms,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigTokens                   => en_US.ApiMemberThemeConfigTokens,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberThemeConfigControls                 => en_US.ApiMemberThemeConfigControls,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberControlThemeConfigAlgorithm         => en_US.ApiMemberControlThemeConfigAlgorithm,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberControlThemeConfigAlgorithms        => en_US.ApiMemberControlThemeConfigAlgorithms,
            CustomizeThemeShowCaseLangResourceKind.ApiMemberControlThemeConfigTokens            => en_US.ApiMemberControlThemeConfigTokens,
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
