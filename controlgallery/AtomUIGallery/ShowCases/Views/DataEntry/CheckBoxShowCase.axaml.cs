using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class CheckBoxShowCase : ReactiveUserControl<CheckBoxViewModel>
{
    public CheckBoxShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}