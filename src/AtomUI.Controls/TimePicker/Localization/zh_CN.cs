using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Utils;

// ReSharper disable once CheckNamespace
namespace AtomUI.Controls.TimePickerLang;

// ReSharper disable once InconsistentNaming
[LanguageProvider(LanguageCode.zh_CN, TimePickerToken.ID)]
internal class zh_CN : AbstractLanguageProvider
{
    public const string AMText = "上午";
    public const string PMText = "下午";
    public const string Now = "现在";
}