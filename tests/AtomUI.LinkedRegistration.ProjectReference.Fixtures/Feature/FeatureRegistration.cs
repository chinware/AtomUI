using AtomUI.Desktop.Controls;

namespace AtomUI.LinkedRegistration.ProjectReference.Fixtures.Feature;

public static class FeatureRegistration
{
    public static IAtomUIBuilder Register(IAtomUIBuilder builder)
    {
        return builder.UseDesktopControls();
    }
}
