using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.SplitButton;

[LanguageProvider(LanguageCode.zh_TW, SplitButtonShowCase.LanguageId)]
internal class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "最基礎的 SplitButton。";
    public const string SizeTitle = "尺寸";
    public const string SizeDescription = "AtomUI 支持小號、默認和大號三種按鈕尺寸。需要大號或小號按鈕時，可將 size 屬性分別設置為 large 或 small；省略 size 屬性時使用默認尺寸。";
    public const string DangerButtonsTitle = "危險按鈕";
    public const string DangerButtonsDescription = "danger 是 antd 4.0 之後的按鈕屬性。";
    public const string CustomIconTitle = "自定義圖標";
    public const string CustomIconDescription = "自定義浮出按鈕圖標。";
    public const string FlyoutTriggerTypeTitle = "浮出觸發類型";
    public const string FlyoutTriggerTypeDescription = "支持兩種觸發類型。";
    public const string P2HeaderCut = "剪切";
    public const string P2HeaderCopy = "複製";
    public const string P2HeaderDelete = "刪除";
    public const string P2ContentHoverMe = "懸停我";
    public const string P2ContentLarge = "大號";
    public const string P2ContentMiddle = "中號";
    public const string P2ContentSmall = "小號";
    public const string P2ContentDefault = "默認";
    public const string P2ContentPrimary = "主要";
    public const string P2ContentClickMe = "點我";

    protected override Type GetResourceKindType() => typeof(SplitButtonShowCaseLangResourceKind);
}

