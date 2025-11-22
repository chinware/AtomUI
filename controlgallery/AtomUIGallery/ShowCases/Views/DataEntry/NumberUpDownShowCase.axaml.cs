using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class NumberUpDownShowCase : ReactiveUserControl<NumberUpDownViewModel>
{
    public NumberUpDownShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}