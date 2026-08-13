using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests;

public class TokenResourceKeyGeneratorTests
{
    [Fact]
    public void Does_Not_Generate_Theme_Sources_Without_Theme_Runtime_Contracts()
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(static path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();
        var compilation = CSharpCompilation.Create(
            "NonThemeAssembly",
            [CSharpSyntaxTree.ParseText(
                "namespace Demo; public sealed class PlainType { }",
                cancellationToken: TestContext.Current.CancellationToken)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var output = RunGenerator(compilation, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        output.SyntaxTrees.Count().ShouldBe(1);
    }

    [Fact]
    public void Publishes_Configured_Control_Catalog_As_Assembly_Metadata()
    {
        var outputCompilation = RunGenerator(
            CreateCompilation(string.Empty, "Acme.Controls"),
            out var diagnostics,
            new TestAnalyzerConfigOptionsProvider(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.AtomUIThemeControlCatalog"] = "Acme.Theme"
            }));

        diagnostics.ShouldBeEmpty();
        GetGeneratedSource(outputCompilation, "ThemeControlCatalogMetadata.g.cs")
            .ShouldContain(
                "[assembly: global::System.Reflection.AssemblyMetadata(\"AtomUIThemeControlCatalog\", \"Acme.Theme\")]");
    }

    [Fact]
    public void Uses_Configured_Control_Catalog_For_Third_Party_Package()
    {
        var compilation = CreateCompilation(
            """
            using Avalonia.Controls;

            namespace Acme.Controls
            {
                public sealed class Rating : Control
                {
                }
            }
            """,
            "Acme.Controls");
        var outputCompilation = RunGenerator(
            compilation,
            out var diagnostics,
            new TestAnalyzerConfigOptionsProvider(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.AtomUIThemeControlCatalog"] = "Acme.Controls"
            }),
            new InMemoryAdditionalText(
                "Themes/RatingTheme.axaml",
                """
                <ControlTheme xmlns="https://github.com/avaloniaui"
                              TargetType="Acme.Controls.Rating" />
                """));

        diagnostics.ShouldBeEmpty();
        GetGeneratedSource(outputCompilation, "TokenResourceConst.g.cs")
            .ShouldContain("new ControlTokenIdentity(\"Acme.Controls\", \"Rating\")");
        GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs")
            .ShouldContain("new ControlTokenIdentity(\"Acme.Controls\", \"Rating\")");
    }

    [Fact]
    public void Generates_Control_Token_Extension_And_Descriptor_Without_Legacy_Shared_Extension()
    {
        var outputCompilation = RunGenerator(CreateCompilation("""
            using Avalonia.Controls;
            using AtomUI.Theme.DesignTokens;

            namespace Demo
            {
                public sealed class Button : Control
                {
                }

                [ControlDesignToken]
                internal sealed class ButtonToken : AbstractControlDesignToken
                {
                    public double Height { get; set; }
                }
            }
            """), out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var tokenResources = GetGeneratedSource(outputCompilation, "TokenResourceConst.g.cs");
        tokenResources.ShouldContain("using AtomUI.Theme.Resources;");
        tokenResources.ShouldContain("public enum ButtonTokenKey");
        tokenResources.ShouldContain("ColorPrimary = (int)SharedTokenKind.ColorPrimary");
        tokenResources.ShouldContain("Height = -1");
        tokenResources.ShouldContain("public static class ButtonTokens");
        tokenResources.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Button\")");
        tokenResources.ShouldContain("public class ButtonTokenResourceExtension : TokenResourceExtension<ButtonTokenKey>");
        tokenResources.ShouldContain("ControlTokenResourceKey.Global(ButtonTokens.Identity, (SharedTokenKind)slot)");
        tokenResources.ShouldContain("return (ButtonTokenKind)ownSlot");
        tokenResources.ShouldContain("if ((uint)slot >= 2u)");
        tokenResources.ShouldContain("if ((uint)ownSlot < 1u)");
        tokenResources.ShouldNotContain("Enum.GetValues");
        tokenResources.ShouldNotContain("kind switch");
        tokenResources.ShouldNotContain("ButtonTokenKey.ColorPrimary =>");
        tokenResources.ShouldNotContain("ButtonTokenSharedTokenResourceExtension");
        tokenResources.ShouldNotContain("ControlSharedTokenResourceExtension");

        var descriptorPool = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        descriptorPool.ShouldContain("typeof(global::Demo.Button)");
        descriptorPool.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Button\")");
        descriptorPool.ShouldContain("static () => new global::Demo.ButtonToken()");
        descriptorPool.ShouldNotContain("DynamicDependency");
        descriptorPool.ShouldNotContain("typeof(global::Demo.ButtonToken)");
    }

