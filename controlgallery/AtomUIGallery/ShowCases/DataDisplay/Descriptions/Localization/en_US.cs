using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Descriptions;

[LanguageProvider(LanguageCode.en_US, DescriptionsShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
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
    public const string P2HeaderUserInfo = "User Info";
    public const string P2HeaderCustomSize = "Custom Size";
    public const string P2HeaderResponsiveDescriptions = "Responsive Descriptions";
    public const string P2LabelUserName = "UserName";
    public const string P2LabelTelephone = "Telephone";
    public const string P2LabelLive = "Live";
    public const string P2LabelRemark = "Remark";
    public const string P2LabelAddress = "Address";
    public const string P2LabelProduct = "Product";
    public const string P2LabelBillingMode = "Billing Mode";
    public const string P2LabelBilling = "Billing";
    public const string P2LabelAutomaticRenewal = "Automatic Renewal";
    public const string P2LabelOrderTime = "Order time";
    public const string P2LabelUsageTime = "Usage Time";
    public const string P2LabelStatus = "Status";
    public const string P2LabelNegotiatedAmount = "Negotiated Amount";
    public const string P2LabelDiscount = "Discount";
    public const string P2LabelOfficialReceipts = "Official Receipts";
    public const string P2LabelOfficial = "Official";
    public const string P2LabelConfigInfo = "Config Info";
    public const string P2LabelTime = "Time";
    public const string P2LabelAmount = "Amount";
    public const string P2LabelHardwareInfo = "Hardware Info";
    public const string P2ContentZhouMaomao = "Zhou Maomao";
    public const string P2ContentHangzhouZhejiang = "Hangzhou, Zhejiang";
    public const string P2ContentEmpty = "empty";
    public const string P2ContentAddress = "No. 18, Wantang Road, Xihu District, Hangzhou, Zhejiang, China";
    public const string P2ContentCloudDatabase = "Cloud Database";
    public const string P2ContentPrepaid = "Prepaid";
    public const string P2ContentYes = "YES";
    public const string P2ContentRunning = "Running";
    public const string P2TextDataDiskTypeMongodb = "Data disk type: MongoDB";
    public const string P2TextDatabaseVersionN3N4 = "Database version: 3.4";
    public const string P2TextPackageDdsMongoMid = "Package: dds.mongo.mid";
    public const string P2TextStorageSpaceN10Gb = "Storage space: 10 GB";
    public const string P2TextReplicationFactorN3 = "Replication factor: 3";
    public const string P2TextRegionEastChinaN1 = "Region: East China 1";
    public const string P2ContentLarge = "Large";
    public const string P2ContentMiddle = "Middle";
    public const string P2ContentSmall = "Small";
    public const string P2ContentEdit = "Edit";
    public const string P2TextCpuN6CoreN3N5Ghz = "CPU: 6 Core 3.5 GHz";

    protected override Type GetResourceKindType() => typeof(DescriptionsShowCaseLangResourceKind);
}
