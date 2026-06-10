using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Descriptions;

[LanguageProvider(LanguageCode.zh_CN, DescriptionsShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string BorderTitle = "边框";
    public const string BorderDescription = "带边框和背景色的 Descriptions。";
    public const string CustomSizeTitle = "自定义尺寸";
    public const string CustomSizeDescription = "自定义尺寸以适配不同容器。";
    public const string ResponsiveTitle = "响应式";
    public const string ResponsiveDescription = "响应式配置可以在小屏设备上更好展示。";
    public const string VerticalTitle = "垂直布局";
    public const string VerticalDescription = "最简单的用法。";
    public const string VerticalBorderTitle = "垂直带边框";
    public const string VerticalBorderDescription = "带边框和背景色的 Descriptions。";
    public const string RowTitle = "整行展示";
    public const string RowDescription = "整行展示。";
    public const string P2HeaderUserInfo = "用户信息";
    public const string P2HeaderCustomSize = "自定义尺寸";
    public const string P2HeaderResponsiveDescriptions = "响应式描述列表";
    public const string P2LabelUserName = "用户名";
    public const string P2LabelTelephone = "电话";
    public const string P2LabelLive = "居住地";
    public const string P2LabelRemark = "备注";
    public const string P2LabelAddress = "地址";
    public const string P2LabelProduct = "产品";
    public const string P2LabelBillingMode = "计费方式";
    public const string P2LabelBilling = "计费";
    public const string P2LabelAutomaticRenewal = "自动续费";
    public const string P2LabelOrderTime = "订购时间";
    public const string P2LabelUsageTime = "使用时间";
    public const string P2LabelStatus = "状态";
    public const string P2LabelNegotiatedAmount = "协商金额";
    public const string P2LabelDiscount = "折扣";
    public const string P2LabelOfficialReceipts = "官方收据";
    public const string P2LabelOfficial = "官方";
    public const string P2LabelConfigInfo = "配置信息";
    public const string P2LabelTime = "时间";
    public const string P2LabelAmount = "金额";
    public const string P2LabelHardwareInfo = "硬件信息";
    public const string P2ContentZhouMaomao = "周毛毛";
    public const string P2ContentHangzhouZhejiang = "浙江杭州";
    public const string P2ContentEmpty = "暂无";
    public const string P2ContentAddress = "中国浙江省杭州市西湖区万塘路 18 号";
    public const string P2ContentCloudDatabase = "云数据库";
    public const string P2ContentPrepaid = "预付费";
    public const string P2ContentYes = "是";
    public const string P2ContentRunning = "运行中";
    public const string P2TextDataDiskTypeMongodb = "数据盘类型：MongoDB";
    public const string P2TextDatabaseVersionN3N4 = "数据库版本：3.4";
    public const string P2TextPackageDdsMongoMid = "套餐：dds.mongo.mid";
    public const string P2TextStorageSpaceN10Gb = "存储空间：10 GB";
    public const string P2TextReplicationFactorN3 = "副本因子：3";
    public const string P2TextRegionEastChinaN1 = "地域：华东 1";
    public const string P2ContentLarge = "大号";
    public const string P2ContentMiddle = "中号";
    public const string P2ContentSmall = "小号";
    public const string P2ContentEdit = "编辑";
    public const string P2TextCpuN6CoreN3N5Ghz = "CPU：6 核 3.5 GHz";

    protected override Type GetResourceKindType() => typeof(DescriptionsShowCaseLangResourceKind);
}
