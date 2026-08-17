using System.Collections.Immutable;
using AtomUI.Generator.Diagnostics;
using AtomUI.Generator.LinkedRegistration.Manifest;
using AtomUI.Generator.LinkedRegistration.Model;
using AtomUI.Generator.LinkedRegistration.Writers;
using AtomUI.LinkedRegistration.Protocol;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.LinkedRegistration;

[Generator]
public sealed class ApplicationRegistrationPlanGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var csharpCandidates = LinkedCSharpUsageCandidateProvider.Create(context).Collect();
        var input = context.CompilationProvider
                           .Combine(csharpCandidates)
                           .Combine(context.AdditionalTextsProvider.Collect())
                           .Combine(context.AnalyzerConfigOptionsProvider);
        context.RegisterSourceOutput(input, static (productionContext, value) =>
        {
            Generate(
                productionContext,
                value.Left.Left.Left,
                value.Left.Left.Right,
                value.Left.Right,
                value.Right);
        });
    }

    private static void Generate(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<LinkedCSharpUsageCandidate> csharpCandidates,
        ImmutableArray<AdditionalText> additionalTexts,
        Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider optionsProvider)
    {
        if (!LinkedRegistrationOptions.IsLinkedPublish(optionsProvider) ||
            !LinkedRegistrationOptions.IsRegistrationPlanOwner(optionsProvider) ||
            LinkedRegistrationOptions.GetDeclaredPackageId(optionsProvider).Length != 0)
        {
            return;
        }

        var catalog = LinkedRegistrationManifestCatalog.Create(
            compilation,
            additionalTexts,
            optionsProvider,
            context.ReportDiagnostic);
        if (catalog.PlanOwners.Count != 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.LinkedPlanOwner,
                Location.None,
                string.Join(", ", catalog.PlanOwners.OrderBy(static owner => owner, StringComparer.Ordinal))));
            return;
        }
        if (catalog.HasErrors)
        {
            return;
        }

        var hasUsageErrors = false;
        var currentUsages = LinkedRegistrationUsageGenerator.CollectApplicationUsages(
            compilation,
            catalog,
            csharpCandidates,
            additionalTexts,
            optionsProvider,
            context.CancellationToken,
            diagnostic => hasUsageErrors |= diagnostic.Severity == DiagnosticSeverity.Error,
            out var applicationBudgetExceeded);
        if (csharpCandidates.Length > LinkedRegistrationAnalysisBudget.MaxSourceCandidates ||
            applicationBudgetExceeded)
        {
            catalog.AddAnalysisBudgetFallback(compilation.AssemblyName ?? "<application>");
        }
        if (hasUsageErrors)
        {
            return;
        }
        var usages = catalog.ReferencedUsages.Concat(currentUsages).ToArray();
        var invokedPackageSet = new HashSet<string>(
            usages.Where(static usage => usage.Kind == LinkedUsageKind.Entry)
                  .Select(static usage => usage.Identity),
            StringComparer.Ordinal);
        if (HasMissingPackageEntry(catalog, usages, invokedPackageSet))
        {
            return;
        }
        var invokedPackages = invokedPackageSet.OrderBy(
            static packageId => packageId,
            StringComparer.Ordinal).ToArray();
        var plans = ImmutableArray.CreateBuilder<ApplicationPackagePlan>(invokedPackages.Length);
        var hasInvalidCallTarget = false;
        foreach (var packageId in invokedPackages)
        {
            if (!catalog.Packages.TryGetValue(packageId, out var package))
            {
                continue;
            }

            var packageUnits = catalog.Units.Values.Where(unit =>
                    string.Equals(unit.PackageId, packageId, StringComparison.Ordinal))
                .OrderBy(static unit => unit.OrderKey)
                .ThenBy(static unit => unit.UnitId, StringComparer.Ordinal)
                .ToArray();
            var hasPackageFallback = usages.Any(usage =>
                                      usage.Kind == LinkedUsageKind.PackageRoot &&
                                      string.Equals(usage.Identity, packageId, StringComparison.Ordinal)) ||
                                     catalog.Fallbacks.Any(fallback =>
                                         fallback.PackageId.Length == 0 ||
                                         string.Equals(fallback.PackageId, packageId, StringComparison.Ordinal));
            var isLegacy = packageUnits.Length == 0 && !hasPackageFallback;
            var useFullFallback = isLegacy || usages.Any(usage =>
                usage.Kind == LinkedUsageKind.PackageRoot &&
                string.Equals(usage.Identity, packageId, StringComparison.Ordinal)) ||
                catalog.Fallbacks.Any(fallback =>
                    // Unresolvable C# dynamic creation (DynamicInvocation) is reported as
                    // guidance (ATOMUILINK010) but never widens the plan: application-declared
                    // controls are already covered by base-type evidence, and purely
                    // string-driven creation is covered by explicit Unit/Package roots.
                    !string.Equals(
                        fallback.Reason,
                        LinkedRegistrationProtocol.FallbackReasonDynamicInvocation,
                        StringComparison.Ordinal) &&
                    (fallback.PackageId.Length == 0 ||
                     string.Equals(fallback.PackageId, packageId, StringComparison.Ordinal)));
            if (isLegacy)
            {
                if (ValidateFragmentMethod(
                        compilation,
                        context,
                        packageId,
                        package.FullFragmentType,
                        package.FullFragmentMethod,
                        FragmentMethodKind.FullRegistrar,
                        "full registrar"))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        GetLegacyDescriptor(LinkedRegistrationOptions.IsRegistrationStrict(optionsProvider)),
                        Location.None,
                        packageId));
                }
                else
                {
                    hasInvalidCallTarget = true;
                }
            }

            var selectedUnitIds = new HashSet<string>(StringComparer.Ordinal);
            if (!useFullFallback)
            {
                if (string.Equals(package.Granularity, "Package", StringComparison.Ordinal))
                {
                    selectedUnitIds.UnionWith(packageUnits.Select(static unit => unit.UnitId));
                }
                if (catalog.RootUnitsByPackage.TryGetValue(packageId, out var rootUnits))
                {
                    selectedUnitIds.UnionWith(rootUnits);
                }
                foreach (var usage in usages)
                {
                    if (usage.Kind == LinkedUsageKind.Control &&
                        catalog.ControlMaps.TryGetValue(usage.Identity, out var control) &&
                        string.Equals(control.PackageId, packageId, StringComparison.Ordinal))
                    {
                        selectedUnitIds.Add(control.UnitId);
                    }
                    else if (usage.Kind == LinkedUsageKind.UnitRoot &&
                             catalog.Units.TryGetValue(usage.Identity, out var unit) &&
                             string.Equals(unit.PackageId, packageId, StringComparison.Ordinal))
                    {
                        selectedUnitIds.Add(unit.UnitId);
                    }
                }
            }

            var selectedUnits = useFullFallback
                ? ImmutableArray<LinkedUnitManifestRecord>.Empty
                : RegistrationUnitGraphPlanner.Plan(
                    packageUnits,
                    catalog.UnitEdges.Where(edge => string.Equals(
                        edge.PackageId,
                        packageId,
                        StringComparison.Ordinal)).ToArray(),
                    selectedUnitIds);
            if (useFullFallback && !isLegacy &&
                !ValidateFragmentMethod(
                    compilation,
                    context,
                    packageId,
                    package.FullFragmentType,
                    package.FullFragmentMethod,
                    FragmentMethodKind.FullRegistrar,
                    "full registrar"))
            {
                hasInvalidCallTarget = true;
            }
            if (!useFullFallback && package.PackageSharedFragmentType is not null &&
                !ValidateFragmentMethod(
                    compilation,
                    context,
                    packageId,
                    package.PackageSharedFragmentType,
                    package.PackageSharedFragmentMethod!,
                    FragmentMethodKind.PackageBuilder,
                    "PackageShared fragment"))
            {
                hasInvalidCallTarget = true;
            }
            foreach (var unit in selectedUnits)
            {
                if (!ValidateFragmentMethod(
                        compilation,
                        context,
                        packageId,
                        unit.FragmentType,
                        unit.FragmentMethod,
                        FragmentMethodKind.PackageBuilder,
                        $"Registration Unit '{unit.UnitId}' fragment"))
                {
                    hasInvalidCallTarget = true;
                }
            }

            plans.Add(new ApplicationPackagePlan(
                packageId,
                useFullFallback,
                selectedUnits,
                package));
        }

        foreach (var fallback in catalog.Fallbacks.OrderBy(
                     static item => item.PackageId,
                     StringComparer.Ordinal).ThenBy(static item => item.Source, StringComparer.Ordinal)
                                              .ThenBy(static item => item.Line)
                                              .ThenBy(static item => item.Column)
                                              .ThenBy(static item => item.Reason, StringComparer.Ordinal))
        {
            // ExtractedManifest marks a consumer-extracted ProjectReference Sidecar: the plan
            // already widened to full fallback, and the delivery mode is not a code problem.
            if (string.Equals(
                    fallback.Reason,
                    LinkedRegistrationProtocol.FallbackReasonExtractedManifest,
                    StringComparison.Ordinal))
            {
                continue;
            }
            if (fallback.PackageId.Length != 0 && !invokedPackageSet.Contains(fallback.PackageId))
            {
                continue;
            }
            var baseDescriptor = string.Equals(
                fallback.Reason,
                LinkedRegistrationProtocol.FallbackReasonDynamicInvocation,
                StringComparison.Ordinal)
                ? AtomUIDiagnosticDescriptors.LinkedDynamicUsageUncovered
                : AtomUIDiagnosticDescriptors.LinkedDynamicUsageWidened;
            context.ReportDiagnostic(Diagnostic.Create(
                GetFallbackDescriptor(
                    LinkedRegistrationOptions.IsRegistrationStrict(optionsProvider),
                    baseDescriptor),
                Location.None,
                fallback.Reason,
                fallback.PackageId.Length == 0 ? "<all invoked packages>" : fallback.PackageId));
        }

        if (hasInvalidCallTarget)
        {
            return;
        }

        context.AddSource(
            "GeneratedApplicationRegistrationPlan.g.cs",
            GeneratedSourceText.From(ApplicationRegistrationPlanWriter.Write(
                compilation.AssemblyName ?? "AtomUI.Application",
                plans.ToImmutable())));
    }

    private static bool HasMissingPackageEntry(
        LinkedRegistrationManifestCatalog catalog,
        IEnumerable<LinkedUsageInfo> usages,
        HashSet<string> invokedPackages)
    {
        foreach (var usage in usages)
        {
            var packageId = usage.Kind switch
            {
                LinkedUsageKind.Control when catalog.ControlMaps.TryGetValue(
                    usage.Identity,
                    out var control) => control.PackageId,
                LinkedUsageKind.UnitRoot when catalog.PackageByUnit.TryGetValue(
                    usage.Identity,
                    out var unitPackageId) => unitPackageId,
                LinkedUsageKind.PackageRoot => usage.Identity,
                _ => string.Empty
            };
            if (packageId.Length != 0 && !invokedPackages.Contains(packageId))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ValidateFragmentMethod(
        Compilation compilation,
        SourceProductionContext context,
        string packageId,
        string typeMetadataName,
        string methodName,
        FragmentMethodKind methodKind,
        string fragmentKind)
    {
        var type = compilation.GetTypeByMetadataName(typeMetadataName);
        var valid = type is not null &&
                    compilation.IsSymbolAccessibleWithin(type, compilation.Assembly) &&
                    type.GetMembers(methodName).OfType<IMethodSymbol>().Any(method =>
                        compilation.IsSymbolAccessibleWithin(method, compilation.Assembly) &&
                        method.IsStatic &&
                        method.Arity == 0 &&
                        method.ReturnsVoid &&
                        HasExpectedParameters(compilation, method, methodKind));
        if (valid)
        {
            return true;
        }

        context.ReportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.LinkedPackageDefinitionInvalid,
            Location.None,
            packageId,
            $"{fragmentKind} '{typeMetadataName}.{methodName}' is not a resolvable public static method"));
        return false;
    }

    private static bool HasExpectedParameters(
        Compilation compilation,
        IMethodSymbol method,
        FragmentMethodKind methodKind)
    {
        var builderType = compilation.GetTypeByMetadataName(
            "AtomUI.Registration.AotTrimControlPackageRegistrationBuilder");
        if (methodKind == FragmentMethodKind.PackageBuilder)
        {
            return builderType is not null &&
                   method.Parameters.Length == 1 &&
                   IsParameter(method.Parameters[0], builderType);
        }

        var themeBuilderType = compilation.GetTypeByMetadataName("AtomUI.Theme.IThemeManagerBuilder");
        var providerType = compilation.GetTypeByMetadataName(
            "AtomUI.Theme.Resources.IControlThemesProvider");
        var identityType = compilation.GetTypeByMetadataName(
            "AtomUI.Theme.Schema.ControlTokenIdentity");
        var assetType = compilation.GetTypeByMetadataName(
            "AtomUI.Theme.Schema.ControlThemeAssetDescriptor");
        var funcType = compilation.GetTypeByMetadataName("System.Func`2");
        var readOnlyListType = compilation.GetTypeByMetadataName(
            "System.Collections.Generic.IReadOnlyList`1");
        if (themeBuilderType is null || providerType is null || identityType is null ||
            assetType is null || funcType is null || readOnlyListType is null)
        {
            return false;
        }

        var includeIdentityType = funcType.Construct(
            identityType,
            compilation.GetSpecialType(SpecialType.System_Boolean));
        var assetListType = readOnlyListType.Construct(assetType);
        var selectAssetsType = funcType.Construct(assetListType, assetListType);
        return method.Parameters.Length == 4 &&
               IsParameter(method.Parameters[0], themeBuilderType) &&
               IsParameter(method.Parameters[1], providerType) &&
               IsParameter(method.Parameters[2], includeIdentityType) &&
               IsParameter(method.Parameters[3], selectAssetsType);
    }

    private static bool IsParameter(IParameterSymbol parameter, ITypeSymbol expectedType)
    {
        return parameter.RefKind == RefKind.None &&
               SymbolEqualityComparer.Default.Equals(parameter.Type, expectedType);
    }

    private static DiagnosticDescriptor GetLegacyDescriptor(bool strict)
    {
        if (!strict)
        {
            return AtomUIDiagnosticDescriptors.LinkedLegacyPackageFallback;
        }
        var descriptor = AtomUIDiagnosticDescriptors.LinkedLegacyPackageFallback;
        return new DiagnosticDescriptor(
            descriptor.Id,
            descriptor.Title,
            descriptor.MessageFormat,
            descriptor.Category,
            DiagnosticSeverity.Error,
            descriptor.IsEnabledByDefault,
            descriptor.Description,
            descriptor.HelpLinkUri,
            descriptor.CustomTags.ToArray());
    }

    private static DiagnosticDescriptor GetFallbackDescriptor(bool strict, DiagnosticDescriptor descriptor)
    {
        if (!strict)
        {
            return descriptor;
        }
        return new DiagnosticDescriptor(
            descriptor.Id,
            descriptor.Title,
            descriptor.MessageFormat,
            descriptor.Category,
            DiagnosticSeverity.Error,
            descriptor.IsEnabledByDefault,
            descriptor.Description,
            descriptor.HelpLinkUri,
            descriptor.CustomTags.ToArray());
    }

    private enum FragmentMethodKind
    {
        PackageBuilder,
        FullRegistrar
    }
}
