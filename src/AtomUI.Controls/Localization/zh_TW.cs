using AtomUI.Theme.Language;

namespace AtomUI.Controls.Localization;

[LanguageProvider(LanguageCode.zh_TW, CommonLangId.Common)]
internal class zh_TW : LanguageProvider
{
    public zh_TW()
        : base(LanguageCode.zh_TW, CommonLangId.Common)
    {
    }

    public const string Ok = "確定";
    public const string Submit = "提交";
    public const string Cancel = "取消";
    public const string Reset = "重置";
    public const string Edit = "編輯";
    public const string Delete = "刪除";
    public const string Save = "保存";
    public const string NoData = "暫無數據";
    public const string Loading = "正在加載數據";
    public const string Optional = "(可選)";
    
    protected override Type GetResourceKindType() => typeof(CommonLangResourceKind);
}