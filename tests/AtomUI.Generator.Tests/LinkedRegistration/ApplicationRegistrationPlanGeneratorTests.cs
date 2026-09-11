extern alias LinkedPublish;

using System.Collections.Immutable;
using System.Text;
using AtomUI.Generator.LinkedRegistration;
using AtomUI.Generator.LinkedRegistration.Manifest;
using ApplicationRegistrationPlanGenerator = LinkedPublish::AtomUI.Generator.LinkedRegistration.ApplicationRegistrationPlanGenerator;
using LinkedRegistrationUsageGenerator = LinkedPublish::AtomUI.Generator.LinkedRegistration.LinkedRegistrationUsageGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class ApplicationRegistrationPlanGeneratorTests
{
    [Fact]
    public void Groups_Invoked_Packages_And_Orders_Deduplicated_Units_Ordinally()
    {
        var desktop = PlanGeneratorTestHost.Package(
            "AtomUI.Desktop.Controls",
            "AtomUI.Desktop.Entry.UseDesktopControls",
            "GeneratedDesktopFullFragment",
            "GeneratedDesktopPackageSharedFragment",
            [
                PlanGeneratorTestHost.Unit("AtomUI.Desktop.Controls", "AtomUI.Desktop.Controls/DatePicker", "GeneratedDatePickerUnitFragment"),
                PlanGeneratorTestHost.Unit("AtomUI.Desktop.Controls", "AtomUI.Desktop.Controls/Button", "GeneratedButtonUnitFragment")
            ],
            [
                PlanGeneratorTestHost.Control("AtomUI.Desktop.Controls", "AtomUI.Desktop.Controls.DatePicker", "AtomUI.Desktop.Controls/DatePicker"),
                PlanGeneratorTestHost.Control("AtomUI.Desktop.Controls", "AtomUI.Desktop.Controls.Button", "AtomUI.Desktop.Controls/Button")
            ]);
        var colorPicker = PlanGeneratorTestHost.Package(
            "AtomUI.Desktop.Controls.ColorPicker",
            "AtomUI.ColorPicker.Entry.UseColorPicker",
            "GeneratedColorPickerFullFragment",
            "GeneratedColorPickerPackageSharedFragment",
            [PlanGeneratorTestHost.Unit("AtomUI.Desktop.Controls.ColorPicker", "AtomUI.Desktop.Controls.ColorPicker/ColorPicker", "GeneratedColorPickerUnitFragment")],
            [PlanGeneratorTestHost.Control("AtomUI.Desktop.Controls.ColorPicker", "AtomUI.ColorPicker.ColorPicker", "AtomUI.Desktop.Controls.ColorPicker/ColorPicker")]);
        var usage = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.Desktop.Controls.DatePicker", "View.axaml", 2, 3),
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.Desktop.Controls.Button", "View.axaml", 4, 3),
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.Desktop.Controls.Button", "View.axaml", 4, 3),
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.ColorPicker.ColorPicker", "View.axaml", 6, 3));

        var result = PlanGeneratorTestHost.Run(
            "AtomUI.Desktop.Entry.UseDesktopControls(); AtomUI.ColorPicker.Entry.UseColorPicker();",
            [colorPicker, usage, desktop]);
        var planSource = result.PlanSource;
        planSource.ShouldNotBeNull();

        result.Diagnostics.Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
              .ShouldBeEmpty();
        planSource!.IndexOf("case \"AtomUI.Desktop.Controls\"", StringComparison.Ordinal)
              .ShouldBeLessThan(planSource.IndexOf("case \"AtomUI.Desktop.Controls.ColorPicker\"", StringComparison.Ordinal));
        planSource.ShouldContain(
            "var packageBuilder = new global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder(\n" +
            "                    builder,\n" +
            "                    provider,\n" +
            "                    includeIdentity,\n" +
            "                    selectAssets);");
        var shared = planSource.IndexOf("global::GeneratedDesktopPackageSharedFragment.Add(packageBuilder);", StringComparison.Ordinal);
        var button = planSource.IndexOf("global::GeneratedButtonUnitFragment.Add(packageBuilder);", StringComparison.Ordinal);
        var datePicker = planSource.IndexOf("global::GeneratedDatePickerUnitFragment.Add(packageBuilder);", StringComparison.Ordinal);
        shared.ShouldBeLessThan(button);
        button.ShouldBeLessThan(datePicker);
        planSource.CountOccurrences("global::GeneratedButtonUnitFragment.Add(packageBuilder);").ShouldBe(1);
        planSource.ShouldContain("packageBuilder.Register();\n                return true;");
    }

    [Fact]
    public void Package_Uncertainty_Uses_Only_That_Packages_Full_Registrar()
    {
        var desktop = PlanGeneratorTestHost.StandardDesktopPackage();
        var colorPicker = PlanGeneratorTestHost.Package(
            "AtomUI.Desktop.Controls.ColorPicker",
            "AtomUI.ColorPicker.Entry.UseColorPicker",
            "GeneratedColorPickerFullFragment",
            "GeneratedColorPickerPackageSharedFragment",
            [PlanGeneratorTestHost.Unit("AtomUI.Desktop.Controls.ColorPicker", "AtomUI.Desktop.Controls.ColorPicker/ColorPicker", "GeneratedColorPickerUnitFragment")],
            [PlanGeneratorTestHost.Control("AtomUI.Desktop.Controls.ColorPicker", "AtomUI.ColorPicker.ColorPicker", "AtomUI.Desktop.Controls.ColorPicker/ColorPicker")]);
        var usage = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(LinkedUsageKind.PackageRoot, "AtomUI.Desktop.Controls", "App.csproj", 0, 0),
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.ColorPicker.ColorPicker", "View.axaml", 3, 4));

        var result = PlanGeneratorTestHost.Run(
            "AtomUI.Desktop.Entry.UseDesktopControls(); AtomUI.ColorPicker.Entry.UseColorPicker();",
            [desktop, colorPicker, usage]);
        var planSource = result.PlanSource;
        planSource.ShouldNotBeNull();

        planSource!.ShouldContain("global::GeneratedDesktopFullFragment.Register(\n                    builder.Theme,");
        planSource.ShouldNotContain("global::GeneratedDesktopPackageSharedFragment.Add(packageBuilder);");
        planSource.ShouldContain("global::GeneratedColorPickerUnitFragment.Add(packageBuilder);");
        planSource.ShouldNotContain("global::GeneratedColorPickerFullFragment.Register(");
    }

    [Fact]
    public void Unknown_Manifest_Major_Is_An_Error_Not_A_Fallback()
    {
        var incompatible = UsageGeneratorTestHost.CreateRawManifestReference(
            "Future.Controls",
            ("AtomUI.Linked.Package.v2", "2|Future.Controls"));

        var result = PlanGeneratorTestHost.Run(string.Empty, [incompatible]);

        result.Diagnostics.ShouldContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK006" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Diagnostics.ShouldNotContain(static diagnostic => diagnostic.Id == "ATOMUILINK003");
    }

    [Fact]
    public void Ordinary_Non_Owner_Class_Library_Does_Not_Install_Or_Report_Zero_Owner()
    {
        var result = PlanGeneratorTestHost.Run(
            string.Empty,
            [],
            new Dictionary<string, string>
            {
                ["build_property.AtomUILinkedPublish"] = "false",
                ["build_property.AtomUIRegistrationPlanOwner"] = "false"
            });

        result.PlanSource.ShouldBeNull();
        result.Diagnostics.ShouldNotContain(static diagnostic => diagnostic.Id == "ATOMUILINK001");
    }

    [Fact]
    public void Current_Owner_Referencing_An_Existing_Owner_Marker_Fails_Closed()
    {
        var existingOwner = UsageGeneratorTestHost.CreateRawManifestReference(
            "Existing.Owner",
            (LinkedRegistrationProtocol.PlanMarkerKey, "Existing.Owner"));

        var result = PlanGeneratorTestHost.Run(string.Empty, [existingOwner]);

        result.Diagnostics.ShouldContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK001" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.PlanSource.ShouldBeNull();
    }

    [Fact]
    public void Invoked_Package_Without_Control_Usage_Remains_In_The_Plan()
    {
        var result = PlanGeneratorTestHost.Run(
            "AtomUI.Desktop.Entry.UseDesktopControls();",
            [PlanGeneratorTestHost.StandardDesktopPackage()]);
        var planSource = result.PlanSource;
        planSource.ShouldNotBeNull();

        planSource!.ShouldContain("case \"AtomUI.Desktop.Controls\"");
        planSource.ShouldContain("global::GeneratedDesktopPackageSharedFragment.Add(packageBuilder);");
        planSource.ShouldNotContain("UnitFragment.Add(packageBuilder);");
        planSource.ShouldContain("packageBuilder.Register();");
    }

    [Fact]
    public void Used_Package_Without_Entry_Is_Diagnosed_And_Prevents_A_Plan_Marker()
    {
        var usage = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.Desktop.Controls.Button", "View.axaml", 3, 4));

        var result = PlanGeneratorTestHost.Run(
            string.Empty,
            [PlanGeneratorTestHost.StandardDesktopPackage(), usage]);
        result.Diagnostics.ShouldContain(static diagnostic => diagnostic.Id == "ATOMUILINK008");
        result.PlanSource.ShouldBeNull();
    }

    [Fact]
    public void Dynamic_Invocation_Fallback_Warns_Without_Widening_The_Package()
    {
        var desktop = PlanGeneratorTestHost.StandardDesktopPackage();
        var usage = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.Desktop.Controls.Button", "View.axaml", 3, 4));
        var dynamicFallback = UsageGeneratorTestHost.CreateManifestReference(
            "Consumer.Usage.DynamicFallback",
            new LinkedFallbackManifestRecord(
                "AtomUI.Desktop.Controls",
                LinkedRegistrationProtocol.FallbackReasonDynamicInvocation,
                "ViewLocator.cs",
                27,
                29));

        var result = PlanGeneratorTestHost.Run(
            "AtomUI.Desktop.Entry.UseDesktopControls();",
            [desktop, usage, dynamicFallback]);
        var planSource = result.PlanSource;
        planSource.ShouldNotBeNull();

        planSource!.ShouldContain("global::GeneratedButtonUnitFragment.Add(packageBuilder);");
        planSource.ShouldNotContain("global::GeneratedDatePickerUnitFragment.Add(packageBuilder);");
        planSource.ShouldNotContain("global::GeneratedDesktopFullFragment.Register(");
        result.Diagnostics.ShouldContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK010" && diagnostic.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void Extracted_Manifest_Fallback_Widens_Without_A_Diagnostic()
    {
        var desktop = PlanGeneratorTestHost.StandardDesktopPackage();
        var usage = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.Desktop.Controls.Button", "View.axaml", 3, 4));
        var extractedFallback = UsageGeneratorTestHost.CreateManifestReference(
            "Consumer.ExtractedSidecar",
            new LinkedFallbackManifestRecord(
                "AtomUI.Desktop.Controls",
                LinkedRegistrationProtocol.FallbackReasonExtractedManifest,
                "<extraction>",
                0,
                0));

        var result = PlanGeneratorTestHost.Run(
            "AtomUI.Desktop.Entry.UseDesktopControls();",
            [desktop, usage, extractedFallback]);
        var planSource = result.PlanSource.ShouldNotBeNull();

        planSource.ShouldContain("global::GeneratedDesktopFullFragment.Register(");
        planSource.ShouldNotContain("global::GeneratedButtonUnitFragment.Add(packageBuilder);");
        result.Diagnostics.ShouldNotContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK002" ||
            diagnostic.Id == "ATOMUILINK007" ||
            diagnostic.Id == "ATOMUILINK010");
    }

    [Fact]
    public void Extracted_Consumer_Assembly_Fallback_Widens_Without_A_Diagnostic()
    {
        var desktop = PlanGeneratorTestHost.StandardDesktopPackage();
        var extractedConsumer = UsageGeneratorTestHost.CreateManifestReference(
            "Consumer.ExtractedAssembly",
            new LinkedUsageManifestRecord(
                LinkedUsageKind.PackageRoot,
                "AtomUI.Desktop.Controls",
                "Consumer.dll",
                0,
                0),
            new LinkedUsageManifestRecord(
                LinkedUsageKind.Entry,
                "AtomUI.Desktop.Controls",
                "Consumer.dll",
                0,
                0),
            new LinkedFallbackManifestRecord(
                "AtomUI.Desktop.Controls",
                LinkedRegistrationProtocol.FallbackReasonExtractedConsumerAssembly,
                "Consumer.dll",
                0,
                0));

        var result = PlanGeneratorTestHost.Run(string.Empty, [desktop, extractedConsumer]);
        var planSource = result.PlanSource.ShouldNotBeNull();

        planSource.ShouldContain("global::GeneratedDesktopFullFragment.Register(");
        result.Diagnostics.ShouldNotContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK002" ||
            diagnostic.Id == "ATOMUILINK007" ||
            diagnostic.Id == "ATOMUILINK010");
    }

    [Fact]
    public void Explicit_Unit_And_Package_Roots_Select_Precise_And_Full_Plans()
    {
        var desktop = PlanGeneratorTestHost.StandardDesktopPackage();
        var colorPicker = PlanGeneratorTestHost.Package(
            "AtomUI.Desktop.Controls.ColorPicker",
            "AtomUI.ColorPicker.Entry.UseColorPicker",
            "GeneratedColorPickerFullFragment",
            "GeneratedColorPickerPackageSharedFragment",
            [PlanGeneratorTestHost.Unit("AtomUI.Desktop.Controls.ColorPicker", "AtomUI.Desktop.Controls.ColorPicker/ColorPicker", "GeneratedColorPickerUnitFragment")],
            []);
        var roots = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(LinkedUsageKind.UnitRoot, "AtomUI.Desktop.Controls/Button", "App.csproj", 0, 0),
            new LinkedUsageManifestRecord(LinkedUsageKind.PackageRoot, "AtomUI.Desktop.Controls.ColorPicker", "App.csproj", 0, 0));

        var result = PlanGeneratorTestHost.Run(
            "AtomUI.Desktop.Entry.UseDesktopControls(); AtomUI.ColorPicker.Entry.UseColorPicker();",
            [desktop, colorPicker, roots]);
        var planSource = result.PlanSource;
        planSource.ShouldNotBeNull();

        planSource!.ShouldContain("global::GeneratedButtonUnitFragment.Add(packageBuilder);");
        planSource.ShouldContain("global::GeneratedColorPickerFullFragment.Register(");
    }

    [Fact]
    public void Legacy_Package_Uses_Known_Strongly_Typed_Full_Registrar()
    {
        var legacy = PlanGeneratorTestHost.Package(
            "Legacy.Controls",
            "Legacy.Entry.UseControls",
            "LegacyGeneratedFullFragment",
            null,
            [],
            []);

        var result = PlanGeneratorTestHost.Run("Legacy.Entry.UseControls();", [legacy]);
        var planSource = result.PlanSource;
        planSource.ShouldNotBeNull();

        result.Diagnostics.ShouldContain(static diagnostic => diagnostic.Id == "ATOMUILINK003");
        planSource!.ShouldContain("global::LegacyGeneratedFullFragment.Register(");
        planSource.ShouldNotContain("Assembly.Load");
        planSource.ShouldNotContain("GetType(");
        planSource.ShouldNotContain("MethodInfo");
    }

    [Fact]
    public void Legacy_Package_Without_A_Resolvable_Full_Registrar_Fails_Closed()
    {
        var legacy = PlanGeneratorTestHost.Package(
            "Broken.Legacy.Controls",
            "Legacy.Entry.UseControls",
            "MissingLegacyFullFragment",
            null,
            [],
            []);

        var result = PlanGeneratorTestHost.Run("Legacy.Entry.UseControls();", [legacy]);

        result.Diagnostics.ShouldContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.PlanSource.ShouldBeNull();
    }

    [Fact]
    public void Unit_Fragment_With_The_Wrong_Parameter_Type_Fails_Closed()
    {
        var package = PlanGeneratorTestHost.Package(
            "Bad.Unit.Controls",
            "Legacy.Entry.UseControls",
            "LegacyGeneratedFullFragment",
            null,
            [PlanGeneratorTestHost.Unit("Bad.Unit.Controls", "Bad.Unit.Controls/Main", "WrongBuilderUnitFragment")],
            []);
        var root = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(
                LinkedUsageKind.UnitRoot,
                "Bad.Unit.Controls/Main",
                "App.csproj",
                0,
                0));

        var result = PlanGeneratorTestHost.Run("Legacy.Entry.UseControls();", [package, root]);

        result.Diagnostics.ShouldContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.PlanSource.ShouldBeNull();
    }

    [Fact]
    public void Full_Registrar_With_The_Wrong_Return_Type_Fails_Closed()
    {
        var package = PlanGeneratorTestHost.Package(
            "Bad.Full.Controls",
            "Legacy.Entry.UseControls",
            "WrongReturnFullFragment",
            null,
            [],
            []);

        var result = PlanGeneratorTestHost.Run("Legacy.Entry.UseControls();", [package]);

        result.Diagnostics.ShouldContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.PlanSource.ShouldBeNull();
    }

    [Fact]
    public void Fragment_Declared_On_An_Inaccessible_Type_Fails_Closed()
    {
        var package = PlanGeneratorTestHost.Package(
            "Hidden.Unit.Controls",
            "Legacy.Entry.UseControls",
            "LegacyGeneratedFullFragment",
            null,
            [PlanGeneratorTestHost.Unit("Hidden.Unit.Controls", "Hidden.Unit.Controls/Main", "HiddenFragmentContainer+HiddenUnitFragment")],
            []);
        var root = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(
                LinkedUsageKind.UnitRoot,
                "Hidden.Unit.Controls/Main",
                "App.csproj",
                0,
                0));

        var result = PlanGeneratorTestHost.Run("Legacy.Entry.UseControls();", [package, root]);

        result.Diagnostics.ShouldContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.PlanSource.ShouldBeNull();
    }

    [Fact]
    public void Referenced_Ordinary_Library_Usage_Gates_Units_But_Control_Package_Usage_Does_Not()
    {
        var desktop = PlanGeneratorTestHost.StandardDesktopPackage();
        var ordinaryUsage = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.Desktop.Controls.Button", "Library.cs", 2, 4));
        var producer = PlanGeneratorTestHost.Package(
            "Producer.Controls",
            "Producer.Entry.UseControls",
            "ProducerFullFragment",
            "ProducerSharedFragment",
            [PlanGeneratorTestHost.Unit("Producer.Controls", "Producer.Controls/Main", "ProducerMainUnitFragment")],
            [],
            [new LinkedUsageManifestRecord(LinkedUsageKind.Control, "AtomUI.Desktop.Controls.DatePicker", "Internal.axaml", 2, 3)]);

        var result = PlanGeneratorTestHost.Run(
            "AtomUI.Desktop.Entry.UseDesktopControls(); Producer.Entry.UseControls();",
            [producer, desktop, ordinaryUsage]);
        var planSource = result.PlanSource;
        planSource.ShouldNotBeNull();

        planSource!.ShouldContain("global::GeneratedButtonUnitFragment.Add(packageBuilder);");
        planSource.ShouldNotContain("global::GeneratedDatePickerUnitFragment.Add(packageBuilder);");
        result.Diagnostics.ShouldNotContain(static diagnostic =>
            diagnostic.Id == "ATOMUILINK008" && diagnostic.GetMessage().Contains("AtomUI.Desktop.Controls", StringComparison.Ordinal));
    }

    [Fact]
    public void Control_package_owned_fallback_evidence_widens_only_that_package()
    {
        var desktop = PlanGeneratorTestHost.StandardDesktopPackage();
        var colorPicker = PlanGeneratorTestHost.Package(
            "AtomUI.Desktop.Controls.ColorPicker",
            "AtomUI.ColorPicker.Entry.UseColorPicker",
            "GeneratedColorPickerFullFragment",
            "GeneratedColorPickerPackageSharedFragment",
            [PlanGeneratorTestHost.Unit(
                "AtomUI.Desktop.Controls.ColorPicker",
                "AtomUI.Desktop.Controls.ColorPicker/ColorPicker",
                "GeneratedColorPickerUnitFragment")],
            [PlanGeneratorTestHost.Control(
                "AtomUI.Desktop.Controls.ColorPicker",
                "AtomUI.ColorPicker.ColorPicker",
                "AtomUI.Desktop.Controls.ColorPicker/ColorPicker")],
            [new LinkedUsageManifestRecord(
                LinkedUsageKind.PackageRoot,
                "AtomUI.Desktop.Controls.ColorPicker",
                "Themes/UnknownTheme.axaml",
                0,
                0)]);
        var applicationUsage = PlanGeneratorTestHost.Usage(
            new LinkedUsageManifestRecord(
                LinkedUsageKind.Control,
                "AtomUI.Desktop.Controls.Button",
                "View.axaml",
                3,
                4),
            new LinkedUsageManifestRecord(
                LinkedUsageKind.Control,
                "AtomUI.ColorPicker.ColorPicker",
                "View.axaml",
                5,
                4));

        var result = PlanGeneratorTestHost.Run(
            "AtomUI.Desktop.Entry.UseDesktopControls(); AtomUI.ColorPicker.Entry.UseColorPicker();",
            [desktop, colorPicker, applicationUsage]);
        var planSource = result.PlanSource.ShouldNotBeNull();

        planSource.ShouldContain("global::GeneratedButtonUnitFragment.Add(packageBuilder);");
        planSource.ShouldNotContain("global::GeneratedDesktopFullFragment.Register(");
        planSource.ShouldContain("global::GeneratedColorPickerFullFragment.Register(");
        planSource.ShouldNotContain("global::GeneratedColorPickerUnitFragment.Add(packageBuilder);");
    }

    [Fact]
    public void Generated_Source_Contains_A_Single_Marker_And_No_Graph_Traversal()
    {
        var result = PlanGeneratorTestHost.Run(string.Empty, []);
        var planSource = result.PlanSource;
        planSource.ShouldNotBeNull();

        planSource!.CountOccurrences(LinkedRegistrationProtocol.PlanMarkerKey).ShouldBe(1);
        planSource.ShouldContain("AotTrimRegistrationPlanRegistry.Install(\"Consumer\", ApplyPackage);");
        planSource.ShouldNotContain("Graph");
        planSource.ShouldNotContain("Breadth");
        planSource.ShouldNotContain("Topological");
        planSource.ShouldNotContain("foreach");
        planSource.ShouldNotContain("while");
    }
}

