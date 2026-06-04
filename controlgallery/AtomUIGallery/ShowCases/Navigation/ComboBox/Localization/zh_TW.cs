using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ComboBox;

[LanguageProvider(LanguageCode.zh_TW, ComboBoxShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "基礎的組合框用法。";
    public const string ItemsSourceTitle = "通過 ItemsSource 生成 ComboBoxItem";
    public const string ItemsSourceDescription = "基於 ItemsSource 和模板生成結構。";
    public const string DisabledTitle = "禁用狀態";
    public const string DisabledDescription = "禁用狀態的組合框。";
    public const string ThreeSizesTitle = "三種尺寸";
    public const string ThreeSizesDescription = "ComboBox 提供三種尺寸：大號（40px）、默認（32px）和小號（24px）。";
    public const string VariantsTitle = "不同形態";
    public const string VariantsDescription = "輸入框的不同形態。";
    public const string PrePostTabTitle = "前置/後置標籤";
    public const string PrePostTabDescription = "前置和後置標籤的使用示例。";
    public const string PrefixSuffixTitle = "前綴和後綴";
    public const string PrefixSuffixDescription = "在輸入框內部添加前綴或後綴圖標。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為輸入框添加錯誤或警告狀態。";
    public const string P2PlaceholderTextPleaseSelect = "請選擇";
    public const string P2ContentPoemLine1 = "床前明月光";
    public const string P2ContentPoemLine2 = "疑是地上霜";
    public const string P2ContentPoemLine3 = "舉頭望明月";
    public const string P2ContentPoemLine4 = "低頭思故鄉";

    protected override Type GetResourceKindType() => typeof(ComboBoxShowCaseLangResourceKind);
}

