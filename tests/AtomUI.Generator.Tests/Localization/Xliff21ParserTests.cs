using AtomUI.Localization.Build;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.Localization;

public class Xliff21ParserTests
{
    [Theory]
    [InlineData("EN-us", "en-US")]
    [InlineData("zh-hant-hk", "zh-Hant-HK")]
    [InlineData("sr-latn-rs", "sr-Latn-RS")]
    [InlineData("zh-cmn-hans-cn", "zh-cmn-Hans-CN")]
    [InlineData("sl-rozaj-biske", "sl-rozaj-biske")]
    [InlineData("en-US-u-ca-gregory", "en-US-u-ca-gregory")]
    [InlineData("de-DE-1996-a-extend1-x-PRIVATE", "de-DE-1996-a-extend1-x-private")]
    [InlineData("en-x-ACME", "en-x-acme")]
    [InlineData("iw-IL", "he-IL")]
    [InlineData("i-klingon", "tlh")]
    public void Build_Parser_Uses_The_Runtime_Bcp47_Canonicalization(
        string source,
        string expected)
    {
        Bcp47LanguageTagParser.TryParse(source, out var canonical).ShouldBeTrue();
        canonical.ShouldBe(expected);
    }

    [Fact]
    public void Parses_A_Canonical_Xliff_21_Translation()
    {
        var result = Xliff21Parser.Parse("""
            <?xml version="1.0" encoding="utf-8"?>
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0"
                   version="2.1"
                   srcLang="en-US"
                   trgLang="zh-CN">
              <file id="TestApp.Localization.LoginLangResourceKind">
                <unit id="10" name="Title">
                  <notes><note>Window title &amp; sign-in heading</note></notes>
                  <segment>
                    <source>Sign in as {0}</source>
                    <target state="translated">以 {0} 登录</target>
                  </segment>
                </unit>
              </file>
            </xliff>
            """);

        result.Errors.ShouldBeEmpty();
        var document = result.Document.ShouldNotBeNull();
        document.SourceLanguage.ShouldBe("en-US");
        document.TargetLanguage.ShouldBe("zh-CN");
        document.File.Id.ShouldBe("TestApp.Localization.LoginLangResourceKind");
        var unit = document.File.Units.ShouldHaveSingleItem();
        unit.Id.ShouldBe(10);
        unit.Name.ShouldBe("Title");
        unit.Source.ShouldBe("Sign in as {0}");
        unit.Target.ShouldBe("以 {0} 登录");
        unit.TargetState.ShouldBe("translated");
        unit.Notes.ShouldHaveSingleItem().ShouldBe("Window title & sign-in heading");
        unit.PlaceholderIndexes.ShouldBe([0]);
    }

    [Fact]
    public void Accepts_Escaped_Braces_And_Multiple_Ordered_Placeholders()
    {
        var result = Xliff21Parser.Parse(CreateDocument(
            source: "{{Page}} {1,-8:N2} / {0}",
            target: "{0} / {{页面}} {1,8:N2}"));

        result.Errors.ShouldBeEmpty();
        result.Document!.File.Units[0].PlaceholderIndexes.ShouldBe([0, 1]);
    }

    [Fact]
    public void Accepts_Dollar_Braced_Template_Tokens_As_Plain_Text()
    {
        var result = Xliff21Parser.Parse(CreateDocument(
            source: "Total ${Total} items",
            target: "共 ${Total} 项"));

        result.Errors.ShouldBeEmpty();
        result.Document!.File.Units[0].PlaceholderIndexes.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("ZH-cn", "canonical BCP 47")]
    [InlineData("zh_CN", "valid BCP 47")]
    [InlineData("en-a", "valid BCP 47")]
    public void Rejects_Invalid_Or_Noncanonical_Target_Language(
        string targetLanguage,
        string messageFragment)
    {
        var result = Xliff21Parser.Parse(CreateDocument(targetLanguage: targetLanguage));

        result.Errors.ShouldHaveSingleItem().Message.ShouldContain(messageFragment);
        result.Document.ShouldBeNull();
    }

