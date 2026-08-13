using AtomUI.Desktop.Controls;
using AtomUI.LinkedRegistration.Fixtures;

return FixtureHost.Run(
    "DynamicFallback",
    static builder => builder.UseDesktopControls());
