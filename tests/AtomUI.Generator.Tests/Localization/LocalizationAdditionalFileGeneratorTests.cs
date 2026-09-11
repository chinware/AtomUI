using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;
using static AtomUI.Generator.Tests.Localization.LocalizationGeneratorTestHost;

namespace AtomUI.Generator.Tests.Localization;

public class LocalizationAdditionalFileGeneratorTests
{
    [Fact]
    public void Accepts_A_Marked_Valid_Xliff_File()
    {
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            LanguageFile(ValidXliff));

        result.Diagnostics.ShouldNotContain(static diagnostic => diagnostic.Id == "ATOMUILOC005");
    }

    [Fact]
    public void Reports_Xliff_Diagnostics_At_The_Additional_File_Line()
    {
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            LanguageFile(ValidXliff.Replace("version=\"2.1\"", "version=\"2.0\"")));

        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUILOC005");
        diagnostic.Severity.ShouldBe(DiagnosticSeverity.Error);
        diagnostic.GetMessage().ShouldContain("version");
        diagnostic.Location.GetLineSpan().Path.ShouldEndWith("zh-CN.xlf");
        diagnostic.Location.GetLineSpan().StartLinePosition.Line.ShouldBe(0);
    }

    [Fact]
    public void Ignores_An_Unmarked_Additional_File()
    {
        var result = Run(
            "namespace TestApp { public sealed class Marker { } }",
            new TestAdditionalText("Localization/zh-CN.xlf", "not xml"));

        result.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Accepts_A_Deferred_Static_Language_Pack()
    {
        var result = Run(
            CatalogSource,
            LanguageFile(SourceXliff),
            ExternalLanguageFile(
                SourceFingerprint,
                contractValidation: "Deferred"));

        result.Diagnostics.ShouldBeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Unknown")]
    [InlineData("0")]
    public void Rejects_A_Static_Language_Pack_Without_A_Valid_Contract_Validation(
        string? contractValidation)
    {
        var result = Run(
            CatalogSource,
            LanguageFile(SourceXliff),
            ExternalLanguageFile(
                SourceFingerprint,
                contractValidation: contractValidation));

        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUILOC005");
        diagnostic.GetMessage().ShouldContain("AtomUILanguageContractValidation");
    }

    [Fact]
    public void Rejects_A_Static_Language_Pack_With_A_Mismatched_Source_Fingerprint()
    {
        var result = Run(
            CatalogSource,
            LanguageFile(SourceXliff),
            ExternalLanguageFile(
                "0000000000000000000000000000000000000000000000000000000000000000"));

        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUILOC005");
        diagnostic.GetMessage().ShouldContain("source fingerprint");
    }

    [Fact]
    public void Rejects_A_Static_Language_Pack_Without_A_Source_Fingerprint()
    {
        var result = Run(
            CatalogSource,
            LanguageFile(SourceXliff),
            ExternalLanguageFile(sourceFingerprint: null));

        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUILOC005");
        diagnostic.GetMessage().ShouldContain("AtomUILanguageSourceFingerprint");
    }

    [Theory]
    [InlineData("5485C3494C44CF2782AF84ADEF0A5FBCD03303E63BF847B51543C3CDAD303EFE")]
    [InlineData("5485c3494c44cf2782af84adef0a5fbcd03303e63bf847b51543c3cdad303ef")]
    public void Rejects_A_Static_Language_Pack_With_An_Invalid_Source_Fingerprint(string fingerprint)
    {
        var result = Run(
            CatalogSource,
            LanguageFile(SourceXliff),
            ExternalLanguageFile(fingerprint));

        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUILOC005");
        diagnostic.GetMessage().ShouldContain("64 lowercase hexadecimal");
    }

    [Fact]
    public void Accepts_A_Static_Language_Pack_With_A_Matching_Source_Fingerprint()
    {
        var result = Run(
            CatalogSource,
            LanguageFile(SourceXliff),
            ExternalLanguageFile(SourceFingerprint));

        result.Diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Does_Not_Require_A_Source_Fingerprint_For_An_Application_Override()
    {
        var result = Run(
            CatalogSource,
            LanguageFile(SourceXliff),
            ExternalLanguageFile(
                sourceFingerprint: null,
                sourceKind: "ApplicationOverride",
                contractValidation: null));

        result.Diagnostics.ShouldBeEmpty();
    }

    private static TestAdditionalText LanguageFile(string content)
    {
        return new TestAdditionalText(
            "Localization/zh-CN.xlf",
            content,
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_metadata.AdditionalFiles.AtomUILanguage"] = "true",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceKind"] = "ModuleBuiltIn",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceIdentity"] = "Test.Package"
            });
    }

    private static TestAdditionalText ExternalLanguageFile(
        string? sourceFingerprint,
        string sourceKind = "StaticLanguagePack",
        string? contractValidation = "Verified")
    {
        var metadata = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["build_metadata.AdditionalFiles.AtomUILanguage"] = "true",
            ["build_metadata.AdditionalFiles.AtomUILanguageSourceKind"] = sourceKind,
            ["build_metadata.AdditionalFiles.AtomUILanguageSourceIdentity"] =
                "Test.Package.I18n.ZhCN",
            ["build_metadata.AdditionalFiles.AtomUILanguageModuleId"] = "Test.Package"
        };
        if (contractValidation is not null)
        {
            metadata["build_metadata.AdditionalFiles.AtomUILanguageContractValidation"] =
                contractValidation;
        }
        if (sourceFingerprint is not null)
        {
            metadata["build_metadata.AdditionalFiles.AtomUILanguageSourceFingerprint"] =
                sourceFingerprint;
        }

        return new TestAdditionalText(
            "packages/Test.Package.I18n.ZhCN/zh-CN.xlf",
            ValidXliff,
            metadata);
    }

    private const string CatalogSource = """
        namespace AtomUI.Localization
        {
            [System.AttributeUsage(System.AttributeTargets.Enum)]
            public sealed class LanguageCatalogAttribute : System.Attribute;
        }
        namespace TestApp.Localization
        {
            [AtomUI.Localization.LanguageCatalog]
            public enum LoginLangResourceKind { Title }
        }
        """;

    private const string SourceFingerprint =
        "ba7998290ad9e6a1345542058a39d88bd6023f5b1371fa184a89c6457e539041";

    private const string ValidXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="zh-CN">
          <file id="TestApp.Localization.LoginLangResourceKind">
            <unit id="Title">
              <segment><source>Sign in</source><target state="translated">登录</target></segment>
            </unit>
          </file>
        </xliff>
        """;

    private const string SourceXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="TestApp.Localization.LoginLangResourceKind">
            <unit id="Title">
              <segment><source>Sign in</source></segment>
            </unit>
          </file>
        </xliff>
        """;
}
