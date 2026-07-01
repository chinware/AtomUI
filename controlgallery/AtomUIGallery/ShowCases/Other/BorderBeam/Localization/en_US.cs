using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.BorderBeam;

[LanguageProvider(LanguageCode.en_US, BorderBeamShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "Other";
    public const string ComponentIntroducedVersion = "v6.0.5";
    public const string PageSubtitle = "Draw an animated highlight along a container boundary.";
    public const string PageDescription = "BorderBeam wraps a content control and renders a non-interactive beam on its border. It is decorative, follows theme motion settings, and can use a solid color or gradient stops.";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Wrap a card to emphasize an important workspace summary without changing the card interaction model.";
    public const string CustomizedColorTitle = "Customized color";
    public const string CustomizedColorDescription = "Use multiple color stops to build different beam presets, following the Ant Design customized color demo.";
    public const string CustomizedColorCardDescription = "The segmented selector switches the color stop collection used by the beam.";
    public const string NonUniformRadiusTitle = "Non-uniform radius";
    public const string NonUniformRadiusDescription = "Set Outset to 0 when the decorated container clips its own corners.";
    public const string NonUniformRadiusCardDescription = "The top corners keep a larger radius while the bottom corners remain square.";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyColor = "Solid beam color. ColorStops takes precedence when the collection is not empty.";
    public const string ApiPropertyColorStops = "Gradient stop collection. Percent uses the public 0-100 range and maps to the visible beam segment.";
    public const string ApiPropertyOutset = "Distance by which the beam expands beyond the effective border. Null uses the effective border thickness.";
    public const string ApiPropertyBorderThickness = "Fallback border thickness when the content does not expose BorderBeam geometry.";
    public const string ApiPropertyCornerRadius = "Fallback corner radius when the content does not expose BorderBeam geometry.";
    public const string ApiPropertyIsMotionEnabled = "Controls whether the beam animation is active.";
    public const string ApiPropertyDuration = "Duration for one full beam cycle.";
    public const string ApiPropertyBeamSize = "Base size of the moving highlight segment.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameBeamSize = "Default size of the moving highlight segment.";
    public const string TokenNameBeamOpacity = "Default opacity of the beam layer.";
    public const string TokenNameMotionDuration = "Default duration for one full beam cycle.";
    public const string TokenNameMaxVisibleStopPercent = "Maximum visible stop percent used to reserve space for the transparent tail.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(BorderBeamShowCaseLangResourceKind);
}
