using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Overview;

[LanguageProvider(LanguageCode.zh_CN, OverviewPage.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string InstallTitle = "安装 AtomUI";
    public const string InstallDescription = "将桌面控件包添加到 Avalonia 应用。";
    public const string DotNetCliTab = ".NET CLI";
    public const string PackageReferenceTab = "PackageReference";
    public const string PackageManagerTab = "包管理器";
    public const string Copy = "复制";
    public const string LearningLinksTitle = "继续学习";
    public const string LearningLinksDescription = "安装完成后，可以从用户手册和 API 参考继续深入。";
    public const string UserManualTitle = "用户手册";
    public const string UserManualDescription = "了解项目理念、快速入门和开发实践。";
    public const string ApiDocsTitle = "API 文档";
    public const string ApiDocsDescription = "查询 AtomUI OSS 类型、成员和版本化接口说明。";
    public const string OpenLearningLink = "打开链接";

    protected override Type GetResourceKindType() => typeof(OverviewPageLangResourceKind);
}
