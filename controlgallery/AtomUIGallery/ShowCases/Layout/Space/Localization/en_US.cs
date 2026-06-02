using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Space;

[LanguageProvider(LanguageCode.en_US, SpaceShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Crowded components horizontal spacing.";
    public const string VerticalSpaceTitle = "Vertical Space";
    public const string VerticalSpaceDescription = "Crowded components vertical spacing.";
    public const string SizeTitle = "Space Size";
    public const string SizeDescription = "Use size to set the spacing. Three sizes are preset: small, middle, and large. You can also customize the spacing. If size is not set, the spacing is small.";
    public const string AlignTitle = "Align";
    public const string AlignDescription = "Config item align.";
    public const string WrapTitle = "Wrap";
    public const string WrapDescription = "Auto wrap line.";
    public const string SplitTitle = "Split";
    public const string SplitDescription = "Crowded components split.";
    public const string CompactFormTitle = "Compact Mode for form component";
    public const string CompactFormDescription = "Compact Mode for form component.";
    public const string CompactButtonTitle = "Button Compact Mode";
    public const string CompactButtonDescription = "Button component compact example.";
    public const string VerticalCompactTitle = "Vertical Compact Mode";
    public const string VerticalCompactDescription = "Vertical Mode for Space.Compact, supports Button only.";

    protected override Type GetResourceKindType() => typeof(SpaceShowCaseLangResourceKind);
}
