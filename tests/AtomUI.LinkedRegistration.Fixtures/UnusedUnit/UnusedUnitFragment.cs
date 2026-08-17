using System.Reflection;
using AtomUI;
using AtomUI.Registration;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;

[assembly: AssemblyMetadata(
    "AtomUI.Linked.Package.v1",
    "1|AtomUI.LinkedRegistration.Fixtures.UnusedUnit|AtomUI.LinkedRegistration.Fixtures.UnusedUnit|Package|AtomUI.LinkedRegistration.Fixtures.UnusedUnitRegistration.UseUnusedUnit|AtomUI.LinkedRegistration.Fixtures.UnusedUnitRegistration|Register||")]

[assembly: AssemblyMetadata(
    "AtomUI.Linked.Unit.v1",
    "1|AtomUI.LinkedRegistration.Fixtures.UnusedUnit|AtomUI.LinkedRegistration.Fixtures.UnusedUnit%2FFixtureUnused|AtomUI.LinkedRegistration.Fixtures.UnusedUnitFragment|Add")]

namespace AtomUI.LinkedRegistration.Fixtures;

public static class UnusedUnitRegistration
{
    [ControlPackageRegistrationEntry]
    public static IAtomUIBuilder UseUnusedUnit(this IAtomUIBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder;
    }

    public static void Register(
        IThemeManagerBuilder themeManagerBuilder,
        IControlThemesProvider controlThemesProvider,
        Func<ControlTokenIdentity, bool>? includeIdentity = null,
        Func<IReadOnlyList<ControlThemeAssetDescriptor>,
            IReadOnlyList<ControlThemeAssetDescriptor>>? selectAssets = null)
    {
        ArgumentNullException.ThrowIfNull(themeManagerBuilder);
        ArgumentNullException.ThrowIfNull(controlThemesProvider);
    }
}

public static class UnusedUnitFragment
{
    public static void Add(AotTrimControlPackageRegistrationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
    }
}
