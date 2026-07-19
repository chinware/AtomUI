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

    public static readonly DiagnosticDescriptor ScopedResourceHostInvalidClassShape = new(
        AtomUIDiagnosticIds.ScopedResourceHostInvalidClassShape,
        "Scoped resource host target must be a non-generic partial class",
        "Type '{0}' must be a non-generic partial top-level class to use [GenerateScopedResourceHost]",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ScopedResourceHostRequiresAvaloniaObject = new(
        AtomUIDiagnosticIds.ScopedResourceHostRequiresAvaloniaObject,
        "Scoped resource host target must inherit AvaloniaObject",
        "Type '{0}' must inherit AvaloniaObject to use [GenerateScopedResourceHost]",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ScopedResourceHostRejectsVisualTarget = new(
        AtomUIDiagnosticIds.ScopedResourceHostRejectsVisualTarget,
        "Scoped resource host target must be non-visual",
        "Type '{0}' inherits a visual type and must not use [GenerateScopedResourceHost]",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ScopedResourceHostRejectsExistingResourceHost = new(
        AtomUIDiagnosticIds.ScopedResourceHostRejectsExistingResourceHost,
        "Scoped resource host target already implements resource host interfaces",
        "Type '{0}' already implements IResourceHost or IThemeVariantHost and must not use [GenerateScopedResourceHost]",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ControlTokenMissingId = new(
        AtomUIDiagnosticIds.ControlTokenMissingId,
        "Control design token requires an ID constant",
        "Control design token '{0}' must declare public const string ID",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ControlTokenInvalidId = new(
        AtomUIDiagnosticIds.ControlTokenInvalidId,
        "Control design token ID must be constant",
        "Control design token '{0}' must declare ID as public const string with a non-empty value",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ThemeAssetMissingIdentity = new(
        AtomUIDiagnosticIds.ThemeAssetMissingIdentity,
        "Theme asset requires a Control Token identity",
        "Theme asset '{0}' uses Token resources but does not declare ControlTokenScope.Identity",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ThemeAssetUnknownIdentity = new(
        AtomUIDiagnosticIds.ThemeAssetUnknownIdentity,
        "Theme asset identity is not registered",
        "Theme asset '{0}' declares unknown Control Token identity '{1}'",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ThemeAssetConflictingIdentity = new(
        AtomUIDiagnosticIds.ThemeAssetConflictingIdentity,
        "Theme asset contains conflicting identities",
        "Theme asset '{0}' declares more than one Control Token identity",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ThemeAssetControlTokenMismatch = new(
        AtomUIDiagnosticIds.ThemeAssetControlTokenMismatch,
        "Theme asset uses a different Control Token family",
        "Theme asset '{0}' declares identity '{1}' but uses Control Token resources for '{2}'",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ThemeAssetDuplicateUri = new(
        AtomUIDiagnosticIds.ThemeAssetDuplicateUri,
        "Theme asset URI is duplicated",
        "Theme asset URI '{0}' is produced by more than one AdditionalFile",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);
}
#pragma warning restore RS2008
