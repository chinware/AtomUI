using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.RadioButton;

[LanguageProvider(LanguageCode.zh_CN, RadioButtonShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最简单的用法。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "RadioButton 的禁用状态。";
    public const string RadioGroupTitle = "单选框组";
    public const string RadioGroupDescription = "一组单选组件。";
    public const string VerticalRadioGroupTitle = "垂直单选框组";
    public const string VerticalRadioGroupDescription = "垂直排列的单选框组。";
    public const string ItemsSourceRadioGroupTitle = "通过 ItemsSource 生成单选框组";
    public const string ItemsSourceRadioGroupDescription = "单选框组。";
    public const string OptionButtonTitle = "选项按钮";
    public const string OptionButtonDescription = "选项按钮组。";
    public const string OptionButtonWithIconTitle = "带图标的选项按钮";
    public const string OptionButtonWithIconDescription = "带图标的选项按钮组。";
    public const string OptionStyleTitle = "选项样式";
    public const string OptionStyleDescription = "选项按钮样式组合。";
    public const string SolidOptionButtonTitle = "实色选项按钮";
    public const string SolidOptionButtonDescription = "实色选项按钮样式。";
    public const string SizeTypeTitle = "尺寸类型";
    public const string SizeTypeDescription = "提供大号、中号和小号三种尺寸，可与输入框配合使用。";

    protected override Type GetResourceKindType() => typeof(RadioButtonShowCaseLangResourceKind);
}
