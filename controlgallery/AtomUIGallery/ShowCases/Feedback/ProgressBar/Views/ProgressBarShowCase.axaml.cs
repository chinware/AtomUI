using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.ProgressBar;

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
