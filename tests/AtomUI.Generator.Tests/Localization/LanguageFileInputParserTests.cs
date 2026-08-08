using AtomUI.Generator.Localization.Xliff;
using Shouldly;
using Xunit;
using static AtomUI.Generator.Tests.Localization.LocalizationGeneratorTestHost;

namespace AtomUI.Generator.Tests.Localization;

public class LanguageFileInputParserTests
{
    [Fact]
    public void Parser_Only_Parses_Text_And_Preserves_Source_Location()
    {
        var result = AdditionalLanguageFileParser.Parse(
            new TestAdditionalText(
                "Localization/Test/ja-JP.xlf",
                """
                <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.0" srcLang="en-US">
                  <file id="Test.Product.Catalog">
                    <unit id="Title"><segment><source>Hello</source></segment></unit>
                  </file>
                </xliff>
                """),
            TestContext.Current.CancellationToken);

        result.File.ShouldBeNull();
        var error = result.Errors.ShouldHaveSingleItem();
        error.Line.ShouldBe(1);
        error.Message.ShouldContain("version");
    }
}
