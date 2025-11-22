using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class ProgressBarShowCase : ReactiveUserControl<ProgressBarViewModel>
{
    public ProgressBarShowCase()
    {
        this.WhenActivated(disposables =>
        {
            
        });
        InitializeComponent();
    }
}