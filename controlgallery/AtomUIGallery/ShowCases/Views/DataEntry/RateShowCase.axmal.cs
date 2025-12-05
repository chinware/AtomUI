using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class RateShowCase : ReactiveUserControl<RateViewModel>
{
    public RateShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}