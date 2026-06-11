using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.RadioButton;

[LanguageProvider(LanguageCode.zh_CN, RadioButtonShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioGroups = "分组";
    public const string ScenarioOptions = "选项";
    public const string ScenarioStyles = "样式";

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
    public const string P2TitleSizetype = "尺寸类型";
    public const string P2ContentRadio = "单选框";
    public const string P2ContentRadio1 = "单选框 1";
    public const string P2ContentRadio2 = "单选框 2";
    public const string P2ContentToggleDisabled = "切换禁用";
    public const string P2TextLinechart = "折线图";
    public const string P2TextDotchart = "点图";
    public const string P2TextBarchart = "柱状图";
    public const string P2TextPiechart = "饼图";
    public const string P2ContentOptionA = "选项 A";
    public const string P2ContentOptionB = "选项 B";
    public const string P2ContentOptionC = "选项 C";
    public const string P2ContentOptionD = "选项 D";
    public const string P2ContentApple = "苹果";
    public const string P2ContentPear = "梨";
    public const string P2ContentOrange = "橙子";
    public const string P2ContentMacos = "macOS";
    public const string P2ContentLinux = "Linux";
    public const string P2ContentWindows = "Windows";
    public const string P2ContentHangzhou = "杭州";
    public const string P2ContentShanghai = "上海";
    public const string P2ContentBeijing = "北京";
    public const string P2ContentChengdu = "成都";

    protected override Type GetResourceKindType() => typeof(RadioButtonShowCaseLangResourceKind);
}