internal static class PlanGeneratorTestHost
{
    private static readonly ImmutableArray<MetadataReference> s_platformReferences =
        ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
        .Split(Path.PathSeparator)
        .Select(static path => (MetadataReference)MetadataReference.CreateFromFile(path))
        .ToImmutableArray();

    internal static MetadataReference StandardDesktopPackage()
    {
        return Package(
            "AtomUI.Desktop.Controls",
            "AtomUI.Desktop.Entry.UseDesktopControls",
            "GeneratedDesktopFullFragment",
            "GeneratedDesktopPackageSharedFragment",
            [
                Unit("AtomUI.Desktop.Controls", "AtomUI.Desktop.Controls/Button", "GeneratedButtonUnitFragment"),
                Unit("AtomUI.Desktop.Controls", "AtomUI.Desktop.Controls/DatePicker", "GeneratedDatePickerUnitFragment")
            ],
            [
                Control("AtomUI.Desktop.Controls", "AtomUI.Desktop.Controls.Button", "AtomUI.Desktop.Controls/Button"),
                Control("AtomUI.Desktop.Controls", "AtomUI.Desktop.Controls.DatePicker", "AtomUI.Desktop.Controls/DatePicker")
            ]);
    }

    internal static LinkedUnitManifestRecord Unit(string packageId, string unitId, string fragmentType) =>
        new(packageId, unitId, fragmentType, "Add");

