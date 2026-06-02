using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Cascader;

[LanguageProvider(LanguageCode.zh_CN, CascaderShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "用于选择省、市、区的级联选择框。";
    public const string DefaultValueTitle = "默认值";
    public const string DefaultValueDescription = "通过数组指定默认值。";
    public const string HoverTitle = "悬停展开";
    public const string HoverDescription = "悬停展开子菜单，点击选择选项。";
    public const string DisabledOptionTitle = "禁用选项";
    public const string DisabledOptionDescription = "通过在 options 中指定 disabled 属性禁用选项。";
    public const string ChangeOnSelectTitle = "选择即变化";
    public const string ChangeOnSelectDescription = "允许只选择父级选项。";
    public const string MultipleTitle = "多选";
    public const string MultipleDescription = "选择多个选项。可以通过添加 disableCheckbox 属性禁用复选框并选择特定项，禁用样式可通过 className 修改。";
    public const string ShowCheckedStrategyTitle = "显示选中策略";
    public const string ShowCheckedStrategyDescription = "使用 showCheckedStrategy 在选择框中展示选中项。";
    public const string SearchTitle = "搜索";
    public const string SearchDescription = "直接搜索并选择选项。";
    public const string LoadOptionsLazilyTitle = "懒加载选项";
    public const string LoadOptionsLazilyDescription = "使用 loadData 懒加载选项。";
    public const string PrefixAndSuffixTitle = "前缀和后缀";
    public const string PrefixAndSuffixDescription = "使用 prefix 自定义前缀内容，使用 suffixIcon 自定义选择框后缀图标，使用 expandIcon 自定义当前项展开图标。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "Cascader 提供四种变体：描边、填充、无边框和下划线。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "可以通过 placement 手动指定弹出层位置。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 Cascader 添加状态，可设置为错误或警告。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "不同尺寸的级联选择框。";
    public const string BasicCascaderViewTitle = "基础 CascaderView";
    public const string BasicCascaderViewDescription = "最基础的用法。";
    public const string GenerateByTemplateTitle = "使用模板生成";
    public const string GenerateByTemplateDescription = "可以使用 Template 机制生成树节点。";
    public const string SearchableTitle = "可搜索";
    public const string SearchableDescription = "可搜索的 CascaderView。";
    public const string SearchableItemsSourceTitle = "可搜索";
    public const string SearchableItemsSourceDescription = "使用 ItemsSource 的可搜索 CascaderView。";
    public const string DefaultExpandedTitle = "默认展开";
    public const string DefaultExpandedDescription = "可以设置默认展开路径。";

    protected override Type GetResourceKindType() => typeof(CascaderShowCaseLangResourceKind);
}
