using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;

[assembly: AvaloniaTestApplication(typeof(AtomUI.Controls.Tests.TestAppBuilder))]
[assembly: AvaloniaTestFramework]
[assembly: AvaloniaTestIsolation(AvaloniaTestIsolationLevel.PerTest)]

namespace AtomUI.Controls.Tests;

internal static class AvaloniaTestApp
{
    private static int _initialized;

    internal static void EnsureInitialized()
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
        this.UseAtomUI(builder => builder.UseCommonControls());
    }
}
