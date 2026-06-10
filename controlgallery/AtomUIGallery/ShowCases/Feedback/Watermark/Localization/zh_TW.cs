using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Watermark;

[LanguageProvider(LanguageCode.zh_TW, WatermarkShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的用法。";
    public const string MultiLineTitle = "多行水印";
    public const string MultiLineDescription = "使用換行指定多行水印內容。";
    public const string ImageWatermarkTitle = "圖片水印";
    public const string ImageWatermarkDescription = "通過 image 指定圖片地址。為確保圖片高清且不被拉伸，請設置寬高，並上傳至少兩倍於顯示寬高的 logo 圖片。";
    public const string CustomConfigurationTitle = "自定義配置";
    public const string CustomConfigurationDescription = "通過配置自定義參數預覽水印效果。";
    public const string P2WatermarkMultiLineText = "AtomUI\n快樂工作";
    public const string P2TextNaturalInteractionDescription = "數字世界的高速迭代讓產品變得更加複雜，而人的意識和注意力資源是有限的。面對這種設計矛盾，追求自然交互將始終是 Ant Design 的一致方向。\n\n自然的用戶認知：根據認知心理學，外部信息約有 80% 通過視覺通道獲得。界面設計中最重要的視覺元素，包括佈局、色彩、插圖、圖標等，都應充分吸收自然規律，從而降低用戶的認知成本，並帶來真實、順暢的感受。在一些場景中，適時加入聽覺、觸覺等其他感官通道，也可以創造更豐富、更自然的產品體驗。\n\n自然的用戶行為：在與系統交互時，設計師應充分理解用戶、系統角色和任務目標之間的關係，並結合上下文組織系統功能和服務。同時，可以運用行為分析、人工智能、傳感器等方法輔助用戶做出有效決策，減少用戶的額外操作，節省用戶的心智和體力資源，讓人機交互更加自然。";

    protected override Type GetResourceKindType() => typeof(WatermarkShowCaseLangResourceKind);
}

