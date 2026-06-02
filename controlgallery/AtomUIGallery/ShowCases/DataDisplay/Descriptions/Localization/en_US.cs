using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Descriptions;

[LanguageProvider(LanguageCode.en_US, DescriptionsShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Simplest Usage.";
    public const string BorderTitle = "border";
    public const string BorderDescription = "Descriptions with border and background color.";
    public const string CustomSizeTitle = "Custom size";
    public const string CustomSizeDescription = "Custom sizes to fit in a variety of containers.";
    public const string ResponsiveTitle = "responsive";
    public const string ResponsiveDescription = "Responsive configuration enables perfect presentation on small screen devices.";
    public const string VerticalTitle = "Vertical";
    public const string VerticalDescription = "Simplest Usage.";
    public const string VerticalBorderTitle = "Vertical border";
    public const string VerticalBorderDescription = "Descriptions with border and background color.";
    public const string RowTitle = "Row";
    public const string RowDescription = "Display of the entire line.";

    protected override Type GetResourceKindType() => typeof(DescriptionsShowCaseLangResourceKind);
}
