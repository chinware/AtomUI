using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class SegmentedShowCase : ReactiveUserControl<SegmentedViewModel>
{
    public SegmentedShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}