    internal static LinkedControlMapManifestRecord Control(string packageId, string metadataName, string unitId) =>
        new(packageId, metadataName, unitId);

    internal static MetadataReference Package(
        string packageId,
        string entry,
        string fullFragmentType,
        string? packageSharedFragmentType,
        IReadOnlyList<LinkedUnitManifestRecord> units,
        IReadOnlyList<LinkedControlMapManifestRecord> controls,
        IReadOnlyList<LinkedUsageManifestRecord>? usages = null)
    {
        entry = entry.Replace(".UseDesktopControls", ".EntryPoint.UseDesktopControls", StringComparison.Ordinal)
                     .Replace(".UseColorPicker", ".EntryPoint.UseColorPicker", StringComparison.Ordinal)
                     .Replace(".UseControls", ".EntryPoint.UseControls", StringComparison.Ordinal);
        var records = new List<LinkedRegistrationManifestRecord>
        {
            new LinkedPackageManifestRecord(
                packageId,
                packageId,
                units.Count == 0 ? "Package" : "Directory",
                entry,
                fullFragmentType,
                "Register",
                packageSharedFragmentType,
                packageSharedFragmentType is null ? null : "Add")
        };
        records.AddRange(units);
        records.AddRange(controls);
        if (usages is not null)
        {
            records.AddRange(usages);
        }
        return UsageGeneratorTestHost.CreateManifestReference(packageId, records.ToArray());
    }

