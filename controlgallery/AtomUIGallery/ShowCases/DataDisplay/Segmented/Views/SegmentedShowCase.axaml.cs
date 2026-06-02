using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Segmented;

public partial class SegmentedShowCase : ReactiveUserControl<SegmentedViewModel>
{
    public SegmentedShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
