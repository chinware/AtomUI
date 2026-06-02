using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.ShowCases;

namespace AtomUIGallery.ShowCases.Localization.ShowCaseScenarioLang;

[LanguageProvider(LanguageCode.zh_CN, ShowCaseScenario.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string Basic = "基础";
    public const string Layout = "布局";
    public const string States = "状态";
    public const string Validation = "校验";
    public const string Dynamic = "动态";
    public const string Presets = "预设";
    public const string Controls = "控件";
    public const string Interaction = "交互";
    public const string Filtering = "筛选";
    public const string Structure = "结构";
    public const string Fixed = "固定";
    public const string Drag = "拖拽";
    public const string Editing = "编辑";
    public const string Paging = "分页";
    public const string Size = "尺寸";
    public const string Align = "对齐";
    public const string CompactForm = "紧凑表单";
    public const string CompactButton = "紧凑按钮";

    protected override Type GetResourceKindType() => typeof(ShowCaseScenarioLangResourceKind);
}
