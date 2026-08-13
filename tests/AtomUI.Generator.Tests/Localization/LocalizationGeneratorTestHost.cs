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
        return Run(source, [], additionalTexts);
    }

    internal static GeneratorRunResult Run(
        string source,
        IReadOnlyList<MetadataReference> additionalReferences,
        params TestAdditionalText[] additionalTexts)
    {
        var compilation = CreateCompilation(source, additionalReferences);
        var optionsProvider = new TestOptionsProvider(additionalTexts);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LocalizationGenerator().AsSourceGenerator()],
            additionalTexts.Cast<AdditionalText>().ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider);
        driver = driver.RunGenerators(compilation, TestContext.Current.CancellationToken);
        return driver.GetRunResult().Results.ShouldHaveSingleItem();
    }

    internal static TestGeneratorExecution RunWithOutputCompilation(
        string source,
        params TestAdditionalText[] additionalTexts)
    {
        return RunWithOutputCompilation(source, [], additionalTexts);
    }

    internal static TestGeneratorExecution RunWithOutputCompilation(
        string source,
        IReadOnlyDictionary<string, string> globalOptions,
        params TestAdditionalText[] additionalTexts)
    {
        var compilation = CreateCompilation(source, []);
        var optionsProvider = new TestOptionsProvider(additionalTexts, globalOptions);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LocalizationGenerator().AsSourceGenerator()],
            additionalTexts.Cast<AdditionalText>().ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var driverDiagnostics,
            TestContext.Current.CancellationToken);
        return new TestGeneratorExecution(
            driver.GetRunResult().Results.ShouldHaveSingleItem(),
            outputCompilation,
            driverDiagnostics);
    }

    internal static TestGeneratorExecution RunWithOutputCompilation(
        string source,
        IReadOnlyList<MetadataReference> additionalReferences,
        params TestAdditionalText[] additionalTexts)
    {
        var compilation = CreateCompilation(source, additionalReferences);
        var optionsProvider = new TestOptionsProvider(additionalTexts);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LocalizationGenerator().AsSourceGenerator()],
            additionalTexts.Cast<AdditionalText>().ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var driverDiagnostics,
            TestContext.Current.CancellationToken);
        return new TestGeneratorExecution(
            driver.GetRunResult().Results.ShouldHaveSingleItem(),
            outputCompilation,
            driverDiagnostics);
    }

    internal static MetadataReference CreateMetadataReference(
        string assemblyName,
        string source)
    {
        var compilation = CreateCompilation(source, [], assemblyName);
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        emitResult.Diagnostics
                  .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                  .ShouldBeEmpty();
        emitResult.Success.ShouldBeTrue();
        return MetadataReference.CreateFromImage(stream.ToArray());
    }

    internal static TestIncrementalGeneratorExecution RunWithUpdatedAdditionalText(
        string source,
        TestAdditionalText original,
        TestAdditionalText replacement,
        params TestAdditionalText[] otherAdditionalTexts)
    {
        var additionalTexts = new[] { original }.Concat(otherAdditionalTexts).ToArray();
        var compilation = CreateCompilation(source, []);
        var optionsProvider = new TestOptionsProvider(additionalTexts);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new LocalizationGenerator().AsSourceGenerator()],
            additionalTexts.Cast<AdditionalText>().ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider,
            new GeneratorDriverOptions(
                IncrementalGeneratorOutputKind.None,
                trackIncrementalGeneratorSteps: true));
        driver = driver.RunGenerators(compilation, TestContext.Current.CancellationToken);
        var firstRun = driver.GetRunResult();
        driver = driver.ReplaceAdditionalText(original, replacement)
                       .RunGenerators(compilation, TestContext.Current.CancellationToken);
        return new TestIncrementalGeneratorExecution(firstRun, driver.GetRunResult());
    }

    internal static CSharpCompilation CreateCompilation(
        string source,
        IReadOnlyList<MetadataReference> additionalReferences,
        string assemblyName = "TestApp")
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(static path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .Concat(additionalReferences)
                         .ToImmutableArray();
        return CSharpCompilation.Create(
            assemblyName,
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

    internal sealed record TestGeneratorExecution(
        GeneratorRunResult Result,
        Compilation OutputCompilation,
        ImmutableArray<Diagnostic> DriverDiagnostics);

    internal sealed record TestIncrementalGeneratorExecution(
        GeneratorDriverRunResult FirstRun,
        GeneratorDriverRunResult SecondRun);

    private sealed class TestOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private static readonly AnalyzerConfigOptions s_empty = new TestOptions(
            new Dictionary<string, string>());
        private static readonly IReadOnlyDictionary<string, string> s_globalValues =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.PackageId"] = "Test.Package",
                ["build_property.AssemblyName"] = "TestApp",
                ["build_property.AtomUILanguageModuleId"] = "Test.Package"
            };
        private readonly IReadOnlyDictionary<string, AnalyzerConfigOptions> _fileOptions;
        private readonly AnalyzerConfigOptions _globalOptions;

        internal TestOptionsProvider(
            IEnumerable<TestAdditionalText> files,
            IReadOnlyDictionary<string, string>? globalOptions = null)
        {
            _fileOptions = files.ToDictionary(
                static file => file.Path,
                static file => (AnalyzerConfigOptions)new TestOptions(file.Metadata),
                StringComparer.Ordinal);
            var values = new Dictionary<string, string>(s_globalValues, StringComparer.Ordinal);
            if (globalOptions is not null)
            {
                foreach (var option in globalOptions)
                {
                    values[option.Key] = option.Value;
                }
            }
            _globalOptions = new TestOptions(values);
        }

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

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
