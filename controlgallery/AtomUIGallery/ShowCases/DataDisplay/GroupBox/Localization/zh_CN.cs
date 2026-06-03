using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.GroupBox;

[LanguageProvider(LanguageCode.zh_CN, GroupBoxShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "GroupBox 控件的基础用法。";
    public const string HeaderPositionTitle = "标题位置";
    public const string HeaderPositionDescription = "GroupBox 标题支持左、中、右三种位置。";
    public const string HeaderStyleTitle = "标题样式";
    public const string HeaderStyleDescription = "GroupBox 标题支持自定义颜色和字体等属性。";
    public const string HeaderIconTitle = "标题图标";
    public const string HeaderIconDescription = "GroupBox 标题支持指定图标。";
    public const string P2HeaderTitleTitleInfo = "标题信息";
    public const string P2TextContentOfGroupBox = "分组框内容";

    protected override Type GetResourceKindType() => typeof(GroupBoxShowCaseLangResourceKind);
}
