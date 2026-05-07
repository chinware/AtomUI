using AtomUI.Theme;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.Localization
{
    public enum ColorPickerLangResourceKind
    {
        EmptyColorText
    }

    public class ColorPickerLangResourceExtension : LanguageResourceExtension<ColorPickerLangResourceKind>
    {
        public ColorPickerLangResourceExtension()
        {
        }

        public ColorPickerLangResourceExtension(ColorPickerLangResourceKind kind) : base(kind)
        {
        }
    }
}