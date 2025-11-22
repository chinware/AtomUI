using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class SeparatorShowCase : ReactiveUserControl<SeparatorViewModel>
{
    public SeparatorShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}