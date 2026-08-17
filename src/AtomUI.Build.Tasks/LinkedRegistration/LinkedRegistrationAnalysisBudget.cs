namespace AtomUI.LinkedRegistration.Protocol;

internal static class LinkedRegistrationAnalysisBudget
{
    internal const int MaxSourceCandidates = 50_000;
    internal const int MaxPackages = 1_024;
    internal const int MaxRegistrationUnits = 2_048;
    internal const int MaxUnitEdges = 10_000;
    internal const int MaxRootUnits = 2_048;
    internal const int MaxControlMappings = 50_000;
    internal const int MaxUsages = 50_000;
    internal const int MaxFallbacks = 4_096;
    internal const int MaxManifestRecords = 64_000;
    internal const int MaxSidecarBytes = 4 * 1024 * 1024;
    internal const int MaxTotalSidecarBytes = 32 * 1024 * 1024;
    internal const int MaxAxamlUsageBytes = 4 * 1024 * 1024;
    internal const int MaxAxamlCandidates = 50_000;
}
