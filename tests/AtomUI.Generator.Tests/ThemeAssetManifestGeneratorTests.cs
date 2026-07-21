using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests;

public class ThemeAssetManifestGeneratorTests
{
    [Fact]
    public void Generates_One_Asset_Identity_And_Manifest_Entry()
    {
        var result = RunGenerator(
            [Asset("Themes/ButtonTheme.axaml", ValidButtonAsset)],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("public static class ButtonThemeAsset");
        source.ShouldContain("public static readonly global::AtomUI.Theme.Schema.ControlTokenIdentity Identity");
        source.ShouldContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"AtomUI\", \"Button\")");
        source.ShouldContain("new global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor(");
        source.ShouldContain("avares://ThemeAssetManifestTests/Themes/ButtonTheme.axaml");
        source.ShouldNotContain("\r");
    }

    [Fact]
    public void Allows_Global_Only_Asset_Without_Control_Identity()
    {
        var result = RunGenerator(
            [Asset("Themes/Global.axaml", "<ResourceDictionary>{atom:SharedTokenResource ColorPrimary}</ResourceDictionary>")],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        result.SyntaxTrees.ShouldNotContain(tree =>
            tree.FilePath.EndsWith("GeneratedControlThemeAssetManifest.g.cs", StringComparison.Ordinal));
    }

    [Fact]
    public void Allows_Explicit_Foreign_Control_Token_Dependencies_With_One_Shared_Scope()
    {
        RunGenerator(
            [Asset(
                "Themes/ButtonComposite.axaml",
                AssetWithIdentity(
                    "Button",
                    "{atom:SharedTokenResource ColorPrimary}{atom:ButtonTokenResource Height}{atom:AlertTokenResource Height}"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Reports_Missing_Unknown_Conflicting_And_Mismatched_Identities()
    {
        var assets = new AdditionalText[]
        {
            Asset("Themes/Missing.axaml", "<ResourceDictionary>{atom:SharedTokenResource ColorPrimary}{atom:ButtonTokenResource Height}</ResourceDictionary>"),
            Asset("Themes/Unknown.axaml", AssetWithIdentity("Missing", "{atom:SharedTokenResource ColorPrimary}")),
            Asset("Themes/Conflict.axaml", AssetWithTwoIdentities("Button", "Alert")),
            Asset("Themes/Mismatch.axaml", AssetWithIdentity("Button", "{atom:AlertTokenResource Height}"))
        };

        RunGenerator(assets, out var diagnostics);

        var diagnosticPaths = diagnostics.ToDictionary(
            static diagnostic => diagnostic.Id,
            static diagnostic => diagnostic.Location.GetLineSpan().Path);
        diagnosticPaths.Count.ShouldBe(4);
        diagnosticPaths["ATOMUIGEN007"].ShouldBe("Themes/Missing.axaml");
        diagnosticPaths["ATOMUIGEN008"].ShouldBe("Themes/Unknown.axaml");
        diagnosticPaths["ATOMUIGEN009"].ShouldBe("Themes/Conflict.axaml");
        diagnosticPaths["ATOMUIGEN010"].ShouldBe("Themes/Mismatch.axaml");
    }

    [Fact]
    public void Reports_Duplicate_Asset_Uris()
    {
        RunGenerator(
            [
                Asset("Themes/ButtonTheme.axaml", ValidButtonAsset),
                Asset("Themes/ButtonTheme.axaml", ValidButtonAsset)
            ],
            out var diagnostics);

        diagnostics.ShouldHaveSingleItem().Id.ShouldBe("ATOMUIGEN011");
    }

    private static CSharpCompilation RunGenerator(
        IEnumerable<AdditionalText> assets,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        var compilation = CreateCompilation();
        var driver = CSharpGeneratorDriver.Create(
            [new ThemeAssetManifestGenerator().AsSourceGenerator()],
            assets.ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            null);
        driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var output,
            out diagnostics,
            TestContext.Current.CancellationToken);
        return (CSharpCompilation)output;
    }

    private static CSharpCompilation CreateCompilation()
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(static path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();
        return CSharpCompilation.Create(
            "ThemeAssetManifestTests",
            [
                CSharpSyntaxTree.ParseText(TokenSource),
                CSharpSyntaxTree.ParseText(AtomUIStubs)
            ],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static string GetGeneratedSource(CSharpCompilation compilation, string fileName)
    {
        return compilation.SyntaxTrees
                          .Single(tree => tree.FilePath.EndsWith(fileName, StringComparison.Ordinal))
                          .GetText(TestContext.Current.CancellationToken)
                          .ToString();
    }

    private static AdditionalText Asset(string path, string text)
    {
        return new InMemoryAdditionalText(path, text);
    }

    private static string AssetWithIdentity(string identity, string content)
    {
        return $$"""
            <ResourceDictionary atom:ControlTokenScope.Identity="{x:Static generated:{{identity}}ThemeAsset.Identity}">
                {{content}}
            </ResourceDictionary>
            """;
    }

    private static string AssetWithTwoIdentities(string first, string second)
    {
        return $$"""
            <ResourceDictionary atom:ControlTokenScope.Identity="{x:Static generated:{{first}}ThemeAsset.Identity}">
                <ControlTheme atom:ControlTokenScope.Identity="{x:Static generated:{{second}}ThemeAsset.Identity}">
                    {atom:SharedTokenResource ColorPrimary}
                </ControlTheme>
            </ResourceDictionary>
            """;
    }

    private const string ValidButtonAsset = """
        <ResourceDictionary atom:ControlTokenScope.Identity="{x:Static generated:ButtonThemeAsset.Identity}">
            {atom:SharedTokenResource ColorPrimary}
            {atom:ButtonTokenResource Height}
        </ResourceDictionary>
        """;

    private const string TokenSource = """
        using AtomUI.Theme.DesignTokens;

        namespace Demo;

        [ControlDesignToken]
        internal sealed class ButtonToken : AbstractControlDesignToken
        {
            public const string ID = "Button";

            public ButtonToken() : base(ID)
            {
            }

            public double Height { get; set; }
        }

        [ControlDesignToken]
        internal sealed class AlertToken : AbstractControlDesignToken
        {
            public const string ID = "Alert";

            public AlertToken() : base(ID)
            {
            }

            public double Height { get; set; }
        }
        """;

    private const string AtomUIStubs = """
        namespace AtomUI.Theme.DesignTokens
        {
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public sealed class ControlDesignTokenAttribute : System.Attribute
            {
            }

            public abstract class AbstractControlDesignToken
            {
                protected AbstractControlDesignToken(string id)
                {
                }
            }
        }
        """;

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly SourceText _text;

        internal InMemoryAdditionalText(string path, string text)
        {
            Path  = path;
            _text = SourceText.From(text);
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            return _text;
        }
    }
}
