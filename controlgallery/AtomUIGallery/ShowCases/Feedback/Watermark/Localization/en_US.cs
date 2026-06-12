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
    public const string PageSubtitle = "Text or image watermarks for protected content.";
    public const string PageDescription = "Watermark attaches repeated text or image glyphs to a target area, helping identify ownership or prevent unauthorized reuse.";
    public const string ComponentCategory = "Feedback";
    public const string ComponentStatusStable = ".NET 10";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyGlyph = "Attached watermark glyph rendered over the target layoutable.";
    public const string ApiPropertyHorizontalSpace = "Horizontal space between repeated watermark glyphs.";
    public const string ApiPropertyVerticalSpace = "Vertical space between repeated watermark glyph rows.";
    public const string ApiPropertyHorizontalOffset = "Horizontal offset before the first glyph in each row.";
    public const string ApiPropertyVerticalOffset = "Vertical offset before the first watermark row.";
    public const string ApiPropertyRotate = "Rotation angle applied to each watermark glyph.";
    public const string ApiPropertyOpacity = "Opacity applied while rendering the watermark glyphs.";
    public const string ApiPropertyIsMirrorUsed = "Mirrors alternating glyphs by applying inverse rotation.";
    public const string ApiPropertyIsCrossUsed = "Offsets alternating rows to create a crossed layout.";
    public const string ApiPropertyText = "Text rendered by TextGlyph.";
    public const string ApiPropertyFontSize = "Font size used by TextGlyph.";
    public const string ApiPropertyForeground = "Brush used to render TextGlyph.";
    public const string ApiPropertySource = "Image source rendered by ImageGlyph.";
    public const string ApiPropertyHeight = "Rendered height of ImageGlyph.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameNoComponentToken = "Watermark currently has no component-specific design token; visual configuration is controlled by glyph properties.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusNotApplicable = "N/A";
    public const string P2WatermarkMultiLineText = "AtomUI\nHappy Working";
    public const string P2TextNaturalInteractionDescription = "The light-speed iteration of the digital world makes products more complex. However, human consciousness and attention resources are limited. Facing this design contradiction, the pursuit of natural interaction will be the consistent direction of Ant Design.\n\nNatural user cognition: According to cognitive psychology, about 80% of external information is obtained through visual channels. The most important visual elements in the interface design, including layout, colors, illustrations, icons, etc., should fully absorb the laws of nature, thereby reducing the user's cognitive cost and bringing authentic and smooth feelings. In some scenarios, opportunely adding other sensory channels such as hearing, touch can create a richer and more natural product experience.\n\nNatural user behavior: In the interaction with the system, the designer should fully understand the relationship between users, system roles, and task objectives, and also contextually organize system functions and services. At the same time, a series of methods such as behavior analysis, artificial intelligence and sensors could be applied to assist users to make effective decisions and reduce extra operations of users, to save users' mental and physical resources and make human-computer interaction more natural.";

    protected override Type GetResourceKindType() => typeof(WatermarkShowCaseLangResourceKind);
}
