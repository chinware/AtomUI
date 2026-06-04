using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Mentions;

[LanguageProvider(LanguageCode.zh_TW, MentionsShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最簡單的用法。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "Mentions 提供四種變體：描邊、填充、無邊框和下划線。";
    public const string AsynchronousLoadingTitle = "異步加載";
    public const string AsynchronousLoadingDescription = "異步加載。";
    public const string CustomizeTriggerTokenTitle = "自定義觸發標記";
    public const string CustomizeTriggerTokenDescription = "通過 prefix 屬性自定義觸發標記，默認為 @，也支持數組。";
    public const string DisabledOrReadOnlyTitle = "禁用或只讀";
    public const string DisabledOrReadOnlyDescription = "配置 disabled 和 readOnly。";
    public const string PlacementTitle = "彈出位置";
    public const string PlacementDescription = "改變建議列表的彈出位置。";
    public const string StatusTitle = "狀態";
    public const string StatusDescription = "通過 status 為 Mentions 添加狀態，可設置為錯誤或警告。";
    public const string AutoSizeTitle = "自動高度";
    public const string AutoSizeDescription = "高度自動調整。";
    public const string WithClearIconTitle = "帶清除圖標";
    public const string WithClearIconDescription = "自定義清除按鈕。";
    public const string P2PlaceholderTextOutlined = "線框風格";
    public const string P2PlaceholderTextFilled = "填充風格";
    public const string P2PlaceholderTextBorderless = "無邊框";
    public const string P2PlaceholderTextUnderlined = "下划線";
    public const string P2PlaceholderTextInputToMentionPeopleToMentionTag = "輸入 @ 提及成員，輸入 # 提及標籤";
    public const string P2PlaceholderTextThisIsDisabledMentions = "這是禁用狀態的 Mentions";
    public const string P2PlaceholderTextThisIsReadonlyMentions = "這是只讀狀態的 Mentions";

    protected override Type GetResourceKindType() => typeof(MentionsShowCaseLangResourceKind);
}

