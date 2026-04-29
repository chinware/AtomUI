using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Controls.Localization
{
    public enum CommonLangResourceKind
    {
        Cancel,
        Delete,
        Edit,
        Loading,
        NoData,
        Ok,
        Optional,
        Reset,
        Save,
        Submit
    }

    public class CommonLangResourceExtension : LanguageResourceExtension<CommonLangResourceKind>
    {
        public CommonLangResourceExtension()
        {
        }

        public CommonLangResourceExtension(CommonLangResourceKind kind) : base(kind)
        {
        }
    }
}