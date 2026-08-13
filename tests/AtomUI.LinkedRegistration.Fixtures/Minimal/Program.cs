using AtomUI.Desktop.Controls;
using AtomUI.LinkedRegistration.Fixtures;

return FixtureHost.Run(
    "Minimal",
    static builder => builder.UseDesktopControls(),
    typeof(AtomUI.Desktop.Controls.Button));
