using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Avatar;

[LanguageProvider(LanguageCode.zh_TW, AvatarShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "提供三種尺寸和兩種形狀。";
    public const string TypeTitle = "類型";
    public const string TypeDescription = "支持圖片、圖標和字母類型，後兩種頭像可自定義顏色和背景色。";
    public const string AutoSetFontSizeTitle = "自動設置字號";
    public const string AutoSetFontSizeDescription = "對於字母類型頭像，當字母過長無法展示時，字號會根據頭像寬度自動調整。也可以使用 gap 設置左右兩側的單位距離。";
    public const string AvatarGroupTitle = "頭像組";
    public const string AvatarGroupDescription = "頭像組展示。";
    public const string P2ContentU = "U";
    public const string P2ContentUser = "用戶";
    public const string P2ContentChangeuser = "切換用戶";
    public const string P2ContentChangegap = "切換間距";
    public const string P2ContentK = "K";
    public const string P2ContentA = "A";

    protected override Type GetResourceKindType() => typeof(AvatarShowCaseLangResourceKind);
}

