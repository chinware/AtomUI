extern alias LinkedPublish;

using System.Collections.Immutable;
using System.Text;
using AtomUI.Generator.LinkedRegistration;
using AtomUI.Generator.LinkedRegistration.Manifest;
using LinkedRegistrationSidecar = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedRegistrationSidecar;
using LinkedRegistrationSidecarCodec = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedRegistrationSidecarCodec;
using LinkedRegistrationUsageGenerator = LinkedPublish::AtomUI.Generator.LinkedRegistration.LinkedRegistrationUsageGenerator;
using LinkedSidecarAssembly = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedSidecarAssembly;
using LinkedSidecarFallback = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedSidecarFallback;
using LinkedSidecarFragment = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedSidecarFragment;
using LinkedSidecarPackage = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedSidecarPackage;
using LinkedSidecarUnit = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedSidecarUnit;
using LinkedSidecarUnitEdge = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedSidecarUnitEdge;
using LinkedSidecarUsage = LinkedPublish::AtomUI.LinkedRegistration.Protocol.LinkedSidecarUsage;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class LinkedRegistrationUsageGeneratorTests
{
    [Fact]
    public void Source_candidate_budget_emits_a_conservative_package_fallback()
    {
        var source = "namespace Consumer { public static class Usage { public static void Run() { " +
                     string.Concat(Enumerable.Repeat(
                         "typeof(Acme.Controls.DatePicker);",
                         50_001)) +
                     " } }";

        var result = UsageGeneratorTestHost.Run([source], [UsageGeneratorTestHost.AcmePackage]);

        result.GeneratedSource.ShouldContain("AtomUI.Linked.Fallback.v1");
        result.GeneratedSource.ShouldContain("AnalysisBudgetExceeded");
    }

    [Theory]
    [InlineData("public sealed class Consumer<T> { public T? Create() => System.Activator.CreateInstance<T>(); }")]
    [InlineData("public sealed class Consumer<T> { public object? Create() => System.Activator.CreateInstance(typeof(T)); }")]
    public void Open_Generic_Dynamic_Creation_Requires_Explicit_Package_Root(string source)
    {
        var result = UsageGeneratorTestHost.Run([source], [UsageGeneratorTestHost.AcmePackage]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK004" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.Control);
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
    }

    [Fact]
    public void Static_Open_Generic_Typeof_Retains_The_Generic_Control_Unit()
    {
        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { public System.Type Value => typeof(Acme.Controls.GenericControl<>); }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Control &&
            usage.Identity == "Acme.Controls.GenericControl`1");
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK004");
    }

    [Fact]
    public void Open_Generic_Control_Dynamic_Creation_Remains_Unsupported()
    {
        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer<T> { public Acme.Controls.GenericControl<T>? Create() => System.Activator.CreateInstance<Acme.Controls.GenericControl<T>>(); }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK004" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
    }

    [Fact]
    public void CSharp_Semantic_Usage_Emits_Control_Metadata_For_Conservative_Type_Positions()
    {
        var source = """
                     using System;
                     using Acme.Controls;

                     public sealed class DerivedPicker : DatePicker { }
                     public sealed class Consumer : GenericControl<DatePicker>
                     {
                         private DatePicker _field = new DatePicker();
                         public DatePicker Property { get; set; } = new();
                         public DatePicker Method(DatePicker parameter) => parameter;
                         public Type RuntimeType => typeof(DatePicker);
                         public object Dynamic() => Activator.CreateInstance<DatePicker>();
                     }
                     """;

        var result = UsageGeneratorTestHost.Run([source], [UsageGeneratorTestHost.AcmePackage]);

        var controls = result.Usages.Where(static usage => usage.Kind == LinkedUsageKind.Control).ToArray();
        controls.ShouldContain(usage => usage.Identity == "Acme.Controls.DatePicker");
        controls.ShouldContain(usage => usage.Identity == "Acme.Controls.GenericControl`1");
        result.GeneratedSource.ShouldNotContain("typeof(");
        result.GeneratedSource.ShouldNotContain("AotTrimRegistrationPlan");
    }

    [Fact]
    public void Static_Control_Member_Call_Emits_Control_Usage()
    {
        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { public void Open(Avalonia.Controls.Control control) => Acme.Controls.DatePicker.SetIsOpen(control, true); }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Control &&
            usage.Identity == "Acme.Controls.DatePicker");
    }

    [Fact]
    public void Custom_Control_Retains_The_Unit_Of_Its_AtomUI_Base()
    {
        var result = UsageGeneratorTestHost.Run(
            ["public sealed class CustomPicker : Acme.Controls.DatePicker { }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Control && usage.Identity == "Acme.Controls.DatePicker");
    }

    [Fact]
    public void Axaml_Usage_Resolves_Through_ControlMap()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/MainView.axaml",
            """
            <AtomUIAxamlUsage Version="1"><Usage Source="Views/MainView.axaml" Line="3" Column="6" Kind="Element" NamespaceUri="using:Acme.Controls" LocalName="DatePicker" TypeName="Acme.Controls.DatePicker" Identity="" /></AtomUIAxamlUsage>
            """);

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class MainView { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            additionalTexts: [axaml]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Control &&
            usage.Identity == "Acme.Controls.DatePicker" &&
            usage.Source == "Views/MainView.axaml" &&
            usage.Line == 3 && usage.Column == 6);
    }

    [Fact]
    public void Unknown_Type_In_Package_Owned_Xml_Namespace_Widens_That_Package()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/UnknownView.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Views/UnknownView.axaml\" Line=\"3\" Column=\"6\" Kind=\"Element\" NamespaceUri=\"https://acme.example/controls\" LocalName=\"FuturePicker\" TypeName=\"\" Identity=\"\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class MainView { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            UsageGeneratorTestHost.LinkedEntryOptions,
            additionalTexts: [axaml]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Acme.Controls");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUILINK002");
    }

    [Fact]
    public void Unknown_Type_In_Multiply_Owned_Xml_Namespace_Is_An_Error()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/AmbiguousNamespace.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Views/AmbiguousNamespace.axaml\" Line=\"3\" Column=\"6\" Kind=\"Element\" NamespaceUri=\"https://shared.example/controls\" LocalName=\"FuturePicker\" TypeName=\"\" Identity=\"\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class MainView { }"] ,
            [UsageGeneratorTestHost.AcmePackage, UsageGeneratorTestHost.OtherPackage],
            additionalTexts: [axaml]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK004" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
    }

    [Fact]
    public void Unknown_Axaml_Namespace_Does_Not_Infer_A_Package_From_A_Control_Short_Name()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/UnknownNamespace.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Views/UnknownNamespace.axaml\" Line=\"3\" Column=\"6\" Kind=\"Element\" NamespaceUri=\"https://unknown.example/controls\" LocalName=\"DatePicker\" TypeName=\"\" Identity=\"\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class MainView { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            additionalTexts: [axaml]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK004" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
    }

    [Fact]
    public void Referenced_Package_Control_Missing_ControlMap_Requires_Package_Fallback()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/UnmappedView.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Views/UnmappedView.axaml\" Line=\"3\" Column=\"6\" Kind=\"Element\" NamespaceUri=\"using:Acme.Controls\" LocalName=\"UnmappedPicker\" TypeName=\"Acme.Controls.UnmappedPicker\" Identity=\"\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class MainView { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            UsageGeneratorTestHost.LinkedEntryOptions,
            additionalTexts: [axaml]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Acme.Controls");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUILINK002");
    }

    [Fact]
    public void Unresolved_AtomUI_Axaml_Type_Widens_Only_Its_Namespace_Package()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/FutureView.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Views/FutureView.axaml\" Line=\"3\" Column=\"6\" Kind=\"Element\" NamespaceUri=\"using:Acme.Controls\" LocalName=\"FuturePicker\" TypeName=\"Acme.Controls.FuturePicker\" Identity=\"\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class MainView { }"] ,
            [UsageGeneratorTestHost.AcmePackage, UsageGeneratorTestHost.OtherPackage],
            UsageGeneratorTestHost.LinkedEntryOptions,
            additionalTexts: [axaml]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Acme.Controls");
        result.Usages.ShouldNotContain(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Other.Controls");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUILINK002");
    }

    [Fact]
    public void Known_Non_AtomUI_Axaml_Type_Is_Ignored()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/ResourceView.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Views/ResourceView.axaml\" Line=\"1\" Column=\"2\" Kind=\"Element\" NamespaceUri=\"using:System.Collections.Generic\" LocalName=\"List`1\" TypeName=\"System.Collections.Generic.List`1\" Identity=\"\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class MainView { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            additionalTexts: [axaml]);

        result.Usages.ShouldBeEmpty();
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK004");
    }

    [Fact]
    public void Xaml_Language_Primitives_Are_Not_Treated_As_Control_Package_Usage()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Themes/Resources.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Themes/Resources.axaml\" Line=\"5\" Column=\"6\" Kind=\"Element\" NamespaceUri=\"http://schemas.microsoft.com/winfx/2006/xaml\" LocalName=\"Double\" TypeName=\"\" Identity=\"\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class ThemeResources { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            additionalTexts: [axaml]);

        result.Usages.ShouldBeEmpty();
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK004");
    }

    [Fact]
    public void Ordinary_Class_Library_Emits_Usage_When_Linked_Mode_Is_Disabled()
    {
        var library = UsageGeneratorTestHost.CompileGeneratedReference(
            "Consumer.Library",
            ["public sealed class Consumer { public Acme.Controls.DatePicker Create() => new(); }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            new Dictionary<string, string>
            {
                ["build_property.AtomUILinkedPublish"] = "false",
                ["build_property.AtomUIRegistrationPlanOwner"] = "false"
            });

        library.GetAssemblyMetadata()
               .ShouldContain(usage =>
                   usage.Kind == LinkedUsageKind.Control &&
                   usage.Identity == "Acme.Controls.DatePicker");
    }

    [Fact]
    public void Ordinary_Build_Records_Package_Fallback_Without_Linked_Warning()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/UnknownView.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Views/UnknownView.axaml\" Line=\"3\" Column=\"6\" Kind=\"Element\" NamespaceUri=\"https://acme.example/controls\" LocalName=\"FuturePicker\" TypeName=\"\" Identity=\"\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class MainView { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            new Dictionary<string, string>
            {
                ["build_property.AtomUILinkedPublish"] = "false",
                ["build_property.AtomUIRegistrationPlanOwner"] = "false"
            },
            [axaml]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Acme.Controls");
        result.Diagnostics.ShouldNotContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK002" || diagnostic.Id == "ATOMUILINK007");
    }

    [Fact]
    public void Registration_Package_Does_Not_Emit_Consumer_Usage_Or_Fallback_Diagnostics()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Themes/InternalTheme.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Themes/InternalTheme.axaml\" Line=\"3\" Column=\"6\" Kind=\"Element\" NamespaceUri=\"using:Acme.Controls\" LocalName=\"UnmappedPicker\" TypeName=\"Acme.Controls.UnmappedPicker\" Identity=\"\" /></AtomUIAxamlUsage>");
        var options = new Dictionary<string, string>
        {
            ["build_property.AtomUIRegistrationPackageId"] = "Composite.Controls"
        };

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class CompositeControl { public Acme.Controls.DatePicker? Picker { get; set; } }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            options,
            [axaml],
            "Composite.Controls");

        result.Usages.ShouldBeEmpty();
        result.Diagnostics.ShouldNotContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK002" || diagnostic.Id == "ATOMUILINK007");
    }

    [Fact]
    public void Registration_Package_Still_Validates_Unknown_Referenced_Manifest_Major()
    {
        var reference = UsageGeneratorTestHost.CreateRawManifestReference(
            "Future.Manifest",
            ("AtomUI.Linked.ControlMap.v2", "2|Acme.Controls|Acme.Controls.DatePicker|Acme.Controls%2FDatePicker"));

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class PackageControl { }"] ,
            [reference],
            new Dictionary<string, string>
            {
                ["build_property.AtomUIRegistrationPackageId"] = "Producer.Controls"
            });

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK006" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Usages.ShouldBeEmpty();
    }

    [Fact]
    public void Registration_Package_Still_Validates_Malformed_And_Conflicting_Manifests()
    {
        var malformed = UsageGeneratorTestHost.CreateRawManifestReference(
            "Malformed.Manifest",
            (LinkedRegistrationProtocol.ControlMapManifestKey, "1|Acme.Controls"));
        var conflicting = UsageGeneratorTestHost.CreateManifestReference(
            "Conflicting.Manifest",
            new LinkedPackageManifestRecord(
                "Conflict.Controls",
                "Conflicting.Manifest",
                "Conflict.Controls.Entry.UseControls",
                "Conflict.Controls.Full",
                "Register",
                null,
                null),
            new LinkedPackageManifestRecord(
                "Conflict.Controls",
                "Conflicting.Manifest",
                "Conflict.Controls.Entry.UseDifferentControls",
                "Conflict.Controls.Full",
                "Register",
                null,
                null));

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class PackageControl { }"] ,
            [malformed, conflicting],
            new Dictionary<string, string>
            {
                ["build_property.AtomUIRegistrationPackageId"] = "Producer.Controls"
            });

        result.Diagnostics.Count(diagnostic => diagnostic.Id == "ATOMUILINK005")
              .ShouldBeGreaterThanOrEqualTo(2);
        result.Usages.ShouldBeEmpty();
    }

    [Fact]
    public void Registration_Package_Still_Validates_Marked_Structured_Axaml()
    {
        var result = UsageGeneratorTestHost.Run(
            ["public sealed class PackageControl { }"] ,
            [],
            new Dictionary<string, string>
            {
                ["build_property.AtomUIRegistrationPackageId"] = "Producer.Controls"
            },
            [UsageGeneratorTestHost.Axaml("obj/AtomUIAxamlUsage.xml", "<WrongRoot Version=\"1\" />")]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK006" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Usages.ShouldBeEmpty();
    }

    [Fact]
    public void Linked_Entry_Reports_Missing_Package_Registration_From_Referenced_Library_Usage()
    {
        var library = UsageGeneratorTestHost.CompileGeneratedReference(
            "Consumer.Library",
            ["public sealed class Consumer { public Acme.Controls.DatePicker Create() => new(); }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        var result = UsageGeneratorTestHost.Run(
            ["public static class Program { public static void Main() { } }"] ,
            [UsageGeneratorTestHost.AcmePackage, library],
            UsageGeneratorTestHost.LinkedEntryOptions);

        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUILINK008");
    }

    [Fact]
    public void Referenced_Library_Entry_Usage_Satisfies_Application_Gating()
    {
        var library = UsageGeneratorTestHost.CompileGeneratedReference(
            "Registered.Library",
            ["public sealed class Consumer { public Acme.Controls.DatePicker? Picker { get; set; } public static void Register() => Acme.Controls.ThemeManagerBuilderExtensions.UseDesktopControls(); }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        var result = UsageGeneratorTestHost.Run(
            ["public static class Program { public static void Main() { } }"] ,
            [UsageGeneratorTestHost.AcmePackage, library],
            UsageGeneratorTestHost.LinkedEntryOptions);

        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK008");
    }

    [Fact]
    public void Invoked_Package_Entry_Satisfies_Linked_Entry_Gating()
    {
        var result = UsageGeneratorTestHost.Run(
            ["public static class Program { public static void Main() { Acme.Controls.ThemeManagerBuilderExtensions.UseDesktopControls(); _ = new Acme.Controls.DatePicker(); } }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            UsageGeneratorTestHost.LinkedEntryOptions);

        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK008");
        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Entry && usage.Identity == "Acme.Controls");
    }

    [Fact]
    public void One_entry_invocation_can_gate_multiple_packages()
    {
        var dependentPackage = UsageGeneratorTestHost.CreateManifestReference(
            "Dependent.Controls",
            new LinkedPackageManifestRecord(
                "Dependent.Controls",
                "Dependent.Controls",
                "Acme.Controls.ThemeManagerBuilderExtensions.UseDesktopControls",
                "Dependent.Controls.Generated.Full",
                "Register",
                null,
                null),
            new LinkedUnitManifestRecord(
                "Dependent.Controls",
                "Dependent.Controls/Shared",
                "Dependent.Controls.Generated.Shared",
                "Add"),
            new LinkedControlMapManifestRecord(
                "Dependent.Controls",
                "Dependent.Controls.SharedControl",
                "Dependent.Controls/Shared"));

        var result = UsageGeneratorTestHost.Run(
            ["public static class Program { public static void Main() { Acme.Controls.ThemeManagerBuilderExtensions.UseDesktopControls(); _ = new Acme.Controls.DatePicker(); Dependent.Controls.SharedControl? shared = null; } }"] ,
            [UsageGeneratorTestHost.AcmePackage, dependentPackage],
            UsageGeneratorTestHost.LinkedEntryOptions);

        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK005");
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK008");
        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Entry && usage.Identity == "Acme.Controls");
        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Entry && usage.Identity == "Dependent.Controls");
    }

    [Fact]
    public void Control_Package_Internal_Usage_Does_Not_Require_Its_Dependency_Entry()
    {
        var result = UsageGeneratorTestHost.Run(
            ["public static class Program { public static void Main() { Composite.Controls.Entry.UseCompositeControls(); } }"] ,
            [UsageGeneratorTestHost.AcmePackage, UsageGeneratorTestHost.CompositePackage],
            UsageGeneratorTestHost.LinkedEntryOptions);

        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK008");
    }

    [Fact]
    public void Explicit_Unit_And_Package_Roots_Emit_Stable_Usage()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "obj/AtomUIAxamlUsage.xml",
            """
            <AtomUIAxamlUsage Version="1"><Usage Source="&lt;Project&gt;" Line="0" Column="0" Kind="PackageRoot" NamespaceUri="" LocalName="" TypeName="" Identity="Acme.Controls" /><Usage Source="&lt;Project&gt;" Line="0" Column="0" Kind="UnitRoot" NamespaceUri="" LocalName="" TypeName="" Identity="Acme.Controls/DatePicker" /></AtomUIAxamlUsage>
            """);

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            additionalTexts: [axaml]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.UnitRoot && usage.Identity == "Acme.Controls/DatePicker");
        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Acme.Controls");
    }

    [Fact]
    public void Unsupported_Dynamic_Use_Warns_Without_Widening()
    {
        var roots = UsageGeneratorTestHost.Axaml(
            "obj/AtomUIAxamlUsage.xml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"&lt;Project&gt;\" Line=\"0\" Column=\"0\" Kind=\"PackageRoot\" NamespaceUri=\"\" LocalName=\"\" TypeName=\"\" Identity=\"Acme.Controls\" /></AtomUIAxamlUsage>");
        var result = UsageGeneratorTestHost.Run(
            ["using System; public sealed class Consumer { private Acme.Controls.DatePicker? _known; public object? Create(Type type) => Activator.CreateInstance(type); }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            UsageGeneratorTestHost.LinkedEntryOptions,
            additionalTexts: [roots]);

        // The only PackageRoot comes from the explicit AtomUIPackageRoot declaration;
        // the unresolvable dynamic creation site itself no longer widens the package.
        result.Usages.Count(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Acme.Controls").ShouldBe(1);
        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK010" && diagnostic.Severity == DiagnosticSeverity.Warning);
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK002");
    }

    [Fact]
    public void Unsupported_Dynamic_Use_With_Invoked_Entry_Warns_Without_A_Package_Root()
    {
        var result = UsageGeneratorTestHost.Run(
            ["using System; public sealed class Consumer { public static void Run() { Other.Controls.Entry.UseOtherControls(); } public object? Create(Type type) => Activator.CreateInstance(type); }"],
            [UsageGeneratorTestHost.AcmePackage, UsageGeneratorTestHost.OtherPackage],
            UsageGeneratorTestHost.LinkedEntryOptions);

        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK010" && diagnostic.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void Unsupported_Dynamic_Use_Without_Explicit_Package_Root_Is_An_Error()
    {
        var result = UsageGeneratorTestHost.Run(
            ["using System; public sealed class Consumer { private Acme.Controls.DatePicker? _known; public object? Create(Type type) => Activator.CreateInstance(type); }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK004" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
    }

    [Fact]
    public void Typeof_Dynamic_Creation_Is_Resolved_Without_Fallback()
    {
        var result = UsageGeneratorTestHost.Run(
            ["using System; public sealed class Consumer { public object? Create() => Activator.CreateInstance(typeof(Acme.Controls.DatePicker)); }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Control && usage.Identity == "Acme.Controls.DatePicker");
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
        result.Diagnostics.ShouldNotContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK002" || diagnostic.Id == "ATOMUILINK004");
    }

    [Fact]
    public void Local_Typeof_Dynamic_Creation_Is_Resolved_Without_Fallback()
    {
        var result = UsageGeneratorTestHost.Run(
            ["using System; public sealed class Consumer { public object? Create() { var type = typeof(Acme.Controls.DatePicker); return Activator.CreateInstance(type); } }"] ,
            [UsageGeneratorTestHost.AcmePackage]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.Control && usage.Identity == "Acme.Controls.DatePicker");
        result.Diagnostics.ShouldNotContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK002" || diagnostic.Id == "ATOMUILINK004");
    }

    [Fact]
    public void Axaml_Uncertainty_Widens_Only_The_Package_Resolved_From_The_Same_Source()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/DynamicView.axaml",
            """
            <AtomUIAxamlUsage Version="1"><Usage Source="Views/DynamicView.axaml" Line="2" Column="4" Kind="Element" NamespaceUri="using:Acme.Controls" LocalName="DatePicker" TypeName="Acme.Controls.DatePicker" Identity="" /><Uncertainty Source="Views/DynamicView.axaml" Line="5" Column="9" Reason="DynamicResourceSource" Value="{Binding Theme}" PackageId="" /></AtomUIAxamlUsage>
            """);

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [UsageGeneratorTestHost.AcmePackage, UsageGeneratorTestHost.OtherPackage],
            UsageGeneratorTestHost.LinkedEntryOptions,
            additionalTexts: [axaml]);

        result.Usages.ShouldContain(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Acme.Controls");
        result.Usages.ShouldNotContain(usage =>
            usage.Kind == LinkedUsageKind.PackageRoot && usage.Identity == "Other.Controls");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUILINK007");
    }

    [Fact]
    public void Unscoped_Axaml_Uncertainty_With_Ambiguous_Source_Is_A_Deterministic_Error()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/Ambiguous.axaml",
            """
            <AtomUIAxamlUsage Version="1"><Usage Source="Views/Ambiguous.axaml" Line="2" Column="4" Kind="Element" NamespaceUri="using:Acme.Controls" LocalName="DatePicker" TypeName="Acme.Controls.DatePicker" Identity="" /><Usage Source="Views/Ambiguous.axaml" Line="3" Column="4" Kind="Element" NamespaceUri="using:Other.Controls" LocalName="OtherPicker" TypeName="Other.Controls.OtherPicker" Identity="" /><Uncertainty Source="Views/Ambiguous.axaml" Line="5" Column="9" Reason="DynamicResourceSource" Value="{Binding Theme}" PackageId="" /></AtomUIAxamlUsage>
            """);

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [UsageGeneratorTestHost.AcmePackage, UsageGeneratorTestHost.OtherPackage],
            additionalTexts: [axaml]);

        result.Diagnostics.Count(diagnostic => diagnostic.Id == "ATOMUILINK004").ShouldBe(1);
        result.Diagnostics.Single(diagnostic => diagnostic.Id == "ATOMUILINK004")
              .Severity.ShouldBe(DiagnosticSeverity.Error);
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
    }

    [Fact]
    public void Explicitly_Scoped_Axaml_Uncertainty_Does_Not_Fall_Through_To_Another_Package()
    {
        var axaml = UsageGeneratorTestHost.Axaml(
            "Views/InvalidScope.axaml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"Views/InvalidScope.axaml\" Line=\"2\" Column=\"4\" Kind=\"Element\" NamespaceUri=\"using:Acme.Controls\" LocalName=\"DatePicker\" TypeName=\"Acme.Controls.DatePicker\" Identity=\"\" /><Uncertainty Source=\"Views/InvalidScope.axaml\" Line=\"5\" Column=\"9\" Reason=\"DynamicResourceSource\" Value=\"{Binding Theme}\" PackageId=\"Missing.Controls\" /></AtomUIAxamlUsage>");

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            additionalTexts: [axaml]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK004" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Usages.ShouldNotContain(usage => usage.Kind == LinkedUsageKind.PackageRoot);
    }

    [Fact]
    public void Strict_Mode_Promotes_Fallback_Warning_Without_Changing_Usage()
    {
        const string source = "using System; public sealed class Consumer { private Acme.Controls.DatePicker? _known; public object? Create(Type type) => Activator.CreateInstance(type); }";
        var roots = UsageGeneratorTestHost.Axaml(
            "obj/AtomUIAxamlUsage.xml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"&lt;Project&gt;\" Line=\"0\" Column=\"0\" Kind=\"PackageRoot\" NamespaceUri=\"\" LocalName=\"\" TypeName=\"\" Identity=\"Acme.Controls\" /></AtomUIAxamlUsage>");
        var warning = UsageGeneratorTestHost.Run(
            [source],
            [UsageGeneratorTestHost.AcmePackage],
            UsageGeneratorTestHost.LinkedEntryOptions,
            additionalTexts: [roots]);
        var strict = UsageGeneratorTestHost.Run(
            [source],
            [UsageGeneratorTestHost.AcmePackage],
            new Dictionary<string, string>
            {
                ["build_property.AtomUIRegistrationStrict"] = "true"
            },
            [roots]);

        warning.Usages.ShouldBe(strict.Usages);
        strict.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK010" && diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Syntax_Reference_Axaml_And_Root_Order_Do_Not_Change_Metadata_Bytes()
    {
        const string firstSource = "public sealed class First { public Acme.Controls.DatePicker? Picker { get; set; } }";
        const string secondSource = "public sealed class Second { public Other.Controls.OtherPicker? Picker { get; set; } }";
        var firstAxaml = UsageGeneratorTestHost.Axaml(
            "obj/roots-a.xml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"&lt;Project&gt;\" Line=\"0\" Column=\"0\" Kind=\"UnitRoot\" NamespaceUri=\"\" LocalName=\"\" TypeName=\"\" Identity=\"Acme.Controls/DatePicker\" /></AtomUIAxamlUsage>");
        var secondAxaml = UsageGeneratorTestHost.Axaml(
            "obj/roots-b.xml",
            "<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"&lt;Project&gt;\" Line=\"0\" Column=\"0\" Kind=\"PackageRoot\" NamespaceUri=\"\" LocalName=\"\" TypeName=\"\" Identity=\"Other.Controls\" /></AtomUIAxamlUsage>");

        var forward = UsageGeneratorTestHost.Run(
            [firstSource, secondSource],
            [UsageGeneratorTestHost.AcmePackage, UsageGeneratorTestHost.OtherPackage],
            additionalTexts: [firstAxaml, secondAxaml]);
        var reverse = UsageGeneratorTestHost.Run(
            [secondSource, firstSource],
            [UsageGeneratorTestHost.OtherPackage, UsageGeneratorTestHost.AcmePackage],
            additionalTexts: [secondAxaml, firstAxaml]);

        forward.GeneratedSource.ShouldBe(reverse.GeneratedSource);
    }

    [Fact]
    public void Absolute_Checkout_And_Reversed_Partial_Order_Do_Not_Change_CSharp_Source_Metadata()
    {
        var first = UsageGeneratorTestHost.RunSources(
            [
                new TestSource("/checkout/one/Controls/PartialPicker.Base.cs", "public partial class PartialPicker : Acme.Controls.DatePicker { }"),
                new TestSource("/checkout/one/Controls/PartialPicker.Other.cs", "public partial class PartialPicker { public Acme.Controls.DatePicker? Picker { get; set; } }")
            ],
            [UsageGeneratorTestHost.AcmePackage],
            new Dictionary<string, string>
            {
                ["build_property.ProjectDir"] = "/checkout/one/"
            });
        var second = UsageGeneratorTestHost.RunSources(
            [
                new TestSource("/different/root/Controls/PartialPicker.Other.cs", "public partial class PartialPicker { public Acme.Controls.DatePicker? Picker { get; set; } }"),
                new TestSource("/different/root/Controls/PartialPicker.Base.cs", "public partial class PartialPicker : Acme.Controls.DatePicker { }")
            ],
            [UsageGeneratorTestHost.AcmePackage],
            new Dictionary<string, string>
            {
                ["build_property.ProjectDir"] = "/different/root/"
            });

        first.GeneratedSource.ShouldBe(second.GeneratedSource);
        first.Usages.ShouldAllBe(usage => !Path.IsPathRooted(usage.Source));
    }

    [Fact]
    public void Windows_Project_Path_Normalization_Is_Case_Insensitive()
    {
        var result = UsageGeneratorTestHost.RunSources(
            [
                new TestSource(
                    "c:\\work\\consumer\\Controls\\PickerConsumer.cs",
                    "public sealed class PickerConsumer { public Acme.Controls.DatePicker Create() => new(); }")
            ],
            [UsageGeneratorTestHost.AcmePackage],
            new Dictionary<string, string>
            {
                ["build_property.ProjectDir"] = "C:\\WORK\\CONSUMER\\"
            });

        result.Usages.ShouldContain(
            usage => usage.Source == "Controls/PickerConsumer.cs",
            string.Join(", ", result.Usages.Select(static usage => usage.Source)));
    }

    [Theory]
    [InlineData("//server/share/consumer/Controls/PickerConsumer.cs", "//SERVER/SHARE/CONSUMER/", "Controls/PickerConsumer.cs")]
    [InlineData("//?/c:/work/consumer/Controls/PickerConsumer.cs", "//?/C:/WORK/CONSUMER/", "Controls/PickerConsumer.cs")]
    [InlineData("//?/unc/server/share/consumer/Controls/PickerConsumer.cs", "//?/UNC/SERVER/SHARE/CONSUMER/", "Controls/PickerConsumer.cs")]
    public void Windows_Network_And_Extended_Project_Paths_Are_Case_Insensitive(
        string sourcePath,
        string projectDirectory,
        string expectedSource)
    {
        var result = UsageGeneratorTestHost.RunSources(
            [
                new TestSource(
                    sourcePath,
                    "public sealed class PickerConsumer { public Acme.Controls.DatePicker Create() => new(); }")
            ],
            [UsageGeneratorTestHost.AcmePackage],
            new Dictionary<string, string>
            {
                ["build_property.ProjectDir"] = projectDirectory
            });

        result.Usages.ShouldContain(
            usage => usage.Source == expectedSource,
            string.Join(", ", result.Usages.Select(static usage => usage.Source)));
    }

    [Fact]
    public void Unknown_Manifest_Major_Version_Fails_Closed()
    {
        var reference = UsageGeneratorTestHost.CreateRawManifestReference(
            "Future.Manifest",
            ("AtomUI.Linked.ControlMap.v2", "2|Acme.Controls|Acme.Controls.DatePicker|Acme.Controls%2FDatePicker"));

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [reference]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK006" && diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Malformed_And_Incomplete_Manifests_Fail_Closed()
    {
        var malformed = UsageGeneratorTestHost.CreateRawManifestReference(
            "Malformed.Manifest",
            (LinkedRegistrationProtocol.ControlMapManifestKey, "1|Acme.Controls"));
        var incomplete = UsageGeneratorTestHost.CreateRawManifestReference(
            "Incomplete.Manifest",
            (LinkedRegistrationProtocol.UnitManifestKey, "1|Missing.Package"));

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [malformed, incomplete]);

        result.Diagnostics.Count(diagnostic => diagnostic.Id == "ATOMUILINK005").ShouldBeGreaterThanOrEqualTo(2);
    }

    [Theory]
    [InlineData(LinkedRegistrationProtocol.PackageManifestKey, "1||Acme.Controls||Acme.Full|Register||")]
    [InlineData(LinkedRegistrationProtocol.UnitManifestKey, "1|Acme.Controls||Acme.Unit|Register")]
    [InlineData(LinkedRegistrationProtocol.ControlMapManifestKey, "1|Acme.Controls||Acme.Controls%2FDatePicker")]
    [InlineData(LinkedRegistrationProtocol.UsageManifestKey, "1|Control||Consumer.cs|0|0")]
    public void Empty_Required_Manifest_Fields_Fail_Closed(string key, string value)
    {
        var reference = UsageGeneratorTestHost.CreateRawManifestReference(
            "Empty.Required.Manifest",
            (key, value));

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [reference]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Conflicting_Duplicate_Manifest_Records_Fail_Closed()
    {
        var reference = UsageGeneratorTestHost.CreateManifestReference(
            "Conflicting.Manifest",
            new LinkedPackageManifestRecord(
                "Conflict.Controls",
                "Conflicting.Manifest",
                "Conflict.Controls.Entry.UseControls",
                "Conflict.Controls.Full",
                "Register",
                null,
                null),
            new LinkedPackageManifestRecord(
                "Conflict.Controls",
                "Conflicting.Manifest",
                "Conflict.Controls.Entry.UseDifferentControls",
                "Conflict.Controls.Full",
                "Register",
                null,
                null),
            new LinkedUnitManifestRecord(
                "Conflict.Controls",
                "Conflict.Controls/Picker",
                "Conflict.Controls.PickerFragment",
                "Add"),
            new LinkedControlMapManifestRecord(
                "Conflict.Controls",
                "Conflict.Controls.Picker",
                "Conflict.Controls/Picker"),
            new LinkedControlMapManifestRecord(
                "Conflict.Controls",
                "Conflict.Controls.Picker",
                "Conflict.Controls/OtherPicker"));

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [reference]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Theory]
    [InlineData("Control", "Missing.Controls.Picker")]
    [InlineData("UnitRoot", "Missing.Controls/Picker")]
    [InlineData("PackageRoot", "Missing.Controls")]
    [InlineData("Entry", "Missing.Controls")]
    public void Referenced_Usage_Must_Resolve_To_Its_Manifest_Relationship(
        string kindName,
        string identity)
    {
        var kind = Enum.Parse<LinkedUsageKind>(kindName);
        var reference = UsageGeneratorTestHost.CreateManifestReference(
            "Invalid.Usage",
            new LinkedUsageManifestRecord(kind, identity, "Consumer.cs", 3, 6));

        var result = UsageGeneratorTestHost.Run(
            ["public static class Program { public static void Main() { } }"] ,
            [reference],
            UsageGeneratorTestHost.LinkedEntryOptions);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void Referenced_Control_Package_Usage_Is_Validated_Even_When_Excluded_From_Entry_Gating()
    {
        var reference = UsageGeneratorTestHost.CreateManifestReference(
            "Invalid.Package.Usage",
            new LinkedPackageManifestRecord(
                "InvalidPackage.Controls",
                "Invalid.Package.Usage",
                "InvalidPackage.Controls.Entry.UseControls",
                "InvalidPackage.Controls.Full",
                "Register",
                null,
                null),
            new LinkedUsageManifestRecord(
                LinkedUsageKind.Control,
                "Missing.Controls.Picker",
                "InternalControl.cs",
                3,
                6));

        var result = UsageGeneratorTestHost.Run(
            ["public static class Program { public static void Main() { } }"] ,
            [reference],
            UsageGeneratorTestHost.LinkedEntryOptions);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK008");
    }

    [Fact]
    public void Conflicting_Unit_Records_With_Different_Fragments_Fail_Closed()
    {
        var reference = UsageGeneratorTestHost.CreateManifestReference(
            "Conflicting.Units",
            new LinkedPackageManifestRecord(
                "Conflict.Controls",
                "Conflicting.Units",
                "Conflict.Controls.Entry.UseControls",
                "Conflict.Controls.Full",
                "Register",
                null,
                null),
            new LinkedUnitManifestRecord(
                "Conflict.Controls",
                "Conflict.Controls/Picker",
                "Conflict.Controls.PickerFragment",
                "Add"),
            new LinkedUnitManifestRecord(
                "Conflict.Controls",
                "Conflict.Controls/Picker",
                "Conflict.Controls.OtherPickerFragment",
                "Register"));

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [reference]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void UnitEdge_Must_Reference_Existing_Unit_Records()
    {
        var reference = UsageGeneratorTestHost.CreateManifestReference(
            "Missing.Unit",
            new LinkedPackageManifestRecord(
                "MissingUnit.Controls",
                "Missing.Unit",
                "Directory",
                "MissingUnit.Controls.Entry.UseControls",
                "MissingUnit.Controls.Full",
                "Register",
                null,
                null),
            new LinkedUnitManifestRecord(
                "MissingUnit.Controls",
                "MissingUnit.Controls/Picker",
                "MissingUnit.Controls.PickerFragment",
                "Add"),
            new LinkedUnitEdgeManifestRecord(
                "MissingUnit.Controls",
                "MissingUnit.Controls/Picker",
                "MissingUnit.Controls/Missing",
                LinkedUnitEdgeEvidenceKind.CSharpCall));

        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [reference]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK005" && diagnostic.Severity == DiagnosticSeverity.Error);
    }

    [Theory]
    [InlineData("<AtomUIAxamlUsage Version=\"1\"><Usage")]
    [InlineData("<AtomUIAxamlUsage Version=\"2\" />")]
    [InlineData("<WrongRoot Version=\"1\" />")]
    [InlineData("<AtomUIAxamlUsage Version=\"1\"><Unknown /></AtomUIAxamlUsage>")]
    [InlineData("<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"View.axaml\" Line=\"1\" Column=\"1\" Kind=\"FutureKind\" NamespaceUri=\"using:Acme.Controls\" LocalName=\"DatePicker\" TypeName=\"Acme.Controls.DatePicker\" Identity=\"\" /></AtomUIAxamlUsage>")]
    [InlineData("<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"View.axaml\" Line=\"-1\" Column=\"1\" Kind=\"Element\" NamespaceUri=\"using:Acme.Controls\" LocalName=\"DatePicker\" TypeName=\"Acme.Controls.DatePicker\" Identity=\"\" /></AtomUIAxamlUsage>")]
    [InlineData("<AtomUIAxamlUsage Version=\"1\"><Usage Source=\"View.axaml\" Line=\"1\" Column=\"1\" Kind=\"Element\" /></AtomUIAxamlUsage>")]
    [InlineData("")]
    [InlineData("   ")]
    public void Invalid_Structured_Axaml_Input_Fails_Closed(string content)
    {
        var result = UsageGeneratorTestHost.Run(
            ["public sealed class Consumer { }"] ,
            [UsageGeneratorTestHost.AcmePackage],
            additionalTexts: [UsageGeneratorTestHost.Axaml("obj/AtomUIAxamlUsage.xml", content)]);

        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUILINK006" && diagnostic.Severity == DiagnosticSeverity.Error);
        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK004");
    }
}

internal static class UsageGeneratorTestHost
{
    internal static readonly ImmutableArray<MetadataReference> PlatformReferences =
        ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
        .Split(Path.PathSeparator)
        .Select(static path => (MetadataReference)MetadataReference.CreateFromFile(path))
        .ToImmutableArray();
    private static readonly Dictionary<MetadataReference, TestAdditionalText> s_sidecars =
        new(ReferenceEqualityComparer.Instance);

    internal static readonly IReadOnlyDictionary<string, string> LinkedEntryOptions =
        new Dictionary<string, string>
        {
            ["build_property.AtomUILinkedPublish"] = "true",
            ["build_property.AtomUIRegistrationPlanOwner"] = "true"
        };

    internal static readonly MetadataReference AcmePackage = CreatePackageReference(
        "Acme.Controls",
        """
        [assembly: Avalonia.Metadata.XmlnsDefinition("https://acme.example/controls", "Acme.Controls")]
        [assembly: Avalonia.Metadata.XmlnsDefinition("https://shared.example/controls", "Acme.Controls")]
        namespace Avalonia.Metadata
        {
            [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
            public sealed class XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace) : System.Attribute { }
        }
        namespace Avalonia.Controls { public class Control { } }
        namespace Acme.Controls
        {
            public class DatePicker : Avalonia.Controls.Control
            {
                public static void SetIsOpen(Avalonia.Controls.Control control, bool value) { }
            }
            public class GenericControl<T> : Avalonia.Controls.Control { }
            public class UnmappedPicker : Avalonia.Controls.Control { }
            public class Helper { }
            public static class ThemeManagerBuilderExtensions { public static void UseDesktopControls() { } }
        }
        """,
        new LinkedControlMapManifestRecord("Acme.Controls", "Acme.Controls.DatePicker", "Acme.Controls/DatePicker"),
        new LinkedControlMapManifestRecord("Acme.Controls", "Acme.Controls.GenericControl`1", "Acme.Controls/GenericControl"));

    internal static readonly MetadataReference OtherPackage = CreatePackageReference(
        "Other.Controls",
        "[assembly: global::Avalonia.Metadata.XmlnsDefinition(\"https://shared.example/controls\", \"Other.Controls\")] namespace Other.Controls { public class OtherPicker : global::Avalonia.Controls.Control { } public static class Entry { public static void UseOtherControls() { } } }",
        [new LinkedControlMapManifestRecord("Other.Controls", "Other.Controls.OtherPicker", "Other.Controls/OtherPicker")],
        [AcmePackage],
        "Other.Controls.Entry.UseOtherControls",
        []);

    internal static readonly MetadataReference CompositePackage = CreatePackageReference(
        "Composite.Controls",
        "namespace Composite.Controls { public static class Entry { public static void UseCompositeControls() { } } }",
        [],
        [AcmePackage],
        "Composite.Controls.Entry.UseCompositeControls",
        [new LinkedUsageManifestRecord(
            LinkedUsageKind.Control,
            "Acme.Controls.DatePicker",
            "CompositeControl.cs",
            12,
            8)]);

    internal static UsageGeneratorExecution Run(
        IReadOnlyList<string> sources,
        IReadOnlyList<MetadataReference> references,
        IReadOnlyDictionary<string, string>? globalOptions = null,
        IReadOnlyList<TestAdditionalText>? additionalTexts = null,
        string assemblyName = "Consumer")
    {
        return RunSources(
            sources.Select(source => new TestSource(GetStableSourcePath(source), source)).ToArray(),
            references,
            globalOptions,
            additionalTexts,
            assemblyName);
    }

    internal static UsageGeneratorExecution RunSources(
        IReadOnlyList<TestSource> sources,
        IReadOnlyList<MetadataReference> references,
        IReadOnlyDictionary<string, string>? globalOptions = null,
        IReadOnlyList<TestAdditionalText>? additionalTexts = null,
        string assemblyName = "Consumer")
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var syntaxTrees = sources.Select(source =>
            CSharpSyntaxTree.ParseText(source.Content, parseOptions, source.Path)).ToArray();
        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees,
            PlatformReferences.Concat(references),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var files = GetSidecars(references)
            .Concat(additionalTexts ?? [])
            .GroupBy(static file => file.Path, StringComparer.Ordinal)
            .Select(static group => group.First())
            .ToImmutableArray();
        var options = new TestOptionsProvider(files, globalOptions);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LinkedRegistrationUsageGenerator().AsSourceGenerator()],
            files.Cast<AdditionalText>().ToImmutableArray(),
            parseOptions,
            options);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var driverDiagnostics,
            TestContext.Current.CancellationToken);
        var runResult = driver.GetRunResult().Results.ShouldHaveSingleItem();
        var generatedSource = runResult.GeneratedSources.ShouldHaveSingleItem().SourceText.ToString();
        var diagnostics = driverDiagnostics.Concat(runResult.Diagnostics).Distinct().ToArray();
        return new UsageGeneratorExecution(
            outputCompilation,
            GetUsageMetadata(outputCompilation.Assembly),
            diagnostics,
            generatedSource);
    }

    internal static MetadataReference CompileGeneratedReference(
        string assemblyName,
        IReadOnlyList<string> sources,
        IReadOnlyList<MetadataReference> references,
        IReadOnlyDictionary<string, string>? globalOptions = null)
    {
        var execution = Run(sources, references, globalOptions, assemblyName: assemblyName);
        using var stream = new MemoryStream();
        var emit = execution.OutputCompilation.Emit(stream);
        emit.Diagnostics.Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        emit.Success.ShouldBeTrue();
        var reference = MetadataReference.CreateFromImage(stream.ToArray());
        RegisterSidecar(reference, assemblyName, execution.Usages);
        return reference;
    }

    internal static TestAdditionalText Axaml(string path, string content)
    {
        return new TestAdditionalText(
            path,
            content,
            new Dictionary<string, string>
            {
                ["build_metadata.AdditionalFiles.AtomUIAxamlUsage"] = "true"
            });
    }

    internal static MetadataReference CreateRawManifestReference(
        string assemblyName,
        params (string Key, string Value)[] metadata)
    {
        var source = new StringBuilder();
        foreach (var item in metadata)
        {
            source.Append("[assembly: global::System.Reflection.AssemblyMetadata(")
                  .Append(SymbolDisplay.FormatLiteral(item.Key, quote: true))
                  .Append(", ")
                  .Append(SymbolDisplay.FormatLiteral(item.Value, quote: true))
                  .AppendLine(")] ");
        }
        return CreateSourceReference(assemblyName, source.ToString(), []);
    }

    internal static MetadataReference CreateManifestReference(
        string assemblyName,
        params LinkedRegistrationManifestRecord[] records)
    {
        var source = new StringBuilder();
        foreach (var record in records)
        {
            LinkedRegistrationMetadataWriter.Write(source, record);
        }
        var reference = CreateSourceReference(assemblyName, source.ToString(), []);
        RegisterSidecar(reference, assemblyName, records);
        return reference;
    }

    internal static ImmutableArray<TestAdditionalText> GetSidecars(
        IEnumerable<MetadataReference> references)
    {
        return references.Where(s_sidecars.ContainsKey)
            .Select(reference => s_sidecars[reference])
            .ToImmutableArray();
    }

    internal static IReadOnlyList<LinkedUsageManifestRecord> GetAssemblyMetadata(
        this MetadataReference reference)
    {
        var compilation = CSharpCompilation.Create(
            "MetadataReader",
            references: PlatformReferences.Add(reference),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var assembly = compilation.GetAssemblyOrModuleSymbol(reference).ShouldBeAssignableTo<IAssemblySymbol>();
        return GetUsageMetadata(assembly!);
    }

    private static IReadOnlyList<LinkedUsageManifestRecord> GetUsageMetadata(IAssemblySymbol assembly)
    {
        var usages = new List<LinkedUsageManifestRecord>();
        foreach (var attribute in assembly.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() != "System.Reflection.AssemblyMetadataAttribute" ||
                attribute.ConstructorArguments.Length != 2 ||
                attribute.ConstructorArguments[0].Value is not string key ||
                attribute.ConstructorArguments[1].Value is not string value ||
                key != LinkedRegistrationProtocol.UsageManifestKey)
            {
                continue;
            }

            LinkedRegistrationManifestCodec.TryDecode(key, value, out var record, out var error)
                .ShouldBeTrue(error);
            usages.Add(record.ShouldBeAssignableTo<LinkedUsageManifestRecord>()!);
        }
        return usages;
    }

    private static MetadataReference CreatePackageReference(
        string packageId,
        string source,
        params LinkedControlMapManifestRecord[] controlMaps)
    {
        return CreatePackageReference(
            packageId,
            source,
            controlMaps,
            [],
            "Acme.Controls.ThemeManagerBuilderExtensions.UseDesktopControls",
            []);
    }

    private static MetadataReference CreatePackageReference(
        string packageId,
        string source,
        IReadOnlyList<LinkedControlMapManifestRecord> controlMaps,
        IReadOnlyList<MetadataReference> references,
        string entry,
        IReadOnlyList<LinkedUsageManifestRecord> usages)
    {
        var metadata = new StringBuilder();
        var records = new List<LinkedRegistrationManifestRecord>();
        var package = new LinkedPackageManifestRecord(
            packageId,
            packageId,
            controlMaps.Count == 0 ? "Package" : "Directory",
            entry,
            packageId + ".Generated.Full",
            "Register",
            null,
            null);
        records.Add(package);
        LinkedRegistrationMetadataWriter.Write(
            metadata,
            package);
        foreach (var unit in controlMaps.Select(static controlMap =>
                     new LinkedUnitManifestRecord(
                         controlMap.PackageId,
                         controlMap.UnitId,
                         controlMap.PackageId + ".Generated.UnitFragment",
                         "Register")).Distinct())
        {
            LinkedRegistrationMetadataWriter.Write(metadata, unit);
            records.Add(unit);
        }
        foreach (var controlMap in controlMaps)
        {
            LinkedRegistrationMetadataWriter.Write(metadata, controlMap);
            records.Add(controlMap);
        }
        foreach (var usage in usages)
        {
            LinkedRegistrationMetadataWriter.Write(metadata, usage);
            records.Add(usage);
        }
        metadata.AppendLine(source);

        var reference = CreateSourceReference(packageId, metadata.ToString(), references);
        RegisterSidecar(reference, packageId, records);
        return reference;
    }

    private static void RegisterSidecar(
        MetadataReference reference,
        string assemblyName,
        IEnumerable<LinkedRegistrationManifestRecord> records)
    {
        var materialized = records.ToArray();
        var packageRecords = materialized.OfType<LinkedPackageManifestRecord>().ToArray();
        var unitRecords = materialized.OfType<LinkedUnitManifestRecord>().ToArray();
        var controlMaps = materialized.OfType<LinkedControlMapManifestRecord>().ToArray();
        var edges = materialized.OfType<LinkedUnitEdgeManifestRecord>().ToArray();
        var roots = materialized.OfType<LinkedRootUnitManifestRecord>().ToArray();
        var sidecar = new LinkedRegistrationSidecar
        {
            Producer = "AtomUI.Generator.Tests",
            Assembly = new LinkedSidecarAssembly
            {
                Name = assemblyName,
                TargetFramework = "net10.0"
            },
            Packages = packageRecords.Select(package => new LinkedSidecarPackage
            {
                Id = package.PackageId,
                AssemblyName = package.AssemblyName,
                Granularity = package.Granularity,
                EntryMethods = package.EntryMethodMetadataNames.Split(
                        new[] { ';' },
                        StringSplitOptions.RemoveEmptyEntries)
                    .Select(static entry => entry.Trim())
                    .Where(static entry => entry.Length != 0)
                    .ToArray(),
                FullFragment = new LinkedSidecarFragment
                {
                    Type = package.FullFragmentType,
                    Method = package.FullFragmentMethod
                },
                SharedFragment = package.PackageSharedFragmentType is null
                    ? null
                    : new LinkedSidecarFragment
                    {
                        Type = package.PackageSharedFragmentType,
                        Method = package.PackageSharedFragmentMethod!
                    },
                Units = unitRecords.Where(unit => unit.PackageId == package.PackageId)
                    .Select(unit => new LinkedSidecarUnit
                    {
                        Id = unit.UnitId,
                        FragmentType = unit.FragmentType,
                        FragmentMethod = unit.FragmentMethod,
                        OrderKey = unit.OrderKey,
                        Controls = controlMaps.Where(control =>
                                control.PackageId == package.PackageId &&
                                control.UnitId == unit.UnitId)
                            .Select(static control => control.MetadataName)
                            .ToArray()
                    })
                    .ToArray(),
                UnitEdges = edges.Where(edge => edge.PackageId == package.PackageId)
                    .Select(static edge => new LinkedSidecarUnitEdge
                    {
                        SourceUnitId = edge.SourceUnitId,
                        TargetUnitId = edge.TargetUnitId,
                        EvidenceKind = edge.EvidenceKind.ToString()
                    })
                    .ToArray(),
                RootUnits = roots.Where(root => root.PackageId == package.PackageId)
                    .Select(static root => root.UnitId)
                    .ToArray()
            }).ToArray(),
            Usages = materialized.OfType<LinkedUsageManifestRecord>()
                .Select(static usage => new LinkedSidecarUsage
                {
                    Kind = usage.Kind.ToString(),
                    Identity = usage.Identity,
                    Source = usage.Source,
                    Line = usage.Line,
                    Column = usage.Column
                })
                .ToArray(),
            Fallbacks = materialized.OfType<LinkedFallbackManifestRecord>()
                .Select(static fallback => new LinkedSidecarFallback
                {
                    PackageId = fallback.PackageId,
                    Reason = fallback.Reason,
                    Source = fallback.Source,
                    Line = fallback.Line,
                    Column = fallback.Column
                })
                .ToArray()
        };
        s_sidecars[reference] = new TestAdditionalText(
            assemblyName + ".atomui-link.json",
            Encoding.UTF8.GetString(LinkedRegistrationSidecarCodec.Write(sidecar)),
            new Dictionary<string, string>
            {
                ["build_metadata.AdditionalFiles.AtomUILinkedSidecar"] = "true"
            });
    }

    private static MetadataReference CreateSourceReference(
        string assemblyName,
        string source,
        IReadOnlyList<MetadataReference> references)
    {
        var compilation = CSharpCompilation.Create(
            assemblyName,
            [CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview))],
            PlatformReferences.Concat(references),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        using var stream = new MemoryStream();
        var emit = compilation.Emit(stream);
        emit.Diagnostics.Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
            .ShouldBeEmpty();
        emit.Success.ShouldBeTrue();
        return MetadataReference.CreateFromImage(stream.ToArray());
    }

    private static string GetStableSourcePath(string source)
    {
        unchecked
        {
            uint hash = 2166136261;
            foreach (var character in source)
            {
                hash = (hash ^ character) * 16777619;
            }
            return "Source-" + hash.ToString("x8", System.Globalization.CultureInfo.InvariantCulture) + ".cs";
        }
    }

    private sealed class TestOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private static readonly AnalyzerConfigOptions s_empty = new TestOptions(
            new Dictionary<string, string>());
        private readonly IReadOnlyDictionary<string, AnalyzerConfigOptions> _fileOptions;

        internal TestOptionsProvider(
            IReadOnlyList<TestAdditionalText> files,
            IReadOnlyDictionary<string, string>? globalOptions)
        {
            GlobalOptions = new TestOptions(globalOptions ?? new Dictionary<string, string>());
            _fileOptions = files.ToDictionary(
                static file => file.Path,
                static file => (AnalyzerConfigOptions)new TestOptions(file.Metadata),
                StringComparer.Ordinal);
        }

        public override AnalyzerConfigOptions GlobalOptions { get; }
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => s_empty;
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) =>
            _fileOptions.TryGetValue(textFile.Path, out var options) ? options : s_empty;
    }

    private sealed class TestOptions(IReadOnlyDictionary<string, string> values) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value) =>
            values.TryGetValue(key, out value!);
    }
}

internal sealed class TestAdditionalText(
    string path,
    string content,
    IReadOnlyDictionary<string, string> metadata) : AdditionalText
{
    public override string Path { get; } = path;
    internal IReadOnlyDictionary<string, string> Metadata { get; } = metadata;
    public override SourceText GetText(CancellationToken cancellationToken = default) =>
        SourceText.From(content, Encoding.UTF8);
}

internal sealed record UsageGeneratorExecution(
    Compilation OutputCompilation,
    IReadOnlyList<LinkedUsageManifestRecord> Usages,
    IReadOnlyList<Diagnostic> Diagnostics,
    string GeneratedSource);

internal sealed record TestSource(string Path, string Content);
