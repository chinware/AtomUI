using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class QRCodeShowCase : ReactiveUserControl<QRCodeViewModel>
{
    public QRCodeShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}