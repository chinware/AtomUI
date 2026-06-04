using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Form;

[LanguageProvider(LanguageCode.zh_TW, FormShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicUsageTitle = "基礎用法";
    public const string BasicUsageDescription = "基礎表單數據控制，包含佈局、初始值、校驗和提交。";
    public const string FormMethodsTitle = "表單方法";
    public const string FormMethodsDescription = "通過 Form 調用表單方法。";
    public const string CustomizedFormControlsTitle = "自定義表單控件";
    public const string CustomizedFormControlsDescription = "Form 中也可以使用自定義或第三方表單控件。控件必須實現 IFormItemAware：";
    public const string OtherFormControlsTitle = "其他表單控件";
    public const string OtherFormControlsDescription = "演示上述示例中未展示的表單控件校驗配置。";
    public const string DynamicFormItemTitle = "動態表單項";
    public const string DynamicFormItemDescription = "動態添加或移除表單項。add 函數支持配置初始值。";
    public const string FormLayoutTitle = "表單佈局";
    public const string FormLayoutDescription = "表單有三種佈局：水平、垂直和行內。";
    public const string LabelCanWrapTitle = "標籤可換行";
    public const string LabelCanWrapDescription = "開啓 labelWrap 後，標籤文本過長時會自動換行。";
    public const string InlineLoginFormTitle = "行內登錄表單";
    public const string InlineLoginFormDescription = "行內登錄表單常用於導航欄。";
    public const string LoginFormTitle = "登錄表單";
    public const string LoginFormDescription = "普通登錄表單，可以包含更多元素。";
    public const string RegistrationTitle = "註冊";
    public const string RegistrationDescription = "填寫此表單創建新賬號。";
    public const string FormVariantsTitle = "表單變體";
    public const string FormVariantsDescription = "改變表單中所有組件的變體，可選描邊、填充、無邊框和下划線。";
    public const string RequiredStyleTitle = "必填樣式";
    public const string RequiredStyleDescription = "通過 requiredMark 切換必填或可選樣式。";
    public const string NoBlockRuleTitle = "非阻塞規則";
    public const string NoBlockRuleDescription = "帶 warningOnly 的規則不會阻止表單提交。";
    public const string ValidateTriggerTitle = "校驗觸發時機";
    public const string ValidateTriggerDescription = "在異步校驗場景中，高頻校驗會給後端帶來壓力。可以通過 validateTrigger 改變校驗時機，通過 validateDebounce 改變校驗頻率，或通過 validateFirst 設置校驗短路。";
    public const string ValidateOnlyTitle = "僅校驗";
    public const string ValidateOnlyDescription = "通過 validateFields 的 validateOnly 動態調整提交按鈕的禁用狀態。";
    public const string TimeRelatedControlsTitle = "時間相關控件";
    public const string TimeRelatedControlsDescription = "時間相關組件的值是 dayjs 對象，提交到服務端前需要預處理。";
    public const string CustomizedValidationTitle = "自定義校驗";
    public const string CustomizedValidationDescription = "不使用 Form 時，也可以通過 validateStatus、help、hasFeedback 等屬性自定義校驗狀態和信息。";
    public const string ScenarioBasic = "基礎";
    public const string ScenarioLayout = "佈局";
    public const string ScenarioStates = "狀態";
    public const string ScenarioValidation = "校驗";
    public const string ScenarioDynamic = "動態";
    public const string ScenarioPresets = "預設";
    public const string ScenarioControls = "控件";
    public const string P2PlaceholderTextSelectAOptionAndChangeInputTextAbove = "請選擇一個選項並修改上方輸入內容";
    public const string P2HeaderMale = "男";
    public const string P2HeaderFemale = "女";
    public const string P2HeaderOther = "其他";
    public const string P2TextChina = "中國";
    public const string P2PlaceholderTextPleaseSelectACountry = "請選擇國家";
    public const string P2HeaderChina = "中國";
    public const string P2HeaderUSA = "美國";
    public const string P2PlaceholderTextPleaseSelectFavouriteColors = "請選擇喜歡的顏色";
    public const string P2HeaderRed = "紅色";
    public const string P2HeaderGreen = "綠色";
    public const string P2HeaderBlue = "藍色";
    public const string P2PlaceholderTextUsername = "用戶名";
    public const string P2PlaceholderTextPassword = "密碼";
    public const string P2TooltipWhatDoYouWantOthersToCallYou = "你希望別人如何稱呼你？";
    public const string P2HeaderZhejiang = "浙江";
    public const string P2HeaderHangzhou = "杭州";
    public const string P2HeaderWestLake = "西湖";
    public const string P2HeaderJiangsu = "江蘇";
    public const string P2HeaderNanjing = "南京";
    public const string P2HeaderZhongHuaMen = "中華門";
    public const string P2PlaceholderTextWebsite = "網站";
    public const string P2HeaderMale2 = "男";
    public const string P2HeaderFemale2 = "女";
    public const string P2HeaderOther2 = "其他";
    public const string P2HeaderDemo = "演示";
    public const string P2HeaderLight = "淺色";
    public const string P2HeaderBamboo = "竹子";
    public const string P2PlaceholderTextSelectDate = "選擇日期";
    public const string P2PlaceholderTextStartDate = "開始日期";
    public const string P2SecondaryPlaceholderTextEndDate = "結束日期";
    public const string P2HeaderParentN1 = "父節點 1";
    public const string P2HeaderChildN1N1 = "子節點 1-1";
    public const string P2HeaderGrandchildN1N1N1 = "孫節點 1-1-1";
    public const string P2HeaderChildN1N2 = "子節點 1-2";
    public const string P2HeaderParentN2 = "父節點 2";
    public const string P2HeaderChildN2N1 = "子節點 2-1";
    public const string P2PlaceholderTextPleaseSelect = "請選擇";
    public const string P2TooltipThisIsARequiredField = "這是必填字段";
    public const string P2PlaceholderTextInputPlaceholder = "輸入佔位符";
    public const string P2TooltipTooltipWithCustomizeIcon = "帶自定義圖標的提示";
    public const string P2HeaderBbb = "BBB";
    public const string P2HeaderAaa = "AAA";
    public const string P2PlaceholderTextValidateRequiredOnblur = "失焦時校驗必填";
    public const string P2PlaceholderTextValidateRequiredDebounceAfterN1s = "1 秒防抖後校驗必填";
    public const string P2PlaceholderTextValidateOneByOne = "逐項校驗";
    public const string P2PlaceholderTextSelectDate2 = "選擇日期！";
    public const string P2PlaceholderTextStartTime = "開始時間";
    public const string P2PlaceholderTextUnavailableChoice = "不可用選項";
    public const string P2PlaceholderTextWarning = "警告";
    public const string P2PlaceholderTextIMTheContentIsBeingValidated = "我是正在校驗的內容";
    public const string P2PlaceholderTextIMTheContent = "我是內容";
    public const string P2PlaceholderTextSelectTime = "選擇時間";
    public const string P2PlaceholderTextIMSelect = "我是選擇器";
    public const string P2HeaderOptionN1 = "選項 1";
    public const string P2HeaderOptionN2 = "選項 2";
    public const string P2HeaderOptionN3 = "選項 3";
    public const string P2HeaderXx = "xx";
    public const string P2PlaceholderTextIMTreeselect = "我是樹選擇器";
    public const string P2PlaceholderTextWithAllowclear = "帶清除按鈕";
    public const string P2PlaceholderTextWithInputPassword = "密碼輸入框";
    public const string P2PlaceholderTextWithInputPasswordAndAllowclear = "帶清除按鈕的密碼輸入框";
    public const string P2ContentRememberMe = "記住我";
    public const string P2ContentItemN1 = "項目 1";
    public const string P2ContentItemN2 = "項目 2";
    public const string P2ContentItemN3 = "項目 3";
    public const string P2ContentA = "A";
    public const string P2ContentB = "B";
    public const string P2ContentC = "C";
    public const string P2ContentD = "D";
    public const string P2ContentE = "E";
    public const string P2ContentF = "F";
    public const string P2ContentClickToUpload = "點擊上傳";
    public const string P2ContentAddField = "添加字段";
    public const string P2ContentAddFieldAtHead = "在開頭添加字段";
    public const string P2TextFormLayout = "表單佈局：";
    public const string P2ContentHorizontal = "水平";
    public const string P2ContentVertical = "垂直";
    public const string P2ContentInline = "行內";
    public const string P2ContentForgotPassword = "忘記密碼";
    public const string P2TextOr = "或";
    public const string P2ContentRegisterNow = "立即註冊！";
    public const string P2TextIHaveReadThe = "我已閱讀";
    public const string P2ContentAgreement = "協議";
    public const string P2ContentFormDisabled = "禁用表單";
    public const string P2ContentCheckbox = "復選框";
    public const string P2ContentApple = "蘋果";
    public const string P2ContentPear = "梨";
    public const string P2TextUpload = "上傳";
    public const string P2ContentButton = "按鈕";
    public const string P2TextFormVariant = "表單變體：";
    public const string P2ContentOutlined = "描邊";
    public const string P2ContentFilled = "填充";
    public const string P2ContentBorderless = "無邊框";
    public const string P2ContentUnderlined = "下划線";
    public const string P2TextRequiredMark = "必填標記：";
    public const string P2ContentDefault = "默認";
    public const string P2ContentOptional = "可選";
    public const string P2ContentHidden = "隱藏";
    public const string P2ContentCustomize = "自定義";
    public const string P2ContentRequired = "必填";
    public const string P2ContentOptional2 = "可選";
    public const string P2ContentSmall = "小號";
    public const string P2ContentMiddle = "中號";
    public const string P2ContentLarge = "大號";
    public const string P2ContentFill = "填充";

    public const string P2LabelTextUrl = "URL";

    public const string P2MessagePleaseEnterURL = "請輸入 URL！";

    public const string P2MessageURLIsNotAValidUrl = "URL 格式無效！";

    public const string P2MessageURLMustBeAtLeastN6Characters = "URL 至少需要 6 個字符！";

    public const string P2LabelTextFieldA = "字段 A";

    public const string P2MessageFieldAMustBeUpToN3Characters = "字段 A 最多 3 個字符！";

    public const string P2LabelTextFieldB = "字段 B";

    public const string P2LabelTextFieldC = "字段 C";

    public const string P2MessageFieldAMustBeUpToN6Characters = "字段 A 最多 6 個字符！";

    public const string P2MessageContinueInputToExceedN6Chars = "繼續輸入直到超過 6 個字符！";

    public const string P2LabelTextName = "姓名";

    public const string P2MessagePleaseEnterName = "請輸入姓名！";

    public const string P2LabelTextAge = "年齡";

    public const string P2MessagePleaseEnterAge = "請輸入年齡！";

    public const string P2LabelTextDatePicker = "日期選擇器";

    public const string P2MessagePleaseSelectTime = "請選擇時間！";
    public const string P2HelpShouldBeCombinationOfNumbersAndAlphabets = "應由數字和字母組合而成";
    public const string P2HelpTheInformationIsBeingValidated = "信息正在校驗中";
    public const string P2HelpSomethingBreaksTheRule = "有內容違反了規則。";
    public const string P2HelpNeedToBeChecked = "需要勾選。";
    public const string P2HelpShouldHaveSomething = "應填寫一些內容";
    public const string P2HelpLongUpload = "一段較長的上傳幫助文本";

    public const string P2LabelTextDatePickerShowTime = "日期選擇器（顯示時間）";

    public const string P2LabelTextRangePicker = "範圍選擇器";

    public const string P2LabelTextRangePickerShowTime = "範圍選擇器（顯示時間）";

    public const string P2LabelTextTimePicker = "時間選擇器";

    public const string P2LabelTextFail = "失敗";

    public const string P2LabelTextWarning = "警告";

    public const string P2LabelTextValidating = "校驗中";

    public const string P2LabelTextSuccess = "成功";

    public const string P2LabelTextError = "錯誤";

    public const string P2LabelTextHorizontal = "水平";

    public const string P2LabelTextVertical = "垂直";

    public const string P2LabelTextVertical2 = "垂直2";

    public const string P2LabelTextNormalLabel = "普通標籤";

    public const string P2MessagePleaseInputYourUsername = "請輸入用戶名！";

    public const string P2LabelTextASuperLongLabelText = "一段超長標籤文本";

    public const string P2LabelTextPassengers = "乘客";

    public const string P2MessagePleaseInputPassengerSNameOrDeleteThisField = "請輸入乘客姓名，或刪除該字段！";

    public const string P2LabelTextPassengers1 = "乘客1";

    public const string P2LabelTextCheckbox = "復選框";

    public const string P2LabelTextRadio = "單選框";

    public const string P2LabelTextInput = "輸入框";

    public const string P2LabelTextSelect = "選擇器";

    public const string P2LabelTextTreeSelect = "樹選擇";

    public const string P2LabelTextCascader = "級聯選擇";

    public const string P2LabelTextInputNumber = "數字輸入框";

    public const string P2LabelTextTextArea = "文本域";

    public const string P2LabelTextSwitch = "開關";

    public const string P2LabelTextUpload = "上傳";

    public const string P2LabelTextButton = "按鈕";

    public const string P2LabelTextSlider = "滑塊";

    public const string P2LabelTextColorPicker = "顏色選擇器";

    public const string P2LabelTextRate = "評分";

    public const string P2LabelTextMentions = "提及";

    public const string P2LabelTextTree = "樹";

    public const string P2MessagePleaseInput = "請輸入！";

    public const string P2LabelTextUsername = "用戶名";

    public const string P2LabelTextPassword = "密碼";

    public const string P2MessagePleaseInputYourPassword = "請輸入密碼！";

    public const string P2LabelTextNote = "備注";

    public const string P2MessagePleaseEnterNote = "請輸入備注";

    public const string P2LabelTextGender = "性別";

    public const string P2MessagePleaseEnterGender = "請選擇性別";

    public const string P2LabelTextCustomizeGender = "自定義性別";

    public const string P2MessagePleaseEnterCustomizeGender = "請輸入自定義性別";

    public const string P2LabelTextPrice = "價格";

    public const string P2MessagePriceMustBeGreaterThanZero = "價格必須大於零！";

    public const string P2LabelTextPlainText = "純文本";

    public const string P2MessagePleaseSelectYourCountry = "請選擇國家！";

    public const string P2LabelTextSelectMultiple = "選擇器（多選）";

    public const string P2MessagePleaseSelectYourFavouriteColors = "請選擇喜歡的顏色！";

    public const string P2LabelTextRadioGroup = "單選組";

    public const string P2LabelTextOptionButtonGroup = "選項按鈕組";

    public const string P2LabelTextCheckboxGroup = "復選框組";

    public const string P2LabelTextDragger = "拖拽上傳";

    public const string P2MessageColorIsRequired = "顏色為必填項！";

    public const string P2ContentLogIn = "登錄";

    public const string P2LabelTextEmail = "郵箱";

    public const string P2MessagePleaseInputYourEMail = "請輸入郵箱！";

    public const string P2LabelTextConfirmPassword = "確認密碼";

    public const string P2MessagePleaseConfirmYourPassword = "請確認密碼！";

    public const string P2LabelTextNickname = "暱稱";

    public const string P2MessagePleaseInputYourNickname = "請輸入暱稱！";

    public const string P2LabelTextHabitualResidence = "常住地";

    public const string P2MessagePleaseSelectYourHabitualResidence = "請選擇常住地！";

    public const string P2LabelTextPhoneNumber = "手機號";

    public const string P2MessagePleaseInputYourPhoneNumber = "請輸入手機號！";

    public const string P2LabelTextDonation = "捐贈";

    public const string P2MessagePleaseInputDonationAmount = "請輸入捐贈金額！";

    public const string P2LabelTextWebsite = "網站";

    public const string P2MessagePleaseInputWebsite = "請輸入網站！";

    public const string P2LabelTextIntro = "簡介";

    public const string P2MessagePleaseInputIntro = "請輸入簡介！";

    public const string P2MessagePleaseSelectGender = "請選擇性別！";

    public const string P2LabelTextCaptcha = "驗證碼";

    public const string P2MessagePleaseInputTheCaptchaYouGot = "請輸入收到的驗證碼！";

    public const string P2ContentRegister = "註冊";

    public const string P3DynamicPassengerLabelFormat = "乘客_{0}";

    public const string P3SubmitSuccessMessage = "提交成功！";

    public const string P3SubmitFailedMessage = "提交失敗！";

    public const string P3GenderNoteFormat = "你好，{0}！";

    public const string P3GetCaptchaButtonText = "獲取驗證碼";

    public const string P3CurrencyRmbHeader = "人民幣";

    public const string P3CurrencyDollarHeader = "美元";

    protected override Type GetResourceKindType() => typeof(FormShowCaseLangResourceKind);
}

