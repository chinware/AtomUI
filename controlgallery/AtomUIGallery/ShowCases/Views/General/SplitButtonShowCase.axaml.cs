using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class SplitButtonShowCase : ReactiveUserControl<SplitButtonViewModel>
{
    public SplitButtonShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}