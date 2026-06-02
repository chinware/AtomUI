using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.LineEdit;

public partial class LineEditShowCase : ReactiveUserControl<LineEditViewModel>
{
    public LineEditShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
