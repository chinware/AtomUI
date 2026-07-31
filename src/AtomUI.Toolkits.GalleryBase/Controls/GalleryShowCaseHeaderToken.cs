using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[ControlDesignToken]
internal class GalleryShowCaseHeaderToken : AbstractControlDesignToken
{
    public const string ID = "GalleryShowCaseHeader";

    public Thickness HeaderMargin { get; set; }
    public double HeaderSpacing { get; set; }
    public double SummarySpacing { get; set; }
    public double TitleFontSize { get; set; }
    public FontWeight TitleFontWeight { get; set; }
    public Thickness TagsMargin { get; set; }
    public double TagItemSpacing { get; set; }
    public double TagLineSpacing { get; set; }
    public double SubtitleFontSize { get; set; }
    public Thickness MetadataPadding { get; set; }
    public CornerRadius MetadataCornerRadius { get; set; }
    public double MetadataMinHeight { get; set; }
    public double MetadataItemSpacing { get; set; }
    public double MetadataLineSpacing { get; set; }
    public double MetadataPairSpacing { get; set; }
    public double MetadataLabelWidth { get; set; }
    public double MetadataValueWidth { get; set; }
    public double MetadataLineHeight { get; set; }
    public FontFamily MetadataValueFontFamily { get; set; } = FontFamily.Default;

    public GalleryShowCaseHeaderToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        HeaderMargin              = new Thickness(SharedToken.SizeUnit * 7, SharedToken.SizeUnit * 6, SharedToken.SizeUnit * 7, SharedToken.SizeUnit * 4 + 2);
        HeaderSpacing             = SharedToken.SizeUnit * 4;
        SummarySpacing            = SharedToken.SizeUnit + 2;
        TitleFontSize             = SharedToken.FontSizeHeading2;
        TitleFontWeight           = SharedToken.FontWeightStrong;
        TagsMargin                = new Thickness(SharedToken.SizeUnit * 2 + 2, 0, 0, 0);
        TagItemSpacing            = SharedToken.SizeUnit * 2 + 2;
        TagLineSpacing            = SharedToken.SizeUnit + 2;
        SubtitleFontSize          = SharedToken.FontSizeLG;
        MetadataPadding           = new Thickness(SharedToken.SizeUnit * 4, SharedToken.SizeUnit * 2);
        MetadataCornerRadius      = SharedToken.BorderRadiusLG;
        MetadataMinHeight         = SharedToken.SizeUnit * 12;
        MetadataItemSpacing       = SharedToken.SizeUnit * 12;
        MetadataLineSpacing       = SharedToken.SizeUnit * 2;
        MetadataPairSpacing       = SharedToken.SizeUnit * 2 + 2;
        MetadataLabelWidth        = SharedToken.SizeUnit * 21;
        MetadataValueWidth        = SharedToken.SizeUnit * 50;
        MetadataLineHeight        = SharedToken.SizeUnit * 5 + 2;
        MetadataValueFontFamily   = FontFamily.Parse("Consolas");
    }

}