    [Theory]
    [InlineData("2.0", "version")]
    [InlineData("", "version")]
    public void Rejects_Unsupported_Xliff_Version(string version, string messageFragment)
    {
        var result = Xliff21Parser.Parse(CreateDocument(version: version));

        result.Errors.ShouldHaveSingleItem().Message.ShouldContain(messageFragment);
    }

    [Theory]
    [InlineData("<file id=\"First\"><unit id=\"10\" name=\"Title\"><segment><source>Title</source></segment></unit></file><file id=\"Second\"><unit id=\"20\" name=\"Body\"><segment><source>Body</source></segment></unit></file>", "exactly one file")]
    [InlineData("<file id=\"Catalog\" />", "at least one unit")]
    [InlineData("<file id=\"Catalog\"><unit id=\"0\" name=\"Title\"><segment><source>Title</source></segment></unit></file>", "positive Int32")]
    [InlineData("<file id=\"Catalog\"><unit id=\"10\" name=\"\"><segment><source>Title</source></segment></unit></file>", "name")]
    [InlineData("<file id=\"Catalog\"><unit id=\"10\" name=\"Title\"><segment><source>Title</source></segment><segment><source>Again</source></segment></unit></file>", "exactly one segment")]
    public void Rejects_Invalid_File_Or_Unit_Structure(string fileContent, string messageFragment)
    {
        var result = Xliff21Parser.Parse($$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0"
                   version="2.1"
                   srcLang="en-US">
              {{fileContent}}
            </xliff>
            """);

        result.Errors.ShouldHaveSingleItem().Message.ShouldContain(messageFragment);
    }

    [Fact]
    public void Rejects_Duplicate_Unit_Ids()
    {
        var result = Xliff21Parser.Parse("""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
              <file id="Catalog">
                <unit id="10" name="Title"><segment><source>Title</source></segment></unit>
                <unit id="10" name="Heading"><segment><source>Heading</source></segment></unit>
              </file>
            </xliff>
            """);

        result.Errors.ShouldHaveSingleItem().Message.ShouldContain("duplicated");
    }

    [Fact]
    public void Rejects_Dtd_And_External_Entity_Input()
    {
        var result = Xliff21Parser.Parse("""
            <!DOCTYPE xliff [<!ENTITY secret SYSTEM "file:///etc/passwd">]>
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
              <file id="Catalog">
                <unit id="10" name="Title"><segment><source>&secret;</source></segment></unit>
              </file>
            </xliff>
            """);

        result.Errors.ShouldHaveSingleItem().Message.ShouldContain("DTD");
        result.Document.ShouldBeNull();
    }

    [Theory]
    [InlineData("unknown", "target state")]
    [InlineData("", "target state")]
    public void Rejects_Unknown_Target_State(string state, string messageFragment)
    {
        var result = Xliff21Parser.Parse(CreateDocument(targetState: state));

        result.Errors.ShouldHaveSingleItem().Message.ShouldContain(messageFragment);
    }

    [Theory]
    [InlineData("Value {", "CompositeFormat")]
    [InlineData("Value }", "CompositeFormat")]
    [InlineData("Value {name}", "CompositeFormat")]
    public void Rejects_Invalid_Composite_Format(string target, string messageFragment)
    {
        var result = Xliff21Parser.Parse(CreateDocument(source: "Value {0}", target: target));

        result.Errors.ShouldHaveSingleItem().Message.ShouldContain(messageFragment);
    }

    [Fact]
    public void Rejects_Source_Target_Placeholder_Mismatch()
    {
        var result = Xliff21Parser.Parse(CreateDocument(
            source: "Selected {0} of {1}",
            target: "已选择 {0}"));

        result.Errors.ShouldHaveSingleItem().Message.ShouldContain("placeholder indexes");
    }

    private static string CreateDocument(
        string source = "Sign in",
        string target = "登录",
        string targetLanguage = "zh-CN",
        string targetState = "translated",
        string version = "2.1")
    {
        return $$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0"
                   version="{{version}}"
                   srcLang="en-US"
                   trgLang="{{targetLanguage}}">
              <file id="TestApp.Localization.LoginLangResourceKind">
                <unit id="10" name="Title">
                  <segment>
                    <source>{{source}}</source>
                    <target state="{{targetState}}">{{target}}</target>
                  </segment>
                </unit>
              </file>
            </xliff>
            """;
    }
}
