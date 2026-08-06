using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using AtomUI.Localization;

[assembly: AvaloniaTestApplication(typeof(AtomUI.Desktop.Controls.Tests.TestAppBuilder))]
[assembly: AvaloniaTestFramework]
[assembly: AvaloniaTestIsolation(AvaloniaTestIsolationLevel.PerTest)]

namespace AtomUI.Desktop.Controls.Tests;

internal static class AvaloniaTestApp
{
    private static int _initialized;

    public static void EnsureInitialized()
    {
        if (Application.Current is not null)
        {
            Volatile.Write(ref _initialized, 1);
            return;
        }

        if (Interlocked.Exchange(ref _initialized, 1) == 1)
        {
            return;
        }

        TestAppBuilder.BuildAvaloniaApp().SetupWithoutStarting();
    }
}

public static class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<TestApplication>()
                         .UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}

internal sealed class TestApplication : Application
{
    public override void Initialize()
    {
        this.UseAtomUI(builder => builder.UseDesktopControls()
                                         .UseDesktopExtras()
                                         .UseLanguages(
                                             LanguageTags.EnUS,
                                             [LanguageTags.EnUS, LanguageTags.ZhCN, LanguageTags.ZhTW]));
    }
}
