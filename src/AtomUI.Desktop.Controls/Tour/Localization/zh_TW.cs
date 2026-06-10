using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.TourLang;

[LanguageProvider(LanguageCode.zh_TW, TourToken.ID)]
internal class zh_TW : LanguageProvider
{
    public zh_TW()
        : base(LanguageCode.zh_TW, TourToken.ID)
    {
    }

    public const string Previous = "上一步";
    public const string Next = "下一步";
    public const string Finish = "結束導覽";
    
    protected override Type GetResourceKindType() => typeof(TourLangResourceKind);
}