using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.LineEdit;

public partial class LineEditShowCase : ReactiveUserControl<LineEditViewModel>
{
    public const string LanguageId = nameof(LineEditShowCase);

    public LineEditShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
