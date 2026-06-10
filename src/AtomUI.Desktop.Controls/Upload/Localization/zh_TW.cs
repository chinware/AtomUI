using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.UploadLang;

[LanguageProvider(LanguageCode.zh_TW, UploadToken.ID)]
internal class zh_TW : LanguageProvider
{
    public zh_TW()
        : base(LanguageCode.zh_TW, UploadToken.ID)
    {
    }

    public const string Uploading = "上傳中...";
    public const string Pending = "等待調度...";
    public const string DragUploadHead = "點擊或拖動文件到此區域進行上傳";
    
    protected override Type GetResourceKindType() => typeof(UploadLangResourceKind);
}