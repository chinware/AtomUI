using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Splash;

public class SplashViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Splash";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    public SplashLogoInfo BasicLogo { get; }
    public SplashLogoInfo ComposedLogo { get; }
    public SplashFooterInfo ComposedFooter { get; }
    public double ProgressValue { get; } = 0.68d;

    private ObservableCollection<SplashApiRow>? _apiRows;
    private ObservableCollection<SplashDesignTokenRow>? _designTokenRows;

    public ObservableCollection<SplashApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SplashDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public SplashViewModel(IScreen screen)
    {
        HostScreen = screen;
        BasicLogo = new SplashLogoInfo(
            "A",
            new SolidColorBrush(Color.Parse("#1677FF")),
            Brushes.White);
        ComposedLogo = new SplashLogoInfo(
            "UI",
            new SolidColorBrush(Color.Parse("#722ED1")),
            Brushes.White);
        ComposedFooter = new SplashFooterInfo("v6", Lang(SplashShowCaseLangResourceKind.P2FooterDesktopOnly));
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new SplashApiRow("Splash.Logo", Lang(SplashShowCaseLangResourceKind.ApiPropertyLogo), "object?", "cyan", "null"),
            new SplashApiRow("Splash.LogoTemplate", Lang(SplashShowCaseLangResourceKind.ApiPropertyLogoTemplate), "IDataTemplate?", "cyan", "null"),
            new SplashApiRow("Splash.Title", Lang(SplashShowCaseLangResourceKind.ApiPropertyTitle), "string?", "cyan", "null"),
            new SplashApiRow("Splash.Subtitle", Lang(SplashShowCaseLangResourceKind.ApiPropertySubtitle), "string?", "cyan", "null"),
            new SplashApiRow("Splash.Message", Lang(SplashShowCaseLangResourceKind.ApiPropertyMessage), "string?", "cyan", "null"),
            new SplashApiRow("Splash.Detail", Lang(SplashShowCaseLangResourceKind.ApiPropertyDetail), "string?", "cyan", "null"),
            new SplashApiRow("Splash.Progress", Lang(SplashShowCaseLangResourceKind.ApiPropertyProgress), "double?", "green", "null"),
            new SplashApiRow("Splash.IsIndeterminate", Lang(SplashShowCaseLangResourceKind.ApiPropertyIsIndeterminate), "bool", "purple", "true"),
            new SplashApiRow("Splash.Status", Lang(SplashShowCaseLangResourceKind.ApiPropertyStatus), "SplashStatus", "purple", "Loading"),
            new SplashApiRow("Splash.IsMotionEnabled", Lang(SplashShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "true"),
            new SplashApiRow("Splash.Footer", Lang(SplashShowCaseLangResourceKind.ApiPropertyFooter), "object?", "cyan", "null"),
            new SplashApiRow("Splash.ShowAsync", Lang(SplashShowCaseLangResourceKind.ApiStaticShowAsync), "Task<Splash>", "geekblue", "static"),
            new SplashApiRow("ISplashService.ShowAsync", Lang(SplashShowCaseLangResourceKind.ApiServiceShowAsync), "Task<Splash>", "geekblue", "instance"),
            new SplashApiRow("Splash.SetProgress", Lang(SplashShowCaseLangResourceKind.ApiMethodSetProgress), "void", "geekblue", "-"),
            new SplashApiRow("SplashOptions.MinimumShowDuration", Lang(SplashShowCaseLangResourceKind.ApiOptionMinimumShowDuration), "TimeSpan", "green", "500ms")
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
            Token("WindowWidth", SplashShowCaseLangResourceKind.TokenNameWindowWidth),
            Token("WindowMinHeight", SplashShowCaseLangResourceKind.TokenNameWindowMinHeight),
            Token("SurfaceCornerRadius", SplashShowCaseLangResourceKind.TokenNameSurfaceCornerRadius),
            Token("SurfaceBoxShadow", SplashShowCaseLangResourceKind.TokenNameSurfaceBoxShadow),
            Token("SurfaceBackground", SplashShowCaseLangResourceKind.TokenNameSurfaceBackground),
            Token("ContentPadding", SplashShowCaseLangResourceKind.TokenNameContentPadding),
            Token("LogoSize", SplashShowCaseLangResourceKind.TokenNameLogoSize),
            Token("TitleFontSize", SplashShowCaseLangResourceKind.TokenNameTitleFontSize),
            Token("SubtitleFontSize", SplashShowCaseLangResourceKind.TokenNameSubtitleFontSize),
            Token("MessageFontSize", SplashShowCaseLangResourceKind.TokenNameMessageFontSize),
            Token("DetailFontSize", SplashShowCaseLangResourceKind.TokenNameDetailFontSize),
            Token("IndicatorSize", SplashShowCaseLangResourceKind.TokenNameIndicatorSize),
            Token("ProgressBarHeight", SplashShowCaseLangResourceKind.TokenNameProgressBarHeight),
            Token("SuccessColor", SplashShowCaseLangResourceKind.TokenNameSuccessColor),
            Token("ErrorColor", SplashShowCaseLangResourceKind.TokenNameErrorColor)
        ];
    }

    private static SplashDesignTokenRow Token(string token, SplashShowCaseLangResourceKind description)
    {
        return new SplashDesignTokenRow(
            token,
            Lang(description),
            Lang(SplashShowCaseLangResourceKind.TokenScopeComponent),
            "cyan",
            Lang(SplashShowCaseLangResourceKind.TokenStatusPreview),
            "processing");
    }

    private static string Lang(SplashShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SplashShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SplashShowCaseLangResourceKind.P2FooterDesktopOnly            => en_US.P2FooterDesktopOnly,
            SplashShowCaseLangResourceKind.ApiPropertyLogo                => en_US.ApiPropertyLogo,
            SplashShowCaseLangResourceKind.ApiPropertyLogoTemplate        => en_US.ApiPropertyLogoTemplate,
            SplashShowCaseLangResourceKind.ApiPropertyTitle               => en_US.ApiPropertyTitle,
            SplashShowCaseLangResourceKind.ApiPropertySubtitle            => en_US.ApiPropertySubtitle,
            SplashShowCaseLangResourceKind.ApiPropertyMessage             => en_US.ApiPropertyMessage,
            SplashShowCaseLangResourceKind.ApiPropertyDetail              => en_US.ApiPropertyDetail,
            SplashShowCaseLangResourceKind.ApiPropertyProgress            => en_US.ApiPropertyProgress,
            SplashShowCaseLangResourceKind.ApiPropertyIsIndeterminate     => en_US.ApiPropertyIsIndeterminate,
            SplashShowCaseLangResourceKind.ApiPropertyStatus              => en_US.ApiPropertyStatus,
            SplashShowCaseLangResourceKind.ApiPropertyIsMotionEnabled     => en_US.ApiPropertyIsMotionEnabled,
            SplashShowCaseLangResourceKind.ApiPropertyFooter              => en_US.ApiPropertyFooter,
            SplashShowCaseLangResourceKind.ApiStaticShowAsync             => en_US.ApiStaticShowAsync,
            SplashShowCaseLangResourceKind.ApiServiceShowAsync            => en_US.ApiServiceShowAsync,
            SplashShowCaseLangResourceKind.ApiMethodSetProgress           => en_US.ApiMethodSetProgress,
            SplashShowCaseLangResourceKind.ApiOptionMinimumShowDuration   => en_US.ApiOptionMinimumShowDuration,
            SplashShowCaseLangResourceKind.TokenNameWindowWidth           => en_US.TokenNameWindowWidth,
            SplashShowCaseLangResourceKind.TokenNameWindowMinHeight       => en_US.TokenNameWindowMinHeight,
            SplashShowCaseLangResourceKind.TokenNameSurfaceCornerRadius   => en_US.TokenNameSurfaceCornerRadius,
            SplashShowCaseLangResourceKind.TokenNameSurfaceBoxShadow      => en_US.TokenNameSurfaceBoxShadow,
            SplashShowCaseLangResourceKind.TokenNameSurfaceBackground     => en_US.TokenNameSurfaceBackground,
            SplashShowCaseLangResourceKind.TokenNameContentPadding        => en_US.TokenNameContentPadding,
            SplashShowCaseLangResourceKind.TokenNameLogoSize              => en_US.TokenNameLogoSize,
            SplashShowCaseLangResourceKind.TokenNameTitleFontSize         => en_US.TokenNameTitleFontSize,
            SplashShowCaseLangResourceKind.TokenNameSubtitleFontSize      => en_US.TokenNameSubtitleFontSize,
            SplashShowCaseLangResourceKind.TokenNameMessageFontSize       => en_US.TokenNameMessageFontSize,
            SplashShowCaseLangResourceKind.TokenNameDetailFontSize        => en_US.TokenNameDetailFontSize,
            SplashShowCaseLangResourceKind.TokenNameIndicatorSize         => en_US.TokenNameIndicatorSize,
            SplashShowCaseLangResourceKind.TokenNameProgressBarHeight     => en_US.TokenNameProgressBarHeight,
            SplashShowCaseLangResourceKind.TokenNameSuccessColor          => en_US.TokenNameSuccessColor,
            SplashShowCaseLangResourceKind.TokenNameErrorColor            => en_US.TokenNameErrorColor,
            SplashShowCaseLangResourceKind.TokenScopeComponent            => en_US.TokenScopeComponent,
            SplashShowCaseLangResourceKind.TokenStatusPreview             => en_US.TokenStatusPreview,
            _                                                             => kind.ToString()
        };
    }
}

public sealed record SplashLogoInfo(string Text, IBrush Background, IBrush Foreground);

public sealed record SplashFooterInfo(string Version, string Description);

public sealed record SplashApiRow(
    string Member,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SplashDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
