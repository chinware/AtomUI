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

        result.Diagnostics.ShouldBeEmpty();
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

    private const string ValidXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="zh-CN">
          <file id="TestApp.Localization.LoginLangResourceKind">
            <unit id="10" name="Title">
              <segment><source>Sign in</source><target state="translated">登录</target></segment>
            </unit>
          </file>
        </xliff>
        """;
}
