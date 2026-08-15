using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests;

public class ThemeSchemaGeneratorTests
{
    [Fact]
    public void Generates_Complete_Aot_Schema_Descriptors_With_Stable_Slots()
    {
        var outputCompilation = RunGenerator(CreateCompilation(TokenSource), out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var source = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");

        source.ShouldContain("namespace AtomUI.Generated.ThemeSchemaGeneratorTests;");
        source.ShouldContain("internal static class GeneratedThemeSchema");
        source.ShouldNotContain("namespace AtomUI.Theme.Schema;");
        source.ShouldNotContain("class ThemeSchemaDescriptorPool");
        source.ShouldContain("typeof(global::Demo.Button)");
        source.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Button\")");
        source.ShouldContain("global::AtomUI.Theme.Resources.SharedTokenKind");
        source.ShouldContain("new TokenDescriptor(\"Alpha\", 0, TokenStage.Seed");
        source.ShouldContain("new TokenDescriptor(\"Zeta\", 1, TokenStage.Alias");
        source.ShouldContain("new TokenDescriptor(\"Height\", 0, TokenStage.Control");
        source.ShouldContain("new TokenDescriptor(\"Label\", 1, TokenStage.Control");
        source.ShouldNotContain("SupportedGlobalToken");
        source.ShouldContain("ThemeTokenValueParser.Parse<global::System.Double>(value)");
        source.ShouldContain("ThemeTokenValueFormatter.Format((global::System.Double)value!)");
        source.ShouldContain("((global::Demo.ButtonToken)token).Height = (global::System.Double)value!");
        source.ShouldContain("static () => new global::Demo.ButtonToken()");
        source.ShouldContain("((global::Demo.ButtonToken)token).CalculateTokenValues(appearance == global::AtomUI.Theme.ThemeAppearance.Dark)");
        source.ShouldContain("new ThemeAlgorithmDescriptor(global::AtomUI.Theme.Algorithms.ThemeAlgorithm.Dark, 1, ThemeAppearanceEffect.Dark");
        source.ShouldContain("new ThemeAlgorithmDescriptor(global::AtomUI.Theme.Algorithms.ThemeAlgorithm.Default, 1, ThemeAppearanceEffect.Light");
        source.ShouldNotContain("Activator.CreateInstance");
        source.ShouldNotContain("PropertyInfo");
        source.ShouldNotContain("typeof(global::Demo.ButtonToken)");
    }

    [Fact]
    public void Descriptor_Output_Is_Independent_Of_Declaration_Order()
    {
        var first = GetGeneratedSource(
            RunGenerator(CreateCompilation(TokenSource), out _),
            "GeneratedThemeSchema.g.cs");
        var second = GetGeneratedSource(
            RunGenerator(CreateCompilation(ReorderedTokenSource), out _),
            "GeneratedThemeSchema.g.cs");

        second.ShouldBe(first);
    }

