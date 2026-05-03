using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.Localization
{
    public enum QRCodeLangResourceKind
    {
        Expired,
        Refresh,
        Scanned
    }

    public class QRCodeLangResourceExtension : LanguageResourceExtension<QRCodeLangResourceKind>
    {
        public QRCodeLangResourceExtension()
        {
        }

        public QRCodeLangResourceExtension(QRCodeLangResourceKind kind) : base(kind)
        {
        }
    }
}