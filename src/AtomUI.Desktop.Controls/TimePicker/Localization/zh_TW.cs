using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.TimePickerLang;

[LanguageProvider(LanguageCode.zh_TW, TimePickerToken.ID)]
internal class zh_TW : LanguageProvider
{
    public zh_TW()
        : base(LanguageCode.zh_TW, TimePickerToken.ID)
    {
    }

    public const string AMText = "上午";
    public const string PMText = "下午";
    public const string Now = "現在";
    
    protected override Type GetResourceKindType() => typeof(TimePickerLangResourceKind);
}