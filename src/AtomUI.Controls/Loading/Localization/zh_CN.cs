using AtomUI.Theme;
using AtomUI.Utils;

namespace AtomUI.Controls.LoadingLang;

[LanguageProvider(LanguageCode.zh_CN, LoadingIndicatorToken.ID)]
internal class zh_CN : AbstractLanguageProvider
{
   public string LoadingText = "加载中";
}