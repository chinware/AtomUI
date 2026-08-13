using System.Reflection;
using AtomUI.Registration;

[assembly: AssemblyMetadata(
    "AtomUI.Linked.Unit.v1",
    "1|AtomUI.Desktop.Controls|AtomUI.Desktop.Controls%2FFixtureUnused|AtomUI.LinkedRegistration.Fixtures.UnusedUnitFragment|Add")]

namespace AtomUI.LinkedRegistration.Fixtures;

public static class UnusedUnitFragment
{
    public static void Add(AotTrimControlPackageRegistrationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.TryEnterUnit("AtomUI.Desktop.Controls/FixtureUnused");
    }
}
