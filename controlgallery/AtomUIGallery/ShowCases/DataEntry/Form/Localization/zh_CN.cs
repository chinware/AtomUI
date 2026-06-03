using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Form;

[LanguageProvider(LanguageCode.zh_CN, FormShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicUsageTitle = "基础用法";
    public const string BasicUsageDescription = "基础表单数据控制，包含布局、初始值、校验和提交。";
    public const string FormMethodsTitle = "表单方法";
    public const string FormMethodsDescription = "通过 Form 调用表单方法。";
    public const string CustomizedFormControlsTitle = "自定义表单控件";
    public const string CustomizedFormControlsDescription = "Form 中也可以使用自定义或第三方表单控件。控件必须实现 IFormItemAware：";
    public const string OtherFormControlsTitle = "其他表单控件";
    public const string OtherFormControlsDescription = "演示上述示例中未展示的表单控件校验配置。";
    public const string DynamicFormItemTitle = "动态表单项";
    public const string DynamicFormItemDescription = "动态添加或移除表单项。add 函数支持配置初始值。";
    public const string FormLayoutTitle = "表单布局";
    public const string FormLayoutDescription = "表单有三种布局：水平、垂直和行内。";
    public const string LabelCanWrapTitle = "标签可换行";
    public const string LabelCanWrapDescription = "开启 labelWrap 后，标签文本过长时会自动换行。";
    public const string InlineLoginFormTitle = "行内登录表单";
    public const string InlineLoginFormDescription = "行内登录表单常用于导航栏。";
    public const string LoginFormTitle = "登录表单";
    public const string LoginFormDescription = "普通登录表单，可以包含更多元素。";
    public const string RegistrationTitle = "注册";
    public const string RegistrationDescription = "填写此表单创建新账号。";
    public const string FormVariantsTitle = "表单变体";
    public const string FormVariantsDescription = "改变表单中所有组件的变体，可选描边、填充、无边框和下划线。";
    public const string RequiredStyleTitle = "必填样式";
    public const string RequiredStyleDescription = "通过 requiredMark 切换必填或可选样式。";
    public const string NoBlockRuleTitle = "非阻塞规则";
    public const string NoBlockRuleDescription = "带 warningOnly 的规则不会阻止表单提交。";
    public const string ValidateTriggerTitle = "校验触发时机";
    public const string ValidateTriggerDescription = "在异步校验场景中，高频校验会给后端带来压力。可以通过 validateTrigger 改变校验时机，通过 validateDebounce 改变校验频率，或通过 validateFirst 设置校验短路。";
    public const string ValidateOnlyTitle = "仅校验";
    public const string ValidateOnlyDescription = "通过 validateFields 的 validateOnly 动态调整提交按钮的禁用状态。";
    public const string TimeRelatedControlsTitle = "时间相关控件";
    public const string TimeRelatedControlsDescription = "时间相关组件的值是 dayjs 对象，提交到服务端前需要预处理。";
    public const string CustomizedValidationTitle = "自定义校验";
    public const string CustomizedValidationDescription = "不使用 Form 时，也可以通过 validateStatus、help、hasFeedback 等属性自定义校验状态和信息。";
    public const string ScenarioBasic = "基础";
    public const string ScenarioLayout = "布局";
    public const string ScenarioStates = "状态";
    public const string ScenarioValidation = "校验";
    public const string ScenarioDynamic = "动态";
    public const string ScenarioPresets = "预设";
    public const string ScenarioControls = "控件";
    public const string P2PlaceholderTextSelectAOptionAndChangeInputTextAbove = "请选择一个选项并修改上方输入内容";
    public const string P2HeaderMale = "男";
    public const string P2HeaderFemale = "女";
    public const string P2HeaderOther = "其他";
    public const string P2TextChina = "中国";
    public const string P2PlaceholderTextPleaseSelectACountry = "请选择国家";
    public const string P2HeaderChina = "中国";
    public const string P2HeaderUSA = "美国";
    public const string P2PlaceholderTextPleaseSelectFavouriteColors = "请选择喜欢的颜色";
    public const string P2HeaderRed = "红色";
    public const string P2HeaderGreen = "绿色";
    public const string P2HeaderBlue = "蓝色";
    public const string P2PlaceholderTextUsername = "用户名";
    public const string P2PlaceholderTextPassword = "密码";
    public const string P2TooltipWhatDoYouWantOthersToCallYou = "你希望别人如何称呼你？";
    public const string P2HeaderZhejiang = "浙江";
    public const string P2HeaderHangzhou = "杭州";
    public const string P2HeaderWestLake = "西湖";
    public const string P2HeaderJiangsu = "江苏";
    public const string P2HeaderNanjing = "南京";
    public const string P2HeaderZhongHuaMen = "中华门";
    public const string P2PlaceholderTextWebsite = "网站";
    public const string P2HeaderMale2 = "男";
    public const string P2HeaderFemale2 = "女";
    public const string P2HeaderOther2 = "其他";
    public const string P2HeaderDemo = "演示";
    public const string P2HeaderLight = "浅色";
    public const string P2HeaderBamboo = "竹子";
    public const string P2PlaceholderTextSelectDate = "选择日期";
    public const string P2PlaceholderTextStartDate = "开始日期";
    public const string P2SecondaryPlaceholderTextEndDate = "结束日期";
    public const string P2HeaderParentN1 = "父节点 1";
    public const string P2HeaderChildN1N1 = "子节点 1-1";
    public const string P2HeaderGrandchildN1N1N1 = "孙节点 1-1-1";
    public const string P2HeaderChildN1N2 = "子节点 1-2";
    public const string P2HeaderParentN2 = "父节点 2";
    public const string P2HeaderChildN2N1 = "子节点 2-1";
    public const string P2PlaceholderTextPleaseSelect = "请选择";
    public const string P2TooltipThisIsARequiredField = "这是必填字段";
    public const string P2PlaceholderTextInputPlaceholder = "输入占位符";
    public const string P2TooltipTooltipWithCustomizeIcon = "带自定义图标的提示";
    public const string P2HeaderBbb = "BBB";
    public const string P2HeaderAaa = "AAA";
    public const string P2PlaceholderTextValidateRequiredOnblur = "失焦时校验必填";
    public const string P2PlaceholderTextValidateRequiredDebounceAfterN1s = "1 秒防抖后校验必填";
    public const string P2PlaceholderTextValidateOneByOne = "逐项校验";
    public const string P2PlaceholderTextSelectDate2 = "选择日期！";
    public const string P2PlaceholderTextStartTime = "开始时间";
    public const string P2PlaceholderTextUnavailableChoice = "不可用选项";
    public const string P2PlaceholderTextWarning = "警告";
    public const string P2PlaceholderTextIMTheContentIsBeingValidated = "我是正在校验的内容";
    public const string P2PlaceholderTextIMTheContent = "我是内容";
    public const string P2PlaceholderTextSelectTime = "选择时间";
    public const string P2PlaceholderTextIMSelect = "我是选择器";
    public const string P2HeaderOptionN1 = "选项 1";
    public const string P2HeaderOptionN2 = "选项 2";
    public const string P2HeaderOptionN3 = "选项 3";
    public const string P2HeaderXx = "xx";
    public const string P2PlaceholderTextIMTreeselect = "我是树选择器";
    public const string P2PlaceholderTextWithAllowclear = "带清除按钮";
    public const string P2PlaceholderTextWithInputPassword = "密码输入框";
    public const string P2PlaceholderTextWithInputPasswordAndAllowclear = "带清除按钮的密码输入框";
    public const string P2ContentRememberMe = "记住我";
    public const string P2ContentItemN1 = "项目 1";
    public const string P2ContentItemN2 = "项目 2";
    public const string P2ContentItemN3 = "项目 3";
    public const string P2ContentA = "A";
    public const string P2ContentB = "B";
    public const string P2ContentC = "C";
    public const string P2ContentD = "D";
    public const string P2ContentE = "E";
    public const string P2ContentF = "F";
    public const string P2ContentClickToUpload = "点击上传";
    public const string P2ContentAddField = "添加字段";
    public const string P2ContentAddFieldAtHead = "在开头添加字段";
    public const string P2TextFormLayout = "表单布局：";
    public const string P2ContentHorizontal = "水平";
    public const string P2ContentVertical = "垂直";
    public const string P2ContentInline = "行内";
    public const string P2ContentForgotPassword = "忘记密码";
    public const string P2TextOr = "或";
    public const string P2ContentRegisterNow = "立即注册！";
    public const string P2TextIHaveReadThe = "我已阅读";
    public const string P2ContentAgreement = "协议";
    public const string P2ContentFormDisabled = "禁用表单";
    public const string P2ContentCheckbox = "复选框";
    public const string P2ContentApple = "苹果";
    public const string P2ContentPear = "梨";
    public const string P2TextUpload = "上传";
    public const string P2ContentButton = "按钮";
    public const string P2TextFormVariant = "表单变体：";
    public const string P2ContentOutlined = "描边";
    public const string P2ContentFilled = "填充";
    public const string P2ContentBorderless = "无边框";
    public const string P2ContentUnderlined = "下划线";
    public const string P2TextRequiredMark = "必填标记：";
    public const string P2ContentDefault = "默认";
    public const string P2ContentOptional = "可选";
    public const string P2ContentHidden = "隐藏";
    public const string P2ContentCustomize = "自定义";
    public const string P2ContentRequired = "必填";
    public const string P2ContentOptional2 = "可选";
    public const string P2ContentSmall = "小号";
    public const string P2ContentMiddle = "中号";
    public const string P2ContentLarge = "大号";
    public const string P2ContentFill = "填充";

    public const string P2LabelTextUrl = "URL";

    public const string P2MessagePleaseEnterURL = "请输入 URL！";

    public const string P2MessageURLIsNotAValidUrl = "URL 格式无效！";

    public const string P2MessageURLMustBeAtLeastN6Characters = "URL 至少需要 6 个字符！";

    public const string P2LabelTextFieldA = "字段 A";

    public const string P2MessageFieldAMustBeUpToN3Characters = "字段 A 最多 3 个字符！";

    public const string P2LabelTextFieldB = "字段 B";

    public const string P2LabelTextFieldC = "字段 C";

    public const string P2MessageFieldAMustBeUpToN6Characters = "字段 A 最多 6 个字符！";

    public const string P2MessageContinueInputToExceedN6Chars = "继续输入直到超过 6 个字符！";

    public const string P2LabelTextName = "姓名";

    public const string P2MessagePleaseEnterName = "请输入姓名！";

    public const string P2LabelTextAge = "年龄";

    public const string P2MessagePleaseEnterAge = "请输入年龄！";

    public const string P2LabelTextDatePicker = "日期选择器";

    public const string P2MessagePleaseSelectTime = "请选择时间！";
    public const string P2HelpShouldBeCombinationOfNumbersAndAlphabets = "应由数字和字母组合而成";
    public const string P2HelpTheInformationIsBeingValidated = "信息正在校验中";
    public const string P2HelpSomethingBreaksTheRule = "有内容违反了规则。";
    public const string P2HelpNeedToBeChecked = "需要勾选。";
    public const string P2HelpShouldHaveSomething = "应填写一些内容";
    public const string P2HelpLongUpload = "一段较长的上传帮助文本";

    public const string P2LabelTextDatePickerShowTime = "日期选择器（显示时间）";

    public const string P2LabelTextRangePicker = "范围选择器";

    public const string P2LabelTextRangePickerShowTime = "范围选择器（显示时间）";

    public const string P2LabelTextTimePicker = "时间选择器";

    public const string P2LabelTextFail = "失败";

    public const string P2LabelTextWarning = "警告";

    public const string P2LabelTextValidating = "校验中";

    public const string P2LabelTextSuccess = "成功";

    public const string P2LabelTextError = "错误";

    public const string P2LabelTextHorizontal = "水平";

    public const string P2LabelTextVertical = "垂直";

    public const string P2LabelTextVertical2 = "垂直2";

    public const string P2LabelTextNormalLabel = "普通标签";

    public const string P2MessagePleaseInputYourUsername = "请输入用户名！";

    public const string P2LabelTextASuperLongLabelText = "一段超长标签文本";

    public const string P2LabelTextPassengers = "乘客";

    public const string P2MessagePleaseInputPassengerSNameOrDeleteThisField = "请输入乘客姓名，或删除该字段！";

    public const string P2LabelTextPassengers1 = "乘客1";

    public const string P2LabelTextCheckbox = "复选框";

    public const string P2LabelTextRadio = "单选框";

    public const string P2LabelTextInput = "输入框";

    public const string P2LabelTextSelect = "选择器";

    public const string P2LabelTextTreeSelect = "树选择";

    public const string P2LabelTextCascader = "级联选择";

    public const string P2LabelTextInputNumber = "数字输入框";

    public const string P2LabelTextTextArea = "文本域";

    public const string P2LabelTextSwitch = "开关";

    public const string P2LabelTextUpload = "上传";

    public const string P2LabelTextButton = "按钮";

    public const string P2LabelTextSlider = "滑块";

    public const string P2LabelTextColorPicker = "颜色选择器";

    public const string P2LabelTextRate = "评分";

    public const string P2LabelTextMentions = "提及";

    public const string P2LabelTextTree = "树";

    public const string P2MessagePleaseInput = "请输入！";

    public const string P2LabelTextUsername = "用户名";

    public const string P2LabelTextPassword = "密码";

    public const string P2MessagePleaseInputYourPassword = "请输入密码！";

    public const string P2LabelTextNote = "备注";

    public const string P2MessagePleaseEnterNote = "请输入备注";

    public const string P2LabelTextGender = "性别";

    public const string P2MessagePleaseEnterGender = "请选择性别";

    public const string P2LabelTextCustomizeGender = "自定义性别";

    public const string P2MessagePleaseEnterCustomizeGender = "请输入自定义性别";

    public const string P2LabelTextPrice = "价格";

    public const string P2MessagePriceMustBeGreaterThanZero = "价格必须大于零！";

    public const string P2LabelTextPlainText = "纯文本";

    public const string P2MessagePleaseSelectYourCountry = "请选择国家！";

    public const string P2LabelTextSelectMultiple = "选择器（多选）";

    public const string P2MessagePleaseSelectYourFavouriteColors = "请选择喜欢的颜色！";

    public const string P2LabelTextRadioGroup = "单选组";

    public const string P2LabelTextOptionButtonGroup = "选项按钮组";

    public const string P2LabelTextCheckboxGroup = "复选框组";

    public const string P2LabelTextDragger = "拖拽上传";

    public const string P2MessageColorIsRequired = "颜色为必填项！";

    public const string P2ContentLogIn = "登录";

    public const string P2LabelTextEmail = "邮箱";

    public const string P2MessagePleaseInputYourEMail = "请输入邮箱！";

    public const string P2LabelTextConfirmPassword = "确认密码";

    public const string P2MessagePleaseConfirmYourPassword = "请确认密码！";

    public const string P2LabelTextNickname = "昵称";

    public const string P2MessagePleaseInputYourNickname = "请输入昵称！";

    public const string P2LabelTextHabitualResidence = "常住地";

    public const string P2MessagePleaseSelectYourHabitualResidence = "请选择常住地！";

    public const string P2LabelTextPhoneNumber = "手机号";

    public const string P2MessagePleaseInputYourPhoneNumber = "请输入手机号！";

    public const string P2LabelTextDonation = "捐赠";

    public const string P2MessagePleaseInputDonationAmount = "请输入捐赠金额！";

    public const string P2LabelTextWebsite = "网站";

    public const string P2MessagePleaseInputWebsite = "请输入网站！";

    public const string P2LabelTextIntro = "简介";

    public const string P2MessagePleaseInputIntro = "请输入简介！";

    public const string P2MessagePleaseSelectGender = "请选择性别！";

    public const string P2LabelTextCaptcha = "验证码";

    public const string P2MessagePleaseInputTheCaptchaYouGot = "请输入收到的验证码！";

    public const string P2ContentRegister = "注册";

    public const string P3DynamicPassengerLabelFormat = "乘客_{0}";

    public const string P3SubmitSuccessMessage = "提交成功！";

    public const string P3SubmitFailedMessage = "提交失败！";

    public const string P3GenderNoteFormat = "你好，{0}！";

    public const string P3GetCaptchaButtonText = "获取验证码";

    public const string P3CurrencyRmbHeader = "人民币";

    public const string P3CurrencyDollarHeader = "美元";

    protected override Type GetResourceKindType() => typeof(FormShowCaseLangResourceKind);
}
