using System.Collections.Immutable;
using System.Text;
using AtomUI.Toolkits.GalleryBase.Generator.ShowCaseSource;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Generator.Tests.ShowCaseSource;

public class ShowCaseCodeSnippetCatalogGeneratorTests
{
    [Fact]
    public void GeneratesCatalogFromDeferredShowCaseItemContent()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    x:Class="AtomUIGallery.ShowCases.General.Button.Views.ButtonShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate>
                                    <StackPanel Spacing="8">
                                        <Button Content="Primary" />
                                    </StackPanel>
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("namespace AtomUIGallery.Generated");
        generatedSource.ShouldContain("viewTypeName == \"AtomUIGallery.ShowCases.General.Button.Views.ButtonShowCase\"");
        generatedSource.ShouldContain("panelKey == \"ExamplesContent\"");
        generatedSource.ShouldContain("itemIndex == 0");
        generatedSource.ShouldContain("TabTitle: \"AXAML\"");
        generatedSource.ShouldContain("Language: \"axaml\"");
        generatedSource.ShouldContain("SourceFilePath: \"controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml\"");
        generatedSource.ShouldContain("<StackPanel Spacing=\\\"8\\\">");
        generatedSource.ShouldContain("<Button Content=\\\"Primary\\\" />");
    }

    [Fact]
    public void ReportsDiagnosticForShowCasePanelWithoutName()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    x:Class="AtomUIGallery.ShowCases.General.Button.Views.ButtonShowCase">
                    <gallery:ShowCasePanel>
                        <gallery:ShowCaseItem Title="Basic">
                            <Button Content="Primary" />
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """));

        RunGenerator(compilation, additionalFiles, out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN101");
        diagnostic.GetMessage().ShouldContain("ShowCasePanel");
        diagnostic.GetMessage().ShouldContain("Name");
    }

    private static CSharpCompilation RunGenerator(CSharpCompilation compilation,
                                                  ImmutableArray<AdditionalText> additionalFiles,
                                                  out ImmutableArray<Diagnostic> diagnostics)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var driver = CSharpGeneratorDriver.Create(
            [new ShowCaseCodeSnippetCatalogGenerator().AsSourceGenerator()],
            additionalTexts: additionalFiles,
            parseOptions: CSharpParseOptions.Default,
            optionsProvider: new InMemoryAnalyzerConfigOptionsProvider(
                new Dictionary<string, string>
                {
                    ["build_property.RootNamespace"] = "AtomUIGallery",
                    ["build_property.ProjectDir"]    = "/repo/"
                }));

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out diagnostics, cancellationToken);
        return (CSharpCompilation)outputCompilation;
    }

    private static CSharpCompilation CreateCompilation()
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();

        return CSharpCompilation.Create(
            "ShowCaseCodeSnippetCatalogGeneratorTests",
            [CSharpSyntaxTree.ParseText(RuntimeStubs)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly SourceText _text;

        public InMemoryAdditionalText(string path, string text)
        {
            Path  = path;
            _text = SourceText.From(text, Encoding.UTF8);
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            return _text;
        }
    }

    private sealed class InMemoryAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptions _globalOptions;

        public InMemoryAnalyzerConfigOptionsProvider(IReadOnlyDictionary<string, string> globalOptions)
        {
            _globalOptions = new InMemoryAnalyzerConfigOptions(globalOptions);
        }

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        {
            return _globalOptions;
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            return _globalOptions;
        }
    }

    private sealed class InMemoryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly IReadOnlyDictionary<string, string> _options;

        public InMemoryAnalyzerConfigOptions(IReadOnlyDictionary<string, string> options)
        {
            _options = options;
        }

        public override bool TryGetValue(string key, out string value)
        {
            return _options.TryGetValue(key, out value!);
        }
    }

    private const string RuntimeStubs = """
        namespace AtomUI.Toolkits.GalleryBase.SourceCode
        {
            public sealed record ShowCaseCodeSnippetGroup(
                string Title,
                System.Collections.Generic.IReadOnlyList<ShowCaseCodeSnippet> Snippets);

            public sealed record ShowCaseCodeSnippet(
                string TabTitle,
                string Language,
                string Text,
                string? SourceFilePath,
                int StartLine,
                int EndLine);
        }
        """;
}
