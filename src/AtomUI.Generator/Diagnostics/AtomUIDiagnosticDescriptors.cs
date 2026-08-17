using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.Diagnostics;

#pragma warning disable RS2008
internal static class AtomUIDiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor LinkedPlanOwner = new(
        AtomUIDiagnosticIds.LinkedPlanOwner,
        "Linked registration requires one application plan owner",
        "Linked registration requires exactly one Application Plan owner; detected '{0}'",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedDynamicUsageWidened = new(
        AtomUIDiagnosticIds.LinkedDynamicUsageWidened,
        "Dynamic AtomUI usage requires package fallback",
        "AtomUI dynamic usage '{0}' cannot be resolved to a Registration Unit; Package '{1}' uses full fallback. Add an AtomUIRegistrationUnitRoot when the Unit is known, or an AtomUIPackageRoot for fully dynamic usage.",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedLegacyPackageFallback = new(
        AtomUIDiagnosticIds.LinkedLegacyPackageFallback,
        "Legacy package requires full linked-registration fallback",
        "Package '{0}' has no compatible linked manifest and requires full fallback. Add <AtomUIPackageRoot Include=\"{0}\" /> or upgrade the package.",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedExplicitRootInvalid = new(
        AtomUIDiagnosticIds.LinkedExplicitRootInvalid,
        "Explicit AtomUI linked-registration root is invalid",
        "Explicit {0} root '{1}' cannot be resolved. Use AtomUIRegistrationUnitRoot for a known Unit or AtomUIPackageRoot for a full Package.",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedPackageDefinitionInvalid = new(
        AtomUIDiagnosticIds.LinkedPackageDefinitionInvalid,
        "AtomUI linked-registration package definition is invalid",
        "Package '{0}' contains an invalid registration granularity, Registration Unit, or PackageShared definition: {1}",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedManifestVersionMismatch = new(
        AtomUIDiagnosticIds.LinkedManifestVersionMismatch,
        "AtomUI linked-registration input is incompatible",
        "Linked-registration input '{0}' is incompatible or malformed: {1}",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedLooseAxamlWidened = new(
        AtomUIDiagnosticIds.LinkedLooseAxamlWidened,
        "Loose AXAML or dynamic theme requires package fallback",
        "Dynamic resource source '{0}' can load Package '{1}'; that Package uses full fallback. Add <AtomUIPackageRoot Include=\"{1}\" /> to declare the boundary explicitly.",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedPackageEntryMissing = new(
        AtomUIDiagnosticIds.LinkedPackageEntryMissing,
        "AtomUI package usage has no registration entry",
        "Package '{0}' is used by '{1}', but its UseXxxControls() registration entry is not invoked",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedPackageEntryInvalid = new(
        AtomUIDiagnosticIds.LinkedPackageEntryInvalid,
        "AtomUI control package registration entry is invalid",
        "Control package registration entry '{0}' is invalid: {1}",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LinkedDynamicUsageUncovered = new(
        AtomUIDiagnosticIds.LinkedDynamicUsageUncovered,
        "Dynamic AtomUI usage is not covered by the static registration plan",
        "AtomUI dynamic usage '{0}' cannot be resolved statically; controls of Package '{1}' created only through this site are not registered. Add an AtomUIRegistrationUnitRoot when the Unit is known, or an AtomUIPackageRoot for fully dynamic usage; no action is required when this site never creates AtomUI controls.",
        AtomUIDiagnosticCategories.LinkedRegistration,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

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

    public static readonly DiagnosticDescriptor ThemeAssetMissingIdentity = new(
        AtomUIDiagnosticIds.ThemeAssetMissingIdentity,
        "Theme asset requires a convention-owned Control",
        "Theme asset '{0}' uses Control Token resources but no public owner Control can be determined from its file, directory, Semantic Part property, or Token family",
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

    public static readonly DiagnosticDescriptor ControlTokenInvalidName = new(
        AtomUIDiagnosticIds.ControlTokenInvalidName,
        "Control design token name does not follow convention",
        "Control design token type '{0}' must end with 'Token' and have a non-empty Control name",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ControlTokenInheritance = new(
        AtomUIDiagnosticIds.ControlTokenInheritance,
        "Control design token cannot inherit another Control Token",
        "Control design token type '{0}' cannot inherit Control Token type '{1}'; inherit AbstractControlDesignToken directly",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ControlTokenMissingControl = new(
        AtomUIDiagnosticIds.ControlTokenMissingControl,
        "Control design token has no matching Control",
        "Control design token type '{0}' requires a matching public Control named '{1}'",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ControlTokenAmbiguousControl = new(
        AtomUIDiagnosticIds.ControlTokenAmbiguousControl,
        "Control design token matches more than one Control",
        "Control design token type '{0}' matches more than one public Control named '{1}'",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ThemeAssetAmbiguousControl = new(
        AtomUIDiagnosticIds.ThemeAssetAmbiguousControl,
        "Theme asset matches more than one Control",
        "Theme asset '{0}' matches more than one public Control named '{1}'",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ThemeAssetSemanticPartTargetMismatch = new(
        AtomUIDiagnosticIds.ThemeAssetSemanticPartTargetMismatch,
        "Semantic Part Theme target must be a Control",
        "Semantic Part Theme asset '{0}' targets '{1}', which is not a public Control",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ThemeAssetUnknownTokenResource = new(
        AtomUIDiagnosticIds.ThemeAssetUnknownTokenResource,
        "Theme asset uses an unknown Control Token resource",
        "Theme asset '{0}' uses unknown Token '{2}' from Control Token family '{1}'",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor ControlTokenGlobalNameConflict = new(
        AtomUIDiagnosticIds.ControlTokenGlobalNameConflict,
        "Control Own Token conflicts with a Global Token",
        "Control '{0}' Own Token '{1}' conflicts with a Global Token",
        AtomUIDiagnosticCategories.Generator,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LocalizationInvalidLanguageData = new(
        AtomUIDiagnosticIds.LocalizationInvalidLanguageData,
        "Pinned language data record is invalid",
        "Language data '{0}' line {1} is invalid: {2}",
        AtomUIDiagnosticCategories.Localization,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LocalizationDuplicateLanguageData = new(
        AtomUIDiagnosticIds.LocalizationDuplicateLanguageData,
        "Pinned language data record is duplicated",
        "Language data '{0}' line {1} duplicates {2}",
        AtomUIDiagnosticCategories.Localization,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LocalizationInvalidCatalog = new(
        AtomUIDiagnosticIds.LocalizationInvalidCatalog,
        "Language Catalog declaration is invalid",
        "Language Catalog '{0}' is invalid: {1}",
        AtomUIDiagnosticCategories.Localization,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LocalizationInvalidCatalogUnit = new(
        AtomUIDiagnosticIds.LocalizationInvalidCatalogUnit,
        "Language Catalog unit is invalid",
        "Language Catalog unit '{0}' in '{1}' is invalid: {2}",
        AtomUIDiagnosticCategories.Localization,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LocalizationInvalidXliff = new(
        AtomUIDiagnosticIds.LocalizationInvalidXliff,
        "XLIFF language document is invalid",
        "XLIFF language document '{0}' is invalid: {1}",
        AtomUIDiagnosticCategories.Localization,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LocalizationCatalogXliffMismatch = new(
        AtomUIDiagnosticIds.LocalizationCatalogXliffMismatch,
        "XLIFF does not match its Language Catalog",
        "XLIFF language document '{0}' does not match Catalog '{1}': {2}",
        AtomUIDiagnosticCategories.Localization,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LocalizationInvalidTranslation = new(
        AtomUIDiagnosticIds.LocalizationInvalidTranslation,
        "XLIFF translation message is invalid",
        "Translation unit '{0}' for language '{1}' is invalid: {2}",
        AtomUIDiagnosticCategories.Localization,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);

    public static readonly DiagnosticDescriptor LocalizationInvalidApplicationHost = new(
        AtomUIDiagnosticIds.LocalizationInvalidApplicationHost,
        "Application cannot host generated localization bootstrap",
        "Application type '{0}' cannot host generated localization bootstrap: {1}",
        AtomUIDiagnosticCategories.Localization,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.Telemetry]);
}
#pragma warning restore RS2008
