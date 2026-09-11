using AtomUI.LinkedRegistration.ProjectReference.Fixtures.Feature;

namespace AtomUI.LinkedRegistration.ProjectReference.Fixtures.Shared;

public static class SharedRegistration
{
    public static IAtomUIBuilder Register(IAtomUIBuilder builder)
    {
        return FeatureRegistration.Register(builder);
    }
}
