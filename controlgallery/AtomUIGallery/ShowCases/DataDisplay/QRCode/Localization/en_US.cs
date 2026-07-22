using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.QRCode;

[LanguageProvider(LanguageCode.en_US, QRCodeShowCase.LanguageId)]
internal partial class en_US
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
    public const string P2TextLoading = "Loading...";
    public const string P2TextQRCodeExpired = "QR code expired";
    public const string P2ContentClickToRefresh = "Click to refresh";
    public const string P2TextScanned = "Scanned";
    public const string P2ContentSmaller = "Smaller";
    public const string P2ContentLarger = "Larger";
    public const string P2ContentHoverMe = "Hover me";
    public const string ScenarioExamples = "Examples";
    public const string PageSubtitle = "Render scannable QR codes with status overlays and optional branding.";
    public const string PageDescription =
        "QRCode encodes text or URLs into a QR image, with configurable size, color, icon, error correction and status content.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string ApiEventRefreshRequested = "Raised when the built-in refresh action is clicked.";

}
