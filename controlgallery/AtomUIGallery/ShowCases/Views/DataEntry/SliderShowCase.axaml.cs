using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class SliderShowCase : ReactiveUserControl<SliderViewModel>
{
    public SliderShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}