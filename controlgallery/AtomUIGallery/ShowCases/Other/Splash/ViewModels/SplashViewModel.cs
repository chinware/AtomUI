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
            _                                                             => kind.ToString()
        };
    }
}

public sealed record SplashLogoInfo(string Text, IBrush Background, IBrush Foreground);

public sealed record SplashFooterInfo(string Version, string Description);
