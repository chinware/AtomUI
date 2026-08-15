using AtomUI.Build.Tasks.LocalizationBuild;
using Shouldly;
using Xunit;

namespace AtomUI.Build.Tasks.Tests;

public class LanguageSourceFingerprintTests
{
    [Fact]
    public void Compute_Changes_When_A_Unit_Key_Changes()
    {
        var original = Compute(SourceXliff("Title", "Sign in"));
        var renamed = Compute(SourceXliff("Heading", "Sign in"));

        renamed.ShouldNotBe(original);
    }

    [Fact]
    public void Compute_Changes_When_English_Source_Changes()
    {
        var original = Compute(SourceXliff("Title", "Sign in"));
        var revised = Compute(SourceXliff("Title", "Log in"));

        revised.ShouldNotBe(original);
    }

    [Fact]
    public void Compute_Is_Independent_Of_Xliff_Unit_Order()
    {
        const string first = """
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
              <file id="TestApp.Localization.Strings">
                <unit id="Title"><segment><source>Sign in</source></segment></unit>
                <unit id="Cancel"><segment><source>Cancel</source></segment></unit>
              </file>
            </xliff>
            """;
        const string reversed = """
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
              <file id="TestApp.Localization.Strings">
                <unit id="Cancel"><segment><source>Cancel</source></segment></unit>
                <unit id="Title"><segment><source>Sign in</source></segment></unit>
              </file>
            </xliff>
            """;

        Compute(reversed).ShouldBe(Compute(first));
    }

    private static string Compute(string content)
    {
        return LanguageSourceFingerprint.Compute(Xliff21Parser.Parse(content).Document!);
    }

    private static string SourceXliff(string key, string source)
    {
        return $$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
              <file id="TestApp.Localization.Strings">
                <unit id="{{key}}"><segment><source>{{source}}</source></segment></unit>
              </file>
            </xliff>
            """;
    }
}
