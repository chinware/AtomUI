using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.Localization
{
    public enum DataGridLangResourceKind
    {
        AscendTooltip,
        CancelConfirmText,
        CancelTooltip,
        DeleteConfirmText,
        DescendTooltip,
        Operating,
        SelectAllFilterItems
    }

    public class DataGridLangResourceExtension : LanguageResourceExtension<DataGridLangResourceKind>
    {
        public DataGridLangResourceExtension()
        {
        }

        public DataGridLangResourceExtension(DataGridLangResourceKind kind) : base(kind)
        {
        }
    }
}