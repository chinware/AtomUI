using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Descriptions;

[LanguageProvider(LanguageCode.zh_TW, DescriptionsShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string PageSubtitle = "以響應式佈局展示分組的唯讀詳情。";
    public const string PageDescription = "Descriptions 用於展示相關的標籤和值，支援邊框、標題、額外操作、響應式列數和垂直佈局。";
    public const string ComponentCategory = "數據展示";
    public const string ComponentStatusStable = "穩定";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計 Token";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string BorderTitle = "邊框";
    public const string BorderDescription = "帶邊框和背景色的 Descriptions。";
    public const string CustomSizeTitle = "自定義尺寸";
    public const string CustomSizeDescription = "自定義尺寸以適配不同容器。";
    public const string ResponsiveTitle = "響應式";
    public const string ResponsiveDescription = "響應式配置可以在小屏設備上更好展示。";
    public const string VerticalTitle = "垂直佈局";
    public const string VerticalDescription = "最簡單的用法。";
    public const string VerticalBorderTitle = "垂直帶邊框";
    public const string VerticalBorderDescription = "帶邊框和背景色的 Descriptions。";
    public const string RowTitle = "整行展示";
    public const string RowDescription = "整行展示。";
    public const string P2HeaderUserInfo = "用戶信息";
    public const string P2HeaderCustomSize = "自定義尺寸";
    public const string P2HeaderResponsiveDescriptions = "響應式描述列表";
    public const string P2LabelUserName = "用戶名";
    public const string P2LabelTelephone = "電話";
    public const string P2LabelLive = "居住地";
    public const string P2LabelRemark = "備注";
    public const string P2LabelAddress = "地址";
    public const string P2LabelProduct = "產品";
    public const string P2LabelBillingMode = "計費方式";
    public const string P2LabelBilling = "計費";
    public const string P2LabelAutomaticRenewal = "自動續費";
    public const string P2LabelOrderTime = "訂購時間";
    public const string P2LabelUsageTime = "使用時間";
    public const string P2LabelStatus = "狀態";
    public const string P2LabelNegotiatedAmount = "協商金額";
    public const string P2LabelDiscount = "折扣";
    public const string P2LabelOfficialReceipts = "官方收據";
    public const string P2LabelOfficial = "官方";
    public const string P2LabelConfigInfo = "配置信息";
    public const string P2LabelTime = "時間";
    public const string P2LabelAmount = "金額";
    public const string P2LabelHardwareInfo = "硬件信息";
    public const string P2ContentZhouMaomao = "周毛毛";
    public const string P2ContentHangzhouZhejiang = "浙江杭州";
    public const string P2ContentEmpty = "暫無";
    public const string P2ContentAddress = "中國浙江省杭州市西湖區萬塘路 18 號";
    public const string P2ContentCloudDatabase = "雲數據庫";
    public const string P2ContentPrepaid = "預付費";
    public const string P2ContentYes = "是";
    public const string P2ContentRunning = "運行中";
    public const string P2TextDataDiskTypeMongodb = "數據盤類型：MongoDB";
    public const string P2TextDatabaseVersionN3N4 = "數據庫版本：3.4";
    public const string P2TextPackageDdsMongoMid = "套餐：dds.mongo.mid";
    public const string P2TextStorageSpaceN10Gb = "存儲空間：10 GB";
    public const string P2TextReplicationFactorN3 = "副本因子：3";
    public const string P2TextRegionEastChinaN1 = "地域：華東 1";
    public const string P2ContentLarge = "大號";
    public const string P2ContentMiddle = "中號";
    public const string P2ContentSmall = "小號";
    public const string P2ContentEdit = "編輯";
    public const string P2TextCpuN6CoreN3N5Ghz = "CPU：6 核 3.5 GHz";

    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "類型";
    public const string ApiColumnDefault = "預設值";
    public const string ApiPropertyIsBordered = "控制描述列表是否顯示邊框。";
    public const string ApiPropertyIsShowColon = "控制每個標籤後是否顯示冒號。";
    public const string ApiPropertyColumnInfo = "設定各響應式斷點下的列數。";
    public const string ApiPropertyHeader = "設定顯示在描述項上方的可選標題內容。";
    public const string ApiPropertyHeaderTemplate = "設定標題內容的渲染模板。";
    public const string ApiPropertyExtra = "設定標題區域右側的額外內容。";
    public const string ApiPropertyExtraTemplate = "設定額外內容的渲染模板。";
    public const string ApiPropertyLayout = "控制標籤和值使用水平佈局還是垂直佈局。";
    public const string ApiPropertySizeType = "控制描述項的間距尺寸。";
    public const string ApiPropertyItemsSource = "設定數據驅動描述項的資料來源。";
    public const string ApiPropertyItems = "取得或設定內聯描述項集合。";

    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameLabelBg = "帶邊框模式下標籤的背景色。";
    public const string TokenNameLabelColor = "標籤文字顏色。";
    public const string TokenNameTitleColor = "標題文字顏色。";
    public const string TokenNameHeaderMargin = "標題區域下邊距。";
    public const string TokenNameItemPaddingLG = "大號尺寸的子項內邊距。";
    public const string TokenNameItemPadding = "中號尺寸的子項內邊距。";
    public const string TokenNameItemPaddingSM = "小號尺寸的子項內邊距。";
    public const string TokenNameColonMargin = "標籤冒號的外邊距。";
    public const string TokenNameContentColor = "內容文字顏色。";
    public const string TokenNameExtraColor = "額外區域文字顏色。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";

    protected override Type GetResourceKindType() => typeof(DescriptionsShowCaseLangResourceKind);
}
