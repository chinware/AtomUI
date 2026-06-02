using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Separator;

[LanguageProvider(LanguageCode.en_US, SeparatorShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
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

    protected override Type GetResourceKindType() => typeof(SeparatorShowCaseLangResourceKind);
}