    [Fact]
    public void Derives_Control_Identity_From_Token_Type_Name_Without_Manual_Id()
    {
        var outputCompilation = RunGenerator(CreateCompilation("""
            using Avalonia.Controls;
            using AtomUI.Theme.DesignTokens;

            namespace Demo
            {
                public sealed class Rating : Control
                {
                }

                [ControlDesignToken]
                internal sealed class RatingToken : AbstractControlDesignToken
                {
                    public double StarGap { get; set; }
                }
            }
            """), out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var descriptorPool = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        descriptorPool.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Rating\")");
        descriptorPool.ShouldContain("static () => new global::Demo.RatingToken()");
    }

    [Fact]
    public void Generates_Control_Identity_And_Resource_Extension_Without_Own_Token()
    {
        var compilation = CreateCompilation("""
            using Avalonia.Controls;

            namespace Demo
            {
                public sealed class Rating : Control
                {
                }
            }
            """);

        var outputCompilation = RunGenerator(
            compilation,
            out var diagnostics,
            new InMemoryAdditionalText(
                "Rating/Themes/RatingTheme.axaml",
                """
                <ControlTheme xmlns="https://github.com/avaloniaui"
                              xmlns:local="https://demo">
                    <Setter Property="MinHeight"
                            Value="{local:RatingTokenResource ControlHeight}" />
                </ControlTheme>
                """));

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var tokenResources = GetGeneratedSource(outputCompilation, "TokenResourceConst.g.cs");
        tokenResources.ShouldContain("public enum RatingTokenKey");
        tokenResources.ShouldContain("public static class RatingTokens");
        tokenResources.ShouldContain("public class RatingTokenResourceExtension");
        tokenResources.ShouldNotContain("RatingTokenKind");

        var schema = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        schema.ShouldContain("typeof(global::Demo.Rating)");
        schema.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Rating\")");
        schema.ShouldNotContain("new global::Demo.RatingToken()");
    }

    [Fact]
    public void Generates_Control_Identity_From_A_Top_Level_Themes_Directory()
    {
        var compilation = CreateCompilation("""
            using Avalonia.Controls;

            namespace Demo
            {
                public sealed class Rating : Control
                {
                }
            }
            """);

        var outputCompilation = RunGenerator(
            compilation,
            out var diagnostics,
            new InMemoryAdditionalText(
                "Themes/RatingTheme.axaml",
                """
                <ControlTheme xmlns="https://github.com/avaloniaui" />
                """));

        diagnostics.ShouldBeEmpty();
        var schema = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        schema.ShouldContain("typeof(global::Demo.Rating)");
        schema.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Rating\")");
    }

    [Fact]
    public void Prefers_Current_Assembly_Control_Over_Referenced_Avalonia_Control_With_The_Same_Name()
    {
        var outputCompilation = RunGenerator(
            CreateCompilationWithReferencedAvaloniaControls("""
                using Avalonia.Controls;

                namespace Demo
                {
                    public sealed class Button : Control
                    {
                    }
                }
                """),
            out var diagnostics,
            new InMemoryAdditionalText(
                "Button/Themes/ButtonTheme.axaml",
                """
                <ControlTheme xmlns="https://github.com/avaloniaui" />
                """));

        diagnostics.ShouldBeEmpty();

        var schema = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        schema.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Button\")");
    }

    [Fact]
    public void Reports_Diagnostic_When_Own_Token_Has_No_Matching_Control()
    {
        RunGenerator(CreateCompilation("""
            using AtomUI.Theme.DesignTokens;

            namespace Demo
            {
                [ControlDesignToken]
                internal sealed class MissingToken : AbstractControlDesignToken
                {
                }
            }
            """), out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN014");
        diagnostic.GetMessage().ShouldContain("matching public Control");
    }

    [Fact]
    public void Reports_Diagnostic_When_Control_Token_Name_Does_Not_Follow_Convention()
    {
        RunGenerator(CreateCompilation("""
            using AtomUI.Theme.DesignTokens;

            namespace Demo
            {
                [ControlDesignToken]
                internal sealed class RatingDesignValues : AbstractControlDesignToken
                {
                    public double StarGap { get; set; }
                }
            }
            """), out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN012");
        diagnostic.GetMessage().ShouldContain("must end with 'Token'");
    }

    [Fact]
    public void Reports_Diagnostic_When_Control_Token_Inherits_Another_Control_Token()
    {
        RunGenerator(CreateCompilation("""
            using AtomUI.Theme.DesignTokens;

            namespace Demo
            {
                internal class LineEditToken : AbstractControlDesignToken
                {
                    public double InputFontSize { get; set; }
                }

                [ControlDesignToken]
                internal sealed class ButtonSpinnerToken : LineEditToken
                {
                    public double HandleWidth { get; set; }
                }
            }
            """), out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN013");
        diagnostic.GetMessage().ShouldContain("cannot inherit Control Token");
    }

    [Fact]
    public void Reports_Diagnostic_When_Own_Token_Name_Conflicts_With_Global_Token()
    {
        RunGenerator(CreateCompilation("""
            using Avalonia.Controls;
            using AtomUI.Theme.DesignTokens;

            namespace Demo
            {
                public sealed class Alert : Control
                {
                }

                [ControlDesignToken]
                internal sealed class AlertToken : AbstractControlDesignToken
                {
                    public double ColorPrimary { get; set; }
                }
            }
            """), out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN019");
        diagnostic.GetMessage().ShouldContain("Own Token 'ColorPrimary'");
        diagnostic.GetMessage().ShouldContain("conflicts with a Global Token");
    }

    private static CSharpCompilation RunGenerator(
        CSharpCompilation compilation,
        out ImmutableArray<Diagnostic> diagnostics,
        params AdditionalText[] additionalTexts)
    {
        return RunGenerator(compilation, out diagnostics, null, additionalTexts);
    }

    private static CSharpCompilation RunGenerator(
        CSharpCompilation compilation,
        out ImmutableArray<Diagnostic> diagnostics,
        AnalyzerConfigOptionsProvider? optionsProvider,
        params AdditionalText[] additionalTexts)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var driver = CSharpGeneratorDriver.Create(
            [new TokenResourceKeyGenerator().AsSourceGenerator()],
            additionalTexts.ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider);

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
        string assemblyName = "TokenResourceKeyGeneratorTests")
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();

        return CSharpCompilation.Create(
            assemblyName,
            [CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(AtomUIStubs)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
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

    private static CSharpCompilation CreateCompilationWithReferencedAvaloniaControls(string source)
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();
        var avaloniaReference = CSharpCompilation.Create(
            "Avalonia.Controls",
            [CSharpSyntaxTree.ParseText("""
                namespace Avalonia.Controls
                {
                    public class Control
                    {
                    }

                    public class Button : Control
                    {
                    }
                }
                """)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .ToMetadataReference();
        var stubsWithoutAvaloniaControl = AtomUIStubs.Replace(
            """
            namespace Avalonia.Controls
            {
                public class Control
                {
                }
            }
            """,
            string.Empty,
            StringComparison.Ordinal);

        return CSharpCompilation.Create(
            "TokenResourceKeyGeneratorTests",
            [CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(stubsWithoutAvaloniaControl)],
            references.Add(avaloniaReference),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private const string AtomUIStubs = """
        namespace AtomUI.Theme
        {
            public interface IThemeManagerBuilder
            {
                void AddControlPackage(ControlPackageRegistration package);
            }

            public sealed class ControlPackageRegistration
            {
                public ControlPackageRegistration(
                    string id,
                    System.Collections.Generic.IEnumerable<AtomUI.Theme.Schema.ControlTokenDescriptor> controls,
                    System.Collections.Generic.IEnumerable<AtomUI.Theme.Schema.ControlThemeAssetDescriptor> assets,
                    AtomUI.Theme.Resources.IControlThemesProvider provider)
                {
                }
            }

            public readonly struct ControlTokenRegistration
            {
                public ControlTokenRegistration(System.Type tokenType)
                {
                }

                public ControlTokenRegistration(System.Type tokenType, string tokenId, string? resourceCatalog)
                {
                }
            }

        }

        namespace AtomUI.Theme.Resources
        {
            public interface IControlThemesProvider
            {
                string Id { get; }
            }

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

        namespace AtomUI.Registration
        {
            public sealed class AotTrimControlPackageRegistrationBuilder
            {
                public bool TryEnterUnit(string unitId) => true;

                public void AddControl(AtomUI.Theme.Schema.ControlTokenDescriptor descriptor)
                {
                }
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

        namespace AtomUI.Theme.Resources
        {
            public enum SharedTokenKind
            {
                ColorPrimary,
                ControlHeight
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
                public ControlTokenIdentity Identity { get; }

                public ControlTokenDescriptor(
                    System.Type controlType,
                    ControlTokenIdentity identity)
                {
                    Identity = identity;
                }

                public ControlTokenDescriptor(
                    System.Type controlType,
                    ControlTokenIdentity identity,
                    System.Collections.Generic.IReadOnlyList<TokenDescriptor> ownTokens,
                    System.Func<AtomUI.Theme.DesignTokens.AbstractControlDesignToken> factory,
                    System.Action<AtomUI.Theme.DesignTokens.AbstractControlDesignToken, AtomUI.Theme.ThemeAppearance> evaluator)
                {
                    Identity = identity;
                }
            }

            public sealed class ControlThemeAssetDescriptor
            {
                public ControlTokenIdentity OwnerIdentity { get; }
                public System.Collections.Generic.IReadOnlyList<ControlTokenIdentity> ReferencedControlIdentities { get; } =
                    System.Array.Empty<ControlTokenIdentity>();
            }

            public sealed class ThemeAlgorithmDescriptor
            {
            }

            public static class ThemeTokenValueParser
            {
                public static T Parse<T>(string value) => default!;
            }

            public static class ThemeTokenValueFormatter
            {
                public static string Format<T>(T value) => string.Empty;
            }

            public static class ThemeResourceValue
            {
                public static object? Project<T>(T value) => value;
            }
        }

        namespace AtomUI.Theme.DesignTokens
        {
            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
            public sealed class ControlDesignTokenAttribute : System.Attribute
            {
            }

            public sealed class NotTokenDefinitionAttribute : System.Attribute
            {
            }

            public abstract class AbstractDesignToken
            {
            }

            public abstract class AbstractControlDesignToken : AbstractDesignToken
            {
                public virtual void CalculateTokenValues(bool isDark)
                {
                }
            }
        }

        namespace Avalonia.Controls
        {
            public class Control
            {
            }
        }

        namespace AtomUI.Generated.TokenResourceKeyGeneratorTests
        {
            internal static class GeneratedControlThemeAssetManifest
            {
                internal static System.Collections.Generic.IReadOnlyList<AtomUI.Theme.Schema.ControlThemeAssetDescriptor> GetDescriptors()
                {
                    return System.Array.Empty<AtomUI.Theme.Schema.ControlThemeAssetDescriptor>();
                }
            }

            internal static class GeneratedControlThemeAssetResources
            {
                internal static void AddResources(
                    AtomUI.Theme.Resources.IControlThemesProvider provider,
                    System.Collections.Generic.IReadOnlyList<AtomUI.Theme.Schema.ControlThemeAssetDescriptor> assets)
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
}
