using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Watermark;

[LanguageProvider(LanguageCode.en_US, WatermarkShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string MultiLineTitle = "Multi-line watermark";
    public const string MultiLineDescription = "Use line-break to specify multi-line text watermark content.";
    public const string ImageWatermarkTitle = "Image watermark";
    public const string ImageWatermarkDescription = "Specify the image address via image. To ensure that the image is high definition and not stretched, set the width and height, and upload at least twice the width and height of the logo image address.";
    public const string CustomConfigurationTitle = "Custom configuration";
    public const string CustomConfigurationDescription = "Preview the watermark effect by configuring custom parameters.";
    public const string P2WatermarkMultiLineText = "AtomUI\nHappy Working";
    public const string P2TextNaturalInteractionDescription = "The light-speed iteration of the digital world makes products more complex. However, human consciousness and attention resources are limited. Facing this design contradiction, the pursuit of natural interaction will be the consistent direction of Ant Design.\n\nNatural user cognition: According to cognitive psychology, about 80% of external information is obtained through visual channels. The most important visual elements in the interface design, including layout, colors, illustrations, icons, etc., should fully absorb the laws of nature, thereby reducing the user's cognitive cost and bringing authentic and smooth feelings. In some scenarios, opportunely adding other sensory channels such as hearing, touch can create a richer and more natural product experience.\n\nNatural user behavior: In the interaction with the system, the designer should fully understand the relationship between users, system roles, and task objectives, and also contextually organize system functions and services. At the same time, a series of methods such as behavior analysis, artificial intelligence and sensors could be applied to assist users to make effective decisions and reduce extra operations of users, to save users' mental and physical resources and make human-computer interaction more natural.";

    protected override Type GetResourceKindType() => typeof(WatermarkShowCaseLangResourceKind);
}
