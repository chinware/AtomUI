namespace AtomUI.Generator.LinkedRegistration;

internal static class LinkedRegistrationProtocol
{
    internal const int ProtocolMajorVersion = 1;
    internal const string MetadataPrefix = "AtomUI.Linked.";
    internal const string PackageManifestKey = MetadataPrefix + "Package.v1";
    internal const string UnitManifestKey = MetadataPrefix + "Unit.v1";
    internal const string ControlMapManifestKey = MetadataPrefix + "ControlMap.v1";
    internal const string UnitEdgeManifestKey = MetadataPrefix + "UnitEdge.v1";
    internal const string RootUnitManifestKey = MetadataPrefix + "RootUnit.v1";
    internal const string UsageManifestKey = MetadataPrefix + "Usage.v1";
    internal const string FallbackManifestKey = MetadataPrefix + "Fallback.v1";
    internal const string PlanMarkerKey = MetadataPrefix + "Plan.v1";

    internal const string FallbackReasonAnalysisBudgetExceeded = "AnalysisBudgetExceeded";
    internal const string FallbackReasonDynamicInvocation = "DynamicInvocation";
    internal const string FallbackReasonUnresolvedOwner = "UnresolvedOwner";
    // Sidecar extracted by the consumer from assembly metadata of a normally-built
    // ProjectReference: package/unit manifests are complete, but C# unit edges are only
    // computed by linked library builds. The plan must widen to full fallback for these
    // packages, and the reason is never surfaced as a diagnostic.
    internal const string FallbackReasonExtractedManifest = "ExtractedManifest";
    // Sidecar reconstructed from the IL of an ordinary precompiled consumer assembly.
    // The entry call is recoverable, but its complete control usage is not, so the plan
    // conservatively keeps the referenced package without surfacing a code warning.
    internal const string FallbackReasonExtractedConsumerAssembly = "ExtractedConsumerAssembly";
}
