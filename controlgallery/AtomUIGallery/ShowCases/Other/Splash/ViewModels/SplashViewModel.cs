using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Media;
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

    private static string Lang(SplashShowCaseLangResourceKind kind)
    {
        return Application.Current is { } application
            ? global::AtomUI.ApplicationExtensions.GetLocalizer(application)?.Get(kind) ?? kind.ToString()
            : kind.ToString();
    }
}

public sealed record SplashLogoInfo(string Text, IBrush Background, IBrush Foreground);

public sealed record SplashFooterInfo(string Version, string Description);
