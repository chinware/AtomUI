using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class AlertShowCase : ReactiveUserControl<AlertViewModel>
{
    public AlertShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}