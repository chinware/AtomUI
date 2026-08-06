using System.Globalization;
using AtomUI.Localization;
using AtomUIGallery.Localization;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Localization;

public sealed class GalleryLanguageDefaultsTests
{
    [Theory]
    [InlineData("en-US", "en-US")]
    [InlineData("en-GB", "en-US")]
    [InlineData("zh-CN", "zh-CN")]
    [InlineData("zh-SG", "zh-CN")]
    [InlineData("zh-Hans", "zh-CN")]
    [InlineData("zh-TW", "zh-TW")]
    [InlineData("zh-HK", "zh-TW")]
    [InlineData("zh-Hant", "zh-TW")]
    [InlineData("ja-JP", "en-US")]
    [InlineData("fr-FR", "en-US")]
    public void Resolve_Maps_System_Culture_To_Installed_Gallery_Language(
        string cultureName,
        string expectedTag)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);

        GalleryLanguageDefaults.Resolve(culture).ShouldBe(LanguageTag.Parse(expectedTag));
    }
}
