using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUI.LinkedRegistration.BinaryReference.Fixtures.Consumer;
using Avalonia;
using Avalonia.Headless;

AppBuilder.Configure<FixtureApplication>()
          .UseHeadless(new AvaloniaHeadlessPlatformOptions())
          .SetupWithoutStarting();
GC.KeepAlive(typeof(Button));

Console.WriteLine("Binary reference registration succeeded.");
return 0;

internal sealed partial class FixtureApplication : Application
{
    public override void Initialize()
    {
        this.UseAtomUI(builder => ConsumerRegistration.Register(builder));
    }
}
