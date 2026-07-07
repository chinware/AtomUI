
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Segmented;

public partial class SegmentedShowCase : GalleryReactiveUserControl<SegmentedViewModel>
{
    public const string LanguageId = nameof(SegmentedShowCase);

    public SegmentedShowCase()
    {
        InitializeComponent();
    }

}
