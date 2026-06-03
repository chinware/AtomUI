using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Avatar;

[LanguageProvider(LanguageCode.zh_CN, AvatarShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "提供三种尺寸和两种形状。";
    public const string TypeTitle = "类型";
    public const string TypeDescription = "支持图片、图标和字母类型，后两种头像可自定义颜色和背景色。";
    public const string AutoSetFontSizeTitle = "自动设置字号";
    public const string AutoSetFontSizeDescription = "对于字母类型头像，当字母过长无法展示时，字号会根据头像宽度自动调整。也可以使用 gap 设置左右两侧的单位距离。";
    public const string AvatarGroupTitle = "头像组";
    public const string AvatarGroupDescription = "头像组展示。";
    public const string P2ContentU = "U";
    public const string P2ContentUser = "用户";
    public const string P2ContentChangeuser = "切换用户";
    public const string P2ContentChangegap = "切换间距";
    public const string P2ContentK = "K";
    public const string P2ContentA = "A";

    protected override Type GetResourceKindType() => typeof(AvatarShowCaseLangResourceKind);
}
