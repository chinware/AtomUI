using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Segmented;

public partial class SegmentedShowCase : GalleryReactiveUserControl<SegmentedViewModel>
{
    public const string LanguageId = nameof(SegmentedShowCase);

    public SegmentedShowCase()
    {
        InitializeComponent();
    }
}
