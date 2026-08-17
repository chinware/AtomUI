using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class LinkedRegistrationDiagnosticContractTests
{
    [Fact]
    public void Linked_Registration_Diagnostic_Ids_Are_Stable_And_Unique()
    {
        var descriptors = GetDescriptors();

        descriptors.Select(static descriptor => descriptor.Id).ShouldBe(
        [
            "ATOMUILINK001",
            "ATOMUILINK002",
            "ATOMUILINK003",
            "ATOMUILINK004",
            "ATOMUILINK005",
            "ATOMUILINK006",
            "ATOMUILINK007",
            "ATOMUILINK008",
            "ATOMUILINK009",
            "ATOMUILINK010"
        ]);
        descriptors.Select(static descriptor => descriptor.Id).Distinct().Count().ShouldBe(10);
    }

    [Fact]
    public void Linked_Registration_Diagnostic_Severities_Match_The_Architecture()
    {
        var descriptors = GetDescriptors();

        descriptors[0].DefaultSeverity.ShouldBe(DiagnosticSeverity.Error);
        descriptors[1].DefaultSeverity.ShouldBe(DiagnosticSeverity.Warning);
        descriptors[2].DefaultSeverity.ShouldBe(DiagnosticSeverity.Warning);
        descriptors[3].DefaultSeverity.ShouldBe(DiagnosticSeverity.Error);
        descriptors[4].DefaultSeverity.ShouldBe(DiagnosticSeverity.Error);
        descriptors[5].DefaultSeverity.ShouldBe(DiagnosticSeverity.Error);
        descriptors[6].DefaultSeverity.ShouldBe(DiagnosticSeverity.Warning);
        descriptors[7].DefaultSeverity.ShouldBe(DiagnosticSeverity.Error);
        descriptors[8].DefaultSeverity.ShouldBe(DiagnosticSeverity.Error);
        descriptors[9].DefaultSeverity.ShouldBe(DiagnosticSeverity.Warning);
        descriptors.ShouldAllBe(descriptor =>
            descriptor.Category == AtomUIDiagnosticCategories.LinkedRegistration);
    }

    [Fact]
    public void Fixable_Diagnostics_Contain_Actionable_Root_Or_Entry_Guidance()
    {
        var descriptors = GetDescriptors();
        var messages = descriptors.Select(descriptor => descriptor.MessageFormat.ToString()).ToArray();

        messages[1].ShouldContain("Package");
        messages[1].ShouldContain("full fallback");
        messages[2].ShouldContain("AtomUIPackageRoot");
        messages[3].ShouldContain("AtomUIRegistrationUnitRoot");
        messages[4].ShouldContain("Unit");
        messages[4].ShouldContain("PackageShared");
        messages[6].ShouldContain("AtomUIPackageRoot");
        messages[7].ShouldContain("UseXxxControls");
        messages[8].ShouldContain("registration entry");
        messages[9].ShouldContain("AtomUIRegistrationUnitRoot");
        messages[9].ShouldContain("AtomUIPackageRoot");
    }

    private static DiagnosticDescriptor[] GetDescriptors()
    {
        return
        [
            AtomUIDiagnosticDescriptors.LinkedPlanOwner,
            AtomUIDiagnosticDescriptors.LinkedDynamicUsageWidened,
            AtomUIDiagnosticDescriptors.LinkedLegacyPackageFallback,
            AtomUIDiagnosticDescriptors.LinkedExplicitRootInvalid,
            AtomUIDiagnosticDescriptors.LinkedPackageDefinitionInvalid,
            AtomUIDiagnosticDescriptors.LinkedManifestVersionMismatch,
            AtomUIDiagnosticDescriptors.LinkedLooseAxamlWidened,
            AtomUIDiagnosticDescriptors.LinkedPackageEntryMissing,
            AtomUIDiagnosticDescriptors.LinkedPackageEntryInvalid,
            AtomUIDiagnosticDescriptors.LinkedDynamicUsageUncovered
        ];
    }
}
