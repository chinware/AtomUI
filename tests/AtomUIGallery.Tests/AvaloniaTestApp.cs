using Avalonia;
using Avalonia.Headless;
using AtomUI;
using AtomUI.Toolkits.GalleryBase;
using AtomUI.Desktop.Controls;
using AtomUI.Localization;
using AtomUI.Toolkits.GalleryBase;
using Xunit;

[assembly: AvaloniaTestApplication(typeof(AtomUIGallery.Tests.TestAppBuilder))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AtomUIGallery.Tests;

internal static class AvaloniaTestApp
{
    private static int _initialized;

    public static void EnsureInitialized()
    {
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

internal sealed partial class TestApplication : Application
{
    public override void Initialize()
    {
        this.UseAtomUI(builder =>
        {
            builder.UseDesktopControls();
            builder.UseDesktopExtras();
            builder.UseDesktopColorPicker();
            builder.UseDesktopDataGrid();
            builder.UseGalleryBase(global::AtomUIGallery.AtomUIGalleryModule.Configure);
            builder.UseGalleryControls();
 builder.UseGalleryBase(AtomUIGalleryModule.Configure);
            builder.UseLanguages(
                LanguageTags.EnUS,
                [LanguageTags.EnUS, LanguageTags.ZhCN, LanguageTags.ZhTW, LanguageTags.PtBR]);
        });
    }
}
