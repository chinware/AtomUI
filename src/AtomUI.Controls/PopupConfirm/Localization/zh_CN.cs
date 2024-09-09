using AtomUI.Theme;
using AtomUI.Utils;

// ReSharper disable once CheckNamespace
namespace AtomUI.Controls.PopupConfirmLang;

// ReSharper disable once InconsistentNaming
[LanguageProvider(LanguageCode.zh_CN, PopupConfirmToken.ID)]
internal class zh_CN : AbstractLanguageProvider
{
   public const string OkText = "确定";
   public const string CancelText = "取消";
}