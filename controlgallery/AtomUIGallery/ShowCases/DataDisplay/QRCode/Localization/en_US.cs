using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.QRCode;

[LanguageProvider(LanguageCode.en_US, QRCodeShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
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
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Render scannable QR codes with status overlays and optional branding.";
    public const string PageDescription =
        "QRCode encodes text or URLs into a QR image, with configurable size, color, icon, error correction and status content.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyValue = "Text or URL encoded into the QR code.";
    public const string ApiPropertyIsBordered = "Shows or hides the QR code frame border.";
    public const string ApiPropertyColor = "Brush used to render QR code modules.";
    public const string ApiPropertySize = "Pixel size of the generated QR code image.";
    public const string ApiPropertyEccLevel = "Error correction level used when generating the QR code.";
    public const string ApiPropertyIconSize = "Pixel size of the optional center icon.";
    public const string ApiPropertyIcon = "Optional image displayed at the center of the QR code.";
    public const string ApiPropertyIconBgColor = "Background brush behind the center icon.";
    public const string ApiPropertyStatus = "Visual status overlay for active, expired, loading or scanned states.";
    public const string ApiPropertyLoadingContent = "Custom content displayed while the QR code is loading.";
    public const string ApiPropertyExpiredContent = "Custom content displayed when the QR code has expired.";
    public const string ApiPropertyScannedContent = "Custom content displayed after the QR code is scanned.";
    public const string ApiEventRefreshRequested = "Raised when the built-in refresh action is clicked.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameQRCodeTextColor = "Default color used to draw QR code modules.";
    public const string TokenNameQRCodeMaskBackgroundColor = "Overlay background color used by non-active status masks.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(QRCodeShowCaseLangResourceKind);
}
