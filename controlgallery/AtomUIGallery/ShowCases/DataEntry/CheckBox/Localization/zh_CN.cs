using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CheckBox;

[LanguageProvider(LanguageCode.zh_CN, CheckBoxShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "禁用状态的复选框。";
    public const string ControlledCheckboxTitle = "受控复选框";
    public const string ControlledCheckboxDescription = "与其他组件联动。";
    public const string CheckboxGroupTitle = "复选框组";
    public const string CheckboxGroupDescription = "通过数组生成一组复选框。";
    public const string CheckAllTitle = "全选";
    public const string CheckAllDescription = "indeterminate 属性可以帮助实现全选效果。";
    public const string UseWithGridTitle = "结合 Grid 使用";
    public const string UseWithGridDescription = "可以在 Checkbox.Group 中结合 Checkbox 和 Grid 实现复杂布局。";
    public const string PageSubtitle = "从用户处收集二元或多选选择。";
    public const string PageDescription = "CheckBox 支持选中、未选中、半选、禁用、受控、分组和全选状态，适用于表单和选项列表。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计 Token";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertyIsChecked = "设置复选框是否为选中、未选中或半选状态。";
    public const string ApiPropertyIsThreeState = "启用后允许复选框进入半选状态。";
    public const string ApiPropertyContent = "显示在复选框指示器旁边的内容。";
    public const string ApiPropertyCommand = "点击复选框时调用的命令。";
    public const string ApiPropertyIsMotionEnabled = "启用复选框状态切换动效。";
    public const string ApiPropertyIsWaveSpiritEnabled = "启用点击水波反馈效果。";
    public const string ApiPropertyItemsSource = "CheckBoxGroup 用于生成复选框项的数据源。";
    public const string ApiPropertyCheckedItems = "CheckBoxGroup 维护的已选项集合。";
    public const string ApiPropertyItemSpacing = "CheckBoxGroup 项之间的水平间距。";
    public const string ApiPropertyLineSpacing = "CheckBoxGroup 换行后的垂直间距。";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "状态";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string TokenNameCheckIndicatorSize = "复选框指示器尺寸。";
    public const string TokenNameCheckedMarkSize = "选中标记尺寸。";
    public const string TokenNameIndicatorTristateMarkSize = "半选标记尺寸。";
    public const string TokenNameTextMargin = "指示器和文本内容之间的外边距。";
    public const string P2ContentCheckbox = "复选框";
    public const string P2ContentUnchecked = "未选中";
    public const string P2ContentIndeterminate = "半选";
    public const string P2ContentChecked = "选中";
    public const string P2ContentCheck = "选中";
    public const string P2ContentUncheck = "取消选中";
    public const string P2ContentEnable = "启用";
    public const string P2ContentDisable = "禁用";
    public const string P2ContentEnabled = "已启用";
    public const string P2ContentDisabled = "已禁用";
    public const string P2ControlledStatusFormat = "{0}-{1}";
    public const string P2ContentApple = "苹果";
    public const string P2ContentPear = "梨";
    public const string P2ContentOrange = "橙子";
    public const string P2ContentCheckAll = "全选";
    public const string P2ContentA = "A";
    public const string P2ContentB = "B";
    public const string P2ContentC = "C";
    public const string P2ContentD = "D";

    protected override Type GetResourceKindType() => typeof(CheckBoxShowCaseLangResourceKind);
}
