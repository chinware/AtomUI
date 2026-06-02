using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.DropdownButton;

public partial class DropdownButtonShowCase : ReactiveUserControl<DropdownButtonViewModel>
{
    public DropdownButtonShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}