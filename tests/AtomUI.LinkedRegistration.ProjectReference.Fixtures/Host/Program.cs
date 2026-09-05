using Avalonia;
using Avalonia.Headless;
using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUI.LinkedRegistration.ProjectReference.Fixtures.Shared;

AppBuilder.Configure<FixtureApplication>()
          .UseHeadless(new AvaloniaHeadlessPlatformOptions())
          .SetupWithoutStarting();
GC.KeepAlive(typeof(Button));

Console.WriteLine("ProjectReference registration succeeded.");
return 0;

internal sealed partial class FixtureApplication : Application
{
    public override void Initialize()
    {
        this.UseAtomUI(builder => SharedRegistration.Register(builder));
    }
}