    internal static MetadataReference Usage(params LinkedUsageManifestRecord[] usages) =>
        UsageGeneratorTestHost.CreateManifestReference("Consumer.Usage." + Guid.NewGuid().ToString("N"), usages);

    internal static PlanGeneratorExecution Run(
        string statements,
        IReadOnlyList<MetadataReference> references,
        IReadOnlyDictionary<string, string>? options = null)
    {
        var source = $$"""
            namespace AtomUI
            {
                public interface IAtomUIBuilder { Theme.IThemeManagerBuilder Theme { get; } }
            }
            namespace AtomUI.Theme
            {
                public interface IThemeManagerBuilder { }
            }
            namespace AtomUI.Theme.Resources
            {
                public interface IControlThemesProvider { }
            }
            namespace AtomUI.Theme.Schema
            {
                public sealed class ControlTokenIdentity { }
                public sealed class ControlThemeAssetDescriptor { }
            }
            namespace AtomUI.Registration
            {
                public delegate bool AotTrimRegistrationPlan(
                    global::AtomUI.IAtomUIBuilder builder,
                    string packageId,
                    global::AtomUI.Theme.Resources.IControlThemesProvider provider,
                    global::System.Func<global::AtomUI.Theme.Schema.ControlTokenIdentity, bool>? includeIdentity,
                    global::System.Func<global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>, global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>>? selectAssets);
                public sealed class AotTrimControlPackageRegistrationBuilder
                {
                    public AotTrimControlPackageRegistrationBuilder(
                        global::AtomUI.IAtomUIBuilder builder,
                        global::AtomUI.Theme.Resources.IControlThemesProvider provider,
                        global::System.Func<global::AtomUI.Theme.Schema.ControlTokenIdentity, bool>? includeIdentity,
                        global::System.Func<global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>, global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>>? selectAssets) { }
                    public void Register() { }
                }
                public static class AotTrimRegistrationPlanRegistry
                {
                    public static void Install(string ownerId, AotTrimRegistrationPlan plan) { }
                }
            }
            public static class GeneratedDesktopFullFragment
            {
                public static void Register(global::AtomUI.Theme.IThemeManagerBuilder builder, global::AtomUI.Theme.Resources.IControlThemesProvider provider, global::System.Func<global::AtomUI.Theme.Schema.ControlTokenIdentity, bool>? includeIdentity, global::System.Func<global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>, global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>>? selectAssets) { }
            }
            public static class GeneratedColorPickerFullFragment
            {
                public static void Register(global::AtomUI.Theme.IThemeManagerBuilder builder, global::AtomUI.Theme.Resources.IControlThemesProvider provider, global::System.Func<global::AtomUI.Theme.Schema.ControlTokenIdentity, bool>? includeIdentity, global::System.Func<global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>, global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>>? selectAssets) { }
            }
            public static class LegacyGeneratedFullFragment
            {
                public static void Register(global::AtomUI.Theme.IThemeManagerBuilder builder, global::AtomUI.Theme.Resources.IControlThemesProvider provider, global::System.Func<global::AtomUI.Theme.Schema.ControlTokenIdentity, bool>? includeIdentity, global::System.Func<global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>, global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>>? selectAssets) { }
            }
            public static class ProducerFullFragment
            {
                public static void Register(global::AtomUI.Theme.IThemeManagerBuilder builder, global::AtomUI.Theme.Resources.IControlThemesProvider provider, global::System.Func<global::AtomUI.Theme.Schema.ControlTokenIdentity, bool>? includeIdentity, global::System.Func<global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>, global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>>? selectAssets) { }
            }
            public static class GeneratedDesktopPackageSharedFragment { public static void Add(global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder) { } }
            public static class GeneratedColorPickerPackageSharedFragment { public static void Add(global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder) { } }
            public static class ProducerSharedFragment { public static void Add(global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder) { } }
            public static class GeneratedButtonUnitFragment { public static void Add(global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder) { } }
            public static class GeneratedDatePickerUnitFragment { public static void Add(global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder) { } }
            public static class GeneratedColorPickerUnitFragment { public static void Add(global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder) { } }
            public static class ProducerMainUnitFragment { public static void Add(global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder) { } }
            public static class WrongBuilderUnitFragment { public static void Add(string builder) { } }
            public static class HiddenFragmentContainer
            {
                private static class HiddenUnitFragment { public static void Add(global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder) { } }
            }
            public static class WrongReturnFullFragment
            {
                public static int Register(global::AtomUI.Theme.IThemeManagerBuilder builder, global::AtomUI.Theme.Resources.IControlThemesProvider provider, global::System.Func<global::AtomUI.Theme.Schema.ControlTokenIdentity, bool>? includeIdentity, global::System.Func<global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>, global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>>? selectAssets) => 0;
            }
            namespace AtomUI.Desktop.Entry { public static class EntryPoint { public static void UseDesktopControls() { } } }
            namespace AtomUI.ColorPicker.Entry { public static class EntryPoint { public static void UseColorPicker() { } } }
            namespace Producer.Entry { public static class EntryPoint { public static void UseControls() { } } }
            namespace Legacy.Entry { public static class EntryPoint { public static void UseControls() { } } }
            public static class Program
            {
                public static void Main()
                {
                    {{statements}}
                }
            }
            """;
        source = source.Replace("AtomUI.Desktop.Entry.UseDesktopControls()", "AtomUI.Desktop.Entry.EntryPoint.UseDesktopControls()", StringComparison.Ordinal)
                       .Replace("AtomUI.ColorPicker.Entry.UseColorPicker()", "AtomUI.ColorPicker.Entry.EntryPoint.UseColorPicker()", StringComparison.Ordinal)
                       .Replace("Producer.Entry.UseControls()", "Producer.Entry.EntryPoint.UseControls()", StringComparison.Ordinal)
                       .Replace("Legacy.Entry.UseControls()", "Legacy.Entry.EntryPoint.UseControls()", StringComparison.Ordinal);
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var compilation = CSharpCompilation.Create(
            "Consumer",
            [CSharpSyntaxTree.ParseText(source, parseOptions, "Program.cs")],
            s_platformReferences.Concat(references),
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        var effectiveOptions = options ?? new Dictionary<string, string>
        {
            ["build_property.AtomUILinkedPublish"] = "true",
            ["build_property.AtomUIRegistrationPlanOwner"] = "true"
        };
        var sidecars = UsageGeneratorTestHost.GetSidecars(references);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [
                new LinkedRegistrationUsageGenerator().AsSourceGenerator(),
                new ApplicationRegistrationPlanGenerator().AsSourceGenerator()
            ],
            sidecars.Cast<AdditionalText>().ToImmutableArray(),
            parseOptions: parseOptions,
            optionsProvider: new PlanOptionsProvider(effectiveOptions, sidecars));
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var driverDiagnostics,
            TestContext.Current.CancellationToken);
        var runResults = driver.GetRunResult().Results;
        runResults.Length.ShouldBe(2);
        var plan = runResults.SelectMany(static result => result.GeneratedSources)
                             .SingleOrDefault(static source =>
            source.HintName == "GeneratedApplicationRegistrationPlan.g.cs");
        return new PlanGeneratorExecution(
            plan.HintName is null ? null : plan.SourceText.ToString(),
            driverDiagnostics.Concat(runResults.SelectMany(static result => result.Diagnostics))
                             .Concat(outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken))
                             .Distinct()
                             .ToArray());
    }

    private sealed class PlanOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptions _options;
        private readonly IReadOnlyDictionary<string, AnalyzerConfigOptions> _fileOptions;

        internal PlanOptionsProvider(
            IReadOnlyDictionary<string, string> values,
            IReadOnlyList<TestAdditionalText> files)
        {
            _options = new PlanOptions(values);
            _fileOptions = files.ToDictionary(
                static file => file.Path,
                static file => (AnalyzerConfigOptions)new PlanOptions(file.Metadata),
                StringComparer.Ordinal);
        }

        public override AnalyzerConfigOptions GlobalOptions => _options;
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _options;
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            _fileOptions.TryGetValue(textFile.Path, out var options) ? options : _options;
    }

    private sealed class PlanOptions(IReadOnlyDictionary<string, string> values) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value) => values.TryGetValue(key, out value!);
    }
}

internal sealed record PlanGeneratorExecution(string? PlanSource, IReadOnlyList<Diagnostic> Diagnostics);

internal static class StringTestExtensions
{
    internal static int CountOccurrences(this string value, string text)
    {
        var count = 0;
        var index = 0;
        while ((index = value.IndexOf(text, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += text.Length;
        }
        return count;
    }
}
