using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests;

public class TokenResourceKeyGeneratorTests
{
    [Fact]
    public void Generates_Control_Shared_Token_Extension_And_Metadata_Registration()
    {
        var outputCompilation = RunGenerator(CreateCompilation("""
            using AtomUI.Theme.TokenSystem;

            namespace Demo
            {
                [ControlDesignToken]
                internal sealed class ButtonToken : AbstractControlDesignToken
                {
                    public const string ID = "Button";

                    public ButtonToken()
                        : base(ID)
                    {
                    }

                    public double Height { get; set; }

                    protected override System.Type GetTokenKindType()
                    {
                        return typeof(object);
                    }
                }
            }
            """), out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var tokenResources = GetGeneratedSource(outputCompilation, "TokenResourceConst.g.cs");
        tokenResources.ShouldContain("public sealed class ButtonTokenSharedTokenResourceExtension : ControlSharedTokenResourceExtension");
        tokenResources.ShouldContain("public ButtonTokenSharedTokenResourceExtension(SharedTokenKind kind)");
        tokenResources.ShouldContain(": base(\"AtomUI\", \"Button\", kind)");
        tokenResources.ShouldContain("public class ButtonTokenResourceExtension : TokenResourceExtension<ButtonTokenKind>");

        var descriptorPool = GetGeneratedSource(outputCompilation, "GeneratedThemeSchema.g.cs");
        descriptorPool.ShouldContain("new ControlTokenIdentity(\"AtomUI\", \"Button\")");
        descriptorPool.ShouldContain("static () => new global::Demo.ButtonToken()");
        descriptorPool.ShouldNotContain("DynamicDependency");
        descriptorPool.ShouldNotContain("typeof(global::Demo.ButtonToken)");
    }

    [Fact]
    public void Reports_Diagnostic_When_Control_Token_Id_Is_Missing()
    {
        RunGenerator(CreateCompilation("""
            using AtomUI.Theme.TokenSystem;

            namespace Demo
            {
                [ControlDesignToken]
                internal sealed class MissingIdToken : AbstractControlDesignToken
                {
                    public MissingIdToken()
                        : base("Missing")
                    {
                    }

                    public double Height { get; set; }

                    protected override System.Type GetTokenKindType()
                    {
                        return typeof(object);
                    }
                }
            }
            """), out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN005");
        diagnostic.GetMessage().ShouldContain("public const string ID");
    }

    [Fact]
    public void Reports_Diagnostic_When_Control_Token_Id_Is_Not_Constant()
    {
        RunGenerator(CreateCompilation("""
            using AtomUI.Theme.TokenSystem;

            namespace Demo
            {
                [ControlDesignToken]
                internal sealed class NonConstantIdToken : AbstractControlDesignToken
                {
                    public static readonly string ID = "NonConstant";

                    public NonConstantIdToken()
                        : base(ID)
                    {
                    }

                    public double Height { get; set; }

                    protected override System.Type GetTokenKindType()
                    {
                        return typeof(object);
                    }
                }
            }
            """), out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN006");
        diagnostic.GetMessage().ShouldContain("public const string");
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

    private static CSharpCompilation CreateCompilation(string source)
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();

        return CSharpCompilation.Create(
            "TokenResourceKeyGeneratorTests",
            [CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(AtomUIStubs)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private const string AtomUIStubs = """
        namespace AtomUI.Theme
        {
            public readonly struct ControlTokenRegistration
            {
                public ControlTokenRegistration(System.Type tokenType)
                {
                }

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

        namespace AtomUI.Theme.Resources
        {
            public class ControlSharedTokenResourceExtension
            {
                public ControlSharedTokenResourceExtension(string? catalog, string controlId, AtomUI.Theme.Styling.SharedTokenKind kind)
                {
                }
            }
        }

        namespace AtomUI.Theme.Styling
        {
            public enum SharedTokenKind
            {
                ColorPrimary
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
                    System.Action<AtomUI.Theme.TokenSystem.AbstractControlDesignToken, bool> evaluator)
                {
                }
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

        namespace AtomUI.Theme.TokenSystem
        {
            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
            public sealed class ControlDesignTokenAttribute : System.Attribute
            {
                public ControlDesignTokenAttribute(string catalog = "AtomUI")
                {
                }
            }

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

                protected abstract System.Type GetTokenKindType();

                public virtual void CalculateTokenValues(bool isDark)
                {
                }
            }
        }
        """;
}
