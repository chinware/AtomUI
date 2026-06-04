using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.DialogLang;

[LanguageProvider(LanguageCode.zh_TW, DialogToken.ID)]
internal class zh_TW : LanguageProvider
{
    public const string Ok = "確定";
    public const string Open = "打開";
    public const string Save = "保存";
    public const string Cancel = "取消";
    public const string Close = "關閉";
    public const string Discard = "丟棄";
    public const string Apply = "應用";
    public const string Reset = "重置";
    public const string Reload = "重新加載";
    public const string RestoreDefaults = "恢復默認值";
    public const string Help = "幫助";
    public const string SaveAll = "全部保存";
    public const string Yes = "是";
    public const string YesToAll = "全部是";
    public const string No = "否";
    public const string NoToAll = "全部否";
    public const string Abort = "中止";
    public const string Retry = "重試";
    public const string Ignore = "忽略";
    
    protected override Type GetResourceKindType() => typeof(DialogLangResourceKind);
}

