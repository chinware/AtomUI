using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.GroupBox;

[LanguageProvider(LanguageCode.zh_TW, GroupBoxShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "GroupBox 控件的基礎用法。";
    public const string HeaderPositionTitle = "標題位置";
    public const string HeaderPositionDescription = "GroupBox 標題支持左、中、右三種位置。";
    public const string HeaderStyleTitle = "標題樣式";
    public const string HeaderStyleDescription = "GroupBox 標題支持自定義顏色和字體等屬性。";
    public const string HeaderIconTitle = "標題圖標";
    public const string HeaderIconDescription = "GroupBox 標題支持指定圖標。";
    public const string P2HeaderTitleTitleInfo = "標題信息";
    public const string P2TextContentOfGroupBox = "分組框內容";

    protected override Type GetResourceKindType() => typeof(GroupBoxShowCaseLangResourceKind);
}

