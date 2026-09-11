using System.ComponentModel;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;

namespace AtomUI.Registration;

[EditorBrowsable(EditorBrowsableState.Never)]
public delegate bool AotTrimRegistrationPlan(
    IAtomUIBuilder builder,
    string packageId,
    IControlThemesProvider provider,
    Func<ControlTokenIdentity, bool>? includeIdentity,
    Func<IReadOnlyList<ControlThemeAssetDescriptor>,
        IReadOnlyList<ControlThemeAssetDescriptor>>? selectAssets);

[EditorBrowsable(EditorBrowsableState.Never)]
public static class AotTrimRegistrationPlanRegistry
{
    private static RegistrationPlanEntry? s_entry;

    public static void Install(string ownerId, AotTrimRegistrationPlan plan)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerId);
        ArgumentNullException.ThrowIfNull(plan);

        var entry = new RegistrationPlanEntry(ownerId, plan);
        var existing = Interlocked.CompareExchange(ref s_entry, entry, null);
        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"AOT/Trim registration plan '{existing.OwnerId}' is already installed; " +
                $"plan '{ownerId}' cannot be installed in the same process.");
        }
    }

    public static IAtomUIBuilder ApplyPackage(
        IAtomUIBuilder builder,
        string packageId,
        IControlThemesProvider provider,
        Func<ControlTokenIdentity, bool>? includeIdentity = null,
        Func<IReadOnlyList<ControlThemeAssetDescriptor>,
            IReadOnlyList<ControlThemeAssetDescriptor>>? selectAssets = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrWhiteSpace(packageId);
        ArgumentNullException.ThrowIfNull(provider);

        var entry = Volatile.Read(ref s_entry);
        if (entry is null)
        {
            throw CreateMissingPackageException(packageId, "No application registration plan is installed");
        }

        if (!entry.Plan(builder, packageId, provider, includeIdentity, selectAssets))
        {
            throw CreateMissingPackageException(
                packageId,
                $"Application registration plan '{entry.OwnerId}' does not contain the package");
        }

        return builder;
    }

    internal static void ResetForTests()
    {
        Volatile.Write(ref s_entry, null);
    }

    private static InvalidOperationException CreateMissingPackageException(
        string packageId,
        string reason)
    {
        return new InvalidOperationException(
            $"{reason} '{packageId}'. Ensure the package's UseXxxControls() registration entry is visible " +
            "from the application or a ProjectReference rebuilt with the linked publish graph. " +
            "For a precompiled library, rebuild it with the current publish graph or upgrade to a version " +
            "that carries linked-registration evidence. Only for genuinely compile-time unknown dynamic " +
            $"control usage, add <AtomUIPackageRoot Include=\"{packageId}\" /> to the application project.");
    }

    private sealed record RegistrationPlanEntry(
        string OwnerId,
        AotTrimRegistrationPlan Plan);
}
