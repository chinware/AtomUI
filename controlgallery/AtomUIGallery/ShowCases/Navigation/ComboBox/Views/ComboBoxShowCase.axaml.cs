using Avalonia;
using Avalonia.Controls;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.ComboBox;

public partial class ComboBoxShowCase : GalleryReactiveUserControl<ComboBoxViewModel>
{
    public const string LanguageId = nameof(ComboBoxShowCase);

    public ComboBoxShowCase()
    {
        InitializeComponent();
    }

}
