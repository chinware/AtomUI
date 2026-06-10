using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Tag;

[LanguageProvider(LanguageCode.zh_CN, TagShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "基础 Tag 用法。可通过 IsClosable 设置为可关闭，并通过 closeIcon 属性自定义关闭按钮；closeIcon 设置为 true 时显示默认关闭按钮。IsClosable Tag 支持 onClose 事件。";
    public const string ColorfulTagTitle = "多彩标签";
    public const string ColorfulTagDescription = "预设了一系列多彩标签样式，可用于不同场景。也可以设置十六进制颜色字符串来自定义颜色。";
    public const string StatusTagTitle = "状态标签";
    public const string StatusTagDescription = "预设了五种不同颜色，可以设置 success、processing、error、default 和 warning 等 color 属性表示具体状态。";
    public const string IconTitle = "图标";
    public const string IconDescription = "Tag 组件可以包含图标。可以通过设置 icon 属性，或在 Tag 内放置 Icon 组件实现。如果需要精确控制图标的位置和布局，应在 Tag 内放置 Icon 组件，而不是使用 icon 属性。";
    public const string BorderlessTitle = "无边框";
    public const string BorderlessDescription = "无边框。";
    public const string P2ContentTagN1 = "标签 1";
    public const string P2ContentLink = "链接";
    public const string P2ContentPreventDefault = "阻止默认行为";
    public const string P2ContentTagN2 = "标签 2";
    public const string P2TextPresets = "预设";
    public const string P2ContentMagenta = "品红色";
    public const string P2ContentRed = "红色";
    public const string P2ContentVolcano = "火山色";
    public const string P2ContentOrange = "橙色";
    public const string P2ContentGold = "金色";
    public const string P2ContentLime = "青柠色";
    public const string P2ContentGreen = "绿色";
    public const string P2ContentCyan = "青色";
    public const string P2ContentBlue = "蓝色";
    public const string P2ContentGeekblue = "极客蓝";
    public const string P2ContentPurple = "紫色";
    public const string P2TextCustom = "自定义";
    public const string P2TextWithoutIcon = "无图标";
    public const string P2ContentSuccess = "成功";
    public const string P2ContentProcessing = "处理中";
    public const string P2ContentError = "错误";
    public const string P2ContentWarning = "警告";
    public const string P2ContentDefault = "默认";
    public const string P2ContentTwitter = "Twitter";
    public const string P2ContentYoutube = "Youtube";
    public const string P2ContentFacebook = "Facebook";
    public const string P2ContentLinkedin = "Linkedin";
    public const string P2ContentTag1 = "标签1";
    public const string P2ContentTag2 = "标签2";
    public const string P2ContentTag3 = "标签3";
    public const string P2ContentTag4 = "标签4";

    public const string P2TextMaterialIcon = "Material 图标";

    protected override Type GetResourceKindType() => typeof(TagShowCaseLangResourceKind);
}
