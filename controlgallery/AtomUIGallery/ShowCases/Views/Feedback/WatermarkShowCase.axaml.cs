using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class WatermarkShowCase : ReactiveUserControl<WatermarkViewModel>
{
    public WatermarkShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}