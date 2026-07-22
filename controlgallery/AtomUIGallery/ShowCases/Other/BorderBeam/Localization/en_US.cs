using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.BorderBeam;

[LanguageProvider(LanguageCode.en_US, BorderBeamShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioExamples = "Examples";
    public const string ComponentCategory = "Other";
    public const string ComponentIntroducedVersion = "v6.0.5";
    public const string PageSubtitle = "Draw an animated highlight along a container boundary.";
    public const string PageDescription = "BorderBeam wraps a content control and renders a non-interactive beam on its border. It is decorative, stays active independently of the global motion setting by default, and can use a solid color or gradient stops.";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Wrap a card to emphasize an important workspace summary without changing the card interaction model.";
    public const string CustomizedColorTitle = "Customized color";
    public const string CustomizedColorDescription = "Use multiple color stops to build different beam presets, following the Ant Design customized color demo.";
    public const string CustomizedColorCardDescription = "The segmented selector switches the color stop collection used by the beam.";
    public const string NonUniformRadiusTitle = "Non-uniform radius";
    public const string NonUniformRadiusDescription = "Set Outset to 0 when the decorated container clips its own corners.";
    public const string NonUniformRadiusCardDescription = "The top corners keep a larger radius while the bottom corners remain square.";

}
