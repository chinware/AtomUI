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
