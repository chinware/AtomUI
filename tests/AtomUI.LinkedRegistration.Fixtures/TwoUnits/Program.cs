using AtomUI.Desktop.Controls;
using AtomUI.LinkedRegistration.Fixtures;

return FixtureHost.Run(
    "TwoUnits",
    static builder => builder.UseDesktopControls(),
    typeof(AtomUI.Desktop.Controls.Button),
    typeof(AtomUI.Desktop.Controls.DatePicker));
