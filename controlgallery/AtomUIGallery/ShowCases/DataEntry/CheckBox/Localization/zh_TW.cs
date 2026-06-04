using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CheckBox;

[LanguageProvider(LanguageCode.zh_TW, CheckBoxShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "禁用狀態的復選框。";
    public const string ControlledCheckboxTitle = "受控復選框";
    public const string ControlledCheckboxDescription = "與其他組件聯動。";
    public const string CheckboxGroupTitle = "復選框組";
    public const string CheckboxGroupDescription = "通過數組生成一組復選框。";
    public const string CheckAllTitle = "全選";
    public const string CheckAllDescription = "indeterminate 屬性可以幫助實現全選效果。";
    public const string UseWithGridTitle = "結合 Grid 使用";
    public const string UseWithGridDescription = "可以在 Checkbox.Group 中結合 Checkbox 和 Grid 實現複雜佈局。";
    public const string P2ContentCheckbox = "復選框";
    public const string P2ContentUnchecked = "未選中";
    public const string P2ContentIndeterminate = "半選";
    public const string P2ContentChecked = "選中";
    public const string P2ContentCheck = "選中";
    public const string P2ContentUncheck = "取消選中";
    public const string P2ContentEnable = "啓用";
    public const string P2ContentDisable = "禁用";
    public const string P2ContentEnabled = "已啓用";
    public const string P2ContentDisabled = "已禁用";
    public const string P2ControlledStatusFormat = "{0}-{1}";
    public const string P2ContentApple = "蘋果";
    public const string P2ContentPear = "梨";
    public const string P2ContentOrange = "橙子";
    public const string P2ContentCheckAll = "全選";
    public const string P2ContentA = "A";
    public const string P2ContentB = "B";
    public const string P2ContentC = "C";
    public const string P2ContentD = "D";

    protected override Type GetResourceKindType() => typeof(CheckBoxShowCaseLangResourceKind);
}

