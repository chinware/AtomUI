using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Select;

[LanguageProvider(LanguageCode.zh_CN, SelectShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础用法。";
    public const string SearchFieldTitle = "带搜索框的选择器";
    public const string SearchFieldDescription = "展开时搜索选项。";
    public const string CustomSearchTitle = "自定义搜索";
    public const string CustomSearchDescription = "使用 filterOption 自定义搜索，对 Header 进行大小写敏感搜索。";
    public const string MultipleSelectionTitle = "多选";
    public const string MultipleSelectionDescription = "多选，从已有项中选择。";
    public const string TagsTitle = "标签";
    public const string TagsDescription = "允许用户从列表中选择标签或输入自定义标签。";
    public const string SizesTitle = "尺寸";
    public const string SizesDescription = "选择器输入区域默认高度为 32px。size 设置为 large 时高度为 40px，设置为 small 时高度为 24px。";
    public const string CustomDropdownOptionsTitle = "自定义下拉选项";
    public const string CustomDropdownOptionsDescription = "使用 optionRender 自定义下拉选项渲染。";
    public const string OptionGroupTitle = "选项分组";
    public const string OptionGroupDescription = "使用 OptGroup 对选项分组。";
    public const string VariantsTitle = "变体";
    public const string VariantsDescription = "Select 提供四种变体：描边、填充、无边框和下划线。";
    public const string HideAlreadySelectedTitle = "隐藏已选项";
    public const string HideAlreadySelectedDescription = "在下拉列表中隐藏已经选择的选项。";
    public const string StatusTitle = "状态";
    public const string StatusDescription = "通过 status 为 Select 添加状态，可设置为错误或警告。";
    public const string MaxCountTitle = "最大数量";
    public const string MaxCountDescription = "可以设置 maxCount 属性控制最多可选项数量。超过限制后，选项会变为禁用状态。";
    public const string ResponsiveMaxTagCountTitle = "响应式 maxTagCount";
    public const string ResponsiveMaxTagCountDescription = "在响应式场景中自动折叠为标签。由于响应式计算有性能成本，不建议在大型表单中使用。";
    public const string PrefixAndSuffixTitle = "前缀和后缀";
    public const string PrefixAndSuffixDescription = "自定义 prefix 和 suffixIcon。";

    protected override Type GetResourceKindType() => typeof(SelectShowCaseLangResourceKind);
}
