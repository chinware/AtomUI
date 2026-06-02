using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Watermark;

[LanguageProvider(LanguageCode.en_US, WatermarkShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string MultiLineTitle = "Multi-line watermark";
    public const string MultiLineDescription = "Use line-break to specify multi-line text watermark content.";
    public const string ImageWatermarkTitle = "Image watermark";
    public const string ImageWatermarkDescription = "Specify the image address via image. To ensure that the image is high definition and not stretched, set the width and height, and upload at least twice the width and height of the logo image address.";
    public const string CustomConfigurationTitle = "Custom configuration";
    public const string CustomConfigurationDescription = "Preview the watermark effect by configuring custom parameters.";

    protected override Type GetResourceKindType() => typeof(WatermarkShowCaseLangResourceKind);
}
