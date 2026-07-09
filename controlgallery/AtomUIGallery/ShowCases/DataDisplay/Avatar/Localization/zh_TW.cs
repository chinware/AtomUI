using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Avatar;

[LanguageProvider(LanguageCode.zh_TW, AvatarShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "數據展示";
    public const string ComponentStatusStable = "穩定";
    public const string PageSubtitle = "使用圖片、圖標或文本頭像表示用戶、團隊或對象。";
    public const string PageDescription = "Avatar 用於在緊湊空間中展示身份信息。它支持圓形和方形、顯式尺寸、文本自動縮放，以及頭像組的折疊展示。";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyShape = "設置頭像形狀，可在圓形和方形之間切換。";
    public const string ApiPropertySize = "設置顯式頭像尺寸，並讓控件進入自定義尺寸模式。";
    public const string ApiPropertySizeType = "使用共享尺寸刻度展示大號、默認和小號頭像。";
    public const string ApiPropertyIcon = "使用圖標作為頭像內容。";
    public const string ApiPropertySrc = "加載 SVG 圖片作為頭像內容。";
    public const string ApiPropertyText = "展示文本內容，並在文本寬於頭像時自動縮放。";
    public const string ApiPropertyGap = "控制文本自動縮放時左右兩側的間距。";
    public const string ApiPropertyMaxDisplayCount = "限制 AvatarGroup 中可見頭像數量，並將剩餘頭像折疊到溢出頭像中。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameAvatarToken = "Avatar 和 AvatarGroup 使用的組件 Token 映射。";
    public const string TokenNameContainerSize = "從共享控件高度派生的默認頭像容器尺寸。";
    public const string TokenNameGroupSpace = "頭像分組時使用的間距和重疊值。";
    public const string TokenNameAvatarColor = "佔位背景上使用的默認頭像前景色。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";
    public const string TokenStatusMapped = "映射";
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "提供三種尺寸和兩種形狀。";
    public const string TypeTitle = "類型";
    public const string TypeDescription = "支持圖片、圖標和字母類型，後兩種頭像可自定義顏色和背景色。";
    public const string AutoSetFontSizeTitle = "自動設置字號";
    public const string AutoSetFontSizeDescription = "對於字母類型頭像，當字母過長無法展示時，字號會根據頭像寬度自動調整。也可以使用 gap 設置左右兩側的單位距離。";
    public const string AvatarGroupTitle = "頭像組";
    public const string AvatarGroupDescription = "頭像組展示。";
    public const string P2ContentU = "U";
    public const string P2ContentUser = "用戶";
    public const string P2ContentChangeuser = "切換用戶";
    public const string P2ContentChangegap = "切換間距";
    public const string P2ContentK = "K";
    public const string P2ContentA = "A";

}