    [Fact]
    public void Ignores_An_Undefined_ThemeAlgorithm_Attribute_Value()
    {
        var invalidSource = TokenSource.Replace(
            "ThemeAlgorithm.Dark, 1, ThemeAppearanceEffect.Dark",
            "(ThemeAlgorithm)999, 1, ThemeAppearanceEffect.Dark",
            StringComparison.Ordinal);

        var outputCompilation = RunGenerator(CreateCompilation(invalidSource), out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        source.ShouldContain("ThemeAlgorithm.Default");
        source.ShouldNotContain("ThemeAlgorithm.Dark");
        source.ShouldNotContain("999");
    }

    [Theory]
    [InlineData("AtomUI.Desktop.Controls.DataGrid", "AtomUIDesktopControlsDataGrid")]
    [InlineData("acme-controls_package", "AcmeControlsPackage")]
    [InlineData("123.Controls", "Assembly123Controls")]
    public void Generated_Namespace_Uses_One_Pascal_Owner_Identifier_Without_Separators(
        string assemblyName,
        string expectedOwnerIdentifier)
    {
        var compilation = CreateCompilation(TokenSource, assemblyName);
        var outputCompilation = RunGenerator(compilation, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        source.ShouldContain($"namespace AtomUI.Generated.{expectedOwnerIdentifier};");
        source.ShouldNotContain("namespace AtomUI.Generated._");
    }

    [Fact]
    public void Ignores_Non_Token_Effective_Global_Calculation_Helpers()
    {
        var outputCompilation = RunGenerator(CreateCompilation("""
            using AtomUI.Theme.DesignTokens;

            namespace Demo
            {
                [ControlDesignToken]
                internal sealed class ButtonToken : AbstractControlDesignToken
                {
                    public double Height { get; set; }

                    public override void CalculateTokenValues(bool isDark)
                    {
                        _ = EffectiveGlobalToken.ColorPalettes.Count;
                        Height = EffectiveGlobalToken.ControlHeight;
                    }
                }
            }
            """), out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        source.ShouldNotContain("\"ColorPalettes\"");
    }

    [Fact]
    public void Generates_One_Package_Registration_Entry()
    {
        var outputCompilation = RunGenerator(
            CreateCompilation(TokenSource),
            out var diagnostics,
            new InMemoryAdditionalText(
                "Button/Themes/ButtonTheme.axaml",
                """
                <ControlTheme xmlns="https://github.com/avaloniaui"
                              xmlns:atom="https://atomui.net"
                              TargetType="Demo.Button">
                    <Setter Property="Tag" Value="{atom:ButtonTokenResource Height}" />
                </ControlTheme>
                """));

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(outputCompilation, "GeneratedControlPackageRegistration.g.cs");
        source.ShouldContain("internal static class GeneratedControlPackageRegistration");
        source.ShouldContain("internal static void Register(");
        source.ShouldContain("global::AtomUI.Theme.IThemeManagerBuilder themeManagerBuilder");
        source.ShouldContain("global::AtomUI.Theme.Resources.IControlThemesProvider controlThemesProvider");
        source.ShouldContain("global::System.Func<global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>, global::System.Collections.Generic.IReadOnlyList<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>>? selectAssets = null");
        source.ShouldContain("GeneratedThemeSchema.GetControls()");
        source.ShouldContain("GeneratedControlThemeAssetManifest.GetDescriptors()");
        source.ShouldContain("asset.ReferencedControlIdentities");
        source.ShouldContain("includeIdentity(referencedIdentity)");
        source.ShouldContain("var packageAssets = selectAssets is null ? selectedAssets : selectAssets(selectedAssets)");
        source.ShouldContain("GeneratedControlThemeAssetResources.AddResources(controlThemesProvider, packageAssets)");
        source.ShouldContain("new global::AtomUI.Theme.ControlPackageRegistration(");
        source.ShouldContain("            packageAssets,");
        source.ShouldContain("themeManagerBuilder.AddControlPackage(package)");
        source.ShouldNotContain("AddControlToken");
        source.ShouldNotContain("AddLanguageProviders");
    }

    [Fact]
    public void Generates_A_Linkable_Registration_Unit_Without_Referencing_The_Full_Schema()
    {
        var outputCompilation = RunGenerator(CreateCompilation(TokenSource), out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(
            outputCompilation,
            "GeneratedRegistrationUnits.g.cs");

        source.ShouldContain("namespace AtomUI.Generated.ThemeSchemaGeneratorTests.LinkedRegistrationV1;");
        source.ShouldContain("EditorBrowsableState.Never");
        source.ShouldContain("public static partial class GeneratedRegistrationUnit_Button_");
        source.ShouldContain("public static void Add(");
        source.ShouldContain("global::AtomUI.Registration.AotTrimControlPackageRegistrationBuilder builder");
        source.ShouldContain("builder.TryEnterUnit(\"ThemeSchemaGeneratorTests/Button\")");
        source.ShouldContain("builder.AddControl(");
        source.ShouldContain("typeof(global::Demo.Button)");
        source.ShouldContain("AssemblyMetadata(\"AtomUI.Linked.Unit.v1\"");
        source.ShouldContain("AssemblyMetadata(\"AtomUI.Linked.ControlMap.v1\"");
        source.ShouldNotContain("AssemblyMetadata(\"AtomUI.Linked.Control.v1\"");
        source.ShouldNotContain("GeneratedThemeSchema.GetControls()");
        source.ShouldNotContain("s_controls");
    }

    [Fact]
    public void Groups_Controls_From_The_Same_Source_Directory_Into_One_Unit()
    {
        var compilation = CreateCompilation(
            """
            namespace Demo;
            public sealed class DatePicker : Avalonia.Controls.Control { }
            """,
            "ThemeSchemaGeneratorTests",
            "DatePicker/DatePicker.cs").AddSyntaxTrees(
                CSharpSyntaxTree.ParseText(
                    """
                    namespace Demo;
                    public sealed class RangeDatePicker : Avalonia.Controls.Control { }
                    """,
                    path: "DatePicker/RangeDatePicker.cs",
                    cancellationToken: TestContext.Current.CancellationToken));
        var outputCompilation = RunGenerator(
            compilation,
            out var diagnostics,
            new InMemoryAdditionalText(
                "DatePicker/Themes/DatePickerTheme.axaml",
                "<ControlTheme xmlns=\"https://github.com/avaloniaui\" TargetType=\"Demo.DatePicker\" />"),
            new InMemoryAdditionalText(
                "DatePicker/Themes/RangeDatePickerTheme.axaml",
                "<ControlTheme xmlns=\"https://github.com/avaloniaui\" TargetType=\"Demo.RangeDatePicker\" />"));

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(outputCompilation, "GeneratedRegistrationUnits.g.cs");
        source.Split("public static partial class GeneratedRegistrationUnit_DatePicker_", StringSplitOptions.None)
              .Length.ShouldBe(2);
        source.ShouldContain("Demo.DatePicker");
        source.ShouldContain("Demo.RangeDatePicker");
        source.ShouldContain("typeof(global::Demo.DatePicker)");
        source.ShouldContain("typeof(global::Demo.RangeDatePicker)");
    }

    [Fact]
    public void Maps_Every_Public_Control_Without_Adding_A_Descriptor_For_Descriptorless_Controls()
    {
        var compilation = CreateCompilation(
            TokenSource,
            sourcePath: "Button/ButtonToken.cs").AddSyntaxTrees(
                CSharpSyntaxTree.ParseText(
                    """
                    namespace Demo;
                    public sealed class LayoutProbe : Avalonia.Controls.Control { }
                    """,
                    path: "Layout/LayoutProbe.cs",
                    cancellationToken: TestContext.Current.CancellationToken));

        var outputCompilation = RunGenerator(compilation, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var unitsSource = GetGeneratedSource(outputCompilation, "GeneratedRegistrationUnits.g.cs");
        unitsSource.ShouldContain("AssemblyMetadata(\"AtomUI.Linked.ControlMap.v1\"");
        unitsSource.ShouldContain("Demo.LayoutProbe");
        unitsSource.ShouldContain("ThemeSchemaGeneratorTests%2FLayout");
        unitsSource.ShouldContain("public static partial class GeneratedRegistrationUnit_Layout_");
        unitsSource.ShouldNotContain("typeof(global::Demo.LayoutProbe)");

        var schemaSource = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        schemaSource.ShouldNotContain("typeof(global::Demo.LayoutProbe)");
    }

    [Fact]
    public void Does_Not_Emit_Linked_Registration_Metadata_Without_A_Package_Entry()
    {
        var outputCompilation = RunGenerator(
            CreateCompilation(TokenSource),
            out var diagnostics,
            includeRegistrationEntry: false);

        diagnostics.ShouldBeEmpty();
        GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs")
            .ShouldContain("typeof(global::Demo.Button)");
        outputCompilation.SyntaxTrees.ShouldNotContain(tree =>
            tree.FilePath.EndsWith("GeneratedRegistrationUnits.g.cs", StringComparison.Ordinal));
        outputCompilation.SyntaxTrees.ShouldNotContain(tree =>
            tree.FilePath.EndsWith(
                "GeneratedFullControlPackageRegistrationFragment.g.cs",
                StringComparison.Ordinal));
    }

    [Fact]
    public void Does_Not_Claim_ControlMap_Ownership_For_A_Referenced_Control()
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();
        var avaloniaControls = CSharpCompilation.Create(
            "Avalonia.Controls",
            [CSharpSyntaxTree.ParseText(
                """
                namespace Avalonia.Controls;
                public class Control { }
                """,
                cancellationToken: TestContext.Current.CancellationToken)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .ToMetadataReference();
        var commonControls = CSharpCompilation.Create(
            "AtomUI.Controls",
            [CSharpSyntaxTree.ParseText(
                """
                [assembly: System.Reflection.AssemblyMetadata("AtomUIThemeControlCatalog", "AtomUI")]
                namespace AtomUI.Controls;
                public class SharedControl : Avalonia.Controls.Control { }
                """,
                cancellationToken: TestContext.Current.CancellationToken)],
            references.Add(avaloniaControls),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .ToMetadataReference();
        var compilation = CreateCompilation(
            """
            namespace Demo;
            public sealed class LocalControl : Avalonia.Controls.Control { }
            """,
            sourcePath: "Local/LocalControl.cs")
            .RemoveAllReferences()
            .AddReferences(references)
            .AddReferences(avaloniaControls, commonControls);

        var outputCompilation = RunGenerator(
            compilation,
            out var diagnostics,
            new InMemoryAdditionalText(
                "Local/Themes/SharedControlTheme.axaml",
                "<ControlTheme xmlns=\"https://github.com/avaloniaui\" TargetType=\"AtomUI.Controls.SharedControl\" />"));

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(outputCompilation, "GeneratedRegistrationUnits.g.cs");
        source.ShouldContain("typeof(global::AtomUI.Controls.SharedControl)");
        source.ShouldNotContain("AtomUI.Controls.SharedControl|ThemeSchemaGeneratorTests%2FLocal");
        source.ShouldContain("Demo.LocalControl");
    }

    [Fact]
    public void Generates_A_Public_Hidden_Full_Registration_Fragment()
    {
        var outputCompilation = RunGenerator(
            CreateCompilation(TokenSource),
            out var diagnostics,
            new InMemoryAdditionalText(
                "Button/Themes/ButtonTheme.axaml",
                "<ControlTheme xmlns=\"https://github.com/avaloniaui\" TargetType=\"Demo.Button\" />"));

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(
            outputCompilation,
            "GeneratedFullControlPackageRegistrationFragment.g.cs");

        source.ShouldContain("public static class GeneratedFullControlPackageRegistrationFragment");
        source.ShouldContain("EditorBrowsableState.Never");
        source.ShouldContain("GeneratedControlPackageRegistration.Register(");
        source.ShouldContain("AssemblyMetadata(\"AtomUI.Linked.Package.v1\"");
    }

    [Fact]
    public void Derives_Stable_Package_Entries_From_Annotated_Method_Symbols()
    {
        var compilation = CreateCompilation(TokenSource).AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(
                """
                using AtomUI.Registration;

                namespace Demo;

                public static class AlternateThemeManagerBuilderExtensions
                {
                    [ControlPackageRegistrationEntry]
                    public static AtomUI.IAtomUIBuilder UseAllControls(
                        this AtomUI.IAtomUIBuilder builder) => builder;
                }
                """,
                path: "AlternateThemeManagerBuilderExtensions.cs",
                cancellationToken: TestContext.Current.CancellationToken));

        var outputCompilation = RunGenerator(
            compilation,
            out var diagnostics,
            includeRegistrationEntry: true,
            new InMemoryAdditionalText(
                "Button/Themes/ButtonTheme.axaml",
                "<ControlTheme xmlns=\"https://github.com/avaloniaui\" TargetType=\"Demo.Button\" />"));

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(
            outputCompilation,
            "GeneratedFullControlPackageRegistrationFragment.g.cs");
        source.ShouldContain(
            "Demo.AlternateThemeManagerBuilderExtensions.UseAllControls%3BDemo.ThemeManagerBuilderExtensions.UseControls");
    }

    [Fact]
    public void Accepts_An_Entry_Return_Type_Assignable_To_IAtomUIBuilder()
    {
        var compilation = CreateCompilation(TokenSource).AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(
                """
                using AtomUI.Registration;

                namespace Demo;

                public interface ICustomAtomUIBuilder : AtomUI.IAtomUIBuilder
                {
                }

                public static class CustomThemeManagerBuilderExtensions
                {
                    [ControlPackageRegistrationEntry]
                    public static ICustomAtomUIBuilder UseCustomControls(
                        this AtomUI.IAtomUIBuilder builder) => (ICustomAtomUIBuilder)builder;
                }
                """,
                path: "CustomThemeManagerBuilderExtensions.cs",
                cancellationToken: TestContext.Current.CancellationToken));

        var outputCompilation = RunGenerator(
            compilation,
            out var diagnostics,
            includeRegistrationEntry: false,
            new InMemoryAdditionalText(
                "Button/Themes/ButtonTheme.axaml",
                "<ControlTheme xmlns=\"https://github.com/avaloniaui\" TargetType=\"Demo.Button\" />"));

        diagnostics.ShouldBeEmpty();
        GetGeneratedSource(
                outputCompilation,
                "GeneratedFullControlPackageRegistrationFragment.g.cs")
            .ShouldContain("Demo.CustomThemeManagerBuilderExtensions.UseCustomControls");
    }

    [Theory]
    [InlineData(
        "public static AtomUI.IAtomUIBuilder UseControls(AtomUI.IAtomUIBuilder builder) => builder;",
        "extension method")]
    [InlineData(
        "public static T UseControls<T>(this T builder) where T : AtomUI.IAtomUIBuilder => builder;",
        "non-generic")]
    [InlineData(
        "internal static AtomUI.IAtomUIBuilder UseControls(this AtomUI.IAtomUIBuilder builder) => builder;",
        "public")]
    [InlineData(
        "public static object UseControls(this AtomUI.IAtomUIBuilder builder) => builder;",
        "assignable to IAtomUIBuilder")]
    public void Reports_Invalid_Package_Entry_Declarations_In_The_Producer(
        string methodDeclaration,
        string expectedReason)
    {
        var compilation = CreateCompilation(TokenSource).AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(
                $$"""
                using AtomUI.Registration;

                namespace Demo;

                public static class InvalidThemeManagerBuilderExtensions
                {
                    [ControlPackageRegistrationEntry]
                    {{methodDeclaration}}
                }
                """,
                path: "InvalidThemeManagerBuilderExtensions.cs",
                cancellationToken: TestContext.Current.CancellationToken));

        RunGenerator(
            compilation,
            out var diagnostics,
            includeRegistrationEntry: false);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUILINK009");
        diagnostic.GetMessage().ShouldContain(expectedReason);
        diagnostic.Location.GetLineSpan().Path.ShouldBe("InvalidThemeManagerBuilderExtensions.cs");
    }

    [Fact]
    public void Rejects_Overloaded_Annotated_Entry_Names()
    {
        var compilation = CreateCompilation(TokenSource).AddSyntaxTrees(
            CSharpSyntaxTree.ParseText(
                """
                using AtomUI.Registration;

                namespace Demo;

                public static class OverloadedThemeManagerBuilderExtensions
                {
                    [ControlPackageRegistrationEntry]
                    public static AtomUI.IAtomUIBuilder UseControls(
                        this AtomUI.IAtomUIBuilder builder) => builder;

                    public static AtomUI.IAtomUIBuilder UseControls(
                        this AtomUI.IAtomUIBuilder builder,
                        bool full) => builder;
                }
                """,
                path: "OverloadedThemeManagerBuilderExtensions.cs",
                cancellationToken: TestContext.Current.CancellationToken));

        RunGenerator(
            compilation,
            out var diagnostics,
            includeRegistrationEntry: false);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUILINK009");
        diagnostic.GetMessage().ShouldContain("overloaded");
    }

    private static CSharpCompilation RunGenerator(
        CSharpCompilation compilation,
        out ImmutableArray<Diagnostic> diagnostics,
        params AdditionalText[] additionalTexts)
    {
        return RunGenerator(
            compilation,
            out diagnostics,
            includeRegistrationEntry: true,
            additionalTexts);
    }

    private static CSharpCompilation RunGenerator(
        CSharpCompilation compilation,
        out ImmutableArray<Diagnostic> diagnostics,
        bool includeRegistrationEntry,
        params AdditionalText[] additionalTexts)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        if (includeRegistrationEntry)
        {
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(
                RegistrationEntrySource,
                path: "ThemeManagerBuilderExtensions.cs",
                cancellationToken: cancellationToken));
        }
        var driver = CSharpGeneratorDriver.Create(
            [new TokenResourceKeyGenerator().AsSourceGenerator()],
            additionalTexts.ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider: null);

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out diagnostics, cancellationToken);
        return (CSharpCompilation)outputCompilation;
    }

    private static string GetGeneratedSource(CSharpCompilation compilation, string fileName)
    {
        return compilation.SyntaxTrees
                          .Single(tree => tree.FilePath.EndsWith(fileName, StringComparison.Ordinal))
                          .GetText(TestContext.Current.CancellationToken)
                          .ToString();
    }

    private static CSharpCompilation CreateCompilation(
        string source,
        string assemblyName = "ThemeSchemaGeneratorTests",
        string sourcePath = "TokenSource.cs")
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();

        return CSharpCompilation.Create(
            assemblyName,
            [CSharpSyntaxTree.ParseText(source, path: sourcePath), CSharpSyntaxTree.ParseText(AtomUIStubs)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private const string TokenSource = """
        using AtomUI.Theme.Algorithms;
        using AtomUI.Theme.Schema;
        using AtomUI.Theme.DesignTokens;

        namespace Demo
        {
            [GlobalDesignToken]
            public partial class DesignToken : AbstractDesignToken
            {
                [DesignTokenKind(DesignTokenKind.Alias)]
                public string Zeta { get; set; } = string.Empty;

                [DesignTokenKind(DesignTokenKind.Seed)]
                public double Alpha { get; set; }
            }

            [ControlDesignToken]
            internal sealed class ButtonToken : AbstractControlDesignToken
            {
                public string Label { get; set; } = string.Empty;
                public double Height { get; set; }

                public override void CalculateTokenValues(bool isDark)
                {
                    Height = EffectiveGlobalToken.ControlHeight;
                }
            }

            [ThemeAlgorithm(ThemeAlgorithm.Default, 1, ThemeAppearanceEffect.Light)]
            internal sealed class DefaultAlgorithm : IThemeAlgorithm
            {
                public DefaultAlgorithm()
                {
                }
            }

            [ThemeAlgorithm(ThemeAlgorithm.Dark, 1, ThemeAppearanceEffect.Dark)]
            internal sealed class DarkAlgorithm : IThemeAlgorithm
            {
                public DarkAlgorithm()
                {
                }
            }
        }
        """;

    private const string ReorderedTokenSource = """
        using AtomUI.Theme.Algorithms;
        using AtomUI.Theme.Schema;
        using AtomUI.Theme.DesignTokens;

        namespace Demo
        {
            [ThemeAlgorithm(ThemeAlgorithm.Dark, 1, ThemeAppearanceEffect.Dark)]
            internal sealed class DarkAlgorithm : IThemeAlgorithm
            {
                public DarkAlgorithm()
                {
                }
            }

            [ThemeAlgorithm(ThemeAlgorithm.Default, 1, ThemeAppearanceEffect.Light)]
            internal sealed class DefaultAlgorithm : IThemeAlgorithm
            {
                public DefaultAlgorithm()
                {
                }
            }

            [ControlDesignToken]
            internal sealed class ButtonToken : AbstractControlDesignToken
            {
                public double Height { get; set; }
                public string Label { get; set; } = string.Empty;

                public override void CalculateTokenValues(bool isDark)
                {
                    Height = EffectiveGlobalToken.ControlHeight;
                }
            }

            [GlobalDesignToken]
            public partial class DesignToken : AbstractDesignToken
            {
                [DesignTokenKind(DesignTokenKind.Seed)]
                public double Alpha { get; set; }

                [DesignTokenKind(DesignTokenKind.Alias)]
                public string Zeta { get; set; } = string.Empty;
            }
        }
        """;

    private const string RegistrationEntrySource = """
        using AtomUI.Registration;

        namespace Demo;

        public static class ThemeManagerBuilderExtensions
        {
            [ControlPackageRegistrationEntry]
            public static AtomUI.IAtomUIBuilder UseControls(
                this AtomUI.IAtomUIBuilder builder) => builder;
        }
        """;

    private const string AtomUIStubs = """
        namespace Avalonia.Controls
        {
            public class Control
            {
            }
        }

        namespace Demo
        {
            public sealed class Button : Avalonia.Controls.Control
            {
            }
        }

        namespace AtomUI.Theme
        {
            public readonly struct ControlTokenRegistration
            {
                public ControlTokenRegistration(System.Type tokenType, string tokenId, string? resourceCatalog)
                {
                }
            }

        }

        namespace AtomUI.Registration
        {
            [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false)]
            public sealed class ControlPackageRegistrationEntryAttribute : System.Attribute
            {
            }

            public sealed class AotTrimControlPackageRegistrationBuilder
            {
                public bool TryEnterUnit(string unitId) => true;

                public void AddControl(AtomUI.Theme.Schema.ControlTokenDescriptor descriptor)
                {
                }
            }
        }

        namespace AtomUI
        {
            public interface IAtomUIBuilder
            {
            }
        }

        namespace AtomUI.Theme.Resources
        {
            public abstract class TokenResourceExtension<TTokenKind>
                where TTokenKind : System.Enum
            {
                protected TokenResourceExtension()
                {
                }

                protected TokenResourceExtension(TTokenKind kind)
                {
                }

                protected virtual object GetResourceKey(TTokenKind kind) => kind;
            }

            public static class ControlTokenResourceKey
            {
                public static object Global(
                    AtomUI.Theme.Schema.ControlTokenIdentity identity,
                    SharedTokenKind kind) => kind;
            }
        }

        namespace AtomUI.Theme
        {
            public enum ThemeAppearance : byte
            {
                Light,
                Dark
            }
        }

        namespace AtomUI.Theme.Schema
        {
            public enum TokenStage : byte
            {
                Seed,
                Map,
                Alias,
                Control
            }

            public enum ThemeAppearanceEffect : byte
            {
                Preserve,
                Light,
                Dark
            }

            public readonly record struct ControlTokenIdentity(string Catalog, string Id);

            public sealed class TokenDescriptor
            {
                public TokenDescriptor(
                    string name,
                    int slot,
                    TokenStage stage,
                    System.Type valueType,
                    object resourceKey,
                    System.Func<string, object?> parser,
                    System.Func<object?, string> formatter,
                    System.Func<AtomUI.Theme.DesignTokens.AbstractDesignToken, object?> getter,
                    System.Action<AtomUI.Theme.DesignTokens.AbstractDesignToken, object?> setter,
                    System.Func<AtomUI.Theme.DesignTokens.AbstractDesignToken, object?> resourceProjector)
                {
                }
            }

            public sealed class ControlTokenDescriptor
            {
                public ControlTokenDescriptor(
                    System.Type controlType,
                    ControlTokenIdentity identity,
                    System.Collections.Generic.IReadOnlyList<TokenDescriptor> ownTokens,
                    System.Func<AtomUI.Theme.DesignTokens.AbstractControlDesignToken> factory,
                    System.Action<AtomUI.Theme.DesignTokens.AbstractControlDesignToken, AtomUI.Theme.ThemeAppearance> evaluator)
                {
                }
            }

            public sealed class ThemeAlgorithmDescriptor
            {
                public ThemeAlgorithmDescriptor(
                    AtomUI.Theme.Algorithms.ThemeAlgorithm algorithm,
                    int revision,
                    ThemeAppearanceEffect appearanceEffect,
                    System.Func<AtomUI.Theme.Algorithms.IThemeAlgorithm> factory)
                {
                }
            }

            public static class ThemeTokenValueParser
            {
                public static T Parse<T>(string value)
                {
                    return default!;
                }
            }

            public static class ThemeTokenValueFormatter
            {
                public static string Format<T>(T value)
                {
                    return string.Empty;
                }
            }

            public static class ThemeResourceValue
            {
                public static object? Project<T>(T value)
                {
                    return value;
                }
            }
        }

        namespace AtomUI.Theme.Algorithms
        {
            public enum ThemeAlgorithm
            {
                Default = 0,
                Dark = 1,
                Compact = 2
            }

            public interface IThemeAlgorithm
            {
            }

            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
            public sealed class ThemeAlgorithmAttribute : System.Attribute
            {
                public ThemeAlgorithmAttribute(
                    ThemeAlgorithm algorithm,
                    int revision,
                    AtomUI.Theme.Schema.ThemeAppearanceEffect appearanceEffect)
                {
                }
            }
        }

        namespace AtomUI.Theme.DesignTokens
        {
            public enum DesignTokenKind
            {
                Seed,
                Map,
                Alias
            }

            [System.AttributeUsage(System.AttributeTargets.Property)]
            public sealed class DesignTokenKindAttribute : System.Attribute
            {
                public DesignTokenKindAttribute(DesignTokenKind kind)
                {
                }
            }

            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
            public sealed class GlobalDesignTokenAttribute : System.Attribute
            {
            }

            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
            public sealed class ControlDesignTokenAttribute : System.Attribute
            {
            }

            [System.AttributeUsage(System.AttributeTargets.Property)]
            public sealed class NotTokenDefinitionAttribute : System.Attribute
            {
            }

            public abstract class AbstractDesignToken
            {
            }

            public sealed class DesignToken : AbstractDesignToken
            {
                [NotTokenDefinition]
                public System.Collections.Generic.IDictionary<string, string> ColorPalettes { get; } =
                    new System.Collections.Generic.Dictionary<string, string>();

                public double ControlHeight { get; set; }
            }

            public abstract class AbstractControlDesignToken : AbstractDesignToken
            {
                protected DesignToken EffectiveGlobalToken { get; } = new DesignToken();

                public virtual void CalculateTokenValues(bool isDark)
                {
                }
            }
        }
        """;

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly Microsoft.CodeAnalysis.Text.SourceText _text;

        internal InMemoryAdditionalText(string path, string text)
        {
            Path = path;
            _text = Microsoft.CodeAnalysis.Text.SourceText.From(text);
        }

        public override string Path { get; }

        public override Microsoft.CodeAnalysis.Text.SourceText GetText(
            CancellationToken cancellationToken = default)
        {
            return _text;
        }
    }

    private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private static readonly AnalyzerConfigOptions s_empty =
            new TestAnalyzerConfigOptions(new Dictionary<string, string>());
        private readonly AnalyzerConfigOptions _globalOptions;

        internal TestAnalyzerConfigOptionsProvider(IReadOnlyDictionary<string, string> globalOptions)
        {
            _globalOptions = new TestAnalyzerConfigOptions(globalOptions);
        }

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => s_empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => s_empty;
    }

    private sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly IReadOnlyDictionary<string, string> _values;

        internal TestAnalyzerConfigOptions(IReadOnlyDictionary<string, string> values)
        {
            _values = values;
        }

        public override bool TryGetValue(string key, out string value)
        {
            return _values.TryGetValue(key, out value!);
        }
    }
}
