using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.Diagnostics;

#pragma warning disable RS2008
internal static class AtomUIDiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor AotMissingGeneratedAccessor = new(
        AtomUIDiagnosticIds.AotMissingGeneratedAccessor,
        "AOT-sensitive data member path requires generated accessor",
        "AOT-sensitive data member path '{0}' on '{1}' requires [GenerateDataMemberAccessors] or an IDataMemberAccessorDescriptor",
        AtomUIDiagnosticCategories.Aot,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor AotMissingGeneratedPath = new(
        AtomUIDiagnosticIds.AotMissingGeneratedPath,
        "AOT-sensitive data member path is not generated",
        "AOT-sensitive data member path '{0}' on '{1}' is not included in generated accessors",
        AtomUIDiagnosticCategories.Aot,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor AotUnverifiableDataMemberPath = new(
        AtomUIDiagnosticIds.AotUnverifiableDataMemberPath,
        "AOT-sensitive data member path cannot be verified",
        "AOT-sensitive data member path '{0}' cannot be verified at compile time; use nameof(Type.Property) or an IDataMemberAccessorDescriptor",
        AtomUIDiagnosticCategories.Aot,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);
}
#pragma warning restore RS2008
