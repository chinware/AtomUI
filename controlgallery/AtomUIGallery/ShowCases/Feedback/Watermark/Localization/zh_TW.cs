using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Watermark;

[LanguageProvider(LanguageCode.zh_TW, WatermarkShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法。";
    public const string MultiLineTitle = "多行水印";
    public const string MultiLineDescription = "使用換行指定多行水印內容。";
    public const string ImageWatermarkTitle = "圖片水印";
    public const string ImageWatermarkDescription = "通過 image 指定圖片地址。為確保圖片高清且不被拉伸，請設置寬高，並上傳至少兩倍於顯示寬高的 logo 圖片。";
    public const string CustomConfigurationTitle = "自定義配置";
    public const string CustomConfigurationDescription = "通過配置自定義參數預覽水印效果。";
    public const string PageSubtitle = "用於受保護內容的文字或圖片水印。";
    public const string PageDescription = "Watermark 將重複的文字或圖片 Glyph 附加到目標區域，用於標識歸屬或降低未經授權複用的風險。";
    public const string ComponentCategory = "反饋";
    public const string ComponentStatusStable = ".NET 10";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "默認值";
    public const string ApiPropertyGlyph = "渲染在目標 Layoutable 上方的附加水印 Glyph。";
    public const string ApiPropertyHorizontalSpace = "重複水印 Glyph 之間的水平間距。";
    public const string ApiPropertyVerticalSpace = "重複水印行之間的垂直間距。";
    public const string ApiPropertyHorizontalOffset = "每行第一個 Glyph 之前的水平偏移。";
    public const string ApiPropertyVerticalOffset = "第一行水印之前的垂直偏移。";
    public const string ApiPropertyRotate = "應用到每個水印 Glyph 的旋轉角度。";
    public const string ApiPropertyOpacity = "渲染水印 Glyph 時使用的不透明度。";
    public const string ApiPropertyIsMirrorUsed = "對交替 Glyph 使用反向旋轉形成鏡像效果。";
    public const string ApiPropertyIsCrossUsed = "偏移交替行以形成交錯佈局。";
    public const string ApiPropertyText = "TextGlyph 渲染的文本。";
    public const string ApiPropertyFontSize = "TextGlyph 使用的字體大小。";
    public const string ApiPropertyForeground = "用於渲染 TextGlyph 的畫刷。";
    public const string ApiPropertySource = "ImageGlyph 渲染的圖片源。";
    public const string ApiPropertyHeight = "ImageGlyph 的渲染高度。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "範圍";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameNoComponentToken = "Watermark 當前沒有組件專屬 Design Token；視覺配置由 Glyph 屬性控制。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusNotApplicable = "N/A";
    public const string P2WatermarkMultiLineText = "AtomUI\n快樂工作";
    public const string P2TextNaturalInteractionDescription = "數字世界的高速迭代讓產品變得更加複雜，而人的意識和注意力資源是有限的。面對這種設計矛盾，追求自然交互將始終是 Ant Design 的一致方向。\n\n自然的用戶認知：根據認知心理學，外部信息約有 80% 通過視覺通道獲得。界面設計中最重要的視覺元素，包括佈局、色彩、插圖、圖標等，都應充分吸收自然規律，從而降低用戶的認知成本，並帶來真實、順暢的感受。在一些場景中，適時加入聽覺、觸覺等其他感官通道，也可以創造更豐富、更自然的產品體驗。\n\n自然的用戶行為：在與系統交互時，設計師應充分理解用戶、系統角色和任務目標之間的關係，並結合上下文組織系統功能和服務。同時，可以運用行為分析、人工智能、傳感器等方法輔助用戶做出有效決策，減少用戶的額外操作，節省用戶的心智和體力資源，讓人機交互更加自然。";

}
