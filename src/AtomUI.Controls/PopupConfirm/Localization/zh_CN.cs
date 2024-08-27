using AtomUI.Theme;
using AtomUI.Utils;

namespace AtomUI.Controls.PopupConfirmLang;

[LanguageProvider(LanguageCode.zh_CN, PopupConfirmToken.ID)]
internal class zh_CN : AbstractLanguageProvider
{
   public const string OkText = "确定";
   public const string CancelText = "取消";
}