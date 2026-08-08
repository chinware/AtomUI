using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.Localization;

public class LanguageGeneratorOptionsTests
{
    [Fact]
    public void GetModuleId_Prefers_The_Explicit_Language_Module_Id()
    {
        var options = new TestAnalyzerConfigOptionsProvider(
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_property.AtomUILanguageModuleId"] = "ThirdParty.Component",
                ["build_property.PackageId"] = "ThirdParty.Component.I18n.JaJP",
                ["build_property.AssemblyName"] = "ThirdParty.Component.LanguagePack"
            });

        LanguageGeneratorOptions.GetModuleId(options, "Fallback.Module")
                                .ShouldBe("ThirdParty.Component");
    }

    private sealed class TestAnalyzerConfigOptionsProvider(
        IReadOnlyDictionary<string, string> globalValues) : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptions _globalOptions = new TestAnalyzerConfigOptions(globalValues);

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => Empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => Empty;

        private static AnalyzerConfigOptions Empty { get; } =
            new TestAnalyzerConfigOptions(new Dictionary<string, string>());
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
