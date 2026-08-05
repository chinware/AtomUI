using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.Localization;

internal static class LocalizationGeneratorTestHost
{
    internal static GeneratorRunResult Run(
        string source,
        params TestAdditionalText[] additionalTexts)
    {
        var compilation = CreateCompilation(source);
        var optionsProvider = new TestOptionsProvider(additionalTexts);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LocalizationGenerator().AsSourceGenerator()],
            additionalTexts.Cast<AdditionalText>().ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider);
        driver = driver.RunGenerators(compilation, TestContext.Current.CancellationToken);
        return driver.GetRunResult().Results.ShouldHaveSingleItem();
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(static path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();
        return CSharpCompilation.Create(
            "TestApp",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    internal sealed class TestAdditionalText : AdditionalText
    {
        private readonly SourceText _text;

        internal TestAdditionalText(
            string path,
            string text,
            IReadOnlyDictionary<string, string>? metadata = null)
        {
            Path = path;
            _text = SourceText.From(text);
            Metadata = metadata ?? new Dictionary<string, string>();
        }

        public override string Path { get; }

        internal IReadOnlyDictionary<string, string> Metadata { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default) => _text;
    }

    private sealed class TestOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private static readonly AnalyzerConfigOptions s_empty = new TestOptions(
            new Dictionary<string, string>());
        private static readonly AnalyzerConfigOptions s_global = new TestOptions(
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.PackageId"] = "Test.Package",
                ["build_property.AssemblyName"] = "TestApp",
                ["build_property.RootNamespace"] = "TestApp"
            });
        private readonly IReadOnlyDictionary<string, AnalyzerConfigOptions> _fileOptions;

        internal TestOptionsProvider(IEnumerable<TestAdditionalText> files)
        {
            _fileOptions = files.ToDictionary(
                static file => file.Path,
                static file => (AnalyzerConfigOptions)new TestOptions(file.Metadata),
                StringComparer.Ordinal);
        }

        public override AnalyzerConfigOptions GlobalOptions => s_global;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => s_empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            return _fileOptions.TryGetValue(textFile.Path, out var options) ? options : s_empty;
        }
    }

    private sealed class TestOptions(
        IReadOnlyDictionary<string, string> values) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value)
        {
            return values.TryGetValue(key, out value!);
        }
    }
}
