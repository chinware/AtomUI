using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class DropdownButtonShowCase : ReactiveUserControl<DropdownButtonViewModel>
{
    public DropdownButtonShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}