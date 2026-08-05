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
    public void Resolve_Uses_Explicit_Formatting_Culture_For_Private_Tag()
    {
        var candidates = LanguageFallbackResolver.Resolve(
            LanguageTag.Parse("fr-CA-x-acme"),
            CultureInfo.GetCultureInfo("fr-CA"));

        candidates.Select(static candidate => candidate.Value)
                  .ShouldBe(["fr-CA-x-acme", "fr", "en-US"]);
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
