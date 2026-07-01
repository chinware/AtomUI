using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Space;

[LanguageProvider(LanguageCode.zh_CN, SpaceShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "为拥挤的组件添加水平间距。";
    public const string VerticalSpaceTitle = "垂直间距";
    public const string VerticalSpaceDescription = "为拥挤的组件添加垂直间距。";
    public const string SizeTitle = "间距尺寸";
    public const string SizeDescription = "使用 size 设置间距，预设 small、middle、large 三种尺寸，也可以自定义间距。未设置 size 时，间距为 small。";
    public const string AlignTitle = "对齐";
    public const string AlignDescription = "配置项目对齐方式。";
    public const string WrapTitle = "自动换行";
    public const string WrapDescription = "自动换行。";
    public const string SplitTitle = "分隔符";
    public const string SplitDescription = "为拥挤的组件添加分隔符。";
    public const string CompactFormTitle = "表单紧凑模式";
    public const string CompactFormDescription = "表单组件的紧凑模式。";
    public const string CompactButtonTitle = "按钮紧凑模式";
    public const string CompactButtonDescription = "按钮组件的紧凑示例。";
    public const string VerticalCompactTitle = "垂直紧凑模式";
    public const string VerticalCompactDescription = "Space.Compact 的垂直模式，仅支持 Button。";
    public const string ScenarioBasic = "基础";
    public const string ScenarioSize = "尺寸";
    public const string ScenarioAlign = "对齐";
    public const string ScenarioCompactForm = "紧凑表单";
    public const string ScenarioCompactButton = "紧凑按钮";
    public const string ScenarioExamples = "示例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "设计变量";
    public const string PageSubtitle = "用一致的间距或紧凑组合排列控件。";
    public const string PageDescription = "Space 提供水平和垂直间距、项目对齐、分隔渲染以及紧凑输入和按钮组合，适合高密度工具界面。";
    public const string ComponentCategory = "布局";
    public const string ComponentStatusStable = "稳定";
    public const string ApiColumnProperty = "属性";
    public const string ApiColumnDescription = "说明";
    public const string ApiColumnType = "类型";
    public const string ApiColumnDefault = "默认值";
    public const string ApiPropertySpaceItemSpacing = "控制主轴方向上项目之间的间距。";
    public const string ApiPropertySpaceLineSpacing = "控制换行行或垂直行之间的间距。";
    public const string ApiPropertySpaceOrientation = "设置子控件按水平或垂直方向排列。";
    public const string ApiPropertySpaceItemsAlignment = "控制每一行内子控件在交叉轴上的对齐方式。";
    public const string ApiPropertySpaceSizeType = "使用预设 token 间距尺寸或自定义间距模式。";
    public const string ApiPropertySpaceItemWidth = "设置后为每个项目应用固定宽度。";
    public const string ApiPropertySpaceItemHeight = "设置后为每个项目应用固定高度。";
    public const string ApiPropertySpaceSplitTemplate = "在相邻项目之间创建分隔内容。";
    public const string ApiPropertyCompactSpaceOrientation = "设置紧凑组合的方向。";
    public const string ApiPropertyCompactSpaceSizeType = "向兼容的紧凑子控件传递尺寸。";
    public const string ApiPropertyCompactSpaceItemSize = "用于控制单个紧凑项比例或固定尺寸的附加属性。";
    public const string TokenColumnToken = "变量";
    public const string TokenColumnDescription = "说明";
    public const string TokenColumnScope = "范围";
    public const string TokenColumnStatus = "状态";
    public const string TokenScopeComponent = "组件";
    public const string TokenStatusDefault = "默认";
    public const string TokenNameGapSmallSize = "小号间距尺寸。";
    public const string TokenNameGapMiddleSize = "中号间距尺寸。";
    public const string TokenNameGapLargeSize = "大号间距尺寸。";
    public const string TokenNameAddonBg = "紧凑附加项背景色。";
    public const string TokenNameAddOnPadding = "默认紧凑附加项水平内边距。";
    public const string TokenNameAddOnPaddingSM = "小号紧凑附加项水平内边距。";
    public const string TokenNameAddOnPaddingLG = "大号紧凑附加项水平内边距。";
    public const string P2ConfirmContentAreYouSureToDeleteThisTask = "确定要删除这个任务吗？";
    public const string P2OkTextOk = "确定";
    public const string P2CancelTextCancel = "取消";
    public const string P2TitleDeleteTheTask = "删除任务";
    public const string P2HeaderCard = "卡片";
    public const string P2HeaderReport = "举报";
    public const string P2HeaderMail = "邮件";
    public const string P2HeaderMobile = "手机";
    public const string P2HeaderN1stItem = "第 1 项";
    public const string P2HeaderN2ndItem = "第 2 项";
    public const string P2HeaderN3rdItem = "第 3 项";
    public const string P2HeaderZhejiang = "浙江";
    public const string P2HeaderJiangsu = "江苏";
    public const string P2TextXihuDistrictHangzhou = "杭州市西湖区";
    public const string P2TextN1 = "+1";
    public const string P2HeaderOption1 = "选项 1";
    public const string P2HeaderOption2 = "选项 2";
    public const string P2TextInputContent = "输入内容";
    public const string P2HeaderOption1N1 = "选项 1-1";
    public const string P2HeaderOption2N1 = "选项 2-1";
    public const string P2HeaderOption2N2 = "选项 2-2";
    public const string P2HeaderBetween = "介于";
    public const string P2HeaderExcept = "排除";
    public const string P2PlaceholderTextMinimum = "最小值";
    public const string P2PlaceholderTextText = "~";
    public const string P2PlaceholderTextMaximum = "最大值";
    public const string P2HeaderSignUp = "注册";
    public const string P2HeaderSignIn = "登录";
    public const string P2PlaceholderTextEmail = "邮箱";
    public const string P2HeaderTextN1 = "文本 1";
    public const string P2HeaderTextN2 = "文本 2";
    public const string P2PlaceholderTextSelectTime = "选择时间";
    public const string P2PlaceholderTextSelectAddress = "选择地址";
    public const string P2HeaderHangzhou = "杭州";
    public const string P2HeaderWestLake = "西湖";
    public const string P2HeaderLingyinShi = "灵隐寺";
    public const string P2HeaderNanjing = "南京";
    public const string P2HeaderZhongHuaMen = "中华门";
    public const string P2PlaceholderTextStartTime = "开始时间";
    public const string P2SecondaryPlaceholderTextEndTime = "结束时间";
    public const string P2PlaceholderTextPleaseSelect = "请选择";
    public const string P2HeaderParentN1 = "父节点 1";
    public const string P2HeaderParentN1N0 = "父节点 1-0";
    public const string P2HeaderLeaf1 = "叶子节点1";
    public const string P2HeaderLeaf2 = "叶子节点2";
    public const string P2HeaderParentN1N1 = "父节点 1-1";
    public const string P2HeaderLeaf3 = "叶子节点3";
    public const string P2PlaceholderTextInputHere = "在此输入";
    public const string P2PlaceholderTextAnotherInput = "另一个输入";
    public const string P2TextCenter = "居中";
    public const string P2ContentPrimary = "主要";
    public const string P2TextBlock = "块";
    public const string P2ContentButton = "按钮";
    public const string P2ContentLink = "链接";
    public const string P2TextSpace = "Space：";
    public const string P2ContentClickToUpload = "点击上传";
    public const string P2ContentConfirm = "确认";
    public const string P2TextCardContent = "卡片内容";
    public const string P2ContentButtonN1 = "按钮 1";
    public const string P2ContentButtonN2 = "按钮 2";
    public const string P2ContentButtonN3 = "按钮 3";
    public const string P2ContentButtonN4 = "按钮 4";
    public const string P2ContentSubmit = "提交";
    public const string P2ContentSearch = "查询";
    public const string P2ContentSmall = "小号";
    public const string P2ContentMiddle = "中号";
    public const string P2ContentLarge = "大号";
    public const string P2ContentCustom = "自定义";
    public const string P2ContentDefault = "默认";
    public const string P2ContentDashed = "虚线";

    public const string P2ToolTipTipCopyGitUrl = "复制 Git URL";

    public const string P2ToolTipTipLike = "点赞";

    public const string P2ToolTipTipComment = "评论";

    public const string P2ToolTipTipStar = "收藏";

    public const string P2ToolTipTipHeart = "喜欢";

    public const string P2ToolTipTipShare = "分享";

    public const string P2ToolTipTipDownload = "下载";

    public const string P2ToolTipTipTooltip = "提示";

    protected override Type GetResourceKindType() => typeof(SpaceShowCaseLangResourceKind);
}
