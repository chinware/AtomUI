
using Avalonia;
using Avalonia.Controls;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.DropdownButton;

public partial class DropdownButtonShowCase : GalleryReactiveUserControl<DropdownButtonViewModel>
{
    public const string LanguageId = nameof(DropdownButtonShowCase);

    public DropdownButtonShowCase()
    {
        InitializeComponent();
    }

}
