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
    public void Accepts_A_Stable_Explicit_Catalog_Contract()
    {
        var result = RunGenerator("""
            [LanguageCatalog(ContractVersion = 2)]
            public enum LoginLangResourceKind
            {
                Title = 10,
                SignIn = 30,
                UserName = 20
            }
            """);

        result.Diagnostics.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(
        "[LanguageCatalog] public sealed class LoginLangResourceKind { }",
        "ATOMUILOC003",
        "LoginLangResourceKind",
        "non-generic enum")]
    [InlineData(
        "[System.Flags, LanguageCatalog] public enum LoginLangResourceKind { Title = 1 }",
        "ATOMUILOC003",
        "LoginLangResourceKind",
        "Flags")]
    [InlineData(
        "[LanguageCatalog(ContractVersion = 0)] public enum LoginLangResourceKind { Title = 1 }",
        "ATOMUILOC003",
        "LoginLangResourceKind",
        "ContractVersion")]
    [InlineData(
        "[LanguageCatalog] public enum LoginLangResourceKind { }",
        "ATOMUILOC003",
        "LoginLangResourceKind",
        "at least one")]
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
        "[LanguageCatalog] public enum LoginLangResourceKind { Title }",
        "Title",
        "explicit")]
    [InlineData(
        "[LanguageCatalog] public enum LoginLangResourceKind { Title = 0 }",
        "0",
        "positive")]
    [InlineData(
        "[LanguageCatalog] public enum LoginLangResourceKind { Title = -1 }",
        "-1",
        "positive")]
    public void Reports_Invalid_Catalog_Unit_Ids(
        string declaration,
        string locationText,
        string messageFragment)
    {
        var diagnostic = RunGenerator(declaration).Diagnostics.ShouldHaveSingleItem();

        AssertDiagnostic(diagnostic, "ATOMUILOC004", locationText, messageFragment);
    }

    [Fact]
    public void Reports_The_Second_Unit_When_Numeric_Id_Is_Duplicated()
    {
        var diagnostic = RunGenerator("""
            [LanguageCatalog]
            public enum LoginLangResourceKind
            {
                Title = 1,
                Heading = 1
            }
            """).Diagnostics.ShouldHaveSingleItem();

        AssertDiagnostic(diagnostic, "ATOMUILOC004", "1", "duplicated");
        diagnostic.GetMessage().ShouldContain("Heading");
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
                public sealed class LanguageCatalogAttribute : System.Attribute
                {
                    public int ContractVersion { get; set; } = 1;
                }
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
