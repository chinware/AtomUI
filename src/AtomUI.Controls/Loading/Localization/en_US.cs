using AtomUI.Theme;
using AtomUI.Utils;

namespace AtomUI.Controls.Lang;

[LanguageProvider(LanguageCode.en_US, LoadingIndicatorToken.ID)]
internal class en_US : AbstractLanguageProvider
{
   public string LoadingText = "Loading";
}