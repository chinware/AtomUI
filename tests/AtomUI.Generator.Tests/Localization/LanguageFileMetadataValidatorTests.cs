using AtomUI.Generator.Localization.Xliff;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;
using static AtomUI.Generator.Tests.Localization.LocalizationGeneratorTestHost;

namespace AtomUI.Generator.Tests.Localization;

public class LanguageFileMetadataValidatorTests
{
    [Fact]
    public void Validator_Keeps_Contract_Mode_Separate_From_Activation_State()
    {
        var parsed = AdditionalLanguageFileParser.Parse(
            new TestAdditionalText("packages/Test.Package.I18n.JaJP/ja-JP.xlf", ValidXliff),
            TestContext.Current.CancellationToken);
        var metadata = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [LanguageGeneratorOptions.SourceKindMetadata] = "StaticLanguagePack",
            [LanguageGeneratorOptions.SourceIdentityMetadata] = "Test.Package.I18n.JaJP",
            [LanguageGeneratorOptions.ModuleIdMetadata] = "Test.Package",
            [LanguageGeneratorOptions.ContractValidationMetadata] = "Deferred",
            [LanguageGeneratorOptions.SourceFingerprintMetadata] = SourceFingerprint
        };

        var result = LanguageFileMetadataValidator.Validate(
            parsed.File.ShouldNotBeNull(),
            new TestOptions(metadata),
            defaultModuleId: "Test.Package");

        result.Diagnostics.ShouldBeEmpty();
        result.Input.ShouldNotBeNull().ContractValidation.ShouldBe(LanguageFileContractValidation.Deferred);
        result.Input.ShouldNotBeOfType<LanguageInputResolution>();
    }

    private sealed class TestOptions(
        IReadOnlyDictionary<string, string> values) : AnalyzerConfigOptions
    {
        public override bool TryGetValue(string key, out string value)
        {
            return values.TryGetValue(key, out value!);
        }
    }

    private const string SourceFingerprint =
        "ba7998290ad9e6a1345542058a39d88bd6023f5b1371fa184a89c6457e539041";

    private const string ValidXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="ja-JP">
          <file id="TestApp.Localization.LoginLangResourceKind">
            <unit id="Title">
              <segment><source>Sign in</source><target state="final">サインイン</target></segment>
            </unit>
          </file>
        </xliff>
        """;
}
