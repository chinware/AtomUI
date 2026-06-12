using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Separator;

[LanguageProvider(LanguageCode.en_US, SeparatorShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "General";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Separate content sections with horizontal or vertical rules.";
    public const string PageDescription = "Separator creates visual rhythm between related sections, supports optional title text, plain text styling, vertical rules, line variants, and density sizes.";
    public const string InfoNamespaceLabel = "Namespace:";
    public const string InfoPackageLabel = "Package:";
    public const string InfoBaseClassLabel = "Base class:";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyTitle = "Text displayed inside the horizontal separator.";
    public const string ApiPropertyTitlePosition = "Positions the title at the left, center, or right of the separator.";
    public const string ApiPropertyTitleColor = "Brush used to render the separator title text.";
    public const string ApiPropertyLineColor = "Brush used to render the separator line.";
    public const string ApiPropertyOrientation = "Controls whether the separator is horizontal or vertical.";
    public const string ApiPropertyOrientationMargin = "Distance between a left or right title and its nearest edge.";
    public const string ApiPropertyVariant = "Changes the separator line between solid, dotted, and dashed.";
    public const string ApiPropertyLineWidth = "Render-scaling independent width of the separator line.";
    public const string ApiPropertyIsPlain = "Uses plain body text styling for the title instead of heading styling.";
    public const string ApiPropertySizeType = "Controls horizontal separator spacing density.";
    public const string ApiPropertyVerticalSeparatorOrientation = "VerticalSeparator overrides Separator orientation to vertical.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameTextPaddingInline = "Inline title padding measured in em units.";
    public const string TokenNameOrientationMarginPercent = "Default title-to-edge ratio when orientation margin is not specified.";
    public const string TokenNameVerticalMarginInline = "Horizontal margin used by vertical separators.";
    public const string TokenNameHorizontalMarginBlockSM = "Vertical block margin for small horizontal separators.";
    public const string TokenNameHorizontalMarginBlock = "Default vertical block margin for horizontal separators.";
    public const string TokenNameHorizontalMarginBlockLG = "Vertical block margin for large horizontal separators.";
    public const string TokenNameHorizontalWithTextGutterMargin = "Vertical margin used by horizontal separators with title text.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string HorizontalTitle = "Horizontal";
    public const string HorizontalDescription = "A Separator is horizontal by default. You can add text within Separator.";
    public const string DividerWithTitleTitle = "Divider with title";
    public const string DividerWithTitleDescription = "Divider with inner title. Set orientation='left/right' to align it.";
    public const string PlainTextTitle = "Text without heading style";
    public const string PlainTextDescription = "You can use non-heading style of divider text by setting the plain property.";
    public const string SpacingSizeTitle = "Set the spacing size of the divider";
    public const string SpacingSizeDescription = "The size of the spacing.";
    public const string VerticalTitle = "Vertical";
    public const string VerticalDescription = "Use type='vertical' to make the divider vertical.";
    public const string VariantTitle = "Variant";
    public const string VariantDescription = "Divider uses the solid variant by default. You can change it to dashed or dotted.";
    public const string P2TitleText = "Text";
    public const string P2TitleLeftText = "Left text";
    public const string P2TitleRightText = "Right text";
    public const string P2TitleLeftTextWithN0Orientationmargin = "Left Text with 0 orientationMargin";
    public const string P2TitleRightTextWithN50pxOrientationmargin = "Right Text with 50px orientationMargin";
    public const string P2TitleLeftText2 = "Left Text";
    public const string P2TitleRightText2 = "Right Text";
    public const string P2TitleSolid = "Solid";
    public const string P2TitleDotted = "Dotted";
    public const string P2TitleDashed = "Dashed";
    public const string P2TextLoremIpsumDolorSitAmetConsecteturAdipiscingElit = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nonne merninisti licere mihi ista probare, quae sunt a te dicta? Refert tamen, quo modo.";
    public const string P2TextItem1 = "Item1";
    public const string P2TextItem2 = "Item2";
    public const string P2TextItem3 = "Item3";

    protected override Type GetResourceKindType() => typeof(SeparatorShowCaseLangResourceKind);
}
