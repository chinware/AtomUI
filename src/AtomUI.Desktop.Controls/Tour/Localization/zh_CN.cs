using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.TourLang;

[LanguageProvider(LanguageCode.zh_CN, TourToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Previous = "上一步";
    public const string Next = "下一步";
    public const string Finish = "结束导览";
    
    protected override Type GetResourceKindType() => typeof(TourLangResourceKind);
}