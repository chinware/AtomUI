using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Alert;

public partial class AlertShowCase : ReactiveUserControl<AlertViewModel>
{
    public AlertShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
