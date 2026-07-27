using System.Reactive.Disposables;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Styling;

namespace AtomUI.Toolkits.GalleryBase.Shell;

internal interface IGallerySystemAppearanceSource
{
    ThemeAppearance GetCurrentAppearance();

    IDisposable Subscribe(Action<ThemeAppearance> handler);
}

internal sealed class GallerySystemAppearanceSource : IGallerySystemAppearanceSource
{
    public static GallerySystemAppearanceSource Instance { get; } = new();

    private GallerySystemAppearanceSource()
    {
    }

    public ThemeAppearance GetCurrentAppearance()
    {
        var application = Application.Current;
        if (application?.PlatformSettings is { } settings)
        {
            return ToThemeAppearance(settings.GetColorValues().ThemeVariant);
        }

        return application?.ActualThemeVariant == ThemeVariant.Dark
            ? ThemeAppearance.Dark
            : ThemeAppearance.Light;
    }

    public IDisposable Subscribe(Action<ThemeAppearance> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        var settings = Application.Current?.PlatformSettings;
        if (settings is null)
        {
            return Disposable.Empty;
        }

        void HandleColorValuesChanged(object? sender, PlatformColorValues values)
        {
            handler(ToThemeAppearance(values.ThemeVariant));
        }

        settings.ColorValuesChanged += HandleColorValuesChanged;
        return Disposable.Create(() => settings.ColorValuesChanged -= HandleColorValuesChanged);
    }

    private static ThemeAppearance ToThemeAppearance(PlatformThemeVariant platformTheme)
    {
        return platformTheme == PlatformThemeVariant.Dark
            ? ThemeAppearance.Dark
            : ThemeAppearance.Light;
    }
}
