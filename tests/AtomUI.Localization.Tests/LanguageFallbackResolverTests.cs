using System.Globalization;
using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LanguageFallbackResolverTests
{
    [Theory]
    [InlineData("zh-Hant-HK", "zh-Hant-HK|zh-Hant|zh|en-US")]
    [InlineData("zh-CN", "zh-CN|zh-Hans|zh|en-US")]
    [InlineData("zh-TW", "zh-TW|zh-Hant|zh|en-US")]
    [InlineData("fr-CA", "fr-CA|fr|en-US")]
    [InlineData("sr-Latn-RS", "sr-Latn-RS|sr-Latn|sr|en-US")]
    [InlineData("fr", "fr|en-US")]
    [InlineData("en-GB", "en-GB|en|en-US")]
    [InlineData("en-US", "en-US")]
    public void Resolve_Returns_Deterministic_Parent_Chain(string language, string expected)
    {
        var tag = LanguageTag.Parse(language);
        StandardLanguageDefinitions.TryCreate(tag, out var definition).ShouldBeTrue();

        var candidates = LanguageFallbackResolver.Resolve(tag, definition!.FormattingCulture);

        string.Join('|', candidates.Select(static candidate => candidate.Value)).ShouldBe(expected);
    }

    [Fact]
    public void Resolve_Uses_Language_Tag_Structure_Instead_Of_Formatting_Culture()
    {
        var candidates = LanguageFallbackResolver.Resolve(
            LanguageTag.Parse("fr-CA-x-acme"),
            CultureInfo.GetCultureInfo("en-US"));

        candidates.Select(static candidate => candidate.Value)
                  .ShouldBe(["fr-CA-x-acme", "fr-CA", "fr", "en-US"]);
    }

    [Theory]
    [InlineData("de-DE-u-co-phonebk", "de-DE-u-co-phonebk|de-DE|de|en-US")]
    [InlineData("sl-rozaj-biske", "sl-rozaj-biske|sl-rozaj|sl|en-US")]
    public void Resolve_Removes_Extensions_And_Variants_Structurally(
        string language,
        string expected)
    {
        var candidates = LanguageFallbackResolver.Resolve(
            LanguageTag.Parse(language),
            CultureInfo.GetCultureInfo("en-US"));

        string.Join('|', candidates.Select(static candidate => candidate.Value)).ShouldBe(expected);
    }

    [Fact]
    public void Resolve_Falls_Back_From_Private_Use_Only_Tag_To_English()
    {
        var candidates = LanguageFallbackResolver.Resolve(
            LanguageTag.Parse("x-acme-private"),
            CultureInfo.GetCultureInfo("en-US"));

        candidates.Select(static candidate => candidate.Value)
                  .ShouldBe(["x-acme-private", "en-US"]);
    }

    [Fact]
    public void Resolve_Rejects_Default_Tag_And_Null_Culture()
    {
        Should.Throw<ArgumentException>(() => LanguageFallbackResolver.Resolve(
            default,
            CultureInfo.GetCultureInfo("en-US")));
        Should.Throw<ArgumentNullException>(() => LanguageFallbackResolver.Resolve(
            LanguageTags.EnUS,
            null!));
    }
}
