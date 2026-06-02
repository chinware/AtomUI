using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Spin;

public partial class SpinShowCase : ReactiveUserControl<SpinViewModel>
{
    public SpinShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
