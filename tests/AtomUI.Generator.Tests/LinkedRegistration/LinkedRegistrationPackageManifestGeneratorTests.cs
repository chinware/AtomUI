extern alias LinkedPublish;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;
using LinkedRegistrationPackageManifestGenerator = LinkedPublish::AtomUI.Generator.LinkedRegistration.LinkedRegistrationPackageManifestGenerator;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class LinkedRegistrationPackageManifestGeneratorTests
{
    [Fact]
    public void Package_Core_Attached_Property_Calls_Do_Not_Root_The_Control_Unit()
    {
        var result = Run(
            new TestSource(
                "/repo/Tooltip/ToolTip.cs",
                """
                namespace Avalonia.Controls { public class Control { } }
                namespace Acme.Controls
                {
                    public sealed class ToolTip : Avalonia.Controls.Control
                    {
                        public static object? GetTip(Avalonia.Controls.Control control) => null;
                        public static void SetIsOpen(Avalonia.Controls.Control control, bool value) { }
                    }
                }
                """),
            new TestSource(
                "/repo/PackageCore/ToolTipService.cs",
                """
                namespace Acme.Controls
                {
                    internal sealed class ToolTipService
                    {
                        public object? Read(Avalonia.Controls.Control control) => ToolTip.GetTip(control);
                        public void Open(Avalonia.Controls.Control control) => ToolTip.SetIsOpen(control, true);
                    }
                }
                """));

        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK002");
        result.GeneratedSource.ShouldNotContain("AtomUI.Linked.RootUnit.v1");
        result.GeneratedSource.ShouldNotContain("AtomUI.Linked.Fallback.v1");
    }

    [Theory]
    [InlineData("public object Create() => new Acme.Controls.ToolTip();")]
    [InlineData("public System.Type GetTypeInfo() => typeof(Acme.Controls.ToolTip);")]
    public void Package_Core_Direct_Type_Evidence_Roots_The_Control_Unit(string evidence)
    {
        var result = Run(
            new TestSource(
                "/repo/Tooltip/ToolTip.cs",
                """
                namespace Avalonia.Controls { public class Control { } }
                namespace Acme.Controls { public class ToolTip : Avalonia.Controls.Control { } }
                """),
            new TestSource(
                "/repo/PackageCore/Bootstrap.cs",
                $$"""
                namespace Acme.Controls.PackageCore
                {
                    public class Bootstrap
                    {
                        {{evidence}}
                    }
                }
                """));

        result.GeneratedSource.ShouldContain("AtomUI.Linked.RootUnit.v1");
        result.GeneratedSource.ShouldContain("Acme.Controls%2FTooltip");
        result.GeneratedSource.ShouldNotContain("AtomUI.Linked.Fallback.v1");
    }

    [Fact]
    public void Package_Core_Control_Returning_Factory_Call_Roots_The_Result_Unit()
    {
        var result = Run(
            new TestSource(
                "/repo/Alert/Alert.cs",
                """
                namespace Avalonia.Controls { public class Control { } }
                namespace Acme.Controls
                {
                    public sealed class Alert : Avalonia.Controls.Control
                    {
                        public static Alert Create() => new Alert();
                    }
                }
                """),
            new TestSource(
                "/repo/PackageCore/Bootstrap.cs",
                """
                namespace Acme.Controls.PackageCore
                {
                    public sealed class Bootstrap
                    {
                        public Acme.Controls.Alert Create() => Acme.Controls.Alert.Create();
                    }
                }
                """));

        result.GeneratedSource.ShouldContain("AtomUI.Linked.RootUnit.v1");
        result.GeneratedSource.ShouldContain("Acme.Controls%2FAlert");
        result.GeneratedSource.ShouldNotContain("AtomUI.Linked.Fallback.v1");
    }

    [Fact]
    public void Package_Core_Generic_Value_Read_Does_Not_Become_A_Control_Factory_Root()
    {
        var result = Run(
            new TestSource(
                "/repo/Primitives/Control.cs",
                """
                namespace Avalonia.Controls
                {
                    public class Control
                    {
                        public T? GetValue<T>(object property) => default;
                    }
                }
                """),
            new TestSource(
                "/repo/Tooltip/ToolTip.cs",
                """
                namespace Acme.Controls
                {
                    public sealed class ToolTip : Avalonia.Controls.Control
                    {
                        public static readonly object ToolTipProperty = new object();
                    }
                }
                """),
            new TestSource(
                "/repo/PackageCore/ToolTipService.cs",
                """
                namespace Acme.Controls.PackageCore
                {
                    public sealed class ToolTipService
                    {
                        public Acme.Controls.ToolTip? Read(Avalonia.Controls.Control control) =>
                            control.GetValue<Acme.Controls.ToolTip>(Acme.Controls.ToolTip.ToolTipProperty);
                    }
                }
                """));

        result.GeneratedSource.ShouldNotContain("AtomUI.Linked.RootUnit.v1");
        result.GeneratedSource.ShouldNotContain("AtomUI.Linked.Fallback.v1");
    }

    [Fact]
    public void Cross_Unit_Call_Still_Emits_A_Direct_Call_Edge()
    {
        var result = Run(
            new TestSource(
                "/repo/Primitives/Control.cs",
                "namespace Avalonia.Controls { public class Control { } }"),
            new TestSource(
                "/repo/Alert/Alert.cs",
                """
                namespace Acme.Controls
                {
                    public sealed class Alert : Avalonia.Controls.Control
                    {
                        public static void Ping() { }
                    }
                }
                """),
            new TestSource(
                "/repo/Button/Button.cs",
                """
                namespace Acme.Controls
                {
                    public sealed class Button : Avalonia.Controls.Control
                    {
                        public void Run() => Alert.Ping();
                    }
                }
                """));

        result.GeneratedSource.ShouldContain("AtomUI.Linked.UnitEdge.v1");
        result.GeneratedSource.ShouldContain("Acme.Controls%2FButton");
        result.GeneratedSource.ShouldContain("Acme.Controls%2FAlert");
        result.GeneratedSource.ShouldContain("CSharpCall");
        result.GeneratedSource.ShouldNotContain("AtomUI.Linked.Fallback.v1");
    }

    [Fact]
    public void Interface_Dispatch_Is_Not_A_Registration_Dependency_Or_Fallback()
    {
        var result = Run(
            new TestSource(
                "/repo/Primitives/Contracts.cs",
                """
                namespace Avalonia.Controls { public class Control { } }
                namespace Acme.Controls.Primitives
                {
                    public interface IPart { void Run(); }
                    public sealed class PartControl : Avalonia.Controls.Control, IPart
                    {
                        public void Run() { }
                    }
                }
                """),
            new TestSource(
                "/repo/Button/Button.cs",
                """
                namespace Acme.Controls.Buttons
                {
                    public sealed class Button : Avalonia.Controls.Control
                    {
                        public void Run(Acme.Controls.Primitives.IPart part) => part.Run();
                    }
                }
                """));

        result.Diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK002");
        result.GeneratedSource.ShouldNotContain("AtomUI.Linked.Fallback.v1");
        var interfaceCallEdges = result.GeneratedSource.Split('\n').Where(static line =>
            line.Contains("AtomUI.Linked.UnitEdge.v1", StringComparison.Ordinal) &&
            line.Contains("CSharpCall", StringComparison.Ordinal)).ToArray();
        interfaceCallEdges.ShouldBeEmpty(string.Join(Environment.NewLine, interfaceCallEdges));
    }

    [Fact]
    public void Cross_Unit_Delegate_Dispatch_Remains_A_Package_Fallback()
    {
        var result = Run(
            new TestSource(
                "/repo/Primitives/Contracts.cs",
                """
                namespace Avalonia.Controls { public class Control { } }
                namespace Acme.Controls.Primitives
                {
                    public delegate void PartAction();
                    public sealed class PartControl : Avalonia.Controls.Control { }
                }
                """),
            new TestSource(
                "/repo/Button/Button.cs",
                """
                namespace Acme.Controls.Buttons
                {
                    public sealed class Button : Avalonia.Controls.Control
                    {
                        public void Run(Acme.Controls.Primitives.PartAction action) => action();
                    }
                }
                """));

        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUILINK002");
        result.GeneratedSource.ShouldContain("AtomUI.Linked.Fallback.v1");
    }

    private static PackageManifestGeneratorExecution Run(params TestSource[] sources)
    {
        var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        var compilation = CSharpCompilation.Create(
            "Acme.Controls",
            sources.Select(source => CSharpSyntaxTree.ParseText(
                source.Content,
                parseOptions,
                source.Path)),
            UsageGeneratorTestHost.PlatformReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LinkedRegistrationPackageManifestGenerator().AsSourceGenerator()],
            parseOptions: parseOptions,
            optionsProvider: new PackageOptionsProvider(new Dictionary<string, string>
            {
                ["build_property.AtomUIRegistrationPackageId"] = "Acme.Controls",
                ["build_property.AtomUIRegistrationGranularity"] = "Directory",
                ["build_property.AtomUILinkedPublish"] = "true",
                ["build_property.ProjectDir"] = "/repo/"
            }));
        driver = driver.RunGenerators(compilation, TestContext.Current.CancellationToken);
        var runResult = driver.GetRunResult().Results.ShouldHaveSingleItem();
        return new PackageManifestGeneratorExecution(
            runResult.GeneratedSources.ShouldHaveSingleItem().SourceText.ToString(),
            runResult.Diagnostics);
    }

    private sealed class PackageOptionsProvider(IReadOnlyDictionary<string, string> values)
        : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptions _options = new PackageOptions(values);
        public override AnalyzerConfigOptions GlobalOptions => _options;
        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => _options;
        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => _options;
    }

    private sealed class PackageOptions(IReadOnlyDictionary<string, string> values)
        : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value) =>
            values.TryGetValue(key, out value!);
    }

    private sealed record PackageManifestGeneratorExecution(
        string GeneratedSource,
        ImmutableArray<Diagnostic> Diagnostics);
}
