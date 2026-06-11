using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.RadioButton;

[LanguageProvider(LanguageCode.zh_TW, RadioButtonShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioGroups = "分組";
    public const string ScenarioOptions = "選項";
    public const string ScenarioStyles = "樣式";

    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "RadioButton 的禁用狀態。";
    public const string RadioGroupTitle = "單選框組";
    public const string RadioGroupDescription = "一組單選組件。";
    public const string VerticalRadioGroupTitle = "垂直單選框組";
    public const string VerticalRadioGroupDescription = "垂直排列的單選框組。";
    public const string ItemsSourceRadioGroupTitle = "通過 ItemsSource 生成單選框組";
    public const string ItemsSourceRadioGroupDescription = "單選框組。";
    public const string OptionButtonTitle = "選項按鈕";
    public const string OptionButtonDescription = "選項按鈕組。";
    public const string OptionButtonWithIconTitle = "帶圖標的選項按鈕";
    public const string OptionButtonWithIconDescription = "帶圖標的選項按鈕組。";
    public const string OptionStyleTitle = "選項樣式";
    public const string OptionStyleDescription = "選項按鈕樣式組合。";
    public const string SolidOptionButtonTitle = "實色選項按鈕";
    public const string SolidOptionButtonDescription = "實色選項按鈕樣式。";
    public const string SizeTypeTitle = "尺寸類型";
    public const string SizeTypeDescription = "提供大號、中號和小號三種尺寸，可與輸入框配合使用。";
    public const string P2TitleSizetype = "尺寸類型";
    public const string P2ContentRadio = "單選框";
    public const string P2ContentRadio1 = "單選框 1";
    public const string P2ContentRadio2 = "單選框 2";
    public const string P2ContentToggleDisabled = "切換禁用";
    public const string P2TextLinechart = "折線圖";
    public const string P2TextDotchart = "點圖";
    public const string P2TextBarchart = "柱狀圖";
    public const string P2TextPiechart = "餅圖";
    public const string P2ContentOptionA = "選項 A";
    public const string P2ContentOptionB = "選項 B";
    public const string P2ContentOptionC = "選項 C";
    public const string P2ContentOptionD = "選項 D";
    public const string P2ContentApple = "蘋果";
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

