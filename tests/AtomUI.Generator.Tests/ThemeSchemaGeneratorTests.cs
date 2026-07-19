using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
        source.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Button\")");
        source.ShouldContain("new TokenDescriptor(\"Alpha\", 0, TokenStage.Seed");
        source.ShouldContain("new TokenDescriptor(\"Zeta\", 1, TokenStage.Alias");
        source.ShouldContain("new TokenDescriptor(\"Height\", 0, TokenStage.Control");
        source.ShouldContain("new TokenDescriptor(\"Label\", 1, TokenStage.Control");
        source.ShouldContain("ThemeTokenValueParser.Parse<global::System.Double>(value)");
        source.ShouldContain("ThemeTokenValueFormatter.Format((global::System.Double)value!)");
        source.ShouldContain("((global::Demo.ButtonToken)token).Height = (global::System.Double)value!");
        source.ShouldContain("static () => new global::Demo.ButtonToken()");
        source.ShouldContain("((global::Demo.ButtonToken)token).CalculateTokenValues(appearance == global::AtomUI.Theme.ThemeAppearance.Dark)");
        source.ShouldContain("new ThemeAlgorithmDescriptor(\"Dark\", 1, ThemeAppearanceEffect.Dark");
        source.ShouldContain("new ThemeAlgorithmDescriptor(\"Default\", 1, ThemeAppearanceEffect.Light");
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
    public void Generated_Namespace_Uses_One_Owner_Identifier_Instead_Of_Assembly_Name_Segments()
    {
        var compilation = CreateCompilation(TokenSource, "AtomUI.Desktop.Controls.DataGrid");
        var outputCompilation = RunGenerator(compilation, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        source.ShouldContain("namespace AtomUI.Generated.AtomUI_Desktop_Controls_DataGrid;");
    }

    private static CSharpCompilation RunGenerator(
        CSharpCompilation compilation,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var driver = CSharpGeneratorDriver.Create(new TokenResourceKeyGenerator());

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
        string assemblyName = "ThemeSchemaGeneratorTests")
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

    private const string TokenSource = """
        using AtomUI.Theme.Algorithms;
        using AtomUI.Theme.Schema;
        using AtomUI.Theme.TokenSystem;

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
                public const string ID = "Button";

                public ButtonToken()
                    : base(ID)
                {
                }

                public string Label { get; set; } = string.Empty;
                public double Height { get; set; }
            }

            [ThemeAlgorithm("Default", 1, ThemeAppearanceEffect.Light)]
            internal sealed class DefaultAlgorithm : IThemeAlgorithm
            {
                public DefaultAlgorithm()
                {
                }
            }

            [ThemeAlgorithm("Dark", 1, ThemeAppearanceEffect.Dark)]
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
        using AtomUI.Theme.TokenSystem;

        namespace Demo
        {
            [ThemeAlgorithm("Dark", 1, ThemeAppearanceEffect.Dark)]
            internal sealed class DarkAlgorithm : IThemeAlgorithm
            {
                public DarkAlgorithm()
                {
                }
            }

            [ThemeAlgorithm("Default", 1, ThemeAppearanceEffect.Light)]
            internal sealed class DefaultAlgorithm : IThemeAlgorithm
            {
                public DefaultAlgorithm()
                {
                }
            }

            [ControlDesignToken]
            internal sealed class ButtonToken : AbstractControlDesignToken
            {
                public const string ID = "Button";

                public ButtonToken()
                    : base(ID)
                {
                }

                public double Height { get; set; }
                public string Label { get; set; } = string.Empty;
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

    private const string AtomUIStubs = """
        namespace AtomUI.Theme
        {
            public readonly struct ControlTokenRegistration
            {
                public ControlTokenRegistration(System.Type tokenType, string tokenId, string? resourceCatalog)
                {
                }
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
                    System.Func<AtomUI.Theme.TokenSystem.AbstractDesignToken, object?> getter,
                    System.Action<AtomUI.Theme.TokenSystem.AbstractDesignToken, object?> setter,
                    System.Func<AtomUI.Theme.TokenSystem.AbstractDesignToken, object?> resourceProjector)
                {
                }
            }

            public sealed class ControlTokenDescriptor
            {
                public ControlTokenDescriptor(
                    ControlTokenIdentity identity,
                    System.Collections.Generic.IReadOnlyList<TokenDescriptor> ownTokens,
                    System.Func<AtomUI.Theme.TokenSystem.AbstractControlDesignToken> factory,
                    System.Action<AtomUI.Theme.TokenSystem.AbstractControlDesignToken, AtomUI.Theme.ThemeAppearance> evaluator)
                {
                }
            }

            public sealed class ThemeAlgorithmDescriptor
            {
                public ThemeAlgorithmDescriptor(
                    string id,
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
            public interface IThemeAlgorithm
            {
            }

            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
            public sealed class ThemeAlgorithmAttribute : System.Attribute
            {
                public ThemeAlgorithmAttribute(
                    string id,
                    int revision,
                    AtomUI.Theme.Schema.ThemeAppearanceEffect appearanceEffect)
                {
                }
            }
        }

        namespace AtomUI.Theme.TokenSystem
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

            public abstract class AbstractControlDesignToken : AbstractDesignToken
            {
                protected AbstractControlDesignToken(string id)
                {
                }

                public virtual void CalculateTokenValues(bool isDark)
                {
                }
            }
        }
        """;
}
