using System.Threading;
using Avalonia;
using Avalonia.Headless;
using AtomUI;
using AtomUI.Desktop.Controls;

[assembly: AvaloniaTestApplication(typeof(AtomUI.Desktop.Controls.Tests.TestAppBuilder))]

namespace AtomUI.Desktop.Controls.Tests;

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

internal sealed class TestApplication : Application
{
    public override void Initialize()
    {
        this.UseAtomUI(builder => builder.UseDesktopControls());
    }
}
