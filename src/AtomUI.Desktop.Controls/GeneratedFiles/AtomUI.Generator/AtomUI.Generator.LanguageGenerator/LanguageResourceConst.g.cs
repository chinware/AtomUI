using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.Localization
{
    public enum PaginationLangResourceKind
    {
        JumpToText,
        PageText,
        TotalInfoFormat
    }

    public class PaginationLangResourceExtension : LanguageResourceExtension<PaginationLangResourceKind>
    {
        public PaginationLangResourceExtension()
        {
        }

        public PaginationLangResourceExtension(PaginationLangResourceKind kind) : base(kind)
        {
        }
    }
}

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