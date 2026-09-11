using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.Localization;

public class LanguageCatalogGeneratorTests
{
    [Fact]
    public void Accepts_A_Key_Based_Catalog_Contract()
    {
        var result = RunGenerator("""
            [LanguageCatalog]
            public enum LoginLangResourceKind
            {
                Title,
                SignIn,
                UserName
            }
            """);

        result.Diagnostics.ShouldNotContain(static diagnostic =>
            diagnostic.Id == "ATOMUILOC003" || diagnostic.Id == "ATOMUILOC004");
    }

    [Theory]
    [InlineData(
        "[LanguageCatalog] public sealed class LoginLangResourceKind { }",
        "ATOMUILOC003",
        "LoginLangResourceKind",
        "non-generic enum")]
    [InlineData(
        "[System.Flags, LanguageCatalog] public enum LoginLangResourceKind { Title }",
        "ATOMUILOC003",
        "LoginLangResourceKind",
        "Flags")]
    [InlineData(
        "[LanguageCatalog] public enum LoginLangResourceKind { }",
        "ATOMUILOC003",
        "LoginLangResourceKind",
        "at least one")]
    [InlineData(
        "[LanguageCatalog] internal enum LoginLangResourceKind { Title }",
        "ATOMUILOC003",
        "LoginLangResourceKind",
        "public")]
    public void Reports_Invalid_Catalog_Declarations(
        string declaration,
        string diagnosticId,
        string locationText,
        string messageFragment)
    {
        var diagnostic = RunGenerator(declaration).Diagnostics.ShouldHaveSingleItem();

        AssertDiagnostic(diagnostic, diagnosticId, locationText, messageFragment);
    }

    [Theory]
    [InlineData(
        "[LanguageCatalog] public enum LoginLangResourceKind { Title = 0 }",
        "0",
        "explicit numeric")]
    [InlineData(
        "[LanguageCatalog] public enum LoginLangResourceKind { Title = -1 }",
        "-1",
        "explicit numeric")]
    public void Reports_Explicit_Catalog_Unit_Values(
        string declaration,
        string locationText,
        string messageFragment)
    {
        var diagnostic = RunGenerator(declaration).Diagnostics.ShouldHaveSingleItem();

        AssertDiagnostic(diagnostic, "ATOMUILOC004", locationText, messageFragment);
    }

    [Fact]
    public void Reports_Explicit_Values_On_All_Catalog_Units()
    {
        var diagnostic = RunGenerator("""
            [LanguageCatalog]
            public enum LoginLangResourceKind
            {
                Title = 1,
                Heading = 1
            }
            """).Diagnostics;

        diagnostic.Length.ShouldBe(2);
        diagnostic.ShouldAllBe(item =>
            item.Id == "ATOMUILOC004" &&
            item.GetMessage().Contains("explicit numeric", StringComparison.Ordinal));
    }

    private static void AssertDiagnostic(
        Diagnostic diagnostic,
        string diagnosticId,
        string locationText,
        string messageFragment)
    {
        diagnostic.Id.ShouldBe(diagnosticId);
        diagnostic.Severity.ShouldBe(DiagnosticSeverity.Error);
        diagnostic.GetMessage().ShouldContain(messageFragment);
        diagnostic.Location.SourceTree.ShouldNotBeNull();
        diagnostic.Location.SourceTree!.GetText(TestContext.Current.CancellationToken)
                  .ToString(diagnostic.Location.SourceSpan)
                  .ShouldBe(locationText);
    }

    private static GeneratorRunResult RunGenerator(string declaration)
    {
        var compilation = CreateCompilation(declaration);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LocalizationGenerator().AsSourceGenerator()],
            parseOptions: (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider: new TestAnalyzerConfigOptionsProvider());
        driver = driver.RunGenerators(
            compilation,
            TestContext.Current.CancellationToken);
        return driver.GetRunResult().Results.ShouldHaveSingleItem();
    }

    private static CSharpCompilation CreateCompilation(string declaration)
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(static path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();
        var source = $$"""
            namespace AtomUI.Localization
            {
                [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
                public sealed class LanguageCatalogAttribute : System.Attribute;
            }

            namespace TestApp.Localization
            {
                using AtomUI.Localization;
                {{declaration}}
            }
            """;

        return CSharpCompilation.Create(
            "TestApp",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private static readonly AnalyzerConfigOptions s_empty = new TestAnalyzerConfigOptions(
            new Dictionary<string, string>());
        private static readonly AnalyzerConfigOptions s_global = new TestAnalyzerConfigOptions(
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.PackageId"] = "Test.Package",
                ["build_property.AssemblyName"] = "TestApp"
            });

        public override AnalyzerConfigOptions GlobalOptions => s_global;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => s_empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => s_empty;
    }

    private sealed class TestAnalyzerConfigOptions(
        IReadOnlyDictionary<string, string> values) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value)
        {
            return values.TryGetValue(key, out value!);
        }
    }
}
