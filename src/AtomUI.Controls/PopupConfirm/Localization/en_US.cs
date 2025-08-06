using AtomUI.Theme;
using AtomUI.Theme.Language;
using AtomUI.Utils;

namespace AtomUI.Controls.PopupConfirmLang;

[LanguageProvider(LanguageCode.en_US, PopupConfirmToken.ID)]
internal class en_US : AbstractLanguageProvider
{
    public const string Ok = "Ok";
    public const string Cancel = "Cancel";
}