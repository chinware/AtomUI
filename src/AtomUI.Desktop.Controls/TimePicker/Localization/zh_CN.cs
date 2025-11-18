using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.TimePickerLang;

[LanguageProvider(LanguageCode.zh_CN, TimePickerToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string AMText = "上午";
    public const string PMText = "下午";
    public const string Now = "现在";
}