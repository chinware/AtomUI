using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

[LanguageProvider(LanguageCode.zh_CN, StepsShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string ScenarioBasic = "基础";
    public const string ScenarioInteractive = "交互";
    public const string ScenarioVertical = "垂直";
    public const string ScenarioDotClickable = "点状与可点击";
    public const string ScenarioNavigation = "导航";
    public const string ScenarioProgress = "进度";
    public const string ScenarioInline = "内联";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计 Token";
    public const string PageSubtitle = "引导用户理解有顺序的任务和流程状态。";
    public const string PageDescription = "Steps 用于展示任务序列、进度、导航状态以及可选的步骤内容，适合需要清晰阶段感的流程。";
    public const string ComponentCategory = "导航";
    public const string ComponentStatusStable = "稳定";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string ApiPropertyCurrentStep = "设置或绑定当前激活步骤索引。";
    public const string ApiPropertyInitialStep = "设置控件首次配置时的初始激活步骤。";
    public const string ApiPropertyProgressValue = "显示步骤项进度时当前步骤使用的百分比。";
    public const string ApiPropertyCurrentStepStatus = "应用到当前步骤的状态。";
    public const string ApiPropertyOrientation = "控制水平或垂直步骤布局。";
    public const string ApiPropertyLabelPlacement = "控制标签水平或垂直排列。";
    public const string ApiPropertySizeType = "控制步骤条视觉尺寸。";
    public const string ApiPropertyItemIndicatorType = "控制指示器样式，例如默认或点状。";
    public const string ApiPropertyStyle = "控制视觉风格，包括导航和内联模式。";
    public const string ApiPropertyIsItemClickable = "允许用户直接选择步骤项。";
    public const string ApiPropertyIsShowItemProgress = "在当前步骤指示器内显示进度。";
    public const string ApiPropertyContentTemplate = "当前步骤内容使用的模板。";
    public const string ApiPropertyCurrentContent = "Steps 暴露的当前选中步骤内容。";
    public const string ApiPropertyStepsItemSubHeader = "显示在步骤标题附近的辅助标题内容。";
    public const string ApiPropertyStepsItemDescription = "显示在步骤标题下方的描述内容。";
    public const string ApiPropertyStepsItemIcon = "显示在步骤指示器中的自定义图标。";
    public const string ApiPropertyStepsItemStatus = "单个步骤项的显式状态。";
    public const string TokenNameDescriptionMaxWidth = "步骤描述区域的最大宽度。";
    public const string TokenNameIconSize = "默认步骤指示器容器尺寸。";
    public const string TokenNameIconFontSize = "默认步骤指示器图标字号。";
    public const string TokenNameIconSizeSM = "小号步骤指示器尺寸。";
    public const string TokenNameDotSize = "点状指示器尺寸。";
    public const string TokenNameDotCurrentSize = "当前点状指示器尺寸。";
    public const string TokenNameDotLineThickness = "点状连接线粗细。";
    public const string TokenNameHorizontalHeaderMargin = "水平步骤的标题外间距。";
    public const string TokenNameVerticalItemSpacing = "垂直步骤项之间的间距。";
    public const string TokenNameVerticalDescriptionPadding = "垂直步骤描述使用的内间距。";
    public const string TokenNameStepsNavActiveColor = "导航步骤使用的激活色。";
    public const string TokenNameStepsProgressSize = "步骤项进度使用的尺寸。";
    public const string TokenNameInlineDotSize = "内联步骤使用的点状尺寸。";
    public const string TokenNameInlineItemPadding = "内联步骤项使用的内间距。";
    public const string TokenNameProcessIconBgColor = "进行中步骤指示器背景色。";
    public const string TokenNameFinishTailColor = "已完成步骤连接线颜色。";
    public const string TokenNameErrorIconBgColor = "错误步骤指示器背景色。";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusStable = "稳定";
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的步骤条。";
    public const string MiniVersionTitle = "迷你版本";
    public const string MiniVersionDescription = "将 SizeType 设置为 Small 可获得迷你版本。";
    public const string WithIconTitle = "带图标";
    public const string WithIconDescription = "可以通过为项目设置 icon 属性使用自定义图标。";
    public const string SwitchStepTitle = "切换步骤";
    public const string SwitchStepDescription = "配合内容和按钮展示流程进度。";
    public const string VerticalTitle = "垂直方向";
    public const string VerticalDescription = "垂直方向的简单步骤条。";
    public const string VerticalMiniVersionTitle = "垂直迷你版本";
    public const string VerticalMiniVersionDescription = "垂直方向的简单迷你步骤条。";
    public const string ErrorStatusTitle = "错误状态";
    public const string ErrorStatusDescription = "通过 Steps 的 status 可以指定当前步骤状态。";
    public const string DotStyleTitle = "点状样式";
    public const string DotStyleDescription = "带进度点样式的步骤条。";
    public const string DotStyleVerticalTitle = "垂直点状样式";
    public const string DotStyleVerticalDescription = "垂直方向带进度点样式的步骤条。";
    public const string ClickableTitle = "可点击";
    public const string ClickableDescription = "设置 IsItemClickable=true 可让步骤项可点击。";
    public const string NavigationStepsTitle = "导航步骤";
    public const string NavigationStepsDescription = "导航式步骤。";
    public const string StepsWithProgressTitle = "带进度的步骤";
    public const string StepsWithProgressDescription = "带进度的步骤条。";
    public const string LabelPlacementTitle = "标签位置";
    public const string LabelPlacementDescription = "将 labelPlacement 设置为 vertical。";
    public const string InlineStepsTitle = "内联步骤";
    public const string InlineStepsDescription = "内联类型步骤，适合在列表内容场景中展示对象的流程和当前状态。";
    public const string P2DescriptionThisIsADescription = "这是一段描述。";
    public const string P2HeaderFinished = "已完成";
    public const string P2HeaderInProgress = "进行中";
    public const string P2HeaderWaiting = "等待中";
    public const string P2HeaderLogin = "登录";
    public const string P2HeaderVerification = "验证";
    public const string P2HeaderPay = "支付";
    public const string P2HeaderDone = "完成";
    public const string P2HeaderFirst = "第一项";
    public const string P2HeaderSecond = "第二项";
    public const string P2HeaderThird = "第三项";
    public const string P2HeaderStepN1 = "步骤 1";
    public const string P2HeaderStepN2 = "步骤 2";
    public const string P2HeaderStepN3 = "步骤 3";
    public const string P2HeaderStepN4 = "步骤 4";
    public const string P2HeaderFinishN1 = "完成 1";
    public const string P2HeaderFinishN2 = "完成 2";
    public const string P2HeaderCurrentProcess = "当前进行中";
    public const string P2HeaderWait = "等待";
    public const string P2SubHeaderLeftTime = "剩余 00:00:08";
    public const string P2SubHeaderWaitingForLongTime = "等待较长时间";
    public const string P2TextAntDesignTitleN1 = "Ant Design 标题 1";
    public const string P2TextAntDesignADesignLanguageForBackgroundApplications = "Ant Design 是由 Ant UED 团队提炼的后台应用设计语言";
    public const string P2TextAntDesignTitleN2 = "Ant Design 标题 2";
    public const string P2TextAntDesignTitleN3 = "Ant Design 标题 3";
    public const string P2TextAntDesignTitleN4 = "Ant Design 标题 4";

    public const string P2ContentFirstContent = "第一步内容";

    public const string P2ContentSecondContent = "第二步内容";

    public const string P2ContentLastContent = "最后一步内容";

    public const string P2ContentNext = "下一步";

    public const string P2ContentPrevious = "上一步";

    public const string P2ContentDone = "完成";

}
