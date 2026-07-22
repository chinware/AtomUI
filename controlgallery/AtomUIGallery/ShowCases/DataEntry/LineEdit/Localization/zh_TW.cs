using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.LineEdit;

[LanguageProvider(LanguageCode.zh_TW, LineEditShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioBasic = "基礎";
    public const string ScenarioState = "狀態";
    public const string ScenarioSearch = "搜索";
    public const string ScenarioTextArea = "文本域";

    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "基礎用法示例。";
    public const string TextBoxTitle = "TextBox";
    public const string TextBoxDescription = "AtomUI TextBox 保留原生 TextBox API，同時使用 LineEdit 的邊框、懸停和聚焦視覺。";
    public const string InputSizesTitle = "輸入框三種尺寸";
    public const string InputSizesDescription = "輸入框有三種尺寸：大號（40px）、默認（32px）和小號（24px）。";
    public const string VariantsTitle = "變體";
    public const string VariantsDescription = "輸入框的變體。";
    public const string DisabledTitle = "禁用";
    public const string DisabledDescription = "輸入框禁用樣式的變體。";
    public const string PrePostTabTitle = "前置/後置標籤";
    public const string PrePostTabDescription = "使用前置和後置標籤的示例。";
    public const string WithClearIconTitle = "帶清除圖標";
    public const string WithClearIconDescription = "帶移除圖標的輸入框，點擊圖標可清空全部內容。";
    public const string PasswordBoxTitle = "密碼框";
    public const string PasswordBoxDescription = "密碼類型輸入框。";
    public const string OtpLineEditTwoWayBindingTitle = "OTP 雙向繫結";
    public const string OtpLineEditTwoWayBindingDescription = "OtpLineEdit.Text 使用單一可繫結值源，可由 ViewModel 設定或清空。";
    public const string OtpLineEditFormTitle = "OTP 表單驗證";
    public const string OtpLineEditFormDescription = "Form validator 透過 Avalonia DataValidationErrors 寫入錯誤。";
    public const string OtpLineEditAntDesignTitle = "一次性密碼框";
    public const string OtpLineEditAntDesignDescription = "一次性密碼輸入框。";
    public const string OtpLineEditSetValueButtonText = "設為 654321";
    public const string OtpLineEditClearButtonText = "清空";
    public const string OtpLineEditCurrentValueFormat = "目前值：{0}";
    public const string OtpLineEditEmptyValueText = "（空）";
    public const string OtpLineEditFormLabelCode = "驗證碼";
    public const string OtpLineEditFormValidationMessage = "請輸入驗證碼";
    public const string PrefixAndSuffixTitle = "前綴和後綴";
    public const string PrefixAndSuffixDescription = "在輸入框內部添加前綴或後綴圖標。";
    public const string PrefixAndSuffixToolTip = "關於此欄位的提示資訊";
    public const string InputStatusTitle = "狀態";
    public const string InputStatusDescription = "通過 status 為 Input 添加狀態，可設置為錯誤或警告。";
    public const string SearchBoxTitle = "搜索框";
    public const string SearchBoxDescription = "將標準輸入框和搜索按鈕組合創建搜索框的示例。";
    public const string SearchEditSizeTypeTitle = "SearchEdit 尺寸";
    public const string SearchEditSizeTypeDescription = "SearchEdit 支持大號、中號、小號，也支持通過 Custom 配合本地高度和字號自定義。";
    public const string DisabledSearchBoxTitle = "禁用搜索框";
    public const string DisabledSearchBoxDescription = "將標準輸入框和搜索按鈕組合創建搜索框的示例。";
    public const string SearchBoxWithLoadingTitle = "帶加載狀態的搜索框";
    public const string SearchBoxWithLoadingDescription = "onSearch 時顯示搜索加載狀態。";
    public const string TextAreaTitle = "文本域";
    public const string TextAreaDescription = "用於多行輸入。";
    public const string AutoSizeTextAreaTitle = "高度自適應內容";
    public const string AutoSizeTextAreaDescription = "textarea 類型 Input 的 autoSize 屬性會讓高度根據內容自動調整。可以向 autoSize 提供選項對象，指定自動調整的最小和最大行數。";
    public const string CharacterCountingTitle = "帶字數統計";
    public const string CharacterCountingDescription = "顯示字數統計。";
    public const string TextAreaStatusTitle = "狀態";
    public const string TextAreaStatusDescription = "通過 status 為 TextArea 添加狀態，可設置為錯誤或警告。";
    public const string P2PlaceholderTextBasicUsage = "基礎用法";
    public const string P2PlaceholderTextTextBox = "AtomUI TextBox";
    public const string P2PlaceholderTextLarge = "大號";
    public const string P2PlaceholderTextMiddle = "中號";
    public const string P2PlaceholderTextSmall = "小號";
    public const string P2PlaceholderTextCustom = "自定義";
    public const string P2TitleNormal = "普通";
    public const string P2PlaceholderTextOutlined = "線框風格";
    public const string P2PlaceholderTextFilled = "填充風格";
    public const string P2PlaceholderTextBorderless = "無邊框";
    public const string P2PlaceholderTextUnderlined = "下划線";
    public const string P2TitleLeftaddonAndRightaddon = "左側附加和右側附加";
    public const string P2TextMysite = "我的站點";
    public const string P2PlaceholderTextInputWithClearIcon = "帶清除圖標的輸入框";
    public const string P2PlaceholderTextTextareaWithClearIcon = "帶清除圖標的文本域";
    public const string P2PlaceholderTextInputPassword = "輸入密碼";
    public const string P2PlaceholderTextEnterYourUsername = "輸入用戶名";
    public const string P2PlaceholderTextError = "錯誤";
    public const string P2PlaceholderTextWarning = "警告";
    public const string P2PlaceholderTextErrorWithPrefix = "帶前綴的錯誤";
    public const string P2PlaceholderTextWarningWithPrefix = "帶前綴的警告";
    public const string P2TitleSearch = "搜索";
    public const string P2PlaceholderTextInputSearchText = "輸入搜索文本";
    public const string P2TitleFilled = "填充風格";
    public const string P2TitleBorderless = "無邊框";
    public const string P2TitleUnderlined = "下划線";
    public const string P2PlaceholderTextInputSearchLoadingDefault = "默認搜索加載";
    public const string P2PlaceholderTextInputSearchLoadingWithEnterbutton = "帶搜索按鈕的搜索加載";
    public const string P2PlaceholderTextMaxlengthIsN6 = "最大長度為 6";
    public const string P2PlaceholderTextDisabled = "禁用";
    public const string P2PlaceholderTextAutosizeHeightBasedOnContentLines = "根據內容行數自動調整高度";
    public const string P2PlaceholderTextAutosizeHeightWithMinimumAndMaximumNumberOf = "按最小和最大行數自動調整高度";
    public const string P2PlaceholderTextCanResize = "可調整大小";
    public const string P2PlaceholderTextDisableResize = "禁止調整大小";

    public const string P2SearchButtonTextSearch = "搜索";

    public const string P2SearchButtonTextText = "搜索一下";
    public const string PageSubtitle = "采集單行文本、搜索輸入、密碼和多行文本。";
    public const string PageDescription = "LineEdit 覆蓋標準輸入框、前後置附加、清除操作、密碼顯示、前綴後綴、校驗狀態、SearchEdit 和 TextArea 場景。";
    public const string ComponentCategory = "數據錄入";
    public const string ComponentStatusStable = "穩定";
    public const string ScenarioExamples = "示例";

}
