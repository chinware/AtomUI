
using Avalonia;
using Avalonia.Controls;
using AtomUI.Desktop.Controls;

namespace AtomUIGallery.ShowCases.GroupBox;

public partial class GroupBoxShowCase : GalleryReactiveUserControl<GroupBoxViewModel>
{
    public const string LanguageId = nameof(GroupBoxShowCase);

    public GroupBoxShowCase()
    {
        InitializeComponent();
    }

}
