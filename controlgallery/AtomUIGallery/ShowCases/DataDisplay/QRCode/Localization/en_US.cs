using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.QRCode;

[LanguageProvider(LanguageCode.en_US, QRCodeShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "Basic usage example.";
    public const string WithIconTitle = "With Icon";
    public const string WithIconDescription = "A QR code with an icon.";
    public const string DifferentStatusTitle = "Different status";
    public const string DifferentStatusDescription = "The QR code status can be controlled by Status. Active, Expired, Loading and Scanned are supported.";
    public const string CustomStatusRendererTitle = "Custom status renderer";
    public const string CustomStatusRendererDescription = "Use LoadingTemplate, ExpiredTemplate and ScannedTemplate to control rendering for different QR code states.";
    public const string CustomSizeTitle = "Custom size";
    public const string CustomSizeDescription = "Custom size.";
    public const string CustomColorTitle = "Custom color";
    public const string CustomColorDescription = "Custom color.";
    public const string ErrorLevelTitle = "Error correction level";
    public const string ErrorLevelDescription = "Set errorLevel to adjust different error correction levels.";
    public const string AdvancedUsageTitle = "Advanced usage";
    public const string AdvancedUsageDescription = "Example with a popover card.";

    protected override Type GetResourceKindType() => typeof(QRCodeShowCaseLangResourceKind);
}
