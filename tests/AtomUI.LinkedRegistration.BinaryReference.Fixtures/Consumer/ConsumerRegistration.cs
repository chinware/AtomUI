using AtomUI.Desktop.Controls;

namespace AtomUI.LinkedRegistration.BinaryReference.Fixtures.Consumer;

public static class ConsumerRegistration
{
    public static IAtomUIBuilder Register(IAtomUIBuilder builder)
    {
        return builder.UseDesktopControls();
    }

    public static Button CreateButton() => new();
}
