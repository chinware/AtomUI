using System.Globalization;
using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LanguageTagTests
{
    [Theory]
    [InlineData("en", "en")]
    [InlineData("EN-us", "en-US")]
    [InlineData("zh-hant-hk", "zh-Hant-HK")]
    [InlineData("sr-latn-rs", "sr-Latn-RS")]
    [InlineData("zh-cmn-hans-cn", "zh-cmn-Hans-CN")]
    [InlineData("sl-rozaj-biske", "sl-rozaj-biske")]
    [InlineData("en-US-u-ca-gregory", "en-US-u-ca-gregory")]
    [InlineData("de-DE-1996-a-extend1-x-PRIVATE", "de-DE-1996-a-extend1-x-private")]
    [InlineData("en-x-ACME", "en-x-acme")]
    [InlineData("x-ACME-private", "x-acme-private")]
    [InlineData("iw-IL", "he-IL")]
    [InlineData("in-ID", "id-ID")]
    [InlineData("ji", "yi")]
    [InlineData("i-klingon", "tlh")]
    [InlineData("en-GB-oed", "en-GB-oxendict")]
    public void Parse_Returns_Canonical_Bcp47_Tag(string source, string expected)
    {
        var language = LanguageTag.Parse(source);

        language.Value.ShouldBe(expected);
        language.ToString().ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("en_US")]
    [InlineData("-en")]
    [InlineData("en-")]
    [InlineData("en--US")]
    [InlineData("e")]
    [InlineData("languages-language")]
    [InlineData("en-a")]
    [InlineData("en-x")]
    [InlineData("en-a-foo-a-bar")]
    [InlineData("en-中文")]
    public void TryParse_Rejects_Invalid_Tags(string source)
    {
        LanguageTag.TryParse(source, out var language).ShouldBeFalse();
        language.ShouldBe(default);
    }

    [Fact]
    public void TryParse_Rejects_Null()
    {
        LanguageTag.TryParse(null, out var language).ShouldBeFalse();
        language.ShouldBe(default);
    }

    [Fact]
    public void Parse_Rejects_Null()
    {
        Should.Throw<ArgumentNullException>(() => LanguageTag.Parse(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("en_US")]
    [InlineData("en-a")]
    public void Parse_Reports_The_Invalid_Source(string source)
    {
        var exception = Should.Throw<FormatException>(() => LanguageTag.Parse(source));

        exception.Message.ShouldContain(source);
    }

    [Fact]
    public void Canonical_Tags_Have_Ordinal_Value_Equality()
    {
        LanguageTag.Parse("EN-us").ShouldBe(LanguageTag.Parse("en-US"));
        LanguageTag.Parse("en-US").GetHashCode().ShouldBe(LanguageTag.Parse("EN-us").GetHashCode());
    }

    [Fact]
    public void Default_Tag_Cannot_Expose_A_Value()
    {
        Should.Throw<InvalidOperationException>(() => default(LanguageTag).Value);
    }

    [Fact]
    public void FromCultureInfo_Uses_Ietf_Language_Tag()
    {
        var language = LanguageTag.FromCultureInfo(CultureInfo.GetCultureInfo("zh-Hant-TW"));

        language.Value.ShouldBe("zh-Hant-TW");
    }

    [Fact]
    public void FromCultureInfo_Rejects_Null()
    {
        Should.Throw<ArgumentNullException>(() => LanguageTag.FromCultureInfo(null!));
    }
}
